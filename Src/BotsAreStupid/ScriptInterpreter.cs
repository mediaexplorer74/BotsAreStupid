// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScriptInterpreter
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

#nullable enable
namespace BotsAreStupid
{
  internal class ScriptInterpreter : ICloneable
  {
    private 
    #nullable disable
    List<string> instructions;
    private string[] menuInstructions;
    private int currentInstruction = -1;
    private int furthestInstruction = -1;
    private float waitTime;
    private float waitTimeStart;
    private int waitFrames;
    private int waitFramesStart;
    private string[] waitCondition;
    private bool waitForMainPlayer;
    private Rectangle? mainPlayerRectCondition;
    private int orbCountCondition = -1;
    private float waveTime;
    private Player player;
    private TextObject sayTextObj;
    private List<TextObject> textObjs = new List<TextObject>();
    private static int currentMenuInstruction = -1;
    private static int randomBotCount = 0;
    private static int randomSaveCount = 0;
    private Dictionary<string, string[]> functions = new Dictionary<string, string[]>();
    private bool inFunctionDefinition;
    private string currentFunctionName;
    private List<string> currentFunctionLines = new List<string>();
    private Dictionary<string, string> variables;
    private bool inBlockComment;

    public Simulation CurrentSimulation { get; private set; }

    public bool IsWalkingLeft { get; private set; }

    public bool IsWalkingRight { get; private set; }

    public bool IsWaving => (double) this.waveTime > 0.0;

    public bool IsSaying
    {
      get => this.sayTextObj != null && this.sayTextObj.IsActive && !this.sayTextObj.ShouldBeDone;
    }

    public bool IsInstructing { get; private set; }

    public bool HasInstructed { get; private set; }

    public bool IsRandom { get; private set; }

    public static int SavedCount { get; private set; }

    public float TimeSinceLastInstruction { get; private set; } = 0.0f;

    public int FurthestInstruction => this.furthestInstruction;

    public int LineCount
    {
      get
      {
        return this.instructions != null ? this.instructions.Count : TextEditor.Instance.ContentEditor.LineCount;
      }
    }

    public int InstructionCount
    {
      get
      {
        int lineCount;
        ScriptParser.CheckScript(this.InstructionArray, this.CurrentSimulation, out ScriptError[] _, out lineCount);
        return lineCount;
      }
    }

    private string[] InstructionArray
    {
      get
      {
        return this.instructions != null ? this.instructions.ToArray() : TextEditor.Instance?.ContentEditor?.GetContent() ?? (string[]) null;
      }
    }

    public ScriptInterpreter(Player player)
    {
      this.SetPlayer(player);
      this.Reset();
      if (!StateManager.IsState(GameState.MainMenu) || this.CurrentSimulation.CheckMode)
        return;
      this.GetMenuInstructions();
    }

    public void SetPlayer(Player player)
    {
      this.player = player;
      this.CurrentSimulation = player.CurrentSimulation;
    }

    public object Clone()
    {
      object obj = this.MemberwiseClone();
      ScriptInterpreter scriptInterpreter = (ScriptInterpreter) obj;
      scriptInterpreter.variables = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> variable in this.variables)
        scriptInterpreter.variables.Add(variable.Key, variable.Value);
      return obj;
    }

    public void Update(float deltaTime)
    {
      if (!this.player.IsActive)
        return;
      if (this.CurrentSimulation.IsMain && this.IsInstructing && StateManager.IsState(GameState.InLevel_AnyWithText))
      {
        TextEditor instance = TextEditor.Instance;
        int num = instance != null ? instance.SkipToLine : -1;
        if (num > this.furthestInstruction && this.LineCount > num && !VarManager.IsOverclocked)
          VarManager.SetInt("overclock", VarManager.GetInt("skipspeed"));
      }
      if (this.IsInstructing || this.HasInstructed)
        this.TimeSinceLastInstruction += deltaTime;
      if ((double) this.waveTime > 0.0)
        this.waveTime -= deltaTime;
      if ((double) this.waitTime > 0.0)
      {
        this.waitTime -= deltaTime;
        if ((double) this.waitTime <= 0.0)
        {
          this.waitTime = 0.0f;
          this.NextInstruction();
        }
      }
      if (this.waitFrames > 0)
      {
        --this.waitFrames;
        if (this.waitFrames <= 0)
        {
          this.waitFrames = 0;
          this.NextInstruction();
        }
      }
      if (this.waitCondition != null && (this.waitForMainPlayer ? Utility.MainPlayer : this.CurrentSimulation.Player) != null && ScriptParser.ParseBool(this.waitForMainPlayer ? Utility.MainPlayer : this.CurrentSimulation.Player, this.waitCondition))
      {
        this.waitCondition = (string[]) null;
        this.waitForMainPlayer = false;
        this.NextInstruction();
      }
      if (this.mainPlayerRectCondition.HasValue)
      {
        Rectangle rectangle = this.mainPlayerRectCondition.Value;
        Player player = SimulationManager.MainSimulation.Player;
        if (player != null && rectangle.Intersects(player.Rectangle))
        {
          this.mainPlayerRectCondition = new Rectangle?();
          this.NextInstruction();
        }
      }
      if (this.orbCountCondition >= 0 && SimulationManager.MainSimulation.CountType(typeof (EnergyOrb)) <= this.orbCountCondition)
      {
        this.orbCountCondition = -1;
        this.NextInstruction();
      }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
    }

    public void StartInstructions(string[] array, bool autosave = true, int tickrate = -1)
    {
      this.StartInstructions(new List<string>((IEnumerable<string>) array), autosave, tickrate);
    }

    public void StartInstructions(List<string> instructions = null, bool autosave = true, int tickrate = -1)
    {
      if (this.IsInstructing || this.HasInstructed)
        return;
      this.CurrentSimulation.SetTickrate(tickrate > 0 ? tickrate : VarManager.GetInt("defaulttickrate"));
      this.instructions = instructions;
      this.Reset(true);
      if (autosave && StateManager.IsState(GameState.InLevel_Default))
        TextEditor.Instance?.SaveInstructions();
      this.IsInstructing = true;
      this.NextInstruction();
    }

    public void Reset(bool starting = false)
    {
      this.currentInstruction = -1;
      this.furthestInstruction = -1;
      this.IsWalkingLeft = false;
      this.IsWalkingRight = false;
      this.IsInstructing = false;
      this.HasInstructed = false;
      this.variables = new Dictionary<string, string>();
      if (!this.CurrentSimulation.IsMain)
        return;
      if (VarManager.GetInt("overclock") == VarManager.GetInt("skipspeed"))
        VarManager.SetInt("overclock", 1);
      TextEditor.Instance?.SetInstructionLine(-1, loadSaved: !starting);
    }

    public void StartMenuInstructions()
    {
      if (!VarManager.HasInt("nextbot") && Utility.GetBool((float) VarManager.GetInt("randombotspercent") / 100f))
      {
        this.StartRandomInstructions();
      }
      else
      {
        int length = this.menuInstructions.Length;
        if (length > 0)
        {
          if (VarManager.HasInt("nextbot"))
          {
            int num = VarManager.GetInt("nextbot");
            VarManager.DeleteInt("nextbot");
            ScriptInterpreter.currentMenuInstruction = num;
          }
          else if (ScriptInterpreter.currentMenuInstruction == -1)
            ScriptInterpreter.currentMenuInstruction = Utility.GetRandom(length);
          else if (!VarManager.GetBool("repeatbots"))
            ++ScriptInterpreter.currentMenuInstruction;
          ScriptInterpreter.currentMenuInstruction %= length;
          this.StartInstructions(
              this.menuInstructions[ScriptInterpreter.currentMenuInstruction].Split('\n'), false, 128);
          MainMenu.Instance?.SetStatusInfo("Loaded");
          MainMenu.Instance?.SetBotId(ScriptInterpreter.currentMenuInstruction);
        }
        else
          this.StartRandomInstructions();
      }
    }

    public async void SaveRandomInstructions()
    {
      string script = "";
      for (int i = 0; i < this.currentInstruction; ++i)
        script = script + this.GetInstruction(i) + "\n";
      script += "start right\n";
      string str = await HttpManager.Post(VarManager.GetString("baseurl") + "MenuScripts/Upload.php", new Dictionary<string, string>()
      {
        {
          "script",
          script
        },
        {
          "uuid",
          VarManager.GetString("uuid")
        }
      });
      ++ScriptInterpreter.randomSaveCount;
      MainMenu instance = MainMenu.Instance;
      if (instance == null)
      {
        script = (string) null;
      }
      else
      {
        instance.SetSavedCount(ScriptInterpreter.randomSaveCount);
        script = (string) null;
      }
    }

    public static string[] GetRandomInstructions()
    {
      int random = Utility.GetRandom(30, 400);
      float maxAmount = Utility.RandomizeNumber(0.7f, 0.69f);
      string[] randomInstructions = new string[random + 3];
      bool flag = true;
      for (int index = 0; index < random; ++index)
      {
        string str = "";
        if (flag)
        {
          switch (Utility.GetRandom(6))
          {
            case 0:
              str = "move right";
              break;
            case 1:
              str = "move left";
              break;
            case 2:
              str = "stop";
              break;
            case 3:
              str = "jump";
              break;
            case 4:
              str = "hook right";
              break;
            case 5:
              str = "hook left";
              break;
            case 6:
              str = "unhook";
              break;
          }
        }
        else
          str = "wait " + Math.Round((double) Utility.RandomizeNumber(0.01f + maxAmount, maxAmount), 3).ToString().Replace(",", ".");
        flag = !flag;
        randomInstructions[index] = str;
      }
      randomInstructions[random] = "unhook";
      randomInstructions[random + 1] = "start right";
      randomInstructions[random + 2] = "jump";
      return randomInstructions;
    }

    public void StartRandomInstructions()
    {
      this.IsRandom = true;
      this.StartInstructions(ScriptInterpreter.GetRandomInstructions(), false, 128);
      MainMenu.Instance?.SetStatusInfo("Random");
      MainMenu.Instance?.SetBotId(ScriptInterpreter.randomBotCount);
      ++ScriptInterpreter.randomBotCount;
    }

    private void NextInstruction()
    {
      if (!this.player.IsActive)
        return;
      ++this.currentInstruction;
      if (this.currentInstruction >= this.LineCount)
      {
        if (VarManager.GetBool("manualControls"))
          return;
        if (this.CurrentSimulation.IsMain)
          TextEditor.Instance?.SetInstructionLine(9999);
        this.IsInstructing = false;
        this.HasInstructed = true;
      }
      else
      {
        if (this.currentInstruction > this.furthestInstruction)
          this.furthestInstruction = this.currentInstruction;
        string instruction = this.GetInstruction(this.currentInstruction);
        if (this.inBlockComment)
        {
          if (ScriptParser.IsBlockCommentEnd(instruction))
            this.inBlockComment = false;
          this.NextInstruction();
        }
        else if (ScriptParser.IsBlockCommentStart(instruction))
        {
          if (!ScriptParser.IsBlockCommentEnd(instruction))
            this.inBlockComment = true;
          this.NextInstruction();
        }
        else if (!ScriptParser.IsExecutableLine(instruction, out ScriptError _, this.currentInstruction, this.CurrentSimulation, this.InstructionArray, this.CurrentSimulation.IsIntro))
        {
          this.NextInstruction();
        }
        else
        {
          string lower1 = instruction.Trim().ToLower();
          this.TimeSinceLastInstruction = 0.0f;
          if (this.CurrentSimulation.IsMain)
          {
            int num;
            if (VarManager.IsOverclocked)
            {
              int currentInstruction = this.currentInstruction;
              int? skipToLine = TextEditor.Instance?.SkipToLine;
              int valueOrDefault = skipToLine.GetValueOrDefault();
              num = currentInstruction >= valueOrDefault & skipToLine.HasValue ? 1 : 0;
            }
            else
              num = 0;
            if (num != 0 && VarManager.GetInt("overclock") == VarManager.GetInt("skipspeed"))
            {
              VarManager.SetInt("overclock", 1);
              if (VarManager.GetBool("pauseonstart"))
                TextEditor.Instance?.OnPauseButtonPress();
            }
            if (StateManager.IsState(GameState.InLevel_AnyWithText))
              TextEditor.Instance?.SetInstructionLine(this.currentInstruction, this.furthestInstruction);
          }
          lower1.Replace(',', '.');
          if (this.inFunctionDefinition)
          {
            if (lower1.StartsWith("functionend"))
            {
              this.inFunctionDefinition = false;
              //this.functions.TryAdd(this.currentFunctionName, this.currentFunctionLines.ToArray());
            }
            else
              this.currentFunctionLines.Add(lower1);
            this.NextInstruction();
          }
          else
          {
            string[] args1 = ScriptParser.GetArgs(lower1);
            try
            {
              int result1;
              for (int index1 = 0; index1 < args1.Length; ++index1)
              {
                string str1 = args1[index1];
                if (str1.StartsWith("ran") && str1.Contains("-"))
                {
                  bool flag = false;
                  if (str1[3] == 'f')
                  {
                    str1 = str1.Substring(1);
                    flag = true;
                  }
                  string[] strArray1 = str1.Substring(3).Split('-');
                  if (flag)
                  {
                    args1[index1] = Utility.GetRandom(float.Parse(strArray1[0], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray1[1], (IFormatProvider) CultureInfo.InvariantCulture)).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                  }
                  else
                  {
                    string[] strArray2 = args1;
                    int index2 = index1;
                    result1 = Utility.GetRandom(int.Parse(strArray1[0]), int.Parse(strArray1[1]));
                    string str2 = result1.ToString();
                    strArray2[index2] = str2;
                  }
                }
              }
              switch (args1[0])
              {
                case "-cleartext":
                  while (this.textObjs.Count > 0)
                    this.textObjs[0].Destroy();
                  this.NextInstruction();
                  break;
                case "-destroylasttext":
                  if (this.textObjs.Count > 0)
                    this.textObjs[this.textObjs.Count - 1].Destroy();
                  this.NextInstruction();
                  break;
                case "-emote":
                  if (args1.Length == 2 && args1[1] == "wave")
                    this.waveTime = 1f;
                  this.NextInstruction();
                  break;
                case "-introwait":
                  int result2;
                  int result3;
                  int result4;
                  int result5;
                  if (args1.Length == 6 && args1[1] == "maininrect" && int.TryParse(args1[2], out result2) && int.TryParse(args1[3], out result3) && int.TryParse(args1[4], out result4) && int.TryParse(args1[5], out result5))
                  {
                    this.mainPlayerRectCondition = new Rectangle?(new Rectangle(result2, result3, result4, result5));
                    break;
                  }
                  int result6;
                  if (args1.Length == 3 && args1[1] == "maxorbs" && int.TryParse(args1[2], out result6))
                  {
                    this.orbCountCondition = result6;
                    break;
                  }
                  if (args1.Length >= 3 && args1[1] == "main")
                  {
                    this.waitForMainPlayer = true;
                    this.ParseWaitCondition(args1, 2);
                    break;
                  }
                  this.NextInstruction();
                  break;
                case "-stopsay":
                  this.sayTextObj?.Destroy();
                  this.sayTextObj = (TextObject) null;
                  this.NextInstruction();
                  break;
                case "-teleport":
                  int result7;
                  int result8;
                  if (args1.Length == 3 && int.TryParse(args1[1], out result7) && int.TryParse(args1[2], out result8))
                    this.player.SetPosition((float) result7, (float) result8);
                  this.NextInstruction();
                  break;
                case "-tickrate":
                  if (args1.Length == 2)
                  {
                    int result9;
                    if (int.TryParse(args1[1], out result9))
                    {
                      this.CurrentSimulation.SetTickrate(result9);
                    }
                    else
                    {
                      switch (args1[1])
                      {
                        case "low":
                          this.CurrentSimulation.SetTickrate(50);
                          break;
                        case "legacy":
                          this.CurrentSimulation.SetTickrate(128);
                          break;
                        case "high":
                          this.CurrentSimulation.SetTickrate(1000);
                          break;
                      }
                    }
                  }
                  this.NextInstruction();
                  break;
                case "function":
                  if (args1.Length == 2 && !string.IsNullOrEmpty(args1[1]))
                  {
                    this.currentFunctionName = args1[1];
                    this.inFunctionDefinition = true;
                    this.currentFunctionLines.Clear();
                  }
                  this.NextInstruction();
                  break;
                case "hook":
                  if (args1.Length <= 1)
                    this.player.Hook();
                  else if (args1.Length == 2)
                    this.player.Hook(new bool?(args1[1] == "right"));
                  else if (args1.Length == 3)
                    this.player.Hook(new bool?(args1[2] == "right"), args1[1] == "down");
                  this.NextInstruction();
                  break;
                case "jump":
                  this.player.Jump();
                  this.NextInstruction();
                  break;
                case "move":
                case "start":
                  if (args1[1] == "right")
                  {
                    this.IsWalkingLeft = false;
                    this.IsWalkingRight = true;
                  }
                  else if (args1[1] == "left")
                  {
                    this.IsWalkingRight = false;
                    this.IsWalkingLeft = true;
                  }
                  this.NextInstruction();
                  break;
                case "repeat":
                  if (int.TryParse(args1[1], out result1))
                    this.AddToVariable("repeats_" + this.currentInstruction.ToString(), 1f);
                  this.NextInstruction();
                  break;
                case "repeatend":
                  int num1 = 1;
                  bool flag1 = false;
                  for (int index3 = this.currentInstruction - 1; index3 >= 0; --index3)
                  {
                    string lower2 = this.GetInstruction(index3).ToLower();
                    if (flag1)
                    {
                      if (ScriptParser.IsBlockCommentStart(lower2))
                        flag1 = false;
                    }
                    else if (ScriptParser.IsBlockCommentEnd(lower2))
                    {
                      if (!ScriptParser.IsBlockCommentStart(lower2))
                        flag1 = true;
                    }
                    else
                    {
                      string[] args2 = ScriptParser.GetArgs(lower2);
                      if (args2.Length != 0)
                      {
                        if (args2[0] == "repeatend")
                          ++num1;
                        else if (args2[0] == "repeat" && ScriptParser.IsExecutableLine(lower2, out ScriptError _, index3, this.CurrentSimulation, this.InstructionArray, this.CurrentSimulation.IsIntro))
                        {
                          --num1;
                          string s = args2[1];
                          string key = "repeats_" + index3.ToString();
                          if (num1 == 0)
                          {
                            int result10;
                            int result11;
                            if (int.TryParse(s, out result10) && int.TryParse(this.GetVariable(key), out result11) && result11 < result10)
                            {
                              this.currentInstruction = index3 - 1;
                              break;
                            }
                            if (s == "while" || s == "until")
                            {
                              List<string> stringList = new List<string>();
                              for (int index4 = 2; index4 < args2.Length; ++index4)
                                stringList.Add(args2[index4]);
                              if (ScriptParser.ParseBool(this.CurrentSimulation.Player, stringList.ToArray()) == (s == "while"))
                                this.currentInstruction = index3 - 1;
                              break;
                            }
                            break;
                          }
                          if (int.TryParse(s, out result1))
                            this.RemoveVariable(key);
                        }
                      }
                    }
                  }
                  this.NextInstruction();
                  break;
                case "say":
                  this.SpawnText(args1, true);
                  this.NextInstruction();
                  break;
                case "stop":
                  if (args1.Length > 1 && args1[1] == "hook")
                  {
                    this.player.Unhook();
                  }
                  else
                  {
                    this.IsWalkingRight = false;
                    this.IsWalkingLeft = false;
                  }
                  this.NextInstruction();
                  break;
                case "text":
                  this.SpawnText(args1);
                  this.NextInstruction();
                  break;
                case "unhook":
                  this.player.Unhook();
                  this.NextInstruction();
                  break;
                case "wait":
                  if (args1[1].EndsWith("f"))
                  {
                    int result12 = 0;
                    int.TryParse(args1[1].Substring(0, args1[1].Length - 1), out result12);
                    if (args1[1].Length == 1)
                      result12 = 1;
                    if (result12 <= 0)
                    {
                      this.NextInstruction();
                      break;
                    }
                    this.waitFrames = result12;
                    this.waitFramesStart = result12;
                    break;
                  }
                  float result13;
                  if (float.TryParse(args1[1], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result13))
                  {
                    if ((double) result13 <= 0.0)
                    {
                      this.NextInstruction();
                      break;
                    }
                    this.waitTime = result13;
                    this.waitTimeStart = result13;
                    break;
                  }
                  this.ParseWaitCondition(args1, 1);
                  break;
                default:
                  int length = lower1.IndexOf('(');
                  int num2 = lower1.LastIndexOf(')');
                  if (length > 0 && num2 > length)
                  {
                    string key = lower1.Substring(0, length);
                    int num3 = num2 - length;
                    string[] strArray = num3 > 1 ? lower1.Substring(length + 1, num3 - 1).Split(' ') : (string[]) null;
                    if (!this.functions.TryGetValue(key, out string[] _))
                      ;
                  }
                  this.NextInstruction();
                  break;
              }
            }
            catch (Exception ex)
            {
              Utility.LogError(ex);
              Console.WriteLine("Error in command execution: " + ex.Message);
              this.NextInstruction();
            }
          }
        }
      }
    }

    public float? GetWaitPercent()
    {
      if ((double) this.waitTime > 0.0)
        return new float?(this.waitTime / this.waitTimeStart);
      return this.waitFrames <= 0 ? new float?() : new float?((float) this.waitFrames / (float) this.waitFramesStart);
    }

    private void GetMenuInstructions()
    {
      string str1 = "";
      string str2 = VarManager.GetString("contentdirectory") + "Instructions/MainMenu.txt";
      if (File.Exists(str2))
        str1 = File.ReadAllText(str2);
      this.menuInstructions = string.IsNullOrEmpty(str1) ? new string[0] : str1.Split('*');
      ScriptInterpreter.SavedCount = this.menuInstructions.Length;
    }

    private void ParseWaitCondition(string[] args, int start)
    {
      if (args[start] == "until")
      {
        List<string> stringList = new List<string>();
        for (int index = start + 1; index < args.Length; ++index)
          stringList.Add(args[index]);
        this.waitCondition = stringList.ToArray();
      }
      else
      {
        if (!(args[start] == "while"))
          return;
        List<string> stringList = new List<string>();
        stringList.Add("not");
        for (int index = start + 1; index < args.Length; ++index)
          stringList.Add(args[index]);
        this.waitCondition = stringList.ToArray();
      }
    }

    private string GetInstruction(int idx)
    {
      return this.instructions == null ? TextEditor.Instance.ContentEditor.GetLineString(idx) : this.instructions[idx];
    }

    private void SpawnText(string[] args, bool isSay = false)
    {
      if (this.CurrentSimulation.CheckMode)
        return;
      string str = "";
      for (int index = isSay ? 2 : 4; index < args.Length; ++index)
        str = str + args[index] + " ";
      float num1 = float.Parse(args[isSay ? 1 : 3], (IFormatProvider) CultureInfo.InvariantCulture);
      int result1;
      int x = isSay ? 0 : (int.TryParse(args[1], out result1) ? result1 : 0);
      int result2;
      int y = isSay ? 0 : (int.TryParse(args[2], out result2) ? result2 : 0);
      Styling styling = new Styling()
      {
        text = str,
        textRevealTime = num1 / 2f,
        textLifeTime = num1
      };
      float num2 = 0.0f;
      bool isIntro = this.CurrentSimulation.IsIntro;
      if (isIntro || this.CurrentSimulation.IsMain && VarManager.GetBool("godmode"))
      {
        //styling.font = TextureManager.GetFont("megaMan2");
        if (isSay)
          num2 = 10f;
      }
      if (isIntro)
        styling.textLifeTime = float.MaxValue;
      TextObject obj = new TextObject(this.CurrentSimulation, (float) x, (float) y, new Styling?(styling));
      if (isSay)
      {
        this.sayTextObj?.Destroy();
        this.sayTextObj = obj;
        obj.SetPosition(this.CurrentSimulation.Player.Center - new Vector2((float) (obj.Width / 2), 40f + num2));
        obj.AttachTo((GameObject) this.CurrentSimulation.Player);
      }
      else
      {
        this.textObjs.Add(obj);
        obj.OnDestroy += (System.Action) (() => this.textObjs.Remove(obj));
      }
    }

    private void SetVariable(string key, string value)
    {
      if (this.variables == null)
        return;
      if (this.variables.ContainsKey(key))
        this.variables[key] = value;
      else
        this.variables.Add(key, value);
    }

    private void RemoveVariable(string key) => this.variables?.Remove(key);

    private string GetVariable(string key)
    {
      if (this.variables == null)
        return (string) null;
      string variable;
      this.variables.TryGetValue(key, out variable);
      return variable;
    }

    private void AddToVariable(string key, float amount)
    {
      float result;
      float num = (float.TryParse(this.GetVariable(key), out result) ? result : 0.0f) + amount;
      this.SetVariable(key, num.ToString());
    }
  }
}

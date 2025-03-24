// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.LevelManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Globalization;
using System.IO;
using System.Xml;


#nullable disable
namespace BotsAreStupid
{
  internal class LevelManager
  {
    public static LevelManager Instance = new LevelManager();
    public static readonly string[] DefaultLevels = new string[18]
    {
      "Basic1",
      "Basic2",
      "Basic3",
      "Basic4",
      "Basic5",
      "Easy1",
      "Easy2",
      "Easy3",
      "Easy4",
      "Medium1",
      "Medium2",
      "Medium3",
      "Medium4",
      "Hard1",
      "Hard2",
      "Hard3",
      "Extreme1",
      "Extreme2"
    };
    private XmlWriterSettings settings;

    public static string CurrentLevelName { private set; get; }

    public static string CurrentLevelID { private set; get; }

    public static string CurrentLevelPath
    {
      get
      {
        return VarManager.GetString(LevelManager.CurrentLevelIsCustom ? "customlevelsdirectory" : "defaultlevelsdirectory") + LevelManager.CurrentLevelNameSimple + ".xml";
      }
    }

    public static string CurrentLevelNameSimple
    {
      get => Utility.SimpleLevelName(LevelManager.CurrentLevelName);
    }

    public static string[] CurrentIntroInstructions { get; private set; }

    public static bool CurrentLevelIsCustom { get; private set; }

    public LevelManager()
    {
      this.settings = new XmlWriterSettings();
      this.settings.Indent = true;
      this.settings.IndentChars = "\t";
      this.settings.NewLineChars = "\n";
    }

    public static bool Load(string name = null, Simulation intoSimulation = null)
    {
      return LevelManager.Instance.LoadLevel(name, intoSimulation);
    }

    public static void ReloadCurrent() => LevelManager.Load(LevelManager.CurrentLevelName);

    public static void Clear()
    {
      SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation => simulation.Clear()));
      SimulationManager.ClearSimulations();
      SimulationManager.SetPaused(false);
    }

    public static void SetCurrentLevelName(string name)
    {
      LevelManager.CurrentLevelName = name;
      LevelManager.CurrentLevelID = (string) null;
      LevelEditor.Instance?.SetLevelNameDisplay(name);
    }

    public static void SaveCurrent() => LevelManager.Instance.SaveLevel();

    public static void SaveAs(string name)
    {
      LevelManager.SetCurrentLevelName(name);
      LevelManager.SaveCurrent();
    }

    public static bool Exists(string levelName)
    {
      levelName = Utility.SimpleLevelName(levelName) + ".xml";
      return File.Exists(VarManager.GetString("defaultlevelsdirectory") + levelName) || File.Exists(VarManager.GetString("customlevelsdirectory") + levelName);
    }

    public static void LoadNext()
    {
      string name = "";
      for (int index = 0; index < LevelManager.DefaultLevels.Length - 1; ++index)
      {
        if (LevelManager.CurrentLevelName == LevelManager.DefaultLevels[index])
          name = LevelManager.DefaultLevels[index + 1];
      }
      LevelManager.Load(name);
      PopupMenu.Instance?.SetActive(false);
      TextEditor.Instance?.UpdateAvailableCommands();
    }

    public static int GetLevelIdx(string name)
    {
      for (int levelIdx = 0; levelIdx < LevelManager.DefaultLevels.Length; ++levelIdx)
      {
        if (LevelManager.DefaultLevels[levelIdx] == name)
          return levelIdx;
      }
      return -1;
    }

    public static void UpdateCustomCreatorNames(string name)
    {
      foreach (string file in Directory.GetFiles(VarManager.GetString("customlevelsdirectory")))
      {
        if (Utility.GetLevelCreatorUUID(file) == VarManager.GetString("uuid"))
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load(file);
          XmlNode xmlNode = ((XmlNode) xmlDocument).SelectSingleNode("/level/meta/creator/name");
          if (xmlNode != null)
          {
            xmlNode.InnerText = name;
            xmlDocument.Save(file);
          }
        }
      }
    }

    public static void PlayIntro()
    {
      if (LevelManager.CurrentIntroInstructions == null || LevelManager.CurrentIntroInstructions.Length == 0)
      {
        PopupMenu.Instance.Inform("Intro not available!", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive));
      }
      else
      {
        SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation =>
        {
          if (!simulation.IsIntro)
            return;
          SimulationManager.RemoveSimulation(simulation);
        }));
        SimulationManager.CreateSimulation(out bool _, "Intro", LevelManager.CurrentIntroInstructions, "IntroBot3000");
      }
    }

    private bool LoadLevel(string name = null, Simulation intoSimulation = null)
    {
      LevelManager.Clear();
      if (name != null)
        LevelManager.SetCurrentLevelName(name);
      bool flag1 = intoSimulation == null && !StateManager.IsState(GameState.InLevel_Watch) && !VarManager.GetBool("checkmode");
      LevelManager.CurrentIntroInstructions = (string[]) null;
      LevelManager.CurrentLevelIsCustom = LevelManager.CurrentLevelName.ToLower().Contains("custom");
      bool flag2 = false;
      string str = (string) null;
      ScoreManager.UpdateOverview();
      try
      {
        using (StreamReader streamReader = new StreamReader((Stream) File.OpenRead(LevelManager.CurrentLevelPath)))
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load((TextReader) streamReader);
          XmlNode documentElement = (XmlNode) xmlDocument.DocumentElement;
          foreach (XmlNode selectNode in documentElement.SelectNodes("//objects/node()"))
            this.LoadObject(selectNode, intoSimulation);
          XmlNode xmlNode1 = documentElement.SelectSingleNode("//introinstructions");
          if (flag1 && xmlNode1 != null && !string.IsNullOrEmpty(xmlNode1.InnerText.Replace("\n", "").Trim()))
          {
            LevelManager.CurrentIntroInstructions = xmlNode1.InnerText.Split('\n');
            if (!VarManager.GetBool("hasplayedintro_" + name))
              LevelManager.PlayIntro();
          }
          XmlNode xmlNode2 = documentElement.SelectSingleNode("//meta/id");
          if (xmlNode2 != null)
            str = xmlNode2.InnerText;
        }
      }
      catch (Exception ex)
      {
        Utility.LogError(ex);
        PopupMenu.Instance?.Inform("An Unknown Error occured while loading*level: " + name + "*Check error.txt for details!", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), false);
        StateManager.TransitionBack(false);
        return false;
      }
      LevelManager.CurrentLevelID = str;
      bool flag3 = StateManager.IsState(GameState.InLevel_AnyWithText);
      if (flag3 & flag1)
      {
        Script currentScript;
        Script[] scripts = ScriptManager.GetScripts(LevelManager.CurrentLevelName, out currentScript);
        if (currentScript == null && scripts != null && scripts.Length != 0)
        {
          currentScript = scripts[0];
          ScriptManager.SaveCurrentScriptName(LevelManager.CurrentLevelName, currentScript.name);
        }
        if (currentScript != null)
        {
          TextEditor.Instance.LoadInstructions(currentScript.instructions, currentScript.name);
          flag2 = true;
        }
      }
      if (flag1 && !flag2 && flag3)
        TextEditor.Instance.LoadInstructions((string[]) null, "default");
      SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation =>
      {
        if (simulation.Player != null)
          return;
        simulation.SpawnPipe?.SpawnPlayerASAP();
      }));
      return true;
    }

        private void LoadObject(XmlNode obj, Simulation intoSimulation = null)
        {
            float x = float.Parse(((XmlNode)obj.Attributes["x"])?.Value, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture);
            float y = float.Parse(((XmlNode)obj.Attributes["y"])?.Value, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture);
            int width = int.Parse(((XmlNode)obj.Attributes["width"])?.Value);
            int height = int.Parse(((XmlNode)obj.Attributes["height"])?.Value);
            int rotation = int.TryParse(((XmlNode)obj.Attributes["rotation"])?.Value, out rotation) ? rotation : 0;
            float collisionHeightPercent = float.TryParse(((XmlNode)obj.Attributes["collisionHeightPercent"])?.Value, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out collisionHeightPercent) ? collisionHeightPercent : 1f;
            Microsoft.Xna.Framework.Rectangle spritePos;
            bool hasSpritePos = Utility.TryParseRectangle(((XmlNode)obj.Attributes["spritepos"])?.Value, out spritePos);
            if (!VarManager.GetBool("checkmode") && obj.Name == "backgroundobject")
            {
                float result;
                BackgroundObject backgroundObject = new BackgroundObject(x, y, width, height, hasSpritePos ? TextureManager.GetTexture("tileset") : (Texture2D)null, new Microsoft.Xna.Framework.Rectangle?(spritePos), (float)rotation, float.TryParse(((XmlNode)obj.Attributes["layerdepth"])?.Value, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result) ? new float?(result) : new float?(), intoSimulation);
            }
            if (intoSimulation != null)
                createObject(intoSimulation);
            else
                SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler)(simulation => createObject(simulation)));

            void createObject(Simulation simulation)
            {
                string name = obj.Name;
                switch (name)
                {
                    case "movingplatform":
                        int result1;
                        int result2;
                        int result3;
                        MovingPlatform movingPlatform = new MovingPlatform(simulation, x, y, width, height, int.TryParse(((XmlNode)obj.Attributes["offsetx"])?.Value, out result1) ? new int?(result1) : new int?(), int.TryParse(((XmlNode)obj.Attributes["offsety"])?.Value, out result2) ? new int?(result2) : new int?(), int.TryParse(((XmlNode)obj.Attributes["speed"])?.Value, out result3) ? new int?(result3) : new int?());
                        break;
                    case "portal":
                        Portal portal = new Portal(simulation, x, y);
                        break;
                    case "platform":
                        if (!hasSpritePos)
                        {
                            Platform platform = new Platform(simulation, x, y, width, height, ColorManager.GetColor("lightslate"));
                            break;
                        }
                        Platform platform1 = new Platform(simulation, x, y, width, height, Microsoft.Xna.Framework.Color.White, TextureManager.GetTexture("tileset"), new Microsoft.Xna.Framework.Rectangle?(spritePos), collisionHeightPercent, (float)rotation);
                        break;
                    case "bouncer":
                        int result4;
                        Bouncer bouncer = new Bouncer(simulation, x, y, width, height, (float)rotation, int.TryParse(((XmlNode)obj.Attributes["bouncespeed"])?.Value, out result4) ? new int?(result4) : new int?());
                        break;
                    case "spawnpipe":
                        int result5;
                        SpawnPipe spawnPipe = new SpawnPipe(simulation, x, y, int.TryParse(((XmlNode)obj.Attributes["playersize"])?.Value, out result5) ? new int?(result5) : new int?(), width, height);
                        break;
                    case "conveyorbelt":
                        int result6;
                        ConveyorBelt conveyorBelt = new ConveyorBelt(simulation, x, y, int.TryParse(((XmlNode)obj.Attributes["movespeed"])?.Value, out result6) ? new int?(result6) : new int?(), width, height);
                        break;
                    case "spike":
                        Spike spike = new Spike(simulation, x, y, width, height, (float)rotation);
                        break;
                    case "energyorb":
                        EnergyOrb energyOrb = new EnergyOrb(simulation, x, y);
                        break;
                    case "booster":
                        int result7;
                        Booster booster = new Booster(simulation, x, y, int.TryParse(((XmlNode)obj.Attributes["boostspeed"])?.Value, out result7) ? new int?(result7) : new int?());
                        break;
                }
            }
        }

    private void SaveLevel()
    {
      Simulation mainSimulation = SimulationManager.MainSimulation;
      LevelManager.CurrentLevelIsCustom = StateManager.IsState(GameState.LevelEditor) && !LevelManager.CurrentLevelName.Contains("AutoSave");
      string text1 = "";
      string str1 = "null";
      string str2 = "null";
      string currentLevelPath = LevelManager.CurrentLevelPath;
      string text2 = (string) null;
      if (File.Exists(currentLevelPath))
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(currentLevelPath);
        XmlNode documentElement = (XmlNode) xmlDocument.DocumentElement;
        XmlNode xmlNode1 = documentElement.SelectSingleNode("//introinstructions");
        if (xmlNode1 != null)
          text1 = xmlNode1.InnerText;
        XmlNode xmlNode2 = documentElement.SelectSingleNode("//meta/id");
        if (xmlNode2 != null)
          text2 = xmlNode2.InnerText;
        str1 = Utility.GetLevelCreatorName(currentLevelPath);
        str2 = Utility.GetLevelCreatorUUID(currentLevelPath);
      }
      if (string.IsNullOrEmpty(text2) && LevelManager.CurrentLevelIsCustom)
      {
        text2 = text2 ?? Guid.NewGuid().ToString();
        LevelManager.CurrentLevelID = text2;
      }
      XmlWriter writer = XmlWriter.Create(currentLevelPath, this.settings);
      writer.WriteStartDocument();
      writer.WriteStartElement("level");
      writer.WriteStartElement("meta");
      if (LevelManager.CurrentLevelIsCustom)
      {
        writer.WriteStartElement("id");
        writer.WriteString(text2);
        writer.WriteEndElement();
      }
      writer.WriteStartElement("creator");
      writer.WriteStartElement("name");
      writer.WriteString(str1 == "null" ? VarManager.GetString("username") : str1);
      writer.WriteEndElement();
      writer.WriteStartElement("uuid");
      writer.WriteString(str2 == "null" ? VarManager.GetString("uuid") : str2);
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteStartElement("lastsaved");
      writer.WriteString(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteStartElement("objects");
      this.SaveObject(writer, (GameObject) mainSimulation.Portal);
      this.SaveObject(writer, (GameObject) mainSimulation.SpawnPipe);
      mainSimulation.ForEachGameObject((Action<GameObject>) (g =>
      {
        if (g is SpawnPipe || g is Portal)
          return;
        this.SaveObject(writer, g);
      }));
      writer.WriteEndElement();
      writer.WriteStartElement("introinstructions");
      writer.WriteString(text1);
      writer.WriteEndElement();
      writer.WriteEndElement();
      writer.WriteEndDocument();
      writer.Close();
    }

    private void SaveObject(XmlWriter writer, GameObject g)
    {
      if (g == null || !g.IsActive)
        return;
      Type type = g.GetType();
      string localName = type.ToString().ToLower().Replace("botsarestupid.", "");
      if (type != typeof (Player) && type != typeof (Particle) && type != typeof (ParticleTrail) && type != typeof (Explosion))
      {
        writer.WriteStartElement(localName);
        XmlWriter xmlWriter1 = writer;
        float num1 = g.X;
        string str1 = num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xmlWriter1.WriteAttributeString("x", str1);
        XmlWriter xmlWriter2 = writer;
        num1 = g.Y;
        string str2 = num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        xmlWriter2.WriteAttributeString("y", str2);
        XmlWriter xmlWriter3 = writer;
        int num2 = g.Width;
        string str3 = num2.ToString();
        xmlWriter3.WriteAttributeString("width", str3);
        XmlWriter xmlWriter4 = writer;
        num2 = g.Height;
        string str4 = num2.ToString();
        xmlWriter4.WriteAttributeString("height", str4);
        if ((double) g.Rotation != 0.0)
        {
          XmlWriter xmlWriter5 = writer;
          num1 = g.Rotation;
          string str5 = num1.ToString();
          xmlWriter5.WriteAttributeString("rotation", str5);
        }
        if (!g.HasGeneratedTexture && (type == typeof (Platform) || type == typeof (BackgroundObject)))
        {
          Microsoft.Xna.Framework.Rectangle? renderSpritePos = g.RenderSpritePos;
          if (renderSpritePos.HasValue)
          {
            Microsoft.Xna.Framework.Rectangle rectangle = renderSpritePos.Value;
            writer.WriteAttributeString("spritepos", rectangle.X.ToString() + " " + rectangle.Y.ToString() + " " + rectangle.Width.ToString() + " " + rectangle.Height.ToString());
          }
          if ((double) g.CollisionHeightPercent != 1.0)
          {
            XmlWriter xmlWriter6 = writer;
            num1 = g.CollisionHeightPercent;
            string str6 = num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            xmlWriter6.WriteAttributeString("collisionHeightPercent", str6);
          }
        }
        if (type == typeof (BackgroundObject))
        {
          XmlWriter xmlWriter7 = writer;
          num1 = g.LayerDepth;
          string str7 = num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          xmlWriter7.WriteAttributeString("layerdepth", str7);
        }
        if (type == typeof (Booster))
        {
          XmlWriter xmlWriter8 = writer;
          num2 = ((Booster) g).BoostSpeed;
          string str8 = num2.ToString();
          xmlWriter8.WriteAttributeString("boostspeed", str8);
        }
        if (type == typeof (ConveyorBelt))
        {
          XmlWriter xmlWriter9 = writer;
          num2 = ((ConveyorBelt) g).MoveSpeed;
          string str9 = num2.ToString();
          xmlWriter9.WriteAttributeString("movespeed", str9);
        }
        if (type == typeof (SpawnPipe))
        {
          XmlWriter xmlWriter10 = writer;
          num2 = ((SpawnPipe) g).PlayerSize;
          string str10 = num2.ToString();
          xmlWriter10.WriteAttributeString("playersize", str10);
        }
        if (type == typeof (Bouncer))
        {
          XmlWriter xmlWriter11 = writer;
          num2 = ((Bouncer) g).BounceSpeed;
          string str11 = num2.ToString();
          xmlWriter11.WriteAttributeString("bouncespeed", str11);
        }
        if (type == typeof (MovingPlatform))
        {
          XmlWriter xmlWriter12 = writer;
          num2 = ((MovingPlatform) g).OffsetX;
          string str12 = num2.ToString();
          xmlWriter12.WriteAttributeString("offsetx", str12);
          XmlWriter xmlWriter13 = writer;
          num2 = ((MovingPlatform) g).OffsetY;
          string str13 = num2.ToString();
          xmlWriter13.WriteAttributeString("offsety", str13);
          XmlWriter xmlWriter14 = writer;
          num2 = ((MovingPlatform) g).Speed;
          string str14 = num2.ToString();
          xmlWriter14.WriteAttributeString("speed", str14);
        }
        writer.WriteEndElement();
      }
    }
  }
}

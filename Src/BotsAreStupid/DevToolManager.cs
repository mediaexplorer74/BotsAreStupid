// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.DevToolManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class DevToolManager
  {
    public static DevToolManager Instance { get; private set; }

    public DevToolManager() => DevToolManager.Instance = this;

    public void RegisterInput(bool enable = true)
    {
      if (enable)
      {
        Input.Register(KeyBind.SpawnBooster, new InputAction.InputHandler(this.SpawnBooster));
        Input.Register(KeyBind.Cancel, new InputAction.InputHandler(this.SpawnParticleTrail), cooldown: 0.01f, longPressDelay: 1f);
        Input.Register(KeyBind.SpawnExplosion, new InputAction.InputHandler(this.SpawnExplosion), cooldown: 0.01f, longPressDelay: 1f);
        Input.Register(KeyBind.SpawnRayCaster, new InputAction.InputHandler(this.SpawnRayCaster));
        Input.Register(KeyBind.CommandLine, new InputAction.InputHandler(this.CommandLine));
      }
      else
      {
        Input.UnRegister(KeyBind.SpawnBooster, new InputAction.InputHandler(this.SpawnBooster));
        Input.UnRegister(KeyBind.Cancel, new InputAction.InputHandler(this.SpawnParticleTrail));
        Input.UnRegister(KeyBind.SpawnExplosion, new InputAction.InputHandler(this.SpawnExplosion));
        Input.UnRegister(KeyBind.SpawnRayCaster, new InputAction.InputHandler(this.SpawnRayCaster));
        Input.UnRegister(KeyBind.CommandLine, new InputAction.InputHandler(this.CommandLine));
      }
    }

    private void SpawnBooster()
    {
      Vector2 mousePos = Utility.GetMousePos(true);
      Booster booster = new Booster(SimulationManager.MainSimulation, mousePos.X - 5f, mousePos.Y - 5f);
    }

    private void SpawnParticleTrail()
    {
      ParticleTrail particleTrail = new ParticleTrail(SimulationManager.MainSimulation, 3f, 8, Color.White, Utility.GetMousePos(true), SimulationManager.MainSimulation.Portal.Center, TextureManager.GetTexture("tileset"), new Rectangle?(TextureManager.GetSpritePos("energyorb")), true);
    }

    private void SpawnExplosion()
    {
      Explosion explosion = new Explosion(SimulationManager.MainSimulation, Utility.GetMousePos(true));
    }

    private void SpawnRayCaster()
    {
      Vector2 mousePos = Utility.GetMousePos(true);
      RayCaster rayCaster = new RayCaster(mousePos.X, mousePos.Y);
    }

    private void CommandLine()
    {
      PopupMenu.Instance.Prompt("Command:", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)), new PromptMenu.InteractionHandlerString(this.HandleCommand));
    }

    private void HandleCommand(string cmd)
    {
      Styling styling1 = new Styling()
      {
        //font = TextureManager.GetFont("megaMan2Small"),
        sliderHeight = 10,
        sliderThumbScale = 0.5f
      };
      Rectangle rectangle1 = new Rectangle(0, 0, 200, 60);
      string[] args = ScriptParser.GetArgs(cmd);
      switch (args[0].ToLower())
      {
        case "animate":
          VarManager.AnimateInt(args[1], int.Parse(args[2]), float.Parse(args[3]));
          break;
        case "animateview":
          float duration = float.Parse(args[4]);
          VarManager.AnimateInt("viewoffsetx", int.Parse(args[1]), duration);
          VarManager.AnimateInt("viewoffsety", int.Parse(args[2]), duration);
          VarManager.AnimateInt("viewscale", int.Parse(args[3]), duration);
          break;
        case "break":
          Console.WriteLine("100 / 0");
          break;
        case "camtools":
          Rectangle rectangle2 = new Rectangle(500, 500, 200, 200);
          Styling styling2 = new Styling();
          styling2.borderColor = ColorManager.GetColor("white");
          styling2.borderWidth = 4;
          styling2.padding = 10;
          Styling? style1 = new Styling?(styling2);
          Rectangle? constraints1 = new Rectangle?();
          MovableFlexbox movableFlexbox1 = new MovableFlexbox(rectangle2, style1, constraints1);
          movableFlexbox1.DrawOnTop = true;
          MovableFlexbox camTools = movableFlexbox1;
          camTools.Add((UIElement) new DisplaySlider(rectangle1, (Slider.SliderChangeHandler) ((v, f) => VarManager.SetInt("camzoom", v)), "Zoom", 0, 1000, 0, style: new Styling?(styling1)));
          camTools.Add((UIElement) new DisplaySlider(rectangle1, (Slider.SliderChangeHandler) ((v, f) => VarManager.SetInt("camspeed", v)), "Cam Speed", 0, 25, VarManager.GetInt("camspeed"), style: new Styling?(styling1)));
          camTools.Add((UIElement) new ToggleLine(rectangle1, "Fixed Pos: ", (Button.OnClick) (() => VarManager.SetBool("camfixedposition", true)), (Button.OnClick) (() => VarManager.SetBool("camfixedposition", false)), "Off", "On"));
          camTools.Add((UIElement) new DisplaySlider(rectangle1, (Slider.SliderChangeHandler) ((v, f) => VarManager.SetInt("camposx", v)), "Fixed X", -2000, 2000, VarManager.GetInt("camposx"), style: new Styling?(styling1)));
          camTools.Add((UIElement) new DisplaySlider(rectangle1, (Slider.SliderChangeHandler) ((v, f) => VarManager.SetInt("camposy", v)), "Fixed Y", -2000, 2000, VarManager.GetInt("camposy"), style: new Styling?(styling1)));
          MovableFlexbox movableFlexbox2 = camTools;
          Rectangle rectangle3 = rectangle1;
          Button.OnClick onClick1 = (Button.OnClick) (() => camTools.Destroy());
          Styling defaultButtonStyling1 = Styling.DefaultButtonStyling;
          styling2 = new Styling();
          styling2.text = "Close";
          Styling? newStyleNullable1 = new Styling?(styling2);
          Styling style2 = Styling.AddTo(defaultButtonStyling1, newStyleNullable1);
          Button newChild1 = new Button(rectangle3, onClick1, style2);
          movableFlexbox2.Add((UIElement) newChild1);
          camTools.Initialize();
          break;
        case "disable":
          if (args[1] != null)
          {
            VarManager.SetBool(args[1], false, false);
            break;
          }
          break;
        case "enable":
          if (args[1] != null)
          {
            VarManager.SetBool(args[1], true, false);
            break;
          }
          break;
        case "gif":
          Game1.Instance.CreateGif(args[1], "move right|wait 1.3|hook right|wait 0.4|unhook".Split('|'));
          break;
        case "levelmousepos":
          Console.WriteLine((object) Utility.GetMousePos(true));
          break;
        case "loadlevel":
          LevelManager.Load(Utility.TranslateLevelname(args[1]));
          break;
        case "log":
          string str = "";
          for (int index = 1; index < args.Length; ++index)
            str = str + args[index] + " ";
          Console.WriteLine(str);
          break;
        case "mousepos":
          Console.WriteLine((object) Utility.GetMousePos());
          break;
        case "randomscore":
          VarManager.SetBool("randomscores", true);
          Utility.PlayRandomScore();
          break;
        case "recpos":
          Game1.Instance.SetWindowPos(69, 69);
          break;
        case "replayintro":
          LevelManager.PlayIntro();
          break;
        case "saveoptions":
          VarManager.SaveOptions();
          break;
        case "screenshot":
          Game1.Instance.CaptureScreenshot();
          break;
        case "setval":
          int result;
          if (int.TryParse(args[2], out result))
          {
            VarManager.SetInt(args[1], result, false);
            break;
          }
          VarManager.SetString(args[1], args[2], false);
          break;
        case "showscore":
          string levelName = args[1];
          int num = int.Parse(args[2]);
          Utility.PlayScore(new ScoreData()
          {
            databaseId = num
          }, levelName);
          break;
        case "showval":
          string name = args[1];
          string info = "null";
          if (VarManager.HasBool(name))
            info = VarManager.GetBool(name).ToString();
          else if (VarManager.HasInt(name))
            info = VarManager.GetInt(name).ToString();
          else if (VarManager.HasString(name))
            info = VarManager.GetString(name);
          PopupMenu.Instance.Inform(info, new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive));
          return;
        case "spawnbiglogo":
          GameObject gameObject1 = new GameObject(SimulationManager.MainSimulation, (float) int.Parse(args[1]), (float) int.Parse(args[2]), 320, 180, new Color?(Color.White), TextureManager.GetTexture("biglogo"));
          break;
        case "spawnlogo":
          GameObject gameObject2 = new GameObject(SimulationManager.MainSimulation, (float) int.Parse(args[1]), (float) int.Parse(args[2]), 465, 40, new Color?(ColorManager.GetColor("red")), TextureManager.GetTexture("logo"));
          break;
        case "tickrate":
          SimulationManager.MainSimulation.SetTickrate(int.Parse(args[1]));
          break;
        case "viewtools":
          Rectangle rectangle4 = new Rectangle(500, 500, 200, 200);
          Styling styling3 = new Styling();
          styling3.borderColor = ColorManager.GetColor("white");
          styling3.borderWidth = 4;
          styling3.padding = 10;
          Styling? style3 = new Styling?(styling3);
          Rectangle? constraints2 = new Rectangle?();
          MovableFlexbox movableFlexbox3 = new MovableFlexbox(rectangle4, style3, constraints2);
          movableFlexbox3.DrawOnTop = true;
          MovableFlexbox viewTools = movableFlexbox3;
          viewTools.Add((UIElement) new DisplaySlider(rectangle1, (Slider.SliderChangeHandler) ((v, f) => VarManager.SetInt("viewoffsetx", -v)), "View X", -2500, 2500, VarManager.GetInt("viewoffsetx"), style: new Styling?(styling1)));
          viewTools.Add((UIElement) new DisplaySlider(rectangle1, (Slider.SliderChangeHandler) ((v, f) => VarManager.SetInt("viewoffsety", -v)), "View Y", -2500, 2500, VarManager.GetInt("viewoffsety"), style: new Styling?(styling1)));
          viewTools.Add((UIElement) new DisplaySlider(rectangle1, (Slider.SliderChangeHandler) ((v, f) => VarManager.SetInt("viewscale", v)), "Viewscale", 1, 200, 100, style: new Styling?(styling1)));
          MovableFlexbox movableFlexbox4 = viewTools;
          Rectangle rectangle5 = rectangle1;
          Button.OnClick onClick2 = (Button.OnClick) (() => viewTools.Destroy());
          Styling defaultButtonStyling2 = Styling.DefaultButtonStyling;
          styling3 = new Styling();
          styling3.text = "Close";
          Styling? newStyleNullable2 = new Styling?(styling3);
          Styling style4 = Styling.AddTo(defaultButtonStyling2, newStyleNullable2);
          Button newChild2 = new Button(rectangle5, onClick2, style4);
          movableFlexbox4.Add((UIElement) newChild2);
          viewTools.Initialize();
          break;
      }
      PopupMenu.Instance.SetActive(false);
    }
  }
}

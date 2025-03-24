// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ToolWindow
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;
using System;
using System.Globalization;

#nullable disable
namespace BotsAreStupid
{
  internal class ToolWindow : MovableFlexbox
  {
    private const int margin = 10;
    private const int padding = 15;
    private const int elementWidth = 220;
    private const int elementHeight = 40;
    private static readonly ColumnInfo[] columnInfos = new ColumnInfo[3]
    {
      new ColumnInfo("No.", 0.2f, false, false),
      new ColumnInfo("Name", 0.65f, false, false),
      new ColumnInfo("", 0.15f, false, false, false)
    };
    private InfoLine[] infoLines = new InfoLine[3];
    private UIElement velocityInfo;
    private InfoLine[] velocityInfoLines = new InfoLine[3];
    private UIElement groundedInfo;
    private InfoLine[] groundedInfoLines = new InfoLine[1];
    private UIElement hookedInfo;
    private InfoLine[] hookedInfoLines = new InfoLine[2];
    private ListTable simulationList;
    private Button addGhostButton;

    public ToolWindow()
      : base(new Microsoft.Xna.Framework.Rectangle(0, 0, 10, 10), new Styling?(new Styling()
      {
        borderColor = ColorManager.GetColor("white"),
        borderWidth = 2,
        defaultColor = new Microsoft.Xna.Framework.Color(ColorManager.GetColor("darkslate"), 0.4f),
        padding = 15,
        isLevelUi = true,
        drawInState = GameState.InLevel_AnyWithText
      }), new Microsoft.Xna.Framework.Rectangle?(Utility.LevelConstraints), 10)
    {
      this.CreateChildren();
    }

    private void CreateChildren()
    {
      BitmapFont font1 = TextureManager.GetFont("megaMan2");
      BitmapFont font2 = TextureManager.GetFont("megaMan2Small");
      Microsoft.Xna.Framework.Color color = ColorManager.GetColor("white");
      ColorManager.GetColor("lightslate");
      UIElement header = new UIElement(new Microsoft.Xna.Framework.Rectangle(0, 0, 250, 40));
      Styling defaultButtonStyling1 = Styling.DefaultButtonStyling with
      {
        borderWidth = 2
      };
      header.AddChild(new Button(new Microsoft.Xna.Framework.Rectangle(header.Width - 40, 0, 40, 40), new Button.OnClick(((UIElement) this).ToggleActive), defaultButtonStyling1).AddChild(new UIElement(Microsoft.Xna.Framework.Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p, 0.4f)))));
      this.SetHeader(header);
      Styling defaultButtonStyling2 = Styling.DefaultButtonStyling with
      {
        borderWidth = 2,
        //font = font2
      };
      DisplaySlider timeScaleSlider = new DisplaySlider(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, 32), (Slider.SliderChangeHandler) ((value, final) => VarManager.SetInt("timescale", value, ignoreListeners: true)), "Timescale:", 1, 200, 100, valueConverter: (DisplaySlider.ValueToString) (value => ((float) value / 100f).ToString("0.00", (IFormatProvider) CultureInfo.InvariantCulture)), enablePromptInput: false);
      VarManager.AddListener("timescale", (VarManager.VarChangeHandler) (v => timeScaleSlider.SetValue(int.Parse(v))));
      this.Add((UIElement) timeScaleSlider);
      Styling infoStyling = new Styling() 
      { 
          //font = font2 
      };
      int height = 8;
      int x = 22 * 2;
      this.velocityInfo = new UIElement(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, height * 7)).AddChild((UIElement) (this.velocityInfoLines[0] = new InfoLine(new Microsoft.Xna.Framework.Rectangle(x, 0, 220 - x, height), "Angle: ", "0", new Styling?(infoStyling)))).AddChild((UIElement) new ToggleLine(new Microsoft.Xna.Framework.Rectangle(x, height / 2 * 3, 220 - x, height), "Show Vector:", (Button.OnClick) (() => VarManager.SetBool("showvelocityvector", true)), (Button.OnClick) (() => VarManager.SetBool("showvelocityvector", false)), "Off", "On", style: new Styling?(infoStyling))).AddChild((UIElement) (this.velocityInfoLines[1] = new InfoLine(new Microsoft.Xna.Framework.Rectangle(x, height / 2 * 7, 220 - x, height), "X-Axis: ", "0", new Styling?(infoStyling)))).AddChild((UIElement) (this.velocityInfoLines[2] = new InfoLine(new Microsoft.Xna.Framework.Rectangle(x, height * 5, 220 - x, height), "Y-Axis: ", "0", new Styling?(infoStyling))));
      this.Add(Accordion.Create(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, height), "Velocity:", out this.infoLines[0], infoStyling, new System.Action(((Flexbox) this).CalculateRectangle), this.velocityInfo));
      this.Add(this.velocityInfo);
      this.groundedInfo = new UIElement(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, height * 2)).AddChild((UIElement) (this.groundedInfoLines[0] = new InfoLine(new Microsoft.Xna.Framework.Rectangle(x, 0, 220 - x, height), "Duration: ", "0", new Styling?(infoStyling))));
      this.Add(Accordion.Create(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, height), "Grounded:", out this.infoLines[1], infoStyling, new System.Action(((Flexbox) this).CalculateRectangle), this.groundedInfo));
      this.Add(this.groundedInfo);
      this.hookedInfo = new UIElement(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, height * 5)).AddChild((UIElement) (this.hookedInfoLines[0] = new InfoLine(new Microsoft.Xna.Framework.Rectangle(x, 0, 220 - x, height), "Angle: ", "0", new Styling?(infoStyling)))).AddChild((UIElement) (this.hookedInfoLines[1] = new InfoLine(new Microsoft.Xna.Framework.Rectangle(x, height / 2 * 3, 220 - x, height), "Length: ", "0", new Styling?(infoStyling)))).AddChild((UIElement) new ToggleLine(new Microsoft.Xna.Framework.Rectangle(x, height * 4, 220 - x, height), "Show Preview:", (Button.OnClick) (() => VarManager.SetBool("showhookpreview", true)), (Button.OnClick) (() => VarManager.SetBool("showhookpreview", false)), "Off", "On", style: new Styling?(infoStyling)));
      this.Add(Accordion.Create(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, height), "Hooked:", out this.infoLines[2], infoStyling, new System.Action(((Flexbox) this).CalculateRectangle), this.hookedInfo));
      this.Add(this.hookedInfo);
      this.Add(new UIElement(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, 0)));
      VarManager.SetBool("showhitboxes_special", true);
      UIElement uiElement1 = new UIElement(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, height * 7)).AddChild((UIElement) new ToggleLine(new Microsoft.Xna.Framework.Rectangle(x, 0, 220 - x, height), "Special:", (Button.OnClick) (() => VarManager.SetBool("showhitboxes_special", true)), (Button.OnClick) (() => VarManager.SetBool("showhitboxes_special", false)), "Off", "On", true, new Styling?(infoStyling))).AddChild((UIElement) new ToggleLine(new Microsoft.Xna.Framework.Rectangle(x, height / 2 * 3, 220 - x, height), "Platforms:", (Button.OnClick) (() => VarManager.SetBool("showhitboxes_platforms", true)), (Button.OnClick) (() => VarManager.SetBool("showhitboxes_platforms", false)), "Off", "On", style: new Styling?(infoStyling))).AddChild((UIElement) new ToggleLine(new Microsoft.Xna.Framework.Rectangle(x, height / 2 * 7, 220 - x, height), "Background:", (Button.OnClick) (() => VarManager.SetBool("showhitboxes_background", true)), (Button.OnClick) (() => VarManager.SetBool("showhitboxes_background", false)), "Off", "On", style: new Styling?(infoStyling))).AddChild((UIElement) new ToggleLine(new Microsoft.Xna.Framework.Rectangle(x, height * 5, 220 - x, height), "Particles:", (Button.OnClick) (() => VarManager.SetBool("showhitboxes_particles", true)), (Button.OnClick) (() => VarManager.SetBool("showhitboxes_particles", false)), "Off", "On", style: new Styling?(infoStyling)));
      this.Add(Accordion.Create(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, height), "Hitboxes:", "showhitboxes", out ToggleLine _, infoStyling, new System.Action(((Flexbox) this).CalculateRectangle), uiElement1));
      this.Add(uiElement1);
      UIElement uiElement2 = new UIElement(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, height * 2)).AddChild((UIElement) new ToggleLine(new Microsoft.Xna.Framework.Rectangle(x, 0, 220 - x, height), "Permanent:", (Button.OnClick) (() => VarManager.SetBool("permanentruler", true)), (Button.OnClick) (() => VarManager.SetBool("permanentruler", false)), "Off", "On", style: new Styling?(infoStyling)));
      this.Add(Accordion.Create(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, height), "Ruler:", "rulerenabled", out ToggleLine _, infoStyling, new System.Action(((Flexbox) this).CalculateRectangle), uiElement2, "With this enabled: click and drag to measure distances."));
      this.Add(uiElement2);
      this.Add(new UIElement(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, 0)));
      bool active = VarManager.HasBool("autoaddghost") && VarManager.GetBool("autoaddghost");
      ArrowSelection autoGhostSelection = new ArrowSelection(new Microsoft.Xna.Framework.Rectangle(0, 0, 220, 30), "Auto:   ", (ArrowSelection.DropdownChangeHandler) (v =>
      {
        VarManager.SetString("autoaddghosttype", v);
        VarManager.SaveOptions();
        SimulationManager.UpdateAutoGhost();
      }), defaultButtonStyling2, VarManager.GetString("autoaddghosttype"), true, (ArrowSelection.ValueDisplayModifier) (v => v.Replace('_', ' ')), new string[3]
      {
        "random",
        "best_personal",
        "best_global"
      });
      autoGhostSelection.SetActive(active);
      string str = "Automatically adds a ghost-bot from the database to the level.\nYou can use this to compare your bot with some other random player,\nyour own personal record or even the global highscore.";
      Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(0, 0, 220, 30);
      Button.OnClick onClick1 = (Button.OnClick) (() =>
      {
        VarManager.SetBool("autoaddghost", true);
        VarManager.SaveOptions();
        autoGhostSelection.SetActive(true);
        this.CalculateRectangle();
        SimulationManager.UpdateAutoGhost();
      });
      Button.OnClick onEnabledClick = (Button.OnClick) (() =>
      {
        VarManager.SetBool("autoaddghost", false);
        VarManager.SaveOptions();
        autoGhostSelection.SetActive(false);
        this.CalculateRectangle();
        SimulationManager.UpdateAutoGhost();
      });
      Styling style1 = Styling.AddTo(defaultButtonStyling2, new Styling?(new Styling()
      {
        text = "Auto Add Ghost:   Off",
        tooltip = str
      }));
      Styling style2 = defaultButtonStyling2;
      Styling styling = new Styling();
      styling.text = "Auto Add Ghost:   On";
      styling.tooltip = str;
      Styling? newStyleNullable1 = new Styling?(styling);
      Styling enabledStyle = Styling.AddTo(style2, newStyleNullable1);
      int num = active ? 1 : 0;
      this.Add((UIElement) new ToggleButton(rectangle1, onClick1, onEnabledClick, style1, enabledStyle, num != 0));
      this.Add((UIElement) autoGhostSelection);
      ListElementAction[] listElementActionArray = new ListElementAction[1]
      {
        new ListElementAction(ListElementActionType.Delete, (ListElementAction.Action) (element => SimulationManager.RemoveSimulation((element as SimulationListElement).Simulation)))
      };
      Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(0, 0, 220, 160);
      ColumnInfo[] columnInfos = ToolWindow.columnInfos;
      ListElementAction[] actions = listElementActionArray;
      styling = new Styling();
      //styling.font = font2;
      styling.borderWidth = 2;
      Styling? style3 = new Styling?(styling);
      this.Add((UIElement) (this.simulationList = new ListTable(rectangle2, columnInfos, actions, rowAmount: 5, style: style3, sortIndicatorHeight: 3, scrollbarEnabled: false)));
      this.simulationList.SortBy(ToolWindow.columnInfos[1].name, new bool?(false));
      this.simulationList.SetActive(false);
      Microsoft.Xna.Framework.Rectangle rectangle3 = new Microsoft.Xna.Framework.Rectangle(0, 0, 220, 30);
      Button.OnClick onClick2 = (Button.OnClick) (() =>
      {
        if (StateManager.IsState(GameState.InLevel_FromEditor))
          PopupMenu.Instance.ShowLoadInstructionsMenu(true);
        else
          Utility.OpenContextMenu(true, (UIElement) this.addGhostButton, ("... From leaderboard", (Button.OnClick) (() => ScoreExplorer.Instance.ShowList(LevelManager.CurrentLevelNameSimple, sortBy: "Time", sortDescending: new bool?(false), createGhostOnWatch: true))), ("... From saved instructions", (Button.OnClick) (() => PopupMenu.Instance.ShowLoadInstructionsMenu(true))));
      });
      Styling style4 = defaultButtonStyling2;
      styling = new Styling();
      styling.text = "Add Ghost ...";
      Styling? newStyleNullable2 = new Styling?(styling);
      Styling style5 = Styling.AddTo(style4, newStyleNullable2);
      this.Add((UIElement) (this.addGhostButton = new Button(rectangle3, onClick2, style5)));
      SimulationManager.OnSimulationCreated += (Action<Simulation>) (simulation =>
      {
        if (!StateManager.IsState(GameState.InLevel_AnyWithText))
          return;
        bool isEmpty = this.simulationList.IsEmpty;
        this.simulationList.AddElement((IListElement) new SimulationListElement(simulation));
        if (!isEmpty)
          return;
        this.simulationList.SetActive(true);
        this.CalculateRectangle();
      });
      SimulationManager.OnSimulationRemoved += (Action<Simulation>) (simulation =>
      {
        if (!StateManager.IsState(GameState.InLevel_AnyWithText))
          return;
        this.simulationList.RemoveElements((Func<IListElement, bool>) (e => (e as SimulationListElement).Simulation == simulation));
        if (!this.simulationList.IsEmpty)
          return;
        this.simulationList.SetActive(false);
        this.CalculateRectangle();
      });
      SimulationManager.OnSimulationsCleared += (System.Action) (() =>
      {
        this.simulationList.SetElements((IListElement[]) null);
        this.simulationList.SetActive(false);
        this.CalculateRectangle();
      });
      this.CalculateRectangle();
    }

    public override void Update(float delta)
    {
      base.Update(delta);
      Player mainPlayer = Utility.MainPlayer;
      if (mainPlayer == null)
        return;
      float velocityMagnitude = mainPlayer.RoundedVelocityMagnitude;
      this.infoLines[0].SetInfoText(((double) velocityMagnitude == 0.0 ? "0" : velocityMagnitude.ToString("0.00")) + "  u/s");
      this.infoLines[1].SetInfoText(mainPlayer.IsGrounded ? "Yes" : "No");
      this.infoLines[2].SetInfoText(mainPlayer.IsHooked ? "Yes" : "No");
      if (this.velocityInfo.IsActive)
      {
        Vector2 velocity = mainPlayer.Velocity;
        velocity.X = (float) Math.Round((double) velocity.X, 2);
        velocity.Y = -1f * (float) Math.Round((double) velocity.Y, 2);
        this.velocityInfoLines[0].SetInfoText(getDegreeString(velocity.GetDegrees(), velocityMagnitude));
        this.velocityInfoLines[1].SetInfoText((double) velocity.X == 0.0 ? "0" : velocity.X.ToString("0.00"));
        this.velocityInfoLines[2].SetInfoText((double) velocity.Y == 0.0 ? "0" : velocity.Y.ToString("0.00"));
      }
      if (this.groundedInfo.IsActive)
      {
        float groundedTime = mainPlayer.GroundedTime;
        this.groundedInfoLines[0].SetInfoText((double) groundedTime == 0.0 ? "0" : groundedTime.ToString("0.00"));
      }
      if (this.hookedInfo.IsActive)
      {
        Hook hookObject = mainPlayer.HookObject;
        float extensionLength = hookObject.ExtensionLength;
        this.hookedInfoLines[0].SetInfoText(getDegreeString(hookObject.Angle, extensionLength));
        this.hookedInfoLines[1].SetInfoText((double) extensionLength == 0.0 ? "0" : extensionLength.ToString("0.00"));
      }

      static string getDegreeString(double degree, float magnitude)
      {
        return (double) magnitude != 0.0 ? (-1.0 * ((degree + 90.0 + 360.0) % 360.0 - 180.0)).ToString("0.00") + " deg" : "-";
      }
    }
  }
}

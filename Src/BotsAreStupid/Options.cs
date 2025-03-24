// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Options
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;

#nullable disable
namespace BotsAreStupid
{
  internal class Options : BasicTabMenu
  {
    private const int padding = 50;
    private const int elementHeight = 70;
    private Button usernameButton;
    private UIElement windowSizeOptions;
    private Button applyWindowSizeButton;
    private ArrowSelection windowSizeSelection;
    private string windowSize;

    public Options(Rectangle rectangle)
      : base(rectangle, GameState.Options, new Styling?(), (Button.OnClick) null, "General", "Gameplay", "Graphics")
    {
      this.windowSize = VarManager.GetString("windowsize");
      this.CreateChildren();
      Input.RegisterOnDown(KeyBind.Enter, new InputAction.InputHandler(this.ApplyWindowSize), GameState.Options);
    }

    private void CreateChildren()
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = ColorManager.GetColor("white");
      Color color3 = ColorManager.GetColor("darkslate");
      Color color4 = ColorManager.GetColor("lightslate");
      BitmapFont font = TextureManager.GetFont("megaMan2");
      Styling style1 = new Styling()
      {
        defaultColor = color3,
        borderColor = color2,
        borderWidth = 4,
        //font = font,
        defaultTextColor = color2,
        centerText = true
      };
      Styling style2 = style1 with
      {
        hoverColor = color4,
        hoverCursor = new bool?(true),
        clickColor = color1
      };
      this.main.GetRectangle();
      Styling enabledStyle = style2;
      UIElement main1 = this.main;
      Rectangle empty1 = Rectangle.Empty;
      int num1 = VarManager.GetInt("mastervolume");
      Styling? style3 = new Styling?();
      DisplaySlider masterSlider;
      DisplaySlider child1 = masterSlider = new DisplaySlider(empty1, (Slider.SliderChangeHandler) ((v, f) => SoundManager.SetVolume("master", v, f)), "Master Volume:", 0, 100, num1, style: style3);
      main1.AddChild((UIElement) child1);
      masterSlider.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 5, 100, parentRect.Width / 5 * 3, 70)));
      VarManager.AddListener("mastervolume", (VarManager.VarChangeHandler) (v => masterSlider.SetValue(int.Parse(v), true)));
      UIElement main2 = this.main;
      Rectangle empty2 = Rectangle.Empty;
      int num2 = VarManager.GetInt("sfxvolume");
      Styling? style4 = new Styling?();
      DisplaySlider sfxSlider;
      DisplaySlider child2 = sfxSlider = new DisplaySlider(empty2, (Slider.SliderChangeHandler) ((v, f) => SoundManager.SetVolume("sfx", v, f)), "SFX Volume:", 0, 100, num2, style: style4);
      main2.AddChild((UIElement) child2);
      sfxSlider.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 5, 195, parentRect.Width / 5 * 3, 70)));
      VarManager.AddListener("sfxvolume", (VarManager.VarChangeHandler) (v => sfxSlider.SetValue(int.Parse(v), true)));
      UIElement main3 = this.main;
      Rectangle empty3 = Rectangle.Empty;
      int num3 = VarManager.GetInt("musicvolume");
      Styling? style5 = new Styling?();
      DisplaySlider musicSlider;
      DisplaySlider child3 = musicSlider = new DisplaySlider(empty3, (Slider.SliderChangeHandler) ((v, f) => SoundManager.SetVolume("music", v, f)), "Music Volume:", 0, 100, num3, style: style5);
      main3.AddChild((UIElement) child3);
      musicSlider.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 5, 290, parentRect.Width / 5 * 3, 70)));
      VarManager.AddListener("musicvolume", (VarManager.VarChangeHandler) (v => musicSlider.SetValue(int.Parse(v), true)));
      string userName = VarManager.GetString("username");
      style2.text = "Username:   " + userName;
      this.usernameButton = new Button(Rectangle.Empty, (Button.OnClick) (() => VarManager.Instance.PromptUsername(fieldValue: userName)), style2);
      this.usernameButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 5, parentRect.Height - 240, parentRect.Width / 5 * 3, 70)));
      this.main.AddChild((UIElement) this.usernameButton);
      VarManager.AddListener("username", (VarManager.VarChangeHandler) (s => this.usernameButton.SetText("Username:   " + s)));
      style2.text = "Developer Tools:   Off";
      enabledStyle.text = "Developer Tools:   On";
      this.main.AddChild(new ToggleButton(Rectangle.Empty, (Button.OnClick) (() => PopupMenu.Instance.Inform("Spawn Booster:   X*Spawn ParticleTrail:   C*Spawn Explosion:   V", new PromptMenu.InteractionHandler(this.EnableDevTools), false)), new Button.OnClick(this.DisableDevTools), style2, enabledStyle).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 5, parentRect.Height - 50 - 70, parentRect.Width / 5 * 3, 70))));
      UIElement child4 = new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 5, parentRect.Height / 4, parentRect.Width / 5 * 3, parentRect.Height / 2)));
      this.contentElements[1].AddChild(child4);
      child4.AddChild(new ArrowSelection(Rectangle.Empty, "State Saves:", new ArrowSelection.DropdownChangeHandler(this.UpdateSavesPerSecond), Styling.AddTo(style1, new Styling?(new Styling()
      {
        tooltip = "Determines how often the state of the Simulation is saved.\nHigher values mean that the game will use more memory, but will allow for smoother backward stepping."
      })), VarManager.GetInt("savespersecond").ToString(), false, (ArrowSelection.ValueDisplayModifier) (s => "   " + s + " / second"), new string[5]
      {
        "5",
        "10",
        "20",
        "50",
        "100"
      }).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, 70))));
      style2.text = "Live Script Editing:   Off";
      enabledStyle.text = "Live Script Editing:   On";
      ToggleButton liveEditingToggle = new ToggleButton(Rectangle.Empty, new Button.OnClick(Options.ToggleLiveScriptEditing), new Button.OnClick(Options.ToggleLiveScriptEditing), style2, enabledStyle, VarManager.GetBool("livescriptchanges"));
      liveEditingToggle.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 120, parentRect.Width, 70)));
      VarManager.AddListener("livescriptchanges", (VarManager.VarChangeHandler) (v => liveEditingToggle.SetEnabled(bool.Parse(v))));
      child4.AddChild((UIElement) liveEditingToggle);
      UIElement child5 = new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 5, parentRect.Height / 4, parentRect.Width / 5 * 3, parentRect.Height / 2)));
      this.contentElements[2].AddChild(child5);
      style2.text = "Fullscreen:   Off";
      enabledStyle.text = "Fullscreen:   On";
      ToggleButton fullscreenToggle = new ToggleButton(Rectangle.Empty, new Button.OnClick(Game1.ToggleFullscreen), new Button.OnClick(Game1.ToggleFullscreen), style2, enabledStyle, VarManager.GetBool("fullscreen"));
      fullscreenToggle.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, 70)));
      VarManager.AddListener("fullscreen", (VarManager.VarChangeHandler) (v => fullscreenToggle.SetEnabled(bool.Parse(v))));
      child5.AddChild((UIElement) fullscreenToggle);
      child5.AddChild(this.windowSizeOptions = new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 120, parentRect.Width, 70))));
      this.windowSizeOptions.AddChild((UIElement) (this.windowSizeSelection = new ArrowSelection(Rectangle.Empty, "Window size:", (ArrowSelection.DropdownChangeHandler) (s => this.windowSize = s), style1, this.windowSize, false, (ArrowSelection.ValueDisplayModifier) (s => "   " + s), new string[9]
      {
        "640x360",
        "960x600",
        "1120x700",
        "1280x800",
        "1440x900",
        "1600x900",
        "1792x1120",
        "1920x1080",
        "2560x1440"
      })));
      this.windowSizeSelection.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width / 3 * 2 - 25, 70)));
      style2.text = "Apply";
      this.windowSizeOptions.AddChild((UIElement) (this.applyWindowSizeButton = new Button(Rectangle.Empty, new Button.OnClick(this.ApplyWindowSize), style2)));
      this.applyWindowSizeButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 3 * 2 + 25, 0, parentRect.Width / 3 - 25, 70)));
      this.windowSizeOptions.SetActive(!VarManager.GetBool("fullscreen"));
      VarManager.AddListener("fullscreen", (VarManager.VarChangeHandler) (v => this.windowSizeOptions.SetActive(!bool.Parse(v))));
      child5.AddChild(new DisplaySlider(Rectangle.Empty, new Slider.SliderChangeHandler(this.UpdateParticleAmount), "Particle Amount:", 0, 4, VarManager.GetInt("particleamount"), " X").SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - 70, parentRect.Width, 70))));
    }

    public override void Update(float deltaTime)
    {
      if (!this.IsActive || !this.IsCurrentState())
        ;
    }

    private void EnableDevTools()
    {
      PopupMenu.Instance.SetActive(false);
      VarManager.SetBool("devtoolsenabled", true);
      DevToolManager.Instance.RegisterInput();
    }

    private void DisableDevTools()
    {
      VarManager.SetBool("devtoolsenabled", false);
      DevToolManager.Instance.RegisterInput(false);
    }

    public static void ToggleLiveScriptEditing()
    {
      VarManager.ToggleBool("livescriptchanges");
      VarManager.SaveOptions();
      if (!StateManager.IsState(GameState.InLevel_Any))
        return;
      if (SimulationManager.HasLockedFrame)
        SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (s => s.ResetLockedFrame()));
      Game1.Instance.OnResetButtonPress();
    }

    private void UpdateSavesPerSecond(string value)
    {
      int result;
      if (!int.TryParse(value, out result))
        return;
      VarManager.SetInt("savespersecond", result);
      VarManager.SaveOptions();
    }

    private void SetApplyButtonActive(bool active)
    {
      if (active)
      {
        Color color = ColorManager.GetColor("red");
        this.applyWindowSizeButton.ChangeStyle(new Styling()
        {
          defaultColor = color,
          hoverColor = Color.Multiply(color, 1.2f),
          hoverCursor = new bool?(true)
        });
      }
      else
      {
        Color color = ColorManager.GetColor("darkslate");
        this.applyWindowSizeButton.ChangeStyle(new Styling()
        {
          defaultColor = color,
          hoverColor = color
        });
      }
    }

    private void ApplyWindowSize()
    {
      if (!this.contentElements[2].IsActive || VarManager.GetBool("fullscreen"))
        return;
      string[] strArray = this.windowSize.Split('x');
      Game1.Instance.SetWindow(int.Parse(strArray[0]), int.Parse(strArray[1]), false);
      VarManager.SetString("windowsize", this.windowSize);
      VarManager.SaveOptions();
      this.SetApplyButtonActive(false);
    }

    private void UpdateParticleAmount(int value, bool final)
    {
      if (!final)
        return;
      VarManager.SetInt("particleamount", value);
      VarManager.SaveOptions();
    }
  }
}

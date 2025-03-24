// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.MainMenu
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.BitmapFonts;

#nullable disable
namespace BotsAreStupid
{
  internal class MainMenu : UIElement
  {
    public const int minHeaderHeight = 150;
    public static int headerHeight = 150;
    private const int logoSize = 600;
    private const int mainButtonHeight = 40;
    private const int mainButtonMargin = 50;
    public const int minFooterHeight = 100;
    public static int footerHeight = 100;
    private const int footerPadding = 40;
    private const int footerButtonHeight = 30;
    private const float menuOptionsOpacity = 0.2f;
    private const int menuOptionsMargin = 20;
    private const int menuOptionsPadding = 20;
    private const int menuOptionsElementWidth = 140;
    private const int menuOptionsElementHeight = 40;
    private const int menuOptionsToggleSize = 20;
    private InfoLine menuOptionsStatusLine;
    private InfoLine menuOptionsBotIdLine;
    private InfoLine menuOptionsSavedCountLine;
    private Button noConnectionWarningElement;
    private bool hasCheckedConnection = false;
    private const string checkingConnectionInfo = "Trying to connect to server...";
    private const string noConnectionWarning = "Could not connect to server... Try again?";
    private const string successfulConnectionInfo = "Successfully connected to server!";
    private float connectedInfoTime = 0.0f;
    private const float connectedInfoTimeMax = 3f;

    public static MainMenu Instance { get; private set; }

    public MainMenu(Rectangle rectangle)
      : base(rectangle)
    {
      MainMenu.Instance = this;
      this.CreateChildren();
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if ((double) this.connectedInfoTime <= 0.0)
        return;
      this.connectedInfoTime -= deltaTime;
      if ((double) this.connectedInfoTime <= 0.0)
        this.noConnectionWarningElement.SetActive(false);
    }

    private void CreateChildren()
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = ColorManager.GetColor("white");
      Color color3 = ColorManager.GetColor("lightslate");
      Color color4 = ColorManager.GetColor("darkslate");
      BitmapFont font1 = TextureManager.GetFont("megaMan2Small");
      BitmapFont font2 = TextureManager.GetFont("courier");
      Styling style1 = new Styling()
      {
        defaultColor = color2,
        hoverColor = new Color(150, 150, 150),
        clickColor = color1
      };
      Styling styling1 = new Styling()
      {
        borderWidth = 4,
        borderColor = color2,
        defaultColor = color3,
        enabledBorders = new BorderInfo(bottom: true)
      };
      Texture2D texture = TextureManager.GetTexture("logo");
      int logoHeight = (int) (600.0 * ((double) texture.Height / (double) texture.Width));
      UIElement child1 = new UIElement(Rectangle.Empty, new Styling?(styling1)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, MainMenu.headerHeight)));
      child1.AddChild(new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 - 300, MainMenu.headerHeight / 2 - logoHeight / 2, 600, logoHeight))));
      this.AddChild(child1);
      UIElement content = new UIElement(new Rectangle(50, 190, 180, 320));
      Styling styling2 = new Styling()
      {
        sliderHeight = 5,
        sliderThumbScale = 0.5f,
        rightText = true,
        //font = font1
      };
      UIElement uiElement = content;
      Rectangle rectangle1 = new Rectangle(20, 20, 140, 40);
      Styling? style2 = new Styling?(styling2);
      DisplaySlider ocSlider;
      DisplaySlider child2 = ocSlider = new DisplaySlider(rectangle1, (Slider.SliderChangeHandler) ((v, f) => VarManager.SetInt("overclock", v, ignoreListeners: true)), "Overclock:", 1, 99, 1, " X", style2);
      uiElement.AddChild((UIElement) child2);
      VarManager.AddListener("overclock", (VarManager.VarChangeHandler) (s => ocSlider.SetValue(int.Parse(s))));
      content.AddChild((UIElement) new DisplaySlider(new Rectangle(20, 80, 140, 40), (Slider.SliderChangeHandler) ((v, f) => VarManager.SetInt("randombotspercent", v, ignoreListeners: true)), "Random:", 0, 100, VarManager.GetInt("randombotspercent"), " / 100", new Styling?(styling2)));
      Styling defaultButtonStyling = Styling.DefaultButtonStyling with
      {
        //font = font1,
        tooltip = "If enabled randomly generated bots that make it through the level\nwill be uploaded to the server.",
        text = "Save: On",
        borderWidth = 2
      };

      Styling style3 = defaultButtonStyling with
      {
        text = "Save: Off"
      };

      content.AddChild((UIElement) new ToggleButton(new Rectangle(20, 140, 140, 40),
          (Button.OnClick) (() => VarManager.ToggleBool("saverandom")),
          (Button.OnClick) (() => VarManager.ToggleBool("saverandom")), style3, defaultButtonStyling));
      defaultButtonStyling.text = "Load Next";
      defaultButtonStyling.tooltip = "Load a previously generated bot by id.";

      content.AddChild((UIElement) new Button(new Rectangle(20, 200, 140, 40),
          (Button.OnClick) (() => PopupMenu.Instance.Prompt("Bot ID:", 
          new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive),
          (PromptMenu.InteractionHandlerString) (s =>
      {
        VarManager.SetInt("nextbot", int.Parse(s));
        SimulationManager.MainSimulation.Player.Kill();
        PopupMenu.Instance.SetActive(false);
      }), (PromptMenu.Validator) ((string input, out string msg) => 
      Utility.NumberValidator(input, out msg, ScriptInterpreter.SavedCount)))), defaultButtonStyling));

      int height = 24;//(int) font1.MeasureStringHalf("a").Height;
      Rectangle rectangle2 = new Rectangle(20, 260, 140, 40);
      content.AddChild((UIElement) (this.menuOptionsStatusLine = new InfoLine(new Rectangle(rectangle2.X, rectangle2.Y, rectangle2.Width, height), "Current:", "")));
      content.AddChild((UIElement) (this.menuOptionsBotIdLine = new InfoLine(new Rectangle(rectangle2.X, rectangle2.Y + rectangle2.Height / 2 - height / 2, rectangle2.Width, height), "Bot ID:", "-")));
      content.AddChild((UIElement) (this.menuOptionsSavedCountLine = new InfoLine(new Rectangle(rectangle2.X, rectangle2.Y + rectangle2.Height - height, rectangle2.Width, height), "Saved:", "0")));
      ContentToggleButton menuOptionsToggle = new ContentToggleButton(new Rectangle(20, 190, 20, 20), content);
      menuOptionsToggle.SetActive(false);
      VarManager.AddListener("devtoolsenabled", (VarManager.VarChangeHandler) (s => menuOptionsToggle.SetActive(bool.Parse(s))));
      int totalHeight = 220;
      style1.spritePos = TextureManager.GetSpritePos("ui_play");
      int playButtonWidth = (int) (40.0 * ((double) style1.spritePos.Width / (double) style1.spritePos.Height));
      this.AddChild(new Button(Rectangle.Empty, new Button.OnClick(this.OnPlayButtonPress), style1).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 - playButtonWidth / 2, mainCenterHeight(parentRect) - totalHeight / 2, playButtonWidth, 40))));
      style1.spritePos = TextureManager.GetSpritePos("ui_leveleditor");
      int levelEditorButtonWidth = (int) (40.0 * ((double) style1.spritePos.Width / (double) style1.spritePos.Height));
      this.AddChild(new Button(Rectangle.Empty, (Button.OnClick) (() => StateManager.TransitionTo(GameState.LevelEditor)), style1).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 - levelEditorButtonWidth / 2, mainCenterHeight(parentRect) - totalHeight / 2 + 40 + 50, levelEditorButtonWidth, 40))));
      style1.spritePos = TextureManager.GetSpritePos("ui_options");
      int optionsButtonWidth = (int) (40.0 * ((double) style1.spritePos.Width / (double) style1.spritePos.Height));
      this.AddChild(new Button(Rectangle.Empty, (Button.OnClick) (() => StateManager.TransitionTo(GameState.Options)), style1).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 - optionsButtonWidth / 2, mainCenterHeight(parentRect) - totalHeight / 2 + 180, optionsButtonWidth, 40))));
      styling1.enabledBorders = new BorderInfo(true);
      UIElement child3 = new UIElement(Rectangle.Empty, new Styling?(styling1)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - MainMenu.footerHeight, parentRect.Width, MainMenu.footerHeight)));
      style1.spritePos = TextureManager.GetSpritePos("ui_quit");
      int quitButtonWidth = (int) (40.0 * ((double) style1.spritePos.Width / (double) style1.spritePos.Height));
      child3.AddChild(new Button(Rectangle.Empty, new Button.OnClick(((Game) Game1.Instance).Exit), style1).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(40, MainMenu.footerHeight / 2 - 15, quitButtonWidth, 40))));
      style1.spritePos = TextureManager.GetSpritePos("ui_credits");
      int creditsButtonWidth = (int) (30.0 * ((double) style1.spritePos.Width / (double) style1.spritePos.Height));
      child3.AddChild(new Button(Rectangle.Empty, (Button.OnClick) (() => StateManager.TransitionTo(GameState.Credits)), style1).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width - 40 - creditsButtonWidth, MainMenu.footerHeight / 2 - 15, creditsButtonWidth, 30))));
      Styling style4 = new Styling()
      {
        defaultColor = color2,
        hoverColor = Color.Gray,
        tooltip = "Join the dedicated Discord server!\n(-> https://discord.gg/a62PN8BBEF)",
        clickColor = color1,
        round = true,
        spritePos = TextureManager.GetSpritePos("ui_discord")
      };
      int discordButtonWidth = (int) (30.0 * ((double) style4.spritePos.Width / (double) style4.spritePos.Height));
      child3.AddChild(new Button(Rectangle.Empty, (Button.OnClick) (() => Utility.OpenURL("https://discord.gg/a62PN8BBEF")), style4).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width - 40 - creditsButtonWidth - 20 - discordButtonWidth, MainMenu.footerHeight / 2 - 15, discordButtonWidth, 30))));
      Vector2 warningSize = (Vector2) font2.MeasureStringHalf("Could not connect to server... Try again?");
      child3.AddChild((UIElement) (this.noConnectionWarningElement = new Button(Rectangle.Empty, (Button.OnClick) (() =>
      {
        this.noConnectionWarningElement.SetText("Trying to connect to server...");
        this.hasCheckedConnection = false;
        this.noConnectionWarningElement.SetClickable(false);
        HttpManager.Instance.CheckConnection();
      }), new Styling()
      {
        text = "Trying to connect to server...",
        defaultTextColor = Color.Orange,
        hoverColor = Color.DarkOrange,
        clickColor = color1,
        textColorEffects = true,
        centerText = true
      })));
      this.noConnectionWarningElement.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 - (int) warningSize.X / 2, parentRect.Height / 2 - (int) warningSize.Y / 2, (int) warningSize.X, (int) warningSize.Y)));
      this.noConnectionWarningElement.SetClickable(false);
      this.AddChild(child3);

      static int mainCenterHeight(Rectangle parentRect)
      {
        return MainMenu.headerHeight + (parentRect.Height - MainMenu.footerHeight - MainMenu.headerHeight) / 2;
      }
    }

    public void SetStatusInfo(string info) => this.menuOptionsStatusLine.SetInfoText(info);

    public void SetSavedCount(int count)
    {
      this.menuOptionsSavedCountLine.SetInfoText(count.ToString());
    }

    public void SetBotId(int id) => this.menuOptionsBotIdLine.SetInfoText(id.ToString());

    public void SetConnectedToServer(bool connected)
    {
      if (connected)
      {
        this.connectedInfoTime = 3f;
        this.noConnectionWarningElement.SetText("Successfully connected to server!");
        this.noConnectionWarningElement.SetClickable(false);
      }
      else
      {
        if (!this.hasCheckedConnection)
        {
          this.hasCheckedConnection = true;
          this.noConnectionWarningElement.SetText("Could not connect to server... Try again?");
          this.noConnectionWarningElement.SetClickable(true);
        }
        this.noConnectionWarningElement.SetActive(true);
      }
    }

    private void OnPlayButtonPress()
    {
      if (VarManager.GetBool("hasplayedintro"))
      {
        StateManager.TransitionTo(GameState.LevelSelection);
      }
      else
      {
        StateManager.TransitionTo(GameState.InLevel_Default, LevelManager.DefaultLevels[0]);
        VarManager.SetBool("hasplayedintro", true);
        VarManager.SaveOptions();
      }
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Game1
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

#nullable enable
namespace BotsAreStupid
{
  internal class Game1 : Game
  {
    private int windowWidth;
    private int windowHeight;
    private bool isFullscreen;
    private Point preFullscreenWindowPosition;
    private 
    #nullable disable
    SpriteBatch spriteBatch;
    private Color backgroundColor;
    private bool doScreenshot;
    private RenderTarget2D levelRender;
    private bool errorShutdown;
    private float shutdownCooldown;
    private Stopwatch watch = new Stopwatch();

    public static Game1 Instance { get; private set; }

    public GraphicsDeviceManager GraphicsDM { get; }

    public Matrix WindowMatrix { get; set; }

    public Game1(string[] args1)
    {
      Game1.Instance = this;
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
      CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
      CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
      VarManager varManager = new VarManager();
      Input input = new Input();
      DevToolManager devToolManager = new DevToolManager();
      ScoreManager scoreManager = new ScoreManager();
      this.GraphicsDM = new GraphicsDeviceManager((Game) this);
      this.GraphicsDM.HardwareModeSwitch = false;
      this.IsFixedTimeStep = true;
      this.GraphicsDM.SynchronizeWithVerticalRetrace = false;
      this.TargetElapsedTime = TimeSpan.FromSeconds(0.004999999888241291);
      this.GraphicsDM.GraphicsProfile = GraphicsProfile.HiDef;
      this.GraphicsDM.PreparingDeviceSettings += (EventHandler<PreparingDeviceSettingsEventArgs>) ((sender, e) =>
      {
        this.GraphicsDM.PreferMultiSampling = true;
        e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;
      });
      this.GraphicsDM.ApplyChanges();
      this.Window.AllowUserResizing = true;
      this.Window.ClientSizeChanged += (EventHandler<EventArgs>) ((sender, args2) => this.SetWindow(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height, this.isFullscreen));
    }

    protected override void Initialize()
    {
      string[] strArray = VarManager.GetString("windowsize").Split('x');
      this.SetWindow(int.Parse(strArray[0]), int.Parse(strArray[1]), VarManager.GetBool("fullscreen"));
      this.Content.RootDirectory = VarManager.GetString("contentdirectory");
      this.IsMouseVisible = true;
      this.backgroundColor = ColorManager.GetColor("darkslate");
      Input.Register(KeyBind.ResetGame, new InputAction.InputHandler(this.OnResetButtonPress), cooldown: 1f);
      Input.Register(KeyBind.ResetGame, new InputAction.InputHandler(this.OnResetButtonPress), cooldown: 1f, ignoreTextLock: true, requireDown: new KeyBind?(KeyBind.Control));
      VarManager.AddListener("viewscale", (VarManager.VarChangeHandler) (s => this.UpdateMatrices()));
      VarManager.AddListener("viewoffsetx", (VarManager.VarChangeHandler) (s => this.UpdateMatrices()));
      VarManager.AddListener("viewoffsety", (VarManager.VarChangeHandler) (s => this.UpdateMatrices()));
      base.Initialize();
    }

    protected override void LoadContent()
    {
      this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
      TextureManager textureManager = new TextureManager(this.Content);
      SoundManager soundManager = new SoundManager(this.Content);
      Directory.CreateDirectory(VarManager.GetString("customlevelsdirectory"));
      StateManager stateManager = new StateManager();
      this.CreateUI();
      this.levelRender = new RenderTarget2D(this.GraphicsDevice, Utility.VirtualWidth, Utility.VirtualHeight);
    }

    protected override void Update(GameTime gameTime)
    {
      try
      {
        float totalSeconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
        Input.Instance.Update(totalSeconds);
        VarManager.Instance.Update(totalSeconds);
        SoundManager.Instance.Update(totalSeconds);
        StateManager.Instance.Update(totalSeconds);
        if (this.errorShutdown)
        {
          if ((double) this.shutdownCooldown <= 0.0)
          {
            this.Exit();
            return;
          }
          this.shutdownCooldown -= totalSeconds;
          UIElement.UpdateAll(totalSeconds);
          return;
        }
        int num = VarManager.HasInt("overclock") ? VarManager.GetInt("overclock") : 1;
        for (int index = 0; index < num; ++index)
          SimulationManager.Update(totalSeconds);
        UIElement.UpdateAll(totalSeconds);
      }
      catch (Exception ex)
      {
        if (!this.errorShutdown)
        {
          PopupMenu.Instance.Inform("Error!   Shutting down...*'" + ex.Message + "'*Check error.txt for details!", new PromptMenu.InteractionHandler(((Game) this).Exit), false);
          this.errorShutdown = true;
          this.shutdownCooldown = 7f;
          Utility.LogError(ex);
        }
      }
      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      Color color = VarManager.GetBool("phaseIV") ? Color.LightYellow : this.backgroundColor;
      if (this.errorShutdown)
      {
        this.GraphicsDevice.Clear(color);
        this.spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, SamplerState.AnisotropicClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, new Matrix?(this.WindowMatrix));
        PopupMenu.Instance.Draw(this.spriteBatch);
        this.spriteBatch.End();
        base.Draw(gameTime);
      }
      else
      {
        this.GraphicsDevice.SetRenderTarget(this.levelRender);
        this.GraphicsDevice.Clear(ClearOptions.Target, color, 1f, 0);
        StateManager.Instance.DrawLevel(this.spriteBatch);
        if (this.doScreenshot)
          this.CaptureScreenshot();
        this.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
        this.GraphicsDevice.Clear(color);
        this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, new Matrix?(StateManager.LevelViewMatrix));
        this.spriteBatch.Draw((Texture2D) this.levelRender, Vector2.Zero, new Rectangle?(), Color.White);
        Utility.DrawOutline(this.spriteBatch, new Rectangle(0, 0, this.levelRender.Width, this.levelRender.Height), 4, ColorManager.GetColor("white"), 1f);
        this.spriteBatch.End();
        StateManager.Instance.DrawUI(this.spriteBatch, SamplerState.AnisotropicClamp);
        base.Draw(gameTime);
      }
    }

    public static (int, int) GetWindowSize(bool transformed = false)
    {
      if (!transformed)
        return (Game1.Instance.windowWidth, Game1.Instance.windowHeight);
      Vector2 vector2 = Vector2.Transform(new Vector2((float) Game1.Instance.windowWidth, (float) Game1.Instance.windowHeight), Matrix.Invert(Game1.Instance.WindowMatrix));
      return ((int) vector2.X, (int) vector2.Y);
    }

    public static void ToggleFullscreen()
    {
      bool enableFullscreen = !VarManager.GetBool("fullscreen");
      string[] strArray = VarManager.GetString("windowsize").Split('x');
      Game1.Instance.SetWindow(int.Parse(strArray[0]), int.Parse(strArray[1]), enableFullscreen);
      VarManager.SetBool("fullscreen", enableFullscreen);
      VarManager.SaveOptions();
    }

    public void SetWindow(int width, int height, bool enableFullscreen)
    {
      DisplayMode currentDisplayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
      float val2_1 = (float) ((double) Utility.VirtualWidth / 3.0 * 2.0);
      float val2_2 = (float) Utility.VirtualHeight / 3f;
      float num1 = Math.Max((float) height, val2_2);
      float num2 = Math.Max((float) width, val2_1);
      if (enableFullscreen == this.isFullscreen && (double) this.windowWidth == (double) num2 && (double) this.windowHeight == (double) num1 && (this.isFullscreen || (double) width >= (double) val2_1 && (double) height >= (double) val2_2))
        return;
      this.windowWidth = (int) num2;
      this.windowHeight = (int) num1;
      bool isFullscreen = this.isFullscreen;
      this.isFullscreen = enableFullscreen;
      Point position = this.Window.Position;
      this.GraphicsDM.PreferredBackBufferWidth = this.windowWidth;
      this.GraphicsDM.PreferredBackBufferHeight = this.windowHeight;
      this.GraphicsDM.ApplyChanges();
      this.GraphicsDM.IsFullScreen = enableFullscreen;
      this.GraphicsDM.ApplyChanges();
      if (isFullscreen)
      {
        if (!enableFullscreen)
        {
          this.Window.Position = this.preFullscreenWindowPosition;
          Point fullscreenWindowPosition = this.preFullscreenWindowPosition;
        }
      }
      else if (enableFullscreen)
      {
        this.preFullscreenWindowPosition = position;
        this.Window.Position = new Point(position.X, position.Y + 5);
      }
      this.Window.IsBorderless = this.isFullscreen;
      this.UpdateMatrices();
    }

    public void SetWindowPos(int x, int y) => this.Window.Position = new Point(x, y);

    public void OnResetButtonPress()
    {
      if (!SimulationManager.HasLockedFrame && Utility.MainPlayer != null)
        Utility.MainPlayer.Kill(true);
      else
        StateManager.Reset(true);
    }

    public void UpdateMatrices()
    {
      StateManager.Instance?.UpdateCurrentMatrices((float) this.windowWidth, (float) this.windowHeight);
    }

    private void CreateUI()
    {
      Rectangle rectangle = new Rectangle(0, 0, Utility.VirtualWidth, Utility.VirtualHeight);
      StateManager.Instance.CreateUI(rectangle);
      PopupMenu popupMenu = new PopupMenu(rectangle);
      TextEditor textEditor = new TextEditor(rectangle);
      LevelEditor levelEditor = new LevelEditor(rectangle);
      ScoreExplorer scoreExplorer = new ScoreExplorer(rectangle);
      Styling buttonDisabledStyling = Styling.DefaultToggleButtonDisabledStyling with
      {
        isLevelUi = true,
        drawInState = GameState.InLevel_Any
      };
      new Button(new Rectangle(rectangle.Width - 30, 10, 20, 20), new Button.OnClick(((UIElement) PopupMenu.Instance).ToggleActive), buttonDisabledStyling).AddChild(new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p, 0.9f))));
      UIElement.InitializeAll();
      ColorManager.SetCurrentScheme(VarManager.GetString("colorscheme"));
    }

    public void CaptureScreenshot()
    {
      if (!this.doScreenshot)
      {
        this.doScreenshot = true;
      }
      else
      {
        this.doScreenshot = false;
        int width = Utility.VirtualWidth - 340;
        Color[] data1 = new Color[Utility.VirtualWidth * Utility.VirtualHeight];
        this.levelRender.GetData<Color>(data1);
        Color[] data2 = new Color[width * Utility.VirtualHeight];
        int index1 = 0;
        for (int index2 = 0; index2 < data1.Length; ++index2)
        {
          if (data1[index2] == Color.Black)
            data1[index2] = this.backgroundColor;
          if (index2 % Utility.VirtualWidth >= 340)
          {
            data2[index1] = data1[index2];
            ++index1;
          }
        }
        Texture2D texture2D = new Texture2D(this.GraphicsDevice, width, Utility.VirtualHeight);
        texture2D.SetData<Color>(data2);
        using (FileStream fileStream = new FileStream(VarManager.GetString("appdirectory") + "screenshot.png", (FileMode) 4))
          texture2D.SaveAsPng((Stream) fileStream, width, Utility.VirtualHeight);
        this.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      }
    }

    public void CreateGif(string levelName, string[] instructions, string playerName = null)
    {
      Simulation simulation = SimulationManager.CreateSimulation(out bool _, "gif", instructions, playerName, levelName: levelName);
      simulation.ForceRenderAll = true;
      RenderTarget2D renderTarget = new RenderTarget2D(this.GraphicsDevice, Utility.VirtualWidth, Utility.VirtualHeight);
      this.GraphicsDevice.SetRenderTarget(renderTarget);
      int num1 = 0;
      int num2 = 0;
      float deltaTime = 0.005f;
      string str = VarManager.GetString("appdirectory") + "gif/";
      Directory.CreateDirectory(str);
      SpriteBatch spriteBatch = new SpriteBatch(this.GraphicsDevice);
      while (!simulation.IsFinished)
      {
        if (simulation.CanBeStarted)
          simulation.StartASAP();
        simulation.Update(deltaTime);
        if (num1 % 10 == 0)
        {
          this.GraphicsDevice.Clear(this.backgroundColor);
          spriteBatch.Begin(SpriteSortMode.FrontToBack, (BlendState) null, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, new Matrix?(this.WindowMatrix));
          simulation.Draw(spriteBatch);
          spriteBatch.End();
          using (FileStream fileStream = new FileStream(str + "frame" + num2.ToString() + ".png", (FileMode) 4))
            renderTarget.SaveAsPng((Stream) fileStream, Utility.VirtualWidth, Utility.VirtualHeight);
          ++num2;
        }
        ++num1;
      }
      this.GraphicsDevice.SetRenderTarget((RenderTarget2D) null);
      SimulationManager.RemoveSimulation(simulation);
      spriteBatch.Dispose();
      renderTarget.Dispose();
      Console.WriteLine("Gif Done!");
    }
  }
}

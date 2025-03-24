// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.LevelEditor
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace BotsAreStupid
{
  internal class LevelEditor : BasicLevelUI
  {
    private const int tabButtonHeight = 50;
    private const int dragToPlaceInfoHeight = 100;
    private const int playButtonHeight = 70;
    private const int bottomButtonHeight = 50;
    private const int topBarButtonHeight = 30;
    private const int padding = 5;
    private const int borderWidth = 4;
    private const int spawnBoxSize = 60;
    private const int spawnBoxMargin = 30;
    private const int selectedCornerSize = 14;
    private const int borderHoverSize = 14;
    private const int sidebarButtonSize = 80;
    private const int sidebarButtonPadding = 20;
    private const int gridToggleButtonWidth = 75;
    private const int gridScaleChangeButtonWidth = 50;
    private const int gridSnapToggleButtonWidth = 80;
    private const int undoRedoButtonSize = 30;
    private const int controlsToggleButtonWidth = 110;
    private const float parameterMenuOpacity = 0.75f;
    private const int parameterMenuWidth = 150;
    private const int parameterMenuPadding = 15;
    private const int parameterMenuSliderHeight = 50;
    private const int parameterMenuSliderAmount = 3;
    private const int parameterMenuOuterMargin = 10;
    private const int parameterMenuToggleSize = 20;
    private new Vector2 lastMousePos;
    private Vector2 mouseDragStart;
    private bool isMouseDragging;
    private List<GameObject> selectedObjects = new List<GameObject>();
    private bool isMoving;
    private Vector2 beforeMovePosition;
    private Vector2 selectionMouseOffset;
    private bool isScaling;
    private Vector2 beforeScalePosition;
    private Vector2 beforeScaleSize;
    private int scaleAxisLock = -1;
    private int scaleCornerId;
    private bool isDuplicateScaling;
    private int activeSpawnerTab;
    private Button[] spawnTabs;
    private GridContainer[] spawnContainer;
    private ContentToggleButton parameterMenuToggle;
    private Flexbox parameterMenu;
    private DisplaySlider[] parameterMenuSliders;
    private UIElement levelNameDisplay;
    private UIElement sideBar;
    private ToggleButton gridToggleButton;
    private Button gridChangeScaleButton;
    private ToggleButton gridSnapToggleButton;
    private Button undoButton;
    private Button redoButton;
    private Button saveButton;
    private Stack<EditorAction> undoActions = new Stack<EditorAction>();
    private Stack<EditorAction> redoActions = new Stack<EditorAction>();
    private GameObject[] clipboard;

    public static LevelEditor Instance { get; private set; }

    public bool HasAutoSaved { get; private set; }

    public bool HasChanges { get; private set; }

    private bool hasSelectedObjects => this.selectedObjects.Count > 0;

    public LevelEditor(Rectangle rectangle)
      : base(rectangle, GameState.LevelEditor)
    {
      LevelEditor.Instance = this;
      this.RegisterInput();
      this.CreateChildren();
      this.SetSpawnerTab("0");
    }

    private void RegisterInput()
    {
      BotsAreStupid.Input.RegisterMouse(InputEvent.OnDown, new InputAction.InputHandler(this.OnMouseDown), GameState.LevelEditor);
      BotsAreStupid.Input.RegisterMouse(InputEvent.OnDoubleDown, new InputAction.InputHandler(this.OnMouseDoubleDown), GameState.LevelEditor);
      BotsAreStupid.Input.RegisterMouse(InputEvent.OnDown, new InputAction.InputHandler(this.OnRightMouseDown), GameState.LevelEditor, MouseButton.Right);
      BotsAreStupid.Input.RegisterMouse(InputEvent.OnUp, new InputAction.InputHandler(this.OnMouseUp), GameState.LevelEditor);
      BotsAreStupid.Input.RegisterMouse(InputEvent.OnUp, new InputAction.InputHandler(this.OnRightMouseUp), GameState.LevelEditor, MouseButton.Right);
      BotsAreStupid.Input.RegisterOnDown(KeyBind.GoRight, new InputAction.InputHandler(this.DuplicateSelected), GameState.LevelEditor, requireDown: new KeyBind?(KeyBind.Control));
      BotsAreStupid.Input.RegisterOnDown(KeyBind.DestroyObject, new InputAction.InputHandler(this.DeleteSelected), GameState.LevelEditor);
      BotsAreStupid.Input.RegisterOnDown(KeyBind.Save, (InputAction.InputHandler) (() =>
      {
        if (this.HasChanges)
          this.PlayInteractionSound();
        this.OnSaveButtonClick();
      }), GameState.LevelEditor, requireDown: new KeyBind?(KeyBind.Control));
      BotsAreStupid.Input.RegisterOnDown(KeyBind.New, new InputAction.InputHandler(this.OnNewButtonClick), GameState.LevelEditor, requireDown: new KeyBind?(KeyBind.Control));
      BotsAreStupid.Input.RegisterOnDown(KeyBind.Enter, new InputAction.InputHandler(this.PlayLevel), GameState.LevelEditor, requireDown: new KeyBind?(KeyBind.Alt));
      BotsAreStupid.Input.Register(KeyBind.Cancel, (InputAction.InputHandler) (() =>
      {
        this.CopySelected();
        this.PlayInteractionSound();
      }), GameState.LevelEditor, requireDown: new KeyBind?(KeyBind.Control));
      BotsAreStupid.Input.Register(KeyBind.SpawnBooster, (InputAction.InputHandler) (() =>
      {
        this.CutSelected();
        this.PlayInteractionSound();
      }), GameState.LevelEditor, requireDown: new KeyBind?(KeyBind.Control));
      BotsAreStupid.Input.Register(KeyBind.SpawnExplosion, (InputAction.InputHandler) (() =>
      {
        this.Paste();
        this.PlayInteractionSound();
      }), GameState.LevelEditor, requireDown: new KeyBind?(KeyBind.Control));
      BotsAreStupid.Input.Register(KeyBind.Up, new InputAction.InputHandler(this.MoveSelectedUp), GameState.LevelEditor, 0.02f);
      BotsAreStupid.Input.Register(KeyBind.Down, new InputAction.InputHandler(this.MoveSelectedDown), GameState.LevelEditor, 0.02f);
      BotsAreStupid.Input.Register(KeyBind.Left, new InputAction.InputHandler(this.MoveSelectedLeft), GameState.LevelEditor, 0.02f);
      BotsAreStupid.Input.Register(KeyBind.Right, new InputAction.InputHandler(this.MoveSelectedRight), GameState.LevelEditor, 0.02f);
      BotsAreStupid.Input.RegisterOnDown(KeyBind.UndoRedo, new InputAction.InputHandler(this.UndoRedoHandler), GameState.LevelEditor);
      BotsAreStupid.Input.RegisterOnDown(KeyBind.Control, (InputAction.InputHandler) (() => this.gridSnapToggleButton.SetEnabled(!this.gridSnapToggleButton.IsEnabled)), GameState.LevelEditor);
      BotsAreStupid.Input.RegisterOnUp(KeyBind.Control, (InputAction.InputHandler) (() => this.gridSnapToggleButton.SetEnabled(!this.gridSnapToggleButton.IsEnabled)), GameState.LevelEditor);
    }

    private void CreateChildren()
    {
      ColorManager.GetColor("red");
      Color color1 = ColorManager.GetColor("white");
      ColorManager.GetColor("darkslate");
      Color color2 = ColorManager.GetColor("lightslate");
      BitmapFont font = TextureManager.GetFont("megaMan2Small");
      Vector2 charSize = (Vector2) font.MeasureStringHalf("a");
      Vector2 virtualSize = Utility.VirtualSize;
      this.AddChild(new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - 100 - 5, parentRect.Width, 100))));
      Styling style1 = Styling.DefaultButtonStyling with
      {
        tooltip = "Rotate selected object(s)"
      };
      Button button1 = new Button(Rectangle.Empty, new Button.OnClick(this.RotateSelected), style1);
      Button button2 = button1;
      Rectangle empty1 = Rectangle.Empty;
      Styling styling1 = new Styling();
      styling1.spritePos = TextureManager.GetSpritePos("ui_rotate");
      styling1.defaultColor = color1;
      Styling? style2 = new Styling?(styling1);
      UIElement child1 = new UIElement(empty1, style2).SetResponsiveRectangle(new UIElement.ResponsiveRectangleHandler(buttonIconRect));
      button2.AddChild(child1);
      style1.tooltip = "Duplicate selected object(s)\nShortcut: Control + D";
      Button button3 = new Button(Rectangle.Empty, new Button.OnClick(this.DuplicateSelected), style1);
      Button button4 = button3;
      Rectangle empty2 = Rectangle.Empty;
      styling1 = new Styling();
      styling1.spritePos = TextureManager.GetSpritePos("ui_duplicate");
      styling1.defaultColor = color1;
      Styling? style3 = new Styling?(styling1);
      UIElement child2 = new UIElement(empty2, style3).SetResponsiveRectangle(new UIElement.ResponsiveRectangleHandler(buttonIconRect));
      button4.AddChild(child2);
      style1.tooltip = "Delete selected object(s)\nShortcut: Backspace";
      Button button5 = new Button(Rectangle.Empty, new Button.OnClick(this.DeleteSelected), style1);
      button5.AddChild(new UIElement(Rectangle.Empty).SetResponsiveRectangle(new UIElement.ResponsiveRectangleHandler(buttonIconRect)));
      this.CreateSpawner();
      this.sideBar = new UIElement(new Rectangle(0, 0, 50, Utility.VirtualHeight));
      GridContainer child3 = new GridContainer(Rectangle.Empty, 80, 80, 20, 20, forceSquareElements: true);
      child3.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, parentRect.Height / 2)));
      child3.SetElements((UIElement) button1, (UIElement) button3, (UIElement) button5);
      this.sideBar.AddChild((UIElement) child3);
      style1.enabledBorders = new BorderInfo(true);
      style1.tooltip = (string) null;
      style1.text = "Save";
      UIElement sideBar1 = this.sideBar;
      Rectangle rectangle1 = new Rectangle(0, this.rectangle.Height - 50, this.rectangle.Width, 50);
      Button.OnClick onClick1 = new Button.OnClick(this.OnSaveButtonClick);
      Styling style4 = style1;
      Styling styling2 = new Styling();
      styling2.useTextRegex = true;
      styling2.tooltip = "Shortcut: Control + S";
      Styling? newStyleNullable1 = new Styling?(styling2);
      Styling style5 = Styling.AddTo(style4, newStyleNullable1);
      Button child4 = this.saveButton = new Button(rectangle1, onClick1, style5);
      sideBar1.AddChild((UIElement) child4);
      this.saveButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - 150 - 70, parentRect.Width, 50)));
      style1.text = "Load";
      this.sideBar.AddChild(new Button(new Rectangle(0, this.rectangle.Height - 50, this.rectangle.Width, 50), new Button.OnClick(this.OnLoadButtonClick), style1).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - 100 - 70, parentRect.Width, 50))));
      style1.text = "New";
      UIElement sideBar2 = this.sideBar;
      Rectangle rectangle2 = new Rectangle(0, this.rectangle.Height - 50, this.rectangle.Width, 50);
      Button.OnClick onClick2 = new Button.OnClick(this.OnNewButtonClick);
      Styling style6 = style1;
      styling2 = new Styling();
      styling2.tooltip = "Shortcut: Control + N";
      Styling? newStyleNullable2 = new Styling?(styling2);
      Styling style7 = Styling.AddTo(style6, newStyleNullable2);
      UIElement child5 = new Button(rectangle2, onClick2, style7).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - 50 - 70, parentRect.Width, 50)));
      sideBar2.AddChild(child5);
      style1 = Styling.RedButtonStyling with
      {
        enabledBorders = new BorderInfo(true),
        text = "Play"
      };
      UIElement sideBar3 = this.sideBar;
      Rectangle empty3 = Rectangle.Empty;
      Button.OnClick onClick3 = new Button.OnClick(this.PlayLevel);
      Styling style8 = style1;
      styling2 = new Styling();
      styling2.tooltip = "Shortcut: Alt + Enter";
      Styling? newStyleNullable3 = new Styling?(styling2);
      Styling style9 = Styling.AddTo(style8, newStyleNullable3);
      UIElement child6 = new Button(empty3, onClick3, style9).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - 70, parentRect.Width, 70)));
      sideBar3.AddChild(child6);
      Rectangle rectangle3 = new Rectangle(0, 0, 150, 80);
      styling2 = new Styling();
      styling2.defaultColor = new Color(color2, 0.75f);
      styling2.borderColor = color1;
      styling2.borderWidth = 2;
      styling2.padding = 15;
      Styling? style10 = new Styling?(styling2);
      Point? center = new Point?();
      this.parameterMenu = new Flexbox(rectangle3, style10, center);
      styling2 = new Styling();
      styling2.sliderThumbScale = 0.5f;
      //styling2.font = font;
      styling2.sliderHeight = 5;
      styling2.rightText = true;
      Styling styling3 = styling2;
      this.parameterMenuSliders = new DisplaySlider[3];
      for (int index = 0; index < 3; ++index)
        this.parameterMenu.Add((UIElement) (this.parameterMenuSliders[index] = new DisplaySlider(new Rectangle(15, 15 * (index + 1) + 50 * index, 120, 50), this.GetSliderChangeHandler(index), "Val" + index.ToString() + ": ", 1, 10, 5, style: new Styling?(styling3))));
      Rectangle rectangle4 = new Rectangle(0, 0, 20, 20);
      Flexbox parameterMenu = this.parameterMenu;
      ContentToggleButton.EnableHandler enableHandler = new ContentToggleButton.EnableHandler(this.UpdateParameterMenu);
      styling2 = new Styling();
      //styling2.font = font;
      styling2.isLevelUi = true;
      styling2.drawInState = GameState.LevelEditor;
      Styling? buttonStyle = new Styling?(styling2);
      Styling? buttonEnabledStyle = new Styling?();
      this.parameterMenuToggle = new ContentToggleButton(rectangle4, (UIElement) parameterMenu, enableHandler, buttonStyle: buttonStyle, buttonEnabledStyle: buttonEnabledStyle);
      this.parameterMenuToggle.SetActive(false);
      this.parameterMenuToggle.AddChild((UIElement) this.parameterMenu);
      Rectangle empty4 = Rectangle.Empty;
      styling2 = new Styling();
      styling2.defaultTextColor = color1;
      //styling2.font = font;
      styling2.centerText = true;
      Styling? style11 = new Styling?(styling2);
      this.AddChild(this.levelNameDisplay = new UIElement(empty4, style11));
      this.levelNameDisplay.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect =>
      {
        Rectangle uiSpaceLevelRect = StateManager.UISpaceLevelRect;
        return new Rectangle(uiSpaceLevelRect.X, uiSpaceLevelRect.Y + uiSpaceLevelRect.Height + (int) charSize.Y, uiSpaceLevelRect.Width, (int) charSize.Y);
      }));
      Styling defaultButtonStyling1 = Styling.DefaultButtonStyling;
      styling2 = new Styling();
      //styling2.font = font;
      styling2.text = "Grid: Off";
      styling2.borderWidth = 2;
      Styling? newStyleNullable4 = new Styling?(styling2);
      Styling style12 = Styling.AddTo(defaultButtonStyling1, newStyleNullable4);
      Styling redButtonStyling = Styling.RedButtonStyling;
      styling2 = new Styling();
      //styling2.font = font;
      styling2.text = "Grid: On";
      styling2.borderWidth = 2;
      Styling? newStyleNullable5 = new Styling?(styling2);
      Styling enabledStyle = Styling.AddTo(redButtonStyling, newStyleNullable5);
      this.gridToggleButton = new ToggleButton(Rectangle.Empty, new Button.OnClick(this.OnGridToggleButtonPress), new Button.OnClick(this.OnGridToggleButtonPress), style12, enabledStyle, true);
      this.gridToggleButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect =>
      {
        Rectangle uiSpaceLevelRect = StateManager.UISpaceLevelRect;
        return new Rectangle(uiSpaceLevelRect.X, uiSpaceLevelRect.Y - 30 - 2, 75, 30);
      }));
      this.AddChild((UIElement) this.gridToggleButton);
      int gridScale = 0;
      Rectangle empty5 = Rectangle.Empty;
      Button.OnClick onClick4 = (Button.OnClick) (() =>
      {
        gridScale = (gridScale + 1) % 5;
        switch (gridScale)
        {
          case 0:
            this.gridChangeScaleButton.SetText("1.0X");
            EditorGrid.CellSize = 28;
            break;
          case 1:
            this.gridChangeScaleButton.SetText("0.5X");
            EditorGrid.CellSize = 14;
            break;
          case 2:
            this.gridChangeScaleButton.SetText("0.25X");
            EditorGrid.CellSize = 7;
            break;
          case 3:
            this.gridChangeScaleButton.SetText("4.0X");
            EditorGrid.CellSize = 112;
            break;
          case 4:
            this.gridChangeScaleButton.SetText("2.0X");
            EditorGrid.CellSize = 56;
            break;
        }
      });
      Styling style13 = style12;
      styling2 = new Styling();
      styling2.text = "1.0X";
      Styling? newStyleNullable6 = new Styling?(styling2);
      Styling style14 = Styling.AddTo(style13, newStyleNullable6);
      this.gridChangeScaleButton = new Button(empty5, onClick4, style14);
      this.gridChangeScaleButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect =>
      {
        Rectangle uiSpaceLevelRect = StateManager.UISpaceLevelRect;
        return new Rectangle(uiSpaceLevelRect.X + 75, uiSpaceLevelRect.Y - 30 - 2, 50, 30);
      }));
      this.AddChild((UIElement) this.gridChangeScaleButton);
      style12.text = "Snap: Off";
      enabledStyle.text = "Snap: On";
      style12.tooltip = "Shortcut: Control";
      enabledStyle.tooltip = style12.tooltip;
      this.gridSnapToggleButton = new ToggleButton(Rectangle.Empty, (Button.OnClick) (() => EditorGrid.SetSnappingEnabled(true)), (Button.OnClick) (() => EditorGrid.SetSnappingEnabled(false)), style12, enabledStyle, EditorGrid.Instance.SnappingEnabled);
      this.gridSnapToggleButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect =>
      {
        Rectangle uiSpaceLevelRect = StateManager.UISpaceLevelRect;
        return new Rectangle(uiSpaceLevelRect.X + 75 + 50, uiSpaceLevelRect.Y - 30 - 2, 80, 30);
      }));
      this.AddChild((UIElement) this.gridSnapToggleButton);
      Rectangle empty6 = Rectangle.Empty;
      Button.OnClick onClick5 = new Button.OnClick(this.Undo);
      Styling defaultButtonStyling2 = Styling.DefaultButtonStyling;
      styling2 = new Styling();
      styling2.text = "<";
      styling2.borderWidth = 2;
      styling2.interactableIgnoreBorder = true;
      Styling? newStyleNullable7 = new Styling?(styling2);
      Styling style15 = Styling.AddTo(defaultButtonStyling2, newStyleNullable7);
      this.undoButton = new Button(empty6, onClick5, style15, true);
      this.undoButton.SetInteractable(false);
      this.undoButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect =>
      {
        Rectangle uiSpaceLevelRect = StateManager.UISpaceLevelRect;
        return new Rectangle(uiSpaceLevelRect.X + uiSpaceLevelRect.Width - 60, uiSpaceLevelRect.Y - 30 - 2, 30, 30);
      }));
      this.AddChild((UIElement) this.undoButton);
      Rectangle empty7 = Rectangle.Empty;
      Button.OnClick onClick6 = new Button.OnClick(this.Redo);
      Styling defaultButtonStyling3 = Styling.DefaultButtonStyling;
      styling2 = new Styling();
      styling2.text = ">";
      styling2.borderWidth = 2;
      styling2.interactableIgnoreBorder = true;
      Styling? newStyleNullable8 = new Styling?(styling2);
      Styling style16 = Styling.AddTo(defaultButtonStyling3, newStyleNullable8);
      this.redoButton = new Button(empty7, onClick6, style16, true);
      this.redoButton.SetInteractable(false);
      this.redoButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect =>
      {
        Rectangle uiSpaceLevelRect = StateManager.UISpaceLevelRect;
        return new Rectangle(uiSpaceLevelRect.X + uiSpaceLevelRect.Width - 30, uiSpaceLevelRect.Y - 30 - 2, 30, 30);
      }));
      this.AddChild((UIElement) this.redoButton);
      style12.text = "Controls: Off";
      enabledStyle.text = "Controls: On";
      style12.tooltip = (string) null;
      enabledStyle.tooltip = (string) null;
      ToggleButton controlsToggleButton = new ToggleButton(Rectangle.Empty, new Button.OnClick(this.EnableManualControls), new Button.OnClick(this.DisableManualControls), style12, enabledStyle);
      controlsToggleButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect =>
      {
        Rectangle uiSpaceLevelRect = StateManager.UISpaceLevelRect;
        return new Rectangle(uiSpaceLevelRect.X, uiSpaceLevelRect.Y + uiSpaceLevelRect.Height + 2, 110, 30);
      }));
      VarManager.AddListener("manualControls", (VarManager.VarChangeHandler) (s => controlsToggleButton.SetEnabled(bool.Parse(s))));
      this.AddChild((UIElement) controlsToggleButton);

      static Rectangle buttonIconRect(Rectangle parentRect)
      {
        return Utility.ScaledCenteredRect(parentRect, 0.4f, useHeightOnly: true);
      }
    }

    private void CreateSpawner()
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = ColorManager.GetColor("white");
      Color color3 = ColorManager.GetColor("darkslate");
      Color color4 = ColorManager.GetColor("lightslate");
      Styling style = new Styling()
      {
        defaultColor = color3,
        hoverColor = color4,
        clickColor = color1,
        borderColor = color2,
        borderWidth = 4,
        //font = TextureManager.GetFont("megaMan2Small"),
        defaultTextColor = color2,
        centerText = true,
        textOffset = 4,
        enabledBorders = new BorderInfo(right: true, bottom: true)
      };
      this.spawnTabs = new Button[3];
      style.text = "Platforms";
      style.value = "0";
      this.spawnTabs[0] = new Button(Rectangle.Empty, new Button.OnClickString(this.SetSpawnerTab), style);
      this.spawnTabs[0].SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width / 3, 50)));
      style.enabledBorders.left = true;
      style.text = "Background";
      style.value = "1";
      this.spawnTabs[1] = new Button(Rectangle.Empty, new Button.OnClickString(this.SetSpawnerTab), style);
      this.spawnTabs[1].SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(this.rectangle.Width / 3, 0, parentRect.Width / 3, 50)));
      style.enabledBorders.right = false;
      style.text = "Special";
      style.value = "2";
      this.spawnTabs[2] = new Button(Rectangle.Empty, new Button.OnClickString(this.SetSpawnerTab), style);
      this.spawnTabs[2].SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(2 * this.rectangle.Width / 3, 0, parentRect.Width / 3 - 1, 50)));
      this.AddChild((UIElement) this.spawnTabs[0]);
      this.AddChild((UIElement) this.spawnTabs[1]);
      this.AddChild((UIElement) this.spawnTabs[2]);
      Simulation mainSimulation = SimulationManager.MainSimulation;
      this.spawnContainer = new GridContainer[3];
      Texture2D texture = TextureManager.GetTexture("tileset");
      this.spawnContainer[0] = this.CreateSpawnContainer((GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 28, 14, color4), (GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 196, 28, Color.White, texture, new Rectangle?(new Rectangle(96, 16, 96, 16)), 0.1f), (GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 56, 28, Color.White, texture, new Rectangle?(new Rectangle(65, 16, 30, 16)), 0.1f), (GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 56, 28, Color.White, texture, new Rectangle?(new Rectangle(33, 16, 30, 16)), 0.1f), (GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 56, 28, Color.White, texture, new Rectangle?(new Rectangle(1, 16, 30, 16)), 0.1f), (GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 56, 28, Color.White, texture, new Rectangle?(new Rectangle(352, 48, 32, 16)), 0.3f), (GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 56, 14, Color.White, texture, new Rectangle?(new Rectangle(352, 48, 32, 5))), (GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 14, 28, Color.White, texture, new Rectangle?(new Rectangle(324, 48, 8, 16))), (GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 28, 28, Color.White, texture, new Rectangle?(new Rectangle(80, 0, 16, 16))), (GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 28, 28, Color.White, texture, new Rectangle?(new Rectangle(320, 16, 16, 16)), 0.2f), (GameObject) new Platform(mainSimulation, 0.0f, 0.0f, 28, 28, Color.White, texture, new Rectangle?(new Rectangle(193, 0, 15, 19)), 0.85f));
      this.spawnContainer[1] = this.CreateSpawnContainer((GameObject) new BackgroundObject(0.0f, 0.0f, 24, 32, texture, new Rectangle?(new Rectangle(130, 0, 12, 16))), (GameObject) new BackgroundObject(0.0f, 0.0f, 24, 16, texture, new Rectangle?(new Rectangle(354, 14, 12, 8))), (GameObject) new BackgroundObject(0.0f, 0.0f, 24, 28, texture, new Rectangle?(new Rectangle(354, 0, 12, 14))), (GameObject) new BackgroundObject(0.0f, 0.0f, 30, 34, texture, new Rectangle?(new Rectangle(221, 144, 15, 17))), (GameObject) new BackgroundObject(0.0f, 0.0f, 20, 34, texture, new Rectangle?(new Rectangle(227, 128, 10, 17))), (GameObject) new BackgroundObject(0.0f, 0.0f, 32, 36, texture, new Rectangle?(new Rectangle(224, 112, 16, 18))), (GameObject) new BackgroundObject(0.0f, 0.0f, 26, 26, texture, new Rectangle?(new Rectangle(207, 116, 13, 13))), (GameObject) new BackgroundObject(0.0f, 0.0f, 24, 34, texture, new Rectangle?(new Rectangle(242, 160, 12, 17))), (GameObject) new BackgroundObject(0.0f, 0.0f, 20, 30, texture, new Rectangle?(new Rectangle(242, 177, 10, 15))), (GameObject) new BackgroundObject(0.0f, 0.0f, 64, 64, texture, new Rectangle?(new Rectangle(368, 0, 32, 32))), (GameObject) new BackgroundObject(0.0f, 0.0f, 20, 32, texture, new Rectangle?(new Rectangle(267, 0, 10, 16))), (GameObject) new BackgroundObject(0.0f, 0.0f, 196, 64, texture, new Rectangle?(new Rectangle(64, 96, 96, 32))), (GameObject) new BackgroundObject(0.0f, 0.0f, 128, 16, texture, new Rectangle?(new Rectangle(0, 8, 64, 8))), (GameObject) new BackgroundObject(0.0f, 0.0f, 26, 28, texture, new Rectangle?(new Rectangle(178, 66, 13, 14))), (GameObject) new BackgroundObject(0.0f, 0.0f, 8, 24, texture, new Rectangle?(new Rectangle(134, 50, 4, 12))), (GameObject) new BackgroundObject(0.0f, 0.0f, 20, 22, texture, new Rectangle?(new Rectangle(176, 38, 10, 11))), (GameObject) new BackgroundObject(0.0f, 0.0f, 20, 28, texture, new Rectangle?(new Rectangle(176, 49, 10, 14))), (GameObject) new BackgroundObject(0.0f, 0.0f, 4, 16, texture, new Rectangle?(new Rectangle(7, 56, 2, 8))), (GameObject) new BackgroundObject(0.0f, 0.0f, 8, 14, texture, new Rectangle?(new Rectangle(6, 65, 4, 7))), (GameObject) new BackgroundObject(0.0f, 0.0f, 82, 80, texture, new Rectangle?(new Rectangle(182, 168, 41, 40))), (GameObject) new BackgroundObject(0.0f, 0.0f, 56, 138, texture, new Rectangle?(new Rectangle(338, 75, 28, 69))), (GameObject) new BackgroundObject(0.0f, 0.0f, 12, 24, texture, new Rectangle?(new Rectangle(277, 64, 6, 12))), (GameObject) new BackgroundObject(0.0f, 0.0f, 60, 30, texture, new Rectangle?(new Rectangle(305, 129, 30, 15))), (GameObject) new BackgroundObject(0.0f, 0.0f, 60, 28, texture, new Rectangle?(new Rectangle(305, 114, 30, 14))), (GameObject) new BackgroundObject(0.0f, 0.0f, 60, 20, texture, new Rectangle?(new Rectangle(273, 118, 30, 10))), (GameObject) new BackgroundObject(0.0f, 0.0f, 64, 76, texture, new Rectangle?(new Rectangle(48, 35, 32, 38))), (GameObject) new BackgroundObject(0.0f, 0.0f, 24, 26, texture, new Rectangle?(new Rectangle(178, 115, 12, 13))), (GameObject) new BackgroundObject(0.0f, 0.0f, 24, 26, texture, new Rectangle?(new Rectangle(290, 67, 12, 13))), (GameObject) new BackgroundObject(0.0f, 0.0f, 24, 26, texture, new Rectangle?(new Rectangle(290, 83, 12, 13))), (GameObject) new BackgroundObject(0.0f, 0.0f, 24, 26, texture, new Rectangle?(new Rectangle(258, 67, 12, 13))), (GameObject) new BackgroundObject(0.0f, 0.0f, 24, 26, texture, new Rectangle?(new Rectangle(258, 83, 12, 13))), (GameObject) new BackgroundObject(0.0f, 0.0f, 24, 26, texture, new Rectangle?(new Rectangle(258, 99, 12, 13))), (GameObject) new BackgroundObject(0.0f, 0.0f, 8, 24, texture, new Rectangle?(new Rectangle(246, 84, 4, 12))), (GameObject) new BackgroundObject(0.0f, 0.0f, 24, 24, texture, new Rectangle?(new Rectangle(238, 70, 12, 12))), (GameObject) new BackgroundObject(0.0f, 0.0f, 28, 20, texture, new Rectangle?(new Rectangle(209, 86, 14, 10))), (GameObject) new BackgroundObject(0.0f, 0.0f, 18, 20, texture, new Rectangle?(new Rectangle(211, 70, 9, 10))));
      this.spawnContainer[1].SetActive(false);
      this.spawnContainer[2] = this.CreateSpawnContainer((GameObject) new EnergyOrb(mainSimulation, 0.0f, 0.0f), (GameObject) new Spike(mainSimulation, 0.0f, 0.0f), (GameObject) new Portal(mainSimulation, 0.0f, 0.0f), (GameObject) new Booster(mainSimulation, 0.0f, 0.0f), (GameObject) new SpawnPipe(mainSimulation, 0.0f, 0.0f), (GameObject) new ConveyorBelt(mainSimulation, 0.0f, 0.0f, new int?(2)), (GameObject) new Bouncer(mainSimulation, 0.0f, 0.0f), (GameObject) new MovingPlatform(mainSimulation, 0.0f, 0.0f), new GameObject(mainSimulation, 0.0f, 0.0f, 2, 2, new Color?(color3)), new GameObject(mainSimulation, 0.0f, 0.0f, 2, 2, new Color?(color3)));
      this.spawnContainer[2].SetActive(false);
      (this.spawnContainer[2].Elements[7] as ObjectSpawnBox).SetLocked();
      (this.spawnContainer[2].Elements[8] as ObjectSpawnBox).SetLocked();
      (this.spawnContainer[2].Elements[9] as ObjectSpawnBox).SetLocked();
    }

    private GridContainer CreateSpawnContainer(params GameObject[] objects)
    {
      Rectangle rectangle = new Rectangle(5, 50, 330, this.rectangle.Height - 10 - 50 - 100);
      Vector2 vector2_1 = new Vector2((float) ((rectangle.Width - 30) / 90), (float) (rectangle.Height / 120));
      Vector2 vector2_2 = new Vector2((float) (((double) rectangle.Width - (double) vector2_1.X * 60.0) / ((double) vector2_1.X + 1.0)), (float) (((double) rectangle.Height - (double) vector2_1.Y * 60.0) / ((double) vector2_1.Y + 1.0)));
      List<UIElement> uiElementList = new List<UIElement>();
      for (int index = 0; index < objects.Length; ++index)
        uiElementList.Add((UIElement) new ObjectSpawnBox(new Rectangle(0, 0, 60, 60), objects[index]));
      GridContainer child = new GridContainer(Rectangle.Empty, 60, 60, (int) vector2_2.X, (int) vector2_2.Y, maxElementSizeRatio: 1.5f, forceSquareElements: true);
      child.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(5, 50, parentRect.Width - 10, parentRect.Height - 10 - 50 - 100)));
      child.SetElements(uiElementList.ToArray());
      this.AddChild((UIElement) child);
      return child;
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (!this.IsActive || !this.IsCurrentState())
        return;
      GameObject gameObject;
      if (!this.isScaling && !this.isMouseDragging && this.MouseIsOnSelected(out gameObject) && gameObject != null)
        UIElement.SetCursorOverride(MouseCursor.SizeAll);
      List<GameObject> selectedObjects = this.selectedObjects;
      // ISSUE: explicit non-virtual call
      if ((selectedObjects != null ? (selectedObjects.Count > 0 ? 1 : 0) : 0) != 0 || this.parameterMenuToggle.IsActive)
        this.UpdateParameterMenu();
      Vector2 vector2_1 = this.LocalMousePos(forceTransformedLevelSpace: true);
      if (this.isMoving)
      {
        RectangleF selectedArea = this.GetSelectedArea();
        Vector2 vector2_2 = vector2_1 + this.selectionMouseOffset;
        int num1 = 0;
        int num2 = 0;
        foreach (GameObject selectedObject in this.selectedObjects)
        {
          if (!selectedObject.IsDead)
            ++num1;
          if (selectedObject.IsActive)
            ++num2;
        }
        if (BotsAreStupid.Input.IsDown(KeyBind.Control) ^ EditorGrid.Instance.SnappingEnabled)
        {
          int num3 = EditorGrid.CellSize / 2;
          float num4 = vector2_2.X - 340f + (float) num3;
          if ((double) selectedArea.Width < (double) EditorGrid.CellSize)
          {
            vector2_2.X -= num4 % (float) num3 - (float) num3;
            if ((double) num4 % (double) EditorGrid.CellSize > (double) num3)
              vector2_2.X += (float) EditorGrid.CellSize - selectedArea.Width;
          }
          else
            vector2_2.X -= num4 % (float) EditorGrid.CellSize - (float) num3;
          float num5 = vector2_2.Y + (float) num3;
          if ((double) selectedArea.Height < (double) EditorGrid.CellSize)
          {
            vector2_2.Y -= num5 % (float) num3 - (float) num3;
            if ((double) num5 % (double) EditorGrid.CellSize > (double) num3)
              vector2_2.Y += (float) EditorGrid.CellSize - selectedArea.Height;
          }
          else
            vector2_2.Y -= num5 % (float) EditorGrid.CellSize - (float) num3;
        }
        vector2_2.X -= vector2_2.X % 1f;
        vector2_2.Y -= vector2_2.Y % 1f;
        Vector2 v = vector2_2 - new Vector2(selectedArea.X, selectedArea.Y);
        for (int index = this.selectedObjects.Count - 1; index >= 0; --index)
          this.selectedObjects[index].Translate(v);
      }
      else
      {
        if (this.isScaling)
          this.ScaleSelected(vector2_1);
        else if (this.isDuplicateScaling)
          this.DuplicateScaleSelected(vector2_1);
        else if (this.isMouseDragging)
          this.SelectAllInRect(Utility.VectorsToRectangle(this.mouseDragStart, vector2_1));
        if (this.selectedObjects.Count == 1 && this.selectedObjects[0].IsScalable)
        {
          int edge;
          if (this.MouseIsOnEdgeRects(out edge))
          {
            UIElement.SetCursorOverride(edge % 2 == 0 ? MouseCursor.SizeWE : MouseCursor.SizeNS);
          }
          else
          {
            int corner;
            if (this.MouseIsOnCornerRects(out corner))
              UIElement.SetCursorOverride(corner % 2 == 0 ? MouseCursor.SizeNWSE : MouseCursor.SizeNESW);
          }
        }
      }
      this.lastMousePos = vector2_1;
    }

    public void DrawLevelUI(SpriteBatch spriteBatch)
    {
      if (!this.hasSelectedObjects || this.isMoving || PopupMenu.Instance.IsActive)
        return;
      RectangleF selectedArea = this.GetSelectedArea(inTransformedLevelSpace: true);
      if (this.selectedObjects.Count == 1)
      {
        if (this.selectedObjects[0].IsScalable && !this.isMouseDragging)
        {
          foreach (Rectangle cornerRect in this.GetCornerRects(selectedArea))
            Utility.DrawOutline(spriteBatch, cornerRect, 1, Color.White);
        }
      }
      else if (!this.isMouseDragging)
        Utility.DrawOutline(spriteBatch, selectedArea, 2, Color.White);
      for (int index = this.selectedObjects.Count - 1; index >= 0; --index)
      {
        Rectangle rectangle = this.selectedObjects[index].Rectangle;
        Vector2 transformedLevelPos1 = Utility.LevelPosToTransformedLevelPos(rectangle.Location.ToVector2());
        Vector2 transformedLevelPos2 = Utility.LevelPosToTransformedLevelPos((rectangle.Location + rectangle.Size).ToVector2());
        Utility.DrawOutline(spriteBatch, Utility.VectorsToRectangle(transformedLevelPos1, transformedLevelPos2), 1, Color.White);
      }
    }

    public void DrawTransformedLevelUI(SpriteBatch spriteBatch)
    {
      if (!this.isMouseDragging)
        return;
      Vector2 v2 = this.LocalMousePos(forceTransformedLevelSpace: true);
      if ((double) (this.mouseDragStart - v2).Length() > 10.0)
        Utility.DrawOutline(spriteBatch, Utility.VectorsToRectangle(this.mouseDragStart, v2), 2, ColorManager.GetColor("red"));
    }

    public void SpawnObject(GameObject obj)
    {
      this.ClearSelected();
      this.isMoving = true;
      this.beforeMovePosition = Vector2.Zero;
      this.AddSelected(obj);
      Vector2 vector2 = this.LocalMousePos();
      Rectangle selectedRectangle = this.GetSelectedRectangle();
      this.selectionMouseOffset = new Vector2((float) selectedRectangle.X, (float) selectedRectangle.Y) - vector2;
      this.parameterMenuToggle.DisableContent();
      this.parameterMenu.SetActive(false);
    }

    public void SetLevelNameDisplay(string name)
    {
      this.levelNameDisplay.SetText("Level:   " + name);
      this.SetHasChanges(false);
    }

    public void UpdateSidebar(int x, int width, int height)
    {
      this.sideBar.SetPosition(new int?(x));
      this.sideBar.SetSize(width, height, true);
    }

    private void AddSelected(GameObject obj) => this.selectedObjects.Add(obj);

    private void RemoveSelected(GameObject obj) => this.selectedObjects.Remove(obj);

    public void ClearHistory()
    {
      this.ClearSelected();
      this.undoActions.Clear();
      this.redoActions.Clear();
      this.undoButton.SetInteractable(false);
      this.redoButton.SetInteractable(false);
    }

    private void ClearSelected() => this.selectedObjects.Clear();

    private void SelectAllInRect(Rectangle rect)
    {
      SimulationManager.MainSimulation.ForEachGameObject((System.Action<GameObject>) (g =>
      {
        if (this.selectedObjects.Contains(g) || !g.IsSelectable || !g.IntersectsRectangle(rect, false))
          return;
        this.AddSelected(g);
      }));
      for (int index = this.selectedObjects.Count - 1; index >= 0; --index)
      {
        GameObject selectedObject = this.selectedObjects[index];
        if (!selectedObject.IntersectsRectangle(rect, false))
          this.RemoveSelected(selectedObject);
      }
    }

    private void OnMouseUp()
    {
      if (this.isMoving)
      {
        this.isMoving = false;
        if (this.selectedObjects.Count > 0)
        {
          Vector2 pos = this.selectedObjects[0].Position;
          Vector2 undoPos = this.beforeMovePosition;
          this.DoAction(new EditorAction((LevelEditor.Action) (objects => this.MoveObjects(objects, pos)), (LevelEditor.Action) (objects => this.MoveObjects(objects, undoPos)), this.selectedObjects.ToArray()));
        }
      }
      if (this.isScaling)
      {
        this.isScaling = false;
        if (this.selectedObjects.Count > 0)
        {
          Vector2 pos = this.selectedObjects[0].Position;
          Vector2 size = this.selectedObjects[0].Size;
          Vector2 undoPos = this.beforeScalePosition;
          Vector2 undoSize = this.beforeScaleSize;
          this.DoAction(new EditorAction((LevelEditor.Action) (objects =>
          {
            objects[0].SetPosition(pos);
            objects[0].Width = (int) size.X;
            objects[0].Height = (int) size.Y;
            objects[0].CalculateRenderScale();
          }), (LevelEditor.Action) (objects =>
          {
            objects[0].SetPosition(undoPos);
            objects[0].Width = (int) undoSize.X;
            objects[0].Height = (int) undoSize.Y;
            objects[0].CalculateRenderScale();
          }), this.selectedObjects.ToArray()));
        }
      }
      this.isMouseDragging = false;
      this.CheckSelectedPositions();
    }

    private void OnRightMouseUp()
    {
      if (!this.isDuplicateScaling)
        return;
      this.isDuplicateScaling = false;
      if (this.selectedObjects.Count > 1)
        this.DoAction(new EditorAction((LevelEditor.Action) (objects =>
        {
          this.ClearSelected();
          for (int index = 0; index < objects.Length; ++index)
          {
            this.AddSelected(objects[index]);
            if (index != 0)
              objects[index].Revive();
          }
        }), (LevelEditor.Action) (objects =>
        {
          for (int index = 1; index < objects.Length; ++index)
            objects[index].Destroy();
          this.ClearSelected();
          this.AddSelected(objects[0]);
        }), this.selectedObjects.ToArray()));
    }

    private void OnMouseDown()
    {
      if (PopupMenu.Instance.IsActive)
        return;
      Vector2 point = this.LocalMousePos(forceTransformedLevelSpace: true);
      int edge = -1;
      int corner = -1;
      if (this.selectedObjects.Count == 1 && this.selectedObjects[0].IsScalable && (this.MouseIsOnEdgeRects(out edge) || this.MouseIsOnCornerRects(out corner)))
      {
        this.isScaling = true;
        this.beforeScalePosition = this.selectedObjects[0].Position;
        this.beforeScaleSize = this.selectedObjects[0].Size;
        if (edge != -1)
        {
          this.scaleAxisLock = edge % 2 == 0 ? 1 : 0;
          this.scaleCornerId = edge;
        }
        else
        {
          if (corner == -1)
            return;
          this.scaleAxisLock = -1;
          this.scaleCornerId = corner;
        }
      }
      else
      {
        GameObject gameObject;
        if (this.MouseIsOnSelected(out gameObject))
        {
          if (BotsAreStupid.Input.IsDown(KeyBind.Shift))
          {
            this.RemoveSelected(gameObject);
          }
          else
          {
            this.isMoving = true;
            this.beforeMovePosition = this.selectedObjects[0].Position;
            Rectangle selectedRectangle = this.GetSelectedRectangle();
            this.selectionMouseOffset = new Vector2((float) selectedRectangle.X, (float) selectedRectangle.Y) - point;
          }
        }
        else
        {
          if (Utility.MouseInside(this.parameterMenuToggle.GlobalRect, inTransformedLevelSpace: true) || this.parameterMenu.IsActive && Utility.MouseInside(this.parameterMenu.GlobalRect, inTransformedLevelSpace: true))
            return;
          GameObject gameObjectByPoint = Utility.GetGameObjectByPoint(SimulationManager.MainSimulation, point, typeof (Particle));
          if (gameObjectByPoint != null && gameObjectByPoint.IsSelectable)
          {
            if (!BotsAreStupid.Input.IsDown(KeyBind.Shift))
              this.ClearSelected();
            this.AddSelected(gameObjectByPoint);
          }
          else if (this.hasSelectedObjects)
            this.ClearSelected();
          else if (340.0 - (double) point.X < -5.0 && (double) Utility.VirtualWidth - (double) point.X > 5.0)
          {
            this.isMouseDragging = true;
            this.mouseDragStart = point;
          }
        }
      }
    }

    private void OnMouseDoubleDown()
    {
      GameObject obj;
      if (PopupMenu.Instance.IsActive || !this.MouseIsOnSelected(out obj))
        return;
      this.ClearSelected();
      List<GameObject> allByType = SimulationManager.MainSimulation.GetAllByType(obj.GetType());
      if (obj is Platform || obj is BackgroundObject)
        allByType.RemoveAll((Predicate<GameObject>) (shared =>
        {
          Rectangle? renderSpritePos1 = shared.RenderSpritePos;
          Rectangle? renderSpritePos2 = obj.RenderSpritePos;
          if (renderSpritePos1.HasValue != renderSpritePos2.HasValue)
            return true;
          return renderSpritePos1.HasValue && renderSpritePos1.GetValueOrDefault() != renderSpritePos2.GetValueOrDefault();
        }));
      foreach (GameObject gameObject in allByType)
        this.AddSelected(gameObject);
      BotsAreStupid.Input.CancelCurrentAction = true;
    }

    private void OnRightMouseDown()
    {
      if (PopupMenu.Instance.IsActive)
        return;
      List<(string, Button.OnClick)> valueTupleList = new List<(string, Button.OnClick)>();
      int edge = -1;
      int corner = -1;
      ContextMenu.Instance?.Close();
      if (this.selectedObjects.Count == 1 && (this.MouseIsOnCornerRects(out corner) || this.MouseIsOnEdgeRects(out edge)))
      {
        this.isDuplicateScaling = true;
        if (edge != -1)
        {
          this.scaleAxisLock = edge % 2 == 0 ? 1 : 0;
          this.scaleCornerId = edge;
        }
        else if (corner != -1)
        {
          this.scaleAxisLock = -1;
          this.scaleCornerId = corner;
        }
      }
      else if (this.selectedObjects.Count > 0 && Utility.MouseInside(this.GetSelectedRectangle(), inTransformedLevelSpace: true))
      {
        GameObject firstSelected = this.selectedObjects[0];
        Rectangle selectedRect = this.GetSelectedRectangle();
        Vector2 virtualSize = Utility.VirtualSize;
        string str = this.selectedObjects.Count > 1 ? "s" : "";
        valueTupleList.Add(("Copy", new Button.OnClick(this.CopySelected)));
        valueTupleList.Add(("Cut%", new Button.OnClick(this.CutSelected)));
        valueTupleList.Add(("Set X Position" + str, (Button.OnClick) (() => PopupMenu.Instance.Prompt("X Position", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), (PromptMenu.InteractionHandlerString) (v =>
        {
          this.MoveAllSelectedAction(new float?(float.Parse(v, (IFormatProvider) CultureInfo.InvariantCulture)));
          PopupMenu.Instance.SetActive(false);
        }), (PromptMenu.Validator) ((string s, out string e) => Utility.FloatNumberValidator(s, out e, (float) ((int) virtualSize.X + firstSelected.Width / 2), (float) (340 - firstSelected.Width / 2))), firstSelected.X.ToString((IFormatProvider) CultureInfo.InvariantCulture)))));
        valueTupleList.Add(("Set Y Position" + str, (Button.OnClick) (() => PopupMenu.Instance.Prompt("Y Position", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), (PromptMenu.InteractionHandlerString) (v =>
        {
          this.MoveAllSelectedAction(y: new float?(float.Parse(v, (IFormatProvider) CultureInfo.InvariantCulture)));
          PopupMenu.Instance.SetActive(false);
        }), (PromptMenu.Validator) ((string s, out string e) => Utility.FloatNumberValidator(s, out e, (float) ((int) virtualSize.Y + firstSelected.Height / 2), (float) (-firstSelected.Height / 2))), firstSelected.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture)))));
        if (this.selectedObjects.Count > 1)
          valueTupleList.Add(("Align Centers", (Button.OnClick) (() =>
          {
            Point center = selectedRect.Center;
            this.MoveAllSelectedAction(new float?((float) center.X), new float?((float) center.Y), true);
          })));
        valueTupleList.Add(("%Rotate", new Button.OnClick(this.RotateSelected)));
        valueTupleList.Add(("Duplicate", new Button.OnClick(this.DuplicateSelected)));
        valueTupleList.Add(("Delete", new Button.OnClick(this.DeleteSelected)));
      }
      else if (Utility.MouseInside(new Rectangle(340, 0, Utility.VirtualWidth - 340, Utility.VirtualHeight), inTransformedLevelSpace: true) && this.clipboard != null)
        valueTupleList.Add(("Paste Here", new Button.OnClick(this.PasteToMouse)));
      if (valueTupleList.Count <= 0)
        return;
      Utility.OpenContextMenu(true, (UIElement) null, valueTupleList.ToArray());
    }

    private Rectangle GetSelectedRectangle(GameObject[] objects = null, bool inTransformedLevelSpace = false)
    {
      return (Rectangle) this.GetSelectedArea(objects, inTransformedLevelSpace);
    }

    private RectangleF GetSelectedArea(GameObject[] objects = null, bool inTransformedLevelSpace = false)
    {
      float x1 = float.MaxValue;
      float y1 = float.MaxValue;
      float x2 = float.MinValue;
      float y2 = float.MinValue;
      for (int index = (objects == null ? this.selectedObjects.Count : objects.Length) - 1; index >= 0; --index)
      {
        GameObject gameObject = objects == null ? this.selectedObjects[index] : objects[index];
        RectangleF rectanglePrecise = gameObject.RectanglePrecise;
        Rectangle rectangle = gameObject.Rectangle;
        if ((double) rectanglePrecise.X < (double) x1)
          x1 = rectanglePrecise.X;
        if ((double) rectanglePrecise.Y < (double) y1)
          y1 = rectanglePrecise.Y;
        float num1 = rectanglePrecise.X + (float) rectangle.Width;
        if ((double) num1 > (double) x2)
          x2 = num1;
        float num2 = rectanglePrecise.Y + (float) rectangle.Height;
        if ((double) num2 > (double) y2)
          y2 = num2;
      }
      if (inTransformedLevelSpace)
      {
        Vector2 transformedLevelPos1 = Utility.LevelPosToTransformedLevelPos(new Vector2(x1, y1));
        x1 = transformedLevelPos1.X;
        y1 = transformedLevelPos1.Y;
        Vector2 transformedLevelPos2 = Utility.LevelPosToTransformedLevelPos(new Vector2(x2, y2));
        x2 = transformedLevelPos2.X;
        y2 = transformedLevelPos2.Y;
      }
      return new RectangleF(x1, y1, x2 - x1, y2 - y1);
    }

    private Rectangle[] GetCornerRects(RectangleF selectedRect)
    {
      int num = 1;
      return new Rectangle[4]
      {
        new Rectangle((int) ((double) selectedRect.X - 14.0), (int) ((double) selectedRect.Y - 14.0), 14 - num, 14 - num),
        new Rectangle((int) ((double) selectedRect.X - 14.0), (int) ((double) selectedRect.Y + (double) selectedRect.Height + (double) num), 14 - num, 14 - num),
        new Rectangle((int) ((double) selectedRect.X + (double) selectedRect.Width + (double) num), (int) ((double) selectedRect.Y + (double) selectedRect.Height + (double) num), 14 - num, 14 - num),
        new Rectangle((int) ((double) selectedRect.X + (double) selectedRect.Width + (double) num), (int) ((double) selectedRect.Y - 14.0), 14 - num, 14 - num)
      };
    }

    private bool MouseIsOnCornerRects(out int corner)
    {
      corner = -1;
      if (!Utility.MouseInside(StateManager.UISpaceLevelRect))
        return false;
      Rectangle[] cornerRects = this.GetCornerRects(this.GetSelectedArea(inTransformedLevelSpace: true));
      for (int index = 0; index < cornerRects.Length; ++index)
      {
        if (Utility.MouseInside(cornerRects[index], true))
        {
          corner = index;
          return true;
        }
      }
      return false;
    }

    private bool MouseIsOnSelected(out GameObject obj)
    {
      obj = (GameObject) null;
      if (!Utility.MouseInside(StateManager.UISpaceLevelRect))
        return false;
      for (int index = this.selectedObjects.Count - 1; index >= 0; --index)
      {
        obj = this.selectedObjects[index];
        if (Utility.MouseInside(obj.Rectangle, inTransformedLevelSpace: true))
          return true;
      }
      return false;
    }

    private bool MouseIsOnEdgeRects(out int edge)
    {
      edge = -1;
      if (!Utility.MouseInside(StateManager.UISpaceLevelRect))
        return false;
      Rectangle selectedRectangle = this.GetSelectedRectangle(inTransformedLevelSpace: true);
      Vector2[] vector2Array = new Vector2[4]
      {
        new Vector2((float) selectedRectangle.X, (float) selectedRectangle.Y),
        new Vector2((float) selectedRectangle.X, (float) (selectedRectangle.Y + selectedRectangle.Height)),
        new Vector2((float) (selectedRectangle.X + selectedRectangle.Width), (float) (selectedRectangle.Y + selectedRectangle.Height)),
        new Vector2((float) (selectedRectangle.X + selectedRectangle.Width), (float) selectedRectangle.Y)
      };
      Rectangle rec = Rectangle.Empty;
      edge = 0;
      while (edge < 4)
      {
        Vector2 vector2_1 = vector2Array[edge];
        Vector2 vector2_2 = vector2Array[(edge + 1) % 4];
        switch (edge)
        {
          case 0:
            rec = new Rectangle((int) ((double) vector2_1.X - 14.0), (int) vector2_1.Y, 14, (int) ((double) vector2_2.Y - (double) vector2_1.Y));
            break;
          case 1:
            rec = new Rectangle((int) vector2_1.X, (int) vector2_1.Y, (int) ((double) vector2_2.X - (double) vector2_1.X), 14);
            break;
          case 2:
            rec = new Rectangle((int) vector2_2.X, (int) vector2_2.Y, 14, (int) ((double) vector2_1.Y - (double) vector2_2.Y));
            break;
          case 3:
            rec = new Rectangle((int) vector2_2.X, (int) ((double) vector2_2.Y - 14.0), (int) ((double) vector2_1.X - (double) vector2_2.X), 14);
            break;
        }
        if (Utility.MouseInside(rec, true))
          return true;
        ++edge;
      }
      edge = -1;
      return false;
    }

    private void ScaleSelected(Vector2 mousePos)
    {
      if (this.selectedObjects.Count <= 0)
        return;
      GameObject selectedObject = this.selectedObjects[0];
      Vector2 vector2_1 = selectedObject.DefaultSize * selectedObject.MinScale;
      Vector2 vector2_2 = selectedObject.DefaultSize * (selectedObject.MaxScale ?? 128.0f);
      if (selectedObject.IsTipped)
      {
        vector2_1 = new Vector2(vector2_1.Y, vector2_1.X);
        vector2_2 = new Vector2(vector2_2.Y, vector2_2.X);
      }
      float zoom = Utility.GetZoom(out float _);
      Vector2 vector2_3 = selectedObject.Position + selectedObject.Size;
      switch (this.scaleCornerId)
      {
        case 0:
          adjustMousePos(new Vector2(14f, 14f) / 2f);
          selectedObject.SetPosition(new Vector2((float) (int) MathHelper.Clamp(mousePos.X, vector2_3.X - vector2_2.X, vector2_3.X - vector2_1.X),
              (float) (int) MathHelper.Clamp(mousePos.Y, vector2_3.Y - vector2_2.Y, vector2_3.Y - vector2_1.Y)));
          selectedObject.Width = (int) ((double) vector2_3.X - (double) selectedObject.X);
          selectedObject.Height = (int) ((double) vector2_3.Y - (double) selectedObject.Y);
          break;
        case 1:
          adjustMousePos(-new Vector2(-14f, 14f) / 2f);
          selectedObject.SetPosition(new Vector2((float) (int)MathHelper.Clamp(mousePos.X, vector2_3.X - vector2_2.X, vector2_3.X - vector2_1.X), selectedObject.Y));
          selectedObject.Width = (int) ((double) vector2_3.X - (double) selectedObject.X);
          selectedObject.Height = (int)MathHelper.Clamp(mousePos.Y - selectedObject.Y, vector2_1.Y, vector2_2.Y);
          break;
        case 2:
          adjustMousePos(-new Vector2(14f, 14f) / 2f);
          selectedObject.Width = (int) MathHelper.Clamp(mousePos.X - selectedObject.X, vector2_1.X, vector2_2.X);
          selectedObject.Height = (int) MathHelper.Clamp(mousePos.Y - selectedObject.Y, vector2_1.Y, vector2_2.Y);
          break;
        case 3:
          adjustMousePos(-new Vector2(14f, -14f) / 2f);
          selectedObject.SetPosition(new Vector2(selectedObject.X, (float) (int)MathHelper.Clamp(mousePos.Y, vector2_3.Y - vector2_2.Y, vector2_3.Y - vector2_1.Y)));
          selectedObject.Width = (int)MathHelper.Clamp(mousePos.X - selectedObject.X, vector2_1.X, vector2_2.X);
          selectedObject.Height = (int) ((double) vector2_3.Y - (double) selectedObject.Y);
          break;
      }
      if (this.scaleAxisLock == 0)
      {
        selectedObject.SetPosition(this.beforeScalePosition.X, selectedObject.Y);
        selectedObject.Width = (int) this.beforeScaleSize.X;
      }
      else if (this.scaleAxisLock == 1)
      {
        selectedObject.SetPosition(selectedObject.X, this.beforeScalePosition.Y);
        selectedObject.Height = (int) this.beforeScaleSize.Y;
      }
      selectedObject.CalculateRenderScale();

      void adjustMousePos(Vector2 amount)
      {
        mousePos += amount / zoom;
        if (!(BotsAreStupid.Input.IsDown(KeyBind.Control) ^ EditorGrid.Instance.SnappingEnabled))
          return;
        int num = EditorGrid.CellSize / 2;
        mousePos.X -= (mousePos.X - 340f + (float) num) % (float) EditorGrid.CellSize - (float) num;
        mousePos.Y -= (mousePos.Y + (float) num) % (float) EditorGrid.CellSize - (float) num;
      }
    }

    private void DuplicateScaleSelected(Vector2 mousePos)
    {
      if (this.selectedObjects.Count <= 0)
        return;
      GameObject selectedObject = this.selectedObjects[this.selectedObjects.Count - 1];
      Rectangle rectangle = selectedObject.Rectangle;
      Vector2 position = selectedObject.Position;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = this.scaleAxisLock == 0 || (double) mousePos.X < (double) rectangle.X;
      bool flag4 = this.scaleAxisLock == 0 || (double) mousePos.X > (double) (rectangle.X + rectangle.Width);
      bool flag5 = this.scaleAxisLock == 1 || (double) mousePos.Y < (double) rectangle.Y;
      bool flag6 = this.scaleAxisLock == 1 || (double) mousePos.Y > (double) (rectangle.Y + rectangle.Height);
      switch (this.scaleCornerId)
      {
        case 0:
          position.X -= (float) rectangle.Width;
          position.Y -= (float) rectangle.Height;
          flag1 = flag3 & flag5;
          flag2 = flag4 & flag6;
          break;
        case 1:
          position.X -= (float) rectangle.Width;
          position.Y += (float) rectangle.Height;
          flag1 = flag3 & flag6;
          flag2 = flag4 & flag5;
          break;
        case 2:
          position.X += (float) rectangle.Width;
          position.Y += (float) rectangle.Height;
          flag1 = flag4 & flag6;
          flag2 = flag3 & flag5;
          break;
        case 3:
          position.X += (float) rectangle.Width;
          position.Y -= (float) rectangle.Height;
          flag1 = flag4 & flag5;
          flag2 = flag3 & flag6;
          break;
      }
      if (this.scaleAxisLock == 0)
        position.X = selectedObject.X;
      else if (this.scaleAxisLock == 1)
        position.Y = selectedObject.Y;
      if (flag1)
      {
        GameObject gameObject = selectedObject.Copy();
        gameObject.SetPosition(position);
        this.AddSelected(gameObject);
        this.PlayInteractionSound(0.1f);
      }
      else if (flag2 && this.selectedObjects.Count > 1)
      {
        this.RemoveSelected(selectedObject);
        selectedObject.Destroy();
        this.PlayInteractionSound(0.1f);
      }
    }

    private void OnDuplicateKey()
    {
      if (!BotsAreStupid.Input.IsDown(KeyBind.Control))
        return;
      this.DuplicateSelected();
    }

    private void DuplicateSelected()
    {
      if (!this.hasSelectedObjects)
        return;
      List<GameObject> copies = new List<GameObject>();
      this.DoAction(new EditorAction((LevelEditor.Action) (objects =>
      {
        this.ClearSelected();
        if (copies.Count > 0)
        {
          foreach (GameObject gameObject in copies)
          {
            gameObject.Revive();
            this.AddSelected(gameObject);
          }
        }
        else
        {
          foreach (GameObject gameObject1 in objects)
          {
            GameObject gameObject2 = gameObject1.Copy();
            copies.Add(gameObject2);
            this.AddSelected(gameObject2);
          }
        }
      }), (LevelEditor.Action) (objects =>
      {
        foreach (BaseObject baseObject in copies)
          baseObject.Destroy();
        this.ClearSelected();
      }), this.selectedObjects.ToArray()));
    }

    private void RotateSelected()
    {
      if (!this.hasSelectedObjects)
        return;
      Vector2[] undoPositions = new Vector2[this.selectedObjects.Count];
      for (int index = 0; index < this.selectedObjects.Count; ++index)
        undoPositions[index] = this.selectedObjects[index].Position;
      this.DoAction(new EditorAction((LevelEditor.Action) (objects =>
      {
        Rectangle selectedRectangle = this.GetSelectedRectangle(objects);
        Vector2 pivot = new Vector2((float) (selectedRectangle.X + selectedRectangle.Width / 2), (float) (selectedRectangle.Y + selectedRectangle.Height / 2));
        foreach (BaseObject baseObject in objects)
          baseObject.RotateAround(pivot, 90f);
      }), (LevelEditor.Action) (objects =>
      {
        for (int index = 0; index < objects.Length; ++index)
        {
          objects[index].Rotate(-90f);
          objects[index].SetPosition(undoPositions[index]);
        }
        this.CheckSelectedPositions();
      }), this.selectedObjects.ToArray()));
    }

    private void DeleteSelected()
    {
      if (!this.hasSelectedObjects)
        return;
      this.DoAction(new EditorAction((LevelEditor.Action) (objects =>
      {
        foreach (BaseObject baseObject in objects)
          baseObject.Destroy();
        this.ClearSelected();
      }), (LevelEditor.Action) (objects =>
      {
        foreach (BaseObject baseObject in objects)
          baseObject.Revive();
      }), this.selectedObjects.ToArray()));
    }

    private void MoveSelectedDir(Vector2 dir)
    {
      if (!this.hasSelectedObjects)
        return;
      if (BotsAreStupid.Input.IsDown(KeyBind.Shift))
      {
        Rectangle selectedRectangle = this.GetSelectedRectangle();
        dir = Vector2.Transform(dir, Matrix.CreateScale((float) (selectedRectangle.Width / 2), (float) (selectedRectangle.Height / 2), 1f));
      }
      Vector2 undoPos = this.selectedObjects[0].Position;
      Vector2 pos = undoPos + dir;
      this.DoAction(new EditorAction((LevelEditor.Action) (objects => this.MoveObjects(objects, pos)), (LevelEditor.Action) (objects => this.MoveObjects(objects, undoPos)), this.selectedObjects.ToArray()));
    }

    private void MoveSelectedUp() => this.MoveSelectedDir(new Vector2(0.0f, -1f));

    private void MoveSelectedDown() => this.MoveSelectedDir(new Vector2(0.0f, 1f));

    private void MoveSelectedLeft() => this.MoveSelectedDir(new Vector2(-1f, 0.0f));

    private void MoveSelectedRight() => this.MoveSelectedDir(new Vector2(1f, 0.0f));

    private void MoveObjects(GameObject[] objects, Vector2 pos)
    {
      for (int index = objects.Length - 1; index >= 0; --index)
      {
        GameObject gameObject = objects[index];
        gameObject.Revive();
        if (index != 0)
        {
          Vector2 vector2 = gameObject.Position - objects[0].Position;
          gameObject.SetPosition(pos + vector2);
        }
      }
      objects[0].SetPosition(pos);
    }

    private void MoveAllSelectedAction(float? x = null, float? y = null, bool centered = false)
    {
      Dictionary<GameObject, Vector2> undoPositions = new Dictionary<GameObject, Vector2>();
      this.DoAction(new EditorAction((LevelEditor.Action) (objects =>
      {
        foreach (GameObject key in objects)
        {
          undoPositions.Add(key, key.Position);
          key.SetPosition(x.HasValue ? x.Value - (centered ? (float) (key.Width / 2) : 0.0f) : key.X, y.HasValue ? y.Value - (centered ? (float) (key.Height / 2) : 0.0f) : key.Y);
        }
      }), (LevelEditor.Action) (objects =>
      {
        foreach (GameObject key in objects)
        {
          Vector2 pos;
          if (undoPositions.TryGetValue(key, out pos))
            key.SetPosition(pos);
        }
      }), this.selectedObjects.ToArray()));
    }

    private void CheckSelectedPositions()
    {
      for (int index = this.selectedObjects.Count - 1; index >= 0; --index)
      {
        if ((double) this.selectedObjects[index].X + (double) this.selectedObjects[index].Width + 15.0 < 340.0)
        {
          this.selectedObjects[index].Destroy();
          this.RemoveSelected(this.selectedObjects[index]);
        }
      }
    }

    private void CopySelected()
    {
      if (this.selectedObjects.Count == 0)
        return;
      this.clipboard = new GameObject[this.selectedObjects.Count];
      for (int index = 0; index < this.selectedObjects.Count; ++index)
        this.clipboard[index] = this.selectedObjects[index].Clone() as GameObject;
    }

    private void CutSelected()
    {
      this.CopySelected();
      this.DeleteSelected();
    }

    private void Paste()
    {
      if (this.clipboard == null || this.clipboard.Length == 0)
        return;
      this.ClearSelected();
      foreach (BaseObject baseObject in this.clipboard)
      {
        GameObject gameObject = baseObject.Clone() as GameObject;
        SimulationManager.MainSimulation.RegisterObject((BaseObject) gameObject);
        this.AddSelected(gameObject);
      }
      this.DoAction(new EditorAction((LevelEditor.Action) (objects =>
      {
        foreach (BaseObject baseObject in objects)
          baseObject.Revive();
      }), (LevelEditor.Action) (objects =>
      {
        foreach (BaseObject baseObject in objects)
          baseObject.Destroy();
        this.ClearSelected();
      }), this.selectedObjects.ToArray()));
    }

    private void PasteToMouse()
    {
      this.Paste();
      Rectangle selectedRectangle = this.GetSelectedRectangle();
      Vector2 v = Utility.GetMousePos(inTransformedLevelSpace: true) - new Vector2((float) selectedRectangle.Width, (float) selectedRectangle.Height) / 2f - new Vector2((float) selectedRectangle.X, (float) selectedRectangle.Y);
      foreach (BaseObject selectedObject in this.selectedObjects)
        selectedObject.Translate(v);
    }

    private void SetSpawnerTab(string id)
    {
      this.activeSpawnerTab = int.Parse(id);
      Color color = ColorManager.GetColor("lightslate");
      Styling newStyle1 = new Styling()
      {
        defaultColor = color,
        borderWidth = -1,
        clickColor = color,
        hoverCursor = new bool?(false)
      };
      Styling newStyle2 = new Styling()
      {
        defaultColor = ColorManager.GetColor("darkslate"),
        borderWidth = 4,
        clickColor = ColorManager.GetColor("red"),
        hoverCursor = new bool?(true)
      };
      for (int index = 0; index < this.spawnTabs.Length; ++index)
      {
        if (index == this.activeSpawnerTab)
        {
          this.spawnTabs[index].ChangeStyle(newStyle1);
          this.spawnContainer[index].SetActive(true);
        }
        else
        {
          this.spawnTabs[index].ChangeStyle(newStyle2);
          this.spawnContainer[index].SetActive(false);
        }
      }
    }

    private void PlayLevel()
    {
      if (LevelManager.CurrentLevelName == "Editor")
      {
        LevelManager.SetCurrentLevelName("AutoSave");
        this.HasAutoSaved = true;
      }
      LevelManager.SaveCurrent();
      this.SetHasChanges(false);
      StateManager.TransitionTo(GameState.InLevel_FromEditor, LevelManager.CurrentLevelName);
    }

    private void OnGridToggleButtonPress()
    {
      EditorGrid.ToggleActive();
      this.gridChangeScaleButton.SetActive(EditorGrid.Instance.IsActive);
      this.gridSnapToggleButton.SetActive(EditorGrid.Instance.IsActive);
    }

    private void OnSaveButtonClick()
    {
      string currentLevelName = LevelManager.CurrentLevelName;
      if (currentLevelName == "AutoSave" || currentLevelName == "Editor")
      {
        PopupMenu.Instance.Prompt("Save Level*Name:", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)), (PromptMenu.InteractionHandlerString) (s =>
        {
          this.SaveNew(s);
          this.SetHasChanges(false);
        }), new PromptMenu.Validator(Utility.LevelNameValidator));
      }
      else
      {
        LevelManager.SaveCurrent();
        this.SetHasChanges(false);
      }
    }

    private void OnLoadButtonClick()
    {
      if (this.HasChanges)
        PopupMenu.Instance.Confirm("You have unsaved changes!*Are you sure?", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), new PromptMenu.InteractionHandler(PopupMenu.Instance.ShowLoadLevelMenu));
      else
        PopupMenu.Instance.ShowLoadLevelMenu();
    }

    private void OnNewButtonClick()
    {
      PopupMenu.Instance.Prompt("New Level*Name:", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)), (PromptMenu.InteractionHandlerString) (s =>
      {
        if (LevelManager.CurrentLevelName != "Editor")
          LevelManager.SaveCurrent();
        LevelManager.Load("Editor");
        this.SaveNew(s);
        this.ClearHistory();
        this.SetHasChanges(false);
      }), new PromptMenu.Validator(Utility.LevelNameValidator));
    }

    private void SaveNew(string name)
    {
      LevelManager.SaveAs(Utility.TranslateLevelname(name));
      PopupMenu.Instance.Inform("Level saved!", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)), false);
    }

    private void UpdateParameterMenu()
    {
      if (!this.hasSelectedObjects || this.isMoving || this.isScaling || this.isMouseDragging)
      {
        this.parameterMenuToggle.SetActive(false);
      }
      else
      {
        bool flag1 = true;
        Type type1 = (Type) null;
        for (int index = this.selectedObjects.Count - 1; index >= 0; --index)
        {
          Type type2 = this.selectedObjects[index].GetType();
          if (flag1 && type1 != (Type) null && type2 != type1)
            flag1 = false;
          type1 = type2;
        }
        if (flag1 && this.selectedObjects[0].GetParameters().Count > 0)
        {
          Rectangle selectedRectangle = this.GetSelectedRectangle(inTransformedLevelSpace: true);
          Vector2 virtualSize = Utility.VirtualSize;
          Rectangle globalRect1 = this.parameterMenuToggle.GlobalRect;
          int num1 = (this.selectedObjects[0].IsScalable ? 14 : 0) + 10;
          bool flag2 = (double) (selectedRectangle.X + selectedRectangle.Width + num1 + 20 + globalRect1.Width + this.parameterMenu.Width) > (double) virtualSize.X;
          bool flag3 = (double) (selectedRectangle.Y + this.parameterMenu.Height + 10) > (double) virtualSize.Y;
          bool flag4 = selectedRectangle.Y < 10;
          int num2 = flag2 ? selectedRectangle.X - num1 - globalRect1.Width : selectedRectangle.X + selectedRectangle.Width + num1;
          int num3 = flag4 ? selectedRectangle.Y + selectedRectangle.Height - globalRect1.Height : selectedRectangle.Y;
          this.parameterMenuToggle.SetActive(true);
          this.parameterMenuToggle.SetPosition(new int?(num2), new int?(num3));
          Rectangle globalRect2 = this.parameterMenuToggle.GlobalRect;
          this.parameterMenu.SetPosition(new int?(flag2 ? -10 - this.parameterMenu.Width : globalRect2.Width + 10), new int?(flag3 ? globalRect2.Height - this.parameterMenu.Height : 0));
          List<ObjectParameter> parameters = this.selectedObjects[0].GetParameters();
          int num4 = 0;
          for (int index1 = 0; index1 < 3 && index1 < parameters.Count; ++index1)
          {
            DisplaySlider parameterMenuSlider = this.parameterMenuSliders[index1];
            ObjectParameter objectParameter = parameters[index1];
            int min = int.MaxValue;
            int max = int.MinValue;
            for (int index2 = this.selectedObjects.Count - 1; index2 >= 0; --index2)
            {
              ObjectParameter parameter = this.selectedObjects[index2].GetParameters()[index1];
              int num5 = (int) parameter.gameObject.GetType().GetProperty(parameter.propertyName).GetValue((object) parameter.gameObject);
              if (num5 < min)
                min = num5;
              if (num5 > max)
                max = num5;
            }
            parameterMenuSlider.SetLabelText(objectParameter.name);
            parameterMenuSlider.SetMinMax(objectParameter.min, objectParameter.max);
            if (min == max)
              parameterMenuSlider.SetValue(min);
            else
              parameterMenuSlider.SetMixedValue(min, max);
            parameterMenuSlider.SetTooltip(objectParameter.tooltip ?? "null");
            parameterMenuSlider.SetActive(true);
            num4 = index1;
          }
          for (int index = num4 + 1; index < 3; ++index)
            this.parameterMenuSliders[index].SetActive(false);
        }
        else
          this.parameterMenuToggle.SetActive(false);
        this.parameterMenu.CalculateRectangle();
      }
    }

    private void EnableManualControls()
    {
      VarManager.SetBool("manualControls", true);
      PopupMenu.Instance.Inform("Move:   WASD / Arrow Keys*Hook:   F*Reset:   R", (PromptMenu.InteractionHandler) (() =>
      {
        PopupMenu.Instance.SetActive(false);
        Utility.MainPlayer?.EnableManualControls();
      }), false);
    }

    public void DisableManualControls()
    {
      VarManager.SetBool("manualControls", false);
      Utility.MainPlayer?.Kill();
    }

    private Slider.SliderChangeHandler GetSliderChangeHandler(int index)
    {
      return (Slider.SliderChangeHandler) ((v, f) => this.SliderChangeHandler(index, v));
    }

    private void SliderChangeHandler(int index, int value)
    {
      for (int index1 = this.selectedObjects.Count - 1; index1 >= 0; --index1)
      {
        List<ObjectParameter> parameters = this.selectedObjects[index1].GetParameters();
        if (index < parameters.Count)
        {
          ObjectParameter objectParameter = parameters[index];
          objectParameter.gameObject.GetType().GetProperty(objectParameter.propertyName).SetValue((object) objectParameter.gameObject, (object) value);
          GameObject.ParameterChangeHandler changeHandler = objectParameter.changeHandler;
          if (changeHandler != null)
            changeHandler(value);
        }
      }
    }

    private void DoAction(EditorAction action, bool isRedo = false)
    {
      this.ClearSelected();
      foreach (GameObject gameObject in action.objects)
        this.AddSelected(gameObject);
      action.Do();
      this.undoActions.Push(action);
      this.undoButton.SetInteractable(true);
      this.SetHasChanges(true);
      if (isRedo)
        return;
      this.redoActions.Clear();
    }

    private void Redo()
    {
      if (this.redoActions.Count <= 0)
        return;
      this.DoAction(this.redoActions.Pop(), true);
      this.PlayInteractionSound();
      if (this.redoActions.Count == 0)
        this.redoButton.SetInteractable(false);
    }

    private void Undo()
    {
      if (this.undoActions.Count <= 0)
        return;
      EditorAction editorAction = this.undoActions.Pop();
      this.ClearSelected();
      foreach (GameObject gameObject in editorAction.objects)
        this.AddSelected(gameObject);
      editorAction.Undo();
      this.redoActions.Push(editorAction);
      this.redoButton.SetInteractable(true);
      this.PlayInteractionSound();
      if (this.undoActions.Count == 0)
        this.undoButton.SetInteractable(false);
    }

    private void UndoRedoHandler()
    {
      if (!BotsAreStupid.Input.IsDown(KeyBind.Control))
        return;
      if (BotsAreStupid.Input.IsDown(KeyBind.Shift))
        this.Redo();
      else
        this.Undo();
    }

    private void SetHasChanges(bool value)
    {
      if (this.HasChanges == value)
        return;
      this.HasChanges = value;
      this.saveButton.SetText("Save" + (this.HasChanges ? "%{red}!%" : ""));
    }

    public delegate void Action(GameObject[] objects);
  }
}

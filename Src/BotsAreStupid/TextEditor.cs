// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.TextEditor
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace BotsAreStupid
{
  internal class TextEditor : BasicLevelUI
  {
    private int executableLines;
    public const int margin = 26;
    private const int headerHeight = 70;
    private const int headerPadding = 17;
    private const int headerButtonMargin = 5;
    private const int buttonAreaHeight = 80;
    private const int buttonSize = 50;
    private const int skipToLineMarkerPadding = 2;
    private const int skipIntroButtonWidth = 220;
    private const float removeLockButtonOffsetRatio = 0.1f;
    private const float removeLockButtonRatio = 0.43f;
    private const float removeLockButtonIconRatio = 0.75f;
    private const int blueprintCardWidth = 160;
    private const int blueprintCardHeight = 50;
    private const int blueprintCardPadding = 0;
    private const int cardHeight = 59;
    private const int cardPadding = 20;
    private const float interactionSoundVolume = 0.25f;
    private const string startButtonTooltip = "Start Instructions\n(Shortcut: Alt/Shift + Enter)";
    private const string resetButtonTooltip = "Reset Bot\n(Shortcut: R)";
    private UIElement window;
    private TextArea textArea;
    private CardHolder<InstructionCard> instructionHolder;
    private CardHolder<MovableCard> blueprintHolder;
    private IContentEditor currentContentEditor;
    private HelpWindow helpWindow;
    private Button startButton;
    private Button pauseButton;
    private Button stepForwardButton;
    private Button stepBackwardButton;
    private Button resetButton;
    private Button removeLockButton;
    private ContentToggleButton helpButton;
    private ContentToggleButton toolWindowButton;
    private UIElement timeInfo;
    private UIElement scriptNameDisplay;
    private Flexbox errorWindow;
    private ContentToggleButton errorButton;
    private Flexbox toolWindow;
    private bool timeInfoModeSinceLast = false;
    private int currentInstructionLine = -1;
    private List<MarkedLine> instructionLines = new List<MarkedLine>();
    private ScriptError[] errors;
    private string currentScriptName;

    public static TextEditor Instance { get; private set; }

    public int SkipToLine { get; private set; }

    public new bool IsInteractable => this.CanInteract();

    public Rectangle GlobalContentRect => this.currentContentEditor.GetGlobalContentRect();

    public int ExecutableLines
    {
      get => !VarManager.GetBool("manualControls") ? this.executableLines : 9999;
    }

    public IContentEditor ContentEditor => this.currentContentEditor;

    public string CurrentScript => this.currentScriptName;

    private bool AllowLiveChanges
    {
      get
      {
        return !StateManager.IsState(GameState.InLevel_Watch) && VarManager.GetBool("livescriptchanges");
      }
    }

    private Rectangle windowRect => this.window.GetRectangle();

    public int CurrentInstructionLine => this.currentInstructionLine;

    public MarkedLine[] InstructionLines => this.instructionLines.ToArray();

    public TextEditor(Rectangle rectangle)
      : base(rectangle, GameState.InLevel_AnyWithText)
    {
      TextEditor.Instance = this;
      this.CreateChildren();
      this.RegisterInput();
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      Player mainPlayer1 = Utility.MainPlayer;
      if (!SimulationManager.IsPaused)
      {
        for (int index = this.instructionLines.Count - 1; index >= 0; --index)
        {
          MarkedLine instructionLine = this.instructionLines[index];
          if ((double) instructionLine.timeLeft > 0.0)
          {
            float num = instructionLine.timeLeft - deltaTime;
            if ((double) num <= 0.0)
              this.instructionLines.RemoveAt(index);
            else
              this.instructionLines[index].timeLeft = num;
          }
        }
      }
      if (!this.startButton.IsInteractable && Utility.MainPlayer != null && Utility.MainPlayer.IsActive && Utility.MainPlayer.IsGrounded && ContextMenu.Instance == null)
        this.startButton.SetInteractable(true);
      if (this.timeInfoModeSinceLast)
      {
        Player mainPlayer2 = Utility.MainPlayer;
        double num = mainPlayer2 != null ? (double) mainPlayer2.ScriptInterpreter.TimeSinceLastInstruction : 0.0;
        this.timeInfo.SetText("Last Command:   " + (num == 0.0 ? num.ToString() : num.ToString("0.000", (IFormatProvider) CultureInfo.InvariantCulture)) + "s ago");
      }
      else
      {
        Player mainPlayer3 = Utility.MainPlayer;
        float num = mainPlayer3 != null ? mainPlayer3.PlayTime : 0.0f;
        this.timeInfo.SetText("Time:   " + ((double) num == 0.0 ? num.ToString() : num.ToString("0.000", (IFormatProvider) CultureInfo.InvariantCulture)) + "s");
      }
    }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            bool flag = this.CanInteract();
            Color redColor = ColorManager.GetColor("red");
            Color whiteColor = ColorManager.GetColor("white");
            //BitmapFont font = TextureManager.GetFont("megaMan2Small");
            int height = 24;//(int) font.MeasureStringHalf("a").Height;
            Utility.DrawText(spriteBatch, /*font*/default, "Length:   " +
                this.ExecutableLines.ToString() + " Lines",
                new Rectangle(this.rectangle.X + this.rectangle.Width - 26 - 300,
                this.rectangle.Y + this.rectangle.Height - height - 8, 300, height),
                whiteColor, false, true, layerDepth: 1f);
            ScriptError? outError;
            if (!flag || (double)this.HoverTime <= 0.5 
                || !Utility.MouseInside(this.textArea.GetGlobalContentRect()) 
                || !this.IsErroneousLine(this.textArea.GetLineFromPos(new Vector2?(),
                false, false, -1), out outError))
                return;
            Vector2 pos = this.LocalMousePos();
            ColorScheme currentScheme = ColorManager.CurrentScheme;
            Utility.DrawText(spriteBatch, pos, new Styling()
            {
                defaultTextColor = currentScheme.weakenedAccentColor,
                defaultColor = currentScheme.backgroundColor,
                margin = 20,
                borderWidth = 2,
                padding = 5,
                //font = TextureManager.GetFont("courier"),
                text = "Error: " + outError?.message,
                borderColor = currentScheme.textColor
            }, 15);
        }

    private void DrawMarkedText(
      SpriteBatch spriteBatch,
      int line,
      Rectangle lineRect,
      int lineNumberX,
      out bool whiteNumberText)
    {
      ColorScheme currentScheme = ColorManager.CurrentScheme;
      Color accentColor = currentScheme.accentColor;
      Color selectionColor = currentScheme.selectionColor;
      Color color = ColorManager.GetColor("orange");
      whiteNumberText = false;
      Rectangle globalRect = this.GlobalRect;
      if (line == this.SkipToLine)
      {
        int num = 3;
        Vector2 start = new Vector2((float) lineNumberX, (float) (lineRect.Y - num));
        Vector2 end = new Vector2((float) (this.windowRect.X + this.windowRect.Width), (float) (lineRect.Y - num));
        Utility.DrawLine(spriteBatch, start, end, num, new Color?(color), 0.0f, 0);
        this.textArea.DrawMarkedLineNumber(spriteBatch, line, color, num);
        whiteNumberText = true;
      }
      ScriptError? outError;
      if (this.IsErroneousLine(line, out outError))
      {
        int argPosition = ScriptParser.GetArgPosition(this.textArea.GetLineString(line), outError.Value.parameter);
        int startChar = argPosition <= 0 ? 0 : argPosition;
        this.textArea.DrawUnderline(spriteBatch, line, Color.Red, startChar);
      }
      for (int index = this.instructionLines.Count - 1; index >= 0; --index)
      {
        if (this.instructionLines[index].line == line)
        {
          float alpha = this.instructionLines[index].timeLeft / this.instructionLines[index].lifeTime;
          this.textArea.DrawMarkedLine(spriteBatch, line, selectionColor, alpha);
        }
      }
      if (this.currentInstructionLine != line)
        return;
            this.textArea.DrawMarkedLine(spriteBatch, line, accentColor, 
                (float)(Utility.MainPlayer.ScriptInterpreter.GetWaitPercent() ?? 1.0), new Color?(selectionColor));
    }

    private void DrawMarkedCard(
      SpriteBatch spriteBatch,
      int line,
      Rectangle lineRect,
      Rectangle positionRect)
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = Utility.ChangeColor(color1, 0.5f);
      Color? nullable = new Color?();
      if (line > 0 && line == this.SkipToLine)
      {
        Rectangle globalRect = this.instructionHolder.GlobalRect;
        int y = lineRect.Y - 10 - 4;

        Utility.DrawLine(spriteBatch, new Vector2((float) globalRect.X,
            (float) y), new Vector2((float) (globalRect.X + globalRect.Width), (float) y),
            8, new Color?(color1), 0.0f, 0);
      }
      for (int index = this.instructionLines.Count - 1; index >= 0; --index)
      {
        if (this.instructionLines[index].line == line)
        {
          float amount = this.instructionLines[index].timeLeft / this.instructionLines[index].lifeTime;
          nullable = new Color?(Color.Lerp(ColorManager.GetColor("white"), color2, amount));
        }
      }
      if (this.currentInstructionLine == line)
        nullable = new Color?(Color.Lerp(color2, color1, 
            (float) (Utility.MainPlayer.ScriptInterpreter.GetWaitPercent() ?? 1.0)));

      if (nullable.HasValue)
        Utility.DrawRect(spriteBatch, lineRect, nullable.Value);
      ScriptError? outError;
      if (!this.IsErroneousLine(line, out outError))
        return;
      string lineString = this.instructionHolder.GetLineString(line);
      int parameter = outError.Value.parameter;
      int num = Enumerable.Count<char>(Enumerable.TakeWhile<char>((IEnumerable<char>) lineString, (Func<char, bool>) (c => (parameter -= c == ' ' ? 1 : 0) > 0)));
      int startChar = num <= 0 ? 0 : num + 1;
      this.instructionHolder.DrawUnderline(spriteBatch, line, Color.Red, startChar);
      Utility.DrawRect(spriteBatch, positionRect, color1);
    }

    protected override void DrawBorder(SpriteBatch spriteBatch)
    {
      if (this.CanInteract())
        Utility.DrawOutline(spriteBatch, this.window.GlobalRect, 4, ColorManager.GetColor("red"));
      base.DrawBorder(spriteBatch);
    }

    public string[] GetInstructionArray() => this.currentContentEditor.GetContent();

    public string GetFullInstructions() => string.Join("|", this.GetInstructionArray());

    public void SetInstructionLine(int index, int furthestIndex = -1, bool loadSaved = false)
    {
      if (furthestIndex == -1)
        furthestIndex = index;
      if (index == -1)
      {
        this.instructionLines.Clear();
      }
      else
      {
        this.instructionLines.RemoveAll((Predicate<MarkedLine>) (il => il.line == this.currentInstructionLine));
        this.instructionLines.Add(new MarkedLine(this.currentInstructionLine, (float) VarManager.GetInt("linefadetime") / 100f));
      }
      this.currentContentEditor.SetFirstEditableLine(furthestIndex + 1);
      this.currentInstructionLine = index;
      if (!this.AllowLiveChanges || StateManager.IsState(GameState.InLevel_Watch))
        this.currentContentEditor.ShowLine(this.currentInstructionLine, loadSaved);
      this.currentContentEditor.UpdateView(true);
    }

    public void SetInstructionLines(MarkedLine[] newInstructionLines)
    {
      this.instructionLines = new List<MarkedLine>((IEnumerable<MarkedLine>) newInstructionLines);
    }

    public void Reset(bool fromButton = false)
    {
      this.SetInstructionLine(-1);
      this.pauseButton.SetActive(false);
      this.startButton.SetActive(true);
      this.startButton.SetTooltip("Start Instructions\n(Shortcut: Alt/Shift + Enter)");
      this.stepForwardButton.SetActive(false);
      this.stepBackwardButton.SetActive(false);
      this.startButton.SetInteractable(false);
      this.currentContentEditor.LoadScroll();
    }

    public void Clear(bool addEmpty = true)
    {
      this.Reset();
      this.SkipToLine = -1;
      this.SetErrors((ScriptError[]) null);
      this.currentContentEditor.Clear(addEmpty);
    }

    public void AddLine(string s, bool force = false)
    {
      if (!(this.CanInteract() | force))
        return;
      this.currentContentEditor.AddLine(s, force: force);
    }

    public void AddLine(string s, Vector2 position)
    {
      if (!this.CanInteract())
        return;
      this.currentContentEditor.AddLine(s, new Vector2?(position));
      if (this.currentContentEditor == this.textArea)
        this.textArea.JumpToEndOfLine();
      this.PlayInteractionSound();
    }

    public void LoadInstructions(string[] newLines, string scriptName = null)
    {
      this.SetCurrentScriptName(scriptName);
      if (!string.IsNullOrEmpty(this.currentScriptName) && (StateManager.IsState(GameState.InLevel_Default) || StateManager.IsState(GameState.InLevel_FromEditor)))
        ScriptManager.SaveCurrentScriptName(LevelManager.CurrentLevelName, scriptName);
      this.Clear(false);
      this.currentContentEditor.AddContent(newLines);
      this.helpButton.DisableContent();
      this.errorButton.DisableContent();
      this.SkipToLine = -1;
      this.UpdateView();
    }

    public void SetCurrentScriptName(string name, string ifEquals = null)
    {
      if (ifEquals != null && !(this.currentScriptName == ifEquals))
        return;
      this.currentScriptName = name;
      this.scriptNameDisplay.SetText(string.IsNullOrEmpty(name) ? " " : "Instructions:   " + name);
    }

    public void SetErrors(ScriptError[] errors)
    {
      this.errors = errors;
      this.errorWindow.Clear();
      //BitmapFont font = TextureManager.GetFont("courier");
      if (errors != null && errors.Length != 0)
      {
        ColorScheme currentScheme = ColorManager.CurrentScheme;
        foreach (ScriptError error in errors)
        {
          string text = Utility.BreakText(error.ToString(), 50, out int _);
          Vector2 vector2 = new Vector2();//(Vector2) font.MeasureStringHalf(text);
          this.errorWindow.Add(new UIElement(new Rectangle(0, 0, (int) vector2.X, (int) vector2.Y)));
        }
        this.errorButton.SetActive(true);
        this.errorButton.SetText(errors.Length.ToString() + " Error" + (errors.Length > 1 ? "s" : ""));
      }
      else
      {
        this.errorButton.SetActive(false);
        this.errorWindow.SetActive(false);
      }
    }

    private void RegisterInput()
    {
      Input.Register(KeyBind.Enter, new InputAction.InputHandler(this.TryStart), GameState.InLevel_AnyWithText, longPressDelay: 0.5f, ignoreTextLock: true);
      Input.Register(KeyBind.Cancel, new InputAction.InputHandler(this.TryEnd), GameState.InLevel_AnyWithText, longPressDelay: 0.5f, requireDown: new KeyBind?(KeyBind.Control));
      Input.Register(KeyBind.Right, (InputAction.InputHandler) (() =>
      {
        if (this.CanInteract() || !SimulationManager.IsPaused)
          return;
        SimulationManager.ProgressStep(Input.IsDown(KeyBind.Shift) ? 20 : 1);
      }), GameState.InLevel_AnyWithText, 0.05f, 0.2f, true);
      Input.Register(KeyBind.Left, (InputAction.InputHandler) (() =>
      {
        if (this.CanInteract() || !SimulationManager.IsPaused)
          return;
        SimulationManager.RevertStep(Input.IsDown(KeyBind.Shift) ? 5 : 1);
      }), GameState.InLevel_AnyWithText, 0.05f, 0.2f, true);
      Input.Register(KeyBind.PageDown, (InputAction.InputHandler) (() =>
      {
        if (!SimulationManager.IsPaused)
          return;
        SimulationManager.ProgressStep(Input.IsDown(KeyBind.Shift) ? 20 : 1);
      }), GameState.InLevel_AnyWithText, 0.05f, 0.2f, true);
      Input.Register(KeyBind.PageUp, (InputAction.InputHandler) (() =>
      {
        if (!SimulationManager.IsPaused)
          return;
        SimulationManager.RevertStep(Input.IsDown(KeyBind.Shift) ? 5 : 1);
      }), GameState.InLevel_AnyWithText, 0.05f, 0.2f, true);
      Input.RegisterOnDown(KeyBind.Space, new InputAction.InputHandler(this.TogglePause), GameState.InLevel_AnyWithText, 0.05f, ignoreTextLock: true);
      Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() => this.OnMouseDown()), GameState.InLevel_AnyWithText);
      Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() => this.OnMouseDown(false)), GameState.InLevel_AnyWithText, MouseButton.Right);
    }

    private void CreateChildren()
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = ColorManager.GetColor("orange");
      Color color3 = ColorManager.GetColor("white");
      Color color4 = ColorManager.GetColor("darkslate");
      Color color5 = ColorManager.GetColor("lightslate");
      Vector2 courierSize = (Vector2) TextureManager.GetFont("courier").MeasureStringHalf("a");
      Vector2 megaMan2SmallSize = (Vector2) TextureManager.GetFont("megaMan2Small").MeasureStringHalf("a");
      this.window = new UIElement(Rectangle.Empty);
      this.window.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(26, 26, parentRect.Width - 52, parentRect.Height - 52)));
      this.AddChild(this.window);
      //BitmapFont font1 = TextureManager.GetFont("megaMan2");
      Styling styling1 = new Styling();
      styling1.defaultColor = color3;
      styling1.defaultTextColor = color4;
      styling1.borderColor = color4;
      styling1.borderWidth = 4;
      //styling1.font = font1;
      styling1.centerText = true;
      styling1.hoverCursor = new bool?(true);
      Styling styling2 = styling1;
      int blueprintHolderWidth = 160;
      IEnumerable<ScriptCommand> scriptCommands = Enumerable.Where<ScriptCommand>((IEnumerable<ScriptCommand>) ScriptParser.Instance.AvailableCommands, (Func<ScriptCommand, bool>) (command => !command.HideBluePrint));
      UIElement window1 = this.window;
      Rectangle empty1 = Rectangle.Empty;
      BotsAreStupid.ContentEditor<MovableCard>.CanInteractCheck canInteractCheck1 = (BotsAreStupid.ContentEditor<MovableCard>.CanInteractCheck) (basic => this.CanInteract(basic));
      styling1 = new Styling();
      styling1.defaultColor = Utility.ChangeColor(color3, -0.5f);
      styling1.borderColor = color4;
      styling1.borderWidth = 8;
      styling1.enabledBorders = new BorderInfo(right: true);
      styling1.padding = 0;
      Styling style1 = styling1;
      Styling cardStyle1 = styling2;
      int minVisibleElements = Enumerable.Count<ScriptCommand>(scriptCommands);
      CardHolder<MovableCard> child1 = this.blueprintHolder = new CardHolder<MovableCard>(empty1, GameState.InLevel_AnyWithText, canInteractCheck1, style1, cardStyle1, 50, minVisibleElements: minVisibleElements);
      window1.AddChild((UIElement) child1);
      this.blueprintHolder.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 70, blueprintHolderWidth, parentRect.Height - 70 - 80)));
      styling2.centerText = false;
      UIElement window2 = this.window;
      Rectangle empty2 = Rectangle.Empty;
      BotsAreStupid.ContentEditor<InstructionCard>.CanInteractCheck canInteractCheck2 = (BotsAreStupid.ContentEditor<InstructionCard>.CanInteractCheck) (basic => this.CanInteract(basic));
      styling1 = new Styling();
      styling1.defaultColor = color3;
      styling1.padding = 20;
      Styling style2 = styling1;
      Styling cardStyle2 = styling2;
      CardHolder<InstructionCard> child2 = this.instructionHolder = new CardHolder<InstructionCard>(empty2, GameState.InLevel_AnyWithText, canInteractCheck2, style2, cardStyle2, 59, true, true, true, true, true, 0.25f, 3);
      window2.AddChild((UIElement) child2);
      this.instructionHolder.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(blueprintHolderWidth, 70, parentRect.Width - blueprintHolderWidth, parentRect.Height - 70 - 80)));
      this.instructionHolder.SetLineDrawModifier(new CardHolder<InstructionCard>.LineDrawModifier(this.DrawMarkedCard));
      this.instructionHolder.OnYScroll += (System.Action) (() =>
      {
        if (!SimulationManager.IsPaused || !this.AllowLiveChanges)
          return;
        this.instructionHolder.SaveScroll();
      });
      foreach (ScriptCommand scriptCommand in scriptCommands)
      {
        ScriptCommand command = scriptCommand;
        MovableCard card = (MovableCard) this.blueprintHolder.AddLine(command.Command, new Vector2?(), false);
        card.OnDrop += (MovableElement.ActionHandler) (pos =>
        {
          if (!Utility.PointInside(card.Center, this.instructionHolder.GlobalRect))
            return;
          ((InstructionCard) this.instructionHolder.AddLine(command.Command, new Vector2?(pos), false)).OpenContext();
        });
      }
      this.window.AddChild((UIElement) (this.textArea = new TextArea(Rectangle.Empty, GameState.InLevel_AnyWithText, (BotsAreStupid.ContentEditor<Line>.CanInteractCheck) (basic => this.CanInteract(basic)), enableLineNumbers: true, enableDeleteButtons: true)));
      this.textArea.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 70, this.windowRect.Width, this.windowRect.Height - 70 - 80)));
      this.textArea.SetLineDrawModifier(new TextArea.LineDrawModifier(this.DrawMarkedText));
      this.textArea.OnYScroll += (System.Action) (() =>
      {
        if (!SimulationManager.IsPaused || !this.AllowLiveChanges)
          return;
        this.textArea.SaveScroll();
      });
      this.SetCardViewActive(false);
      this.AddChild((UIElement) (this.helpWindow = new HelpWindow(new Rectangle(this.rectangle.Width - 26, 26, 0, 0))));
      this.helpWindow.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width - 26, 26, this.helpWindow.Width, this.helpWindow.Height)));
      this.helpWindow.AddResponsiveAction((UIElement.ResponsiveActionHandler) (parentRect => this.helpWindow.CalculateRectangle()));
      this.toolWindow = (Flexbox) new ToolWindow();
      //BitmapFont font2 = TextureManager.GetFont("megaMan2");
      Rectangle rectangle1 = new Rectangle(0, 0, this.windowRect.Width, 70);
      Styling styling3 = new Styling();
      styling3.defaultColor = color4;
      styling3.text = "Instructions";
      styling3.defaultTextColor = color3;
      styling3.textOffset = 17;
      //styling3.font = font2;
      Styling? style3 = new Styling?(styling3);
      UIElement child3 = new UIElement(rectangle1, style3);
      child3.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, 70)));
      styling3 = new Styling();
      styling3.defaultColor = Utility.ChangeColor(color5, 0.1f);
      styling3.hoverColor = Utility.ChangeColor(color5, 0.25f);
      styling3.clickColor = color1;
      styling3.round = true;
      Styling style4 = styling3;
      Vector2 vector2 = new Vector2();//font2.MeasureStringHalf("Instructions");
      int x = (int) vector2.X;
      int y = (int) vector2.Y;
      int headerButtonSize = 25;
      int num1 = (this.windowRect.Width - 17 - x - headerButtonSize * 2 - 5) / 2;
      child3.AddChild((UIElement) (this.helpButton = new ContentToggleButton(new Rectangle(this.windowRect.Width - 17 - headerButtonSize * 2 - 5, 17 + y / 2 - headerButtonSize / 2, headerButtonSize, headerButtonSize), (UIElement) this.helpWindow, (ContentToggleButton.EnableHandler) (() =>
      {
        if (!this.errorWindow.IsActive)
          return;
        this.errorButton.DisableContent();
      }), defaultStyling: false, buttonStyle: new Styling?(style4), createIcon: false)));
      ContentToggleButton helpButton = this.helpButton;
      Rectangle empty3 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultColor = color3;
      styling3.spritePos = TextureManager.GetSpritePos("ui_help");
      Styling? style5 = new Styling?(styling3);
      UIElement child4 = new UIElement(empty3, style5).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, parentRect.Height)));
      helpButton.AddChild(child4);
      this.helpButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width - 17 - headerButtonSize * 2 - 5, parentRect.Height / 2 - headerButtonSize / 2, headerButtonSize, headerButtonSize)));
      child3.AddChild((UIElement) (this.toolWindowButton = new ContentToggleButton(new Rectangle(this.windowRect.Width - 17 - headerButtonSize, 17 + y / 2 - headerButtonSize / 2, headerButtonSize, headerButtonSize), (UIElement) this.toolWindow, defaultStyling: false, buttonStyle: new Styling?(style4), createIcon: false)));
      this.toolWindowButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width - 17 - headerButtonSize, parentRect.Height / 2 - headerButtonSize / 2, headerButtonSize, headerButtonSize)));
      ContentToggleButton toolWindowButton = this.toolWindowButton;
      Rectangle empty4 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultColor = color3;
      styling3.spritePos = TextureManager.GetSpritePos("ui_tools");
      Styling? style6 = new Styling?(styling3);
      UIElement child5 = new UIElement(empty4, style6).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, parentRect.Height)));
      toolWindowButton.AddChild(child5);
      this.window.AddChild(child3);
      Rectangle rectangle2 = new Rectangle(0, this.windowRect.Height - 80, this.windowRect.Width, 80);
      styling3 = new Styling();
      styling3.defaultColor = color4;
      Styling? style7 = new Styling?(styling3);
      UIElement child6 = new UIElement(rectangle2, style7);
      child6.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - 80, parentRect.Width, 80)));
      int buttonY = 15;
      Rectangle rectangle3 = new Rectangle((int) ((double) this.windowRect.Width / 3.0 - 25.0), buttonY, 50, 50);
      style4.tooltip = "Pause Instructions\n(Shortcut: Space)";
      this.pauseButton = new Button(rectangle3, new Button.OnClick(this.OnPauseButtonPress), style4);
      this.pauseButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle((int) ((double) parentRect.Width / 3.0 - 25.0), buttonY, 50, 50)));
      Button pauseButton = this.pauseButton;
      Rectangle empty5 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultColor = color3;
      styling3.spritePos = TextureManager.GetSpritePos("ui_pause");
      Styling? style8 = new Styling?(styling3);
      UIElement child7 = new UIElement(empty5, style8).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p, 1f)));
      pauseButton.AddChild(child7);
      child6.AddChild((UIElement) this.pauseButton);
      style4.tooltip = "Start Instructions\n(Shortcut: Alt/Shift + Enter)";
      this.startButton = new Button(rectangle3, (Button.OnClick) (() => this.OnStartButtonPress()), (Button.OnClick) (() => this.OnStartButtonPress(true)), style4);
      this.startButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle((int) ((double) parentRect.Width / 3.0 - 25.0), buttonY, 50, 50)));
      Button startButton = this.startButton;
      Rectangle empty6 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultColor = color3;
      styling3.spritePos = TextureManager.GetSpritePos("ui_start");
      Styling? style9 = new Styling?(styling3);
      UIElement child8 = new UIElement(empty6, style9).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p, 1f)));
      startButton.AddChild(child8);
      this.startButton.SetDynamicTooltip((Func<string>) (() => "Resume Instructions\n" + Utility.ListShortcuts(!this.AllowLiveChanges || SimulationManager.HasMainScriptFinished ? "Space" : (string) null, "Alt/Shift + Enter")));
      child6.AddChild((UIElement) this.startButton);
      this.resetButton = new Button(new Rectangle((int) ((double) this.windowRect.Width / 3.0 * 2.0 - 25.0), buttonY, 50, 50), new Button.OnClick(Game1.Instance.OnResetButtonPress), new Button.OnClick(this.OnResetButtonRightPress), style4);
      this.resetButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle((int) ((double) parentRect.Width / 3.0 * 2.0 - 25.0), buttonY, 50, 50)));
      this.resetButton.SetDynamicTooltip((Func<string>) (() => "Reset Bot" + (SimulationManager.HasLockedFrame ? " to Locked Frame" : "") + "\n(Shortcut: " + (!SimulationManager.HasMainStarted || SimulationManager.IsPaused && this.AllowLiveChanges ? "Control + " : "") + "R)"));
      Button resetButton1 = this.resetButton;
      Rectangle empty7 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultColor = color3;
      styling3.spritePos = TextureManager.GetSpritePos("ui_reset");
      Styling? style10 = new Styling?(styling3);
      UIElement child9 = new UIElement(empty7, style10).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => Utility.ScaledCenteredRect(parentRect, 1f)));
      resetButton1.AddChild(child9);
      Button resetButton2 = this.resetButton;
      Rectangle empty8 = Rectangle.Empty;
      Button.OnClick onClick = new Button.OnClick(this.OnRemoveLockButtonPress);
      styling3 = new Styling();
      styling3.round = true;
      styling3.defaultColor = Utility.ChangeColor(color2, -0.1f);
      styling3.hoverColor = Utility.ChangeColor(color2, -0.2f);
      styling3.clickColor = Utility.ChangeColor(color2, -0.3f);
      styling3.tooltip = "Remove frame lock";
      Styling style11 = styling3;
      Button child10 = this.removeLockButton = new Button(empty8, onClick, style11);
      resetButton2.AddChild((UIElement) child10);
      this.removeLockButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle((int) ((double) parentRect.Width * 0.67000001668930054), (int) -((double) parentRect.Height * 0.10000000149011612), (int) ((double) parentRect.Width * 0.43000000715255737), (int) ((double) parentRect.Width * 0.43000000715255737))));
      this.removeLockButton.DrawOnTop = true;
      this.removeLockButton.AllowOccludedParentHover = false;
      Rectangle empty9 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultColor = color3;
      styling3.spritePos = TextureManager.GetSpritePos("ui_locked");
      styling3.hoverSpritePos = new Rectangle?(TextureManager.GetSpritePos("ui_unlocked"));
      Styling? style12 = new Styling?(styling3);
      UIElement child11 = new UIElement(empty9, style12).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle((int) ((double) parentRect.Width * 0.25 / 2.0) + 1, (int) ((double) parentRect.Height * 0.25 / 2.0), (int) ((double) parentRect.Width * 0.75), (int) ((double) parentRect.Height * 0.75))));
      child11.CheckParentForHover = true;
      this.removeLockButton.AddChild(child11);
      this.removeLockButton.SetActive(false);
      this.pauseButton.SetActive(false);
      child6.AddChild((UIElement) this.resetButton);
      this.window.AddChild(child6);
      int num2 = 20;
      this.stepForwardButton = new Button(new Rectangle(50 + num2 / 3, 25 - num2 / 2, num2, num2), (Button.OnClick) (() => SimulationManager.ProgressStep(Input.IsDown(KeyBind.Shift) ? 20 : 1)), style4, triggerWhileDown: true);
      Button stepForwardButton = this.stepForwardButton;
      Rectangle empty10 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.rotation = new float?(180f);
      styling3.defaultColor = color3;
      styling3.spritePos = TextureManager.GetSpritePos("ui_step");
      Styling? style13 = new Styling?(styling3);
      UIElement child12 = new UIElement(empty10, style13).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p, 1f)));
      stepForwardButton.AddChild(child12);
      this.stepForwardButton.SetDynamicTooltip((Func<string>) (() => "Next Frame\n" + Utility.ListShortcuts(!this.AllowLiveChanges || SimulationManager.HasMainScriptFinished ? "RightArrow" : (string) null, "PageDown")));
      this.startButton.AddChild((UIElement) this.stepForwardButton);
      this.stepForwardButton.SetActive(false);
      this.stepBackwardButton = new Button(new Rectangle(-num2 - num2 / 3, 25 - num2 / 2, num2, num2), (Button.OnClick) (() => SimulationManager.RevertStep()), style4, triggerWhileDown: true);
      Button stepBackwardButton = this.stepBackwardButton;
      Rectangle empty11 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultColor = color3;
      styling3.spritePos = TextureManager.GetSpritePos("ui_step");
      Styling? style14 = new Styling?(styling3);
      UIElement child13 = new UIElement(empty11, style14).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p, 1f)));
      stepBackwardButton.AddChild(child13);
      this.stepBackwardButton.SetDynamicTooltip((Func<string>) (() => "Previous Saved Frame\n" + Utility.ListShortcuts(!this.AllowLiveChanges || SimulationManager.HasMainScriptFinished ? "LeftArrow" : (string) null, "PageUp")));
      this.startButton.AddChild((UIElement) this.stepBackwardButton);
      this.stepBackwardButton.SetActive(false);
      Rectangle empty12 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.borderColor = color1;
      styling3.borderWidth = 2;
      styling3.defaultColor = color3;
      styling3.padding = 10;
      Styling? style15 = new Styling?(styling3);
      Point? center = new Point?();
      this.AddChild((UIElement) (this.errorWindow = new Flexbox(empty12, style15, center)));
      this.errorWindow.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.X + parentRect.Width + 2, parentRect.Y, 10, 10)));
      ColorManager.OnColorSchemeChange += (Action<ColorScheme>) (s => this.errorWindow.ChangeStyle(new Styling()
      {
        color = s.backgroundColor,
        defaultColor = s.backgroundColor,
        borderColor = s.accentColor
      }));
      UIElement window3 = this.window;
      Rectangle empty13 = Rectangle.Empty;
      Flexbox errorWindow = this.errorWindow;
      ContentToggleButton.EnableHandler enableHandler = (ContentToggleButton.EnableHandler) (() =>
      {
        if (!this.helpWindow.IsActive)
          return;
        this.helpButton.DisableContent();
      });
      styling3 = new Styling();
      styling3.defaultTextColor = color1;
      styling3.centerText = true;
      styling3.borderWidth = 0;
      styling3.hoverColor = Utility.ChangeColor(color1, 0.25f);
      styling3.textColorEffects = true;
      styling3.clickColor = Utility.ChangeColor(color1, 0.5f);
      Styling? buttonStyle = new Styling?(styling3);
      Styling? buttonEnabledStyle = new Styling?();
      ContentToggleButton child14 = this.errorButton = new ContentToggleButton(empty13, (UIElement) errorWindow, enableHandler, defaultStyling: false, buttonStyle: buttonStyle, buttonEnabledStyle: buttonEnabledStyle, createIcon: false);
      window3.AddChild((UIElement) child14);
      this.errorButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, this.windowRect.Height - 80 - (int) courierSize.Y * 2, this.windowRect.Width, (int) courierSize.Y)));
            //BitmapFont font3 = TextureManager.GetFont("megaMan2Small");
            int megaMan2SmallHeight = 24;//(int) font3.MeasureStringHalf("a").Height;
      styling3 = new Styling();
      //styling3.font = font3;
      styling3.defaultTextColor = color3;
      styling3.textColorEffects = true;
      styling3.hoverColor = Utility.ChangeColor(color3, -0.2f);
      styling3.tooltip = "Shows the Total Time of the script execution.\n-> Click to toggle to Time since last Command.";
      Styling style16 = styling3;
      Styling enabledStyle = style16 with
      {
        tooltip = "Shows the Time since the last Command\nwas executed.\n-> Click to toggle to Total Time."
      };
      this.AddChild(this.timeInfo = (UIElement) new ToggleButton(Rectangle.Empty, (Button.OnClick) (() => this.timeInfoModeSinceLast = true), (Button.OnClick) (() => this.timeInfoModeSinceLast = false), style16, enabledStyle));
      this.timeInfo.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.X + 26, parentRect.Y + parentRect.Height - megaMan2SmallHeight - 8, parentRect.Width / 2, megaMan2SmallHeight)));
      Rectangle empty14 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultTextColor = color3;
      //styling3.font = font3;
      styling3.centerText = true;
      Styling? style17 = new Styling?(styling3);
      this.AddChild(this.scriptNameDisplay = new UIElement(empty14, style17));
      this.scriptNameDisplay.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect =>
      {
        Rectangle uiSpaceLevelRect = StateManager.UISpaceLevelRect;
        return new Rectangle(uiSpaceLevelRect.X, uiSpaceLevelRect.Y + uiSpaceLevelRect.Height + (int) megaMan2SmallSize.Y, uiSpaceLevelRect.Width, (int) megaMan2SmallSize.Y);
      }));
    }

    public void SetRightBlueprints(bool right)
    {
      int blueprintHolderWidth = 160;
      if (right)
      {
        this.instructionHolder.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 70, parentRect.Width - blueprintHolderWidth, parentRect.Height - 70 - 80)));
        this.blueprintHolder.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width - blueprintHolderWidth, 70, blueprintHolderWidth, parentRect.Height - 70 - 80)));
      }
      else
      {
        this.instructionHolder.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(blueprintHolderWidth, 70, parentRect.Width - blueprintHolderWidth, parentRect.Height - 70 - 80)));
        this.blueprintHolder.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 70, blueprintHolderWidth, parentRect.Height - 70 - 80)));
      }
    }

    public void UpdateView()
    {
      this.currentContentEditor.UpdateView();
      this.removeLockButton.SetActive(SimulationManager.HasLockedFrame);
      this.stepBackwardButton.SetInteractable(SimulationManager.MainSimulation.CanStepBackwards);
    }

    public void UpdateAvailableCommands() => this.helpWindow.UpdateAvailableCommands();

    private void SetCardViewActive(bool active)
    {
      if (this.currentContentEditor != null)
      {
        // ISSUE: method pointer
        //this.currentContentEditor.OnViewUpdate -= this.OnContentViewUpdate;//new IContentEditor.ViewUpdateHandler((object) this, 
        //    __methodptr(\u003CSetCardViewActive\u003Eg__OnContentViewUpdate\u007C92_0));
      }
      this.instructionHolder.SetActive(active);
      this.blueprintHolder.SetActive(active);
      this.textArea.SetActive(!active);
      this.currentContentEditor = active ? (IContentEditor) this.instructionHolder : (IContentEditor) this.textArea;
            // ISSUE: method pointer
      //this.currentContentEditor.OnViewUpdate += this.OnContentViewUpdate;//new IContentEditor.ViewUpdateHandler((object) this, 
       //   __methodptr(\u003CSetCardViewActive\u003Eg__OnContentViewUpdate\u007C92_0));
    }

    private void TryStart()
    {
      if (!Input.IsDown(KeyBind.Alt) && !Input.IsDown(KeyBind.Shift))
        return;
      this.PlayInteractionSound();
      if (this.startButton.IsInteractable && (!SimulationManager.HasMainStarted || SimulationManager.IsPaused))
        this.OnStartButtonPress();
      else
        this.TryEnd();
      Input.CancelCurrentAction = true;
    }

    private void TryEnd()
    {
      if (!this.resetButton.IsInteractable || !SimulationManager.HasMainStarted)
        return;
      Game1.Instance.OnResetButtonPress();
    }

    private void OnMouseDown(bool left = true)
    {
      Utility.GetMousePos();
      bool flag = this.CanInteract();
      if (!flag)
      {
        if (VarManager.GetBool("manualControls") && Utility.MouseInside(this.GlobalRect))
        {
          PopupMenu.Instance.Confirm("Manual Controls are enabled!*Disable them?", 
              (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)),
              new PromptMenu.InteractionHandler(PopupMenu.Instance.DisableManualControls));
        }
        else
        {
          if (!Utility.MouseInside(this.currentContentEditor.GetGlobalContentRect()))
            return;
          if (SimulationManager.MainSimulation.IsScriptFinished)
            Game1.Instance.OnResetButtonPress();
          else if (this.AllowLiveChanges && !SimulationManager.IsPaused)
            this.OnPauseButtonPress();
        }
      }
      else
      {
        if (!flag || left || !Utility.MouseInside(this.currentContentEditor.GetGlobalContentRect()))
          return;
        int lineFromPos = this.currentContentEditor.GetLineFromPos();
        this.SkipToLine = this.SkipToLine != lineFromPos ? lineFromPos : -1;
        this.PlayInteractionSound(0.25f);
      }
    }

    private void OnStartButtonPress(bool right = false)
    {
      if (!this.CanInteract(true) && !StateManager.IsState(GameState.InLevel_Watch))
        return;
      if (right && !SimulationManager.IsPaused)
      {
        if (VarManager.GetBool("pauseonstart"))
          Utility.OpenContextMenu(false, (UIElement) this.startButton,
              ("Disable pause on start", (Button.OnClick) (() => VarManager.SetBool("pauseonstart", false))));
        else
          Utility.OpenContextMenu(false, (UIElement) this.startButton, 
              ("Enable pause on start", (Button.OnClick) (() => VarManager.SetBool("pauseonstart", true))));
      }
      else
        this.StartInstructions();
    }

    public void StartInstructions(bool autosave = true)
    {
      this.startButton.SetActive(false);
      this.pauseButton.SetActive(true);
      if (SimulationManager.IsPaused)
      {
        SimulationManager.SetPaused(false);
      }
      else
      {
        this.currentContentEditor.SaveScroll();
        SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation =>
        {
          if (simulation.IsIntro)
            return;
          simulation.StartASAP(autosave);
        }));
        this.helpButton.DisableContent();
        this.errorButton.DisableContent();
        if (VarManager.GetBool("pauseonstart") && this.SkipToLine <= 0)
          this.OnPauseButtonPress();
      }
    }

    public void SaveInstructions()
    {
      if (string.IsNullOrEmpty(this.currentScriptName) 
                || !StateManager.IsState(GameState.InLevel_Default)
                && !StateManager.IsState(GameState.InLevel_FromEditor))
        return;
      ScriptManager.SaveScript(LevelManager.CurrentLevelName, 
          this.currentScriptName, this.GetInstructionArray());
    }

    public void OnPauseButtonPress()
    {
      if (!this.CanInteract(true) && !StateManager.IsState(GameState.InLevel_Default))
        return;
      this.pauseButton.SetActive(false);
      this.startButton.SetActive(true);
      this.stepForwardButton.SetActive(true);
      this.stepBackwardButton.SetActive(true);
      SimulationManager.SetPaused(true);
      this.UpdateView();
    }

    private void TogglePause()
    {
      if (SimulationManager.IsPaused && (!this.AllowLiveChanges || SimulationManager.HasMainScriptFinished))
      {
        this.OnStartButtonPress();
      }
      else
      {
        if (!SimulationManager.MainSimulation.IsScriptRunning && !SimulationManager.MainSimulation.IsScriptFinished)
          return;
        this.OnPauseButtonPress();
      }
    }

    private void OnHelpButtonPress()
    {
      if (!this.CanInteract())
        return;
      this.helpWindow.ToggleActive();
      if (this.helpWindow.IsActive && this.errorWindow.IsActive)
        this.errorButton.DisableContent();
    }

    private void OnResetButtonRightPress()
    {
      if (!this.AllowLiveChanges || !SimulationManager.HasMainStarted 
                || SimulationManager.MainSimulation.IsScriptFinished || StateManager.IsState(GameState.InLevel_Watch))
        return;
      List<(string, Button.OnClick)> valueTupleList = new List<(string, Button.OnClick)>();
      valueTupleList.Add(("Lock current frame", (Button.OnClick) (() =>
      {
        SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (s => s.LockCurrentFrame()));
        this.UpdateView();
        this.stepBackwardButton.SetInteractable(false);
      })));
      if (SimulationManager.MainSimulation.HasLockedFrame)
        valueTupleList.Add(("Remove frame lock", new Button.OnClick(this.OnRemoveLockButtonPress)));
      Utility.OpenContextMenu(false, (UIElement) this.resetButton, valueTupleList.ToArray());
    }

    private void OnRemoveLockButtonPress()
    {
      SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (s => s.ResetLockedFrame()));
      Game1.Instance.OnResetButtonPress();
      Input.CancelCurrentAction = true;
      this.UpdateView();
    }

    private bool CanInteract(bool basic = false)
    {
      return this.IsCurrentState() && !PopupMenu.Instance.IsActive
                && !ScoreExplorer.Instance.IsActive 
                && (basic || !SimulationManager.HasMainStarted || SimulationManager.IsPaused
                && this.AllowLiveChanges && !SimulationManager.MainSimulation.IsScriptFinished) 
                && (basic || !StateManager.IsState(GameState.InLevel_Watch))
                && (basic || !VarManager.GetBool("manualControls"));
    }

    private bool IsErroneousLine(int line, out ScriptError? outError)
    {
      outError = new ScriptError?();
      if (this.errors == null)
        return false;
      foreach (ScriptError error in this.errors)
      {
        if (error.line - 1 == line)
        {
          outError = new ScriptError?(error);
          return true;
        }
      }
      return false;
    }
  }
}

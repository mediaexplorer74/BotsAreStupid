// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.TextArea
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
  internal class TextArea : ContentEditor<Line>
  {
    private const int maxLines = 999;
    private const int defaultLineHeight = 20;
    private int lineHeight = 20;
    private const int defaultDeleteButtonSize = 11;
    private int deleteButtonSize = 11;
    private const int defaultCursorPointSize = 6;
    private int cursorPointSize = 6;
    private const int paddingTop = 20;
    private const int paddingBottom = 20;
    private const int defaultPaddingLeft = 10;
    private int paddingLeft = 10;
    private const int paddingRight = 20;
    private const int contextMenuPadding = 10;
    private const float cursorBlinkSpeed = 6f;
    private Vector2 charSize;
    //private BitmapFont font;
    private Texture2D cursorPointTexture;
    private TextSize currentTextSize;
    private Button[] deleteButtons;
    private Scrollbar horizontalScroll;
    private UIElement deleteButtonContainer;
    private int currentLine = 0;
    private int currentChar = 0;
    private int maxVisibleChars = 0;
    private int scrollOffsetX;
    private int savedScrollOffsetX;
    private Stack<TextAreaState> undoStates = new Stack<TextAreaState>();
    private Stack<TextAreaState> redoStates = new Stack<TextAreaState>();
    private float dragScrollCooldown = 0.0f;
    private float dragScrollCooldownMax = 0.2f;
    private bool isSelectionDragging;
    private Point selectionStart;
    private Point selectionEnd;
    private float doubleClickTime;
    private float tripleClickTime;
    private const float multiClickTime = 0.2f;
    private float timeSinceAction;
    private TextArea.LineDrawModifier lineDrawModifier;
    private bool contextMenuCanAutoSelectOption;

    protected override int PaddedElementHeight => this.lineHeight;

    protected override int ElementPadding => 20;

    protected override int AvailableContentHeight => this.rectangle.Height - 20 - 20;

    protected override int AvailableContentWidth => this.rectangle.Width - this.textX - 20;

    private int lineNumberDigits => this.content.Count < 100 ? 2 : 3;

    private int deleteButtonX => !this.enableDeleteButtons ? 0 : this.paddingLeft;

    private int lineNumberX
    {
      get
      {
        return this.deleteButtonX + (this.enableDeleteButtons ? this.deleteButtonSize : 0) + (this.enableLineNumbers ? this.paddingLeft : 0);
      }
    }

    private int textX
    {
      get
      {
        return this.lineNumberX + (this.enableLineNumbers ? (int) this.charSize.X * this.lineNumberDigits : 0) + this.paddingLeft;
      }
    }

    private bool HasSelection => !this.selectionStart.Equals(this.selectionEnd);

    private Point SelectionStart
    {
      get
      {
        return this.selectionEnd.X <= this.selectionStart.X && (this.selectionStart.X > this.selectionEnd.X || this.selectionEnd.Y <= this.selectionStart.Y) ? this.selectionEnd : this.selectionStart;
      }
    }

    private Point SelectionEnd
    {
      get
      {
        return this.selectionEnd.X <= this.selectionStart.X && (this.selectionEnd.X != this.selectionStart.X || this.selectionEnd.Y < this.selectionStart.Y) ? this.selectionStart : this.selectionEnd;
      }
    }

    public TextArea(
      Rectangle rectangle,
      GameState gameState,
      ContentEditor<Line>.CanInteractCheck canInteractCheck,
      string allowedChars = "[def]!/().-",
      bool enableLineNumbers = false,
      bool enableDeleteButtons = false,
      Styling? style = null)
      : base(rectangle, gameState, canInteractCheck, allowedChars, (enableLineNumbers ? 1 : 0) != 0, (enableDeleteButtons ? 1 : 0) != 0, true, style: new Styling?(Styling.AddTo(new Styling()
      {
        defaultColor = ColorManager.GetColor("white"),
        defaultTextColor = Color.Black
      }, new Styling?(style ?? Styling.Null))))
    {
      this.SetTextSize((TextSize) VarManager.GetInt("textsize"));
      this.AddResponsiveAction((UIElement.ResponsiveActionHandler) (parentRect => this.UpdateCharSize()));
      this.content.Add(new Line());
      this.CreateChildren();
      VarManager.AddListener("textsize", (VarManager.VarChangeHandler) (v => this.SetTextSize((TextSize) int.Parse(v))));
    }

    protected override void RegisterInput(GameState gameState)
    {
      base.RegisterInput(gameState);
      Input.RegisterTextHandler(new Input.TextInputHandler(this.HandleTextInput), "[def]!?/().,-*#");
      Input.Register(KeyBind.Up, (InputAction.InputHandler) (() => this.PreviousLine(true)), gameState, 0.07f, 0.2f, true);
      Input.Register(KeyBind.Down, (InputAction.InputHandler) (() => this.NextLine(canChangeScroll: true)), gameState, 0.07f, 0.2f, true);
      Input.Register(KeyBind.Left, (InputAction.InputHandler) (() => this.PreviousChar(canChangeScroll: true)), gameState, 0.05f, 0.2f, true);
      Input.Register(KeyBind.Right, (InputAction.InputHandler) (() => this.NextChar(canChangeScroll: true)), gameState, 0.05f, 0.2f, true);
      Input.Register(KeyBind.Enter, (InputAction.InputHandler) (() => this.NewLine(isManual: true)), gameState, longPressDelay: 0.5f, ignoreTextLock: true);
      Input.Register(KeyBind.DestroyObject, (InputAction.InputHandler) (() => this.RemoveChar()), gameState, 0.05f, ignoreTextLock: true);
      Input.Register(KeyBind.DeleteChar, (InputAction.InputHandler) (() => this.RemoveChar(front: true)), gameState, 0.05f, ignoreTextLock: true);
      Input.Register(KeyBind.Home, (InputAction.InputHandler) (() => this.FirstChar()), gameState, longPressDelay: 0.2f, ignoreTextLock: true);
      Input.Register(KeyBind.Screenshot, (InputAction.InputHandler) (() => this.LastChar()), gameState, longPressDelay: 0.2f, ignoreTextLock: true);
      Input.Register(KeyBind.Cancel, new InputAction.InputHandler(this.CopySelection), gameState, ignoreTextLock: true, requireDown: new KeyBind?(KeyBind.Control));
      Input.Register(KeyBind.SpawnBooster, new InputAction.InputHandler(this.CutSelection), gameState, ignoreTextLock: true, requireDown: new KeyBind?(KeyBind.Control));
      Input.Register(KeyBind.SpawnExplosion, new InputAction.InputHandler(this.Paste), gameState, ignoreTextLock: true, requireDown: new KeyBind?(KeyBind.Control));
      Input.Register(KeyBind.GoLeft, new InputAction.InputHandler(this.SelectAll), gameState, ignoreTextLock: true, requireDown: new KeyBind?(KeyBind.Control));
      Input.Register(KeyBind.UndoRedo, (InputAction.InputHandler) (() =>
      {
        if (Input.IsDown(KeyBind.Shift))
          this.Redo();
        else
          this.Undo();
      }), gameState, 0.075f, ignoreTextLock: true, requireDown: new KeyBind?(KeyBind.Control));
      Input.RegisterOnDown(KeyBind.Tab, new InputAction.InputHandler(this.AutoCompleteLine), gameState, ignoreTextLock: true);
      Input.RegisterOnScroll(new Input.MouseScrollHandler(this.OnHorizontalMouseScroll), true);
      Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() => this.OnMouseDown()), gameState);
      Input.RegisterMouse(InputEvent.OnUp, new InputAction.InputHandler(this.OnMouseUp), gameState);
    }

    private void CreateChildren()
    {
      if (this.enableDeleteButtons)
      {
        this.deleteButtonContainer = new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 20, this.deleteButtonSize, parentRect.Height - 20 - 20)));
        this.AddChild(this.deleteButtonContainer);
        this.AddResponsiveAction((UIElement.ResponsiveActionHandler) (parentRect => this.CreateDeleteButtons()));
      }
      this.AddChild((UIElement) (this.horizontalScroll = new Scrollbar(Rectangle.Empty, 0, 0, (Scrollbar.ChangeHandler) (offset => this.scrollOffsetX = offset), vertical: false)));
      this.horizontalScroll.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - 15, parentRect.Width, 15)));
      this.horizontalScroll.AddResponsiveAction((UIElement.ResponsiveActionHandler) (parentRect => this.horizontalScroll.SetDisplaySize(this.maxVisibleChars, true)));
    }

    public override void Update(float deltaTime)
    {
      if ((double) this.dragScrollCooldown > 0.0)
        this.dragScrollCooldown -= deltaTime;
      if ((double) this.doubleClickTime > 0.0)
        this.doubleClickTime -= deltaTime;
      if ((double) this.tripleClickTime > 0.0)
        this.tripleClickTime -= deltaTime;
      this.timeSinceAction += deltaTime;
      if (!this.isSelectionDragging || !this.CanInteract())
        return;
      this.currentLine = this.GetLineFromPos(new Vector2?(), true, false, -1);
      this.currentChar = this.GetCharFromMouse(this.currentLine);
      this.selectionEnd = new Point(this.currentLine, this.currentChar);
      Vector2 mousePos = Utility.GetMousePos();
      float num1 = this.dragScrollCooldown;
      int num2 = 20;
      int num3 = this.rectangle.Height - 20;
      if ((double) this.dragScrollCooldown <= 0.0 && this.scrollOffsetY > 0 && this.currentLine <= this.scrollOffsetY && (double) mousePos.Y < (double) num2)
      {
        --this.scrollOffsetY;
        num1 = this.dragScrollCooldownMax;
      }
      else if ((double) this.dragScrollCooldown <= 0.0 && this.currentLine >= this.scrollOffsetY + this.maxVisibleLines - 1 && (double) mousePos.Y > (double) num3)
      {
        ++this.scrollOffsetY;
        num1 = this.dragScrollCooldownMax;
      }
      int num4 = this.rectangle.Width - 20;
      if ((double) this.dragScrollCooldown <= 0.0 && this.scrollOffsetX > 0 && this.currentChar <= this.scrollOffsetX && (double) mousePos.X < (double) this.textX)
      {
        --this.scrollOffsetX;
        num1 = this.dragScrollCooldownMax;
      }
      else if ((double) this.dragScrollCooldown <= 0.0 && this.currentChar >= this.scrollOffsetX + this.maxVisibleChars - 1 && (double) mousePos.X > (double) num4)
      {
        ++this.scrollOffsetX;
        num1 = this.dragScrollCooldownMax;
      }
      if ((double) num1 != (double) this.dragScrollCooldown)
      {
        this.PlayInteractionSound();
        this.dragScrollCooldown = num1;
      }
      this.UpdateView(false);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
      ColorScheme currentScheme = ColorManager.CurrentScheme;
      Color backgroundColor = currentScheme.backgroundColor;
      Color textColor = currentScheme.textColor;
      Color commentColor = currentScheme.commentColor;
      Color cursorColor = currentScheme.cursorColor;
      Color selectionColor = currentScheme.selectionColor;
      Color color1 = ColorManager.GetColor("red");
      Rectangle globalRect = this.GlobalRect;
      Rectangle globalContentRect = this.GetGlobalContentRect();
      int availableContentWidth = this.AvailableContentWidth;
      bool flag = this.CanInteract();
      if (!StateManager.IsState(GameState.InLevel_Watch) && VarManager.GetBool("livescriptchanges") && this.firstEditableLine != -1 && this.scrollOffsetY < this.firstEditableLine)
      {
        Color color2 = new Color(Color.Black, 0.075f);
        Rectangle rectangle = globalRect;
        int num = this.firstEditableLine - this.scrollOffsetY;
        if (num < this.maxVisibleLines)
          rectangle.Height = 20 + num * this.lineHeight;
        Utility.DrawRect(spriteBatch, rectangle, color2);
      }
      if (flag && this.HasSelection && (this.selectionStart.X != this.selectionEnd.X || this.IsInView(this.selectionStart) || this.IsInView(this.selectionEnd)))
      {
        Color currsentSelectionColor = !this.isSelectionDragging ? selectionColor : cursorColor;
        int padding = 0;
        this.ForEachSelection((TextArea.SelectionAction) ((line, start, end) => this.DrawMarkedLine(spriteBatch, line, currsentSelectionColor, padding: padding, startChar: start, endChar: end)));
      }
      else
      {
        Line line;
        if (flag && this.IsInView(this.currentLine, this.currentChar) && this.TryGetLine(this.currentLine, out line))
        {
          string text = line.GetSpan(this.scrollOffsetX, this.currentChar).Replace(' ', 'a');
          Rectangle rectangle = new Rectangle(globalRect.X + this.textX 
              + /*(int) this.font.MeasureStringHalf(text).Width*/24
              , globalRect.Y + 20 + Math.Max(0, this.currentLine - this.scrollOffsetY) * this.lineHeight, this.lineHeight / 6, (int) this.charSize.Y);
          Color color3 = Math.Sin((double) this.timeSinceAction * 6.0) > 0.0 ? new Color(cursorColor, 0.5f) : Color.Transparent;
          Utility.DrawRect(spriteBatch, rectangle, color3);
        }
      }
      for (int index = 0; index < this.maxVisibleLines; ++index)
      {
        int num = this.scrollOffsetY + index;
        if (num >= 0 && num < this.content.Count)
        {
          Line line1 = this.content[num];
          string line2 = line1.ToString();
          string str1 = this.scrollOffsetX <= line1.Length ? line2.Substring(this.scrollOffsetX, Math.Min(line1.Length - this.scrollOffsetX, this.maxVisibleChars + 1)) : "";
          int x = globalRect.X + this.textX;
          int y = globalRect.Y + 20 + index * this.lineHeight;
          Rectangle lineRect = new Rectangle(x, y, availableContentWidth, this.lineHeight);
          bool whiteNumberText = false;
          TextArea.LineDrawModifier lineDrawModifier = this.lineDrawModifier;
          if (lineDrawModifier != null)
            lineDrawModifier(spriteBatch, num, lineRect, globalRect.X + this.lineNumberX, out whiteNumberText);
          if (this.enableLineNumbers)
          {
            string str2 = (num + 1).ToString();
            Utility.DrawText(spriteBatch, new Vector2((float) (globalRect.X + this.lineNumberX), (float) (y + 1)), new Styling()
            {
              //font = this.font,
              defaultTextColor = whiteNumberText ? backgroundColor : textColor,
              text = str2
            });
          }
          Color color4 = line1.InBlockComment || ScriptParser.IsSingleComment(line2) ? commentColor : textColor;
          if (num < this.firstEditableLine)
            color4 = Utility.ChangeColor(color4, 0.25f);
          //BitmapFontExtensions.DrawString(spriteBatch, this.font, str1, new Vector2((float) x, (float) y), color4, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1f, new Rectangle?());
        }
      }
      if (!flag || MovableElement.CurrentMovedObject == null || !Utility.MouseInside(this.GetGlobalContentRect()))
        return;
      int lineFromPos = this.GetLineFromPos(new Vector2?(Utility.GetMousePos()), true, true, 0);
      int y1 = globalRect.Y + 20 + (lineFromPos - this.scrollOffsetY) * this.lineHeight;
      if (lineFromPos >= 0 && lineFromPos < this.content.Count && this.content[lineFromPos].IsEmpty)
      {
        int num = this.content[lineFromPos].BlockCharAmount * (int) this.charSize.X;
        Utility.DrawRect(spriteBatch, new Rectangle(globalContentRect.X + num, y1, Math.Min(globalContentRect.Width, MovableElement.CurrentMovedObject.Width), this.lineHeight), color1);
      }
      else if (this.content.Count > 0 && lineFromPos >= this.content.Count && this.content[this.content.Count - 1].IsEmpty)
      {
        int y2 = globalRect.Y + 20 + (this.content.Count - 1 - this.scrollOffsetY) * this.lineHeight;
        int num = this.content[this.content.Count - 1].BlockCharAmount * (int) this.charSize.X;
        Utility.DrawRect(spriteBatch, new Rectangle(globalContentRect.X + num, y2, Math.Min(globalContentRect.Width, MovableElement.CurrentMovedObject.Width), this.lineHeight), color1);
      }
      else
      {
        int y3 = y1 - 2;
        Utility.DrawLine(spriteBatch, new Vector2((float) globalRect.X, (float) y3), new Vector2((float) (globalRect.X + globalRect.Width), (float) y3), 4, new Color?(color1), 0.0f, 0);
      }
    }

    public void DrawMarkedLine(
      SpriteBatch spriteBatch,
      int line,
      Color color,
      float alpha = 1f,
      Color? fadeTo = null,
      int padding = 4,
      int startChar = -1,
      int endChar = 2147483647)
    {
      if (line >= this.content.Count || line < this.scrollOffsetY || line >= this.scrollOffsetY + this.maxVisibleLines)
        return;
      Line line1 = this.content[line];
      startChar = MathHelper.Clamp(startChar, line1.BlockCharAmount, line1.Length);
      startChar = MathHelper.Clamp(startChar, this.scrollOffsetX, this.scrollOffsetX + this.maxVisibleChars);
      endChar = MathHelper.Clamp(Math.Min(endChar, line1.Length), this.scrollOffsetX, this.scrollOffsetX + this.maxVisibleChars);
      if (endChar == line1.Length && endChar != startChar)
        --endChar;
      Rectangle globalRect = this.GlobalRect;
      string text = line1.GetSpan(this.scrollOffsetX, startChar).Replace(' ', 'a');
      Rectangle rectangle = new Rectangle(globalRect.X + this.textX - padding 
          + /*(int) this.font.MeasureStringHalf(text).Width*/24 - 2, 
          globalRect.Y + 20 + (line - this.scrollOffsetY) * this.lineHeight - 1, 
          /*(int) this.font.MeasureStringHalf(line1.GetSpan(startChar, endChar + 1).Replace(' ', 'a')).Width*/24
          + padding * 2 + 6, this.lineHeight);
      Color color1 = Color.Lerp(fadeTo ?? Color.Transparent, color, alpha);
      Utility.DrawRect(spriteBatch, rectangle, color1);
    }

    public void DrawUnderline(
      SpriteBatch spriteBatch,
      int line,
      Color color,
      int startChar = 0,
      int endChar = 2147483647)
    {
      if (line >= this.content.Count || line < this.scrollOffsetY || line >= this.scrollOffsetY + this.maxVisibleLines)
        return;
      Line line1 = this.content[line];
      startChar = MathHelper.Clamp(startChar, line1.BlockCharAmount, line1.Length);
      startChar = MathHelper.Clamp(startChar, this.scrollOffsetX, this.scrollOffsetX + this.maxVisibleChars);
      endChar = MathHelper.Clamp(Math.Min(endChar, line1.Length), this.scrollOffsetX, this.scrollOffsetX + this.maxVisibleChars);
      if (endChar == line1.Length && endChar != startChar)
        --endChar;
      Rectangle globalRect = this.GlobalRect;
      string text = line1.GetSpan(this.scrollOffsetX, startChar).Replace(' ', 'a');
      Rectangle rectangle = new Rectangle(globalRect.X + this.textX + /*(int) this.font.MeasureStringHalf(text).Width*/24, 
          globalRect.Y + 20 + (line - this.scrollOffsetY) * this.lineHeight - 1,
          /*(int) this.font.MeasureStringHalf(line1.GetSpan(startChar, endChar + 1).Replace(' ', 'a')).Width*/24,
          this.lineHeight);
      Vector2 start = new Vector2((float) rectangle.X, (float) (rectangle.Y + rectangle.Height - 4));
      Vector2 end = new Vector2((float) (rectangle.X + rectangle.Width), (float) (rectangle.Y + rectangle.Height - 4));
      Utility.DrawSquigglyLine(spriteBatch, start, end, 6, new Color?(color), 0.25f, 8);
    }

    public void DrawMarkedLineNumber(SpriteBatch spriteBatch, int line, Color color, int padding = 2)
    {
      if (line >= this.content.Count || line < this.scrollOffsetY || line >= this.scrollOffsetY + this.maxVisibleLines)
        return;
      Rectangle globalRect = this.GlobalRect;
      int num = globalRect.Y + 20 + (line - this.scrollOffsetY) * this.lineHeight;
      int length = (line + 1).ToString().Length;
      Rectangle rectangle = new Rectangle(globalRect.X + this.lineNumberX - padding, num - padding, (int) this.charSize.X * length + 2 * padding, this.lineHeight + 2 * padding);
      Utility.DrawRect(spriteBatch, rectangle, color);
    }

    public override void LoadScroll()
    {
      base.LoadScroll();
      this.scrollOffsetX = this.savedScrollOffsetX;
    }

    public override void SaveScroll()
    {
      base.SaveScroll();
      this.savedScrollOffsetX = this.scrollOffsetX;
    }

    public override void Clear(bool addEmpty = true)
    {
      this.currentChar = 0;
      this.currentLine = 0;
      this.savedScrollOffsetX = 0;
      this.scrollOffsetX = 0;
      this.ClearSelection();
      this.undoStates.Clear();
      this.redoStates.Clear();
      base.Clear(addEmpty);
    }

    public override object AddLine(string lineContent, Vector2? position = null, bool force = false)
    {
      if (this.CanInteract() | force)
      {
        this.SaveCurrentState();
        this.contextMenuCanAutoSelectOption = true;
        int val1 = position.HasValue ? this.GetLineFromPos(new Vector2?(position.Value), true, true, 0) : -1;
        if (val1 >= 0)
        {
          this.currentLine = val1;
          if (val1 < this.content.Count && this.content[this.currentLine].IsEmpty)
            this.currentChar = 0;
          else if (this.content.Count > 0 && val1 >= this.content.Count && this.content[this.content.Count - 1].IsEmpty)
          {
            this.currentLine = this.content.Count - 1;
            this.currentChar = 0;
          }
          else
          {
            Line line = new Line(lineContent);
            this.content.Insert(Math.Min(val1, this.content.Count), line);
            this.currentChar = int.MaxValue;
            this.UpdateView(false);
            return (object) line;
          }
        }
        if (this.currentChar != 0 || this.currentLine >= this.content.Count || !this.content[this.currentLine].IsEmpty)
          this.NewLine(true, canStart: false);
        if (this.content[this.currentLine].IsEmpty)
        {
          this.AddToCurrentLine(lineContent);
          this.UpdateView(false);
          return (object) this.content[this.currentLine];
        }
      }
      return (object) null;
    }

    public override string[] GetContent()
    {
      string[] content = new string[this.content.Count];
      for (int index = 0; index < this.content.Count; ++index)
        content[index] = this.content[index].ToStringRaw();
      return content;
    }

    public override string GetLineString(int index)
    {
      return index < this.content.Count ? this.content[index].ToStringRaw() : (string) null;
    }

    public bool TryGetLine(int index, out Line line)
    {
      if (index >= 0 && index < this.content.Count)
      {
        line = this.content[index];
        return true;
      }
      line = (Line) null;
      return false;
    }

    public override void AddContent(string[] newContent)
    {
      bool flag1 = false;
      if (newContent != null)
      {
        for (int index = 0; index < newContent.Length; ++index)
        {
          string str = newContent[index];
          bool flag2 = string.IsNullOrWhiteSpace(str);
          if (!flag1 && !flag2)
            flag1 = true;
          if (flag1 && (!flag2 || index != newContent.Length - 1))
            this.content.Add(new Line(str.Trim()));
        }
      }
      if (!flag1)
        this.content.Add(new Line());
      this.currentLine = this.content.Count - 1;
      if (this.currentLine > -1)
      {
        this.currentChar = StateManager.IsState(GameState.InLevel_Watch) ? 0 : this.content[this.currentLine].Length;
        this.ClearSelection();
      }
      this.UpdateView(false);
    }

    public void JumpToEndOfLine()
    {
      this.currentChar = int.MaxValue;
      this.UpdateView(false);
    }

    private void SetTextSize(TextSize size)
    {
      switch (size)
      {
        case TextSize.Small:
          //this.font = TextureManager.GetFont("courier");
          this.lineHeight = 20;
          this.deleteButtonSize = 11;
          this.cursorPointSize = 6;
          this.paddingLeft = 10;
          break;
        case TextSize.Medium:
         // this.font = TextureManager.GetFont("couriermedium");
          this.lineHeight = 28;
          this.deleteButtonSize = 15;
          this.cursorPointSize = 8;
          this.paddingLeft = 14;
          break;
        case TextSize.Large:
          //this.font = TextureManager.GetFont("courierbig");
          this.lineHeight = 40;
          this.deleteButtonSize = 22;
          this.cursorPointSize = 12;
          this.paddingLeft = 20;
          break;
      }
      this.currentTextSize = size;
      this.UpdateCharSize();
      this.CreateDeleteButtons();
    }

    public override void UpdateView(bool ignoreContext = false)
    {
      if (this.content.Count == 0)
        return;
      if (this.content.Count > 999)
        this.content.RemoveRange(999, this.content.Count - 999);
      if (SimulationManager.IsPaused && !SimulationManager.MainSimulation.IsScriptFinished && VarManager.GetBool("livescriptchanges") && this.content.Count == this.firstEditableLine)
        this.AddLine("", new Vector2?(new Vector2(0.0f, (float) this.content.Count)), true);
      int num = 0;
      bool flag1 = false;
      for (int index = 0; index < this.content.Count; ++index)
      {
        Line line = this.content[index];
        string str = line.ToStringRaw().Trim();
        line.InBlockComment = flag1;
        bool flag2 = false;
        bool flag3 = false;
        if (flag1)
        {
          if (ScriptParser.IsBlockCommentEnd(str))
          {
            flag1 = false;
            flag3 = true;
          }
        }
        else if (ScriptParser.IsBlockCommentStart(str))
        {
          flag2 = true;
          line.InBlockComment = true;
          if (ScriptParser.IsBlockCommentEnd(str))
            flag3 = true;
          else
            flag1 = true;
        }
        int blockDepth = line.BlockDepth;
        if (num > 0 && !flag1 && ScriptParser.IsBlockEnd(str))
          --num;
        line.BlockDepth = num;
        if (flag2 && !flag3 || !flag1 && ScriptParser.IsBlockStart(str))
          ++num;
        if (flag3 && str.Length > 2)
          ++line.BlockDepth;
        if (index == this.currentLine)
        {
          if (blockDepth < line.BlockDepth)
            this.currentChar += 3;
          else if (blockDepth > line.BlockDepth)
            this.currentChar -= 3;
        }
      }
      int min = this.CanInteract() ? MathHelper.Clamp(this.firstEditableLine, 0, this.content.Count - 1) : 0;
      this.currentLine = this.content.Count > 0 ? MathHelper.Clamp(this.currentLine, min, this.content.Count - 1) : 0;
      Line line1 = this.content[this.currentLine];
      if (this.currentLine == min - 1)
        this.currentChar = line1.Length;
      this.currentChar = MathHelper.Clamp(this.currentChar, line1.BlockCharAmount, line1.Length);
      if (this.currentChar > this.scrollOffsetX + this.maxVisibleChars - 1)
        this.scrollOffsetX = this.currentChar - this.maxVisibleChars;
      if (this.currentChar < this.scrollOffsetX)
        this.scrollOffsetX = this.currentChar;
      if (line1.Length < this.maxVisibleChars || this.scrollOffsetX < line1.BlockCharAmount)
        this.scrollOffsetX = 0;
      this.scrollOffsetX = MathHelper.Clamp(this.scrollOffsetX, 0, Math.Max(0, line1.Length - this.maxVisibleChars + 1));
      this.selectionStart.X = MathHelper.Clamp(this.selectionStart.X, min, this.content.Count - 1);
      this.selectionEnd.X = MathHelper.Clamp(this.selectionEnd.X, min, this.content.Count - 1);
      this.selectionStart.Y = MathHelper.Clamp(this.selectionStart.Y, 0, this.content[this.selectionStart.X].Length);
      this.selectionEnd.Y = MathHelper.Clamp(this.selectionEnd.Y, 0, this.content[this.selectionEnd.X].Length);
      base.UpdateView(false);
      this.UpdateDeleteButtons();
      this.horizontalScroll.UpdateView(this.GetLongestLine(), this.scrollOffsetX);
      if (ignoreContext || line1.InBlockComment)
        return;
      this.UpdateContextMenu();
    }

    private void UpdateContextMenu(bool forceOpen = false, bool forceDropdown = false)
    {
      try
      {
        bool autoSelectOption = this.contextMenuCanAutoSelectOption;
        if (autoSelectOption)
          this.contextMenuCanAutoSelectOption = false;
        if (this.selectionStart.X != this.selectionEnd.X 
                    || !this.IsInView(this.selectionStart) && !this.IsInView(this.selectionEnd))
        {
          ContextMenu.Instance?.Close();
        }
        else
        {
          Line l = this.content[this.currentLine];
          string line1 = l.ToString();
          int num1 = -1;
          string str1 = (string) null;
          string[] args1 = ScriptParser.GetArgs(line1);
          for (int idx = 0; idx < args1.Length; ++idx)
          {
            string str2 = args1[idx];
            int argPosition = ScriptParser.GetArgPosition(line1, idx);
            if (argPosition != -1 && argPosition == this.SelectionStart.Y && argPosition + str2.Length - 1 == this.SelectionEnd.Y && (str2.Length > 1 | forceOpen || (double) this.tripleClickTime > 0.0))
            {
              num1 = idx;
              str1 = str2;
              break;
            }
          }
          ScriptCommand command;
          bool flag1 = ScriptParser.IsCompleteLine(line1, out ScriptError _, out command, SimulationManager.MainSimulation);
          if (!this.CanInteract() || num1 <= 0 & flag1)
          {
            ContextMenu.Instance?.Close();
          }
          else
          {
            string lower = l.ToString().ToLower();
            if (string.IsNullOrEmpty(lower))
            {
              ContextMenu.Instance?.Close();
            }
            else
            {
              string[] args2 = ScriptParser.GetArgs(lower);
              if (args2.Length == 0)
              {
                ContextMenu.Instance?.Close();
              }
              else
              {
                int argIdx = ScriptParser.GetArgIndex(lower, this.currentChar);
                List<ScriptCommand> commands = new List<ScriptCommand>();
                string str3 = args2[argIdx];
                bool isWhiteSpace = char.IsWhiteSpace(this.GetChar() ?? (this.currentChar == lower.Length ? ' ' : 'a'));
                bool lastIsWhiteSpace = this.currentChar > 0 && char.IsWhiteSpace(lower[this.currentChar - 1]);
                if (isWhiteSpace & lastIsWhiteSpace)
                {
                  str3 = "";
                  argIdx++;
                }
                if (args2.Length >= 2 && num1 >= 1)
                {
                  str3 = "";
                  command = ScriptParser.FindCommand(args2[0], SimulationManager.MainSimulation);
                }
                Rectangle contentRect = this.GetGlobalContentRect();
                int end = isWhiteSpace & lastIsWhiteSpace ? this.currentChar : ScriptParser.GetArgPosition(lower, argIdx);
                string text = l.GetSpan(this.scrollOffsetX, end).Replace(' ', 'a');
                float x = (float) ((double) contentRect.X + /*(double) this.font.MeasureStringHalf(text).Width*/24 - 10.0);
                int menuY = contentRect.Y + (this.currentLine - this.scrollOffsetY) * this.lineHeight + this.lineHeight / 2;
                if (command != null)
                {
                  ScriptCommand scriptCommand = command;
                  int idx1 = -1;
                  for (int idx2 = 0; idx2 < args2.Length && ScriptParser.GetArgPosition(lower, idx2) <= Math.Max(this.SelectionStart.Y, this.currentChar) + 1; ++idx2)
                  {
                    string searchValue = args2[idx2];
                    ScriptCommand parameter = scriptCommand.FindParameter(searchValue);
                    if (parameter != null)
                    {
                      if (!forceDropdown && parameter is ScriptNumber || num1 <= 0 || idx2 < num1)
                      {
                        scriptCommand = parameter;
                        idx1 = idx2;
                      }
                      else
                        break;
                    }
                  }
                  List<ScriptCommand> availableParameters = scriptCommand.GetAvailableParameters(str3, SimulationManager.MainSimulation, autoSelectOption);
                  if (!forceDropdown && idx1 == num1 && scriptCommand is ScriptNumber)
                  {
                    ScriptNumber scriptNumber1 = scriptCommand as ScriptNumber;
                    string s = ScriptParser.GetArg(lower, idx1);
                    if (!string.IsNullOrEmpty(scriptNumber1.Suffix) && s.Contains(scriptNumber1.Suffix))
                      s = s.Replace(scriptNumber1.Suffix, "");
                    ScriptNumber scriptNumber2 = scriptNumber1;
                    Line l1 = l;
                    string[] allWords = args2;
                    int argIdx1 = argIdx;
                    int yPosition = menuY;
                    Rectangle contentRect1 = contentRect;
                    bool flag2 = availableParameters.Count > 1;
                    float result;
                    double currentValue = float.TryParse(s, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? (double) result : 1.0;
                    int num2 = flag2 ? 1 : 0;
                    this.OpenNumberContextMenu(scriptNumber2, l1, allWords, argIdx1, yPosition, contentRect1, (float) currentValue, num2 != 0);
                    return;
                  }
                  commands.AddRange((IEnumerable<ScriptCommand>) availableParameters);
                }
                else if (args2.Length == 1 && argIdx == 0)
                  commands.AddRange((IEnumerable<ScriptCommand>) ScriptParser.GetCommandsStartingWith(str3, SimulationManager.MainSimulation));
                if (commands.Count == 0)
                {
                  ContextMenu.Instance?.Close();
                }
                else
                {
                  List<(string, Button.OnClick)> valueTupleList = new List<(string, Button.OnClick)>();
                  foreach (ScriptCommand scriptCommand in commands)
                  {
                    ScriptCommand cmd = scriptCommand;
                    string str4 = cmd.ContextName;
                    if (!string.IsNullOrEmpty(str3))
                      str4 = "%{red}" + str3 + "%" + str4.Substring(str3.Length);
                    valueTupleList.Add((str4, (Button.OnClick) (() =>
                    {
                      this.SaveCurrentState();
                      string s = cmd.Command;
                      if (cmd is ScriptNumber)
                        s = 1.ToString("0." + new string('0', (cmd as ScriptNumber).Decimals), (IFormatProvider) CultureInfo.InvariantCulture) + (cmd as ScriptNumber).Suffix;
                      else if (cmd is ScriptText)
                        s = "Hello World!";
                      if (isWhiteSpace & lastIsWhiteSpace)
                        l.Add(s, this.currentChar);
                      else
                        l.SetText(ScriptParser.SetArg(l.ToStringRaw(), argIdx, s));
                      string[] args3 = ScriptParser.GetArgs(l.ToString());
                      if (args3.Length - 1 == argIdx && (cmd.Parameters.Length != 0 || cmd is NotCommand))
                      {
                        l.Add(" ");
                        this.currentChar = int.MaxValue;
                      }
                      else
                      {
                        int argPosition = ScriptParser.GetArgPosition(l.ToString(), argIdx);
                        this.currentChar = argPosition >= 0 ? argPosition + args3[argIdx].Length : int.MaxValue;
                      }
                      if (cmd is ScriptText)
                        this.currentChar = int.MaxValue;
                      if (cmd.Parameters.Length == 0)
                      {
                        string line2 = l.ToString();
                        int length = ScriptParser.GetArgPosition(line2, argIdx) + s.Length;
                        if (length < l.Length)
                          l.SetText(line2.Substring(0, length));
                      }
                      this.ClearSelection();
                      this.UpdateView(false);
                      this.ClearSelection();
                      if (!(cmd is ScriptNumber))
                        return;
                      this.OpenNumberContextMenu(cmd as ScriptNumber, l, args3, argIdx, menuY, contentRect, enableOtherButton: commands.Count > 1);
                    })));
                  }
                  if (autoSelectOption && valueTupleList.Count == 1)
                    valueTupleList[0].Item2();
                  else
                    Utility.OpenContextMenu(false, (UIElement) null,
                        new Vector2?(new Vector2(x, (float) menuY)),
                        false, new int?(this.lineHeight / 2), true, 
                        /*this.font*/default, 
                        new Rectangle?(), new int?(10), valueTupleList.ToArray());
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Exception occured when trying to open ContextMenu: " + ex.Message);
        ContextMenu.Instance?.Close();
      }
    }

    private void OpenNumberContextMenu(
      ScriptNumber scriptNumber,
      Line l,
      string[] allWords,
      int argIdx,
      int yPosition,
      Rectangle contentRect,
      float currentValue = 1f,
      bool enableOtherButton = true)
    {
      updateSelection();
      Vector2? position = new Vector2?(new Vector2((float) contentRect.X + (float) (this.currentChar - this.scrollOffsetX + (this.selectionEnd.Y - this.currentChar) / 2) * this.charSize.X, (float) yPosition));
      Decimal currentValue1 = (Decimal) currentValue;
      Utility.ContextValueChangeHandler changeHandler = (Utility.ContextValueChangeHandler) ((v, f) =>
      {
        l.SetText(ScriptParser.SetArg(l.ToStringRaw(), argIdx, v.ToString("0." + new string('0', Math.Max(scriptNumber.Decimals - 1, 0)), (IFormatProvider) CultureInfo.InvariantCulture) + scriptNumber.Suffix));
        updateSelection();
        base.UpdateView(false);
      });
      int num1 = this.currentTextSize == TextSize.Large ? 1 : 0;
      int decimals = scriptNumber.Decimals;
      int num2 = this.lineHeight / 2;
      bool flag = enableOtherButton;
      Rectangle? targetRect = new Rectangle?();
      int positionYMargin = num2;
      System.Action preChangeHandler = (System.Action) (() => this.SaveCurrentState());
      int num3 = flag ? 1 : 0;
      Button.OnClick otherButtonHandler = (Button.OnClick) (() => this.UpdateContextMenu(true, true));
      Utility.OpenNumberContextMenu(false, (UIElement) null, position, currentValue1, changeHandler, true, num1 != 0, decimals, targetRect, true, positionYMargin, preChangeHandler, num3 != 0, otherButtonHandler);

      void updateSelection()
      {
        allWords = ScriptParser.GetArgs(l.ToString());
        int y1 = Math.Max(ScriptParser.GetArgPosition(l.ToString(), argIdx), 0);
        int y2 = Math.Min(y1 + allWords[argIdx].Length - 1, l.Length - 1);
        this.currentChar = this.selectionEnd.Y + (y1 == y2 ? 1 : 0);
        if (this.selectionStart.Y > this.selectionEnd.Y)
        {
          this.selectionEnd = new Point(this.currentLine, y1);
          this.selectionStart = new Point(this.currentLine, y2);
        }
        else
        {
          this.selectionStart = new Point(this.currentLine, y1);
          this.selectionEnd = new Point(this.currentLine, y2);
        }
      }
    }

    private void CreateDeleteButtons()
    {
      if (!this.enableDeleteButtons || this.deleteButtonContainer == null)
        return;
      Styling style = new Styling()
      {
        spritePos = TextureManager.GetSpritePos("ui_roundcross"),
        defaultColor = new Color(50, 50, 50, 110),
        hoverColor = new Color(50, 50, 50, 190),
        clickColor = ColorManager.GetColor("red")
      };
      this.deleteButtonContainer.ClearChildren();
      this.deleteButtons = new Button[this.maxVisibleLines];
      for (int index = 0; index < this.maxVisibleLines; ++index)
      {
        int y = index * this.lineHeight + this.lineHeight / 2 - this.deleteButtonSize / 2 - 1;
        style.value = index.ToString();
        this.deleteButtons[index] = new Button(new Rectangle(this.deleteButtonX, y, this.deleteButtonSize, this.deleteButtonSize), new Button.OnClickString(this.OnDeleteButtonPress), style);
        this.deleteButtonContainer.AddChild((UIElement) this.deleteButtons[index]);
      }
      this.UpdateDeleteButtons();
    }

    private void UpdateDeleteButtons()
    {
      if (!this.enableDeleteButtons || this.deleteButtons == null)
        return;
      bool flag = this.CanInteract();
      for (int index = 0; index < this.deleteButtons.Length; ++index)
      {
        Button deleteButton = this.deleteButtons[index];
        if (deleteButton != null)
        {
          bool active = flag && index + this.scrollOffsetY >= this.firstEditableLine && index + this.scrollOffsetY < this.content.Count && (this.content[index].Length != 0 || this.content.Count != 1);
          deleteButton.SetActive(active);
        }
      }
    }

    private void ShowCursor()
    {
      if (SimulationManager.MainSimulation.IsScriptRunning)
        return;
      int scrollOffsetY = this.scrollOffsetY;
      if (this.currentLine > this.scrollOffsetY + this.maxVisibleLines - 1)
        this.scrollOffsetY = this.currentLine - this.maxVisibleLines + 1;
      if (this.currentLine < this.scrollOffsetY)
        this.scrollOffsetY = this.currentLine;
      if (this.scrollOffsetY != scrollOffsetY)
        this.UpdateView(false);
    }

    private void OnMouseDown()
    {
      Utility.GetMousePos();
      if (!this.CanInteract())
        return;
      Rectangle globalRect = this.GlobalRect;
      if (Utility.MouseInside(this.GetGlobalContentRect()))
      {
        int lineFromPos = this.GetLineFromPos(new Vector2?(), true, false, -1);
        if (lineFromPos < this.firstEditableLine)
          return;
        this.currentLine = lineFromPos;
        this.currentChar = this.GetCharFromMouse();
        this.ClearSelection();
        if ((double) this.tripleClickTime > 0.0)
        {
          this.selectionStart.Y = 0;
          this.selectionEnd.Y = this.content[this.currentLine].Length;
          this.currentChar = this.selectionEnd.Y;
        }
        else if ((double) this.doubleClickTime > 0.0)
        {
          string str = this.content[this.currentLine].ToString();
          int num = str.Substring(this.currentChar).IndexOf(' ');
          this.selectionStart.Y = str.Substring(0, this.currentChar).LastIndexOf(' ') + 1;
          this.selectionEnd.Y = this.currentChar >= str.Length - 1 || num == -1 ? str.Length - 1 : this.currentChar + num - 1;
          this.tripleClickTime = 0.2f;
          this.currentChar = this.selectionEnd.Y;
          this.UpdateContextMenu(true);
        }
        else
        {
          this.isSelectionDragging = true;
          this.doubleClickTime = 0.2f;
        }
        this.PlayInteractionSound(0.25f);
        this.UpdateView(false);
        this.timeSinceAction = 0.0f;
      }
    }

    private void OnMouseUp()
    {
      if (!this.isSelectionDragging)
        return;
      this.isSelectionDragging = false;
      this.currentLine = this.GetLineFromPos(new Vector2?(), true, false, -1);
      this.currentChar = this.GetCharFromMouse(this.currentLine);
      if (this.HasSelection)
        this.PlayInteractionSound(0.25f);
      this.UpdateView(false);
      this.timeSinceAction = 0.0f;
    }

    private void UpdateCharSize()
    {
            this.charSize = (Vector2)new Vector2();//this.font.MeasureStringHalf("a");
      int maxVisibleChars = this.maxVisibleChars;
      this.maxVisibleChars = (int) ((double) this.AvailableContentWidth / (double) this.charSize.X) - 3;
      int num = this.maxVisibleChars - maxVisibleChars;
      if (num > 0)
        this.scrollOffsetX -= num;
      this.cursorPointTexture = TextureManager.GetCircleTexture((int) ((double) this.cursorPointSize * 0.800000011920929));
      this.horizontalScroll?.SetDisplaySize(this.maxVisibleChars, true);
      this.UpdateView(false);
    }

    public override Rectangle GetGlobalContentRect()
    {
      Rectangle globalRect = this.GlobalRect;
      return new Rectangle(globalRect.X + this.textX, globalRect.Y + 20, this.AvailableContentWidth, this.AvailableContentHeight);
    }

    public void SetLineDrawModifier(TextArea.LineDrawModifier lineDrawModifier)
    {
      this.lineDrawModifier = lineDrawModifier;
    }

    private char? GetChar(int? pos = null)
    {
      int? nullable = pos;
      int pos1 = nullable ?? this.currentChar;
      int num;
      if (this.content.Count != 0)
      {
        int length = this.content[this.currentLine].Length;
        nullable = pos;
        int valueOrDefault = nullable.GetValueOrDefault();
        num = length < valueOrDefault & nullable.HasValue ? 1 : 0;
      }
      else
        num = 1;
      if (num != 0)
        return new char?();
      string at = this.content[this.currentLine].GetAt(pos1);
      return string.IsNullOrEmpty(at) ? new char?() : new char?(at[0]);
    }

        private bool CharIsNumber()
        {
            char? character = this.GetChar(new int?(this.currentChar - 1));
            return character.HasValue && char.IsNumber(character.Value);
        }

    private int GetLongestLine()
    {
      int longestLine = 0;
      foreach (Line line in this.content)
      {
        if (line.Length > longestLine)
          longestLine = line.Length;
      }
      return longestLine;
    }

    private int GetCharFromMouse(int lineId = -1)
    {
      lineId = lineId == -1 ? this.currentLine : lineId;
      return MathHelper.Clamp(((int) Utility.GetMousePos().X - this.GlobalRect.X - this.textX) / (int) this.charSize.X + this.scrollOffsetX, 0, this.content[lineId].Length);
    }

    private bool IsInView(Point p)
    {
      return p.X < this.scrollOffsetY + this.maxVisibleLines && p.X >= this.scrollOffsetY && p.Y <= this.scrollOffsetX + this.maxVisibleChars && p.Y >= this.scrollOffsetX;
    }

    private bool IsInView(int x, int y) => this.IsInView(new Point(x, y));

    private void PreviousLine(bool canChangeScroll = false)
    {
      if (!this.CanInteract())
        return;
      this.timeSinceAction = 0.0f;
      bool flag1 = Input.IsDown(KeyBind.Shift);
      bool flag2 = Input.IsDown(KeyBind.Alt);
      bool flag3 = Input.IsDown(KeyBind.Control);
      if (flag3 && this.CharIsNumber())
        this.IncreaseNumber();
      else if (this.currentLine > 0)
      {
        if (flag3)
        {
          this.currentLine = Math.Max(0, this.firstEditableLine);
        }
        else
        {
          if (flag2 && this.currentLine > this.firstEditableLine)
          {
            this.SaveCurrentState();
            Line line = this.content[this.currentLine];
            this.content[this.currentLine] = this.content[this.currentLine - 1];
            this.content[this.currentLine - 1] = line;
          }
          --this.currentLine;
        }
        if (canChangeScroll && this.currentLine < this.scrollOffsetY)
          this.scrollOffsetY = this.currentLine;
      }
      this.UpdateView(false);
      if (flag1 && !(flag3 | flag2))
      {
        this.selectionEnd.X = this.currentLine;
        this.selectionEnd.Y = this.currentChar;
      }
      else
        this.ClearSelection();
      this.PlayInteractionSound(0.25f);
    }

    private void NextLine(
      bool forceNext = false,
      bool ignoreFocus = false,
      bool isPasting = false,
      bool isCreating = false,
      bool isManual = false,
      bool canChangeScroll = false)
    {
      if (!(this.CanInteract() | ignoreFocus))
        return;
      this.timeSinceAction = 0.0f;
      bool flag1 = Input.IsDown(KeyBind.Shift);
      bool flag2 = Input.IsDown(KeyBind.Alt);
      if (!forceNext && this.CharIsNumber() && Input.IsDown(KeyBind.Control))
        this.DecreaseNumber();
      else if (this.currentLine < this.content.Count - 1)
      {
        if (!isPasting && !isCreating && Input.IsDown(KeyBind.Control))
        {
          this.currentLine = this.content.Count - 1;
        }
        else
        {
          if (((isPasting ? 0 : (!isManual ? 1 : 0)) & (flag2 ? 1 : 0)) != 0)
          {
            Line line = this.content[this.currentLine];
            this.content[this.currentLine] = this.content[this.currentLine + 1];
            this.content[this.currentLine + 1] = line;
          }
          ++this.currentLine;
        }
        if (canChangeScroll && this.currentLine > this.scrollOffsetY + (this.maxVisibleLines - 1))
          this.scrollOffsetY = this.currentLine - (this.maxVisibleLines - 1);
      }
      this.UpdateView(false);
      if (flag1 && !flag2)
      {
        this.selectionEnd.X = this.currentLine;
        this.selectionEnd.Y = this.currentChar;
      }
      else
        this.ClearSelection();
      if (!forceNext && !isPasting)
        this.PlayInteractionSound(0.25f);
    }

    private void PreviousChar(bool noSkip = false, bool canChangeScroll = false)
    {
      if (!this.CanInteract())
        return;
      this.timeSinceAction = 0.0f;
      bool flag = Input.IsDown(KeyBind.Shift);
      if (flag && !this.HasSelection)
        this.ClearSelection();
      if (!noSkip && Input.IsDown(KeyBind.Control))
        this.FirstChar(false);
      else
        --this.currentChar;
      if (this.currentChar < this.content[this.currentLine].BlockCharAmount)
      {
        if (this.currentLine > this.firstEditableLine)
        {
          --this.currentLine;
          this.currentChar = this.content[this.currentLine].Length;
          if (canChangeScroll && this.currentLine < this.scrollOffsetY)
            this.scrollOffsetY = this.currentLine;
        }
        else
          this.currentChar = 0;
      }
      if (flag)
      {
        this.selectionEnd.X = this.currentLine;
        this.selectionEnd.Y = this.currentChar;
      }
      else
        this.ClearSelection();
      this.UpdateView(false);
      this.PlayInteractionSound(0.25f);
    }

    private void NextChar(bool noSkip = false, bool canChangeScroll = false)
    {
      if (!this.CanInteract())
        return;
      this.timeSinceAction = 0.0f;
      bool flag = Input.IsDown(KeyBind.Shift);
      if (flag && !this.HasSelection)
        this.ClearSelection();
      if (!noSkip && Input.IsDown(KeyBind.Control))
        this.LastChar(false);
      else
        ++this.currentChar;
      int length = this.content[this.currentLine].Length;
      if (this.currentChar > length)
      {
        if (this.currentLine < this.content.Count - 1)
        {
          ++this.currentLine;
          this.currentChar = 0;
          if (canChangeScroll && this.currentLine > this.scrollOffsetY + (this.maxVisibleLines - 1))
            this.scrollOffsetY = this.currentLine - (this.maxVisibleLines - 1);
        }
        else
          this.currentChar = length;
      }
      if (flag)
      {
        this.selectionEnd.X = this.currentLine;
        this.selectionEnd.Y = this.currentChar;
      }
      else
        this.ClearSelection();
      this.UpdateView(false);
      this.PlayInteractionSound(0.25f);
    }

    private void FirstChar(bool playSound = true)
    {
      if (!this.CanInteract())
        return;
      this.timeSinceAction = 0.0f;
      this.currentChar = 0;
      if (playSound)
        this.PlayInteractionSound(0.25f);
      this.UpdateView(false);
    }

    private void LastChar(bool playSound = true)
    {
      if (!this.CanInteract())
        return;
      this.timeSinceAction = 0.0f;
      this.currentChar = this.content[this.currentLine].Length;
      if (playSound)
        this.PlayInteractionSound(0.25f);
      this.UpdateView(false);
    }

    private void OnHorizontalMouseScroll(float scrollDiff)
    {
      if (!this.CanInteract(true) || !this.MouseOnRectangle())
        return;
      int scrollOffsetX = this.scrollOffsetX;
      int longestLine = this.GetLongestLine();
      if ((double) scrollDiff > 0.0)
      {
        if (this.scrollOffsetX < longestLine - this.maxVisibleChars)
          ++this.scrollOffsetX;
      }
      else if ((double) scrollDiff < 0.0 && this.scrollOffsetX > 0)
        --this.scrollOffsetX;
      this.UpdateView(false);
      if (scrollOffsetX == this.scrollOffsetX)
        return;
      this.PlayInteractionSound(0.025f);
    }

    private void HandleTextInput(char c)
    {
      if (!this.CanInteract())
        return;
      switch (c)
      {
        case '"':
          c = '\'';
          break;
        case ',':
          c = '.';
          break;
      }
      this.ShowCursor();
      if (StateManager.IsState(GameState.InLevel_AnyWithText) && (Utility.MainPlayer == null || (double) Utility.MainPlayer.LifeTime < 0.10000000149011612))
        return;
      this.AddToCurrentLine(c.ToString());
      this.PlayInteractionSound(0.25f);
      if (this.content[this.currentLine].Length < 3)
        this.UpdateDeleteButtons();
      this.UpdateView(false);
    }

    private void RemoveChar(bool saveState = true, bool front = false)
    {
      if (!this.CanInteract())
        return;
      if (saveState)
        this.SaveCurrentState();
      if (this.HasSelection)
      {
        Point selectionStart = this.SelectionStart;
        Point selecEnd = this.SelectionEnd;
        List<Line> remove = new List<Line>();
        bool moveNextLine = true;
        this.ForEachSelection((TextArea.SelectionAction) ((line, start, end) =>
        {
          if (line >= this.content.Count)
            return;
          Line line1 = this.content[line];
          start = MathHelper.Clamp(start, 0, line1.Length);
          end = MathHelper.Clamp(end, 0, line1.Length);
          line1.Remove(start, end + 1);
          if (line1.RawLength == 0 && line > 0 && line != this.SelectionStart.X)
          {
            remove.Add(line1);
            if (line == selecEnd.X)
              moveNextLine = false;
          }
        }));
        foreach (Line line in remove)
          this.content.Remove(line);
        this.currentLine = this.SelectionStart.X;
        this.currentChar = this.SelectionStart.Y;
        if (moveNextLine && selectionStart.X != selecEnd.X && this.content.Count > this.currentLine + 1)
        {
          this.content[this.currentLine].Add(this.content[this.currentLine + 1].ToStringRaw());
          this.content.RemoveAt(this.currentLine + 1);
        }
        this.isSelectionDragging = false;
        this.ClearSelection();
      }
      else
      {
        bool flag = this.currentLine > this.firstEditableLine;
        if (flag && Input.IsDown(KeyBind.Shift))
        {
          this.RemoveLine(this.currentLine);
        }
        else
        {
          if (front)
            this.NextChar(true);
          Line line2 = this.content[this.currentLine];
          if (line2.RawLength > 0)
          {
            if (this.currentChar > line2.BlockCharAmount)
            {
              line2.Remove(this.currentChar - 1);
              --this.currentChar;
              if (this.scrollOffsetX > 0)
                --this.scrollOffsetX;
            }
            else if (flag)
            {
              Line line3 = this.content[this.currentLine - 1];
              this.currentChar = line3.Length;
              line3.SetText(line3.ToStringRaw() + line2.ToStringRaw());
              this.content.RemoveAt(this.currentLine);
              --this.currentLine;
              this.UpdateView(false);
            }
          }
          else if (flag && this.content.Count > 1)
          {
            int num = this.currentLine - 1;
            this.RemoveLine(this.currentLine);
            this.currentLine = num;
            this.currentChar = int.MaxValue;
            this.UpdateView(false);
          }
        }
      }
      this.PlayInteractionSound(0.25f);
      this.UpdateView(false);
    }

    private Line AddToCurrentLine(string s)
    {
      this.SaveCurrentState();
      if (this.HasSelection)
        this.RemoveChar(false);
      Line currentLine = this.content[this.currentLine];
      currentLine.Add(s, this.currentChar);
      this.currentChar += s.Length;
      if (this.currentChar + this.scrollOffsetX > this.maxVisibleChars)
        this.scrollOffsetX += s.Length;
      return currentLine;
    }

    private void SetChar(string s, int? pos = null)
    {
      int? nullable = pos;
      int pos1 = nullable ?? this.currentChar;
      int num;
      if (this.content.Count != 0)
      {
        int length = this.content[this.currentLine].Length;
        nullable = pos;
        int valueOrDefault = nullable.GetValueOrDefault();
        num = length < valueOrDefault & nullable.HasValue ? 1 : 0;
      }
      else
        num = 1;
      if (num != 0)
        return;
      this.content[this.currentLine].SetAt(pos1, s);
    }

    private void IncreaseNumber()
    {
      this.SaveCurrentState();
      int num1 = this.currentChar - 1;
      int num2 = int.Parse(this.GetChar(new int?(num1)).ToString()) + 1;
      if (num2 == 10)
        num2 = 0;
      this.SetChar(num2.ToString(), new int?(num1));
      this.PlayInteractionSound(0.25f);
    }

    private void DecreaseNumber()
    {
      this.SaveCurrentState();
      int num1 = this.currentChar - 1;
      int num2 = int.Parse(this.GetChar(new int?(num1)).ToString()) - 1;
      if (num2 == -1)
        num2 = 9;
      this.SetChar(num2.ToString(), new int?(num1));
      this.PlayInteractionSound(0.25f);
    }

    private Line NewLine(bool ignoreFocus = false, bool isPasting = false, bool canStart = true, bool isManual = false)
    {
      Line line1 = (Line) null;
      if (this.CanInteract() | ignoreFocus)
      {
        if (!isPasting)
          this.SaveCurrentState();
        if (this.currentChar == 0 && (Enumerable.Count<Line>((IEnumerable<Line>) this.content) == 0 || this.content[this.currentLine].Length > 0))
        {
          this.content.Insert(this.currentLine, line1 = new Line());
        }
        else
        {
          this.currentLine = Math.Min(this.currentLine, this.content.Count - 1);
          Line line2 = this.content[this.currentLine];
          if (this.currentChar < line2.Length)
          {
            this.content.Insert(this.currentLine + 1, line1 = new Line(line2.Substring(this.currentChar)));
            line2.SetText(line2.Substring(0, this.currentChar));
          }
          else
            this.content.Insert(this.currentLine + 1, line1 = new Line());
        }
        this.currentChar = 0;
        this.NextLine(true, true, isPasting, true, isManual, true);
        this.UpdateDeleteButtons();
        if (!ignoreFocus && !isPasting)
          this.PlayInteractionSound(0.25f);
        this.UpdateView(false);
      }
      return line1;
    }

    private void RemoveLine(int lineIndex)
    {
      if (lineIndex < this.content.Count)
      {
        if (this.content.Count > 1)
          this.content.RemoveAt(lineIndex);
        else
          this.content[0].Clear();
      }
      if (this.scrollOffsetY > 0)
        --this.scrollOffsetY;
      this.currentLine = lineIndex;
      this.currentChar = int.MaxValue;
      this.UpdateView(false);
    }

    private void OnDeleteButtonPress(string s)
    {
      if (!this.CanInteract())
        return;
      this.SaveCurrentState();
      this.RemoveLine(int.Parse(s) + this.scrollOffsetY);
    }

    private void AutoCompleteLine()
    {
      if (!this.CanInteract())
        return;
      ContextMenu.Instance?.SelectCurrent();
    }

    private void ForEachSelection(TextArea.SelectionAction action)
    {
      Point selectionStart = this.SelectionStart;
      Point selectionEnd = this.SelectionEnd;
      if (selectionStart.X == selectionEnd.X)
      {
        doAction(selectionStart.X, selectionStart.Y, selectionEnd.Y);
      }
      else
      {
        doAction(selectionStart.X, selectionStart.Y, 1073741823);
        for (int lineIdx = selectionStart.X + 1; lineIdx <= selectionEnd.X; ++lineIdx)
        {
          if (lineIdx == selectionEnd.X)
            doAction(lineIdx, 0, selectionEnd.Y);
          else
            doAction(lineIdx, 0, 1073741823);
        }
      }

      void doAction(int lineIdx, int start, int end)
      {
        Line line;
        if (!this.TryGetLine(lineIdx, out line))
          return;
        action(lineIdx, Math.Max(start, line.BlockCharAmount), end);
      }
    }

    private void ClearSelection()
    {
      this.selectionStart = new Point(this.currentLine, this.currentChar);
      this.selectionEnd = this.selectionStart;
    }

    private void CopySelection()
    {
      if (!this.CanInteract())
        return;
      if (this.HasSelection)
      {
        string text = "";
        this.ForEachSelection((TextArea.SelectionAction) ((line, start, end) =>
        {
          string str = this.content[line].ToString();
          text += str.Substring(start, Math.Min(end + 1, str.Length) - start);
          if (line >= this.SelectionEnd.X)
            return;
          text += "\n";
        }));
        Utility.clipboard = text;
      }
      else
      {
        char? nullable = this.GetChar();
        if (nullable.HasValue && !string.IsNullOrEmpty(nullable.ToString()))
          Utility.clipboard = nullable.ToString();
      }
      this.PlayInteractionSound(0.25f);
    }

    private void CutSelection()
    {
      if (!this.CanInteract())
        return;
      if (this.HasSelection)
      {
        this.CopySelection();
        this.RemoveChar();
      }
      else
      {
        char? nullable = this.GetChar();
        if (nullable.HasValue && !string.IsNullOrEmpty(nullable.ToString()))
        {
          Utility.clipboard = nullable.ToString();
          this.RemoveChar(front: true);
        }
      }
      this.PlayInteractionSound(0.25f);
    }

    private void Paste()
    {
      if (string.IsNullOrEmpty(Utility.clipboard) || !this.CanInteract())
        return;
      this.SaveCurrentState();
      if (this.HasSelection)
        this.RemoveChar(false);
      string[] strArray = Utility.clipboard.Split('\n');
      int length = strArray.Length;
      if (length == 1)
      {
        this.content[this.currentLine].Insert(this.currentChar, Utility.clipboard);
        this.currentChar += Utility.clipboard.Length;
      }
      else if (length > 1)
      {
        Line line = this.content[this.currentLine];
        string str = line.Substring(this.currentChar);
        line.SetText(line.Substring(0, this.currentChar) + strArray[0]);
        this.currentChar = line.Length;
        for (int index = 1; index < length; ++index)
        {
          this.NewLine(isPasting: true, canStart: false);
          this.content[this.currentLine].SetText(strArray[index] + (index == length - 1 ? str : ""));
          this.currentChar = this.content[this.currentLine].Length;
        }
        this.currentChar = strArray[length - 1].Length;
      }
      this.PlayInteractionSound(0.25f);
      this.UpdateView(false);
    }

    private void SelectAll()
    {
      if (!this.CanInteract())
        return;
      this.selectionStart = Point.Zero;
      int num = this.content.Count - 1;
      this.selectionEnd = new Point(num, this.content[num].Length);
      this.PlayInteractionSound(0.25f);
    }

    private TextAreaState GetCurrentState()
    {
      List<Line> lines = new List<Line>();
      foreach (Line line in this.content)
        lines.Add(new Line(line.ToStringRaw()));
      return new TextAreaState(lines, this.currentLine, this.currentChar);
    }

    private void SaveCurrentState(bool redo = false, bool clearRedo = true)
    {
      if (redo)
      {
        this.redoStates.Push(this.GetCurrentState());
      }
      else
      {
        this.undoStates.Push(this.GetCurrentState());
        if (clearRedo)
          this.redoStates.Clear();
      }
      this.timeSinceAction = 0.0f;
    }

    private void Undo()
    {
      if (!this.CanInteract() || this.undoStates.Count <= 0)
        return;
      this.SaveCurrentState(true);
      TextAreaState textAreaState = this.undoStates.Pop();
      this.content = textAreaState.lines;
      this.currentLine = textAreaState.currentLine;
      this.currentChar = textAreaState.currentChar;
      this.PlayInteractionSound(0.25f);
      this.UpdateView(false);
    }

    private void Redo()
    {
      if (!this.CanInteract() || this.redoStates.Count <= 0)
        return;
      this.SaveCurrentState(clearRedo: false);
      TextAreaState textAreaState = this.redoStates.Pop();
      this.content = textAreaState.lines;
      this.currentLine = textAreaState.currentLine;
      this.currentChar = textAreaState.currentChar;
      this.PlayInteractionSound(0.25f);
      this.UpdateView(false);
    }

    private delegate void SelectionAction(int line, int start, int end);

    public delegate void LineDrawModifier(
      SpriteBatch spriteBatch,
      int line,
      Rectangle lineRect,
      int numberX,
      out bool whiteNumberText);
  }
}

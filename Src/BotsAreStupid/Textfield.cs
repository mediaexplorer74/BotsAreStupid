// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Textfield
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.BitmapFonts;

#nullable disable
namespace BotsAreStupid
{
  internal class Textfield : UIElement
  {
    private const int defaultTextOffset = 20;
    private Line line;
    private Vector2 charSize;
    private Vector2 cursorSize;
    private Color cursorColor;
    private int currentChar = 0;
    private int maxChars;
    private float cooldown = 0.0f;
    private float cooldownMax = 0.1f;
    private int scrollOffset = 0;

    public event Textfield.ValueChangeHandler OnValueChange;

    public Textfield(
      Rectangle rectangle,
      GameState gameState,
      Styling? styling = null,
      int cursorWidth = -1,
      Color? cursorColor = null,
      string inlineLabel = "")
      : base(rectangle, new Styling?(Styling.AddTo(new Styling()
      {
        defaultColor = ColorManager.GetColor("white"),
        defaultTextColor = Color.Black,
        textOffset = 20
      }, new Styling?(styling ?? Styling.Null))))
    {
      Textfield textfield = this;
      this.line = new Line();
      Input.RegisterTextHandler(new Input.TextInputHandler(this.HandleTextInput), "[def]!.,-_@", gameState);
      Input.Register(KeyBind.DestroyObject, new InputAction.InputHandler(this.RemoveChar), gameState, 0.1f, ignoreTextLock: true);
      Input.Register(KeyBind.Left, new InputAction.InputHandler(this.PreviousChar), gameState, 0.08f, ignoreTextLock: true);
      Input.Register(KeyBind.Right, new InputAction.InputHandler(this.NextChar), gameState, 0.08f, ignoreTextLock: true);
      ref Styling? local1 = ref styling;
    //BitmapFont font = (local1.HasValue ? local1.GetValueOrDefault().font : (BitmapFont) null) ?? TextureManager.GetFont("courier");
    this.charSize = (Vector2)new Vector2(24,24);//font.MeasureStringHalf("a");
      bool flag = false;
      ref Styling? local2 = ref styling;
      int textOffset = local2.HasValue ? local2.GetValueOrDefault().textOffset : 20;
      if (!string.IsNullOrEmpty(inlineLabel))
      {
        this.AddChild(new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, parentRect.Height))));
        flag = true;
      }
          
      if (flag)
                textOffset = 2 * textOffset + 24;//(int) font.MeasureStringHalf(inlineLabel).Width;
      
      int num1 = rectangle.Width - textOffset;
      ref Styling? local3 = ref styling;
      int num2 = local3.HasValue ? local3.GetValueOrDefault().textOffsetRight : 20;
      this.maxChars = (num1 - num2) / (int) this.charSize.X;
      this.AddResponsiveAction((UIElement.ResponsiveActionHandler) (parentRect =>
      {
          Textfield textfield1 = default;//closure_0;
        int num3 = /*closure_0.GlobalRect.Width*/24 - textOffset;
        ref Styling? local4 = ref styling;
        int num4 = local4.HasValue ? local4.GetValueOrDefault().textOffsetRight : 20;
       int num5 = (num3 - num4) / 12;//(int) closure_0.charSize.X;
        textfield1.maxChars = num5;
      }));
      this.style.textOffset = textOffset;
      this.cursorColor = cursorColor ?? ColorManager.GetColor("cursor");
      this.cursorSize = new Vector2(cursorWidth > 0 ? (float) cursorWidth : this.charSize.X, this.charSize.Y);
    }

    public override void SetActive(bool active)
    {
      base.SetActive(active);
      if (!active)
        return;
      this.cooldown = this.cooldownMax;
      this.scrollOffset = 0;
    }

    public override void Update(float deltaTime)
    {
      if ((double) this.cooldown <= 0.0)
        return;
      this.cooldown -= deltaTime;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if (!this.IsActive)
        return;
      base.Draw(spriteBatch);
      Rectangle rectangle = new Rectangle((int) ((double) (this.GlobalRect.X + this.style.textOffset) + (double) (this.currentChar - this.scrollOffset) * (double) this.charSize.X), (int) ((double) (this.GlobalRect.Y + this.rectangle.Height / 2) - (double) this.cursorSize.Y / 2.0), (int) this.cursorSize.X, (int) this.cursorSize.Y);
      Utility.DrawRect(spriteBatch, rectangle, this.cursorColor);
    }

    public string GetValue() => this.line.ToString();

    public void Reset()
    {
      this.line.Clear();
      this.currentChar = 0;
      this.style.text = this.line.ToString();
    }

    public void SetValue(string value)
    {
      this.line = new Line(value);
      this.currentChar = value.Length;
      this.UpdateText();
    }

    private void HandleTextInput(char c)
    {
      if ((double) this.cooldown > 0.0 || !this.IsActive)
        return;
      this.AddChar(c);
      this.UpdateText();
      this.PlayInteractionSound();
      Textfield.ValueChangeHandler onValueChange = this.OnValueChange;
      if (onValueChange != null)
        onValueChange(this.GetValue());
    }

    private void UpdateText()
    {
      this.style.text = this.line.Length > this.maxChars ? this.line.ToString().Substring(this.scrollOffset, this.maxChars) : this.line.ToString();
    }

    private void AddChar(char c)
    {
      this.line.Add(c.ToString(), this.currentChar);
      ++this.currentChar;
      if (this.currentChar + this.scrollOffset <= this.maxChars)
        return;
      ++this.scrollOffset;
    }

    private void PreviousChar()
    {
      if (!this.IsActive || this.currentChar <= 0)
        return;
      if (Input.IsDown(KeyBind.Control))
        this.currentChar = 0;
      else
        --this.currentChar;
      if (this.currentChar < this.scrollOffset)
      {
        this.scrollOffset = this.currentChar;
        this.UpdateText();
      }
      this.PlayInteractionSound();
    }

    private void NextChar()
    {
      if (!this.IsActive || this.currentChar >= this.line.Length)
        return;
      if (Input.IsDown(KeyBind.Control))
        this.currentChar = this.line.Length;
      else
        ++this.currentChar;
      if (this.currentChar > this.maxChars + this.scrollOffset - 1 && this.currentChar != this.line.Length)
      {
        ++this.scrollOffset;
        this.UpdateText();
      }
      this.PlayInteractionSound();
    }

    private void RemoveChar()
    {
      if (!this.IsActive || !this.line.Remove(this.currentChar - 1))
        return;
      if (this.currentChar > 0)
        --this.currentChar;
      if (this.scrollOffset > 0)
        --this.scrollOffset;
      this.UpdateText();
      this.PlayInteractionSound();
      Textfield.ValueChangeHandler onValueChange = this.OnValueChange;
      if (onValueChange != null)
        onValueChange(this.GetValue());
    }

    public delegate void ValueChangeHandler(string value);
  }
}

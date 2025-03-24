// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ArrowSelection
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;

#nullable disable
namespace BotsAreStupid
{
  internal class ArrowSelection : UIElement
  {
    private ArrowSelection.DropdownChangeHandler changeHandler;
    private ArrowSelection.ValueDisplayModifier valueDisplayModifier;
    private const int buttonWidth = 50;
    private string label;
    private int currentValueIdx;
    private string[] values;
    private Button leftButton;
    private Button rightButton;
    private bool smallArrows;

    public ArrowSelection(
      Rectangle rectangle,
      string label,
      ArrowSelection.DropdownChangeHandler changeHandler,
      Styling style,
      string currentValue,
      bool smallArrows,
      ArrowSelection.ValueDisplayModifier valueDisplayModifier,
      params string[] values)
      : base(rectangle, new Styling?(style))
    {
      this.changeHandler = changeHandler;
      this.values = values;
      this.currentValueIdx = this.GetCurrentValueIndex(currentValue);
      this.label = label;
      this.smallArrows = smallArrows;
      this.valueDisplayModifier = valueDisplayModifier;
      this.CreateChildren();
      this.UpdateView();
    }

    public override void SetActive(bool active)
    {
      base.SetActive(active);
      if (!active)
        return;
      this.UpdateView();
    }

    private void CreateChildren()
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = ColorManager.GetColor("white");
      Color color3 = ColorManager.GetColor("darkslate");
      Color color4 = ColorManager.GetColor("lightslate");
      //BitmapFont font = TextureManager.GetFont(this.smallArrows ? "megaMan2Small" : "megaMan2");
      int scaledButtonWidth = this.smallArrows ? 25 : 50;
      Styling styling = new Styling();
      styling.defaultColor = color3;
      styling.hoverColor = color4;
      styling.clickColor = color1;
      styling.borderColor = this.style.borderColor;
      styling.borderWidth = this.style.borderWidth;
      //styling.font = font;
      styling.defaultTextColor = color2;
      styling.centerText = true;
      styling.hoverCursor = new bool?(true);
      Styling style1 = styling;
      this.AddChild((UIElement) (this.leftButton = new Button(Rectangle.Empty, (Button.OnClick) (() => this.ChangeCurrent(false)), style1)));
      this.leftButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, scaledButtonWidth, parentRect.Height)));
      Button leftButton = this.leftButton;
      Rectangle empty1 = Rectangle.Empty;
      styling = new Styling();
      styling.spritePos = TextureManager.GetSpritePos("ui_left");
      styling.defaultColor = color2;
      Styling? style2 = new Styling?(styling);
      UIElement child1 = new UIElement(empty1, style2).SetResponsiveRectangle(new UIElement.ResponsiveRectangleHandler(buttonIconRect));
      leftButton.AddChild(child1);
      this.AddChild((UIElement) (this.rightButton = new Button(Rectangle.Empty, (Button.OnClick) (() => this.ChangeCurrent()), style1)));
      this.rightButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width - scaledButtonWidth, 0, scaledButtonWidth, parentRect.Height)));
      Button rightButton = this.rightButton;
      Rectangle empty2 = Rectangle.Empty;
      styling = new Styling();
      styling.spritePos = TextureManager.GetSpritePos("ui_right");
      styling.defaultColor = color2;
      Styling? style3 = new Styling?(styling);
      UIElement child2 = new UIElement(empty2, style3).SetResponsiveRectangle(new UIElement.ResponsiveRectangleHandler(buttonIconRect));
      rightButton.AddChild(child2);

      static Rectangle buttonIconRect(Rectangle parentRect)
      {
        return Utility.ScaledCenteredRect(parentRect, 0.75f, true);
      }
    }

    private void ChangeCurrent(bool increase = true)
    {
      this.currentValueIdx += increase ? 1 : -1;
      this.changeHandler(this.values[this.currentValueIdx]);
      this.UpdateView();
    }

    private void UpdateView()
    {
      string v = this.values[this.currentValueIdx];
      if (this.valueDisplayModifier != null)
        v = this.valueDisplayModifier(v);
      this.style.text = this.label + " " + v;
      this.leftButton.SetActive(this.currentValueIdx > 0);
      this.rightButton.SetActive(this.currentValueIdx < this.values.Length - 1);
    }

    private int GetCurrentValueIndex(string s)
    {
      for (int currentValueIndex = 0; currentValueIndex < this.values.Length; ++currentValueIndex)
      {
        if (this.values[currentValueIndex] == s)
          return currentValueIndex;
      }
      return -1;
    }

    public delegate void DropdownChangeHandler(string s);

    public delegate string ValueDisplayModifier(string v);
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.BasicMenu
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal abstract class BasicMenu : UIElement
  {
    protected const int borderWidth = 4;
    protected UIElement header;
    protected UIElement main;
    protected Styling buttonStyling;

    protected virtual int headerHeight => 100;

    public BasicMenu(
      Rectangle rectangle,
      GameState drawInState,
      Styling? buttonStyling = null,
      Button.OnClick backButtonHandler = null)
      : base(rectangle)
    {
      this.buttonStyling = buttonStyling ?? new Styling();
      this.CreateChildren(backButtonHandler);
    }

    private void CreateChildren(Button.OnClick backButtonHandler)
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = ColorManager.GetColor("white");
      Color color3 = ColorManager.GetColor("lightslate");
      Color color4 = ColorManager.GetColor("darkslate");
      this.buttonStyling = Styling.AddTo(new Styling()
      {
        borderWidth = 4,
        borderColor = color2,
        defaultColor = color4,
        hoverColor = color3,
        clickColor = color1,
        centerText = true,
        defaultTextColor = color2,
        //font = TextureManager.GetFont("megaMan2Big"),
        enabledBorders = new BorderInfo(right: true, bottom: true)
      }, new Styling?(this.buttonStyling));
      this.header = new UIElement(Rectangle.Empty);
      this.header.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, this.headerHeight)));
      this.buttonStyling.text = "Back";
      Button child = new Button(Rectangle.Empty, backButtonHandler == null ? (Button.OnClick) (() => StateManager.TransitionBack()) : backButtonHandler, this.buttonStyling);
      child.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, (int) ((double) parentRect.Width / 5.0), this.headerHeight)));
      this.header.AddChild((UIElement) child);
      this.AddChild(this.header);
      this.main = new UIElement(Rectangle.Empty);
      this.main.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, this.headerHeight, parentRect.Width, parentRect.Height - this.headerHeight)));
      this.AddChild(this.main);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.InfoLine
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal class InfoLine : UIElement
  {
    private UIElement label;
    private UIElement info;

    public InfoLine(Rectangle rectangle, string labelText, string infoText, Styling? style = null)
      : base(rectangle, style)
    {
      Styling styling = new Styling()
      {
        //font = (style?.font ?? TextureManager.GetFont("megaMan2")),
        defaultTextColor = ColorManager.GetColor("white")
      } with
      {
        text = labelText
      };
      this.AddChild(this.label = new UIElement(new Rectangle(0, 0, rectangle.Width, rectangle.Height),
          new Styling?(styling)));
      styling.text = infoText;
      styling.defaultTextColor = ColorManager.GetColor("red");
      styling.rightText = true;
      this.AddChild(this.info = new UIElement(new Rectangle(0, 0, rectangle.Width, rectangle.Height),
          new Styling?(styling)));
    }

    public void SetLabelText(string text) => this.label?.SetText(text);

    public void SetInfoText(string text) => this.info?.SetText(text);
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ToggleLine
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal class ToggleLine : UIElement
  {
    private UIElement label;
    private ToggleButton toggle;

    public ToggleLine(
      Rectangle rectangle,
      string labelText,
      Button.OnClick onClick,
      Button.OnClick onEnabledClick,
      string infoText,
      string enabledInfoText,
      bool enabled = false,
      Styling? style = null)
      : base(rectangle, style)
    {
      Styling styling1 = new Styling();
      //styling1.font = style?.font ?? TextureManager.GetFont("megaMan2");
      styling1.defaultTextColor = ColorManager.GetColor("white");
      Styling styling2 = styling1 with { text = labelText };
      this.AddChild(this.label = new UIElement(new Rectangle(0, 0, rectangle.Width, rectangle.Height), new Styling?(styling2)));
      Styling clickableRedTextStyling = Styling.ClickableRedTextStyling with
      {
        //font = styling2.font,
        rightText = true
      };
      Rectangle rectangle1 = new Rectangle(0, 0, rectangle.Width, rectangle.Height);
      Button.OnClick onClick1 = onClick;
      Button.OnClick onEnabledClick1 = onEnabledClick;
      Styling style1 = clickableRedTextStyling;
      styling1 = new Styling();
      styling1.text = infoText;
      Styling? newStyleNullable1 = new Styling?(styling1);
      Styling style2 = Styling.AddTo(style1, newStyleNullable1);
      Styling style3 = clickableRedTextStyling;
      styling1 = new Styling();
      styling1.text = enabledInfoText;
      Styling? newStyleNullable2 = new Styling?(styling1);
      Styling enabledStyle = Styling.AddTo(style3, newStyleNullable2);
      int num = enabled ? 1 : 0;
      this.AddChild((UIElement) (this.toggle = new ToggleButton(rectangle1, onClick1, onEnabledClick1, style2, enabledStyle, num != 0)));
    }
  }
}

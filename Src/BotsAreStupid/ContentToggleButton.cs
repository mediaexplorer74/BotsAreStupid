// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ContentToggleButton
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal class ContentToggleButton : ToggleButton
  {
    private ContentToggleButton.EnableHandler enableHandler;
    private UIElement content;

    public ContentToggleButton(
      Rectangle rectangle,
      UIElement content,
      ContentToggleButton.EnableHandler enableHandler = null,
      bool enabled = false,
      bool defaultStyling = true,
      Styling? buttonStyle = null,
      Styling? buttonEnabledStyle = null,
      bool createIcon = true)
      : base(rectangle, (Button.OnClick) null, new Button.OnClick(content.ToggleActive), ContentToggleButton.GetStyle(defaultStyling, buttonStyle, Styling.DefaultToggleButtonDisabledStyling), ContentToggleButton.GetStyle(defaultStyling, buttonEnabledStyle ?? buttonStyle, Styling.DefaultToggleButtonEnabledStyling), enabled)
    {
      content.SetActive(enabled);
      this.content = content;
      this.onClick = new Button.OnClick(this.EnableContent);
      this.enableHandler = enableHandler;
      if (!createIcon)
        return;
      this.AddChild(new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p, 0.9f, useHeightOnly: true))));
    }

    public void DisableContent()
    {
      if (this.enabled)
        this.Trigger(true);
      this.content.SetActive(false);
    }

    public void EnableContent()
    {
      ContentToggleButton.EnableHandler enableHandler = this.enableHandler;
      if (enableHandler != null)
        enableHandler();
      this.content.SetActive(true);
    }

    private static Styling GetStyle(bool useDefault, Styling? buttonStyle, Styling defaultStyling)
    {
      if (!buttonStyle.HasValue)
        return defaultStyling;
      return useDefault ? Styling.AddTo(defaultStyling, new Styling?(buttonStyle.Value)) : buttonStyle.Value;
    }

    public delegate void EnableHandler();
  }
}

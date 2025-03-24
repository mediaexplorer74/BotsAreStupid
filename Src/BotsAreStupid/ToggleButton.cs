// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ToggleButton
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal class ToggleButton : Button
  {
    protected bool enabled = false;
    private Styling defaultStyle;
    private Styling enabledStyle;
    protected Button.OnClick onEnabledClick;

    public bool IsEnabled => this.enabled;

    public ToggleButton(
      Rectangle rectangle,
      Button.OnClick onClick,
      Button.OnClick onEnabledClick,
      Styling style,
      Styling enabledStyle,
      bool enabled = false)
      : base(rectangle, onClick, style)
    {
      this.defaultStyle = style;
      this.enabledStyle = enabledStyle;
      this.onEnabledClick = onEnabledClick;
      if (!enabled)
        return;
      this.enabled = true;
      this.ChangeStyle(enabledStyle);
    }

    public void SetEnabled(bool value)
    {
      this.enabled = value;
      if (this.enabled)
        this.ChangeStyle(this.enabledStyle);
      else
        this.ChangeStyle(this.defaultStyle);
    }

    public override void SetText(string s)
    {
      base.SetText(s);
      this.defaultStyle.text = s;
      this.enabledStyle.text = s;
    }

    public override void Trigger(bool right = true)
    {
      if (!this.enabled)
      {
        Button.OnClick onClick = this.onClick;
        if (onClick != null)
          onClick();
        this.enabled = true;
        this.ChangeStyle(this.enabledStyle);
      }
      else
      {
        Button.OnClick onEnabledClick = this.onEnabledClick;
        if (onEnabledClick != null)
          onEnabledClick();
        this.enabled = false;
        this.ChangeStyle(this.defaultStyle);
      }
    }
  }
}

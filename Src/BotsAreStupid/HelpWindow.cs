// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.HelpWindow
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace BotsAreStupid
{
  internal class HelpWindow : Flexbox
  {
    private const int elementHeight = 25;
    private const int elementWidth = 250;
    private const int padding = 10;

    public HelpWindow(Rectangle rectangle)
      : base(rectangle, new Styling?(new Styling()
      {
        defaultColor = ColorManager.GetColor("white"),
        padding = 10,
        margin = 10
      }))
    {
      this.CreateChildren();
      this.DrawOnTop = true;
      ColorManager.OnColorSchemeChange += new Action<ColorScheme>(this.OnColorSchemeChange);
    }

    protected override void DrawBorder(SpriteBatch spriteBatch)
    {
      base.DrawBorder(spriteBatch);
      Utility.DrawOutline(spriteBatch, this.GlobalRect, 4, ColorManager.GetColor("red"));
    }

    public void UpdateAvailableCommands()
    {
      this.Clear();
      ColorScheme currentScheme = ColorManager.CurrentScheme;
      Styling style = new Styling()
      {
        defaultTextColor = currentScheme.textColor,
        hoverColor = currentScheme.commentColor,
        clickColor = ColorManager.GetColor("red"),
        textColorEffects = true,
        tooltipDelay = 0.5f,
        textOffset = 10,
        //font = TextureManager.GetFont("couriermedium")
      };
      foreach (ScriptCommand scriptCommand in Enumerable.Where<ScriptCommand>((IEnumerable<ScriptCommand>) ScriptParser.Instance.AvailableCommands, (Func<ScriptCommand, bool>) (command => !command.HideBluePrint && command.IsUnlockedIn(SimulationManager.MainSimulation))))
      {
        style.text = scriptCommand.Command + (scriptCommand.Parameters.Length != 0 ? " " : "");
        style.tooltip = scriptCommand.Tooltip;
        this.Add((UIElement) new Button(new Rectangle(0, 0, 250, 25), new Button.OnClickString(this.HandleLineClick), style, true));
      }
    }

    private void OnColorSchemeChange(ColorScheme colorScheme)
    {
      this.style.color = colorScheme.backgroundColor;
      this.style.defaultColor = colorScheme.backgroundColor;
      this.style.hoverColor = Utility.ChangeColor(colorScheme.backgroundColor, -0.05f);
      if (!this.IsCurrentState())
        return;
      this.UpdateAvailableCommands();
    }

    private void CreateChildren()
    {
      Styling style1 = new Styling()
      {
        text = "Commands:",
        borderColor = ColorManager.GetColor("darkslate"),
        borderWidth = 2,
        textOffset = 20,
        //font = TextureManager.GetFont("megaMan2"),
        defaultColor = ColorManager.GetColor("lightslate"),
        defaultTextColor = ColorManager.GetColor("white")
      };
      this.SetHeader(new UIElement(new Rectangle(0, 0, 270, 50), new Styling?(style1)));
      this.UpdateAvailableCommands();
      UIElement footer = new UIElement(new Rectangle(0, 0, 270, 100));
      Styling style2 = Styling.AddTo(style1, new Styling?(new Styling()
      {
        centerText = true,
        hoverColor = style1.borderColor
      })) with
      {
        text = "Shortcuts",
        tooltip = "- Right Click on line: Add/Remove skip point\n- R: Reset Level\n- Alt/Shift + Enter: Start Instructions\n\n- Shift + ArrowKey: Grow Selection\n- Alt + UpArrow/DownArrow: Move Line\n- Control + UpArrow/DownArrow on number: Increase/Decrease Number\n- Control + ArrowKey: Jump to end\n- Home: Jump to beginning of line\n- End: Jump to end of line\n- Shift + Backspace/Delete: Remove Line\n\n- Control + C: Copy Selection\n- Control + X: Cut Selection\n- Control + V: Paste Selection"
      };
      footer.AddChild(new UIElement(new Rectangle(0, 0, footer.Width, 50), new Styling?(Styling.AddTo(style2, new Styling?(new Styling()
      {
        tooltipDelay = 0.1f
      })))));
      style2.clickColor = ColorManager.GetColor("red");
      style2.text = "Replay Intro";
      style2.tooltip = "Replay the Introduction script, which plays when\nstarting the Level for the first time.";
      footer.AddChild((UIElement) new Button(new Rectangle(0, 50, footer.Width, 50), new Button.OnClick(LevelManager.PlayIntro), style2));
      this.SetFooter(footer);
    }

    private void HandleLineClick(string s) => Utility.CreateMovableCommand(s);
  }
}

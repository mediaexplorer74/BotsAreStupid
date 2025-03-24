// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Accordion
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal static class Accordion
  {
    public static UIElement Create(
      Rectangle rectangle,
      string label,
      out InfoLine infoLine,
      Styling infoStyling,
      System.Action toggleHandler,
      UIElement contentElement)
    {
      UIElement element = new UIElement(rectangle);
      int height = rectangle.Height;
      int num = rectangle.Width / 10;
      element.AddChild((UIElement) (infoLine = new InfoLine(new Rectangle(0, 0, rectangle.Width - height * 3, height), label, "-", new Styling?(infoStyling))));
      Accordion.CreateToggle(rectangle, height, element, contentElement, toggleHandler);
      contentElement.SetActive(false);
      return element;
    }

    public static UIElement Create(
      Rectangle rectangle,
      string label,
      string varName,
      out ToggleLine toggleLine,
      Styling infoStyling,
      System.Action toggleHandler,
      UIElement contentElement,
      string tooltip = null)
    {
      UIElement element = new UIElement(rectangle);
      int height = rectangle.Height;
      int num = rectangle.Width / 10;
      element.AddChild((UIElement) (toggleLine = new ToggleLine(new Rectangle(0, 0, rectangle.Width - height * 3, height), label, (Button.OnClick) (() => VarManager.SetBool(varName, true)), (Button.OnClick) (() => VarManager.SetBool(varName, false)), "Off", "On", VarManager.GetBool(varName), new Styling?(infoStyling))));
      Accordion.CreateToggle(rectangle, height, element, contentElement, toggleHandler);
      contentElement.SetActive(false);
      return element;
    }

    private static void CreateToggle(
      Rectangle rectangle,
      int fifthHeight,
      UIElement element,
      UIElement contentElement,
      System.Action toggleHandler)
    {
      Color color = ColorManager.GetColor("white");
      Styling style = new Styling()
      {
        round = true,
        rotation = new float?(0.0f),
        defaultColor = color,
        hoverColor = Utility.ChangeColor(color, -0.3f),
        spritePos = TextureManager.GetSpritePos("ui_right")
      };
      element.AddChild((UIElement) new ToggleButton(new Rectangle(rectangle.Width - fifthHeight * 2, -fifthHeight / 2, fifthHeight * 2, fifthHeight * 2), (Button.OnClick) (() =>
      {
        contentElement.SetActive(true);
        System.Action action = toggleHandler;
        if (action == null)
          return;
        action();
      }), (Button.OnClick) (() =>
      {
        contentElement.SetActive(false);
        System.Action action = toggleHandler;
        if (action == null)
          return;
        action();
      }), style, Styling.AddTo(style, new Styling?(new Styling()
      {
        rotation = new float?(90f)
      }))));
    }
  }
}

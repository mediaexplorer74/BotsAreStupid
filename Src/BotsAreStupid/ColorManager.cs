// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ColorManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace BotsAreStupid
{
  internal class ColorManager
  {
    public static ColorManager Instance = new ColorManager();
    public static ColorScheme CurrentScheme;
    public static string[] AvailableSchemes = Enumerable.ToArray<string>((IEnumerable<string>) ColorManager.Instance.schemes.Keys);
    private Dictionary<string, Color> colors = new Dictionary<string, Color>();
    private Dictionary<string, ColorScheme> schemes = new Dictionary<string, ColorScheme>();

    public static event Action<ColorScheme> OnColorSchemeChange;

    public ColorManager()
    {
      ColorManager.Instance = this;
      this.colors.Add("white", new Color(244, 244, 228));
      this.colors.Add("red", new Color(200, 87, 117));
      this.colors.Add("lightslate", new Color(47, 54, 73));
      this.colors.Add("darkslate", new Color(24, 28, 41));
      this.colors.Add("cursor", new Color(50, 50, 50, 100));
      this.colors.Add("gray", new Color(83, 94, 105));
      this.colors.Add("orange", new Color((int) byte.MaxValue, 173, 81));
      this.schemes.Add("default", new ColorScheme()
      {
        backgroundColor = ColorManager.GetColor("white"),
        textColor = Color.Black,
        commentColor = Color.DarkSlateGray,
        cursorColor = ColorManager.GetColor("cursor"),
        selectionColor = Utility.ChangeColor(ColorManager.GetColor("red"), 0.5f),
        accentColor = ColorManager.GetColor("red"),
        weakenedAccentColor = Utility.ChangeColor(ColorManager.GetColor("red"), -0.25f)
      });
      this.schemes.Add("dark", new ColorScheme()
      {
        backgroundColor = Utility.ChangeColor(ColorManager.GetColor("darkslate"), 0.1f),
        textColor = ColorManager.GetColor("white"),
        commentColor = Utility.ChangeColor(ColorManager.GetColor("white"), -0.4f),
        cursorColor = Utility.ChangeColor(ColorManager.GetColor("cursor"), 0.3f),
        selectionColor = Utility.ChangeColor(ColorManager.GetColor("red"), -0.25f),
        accentColor = ColorManager.GetColor("red"),
        weakenedAccentColor = Utility.ChangeColor(ColorManager.GetColor("red"), 0.25f)
      });
      ColorScheme colorScheme;
      if (VarManager.HasString("colorscheme") && ColorManager.TryGetScheme(VarManager.GetString("colorscheme"), out colorScheme))
        ColorManager.CurrentScheme = colorScheme;
      else
        ColorManager.CurrentScheme = this.schemes["default"];
    }

    public static Color GetColor(string name)
    {
      Color color;
      ColorManager.Instance.colors.TryGetValue(name.ToLower(), out color);
      return color;
    }

    private static bool TryGetScheme(string name, out ColorScheme colorScheme)
    {
      return ColorManager.Instance.schemes.TryGetValue(name, out colorScheme);
    }

    public static void SetCurrentScheme(string name)
    {
      ColorScheme colorScheme;
      if (!ColorManager.TryGetScheme(name, out colorScheme))
        return;
      ColorManager.CurrentScheme = colorScheme;
      VarManager.SetString("colorscheme", name);
      VarManager.SaveOptions();
      Action<ColorScheme> colorSchemeChange = ColorManager.OnColorSchemeChange;
      if (colorSchemeChange != null)
        colorSchemeChange(ColorManager.CurrentScheme);
    }
  }
}

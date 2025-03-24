// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Styling
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.BitmapFonts;

#nullable disable
namespace BotsAreStupid
{
  internal struct Styling
  {
    public int borderWidth;
    public Color borderColor;
    public Color color;
    public Color textColor;
    public Color defaultTextColor;
    public Color defaultColor;
    public Color hoverColor;
    public bool? hoverCursor;
    public Color clickColor;
    public bool textColorEffects;
    public bool centerText;
    public bool rightText;
    public bool useTextRegex;
    public Texture2D texture;
    public Rectangle spritePos;
    public Rectangle? hoverSpritePos;
    public bool round;
    public GameState drawInState;
    public string text;
    public int textOffset;
    public int textOffsetRight;
    //public BitmapFont font;
    public string value;
    public string tooltip;
    public float tooltipDelay;
    public int sliderHeight;
    public float sliderThumbScale;
    public float textRevealTime;
    public float textLifeTime;
    public int margin;
    public int padding;
    public float? rotation;
    public BorderInfo enabledBorders;
    public bool isLevelUi;
    public bool isPriority;
    public int shadowSize;
    public bool interactableIgnoreBorder;
    public static Styling DefaultButtonStyling = new Styling()
    {
      defaultColor = ColorManager.GetColor("darkSlate"),
      hoverColor = ColorManager.GetColor("lightSlate"),
      clickColor = ColorManager.GetColor("red"),
      defaultTextColor = ColorManager.GetColor("white"),
      //font = TextureManager.GetFont("megaMan2"),
      borderColor = ColorManager.GetColor("white"),
      borderWidth = 4,
      centerText = true
    };
    public static Styling Null = new Styling();

    public static Styling AddTo(Styling style, Styling? newStyleNullable)
    {
      if (!newStyleNullable.HasValue)
        return style;
      Styling styling = newStyleNullable.Value;
      if (styling.defaultColor != Color.Transparent)
        style.defaultColor = styling.defaultColor;
      if (styling.hoverColor != Color.Transparent)
        style.hoverColor = styling.hoverColor;
      if (styling.clickColor != Color.Transparent)
        style.clickColor = styling.clickColor;
      if (styling.textColor != Color.Transparent)
        style.textColor = styling.textColor;
      if (styling.defaultTextColor != Color.Transparent)
        style.defaultTextColor = styling.defaultTextColor;
      if (styling.borderWidth != 0)
        style.borderWidth = styling.borderWidth;
      if (styling.borderColor != Color.Transparent)
        style.borderColor = styling.borderColor;
      if (styling.texture != null)
        style.texture = styling.texture;
      if (styling.spritePos != Rectangle.Empty)
        style.spritePos = styling.spritePos;
      int num;
      if (styling.hoverSpritePos.HasValue)
      {
        Rectangle? hoverSpritePos = styling.hoverSpritePos;
        Rectangle empty = Rectangle.Empty;
        num = hoverSpritePos.HasValue ? (hoverSpritePos.HasValue ? (hoverSpritePos.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1;
      }
      else
        num = 0;
      if (num != 0)
        style.hoverSpritePos = styling.hoverSpritePos;
      if (!string.IsNullOrEmpty(styling.text))
        style.text = styling.text;
      if (!string.IsNullOrEmpty(styling.value))
        style.value = styling.value;
      if (!string.IsNullOrEmpty(styling.tooltip))
        style.tooltip = styling.tooltip;
      if (styling.tooltip?.ToLower() == "null")
        style.tooltip = (string) null;
      if (styling.textOffset != 0)
        style.textOffset = styling.textOffset;
      //if (styling.font != null)
      //  style.font = styling.font;
      if ((double) styling.textRevealTime != 0.0)
        style.textRevealTime = styling.textRevealTime;
      if ((double) styling.textLifeTime != 0.0)
        style.textLifeTime = styling.textLifeTime;
      if (styling.margin != 0)
        style.margin = styling.margin;
      if (styling.padding != 0)
        style.padding = styling.padding;
      if (styling.shadowSize != 0)
        style.shadowSize = styling.shadowSize;
      if ((double) styling.tooltipDelay != 0.0)
        style.tooltipDelay = styling.tooltipDelay;
      if (styling.rotation.HasValue)
        style.rotation = styling.rotation;
      if (styling.isLevelUi)
        style.isLevelUi = true;
      if (styling.isPriority)
        style.isPriority = true;
      if (styling.centerText)
        style.centerText = true;
      if (styling.useTextRegex)
        style.useTextRegex = true;
      if (styling.interactableIgnoreBorder)
        style.interactableIgnoreBorder = true;
      style.hoverCursor = styling.hoverCursor ?? style.hoverCursor;
      if (!styling.enabledBorders.Equals((object) BorderInfo.None))
        style.enabledBorders = styling.enabledBorders;
      if (styling.drawInState != 0)
        style.drawInState = styling.drawInState;
      return style;
    }

    public static Styling DefaultToggleButtonDisabledStyling
    {
      get
      {
        return Styling.DefaultButtonStyling with
        {
          borderWidth = 2,
          defaultColor = new Color(ColorManager.GetColor("lightslate"), 0.2f),
          hoverColor = new Color(ColorManager.GetColor("lightslate"), 0.5f),
          clickColor = ColorManager.GetColor("lightslate")
        };
      }
    }

    public static Styling DefaultToggleButtonEnabledStyling
    {
      get
      {
        return Styling.DefaultButtonStyling with
        {
          borderWidth = 2,
          defaultColor = new Color(ColorManager.GetColor("red"), 0.2f),
          hoverColor = new Color(ColorManager.GetColor("red"), 0.5f),
          clickColor = ColorManager.GetColor("red")
        };
      }
    }

    public static Styling RedButtonStyling
    {
      get
      {
        Styling defaultButtonStyling = Styling.DefaultButtonStyling with
        {
          defaultColor = ColorManager.GetColor("red")
        };
        defaultButtonStyling.hoverColor = new Color(defaultButtonStyling.defaultColor, 0.5f);
        defaultButtonStyling.clickColor = new Color(defaultButtonStyling.defaultColor, 0.2f);
        return defaultButtonStyling;
      }
    }

    public static Styling ClickableRedTextStyling
    {
      get
      {
        Styling clickableRedTextStyling = new Styling()
        {
          textColorEffects = true,
          defaultTextColor = ColorManager.GetColor("red")
        };
        clickableRedTextStyling.hoverColor = Utility.ChangeColor(clickableRedTextStyling.defaultTextColor, 0.4f);
        clickableRedTextStyling.clickColor = Utility.ChangeColor(clickableRedTextStyling.defaultTextColor, -0.2f);
        //clickableRedTextStyling.font = TextureManager.GetFont("megaMan2");
        return clickableRedTextStyling;
      }
    }
  }
}

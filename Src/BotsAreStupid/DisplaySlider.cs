// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.DisplaySlider
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class DisplaySlider : UIElement
  {
    private DisplaySlider.ValueToString valueConverter;
    private const int margin = 25;
    private const int defaultSliderHeight = 15;
    private Slider slider;
    private UIElement label;
    private UIElement display;
    private string suffix;
    private bool enablePromptInput;
    private Slider.SliderChangeHandler changeHandler;

    private string LabelText { get; set; }

    public DisplaySlider(
      Rectangle rectangle,
      Slider.SliderChangeHandler changeHandler,
      string labelText,
      int minValue,
      int maxValue,
      int value = -1,
      string suffix = "",
      Styling? style = null,
      DisplaySlider.ValueToString valueConverter = null,
      bool enablePromptInput = true)
      : base(rectangle, style)
    {
      this.changeHandler = changeHandler;
      this.suffix = suffix;
      this.valueConverter = valueConverter;
      this.enablePromptInput = enablePromptInput;
      //BitmapFont font = style?.font ?? TextureManager.GetFont("megaMan2");
      int labelSizeX = 24;//(int) font.MeasureStringHalf(labelText).Width;
      this.LabelText = labelText;
      this.AddChild(this.label = new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, labelSizeX, parentRect.Height / 3))));
      int displayOffset = (style.HasValue ? (style.GetValueOrDefault().rightText ? 1 : 0) : 0) != 0 ? 0 : 25;
      Color color = ColorManager.GetColor("red");
      Styling styling;
      if (enablePromptInput)
      {
        this.AddChild(this.display = new Button(Rectangle.Empty, (Button.OnClick) (() => PopupMenu.Instance.Prompt(this.LabelText, new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), (PromptMenu.InteractionHandlerString) (s =>
        {
          this.SetValue(int.Parse(s));
          PopupMenu.Instance.SetActive(false);
        }), (PromptMenu.Validator) ((string input, out string msg) => Utility.NumberValidator(input, out msg, this.slider.GetMax(), this.slider.GetMin())))), Styling.ClickableRedTextStyling with
        {
          //font = font,
          text = this.GetValueString(value) + suffix,
          rightText = style.HasValue && style.GetValueOrDefault().rightText
        }).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(labelSizeX + displayOffset, 0, parentRect.Width - labelSizeX - displayOffset, parentRect.Height / 3))));
      }
      else
      {
        Rectangle empty = Rectangle.Empty;
        styling = new Styling();
        styling.text = this.GetValueString(value) + suffix;
        styling.defaultTextColor = color;
        //styling.font = font;
        styling.rightText = style.HasValue && style.GetValueOrDefault().rightText;
        Styling? style1 = new Styling?(styling);
        this.AddChild(this.display = new UIElement(empty, style1).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(labelSizeX + displayOffset, 0, parentRect.Width - labelSizeX - displayOffset, parentRect.Height / 3))));
      }
            int sliderHeight = !style.HasValue || (style.HasValue ? (style.GetValueOrDefault().sliderHeight == 0 ? 1 : 0) : 0) != 0
                      ? 15 : 24;//style?.sliderHeight.Value;
            float num1 = !style.HasValue || (style.HasValue ? ((double)style.GetValueOrDefault().sliderThumbScale == 0.0 ? 1 : 0) : 0) != 0
                      ? 1f : 24;//style?.sliderThumbScale.Value;
      Rectangle empty1 = Rectangle.Empty;
      Slider.SliderChangeHandler changeHandler1 = new Slider.SliderChangeHandler(this.OnSliderChange);
      int minValue1 = minValue;
      int maxValue1 = maxValue;
      styling = new Styling();
      styling.defaultColor = ColorManager.GetColor("white");
      styling.hoverCursor = new bool?(true);
      styling.sliderThumbScale = num1;
      Styling style2 = styling;
      int num2 = value;
      this.AddChild((UIElement) (this.slider = new Slider(empty1, changeHandler1, minValue1, maxValue1, style2, num2)));
      this.slider.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - 2 * sliderHeight, parentRect.Width, sliderHeight)));
    }

    private string GetValueString(int val)
    {
      return this.valueConverter != null ? this.valueConverter(val) : val.ToString();
    }

    public void SetLabelText(string text)
    {
      this.label?.SetText(text);
      this.LabelText = text;
    }

    public void SetMinMax(int min, int max) => this.slider.SetMinMax(new int?(min), new int?(max));

    public void SetValue(int value, bool ignoreHandler = false)
    {
      this.display?.SetText(value.ToString() + this.suffix);
      this.slider.SetValue(value, ignoreHandler);
    }

    public void SetMixedValue(int min, int max)
    {
      this.SetValue((int) Math.Ceiling((double) (min + max) / 2.0));
      this.display?.SetText(min.ToString() + " to " + max.ToString() + this.suffix);
    }

    private void OnSliderChange(int value, bool final = false)
    {
      this.display?.SetText(this.GetValueString(value) + this.suffix);
      Slider.SliderChangeHandler changeHandler = this.changeHandler;
      if (changeHandler == null)
        return;
      changeHandler(value, final);
    }

    public delegate string ValueToString(int val);
  }
}

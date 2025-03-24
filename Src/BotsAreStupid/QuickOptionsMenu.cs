// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.QuickOptionsMenu
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal class QuickOptionsMenu : BasicTabMenu
  {
    private const int buttonHeight = 50;
    private const int padding = 50;
    private const int sliderHeight = 55;
    private const int sliderPadding = 10;

    protected override int headerHeight => 50;

    public QuickOptionsMenu(
      Rectangle rectangle,
      Styling buttonStyling,
      Button.OnClick backButtonHandler)
      : base(rectangle, GameState.Any, new Styling?(buttonStyling), backButtonHandler, "General", "Editor")
    {
      this.CreateChildren();
    }

    private void CreateChildren()
    {
      Styling styling = new Styling()
      {
        rightText = true,
        sliderHeight = 11,
        sliderThumbScale = 0.75f
      };
      this.buttonStyling.enabledBorders = BorderInfo.All;
      UIElement main1 = this.main;
      Rectangle empty1 = Rectangle.Empty;
      int num1 = VarManager.GetInt("mastervolume");
      Styling? style1 = new Styling?(styling);
      DisplaySlider masterSlider;
      DisplaySlider child1 = masterSlider = new DisplaySlider(empty1, (Slider.SliderChangeHandler) ((v, f) => SoundManager.SetVolume("master", v, f)), "Master Volume:", 0, 100, num1, style: style1, enablePromptInput: false);
      main1.AddChild((UIElement) child1);
      masterSlider.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(50, 55, parentRect.Width - 100, 55)));
      VarManager.AddListener("mastervolume", (VarManager.VarChangeHandler) (v => masterSlider.SetValue(int.Parse(v), true)));
      UIElement main2 = this.main;
      Rectangle empty2 = Rectangle.Empty;
      int num2 = VarManager.GetInt("sfxvolume");
      Styling? style2 = new Styling?(styling);
      DisplaySlider sfxSlider;
      DisplaySlider child2 = sfxSlider = new DisplaySlider(empty2, (Slider.SliderChangeHandler) ((v, f) => SoundManager.SetVolume("sfx", v, f)), "SFX Volume:", 0, 100, num2, style: style2, enablePromptInput: false);
      main2.AddChild((UIElement) child2);
      sfxSlider.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(50, 120, parentRect.Width - 100, 55)));
      VarManager.AddListener("sfxvolume", (VarManager.VarChangeHandler) (v => sfxSlider.SetValue(int.Parse(v), true)));
      UIElement main3 = this.main;
      Rectangle empty3 = Rectangle.Empty;
      int num3 = VarManager.GetInt("musicvolume");
      Styling? style3 = new Styling?(styling);
      DisplaySlider musicSlider;
      DisplaySlider child3 = musicSlider = new DisplaySlider(empty3, (Slider.SliderChangeHandler) ((v, f) => SoundManager.SetVolume("music", v, f)), "Music Volume:", 0, 100, num3, style: style3, enablePromptInput: false);
      main3.AddChild((UIElement) child3);
      musicSlider.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(50, 185, parentRect.Width - 100, 55)));
      VarManager.AddListener("musicvolume", (VarManager.VarChangeHandler) (v => musicSlider.SetValue(int.Parse(v), true)));
      Styling buttonStyling1 = this.buttonStyling;
      this.buttonStyling.tooltip = (string) null;
      buttonStyling1.tooltip = (string) null;
      this.buttonStyling.text = "Fullscreen:   Off";
      buttonStyling1.text = "Fullscreen:   On";
      ToggleButton fullscreenToggle = new ToggleButton(Rectangle.Empty, new Button.OnClick(Game1.ToggleFullscreen), new Button.OnClick(Game1.ToggleFullscreen), this.buttonStyling, buttonStyling1, VarManager.GetBool("fullscreen"));
      fullscreenToggle.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(50, parentRect.Height - 100 - 10, parentRect.Width - 100, 55)));
      VarManager.AddListener("fullscreen", (VarManager.VarChangeHandler) (v => fullscreenToggle.SetEnabled(bool.Parse(v))));
      this.main.AddChild((UIElement) fullscreenToggle);
      Styling buttonStyling2 = this.buttonStyling with
      {
        clickColor = this.buttonStyling.defaultColor
      };
      ArrowSelection child4 = new ArrowSelection(Rectangle.Empty, "Scale:   ", (ArrowSelection.DropdownChangeHandler) (v =>
      {
        VarManager.SetInt("editoruiscale", (int) ((double) float.Parse(v) * 100.0));
        Game1.Instance.UpdateMatrices();
        VarManager.SaveOptions();
      }), buttonStyling2, ((float) VarManager.GetInt("editoruiscale") / 100f).ToString("0.##"), false, (ArrowSelection.ValueDisplayModifier) (v => v + "x"), new string[5]
      {
        "0.5",
        "0.75",
        "1",
        "1.25",
        "1.5"
      });
      child4.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(50, 50, parentRect.Width - 100, 50)));
      this.contentElements[1].AddChild((UIElement) child4);
      ArrowSelection child5 = new ArrowSelection(Rectangle.Empty, "Text Size:   ", (ArrowSelection.DropdownChangeHandler) (v =>
      {
        VarManager.SetInt("textsize", int.Parse(v) - 1);
        VarManager.SaveOptions();
      }), buttonStyling2, (VarManager.GetInt("textsize") + 1).ToString(), false, (ArrowSelection.ValueDisplayModifier) (v => v.Replace("1", "Small").Replace("2", "Medium").Replace("3", "Large")), new string[3]
      {
        "1",
        "2",
        "3"
      });
      child5.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(50, 125, parentRect.Width - 100, 50)));
      this.contentElements[1].AddChild((UIElement) child5);
      ArrowSelection child6 = new ArrowSelection(Rectangle.Empty, "Color Scheme:   ", (ArrowSelection.DropdownChangeHandler) (v => ColorManager.SetCurrentScheme(v)), buttonStyling2, VarManager.GetString("colorscheme"), false, (ArrowSelection.ValueDisplayModifier) null, ColorManager.AvailableSchemes);
      child6.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(50, 200, parentRect.Width - 100, 50)));
      this.contentElements[1].AddChild((UIElement) child6);
      this.buttonStyling.text = "Live Script Editing:   Off";
      buttonStyling1.text = "Live Script Editing:   On";
      ToggleButton liveEditingToggle = new ToggleButton(Rectangle.Empty, new Button.OnClick(Options.ToggleLiveScriptEditing), new Button.OnClick(Options.ToggleLiveScriptEditing), this.buttonStyling, buttonStyling1, VarManager.GetBool("livescriptchanges"));
      liveEditingToggle.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(50, parentRect.Height - 100 - 10, parentRect.Width - 100, 55)));
      VarManager.AddListener("livescriptchanges", (VarManager.VarChangeHandler) (v => liveEditingToggle.SetEnabled(bool.Parse(v))));
      this.contentElements[1].AddChild((UIElement) liveEditingToggle);
    }
  }
}

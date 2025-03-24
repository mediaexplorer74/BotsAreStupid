// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.BasicTabMenu
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal abstract class BasicTabMenu : BasicMenu
  {
    private int activeTab;
    private Button[] tabButtons;
    protected UIElement[] contentElements;

    public BasicTabMenu(
      Rectangle rectangle,
      GameState drawInState,
      Styling? buttonStyling,
      Button.OnClick backButtonHandler,
      params string[] tabLabels)
      : base(rectangle, drawInState, buttonStyling, backButtonHandler)
    {
      this.CreateChildren(tabLabels);
      this.SetTabActive(0);
    }

    private void CreateChildren(string[] tabLabels)
    {
      int tabAmount = tabLabels.Length;
      this.tabButtons = new Button[tabAmount];
      this.contentElements = new UIElement[tabAmount];
      this.buttonStyling.enabledBorders.left = true;
      for (int index = 0; index < tabAmount; ++index)
      {
        if (index == tabAmount - 1)
          this.buttonStyling.enabledBorders.right = false;
        this.buttonStyling.text = tabLabels[index];
        int idx = index;
        this.header.AddChild((UIElement) (this.tabButtons[index] = new Button(Rectangle.Empty, (Button.OnClick) (() => this.SetTabActive(idx)), this.buttonStyling)));
        this.tabButtons[index].SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect =>
        {
          float num = (float) parentRect.Width / 5f;
          float width = ((float) parentRect.Width - num) / (float) tabAmount;
          return new Rectangle((int) ((double) num + (double) width * (double) idx), 0, (int) width, this.headerHeight);
        }));
        if (index == 0)
        {
          this.contentElements[index] = this.main;
        }
        else
        {
          this.AddChild(this.contentElements[index] = this.main.Copy());
          this.contentElements[index].SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, this.headerHeight, parentRect.Width, parentRect.Height - this.headerHeight)));
        }
      }
    }

    protected virtual void SetTabActive(int tab)
    {
      this.activeTab = tab;
      Color color = ColorManager.GetColor("lightslate");
      Styling styling1 = new Styling()
      {
        defaultColor = color,
        borderWidth = -1,
        clickColor = color,
        hoverCursor = new bool?(false)
      };
      Styling styling2 = new Styling()
      {
        defaultColor = ColorManager.GetColor("darkslate"),
        borderWidth = 4,
        clickColor = ColorManager.GetColor("red"),
        hoverCursor = new bool?(true)
      };
      for (int index = 0; index < this.tabButtons.Length; ++index)
      {
        bool active = this.activeTab == index;
        this.tabButtons[index].ChangeStyle(active ? styling1 : styling2);
        this.contentElements[index].SetActive(active);
      }
    }
  }
}

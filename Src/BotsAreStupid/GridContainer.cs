// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.GridContainer
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class GridContainer : UIElement
  {
    private const int pageButtonMarginSide = 30;
    private int columns;
    private int rows;
    private int minElementWidth;
    private int minElementHeight;
    private float maxElementSizeRatio;
    private bool forceSquareElements;
    private int horizontalPadding;
    private int verticalPadding;
    private int pageButtonSize;
    private int pageButtonMarginBottom;
    private int currentPage;
    private int pageCount;
    private UIElement pageInfo;
    private Button nextPageButton;
    private Button previousPageButton;

    public UIElement[] Elements { get; private set; }

    public GridContainer(
      Rectangle rectangle,
      int minElementWidth,
      int minElementHeight,
      int horizontalPadding,
      int verticalPadding,
      int pageButtonMarginBottom = 10,
      int pageButtonSize = 20,
      float maxElementSizeRatio = 1.2f,
      bool forceSquareElements = false)
      : base(rectangle)
    {
      this.minElementWidth = minElementWidth;
      this.minElementHeight = minElementHeight;
      this.horizontalPadding = horizontalPadding;
      this.verticalPadding = verticalPadding;
      this.pageButtonMarginBottom = pageButtonMarginBottom;
      this.pageButtonSize = pageButtonSize;
      this.maxElementSizeRatio = maxElementSizeRatio;
      this.forceSquareElements = forceSquareElements;
    }

    public void SetElements(params UIElement[] elements)
    {
      this.Elements = elements;
      this.Children.Clear();
      this.currentPage = 1;
      this.columns = (int) ((double) this.rectangle.Width / ((double) this.minElementWidth + (double) this.horizontalPadding));
      this.rows = (int) ((double) this.rectangle.Height / ((double) this.minElementHeight + (double) this.verticalPadding));
      Color color = ColorManager.GetColor("white");
      BitmapFont font = TextureManager.GetFont("megaMan2");
            int pageInfoWidth = 24;//(int) font.MeasureStringHalf(this.pageCount.ToString() + "/" + this.pageCount.ToString()).Width;
      int y = this.rectangle.Height - this.pageButtonSize - this.pageButtonMarginBottom;
      this.AddChild(this.pageInfo = new UIElement(new Rectangle(0, y, this.rectangle.Width, this.pageButtonSize)));
      this.pageInfo.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - this.pageButtonSize - this.pageButtonMarginBottom, this.rectangle.Width, this.pageButtonSize)));
      Styling defaultButtonStyling = Styling.DefaultButtonStyling with
      {
        borderWidth = 2
      };
      this.AddChild((UIElement) (this.previousPageButton = new Button(new Rectangle(this.rectangle.Width / 2 - pageInfoWidth / 2 - 30 - this.pageButtonSize, y, this.pageButtonSize, this.pageButtonSize), new Button.OnClick(this.NextPage), defaultButtonStyling)));
      this.previousPageButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 - pageInfoWidth / 2 - 30 - this.pageButtonSize, parentRect.Height - this.pageButtonSize - this.pageButtonMarginBottom, this.pageButtonSize, this.pageButtonSize)));
      this.previousPageButton.AddChild(new UIElement(Rectangle.Empty).SetResponsiveRectangle(new UIElement.ResponsiveRectangleHandler(buttonIconRect)));
      this.AddChild((UIElement) (this.nextPageButton = new Button(new Rectangle(this.rectangle.Width / 2 + pageInfoWidth / 2 + 30, y, this.pageButtonSize, this.pageButtonSize), new Button.OnClick(this.PreviousPage), defaultButtonStyling)));
      this.nextPageButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 + pageInfoWidth / 2 + 30, parentRect.Height - this.pageButtonSize - this.pageButtonMarginBottom, this.pageButtonSize, this.pageButtonSize)));
      this.nextPageButton.AddChild(new UIElement(Rectangle.Empty).SetResponsiveRectangle(new UIElement.ResponsiveRectangleHandler(buttonIconRect)));
      foreach (UIElement element in elements)
        this.AddChild(element);
      this.AddResponsiveAction((UIElement.ResponsiveActionHandler) (parentRect =>
      {
        this.columns = Math.Max(1, (int) ((double) this.rectangle.Width / ((this.forceSquareElements ? (double) Math.Min(this.minElementWidth, this.minElementHeight) : (double) this.minElementWidth) + (double) this.horizontalPadding)));
        this.rows = Math.Max(1, (int) ((double) this.rectangle.Height / ((this.forceSquareElements ? (double) Math.Min(this.minElementWidth, this.minElementHeight) : (double) this.minElementHeight) + (double) this.verticalPadding)));
        this.pageCount = (elements.Length - 1) / (this.rows * this.columns) + 1;
        if (this.currentPage > this.pageCount)
          this.currentPage = this.pageCount;
        int num1 = Math.Min((this.rectangle.Width - (this.columns + 1) * this.horizontalPadding) / this.columns, (int) ((double) this.minElementWidth * (double) this.maxElementSizeRatio));
        int num2 = Math.Min((this.rectangle.Height - (this.rows + 1) * this.verticalPadding) / this.rows, (int) ((double) this.minElementHeight * (double) this.maxElementSizeRatio));
        if (this.forceSquareElements)
        {
          num1 = Math.Min(num1, num2);
          num2 = num1;
        }
        int num3 = this.rectangle.Width / 2 - (int) ((double) this.columns / 2.0 * (double) (num1 + this.horizontalPadding)) + this.horizontalPadding / 2;
        int num4 = this.rectangle.Height / 2 - (int) ((double) this.rows / 2.0 * (double) (num2 + this.verticalPadding)) + this.verticalPadding / 2;
        for (int index = 0; index < elements.Length; ++index)
        {
          UIElement element = elements[index];
          int num5 = index % (this.columns * this.rows);
          int num6 = num5 % this.columns;
          int num7 = num5 / this.columns;
          int num8 = num3 + num6 * (num1 + this.horizontalPadding);
          int num9 = num4 + num7 * (num2 + this.verticalPadding);
          element.SetPosition(new int?(num8), new int?(num9));
          element.SetSize(num1, num2);
        }
        this.UpdateView();
      }));
      this.UpdateView();

      static Rectangle buttonIconRect(Rectangle parentRect)
      {
        return Utility.ScaledCenteredRect(parentRect, 0.6f);
      }
    }

    private void NextPage()
    {
      --this.currentPage;
      this.UpdateView();
    }

    private void PreviousPage()
    {
      ++this.currentPage;
      this.UpdateView();
    }

    private void UpdateView()
    {
      bool active = this.pageCount > 1;
      this.pageInfo?.SetActive(active);
      this.pageInfo?.SetText(this.currentPage.ToString() + "/" + this.pageCount.ToString());
      this.nextPageButton?.SetActive(active && this.currentPage < this.pageCount);
      this.previousPageButton?.SetActive(active && this.currentPage > 1);
      for (int index = 0; index < this.Children.Count - 3; ++index)
      {
        UIElement child = this.Children[index + 3];
        int num1 = (this.currentPage - 1) * (this.rows * this.columns);
        int num2 = this.currentPage * (this.rows * this.columns) - 1;
        child.SetActive(index >= num1 && index <= num2);
      }
    }
  }
}

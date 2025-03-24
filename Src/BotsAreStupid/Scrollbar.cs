// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Scrollbar
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class Scrollbar : UIElement
  {
    private int elements;
    private int offset = 0;
    private int displaySize;
    private int minElementsVisible;
    private UIElement slider;
    private bool isGrabbed;
    private new Vector2 lastMousePos;
    private Scrollbar.ChangeHandler changeHandler;
    private bool vertical;

    public Rectangle GlobalSliderRect => this.slider.GlobalRect;

    public bool SliderActive => this.slider.IsActive;

    public Scrollbar(
      Rectangle rectangle,
      int displaySize,
      int elements,
      Scrollbar.ChangeHandler changeHandler,
      Styling? style = null,
      bool vertical = true,
      int minElementsVisible = 1)
      : base(rectangle, style)
    {
      Scrollbar scrollbar = this;
      this.displaySize = displaySize;
      this.elements = elements;
      this.changeHandler = changeHandler;
      this.vertical = vertical;
      this.minElementsVisible = minElementsVisible;
      this.AddChild(this.slider = new Button(Rectangle.Empty, (Button.OnClick) (() => scrollbar.isGrabbed = true), new Styling()
      {
        defaultColor = new Color(Color.Gray, 0.75f),
        hoverColor = new Color(Color.Gray, 0.9f),
        clickColor = Color.Gray,
        hoverCursor = new bool?(true)
      }, true).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, vertical ? parentRect.Width : parentRect.Height, vertical ? parentRect.Width : parentRect.Height))).AddResponsiveAction((UIElement.ResponsiveActionHandler) (parentRect => scrollbar.UpdateSlider())));
      Input.RegisterMouse(InputEvent.OnUp, new InputAction.InputHandler(this.OnMouseUp));
    }

    public void SetDisplaySize(int displaySize, bool setMinElementsVisible = false)
    {
      this.displaySize = displaySize;
      if (!setMinElementsVisible)
        return;
      this.minElementsVisible = displaySize;
    }

    public void UpdateView(int elements, int offset)
    {
      this.elements = elements;
      this.offset = offset;
      this.UpdateSlider();
    }

    public void UpdateSlider()
    {
      this.slider.SetActive(this.elements > this.displaySize);
      Rectangle sliderRect = this.GetSliderRect();
      Rectangle rectangle = this.slider.GetRectangle();
      if (this.vertical)
      {
        this.slider.SetPosition(new int?(rectangle.X), new int?(sliderRect.Y));
        this.slider.SetSize(rectangle.Width, sliderRect.Height);
      }
      else
      {
        this.slider.SetPosition(new int?(sliderRect.X), new int?(rectangle.Y));
        this.slider.SetSize(sliderRect.Width, rectangle.Height);
      }
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      Vector2 vector2 = this.LocalMousePos();
      if (this.isGrabbed)
      {
        Rectangle rectangle = this.slider.GetRectangle();
        Rectangle globalRect = this.GlobalRect;
        int offset = this.offset;
        if (this.vertical)
        {
          float num = MathHelper.Clamp(vector2.Y - (float) globalRect.Y - (float) (rectangle.Height / 2), 0.0f, (float) (globalRect.Height - rectangle.Height));
          this.offset = (int) ((double) (this.elements - this.minElementsVisible) * ((double) num / (double) (globalRect.Height - rectangle.Height)));
          this.slider.SetPosition(new int?(rectangle.X), new int?((int) num));
        }
        else
        {
          float num = MathHelper.Clamp(vector2.X - (float) globalRect.X - (float) (rectangle.Width / 2), 0.0f, (float) (globalRect.Width - rectangle.Width));
          this.offset = (int) ((double) (this.elements - this.minElementsVisible) * ((double) num / (double) (globalRect.Width - rectangle.Width)));
          this.slider.SetPosition(new int?((int) num), new int?(rectangle.Y));
        }
        if (this.offset != offset)
        {
          this.PlayInteractionSound(0.025f);
          Scrollbar.ChangeHandler changeHandler = this.changeHandler;
          if (changeHandler != null)
            changeHandler(this.offset);
        }
      }
      this.lastMousePos = vector2;
    }

    private Rectangle GetSliderRect()
    {
      if (this.rectangle.Width == 0 || this.rectangle.Height == 0)
        return Rectangle.Empty;
      if (this.vertical)
      {
        int height = MathHelper.Clamp((int) ((double) this.displaySize / (double) this.elements * (double) this.rectangle.Height), this.rectangle.Width, this.rectangle.Height);
        return new Rectangle(0, Math.Min((int) ((double) this.offset / (double) this.elements * (double) this.rectangle.Height), this.rectangle.Height - height), this.rectangle.Width, height);
      }
      int width = MathHelper.Clamp((int) ((double) this.displaySize / (double) this.elements * (double) this.rectangle.Width), this.rectangle.Height, this.rectangle.Width);
      return new Rectangle(Math.Min((int) ((double) this.offset / (double) this.elements * (double) this.rectangle.Width), this.rectangle.Width - width), 0, width, this.rectangle.Height);
    }

    private void OnMouseUp()
    {
      if (!this.isGrabbed)
        return;
      this.isGrabbed = false;
    }

    public delegate void ChangeHandler(int offset);
  }
}

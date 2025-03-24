// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Slider
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class Slider : UIElement
  {
    private Slider.SliderChangeHandler changeHandler;
    protected int value;
    private int minValue;
    private int maxValue;
    private bool isGrabbed;
    private Texture2D thumbTexture;
    private Rectangle thumbSpritePos;
    private System.Action OnStartDrag;

    public bool IsGrabbed => this.isGrabbed;

    public Slider(
      Rectangle rectangle,
      Slider.SliderChangeHandler changeHandler,
      int minValue,
      int maxValue,
      Styling style,
      int value = -1,
      System.Action onStartDrag = null)
      : base(rectangle, new Styling?(style))
    {
      this.minValue = minValue;
      this.maxValue = maxValue;
      this.value = value != -1 ? value : minValue + (maxValue - minValue) / 2;
      this.changeHandler = changeHandler;
      this.OnStartDrag = onStartDrag;
      this.thumbTexture = TextureManager.GetTexture("tileset");
      this.thumbSpritePos = TextureManager.GetSpritePos("energyorb");
      Input.RegisterMouse(InputEvent.OnDown, new InputAction.InputHandler(this.OnMouseDown));
      Input.RegisterMouse(InputEvent.OnUp, new InputAction.InputHandler(this.OnMouseUp));
      Input.RegisterOnScroll(new Input.MouseScrollHandler(this.OnMouseScroll), priority: true);
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (!this.isGrabbed)
        return;
      this.GetValueFromMouse();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
      float num = (float) (((double) this.style.sliderThumbScale == 0.0 ? 1.0 : (double) this.style.sliderThumbScale) * 2.0);
      Rectangle globalRect = this.GlobalRect;
      float x = (float) globalRect.X + (float) globalRect.Width * this.GetPercentage();
      int y = globalRect.Y + globalRect.Height / 2;
      Utility.DrawLine(spriteBatch, new Vector2((float) globalRect.X, (float) globalRect.Y), new Vector2(x, (float) globalRect.Y), this.rectangle.Height, new Color?(ColorManager.GetColor("red")), 0.0f, 0);
      Vector2 vector2_1 = new Vector2((float) (this.thumbSpritePos.Width / 2), (float) (this.thumbSpritePos.Height / 2));
      Vector2 vector2_2 = new Vector2(x, (float) y);
      spriteBatch.Draw(this.thumbTexture, vector2_2, new Rectangle?(this.thumbSpritePos), Color.White, 0.0f, vector2_1, num, SpriteEffects.None, 0.5f);
    }

    public void SetMinMax(int? min = null, int? max = null)
    {
      int? nullable = min;
      this.minValue = nullable ?? this.minValue;
      nullable = max;
      this.maxValue = nullable ?? this.maxValue;
    }

    public int GetMin() => this.minValue;

    public int GetMax() => this.maxValue;

    public int GetValue() => this.value;

    public void SetValue(int value, bool ignoreHandler = false)
    {
      this.value = value;
      if (ignoreHandler)
        return;
      Slider.SliderChangeHandler changeHandler = this.changeHandler;
      if (changeHandler == null)
        return;
      changeHandler(value, true);
    }

    private float GetPercentage()
    {
      int num = this.maxValue - this.minValue;
      return (float) (MathHelper.Clamp(this.value, this.minValue, this.maxValue) - this.minValue) / (float) num;
    }

    private void SetPercentage(float percentage)
    {
      int num = this.maxValue - this.minValue;
    }

    private void OnMouseDown()
    {
      if (!this.IsActive || !this.IsCurrentState() || !this.CheckMouseInside())
        return;
      this.isGrabbed = true;
      System.Action onStartDrag = this.OnStartDrag;
      if (onStartDrag != null)
        onStartDrag();
    }

    private bool CheckMouseInside()
    {
      if (Utility.MouseInside(this.GlobalRect, this.IsLevelUI))
        return true;
      Rectangle globalRect = this.GlobalRect;
      Vector2 vector2 = new Vector2((float) globalRect.X + (float) globalRect.Width * this.GetPercentage(), (float) (globalRect.Y + globalRect.Height / 2));
      float num = (float) ((double) this.thumbSpritePos.Width * (((double) this.style.sliderThumbScale == 0.0 ? 1.0 : (double) this.style.sliderThumbScale) * 2.0) / 2.0);
      return (double) Vector2.Distance(vector2, this.LocalMousePos()) < (double) num;
    }

    private void OnMouseUp()
    {
      if (!this.isGrabbed)
        return;
      this.isGrabbed = false;
      Slider.SliderChangeHandler changeHandler = this.changeHandler;
      if (changeHandler != null)
        changeHandler(this.value, true);
    }

    private void GetValueFromMouse()
    {
      Vector2 vector2 = this.LocalMousePos();
      int num1 = this.maxValue - this.minValue;
      int num2 = this.rectangle.Width / num1;
      float num3 = (vector2.X - (float) this.GlobalRect.X + (float) (num2 / 2)) / (float) this.rectangle.Width;
      int num4 = this.value;
      this.value = MathHelper.Clamp((int) ((double) this.minValue + (double) num1 * (double) num3), this.minValue, this.maxValue);
      if (num4 == this.value)
        return;
      this.PlayInteractionSound(0.05f);
      Slider.SliderChangeHandler changeHandler = this.changeHandler;
      if (changeHandler != null)
        changeHandler(this.value);
    }

    private void OnMouseScroll(float amount)
    {
      if (!this.IsActive || !this.IsCurrentState() || !Utility.MouseInside(this.GlobalRect, this.IsLevelUI))
        return;
      if ((double) amount > 0.0)
        ++this.value;
      else if ((double) amount < 0.0)
        --this.value;
      this.SetValue(MathHelper.Clamp(this.value, this.minValue, this.maxValue));
      this.PlayInteractionSound(0.05f);
      Input.CancelCurrentAction = true;
    }

    public delegate void SliderChangeHandler(int v, bool final = false);
  }
}

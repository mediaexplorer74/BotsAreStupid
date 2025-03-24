// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.MovableFlexbox
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class MovableFlexbox : Flexbox
  {
    private bool isDragged;
    private Vector2 dragOffset;
    private Rectangle constraints;
    private int margin;

    public MovableFlexbox(Rectangle rectangle, Styling? style, Rectangle? constraints = null, int margin = 0)
      : base(rectangle, style)
    {
      Input.RegisterMouse(InputEvent.OnDown, new InputAction.InputHandler(this.OnMouseDown));
      Input.RegisterMouse(InputEvent.OnUp, new InputAction.InputHandler(this.OnMouseUp));
      this.constraints = constraints.HasValue ? constraints.Value : Rectangle.Empty;
      this.margin = margin;
    }

    public override void SetActive(bool active)
    {
      if (active)
        this.CheckConstraints();
      base.SetActive(active);
    }

    public override void Update(float delta)
    {
      if (this.IsActive && this.isDragged)
      {
        this.SetPosition(this.LocalMousePos() + this.dragOffset);
        this.CheckConstraints();
      }
      base.Update(delta);
    }

    private void OnMouseDown()
    {
      if (!this.IsActive)
        return;
      Vector2 pos = this.LocalMousePos();
      if (!Utility.PointInside(pos, this.GlobalRect))
        return;
      List<UIElement> childrenOfType = UIElement.FindChildrenOfType<Slider>((UIElement) this);
      if (childrenOfType.Count > 0)
      {
        foreach (UIElement uiElement in childrenOfType)
        {
          if (Utility.PointInside(pos, uiElement.GlobalRect))
            return;
        }
      }
      this.isDragged = true;
      this.dragOffset = new Vector2((float) this.rectangle.X, (float) this.rectangle.Y) - pos;
    }

    private void OnMouseUp()
    {
      if (!this.isDragged)
        return;
      this.isDragged = false;
    }

    private void CheckConstraints()
    {
      if (!(this.constraints != Rectangle.Empty))
        return;
      this.rectangle = Utility.ClampToRectangle(this.rectangle, this.constraints, this.margin);
    }
  }
}

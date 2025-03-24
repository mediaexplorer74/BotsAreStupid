// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.MovableElement
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class MovableElement : UIElement
  {
    public static MovableElement CurrentMovedObject;
    protected MovableElement.CanInteractCheck CanInteract;
    private bool isBeingMoved;
    private Vector2 dragOffset;
    private bool isDragStarting;
    private float dragDelay;
    private float dragDelayMax;
    private bool xLocked;
    private bool yLocked;
    private const int baseHeight = 0;
    private const int draggedHeight = 15;
    private const float heightChangeSpeed = 300f;
    private float currentHeight = 0.0f;
    private Vector2 defaultPos;
    private Vector2 lastTargetPos;
    private InputAction mouseDownAction;
    private InputAction mouseUpAction;

    public bool IsBeingMoved
    {
      get => this.isBeingMoved;
      private set
      {
        this.isBeingMoved = value;
        MovableElement.CurrentMovedObject = value ? this : (MovableElement) null;
      }
    }

    public event MovableElement.ActionHandler OnDrop;

    public event MovableElement.ActionHandler OnMove;

    public MovableElement(
      Rectangle rectangle,
      Styling? style,
      float dragDelay = 0.0f,
      bool xLocked = false,
      bool yLocked = false)
      : base(rectangle, style)
    {
      this.dragDelayMax = dragDelay;
      this.xLocked = xLocked;
      this.yLocked = yLocked;
      this.mouseDownAction = Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() => this.OnMouseDown()));
      this.mouseUpAction = Input.RegisterMouse(InputEvent.OnUp, new InputAction.InputHandler(this.OnMouseUp));
    }

    public override void Destroy(bool first = true)
    {
      base.Destroy(first);
      if (MovableElement.CurrentMovedObject == this)
        MovableElement.CurrentMovedObject = (MovableElement) null;
      Input.UnRegisterMouse(this.mouseDownAction);
      Input.UnRegisterMouse(this.mouseUpAction);
    }

    public void SetCanInteractCheck(MovableElement.CanInteractCheck canInteract)
    {
      this.CanInteract = canInteract;
    }

    public override void Update(float delta)
    {
      if (this.IsActive)
      {
        if (this.isDragStarting)
        {
          this.dragDelay -= delta;
          Vector2 pos = this.LocalMousePos();
          if ((double) this.dragDelay <= 0.0)
          {
            this.isDragStarting = false;
            this.dragOffset = new Vector2((float) this.rectangle.X, (float) this.rectangle.Y) - pos;
            this.DrawOnTop = true;
            this.IsBeingMoved = true;
          }
          else
          {
            MovableElement.CanInteractCheck canInteract = this.CanInteract;
            if ((canInteract != null ? (canInteract() ? 1 : 0) : 1) == 0 || !Utility.PointInside(pos, this.GlobalRect))
              this.isDragStarting = false;
          }
        }
        else if (this.IsBeingMoved)
        {
          Vector2 position1 = this.LocalMousePos() + this.dragOffset - new Vector2(this.currentHeight, this.currentHeight);
          if (this.xLocked)
            position1.X = (float) this.rectangle.X;
          if (this.yLocked)
            position1.Y = (float) this.rectangle.Y;
          if (position1 != this.lastTargetPos)
          {
            this.SetPosition(position1);
            Rectangle globalRect = this.GlobalRect;
            Vector2 position2 = new Vector2((float) globalRect.X, (float) globalRect.Y);
            MovableElement.ActionHandler onMove = this.OnMove;
            if (onMove != null)
              onMove(position2);
          }
          this.lastTargetPos = position1;
        }
      }
      int num1 = this.IsBeingMoved ? 15 : 0;
      if ((double) this.currentHeight != (double) num1)
      {
        int num2 = Math.Sign((float) num1 - this.currentHeight);
        this.currentHeight += (float) num2 * 300f * delta;
        if (num2 > 0 && (double) this.currentHeight > (double) num1 || num2 < 0 && (double) this.currentHeight < (double) num1)
          this.currentHeight = (float) num1;
        this.style.shadowSize = (int) this.currentHeight;
      }
      base.Update(delta);
    }

    public void SetDefaultPosition(Vector2 defaultPos, bool keepPosition)
    {
      this.defaultPos = defaultPos;
      if (keepPosition)
        return;
      this.SetPosition(defaultPos);
    }

    public void StartDrag()
    {
      this.isDragStarting = true;
      this.dragDelay = this.dragDelayMax;
    }

    public void CenterOnPosition(Vector2 pos)
    {
      this.SetPosition(pos - new Vector2((float) this.Width, (float) this.Height) / 2f + Vector2.One * 15f);
    }

    protected virtual bool OnMouseDown()
    {
      int num;
      if (this.IsActive && this.IsCurrentState())
      {
        MovableElement.CanInteractCheck canInteract = this.CanInteract;
        num = (canInteract != null ? (canInteract() ? 1 : 0) : 1) == 0 ? 1 : 0;
      }
      else
        num = 1;
      if (num != 0 || !this.MouseOnRectangle())
        return false;
      bool flag = false;
      foreach (UIElement uiElement in UIElement.FindChildrenOfType<Button>((UIElement) this))
      {
        if (uiElement.MouseOnRectangle())
          flag = true;
      }
      if (!flag)
        this.StartDrag();
      return true;
    }

    protected virtual void OnMouseUp()
    {
      if (this.isDragStarting)
      {
        this.isDragStarting = false;
      }
      else
      {
        if (!this.IsBeingMoved)
          return;
        Rectangle globalRect = this.GlobalRect;
        Vector2 position = new Vector2((float) globalRect.X, (float) globalRect.Y);
        MovableElement.ActionHandler onDrop = this.OnDrop;
        if (onDrop != null)
          onDrop(position);
        this.IsBeingMoved = false;
        this.SetPosition(this.defaultPos);
        this.DrawOnTop = false;
      }
    }

    public delegate bool CanInteractCheck(bool basic = false);

    public delegate void ActionHandler(Vector2 position);
  }
}

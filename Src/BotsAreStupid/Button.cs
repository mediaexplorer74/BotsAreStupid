// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Button
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class Button : UIElement
  {
    protected Button.OnClick onClick;
    protected Button.OnClick onRightClick;
    private Button.OnClickString onClickString;
    private bool isClickable = true;
    private bool isPressed;
    private bool isRightPressed;
    private bool triggerOnDown;
    private bool triggerWhileDown;
    private float pressTime = 0.0f;
    private float longPressDelay = 0.2f;
    private float triggerCooldown = 0.0f;
    private float triggerCooldownMax = 0.05f;
    private List<(InputAction, MouseButton)> inputActions = new List<(InputAction, MouseButton)>();

    public Button(
      Rectangle rectangle,
      Button.OnClick onClick,
      Styling style,
      bool triggerOnDown = false,
      bool triggerWhileDown = false)
      : base(rectangle, new Styling?(style))
    {
      this.onClick = onClick;
      this.triggerOnDown = triggerOnDown;
      this.triggerWhileDown = triggerWhileDown;
      this.inputActions.Add((Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() => this.OnMouseDown())), MouseButton.Left));
      this.inputActions.Add((Input.RegisterMouse(InputEvent.OnUp, (InputAction.InputHandler) (() => this.OnMouseUp())), MouseButton.Left));
      this.ChangeStyle(new Styling()
      {
        hoverCursor = new bool?(true)
      });
    }

    public Button(
      Rectangle rectangle,
      Button.OnClick onClick,
      Button.OnClick onRightClick,
      Styling style,
      bool triggerOnDown = false,
      bool triggerWhileDown = false)
      : base(rectangle, new Styling?(style))
    {
      this.onClick = onClick;
      this.onRightClick = onRightClick;
      this.triggerOnDown = triggerOnDown;
      this.triggerWhileDown = triggerWhileDown;
      this.inputActions.Add((Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() => this.OnMouseDown())), MouseButton.Left));
      this.inputActions.Add((Input.RegisterMouse(InputEvent.OnUp, (InputAction.InputHandler) (() => this.OnMouseUp())), MouseButton.Left));
      this.inputActions.Add((Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() => this.OnMouseDown(false)), button: MouseButton.Right), MouseButton.Right));
      this.inputActions.Add((Input.RegisterMouse(InputEvent.OnUp, (InputAction.InputHandler) (() => this.OnMouseUp(false)), button: MouseButton.Right), MouseButton.Right));
      this.ChangeStyle(new Styling()
      {
        hoverCursor = new bool?(true)
      });
    }

    public Button(
      Rectangle rectangle,
      Button.OnClickString onClickString,
      Styling style,
      bool triggerOnDown = false,
      bool triggerWhileDown = false)
      : base(rectangle, new Styling?(style))
    {
      this.onClickString = onClickString;
      this.triggerOnDown = triggerOnDown;
      this.triggerWhileDown = triggerWhileDown;
      this.inputActions.Add((Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() => this.OnMouseDown())), MouseButton.Left));
      this.inputActions.Add((Input.RegisterMouse(InputEvent.OnUp, (InputAction.InputHandler) (() => this.OnMouseUp())), MouseButton.Left));
      this.ChangeStyle(new Styling()
      {
        hoverCursor = new bool?(true)
      });
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (!this.isPressed)
        return;
      if (this.IsHovered)
      {
        if (this.triggerWhileDown)
        {
          this.pressTime += deltaTime;
          this.triggerCooldown += deltaTime;
          if ((double) this.pressTime >= (double) this.longPressDelay && (double) this.triggerCooldown > (double) this.triggerCooldownMax)
          {
            this.triggerCooldown = 0.0f;
            this.Trigger();
          }
        }
        if (this.style.textColorEffects)
          this.style.textColor = this.style.clickColor;
        else
          this.style.color = this.style.clickColor;
      }
      else
        this.isPressed = false;
    }

    public void SetClickable(bool value)
    {
      this.isClickable = value;
      if (value)
        return;
      this.IsHovered = false;
      this.isPressed = false;
    }

    protected override bool CheckHover() => this.isClickable && base.CheckHover();

    private void OnMouseDown(bool left = true)
    {
      this.CheckHover();
      if (!this.IsActive || !this.IsCurrentState() || !this.IsHovered)
        return;
      if (left)
        this.isPressed = true;
      else
        this.isRightPressed = true;
      SoundManager.Play("click", 0.5f);
      this.pressTime = 0.0f;
      if (this.style.textColorEffects)
        this.style.textColor = this.style.clickColor;
      else
        this.style.color = this.style.clickColor;
      if (this.triggerOnDown)
        this.Trigger(left);
      Input.CancelCurrentAction = true;
    }

    private void OnMouseUp(bool left = true)
    {
      if ((!left || !this.isPressed) && (left || !this.isRightPressed))
        return;
      if (!this.triggerOnDown)
        this.Trigger(left);
      if (left)
        this.isPressed = false;
      else
        this.isRightPressed = false;
      if (this.style.textColorEffects)
        this.style.textColor = this.style.defaultTextColor;
      else
        this.style.color = this.style.defaultColor;
    }

    public virtual void Trigger(bool left = true)
    {
      if (this.onClick != null)
      {
        if (left)
          this.onClick();
        else
          this.onRightClick();
      }
      if (this.onClickString == null)
        return;
      this.onClickString(string.IsNullOrEmpty(this.style.value) ? this.style.text : this.style.value);
    }

    public override void Destroy(bool first = true)
    {
      base.Destroy(first);
      foreach ((InputAction, MouseButton) inputAction in this.inputActions)
        Input.UnRegisterMouse(inputAction.Item1, inputAction.Item2);
    }

    public delegate void OnClick();

    public delegate void OnClickString(string s);
  }
}

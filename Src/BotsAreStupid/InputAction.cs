// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.InputAction
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework.Input;

#nullable disable
namespace BotsAreStupid
{
  internal class InputAction
  {
    private InputEvent inputEvent;
    private GameState gameState;
    private float currentCooldown = 0.0f;
    private float cooldownTime;
    private float currentLongPressDelay;
    private float longPressDelay;
    private bool ignoreTextLock;
    private bool isPressed;
    private KeyBind? requireDown;
    private Keys? key;
    private const float multiClickTime = 0.2f;
    private float multiClickTimeLeft;
    private int multiClickCount = 0;

    public InputAction.InputHandler handler { get; private set; }

    public InputAction(
      InputEvent inputEvent,
      InputAction.InputHandler handler,
      GameState gameState = GameState.Any,
      Keys? key = null,
      float cooldownTime = 0.2f,
      float longPressDelay = 0.3f,
      bool ignoreTextLock = false,
      KeyBind? requireDown = null)
    {
      this.inputEvent = inputEvent;
      this.handler = handler;
      this.cooldownTime = cooldownTime;
      this.longPressDelay = longPressDelay;
      this.currentLongPressDelay = longPressDelay;
      this.ignoreTextLock = ignoreTextLock;
      this.key = key;
      this.gameState = gameState;
      this.requireDown = requireDown;
    }

    public void Update(float deltaTime)
    {
      if (this.isPressed && (double) this.currentLongPressDelay > 0.0)
        this.currentLongPressDelay -= deltaTime;
      if ((double) this.currentCooldown > 0.0)
        this.currentCooldown -= deltaTime;
      if ((double) this.multiClickTimeLeft <= 0.0)
        return;
      this.multiClickTimeLeft -= deltaTime;
      if ((double) this.multiClickTimeLeft <= 0.0)
        this.multiClickCount = 0;
    }

    public void KeyDown()
    {
      if (!StateManager.IsState(this.gameState) || this.inputEvent != InputEvent.Down && this.inputEvent != InputEvent.OnDownAndDown || (double) this.currentLongPressDelay > 0.0 || (double) this.currentCooldown > 0.0)
        return;
      if (this.CanInvoke())
      {
        InputAction.InputHandler handler = this.handler;
        if (handler != null)
          handler();
      }
      this.currentCooldown = this.cooldownTime;
    }

    public void OnKeyDown()
    {
      if (!StateManager.IsState(this.gameState))
        return;
      this.isPressed = true;
      if (this.inputEvent == InputEvent.OnDoubleDown)
      {
        if ((double) this.multiClickTimeLeft > 0.0)
        {
          ++this.multiClickCount;
          if (this.inputEvent == InputEvent.OnDoubleDown && this.multiClickCount == 1 && this.CanInvoke())
          {
            InputAction.InputHandler handler = this.handler;
            if (handler != null)
              handler();
          }
        }
        this.multiClickTimeLeft = 0.2f;
      }
      else if ((this.inputEvent == InputEvent.OnDown || this.inputEvent == InputEvent.OnDownAndDown) && this.CanInvoke())
      {
        InputAction.InputHandler handler = this.handler;
        if (handler != null)
          handler();
      }
    }

    public void OnKeyUp()
    {
      this.isPressed = false;
      this.currentCooldown = 0.0f;
      this.currentLongPressDelay = this.longPressDelay;
      if (!StateManager.IsState(this.gameState) || this.inputEvent != InputEvent.OnUp || !this.CanInvoke())
        return;
      InputAction.InputHandler handler = this.handler;
      if (handler != null)
        handler();
    }

    private bool CanInvoke()
    {
      return (this.ignoreTextLock || !UIElement.IsTypeActive(typeof (Textfield)) && (!TextEditor.Instance.IsActive || !TextEditor.Instance.IsInteractable)) && (!this.requireDown.HasValue || BotsAreStupid.Input.IsDown(this.requireDown.Value));
    }

    public delegate void InputHandler();
  }
}

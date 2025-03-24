// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.KeyData
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class KeyData
  {
    private bool isPressed;
    private List<InputAction> actions = new List<InputAction>();
    private bool simulatedDown;

    public Keys Key { get; private set; }

    public KeyData(Keys key) => this.Key = key;

    public void Update(float deltaTime)
    {
      this.ForEachAction((KeyData.ForEachHandler) (a => a.Update(deltaTime)));
      if (this.simulatedDown || Keyboard.GetState().IsKeyDown(this.Key))
      {
        if (!this.isPressed)
          this.OnKeyDown();
        else
          this.KeyDown();
      }
      else if (this.isPressed)
        this.OnKeyUp();
    }

    public void Register(
      InputEvent inputEvent,
      InputAction.InputHandler handler,
      GameState gameState,
      float cooldownTime,
      float longPressDelay,
      bool ignoreTextLock,
      KeyBind? requireDown)
    {
      this.actions.Add(new InputAction(inputEvent, handler, gameState, new Keys?(this.Key), cooldownTime, longPressDelay, ignoreTextLock, requireDown));
    }

    public void UnRegister(InputAction.InputHandler handler)
    {
      this.ForEachAction((KeyData.ForEachHandler) (a =>
      {
        if (!(a.handler == handler))
          return;
        this.actions.Remove(a);
      }), false);
    }

    public void SimulateDown(bool down) => this.simulatedDown = down;

    private void KeyDown() => this.ForEachAction((KeyData.ForEachHandler) (a => a.KeyDown()));

    private void OnKeyDown()
    {
      this.isPressed = true;
      this.ForEachAction((KeyData.ForEachHandler) (a => a.OnKeyDown()));
    }

    private void OnKeyUp()
    {
      this.isPressed = false;
      this.ForEachAction((KeyData.ForEachHandler) (a => a.OnKeyUp()));
    }

    private void ForEachAction(KeyData.ForEachHandler handler, bool canBeCanceled = true)
    {
      if (canBeCanceled)
        BotsAreStupid.Input.CancelCurrentAction = false;
      for (int index = this.actions.Count - 1; index >= 0 && (!canBeCanceled || !BotsAreStupid.Input.CancelCurrentAction); --index)
      {
        InputAction action = this.actions[index];
        if (action != null)
          handler(action);
      }
    }

    private delegate void ForEachHandler(InputAction action);
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Input
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#nullable enable
namespace BotsAreStupid
{
  internal class Input
  {
    private 
    #nullable disable
    KeyData[] keyData;
    private List<InputAction>[] mouseActions = new List<InputAction>[3]
    {
      new List<InputAction>(),
      new List<InputAction>(),
      new List<InputAction>()
    };
    private List<BotsAreStupid.Input.MouseScrollHandler> mouseScrollHandlers = new List<BotsAreStupid.Input.MouseScrollHandler>();
    private List<BotsAreStupid.Input.MouseScrollHandler> horizontalMouseScrollHandlers = new List<BotsAreStupid.Input.MouseScrollHandler>();
    private float scrollValue;
    private float horizontalScrollValue;
    private bool[] isMouseDown = new bool[3];

    public static BotsAreStupid.Input Instance { get; private set; }

    private event BotsAreStupid.Input.TextInputHandler OnTextInput;

    public void CallOnTextInput(char c)
    {
      BotsAreStupid.Input.TextInputHandler onTextInput = this.OnTextInput;
      if (onTextInput == null)
        return;
      onTextInput(c);
    }

    public static bool[] MouseJustDowned { get; private set; } = new bool[3];

    public static bool CancelCurrentAction { get; set; }

    public Input()
    {
      BotsAreStupid.Input.Instance = this;
      Game1.Instance.Window.TextInput += (EventHandler<TextInputEventArgs>) ((sender, args) => this.OnTextInput(args.Character));
      Keys[] values = (Keys[]) Enum.GetValues(typeof (Keys));
      this.keyData = new KeyData[values.Length];
      for (int index = 0; index < values.Length; ++index)
      {
        KeyData keyData = new KeyData(values[index]);
        this.keyData[index] = keyData;
      }
    }

    public static void Register(
      KeyBind action,
      InputAction.InputHandler handler,
      GameState gameState = GameState.Any,
      float cooldown = 0.2f,
      float longPressDelay = 0.3f,
      bool ignoreTextLock = false,
      KeyBind? requireDown = null)
    {
      BotsAreStupid.Input.Instance.GetKeyDataByKey((Keys) action).Register(InputEvent.OnDownAndDown, handler, gameState, cooldown, longPressDelay, ignoreTextLock, requireDown);
    }

    public static void RegisterOnDown(
      KeyBind action,
      InputAction.InputHandler handler,
      GameState gameState = GameState.Any,
      float cooldown = 0.2f,
      float longPressDelay = 0.3f,
      bool ignoreTextLock = false,
      KeyBind? requireDown = null)
    {
      BotsAreStupid.Input.Instance.GetKeyDataByKey((Keys) action).Register(InputEvent.OnDown, handler, gameState, cooldown, longPressDelay, ignoreTextLock, requireDown);
    }

    public static void RegisterOnUp(
      KeyBind action,
      InputAction.InputHandler handler,
      GameState gameState = GameState.Any,
      float cooldown = 0.2f,
      float longPressDelay = 0.3f,
      bool ignoreTextLock = false,
      KeyBind? requireDown = null)
    {
      BotsAreStupid.Input.Instance.GetKeyDataByKey((Keys) action).Register(InputEvent.OnUp, handler, gameState, cooldown, longPressDelay, ignoreTextLock, requireDown);
    }

    public static void RegisterTextHandler(
      BotsAreStupid.Input.TextInputHandler handler,
      string allowedChars,
      GameState gameState = GameState.Any)
    {
      CharacterSet charSet = new CharacterSet(allowedChars);
      BotsAreStupid.Input.Instance.OnTextInput += (BotsAreStupid.Input.TextInputHandler) (c =>
      {
        if (!StateManager.IsState(gameState) || !charSet.Allows(c))
          return;
        handler(c);
      });
    }

    public static InputAction RegisterMouse(
      InputEvent inputEvent,
      InputAction.InputHandler handler,
      GameState gameState = GameState.Any,
      MouseButton button = MouseButton.Left)
    {
      InputAction inputAction = new InputAction(inputEvent, handler, gameState, ignoreTextLock: true);
      BotsAreStupid.Input.Instance.mouseActions[(int) button].Add(inputAction);
      return inputAction;
    }

    public static void RegisterOnScroll(
      BotsAreStupid.Input.MouseScrollHandler handler,
      bool horizontal = false,
      bool priority = false)
    {
      List<BotsAreStupid.Input.MouseScrollHandler> mouseScrollHandlerList = horizontal ? BotsAreStupid.Input.Instance.horizontalMouseScrollHandlers : BotsAreStupid.Input.Instance.mouseScrollHandlers;
      if (priority)
        mouseScrollHandlerList.Insert(0, handler);
      else
        mouseScrollHandlerList.Add(handler);
    }

    public static void UnRegister(KeyBind action, InputAction.InputHandler handler)
    {
      BotsAreStupid.Input.Instance.GetKeyDataByKey((Keys) action).UnRegister(handler);
    }

    public static void UnRegisterMouse(InputAction inputAction, MouseButton button = MouseButton.Left)
    {
      List<InputAction> mouseAction = BotsAreStupid.Input.Instance.mouseActions[(int) button];
      if (!mouseAction.Contains(inputAction))
        return;
      mouseAction.Remove(inputAction);
    }

    public static void UnRegisterOnScroll(BotsAreStupid.Input.MouseScrollHandler handler, bool horizontal = false)
    {
      if (horizontal)
        BotsAreStupid.Input.Instance.horizontalMouseScrollHandlers.Remove(handler);
      else
        BotsAreStupid.Input.Instance.mouseScrollHandlers.Remove(handler);
    }

    public static bool IsDown(KeyBind action) => BotsAreStupid.Input.Instance.IsPressed(action);

    public void Update(float deltaTime)
    {
      foreach (KeyData keyData in this.keyData)
        keyData.Update(deltaTime);
      foreach (List<InputAction> mouseAction in this.mouseActions)
      {
        for (int index = mouseAction.Count - 1; index >= 0; --index)
        {
          if (index < mouseAction.Count)
            mouseAction[index].Update(deltaTime);
        }
      }
      MouseState state = Mouse.GetState();
      Vector2? nullable = new Vector2?();
      for (int index = 0; index < BotsAreStupid.Input.MouseJustDowned.Length; ++index)
        BotsAreStupid.Input.MouseJustDowned[index] = false;
      List<InputAction> mouseAction1 = this.mouseActions[0];
      List<InputAction> mouseAction2 = this.mouseActions[1];
      List<InputAction> mouseAction3 = this.mouseActions[2];
      BotsAreStupid.Input.CancelCurrentAction = false;
      if (state.LeftButton == ButtonState.Pressed)
      {
        if (!this.isMouseDown[0])
        {
          for (int index = mouseAction1.Count - 1; index >= 0 && !BotsAreStupid.Input.CancelCurrentAction; --index)
          {
            if (index < mouseAction1.Count)
              mouseAction1[index].OnKeyDown();
          }
          this.isMouseDown[0] = true;
          BotsAreStupid.Input.MouseJustDowned[0] = true;
        }
      }
      else if (this.isMouseDown[0])
      {
        for (int index = mouseAction1.Count - 1; index >= 0 && !BotsAreStupid.Input.CancelCurrentAction; --index)
        {
          if (index < mouseAction1.Count)
            mouseAction1[index].OnKeyUp();
        }
        this.isMouseDown[0] = false;
      }
      BotsAreStupid.Input.CancelCurrentAction = false;
      if (state.RightButton == ButtonState.Pressed)
      {
        if (!this.isMouseDown[1])
        {
          for (int index = mouseAction2.Count - 1; index >= 0 && !BotsAreStupid.Input.CancelCurrentAction; --index)
          {
            if (index < mouseAction2.Count)
              mouseAction2[index].OnKeyDown();
          }
          this.isMouseDown[1] = true;
          BotsAreStupid.Input.MouseJustDowned[1] = true;
        }
      }
      else if (this.isMouseDown[1])
      {
        for (int index = mouseAction2.Count - 1; index >= 0; --index)
        {
          if (index < mouseAction2.Count)
            mouseAction2[index].OnKeyUp();
        }
        this.isMouseDown[1] = false;
      }
      BotsAreStupid.Input.CancelCurrentAction = false;
      if (state.MiddleButton == ButtonState.Pressed)
      {
        if (!this.isMouseDown[2])
        {
          for (int index = mouseAction3.Count - 1; index >= 0 && !BotsAreStupid.Input.CancelCurrentAction; --index)
          {
            if (index < mouseAction3.Count)
              mouseAction3[index].OnKeyDown();
          }
          this.isMouseDown[2] = true;
          BotsAreStupid.Input.MouseJustDowned[2] = true;
        }
      }
      else if (this.isMouseDown[2])
      {
        for (int index = mouseAction3.Count - 1; index >= 0; --index)
        {
          if (index < mouseAction3.Count)
            mouseAction3[index].OnKeyUp();
        }
        this.isMouseDown[2] = false;
      }
      float scroll = nullable.HasValue ? nullable.GetValueOrDefault().Y : (float) state.ScrollWheelValue - this.scrollValue;
      if ((double) scroll != 0.0)
      {
        BotsAreStupid.Input.CancelCurrentAction = false;
        for (int index = 0; index < this.mouseScrollHandlers.Count && !BotsAreStupid.Input.CancelCurrentAction; ++index)
        {
          BotsAreStupid.Input.MouseScrollHandler mouseScrollHandler = this.mouseScrollHandlers[index];
          if (mouseScrollHandler != null)
            mouseScrollHandler(scroll);
        }
      }
      this.scrollValue = (float) state.ScrollWheelValue;
      if ((nullable.HasValue ? (double) nullable.GetValueOrDefault().X : (double) state.HorizontalScrollWheelValue - (double) this.horizontalScrollValue) != 0.0)
      {
        BotsAreStupid.Input.CancelCurrentAction = false;
        for (int index = 0; index < this.horizontalMouseScrollHandlers.Count && !BotsAreStupid.Input.CancelCurrentAction; ++index)
        {
          BotsAreStupid.Input.MouseScrollHandler mouseScrollHandler = this.horizontalMouseScrollHandlers[index];
          if (mouseScrollHandler != null)
            mouseScrollHandler(scroll);
        }
      }
      this.horizontalScrollValue = (float) state.HorizontalScrollWheelValue;
    }

    public KeyData GetKeyDataByKey(Keys key)
    {
      foreach (KeyData keyDataByKey in this.keyData)
      {
        if (keyDataByKey.Key == key)
          return keyDataByKey;
      }
      return (KeyData) null;
    }

    private bool IsPressed(KeyBind action)
    {
      Keys key = (Keys) action;
      KeyboardState state = Keyboard.GetState();
      return key == Keys.LeftAlt && state.IsKeyDown(Keys.RightAlt) || key == Keys.LeftShift && state.IsKeyDown(Keys.RightShift) || key == Keys.LeftControl && state.IsKeyDown(Keys.RightControl) || state.IsKeyDown(key);
    }

    public delegate void TextInputHandler(char c);

    public delegate void MouseScrollHandler(float scroll);
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ContextMenu
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using BotsAreStupid.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal class ContextMenu : UIElement
  {
    private InputAction mouseAction;
    private Rectangle? targetRect;
    private Color targetColor;

    public static ContextMenu Instance { get; private set; }

    public UIElement OpenedBy { get; private set; }

    public ContextMenu(
      UIElement openingElement,
      Rectangle rect,
      Styling styling,
      Rectangle? targetRect,
      Color? targetColor = null)
      : base(rect, new Styling?(styling))
    {
      ContextMenu.Instance?.Close();
      ContextMenu.Instance = this;
      this.DrawOnTop = true;
      this.OpenedBy = openingElement;
      this.OpenedBy?.SetInteractable(false);
      this.targetRect = targetRect;
      this.targetColor = targetColor ?? new Color(Color.Black, 0.1f);
      StateManager.OnStateChange += new StateManager.StateChangeHandler(this.Close);
      this.mouseAction = Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() =>
      {
        if (!this.MouseOnRectangle())
          this.Close();
        else
          Input.CancelCurrentAction = true;
      }));
    }

    public event System.Action OnClose;

    public void Close(AbstractState from = null, AbstractState to = null)
    {
      System.Action onClose = this.OnClose;
      if (onClose != null)
        onClose();
      StateManager.OnStateChange -= new StateManager.StateChangeHandler(this.Close);
      Input.UnRegisterMouse(this.mouseAction);
      ContextMenu.Instance = (ContextMenu) null;
      this.Destroy();
      this.OpenedBy?.SetInteractable(true);
    }

    public event System.Action OnSelectCurrent;

    public void SelectCurrent()
    {
      System.Action onSelectCurrent = this.OnSelectCurrent;
      if (onSelectCurrent == null)
        return;
      onSelectCurrent();
    }

    protected override void DrawTexture(SpriteBatch spriteBatch)
    {
      base.DrawTexture(spriteBatch);
      if (!this.IsActive || !this.targetRect.HasValue)
        return;
      Utility.DrawRect(spriteBatch, this.targetRect.Value, this.targetColor);
    }
  }
}

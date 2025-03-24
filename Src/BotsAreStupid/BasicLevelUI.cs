// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.BasicLevelUI
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#nullable disable
namespace BotsAreStupid
{
  internal abstract class BasicLevelUI : UIElement
  {
    private bool isHoveringResize;
    private bool isResizing;
    private bool isHoveringRightResize;
    private bool isResizingRight;

    public BasicLevelUI(Rectangle rectangle, GameState drawInState)
      : base(new Rectangle(rectangle.X, rectangle.Y, 340, rectangle.Height))
    {
      BotsAreStupid.Input.RegisterMouse(InputEvent.OnDown, new InputAction.InputHandler(this.OnMouseDown), this.style.drawInState);
      BotsAreStupid.Input.RegisterMouse(InputEvent.OnUp, new InputAction.InputHandler(this.OnMouseUp), this.style.drawInState);
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (!this.IsActive || !this.IsCurrentState())
        return;
      if (this.isResizing)
      {
        if (MovableElement.CurrentMovedObject != null)
          this.isResizing = false;
        else if (UIElement.mouseHasMoved)
        {
          VarManager.SetInt("minleveluiwidth", (int) Utility.GetMousePos().X);
          Game1.Instance.UpdateMatrices();
        }
      }
      this.isHoveringResize = this.IsActive && this.MouseOnResizeRect();
      if (this.isResizingRight)
      {
        if (MovableElement.CurrentMovedObject != null)
          this.isResizingRight = false;
        else if (UIElement.mouseHasMoved)
        {
          Vector2 mousePos = Utility.GetMousePos();
          VarManager.SetInt("minsidebarwidth", (int) ((double) Game1.GetWindowSize().Item1 - (double) mousePos.X));
          Game1.Instance.UpdateMatrices();
        }
      }
      this.isHoveringRightResize = this.IsActive && this.MouseOnResizeRect(true);
      if (!this.isHoveringResize && !this.isHoveringRightResize)
        return;
      UIElement.SetCursorOverride(MouseCursor.SizeWE);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
      if (this.isHoveringResize)
      {
        Rectangle globalRect = this.GlobalRect;
        Vector2 start = new Vector2((float) (globalRect.X + globalRect.Width + this.style.borderWidth / 2), (float) globalRect.Y);
        Utility.DrawLine(spriteBatch, start, new Vector2(start.X, start.Y + (float) globalRect.Height), this.style.borderWidth, new Color?(Utility.ChangeColor(this.style.borderColor, -0.25f)), 0.0f, 0);
      }
      if (VarManager.GetInt("sidebarwidth") <= 0)
        return;
      int x = VarManager.GetInt("sidebarx");
      Rectangle globalRect1 = this.GlobalRect;
      Color color = this.isHoveringRightResize ? Utility.ChangeColor(this.style.borderColor, -0.25f) : this.style.borderColor;
      Utility.DrawLine(spriteBatch, new Vector2((float) x, (float) globalRect1.Y), new Vector2((float) x, (float) globalRect1.Height), this.style.borderWidth, new Color?(color), 0.0f, 0);
    }

    private void OnMouseDown()
    {
      if (this.isHoveringResize)
        this.isResizing = true;
      if (!this.isHoveringRightResize)
        return;
      this.isResizingRight = true;
    }

    private void OnMouseUp()
    {
      if (this.isResizing)
        this.isResizing = false;
      if (!this.isResizingRight)
        return;
      this.isResizingRight = false;
    }

    private bool MouseOnResizeRect(bool right = false)
    {
      if (!this.IsActive || !this.IsInteractable)
        return false;
      Rectangle globalRect = this.GlobalRect;
      return right ? Utility.MouseInside(new Rectangle(VarManager.GetInt("sidebarx") - this.style.borderWidth, globalRect.Y, this.style.borderWidth, globalRect.Height)) : Utility.MouseInside(new Rectangle(globalRect.X + globalRect.Width - this.style.borderWidth / 2, globalRect.Y, this.style.borderWidth, globalRect.Height));
    }
  }
}

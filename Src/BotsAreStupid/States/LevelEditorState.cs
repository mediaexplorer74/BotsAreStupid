// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.States.LevelEditorState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#nullable disable
namespace BotsAreStupid.States
{
  internal class LevelEditorState : AbstractLevelState
  {
    private bool isDraggingView;
    private Vector2 viewDragStart;
    private Vector2 lastMousePos;

    public override GameState Type => GameState.LevelEditor;

    protected override bool EnableSidebar => true;

    protected override bool CanMouseZoom => true;

    protected override bool FixedCamPosition => true;

    protected override void Initialize()
    {
      base.Initialize();
      BotsAreStupid.Input.RegisterMouse(InputEvent.OnDown, new InputAction.InputHandler(this.HandleMiddleMouseDown), GameState.LevelEditor, MouseButton.Middle);
      BotsAreStupid.Input.RegisterMouse(InputEvent.OnUp, new InputAction.InputHandler(this.HandleMiddleMouseUp), GameState.LevelEditor, MouseButton.Middle);
    }

    public override void Enter(StateTransition transition)
    {
      base.Enter(transition);
      if (transition.PreviousState.Type == GameState.MainMenu)
        LevelManager.Load(LevelEditor.Instance.HasAutoSaved ? "AutoSave" : "Editor");
      else if (transition.NextPlayerName != null)
        LevelManager.Load(transition.NextPlayerName);
      else
        LevelManager.Load();
      LevelEditor.Instance.ClearHistory();
      PopupMenu instance = PopupMenu.Instance;
      AbstractState navigateBackState = this.NavigateBackState;
      string text = (navigateBackState != null ? (navigateBackState.Type == GameState.LevelSelection ? 1 : 0) : 0) != 0 ? "Back to Levels" : "Back to Menu";
      instance.SetExitButtonText(text);
    }

    protected override void UpdateNavigateBackState(AbstractState previousState)
    {
      if (previousState is AbstractLevelWithTextState)
        return;
      this.NavigateBackState = previousState;
    }

    public override void UpdateMatrices(float windowWidth, float windowHeight)
    {
      base.UpdateMatrices(windowWidth, windowHeight);
      LevelEditor.Instance.SetSize(VarManager.GetInt("leveluiwidth"), VarManager.GetInt("leveluiheight"), true);
      LevelEditor.Instance.UpdateSidebar(VarManager.GetInt("sidebarx"), VarManager.GetInt("sidebarwidth"), VarManager.GetInt("leveluiheight"));
    }

    public override void DrawLevelUI(SpriteBatch spriteBatch)
    {
      base.DrawLevelUI(spriteBatch);
      LevelEditor.Instance.DrawLevelUI(spriteBatch);
    }

    public override void DrawTransformedLevelUI(SpriteBatch spriteBatch)
    {
      base.DrawTransformedLevelUI(spriteBatch);
      LevelEditor.Instance.DrawTransformedLevelUI(spriteBatch);
    }

    public override void Update(float deltaTime)
    {
      if (this.isDraggingView)
      {
        Vector2 mousePos = Utility.GetMousePos(inTransformedLevelSpace: true);
        if (mousePos != this.lastMousePos)
        {
          Vector2 vector2 = this.viewDragStart - mousePos;
          VarManager.SetInt("camposx", (int) vector2.X, addToValue: true);
          VarManager.SetInt("camposy", (int) vector2.Y, addToValue: true);
        }
        this.lastMousePos = mousePos;
        UIElement.SetCursorOverride(MouseCursor.SizeAll);
      }
      base.Update(deltaTime);
    }

    private void HandleMiddleMouseDown()
    {
      this.viewDragStart = Utility.GetMousePos(inTransformedLevelSpace: true);
      this.isDraggingView = true;
    }

    private void HandleMiddleMouseUp() => this.isDraggingView = false;
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.States.AbstractLevelWithTextState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended;
//using MonoGame.Extended.BitmapFonts;

#nullable disable
namespace BotsAreStupid.States
{
  internal abstract class AbstractLevelWithTextState : AbstractLevelState
  {
    private InputAction onMouseDownAction;
    private InputAction onMouseUpAction;
    private Vector2? rulerStartPosition;
    private Vector2? rulerEndPosition;

    protected override bool CanMouseZoom => true;

    public override void Enter(StateTransition transition)
    {
      base.Enter(transition);
      if (string.IsNullOrEmpty(transition.NextLevelName))
        LevelManager.ReloadCurrent();
      TextEditor.Instance.UpdateView();
      TextEditor.Instance.UpdateAvailableCommands();
      PopupMenu.Instance.SetExitButtonText("Back to Levels");
      this.onMouseDownAction = Input.RegisterMouse(InputEvent.OnDown, new InputAction.InputHandler(this.OnMouseDown), GameState.InLevel_AnyWithText);
      this.onMouseUpAction = Input.RegisterMouse(InputEvent.OnUp, new InputAction.InputHandler(this.OnMouseUp), GameState.InLevel_AnyWithText);
      this.rulerStartPosition = new Vector2?();
    }

    public override void Exit(bool closePopupMenu = true)
    {
      base.Exit(closePopupMenu);
      Input.UnRegisterMouse(this.onMouseDownAction);
      Input.UnRegisterMouse(this.onMouseUpAction);
    }

    private void OnMouseDown()
    {
      if (PopupMenu.Instance.IsActive || UIElement.IsAnyHovered || !VarManager.GetBool("rulerenabled") || !Utility.MouseInside(this.LevelRect, inTransformedLevelSpace: true))
        return;
      this.rulerStartPosition = new Vector2?(Utility.GetMousePos(inTransformedLevelSpace: true));
      this.rulerEndPosition = new Vector2?();
    }

    private void OnMouseUp()
    {
      if (!PopupMenu.Instance.IsActive && !UIElement.IsAnyHovered && VarManager.GetBool("permanentruler"))
        this.rulerEndPosition = new Vector2?(Utility.GetMousePos(inTransformedLevelSpace: true));
      else
        this.rulerStartPosition = new Vector2?();
    }

    public override void DrawTransformedLevelUI(SpriteBatch spriteBatch)
    {
      this.Draw(spriteBatch);
      Player hoveredPlayer = (Player) null;
      string playerName = (string) null;
      SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation =>
      {
        if (simulation.Player == null || !Utility.MouseInside(simulation.Player.Rectangle, inTransformedLevelSpace: true))
          return;
        hoveredPlayer = simulation.Player;
        playerName = simulation.PlayerName;
      }));
      if (hoveredPlayer != null)
        Utility.DrawText(spriteBatch, TextureManager.GetFont("megaMan2Small"), playerName ?? "Unknown", new Microsoft.Xna.Framework.Rectangle((int) ((double) hoveredPlayer.X + (double) (hoveredPlayer.Width / 2)), (int) ((double) hoveredPlayer.Y + (double) hoveredPlayer.Height + 15.0), 0, 0), ColorManager.GetColor("white"), layerDepth: LayerDepths.PlayerName);
      if (!this.rulerStartPosition.HasValue)
        return;
      Vector2 vector2_1 = this.rulerStartPosition.Value;
      Microsoft.Xna.Framework.Color color = ColorManager.GetColor("white");
      Vector2 vector2_2 = this.rulerEndPosition ?? Utility.GetMousePos(inTransformedLevelSpace: true);
      Utility.DrawLine(spriteBatch, vector2_1, vector2_2, 2, new Microsoft.Xna.Framework.Color?(color), 1f, 0);
      Vector2 vector2_3 = vector2_2 - vector2_1;
      Vector2 vector = Vector2.Normalize(vector2_3);
      Vector2 dir = Utility.RotateVector(vector, -90f);
      Utility.DrawLine(spriteBatch, vector2_1 - dir * 7f - vector * 2f / 2f, dir, 12, 2, new Microsoft.Xna.Framework.Color?(color), 1f);
      Utility.DrawLine(spriteBatch, vector2_2 - dir * 7f - vector * 2f / 2f, dir, 12, 2, new Microsoft.Xna.Framework.Color?(color), 1f);
      string text = vector2_3.Length().ToString("0.00") + " u";
      BitmapFont font = TextureManager.GetFont("courier");
      /*SharpDX.Size2*/Vector2 size = default;//font.MeasureStringHalf(text);
      Microsoft.Xna.Framework.Rectangle alignedRectangle = 
                Utility.GetLineAlignedRectangle(vector2_1, vector2_2, 
                (Vector2) size, clampToRect: new Microsoft.Xna.Framework.Rectangle?(this.LevelRect));
      Utility.DrawText(spriteBatch, font, text, alignedRectangle, color, layerDepth: 1f);
    }

    public override void Reset(bool invokedByButton = false)
    {
      base.Reset(invokedByButton);
      if (SimulationManager.HasLockedFrame)
        TextEditor.Instance.OnPauseButtonPress();
      else
        TextEditor.Instance.Reset(invokedByButton);
    }

    public override void OnLevelFinish(Simulation simulation, string levelName)
    {
      base.OnLevelFinish(simulation, levelName);
      if (this.Type == GameState.InLevel_FromEditor || this.Type == GameState.InLevel_Watch)
        return;
      for (int index = 0; index < LevelManager.DefaultLevels.Length; ++index)
      {
        if (LevelManager.CurrentLevelName == LevelManager.DefaultLevels[index] && VarManager.GetInt("unlockedlevels") < index + 2)
        {
          VarManager.SetInt("unlockedlevels", index + 2);
          VarManager.SaveOptions();
        }
      }
    }

    protected override void UpdateNavigateBackState(AbstractState previousState)
    {
      base.UpdateNavigateBackState(previousState);
      if (!(previousState is AbstractLevelWithTextState))
        return;
      this.NavigateBackState = previousState.NavigateBackState;
    }

    public override void UpdateMatrices(float windowWidth, float windowHeight)
    {
      TextEditor.Instance?.SetRightBlueprints((double) windowWidth / (double) windowHeight < 0.60000002384185791);
      base.UpdateMatrices(windowWidth, windowHeight);
      TextEditor.Instance?.SetSize(VarManager.GetInt("leveluiwidth"), VarManager.GetInt("leveluiheight"), true);
    }
  }
}

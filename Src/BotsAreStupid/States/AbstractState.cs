// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.States.AbstractState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace BotsAreStupid.States
{
  internal abstract class AbstractState
  {
    public abstract GameState Type { get; }

    public AbstractState NavigateBackState { get; protected set; }

    public Matrix? LevelViewMatrix { get; protected set; } = new Matrix?();

    public Matrix? LevelCameraMatrix { get; protected set; } = new Matrix?();

    public virtual Rectangle LevelRect
    {
      get => new Rectangle(0, 0, Utility.VirtualWidth, Utility.VirtualHeight);
    }

    public UIElement MainUIElement { get; protected set; }

    protected virtual bool ScaleUI => true;

    public AbstractState() => this.Initialize();

    protected virtual void Initialize()
    {
    }

    public virtual void CreateUI(Rectangle fullRect)
    {
    }

    public virtual void Enter(StateTransition transition)
    {
      if (!transition.IsNavigatingBack)
        this.UpdateNavigateBackState(transition.PreviousState);
      VarManager.SetInt("camzoom", 0);
      (int, int) windowSize = Game1.GetWindowSize();
      this.UpdateMatrices((float) windowSize.Item1, (float) windowSize.Item2);
      if (!(this is AbstractLevelState) || !(transition.PreviousState is AbstractLevelState))
        VarManager.SetInt("timescale", 100);
      if (!(this is MainMenuState))
        VarManager.SetInt("overclock", 1);
      if (!string.IsNullOrEmpty(transition.NextLevelName))
        LevelManager.Load(transition.NextLevelName);
      SimulationManager.MainSimulation.SetPlayerName(transition.NextPlayerName ?? VarManager.GetString("username"));
    }

    public virtual void Exit(bool closePopupMenu = true)
    {
      LevelManager.Clear();
      if (!closePopupMenu)
        return;
      PopupMenu.Instance.SetActive(false);
    }

    public virtual void Update(float deltaTime)
    {
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
    }

    public virtual void DrawLevelUI(SpriteBatch spriteBatch)
    {
    }

    public virtual void DrawTransformedLevelUI(SpriteBatch spriteBatch)
    {
    }

    public virtual void Reset(bool invokedByButton = false)
    {
      SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation =>
      {
        if (simulation.IsIntro)
          return;
        simulation.Reset();
      }));
      SimulationManager.SetPaused(false);
    }

    public virtual void OnLevelFinish(Simulation simulation, string levelName)
    {
      if (simulation.Player.ScriptInterpreter.IsRandom && VarManager.GetBool("saverandom"))
        simulation.Player.ScriptInterpreter.SaveRandomInstructions();
      this.Reset();
      SoundManager.Play("success", 0.8f);
      if (!VarManager.GetBool("randomscores"))
        return;
      Utility.PlayRandomScore();
      SimulationManager.UpdateAutoGhost();
    }

    public virtual void OnPlayerDeath() => this.Reset();

    public virtual void UpdateMatrices(float windowWidth, float windowHeight)
    {
      Vector2 virtualSize = Utility.VirtualSize;
      float val1 = windowWidth / virtualSize.X;
      float val2 = windowHeight / virtualSize.Y;
      float num1 = val2;
      if (!this.ScaleUI)
      {
        float num2 = Math.Min(val1, val2);
        num1 = (double) num2 >= 0.550000011920929 ? ((double) num2 >= 0.699999988079071 ? ((double) num2 >= 0.85000002384185791 ? ((double) num2 >= 1.1000000238418579 ? ((double) num2 >= 1.5 ? ((double) num2 >= 2.0 ? 2f : 1.5f) : 1.2f) : 1f) : 0.75f) : 0.6f) : 0.45f;
      }
      Game1.Instance.WindowMatrix = Matrix.CreateScale(num1, num1, 1f);
      ScoreExplorer.Instance.SetPosition(new int?(0), new int?(0));
      ScoreExplorer.Instance.SetSize((int) ((double) windowWidth * 1.0 / (double) num1), (int) ((double) windowHeight * 1.0 / (double) num1), true);
      Vector2 vector2 = new Vector2((float) PopupMenu.Instance.Width, (float) PopupMenu.Instance.Height);
      PopupMenu.Instance.SetPosition(new Vector2(windowWidth, windowHeight) * 1f / num1 / 2f - vector2 / 2f);
      if (this.ScaleUI)
        this.MainUIElement?.SetSize((int) ((double) windowWidth * (1.0 / (double) num1)) + 1, (int) virtualSize.Y + 1, true);
      else
        this.MainUIElement?.SetSize((int) ((double) windowWidth * (1.0 / (double) num1)) + 1, (int) ((double) windowHeight * (1.0 / (double) num1)) + 1, true);
    }

    protected virtual void UpdateNavigateBackState(AbstractState previousState)
    {
      this.NavigateBackState = previousState;
    }
  }
}

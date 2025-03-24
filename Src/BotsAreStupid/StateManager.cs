// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.StateManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using BotsAreStupid.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class StateManager
  {
    public static StateManager Instance;
    private static AbstractState currentState;
    private static StateTransition currentTransition;
    private static Dictionary<GameState, AbstractState> states = new Dictionary<GameState, AbstractState>();

    public static Rectangle LevelRect
    {
      get
      {
        AbstractState currentState = StateManager.currentState;
        return currentState == null ? new Rectangle(0, 0, Utility.VirtualWidth, Utility.VirtualHeight) : currentState.LevelRect.Transform(StateManager.LevelViewMatrix);
      }
    }

    public static Rectangle UISpaceLevelRect
    {
      get => StateManager.LevelRect.Transform(Matrix.Invert(Game1.Instance.WindowMatrix));
    }

    public static Matrix LevelViewMatrix
    {
      get => (Matrix?) StateManager.currentState?.LevelViewMatrix ?? Game1.Instance.WindowMatrix;
    }

    public static Matrix LevelCameraMatrix
    {
      get => (Matrix?) StateManager.currentState?.LevelCameraMatrix ?? Matrix.Identity;
    }

    public static event StateManager.StateChangeHandler OnStateChange;

    public StateManager()
    {
      StateManager.Instance = this;
      this.RegisterState((AbstractState) new SplashScreenState());
      this.RegisterState((AbstractState) new MainMenuState());
      this.RegisterState((AbstractState) new LevelSelectionState());
      this.RegisterState((AbstractState) new LevelEditorState());
      this.RegisterState((AbstractState) new OptionsState());
      this.RegisterState((AbstractState) new CreditsState());
      this.RegisterState((AbstractState) new DefaultLevelState());
      this.RegisterState((AbstractState) new FromEditorLevelState());
      this.RegisterState((AbstractState) new WatchLevelState());
      Input.RegisterOnDown(KeyBind.Escape, new InputAction.InputHandler(this.OnEscape), cooldown: 1f, ignoreTextLock: true);
      SimulationManager.OnLevelFinished += (SimulationManager.LevelFinishedHandler) ((simulation, levelName) => StateManager.currentState?.OnLevelFinish(simulation, levelName));
      SimulationManager.OnPlayerDeath += new SimulationManager.PlayerDeathHandler(this.OnPlayerDeath);
      StateManager.TransitionTo(GameState.SplashScreen);
    }

    public void CreateUI(Rectangle fullRect)
    {
      foreach (KeyValuePair<GameState, AbstractState> state in StateManager.states)
        state.Value.CreateUI(fullRect);
    }

    private void RegisterState(AbstractState state)
    {
      //if (StateManager.states.TryAdd(state.Type, state))
      //  return;
      Console.WriteLine("[StateManager] Tried to register duplicate state of type: " + state.Type.ToString());
    }

    public static bool IsState(GameState state)
    {
      if (StateManager.Instance == null)
        return false;
      if (state == GameState.InLevel_Any && StateManager.currentState != null || state == GameState.InLevel_AnyWithText && StateManager.currentState != null)
        return StateManager.currentState.Type >= state;
      int num;
      if (state != GameState.Any)
      {
        AbstractState currentState = StateManager.currentState;
        num = currentState != null ? (currentState.Type == state ? 1 : 0) : 0;
      }
      else
        num = 1;
      return num != 0;
    }

    public void Update(float deltaTime)
    {
      if (StateManager.currentTransition != null)
      {
        bool crossedHalfTime;
        StateManager.currentTransition.Update(deltaTime, out crossedHalfTime);
        if (crossedHalfTime)
          this.NextState();
        if (StateManager.currentTransition.IsDone)
          StateManager.currentTransition = (StateTransition) null;
      }
      StateManager.currentState?.Update(deltaTime);
    }

    public void DrawLevel(SpriteBatch spriteBatch)
    {
      if (StateManager.currentState == null)
        return;
      spriteBatch.Begin(SpriteSortMode.FrontToBack, (BlendState) null, SamplerState.PointClamp, (DepthStencilState) null, RasterizerState.CullNone, (Effect) null, new Matrix?(StateManager.LevelCameraMatrix));
      EditorGrid.Instance.TryDraw(spriteBatch);
      SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation => simulation.Draw(spriteBatch)));
      spriteBatch.End();
    }

    public void DrawUI(SpriteBatch spriteBatch, SamplerState uiSamplerState)
    {
      RasterizerState rasterizerState = new RasterizerState()
      {
        MultiSampleAntiAlias = true
      };
      spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, uiSamplerState, (DepthStencilState) null, rasterizerState, (Effect) null, new Matrix?(Game1.Instance.WindowMatrix));
      if (!VarManager.HasBool("hideui") || !VarManager.GetBool("hideui"))
        UIElement.DrawAllUI(spriteBatch);
      StateManager.currentState?.Draw(spriteBatch);
      spriteBatch.End();
      spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, uiSamplerState, (DepthStencilState) null, rasterizerState, (Effect) null, new Matrix?(StateManager.LevelViewMatrix));
      StateManager.currentState?.DrawLevelUI(spriteBatch);
      spriteBatch.End();
      spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, uiSamplerState, (DepthStencilState) null, rasterizerState, (Effect) null, new Matrix?(StateManager.LevelViewMatrix));
      UIElement.DrawAllLevelUI(spriteBatch);
      spriteBatch.End();
      spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, uiSamplerState, (DepthStencilState) null, rasterizerState, (Effect) null, new Matrix?(StateManager.LevelCameraMatrix * StateManager.LevelViewMatrix));
      StateManager.currentState?.DrawTransformedLevelUI(spriteBatch);
      spriteBatch.End();
      if (StateManager.currentTransition == null)
        return;
      spriteBatch.Begin(SpriteSortMode.Deferred, (BlendState) null, uiSamplerState, (DepthStencilState) null, (RasterizerState) null, (Effect) null, new Matrix?(Game1.Instance.WindowMatrix));
      Vector2 virtualSize = Utility.VirtualSize;
      float alpha = (float) Math.Sin((double) StateManager.currentTransition.Progress * Math.PI);
      Utility.DrawRect(spriteBatch, new Rectangle(-10000, -10000, 20000, 20000), new Color(Color.Black, alpha));
      spriteBatch.End();
    }

    public void UpdateCurrentMatrices(float windowWidth, float windowHeight)
    {
      StateManager.currentState?.UpdateMatrices(windowWidth, windowHeight);
    }

    public static void TransitionTo(
      GameState state,
      string levelName = null,
      string playerName = null,
      bool closePopupMenu = true,
      bool isNavigatingBack = false,
      bool allowReEntry = false)
    {
      if (StateManager.Instance == null)
        return;
      bool flag = allowReEntry && StateManager.IsState(state);
      if (!flag && StateManager.IsState(state))
        return;
      StateManager.currentTransition = new StateTransition(flag ? StateManager.currentState.NavigateBackState : StateManager.currentState, state, levelName, playerName, closePopupMenu, isNavigatingBack);
    }

    public static void TransitionBack(bool closePopupMenu = true)
    {
      if (StateManager.Instance == null || StateManager.currentState?.NavigateBackState == null)
        return;
      StateManager.TransitionTo(StateManager.currentState.NavigateBackState.Type, closePopupMenu: closePopupMenu, isNavigatingBack: true);
    }

    private void NextState()
    {
      AbstractState abstractState;
      if (StateManager.currentTransition == null || !StateManager.states.TryGetValue(StateManager.currentTransition.NextState, out abstractState))
        return;
      ScriptManager.legacyScriptContainer?.Destroy();
      StateManager.currentState?.Exit(StateManager.currentTransition.ClosePopupMenu);
      StateManager.currentState = abstractState;
      StateManager.currentState.Enter(StateManager.currentTransition);
      StateManager.StateChangeHandler onStateChange = StateManager.OnStateChange;
      if (onStateChange == null)
        return;
      onStateChange(StateManager.currentTransition.PreviousState, StateManager.currentState);
    }

    public static void Reset(bool invokedByButton = false)
    {
      if (StateManager.Instance == null)
        return;
      StateManager.currentState?.Reset(invokedByButton);
      if (!StateManager.IsState(GameState.MainMenu))
        VarManager.SetInt("overclock", 1);
      GC.Collect();
    }

    private void OnPlayerDeath(Simulation simulation, bool invokedByButton = false)
    {
      if (!simulation.IsMain)
        return;
      StateManager.currentState?.OnPlayerDeath();
      if (invokedByButton || !VarManager.GetBool("randomscores"))
        return;
      Utility.PlayRandomScore();
      SimulationManager.UpdateAutoGhost();
    }

    private void OnEscape()
    {
      if (PopupMenu.Instance.IsActive)
        PopupMenu.Instance.SetActive(false);
      else if (StateManager.currentState is AbstractLevelState || StateManager.IsState(GameState.LevelEditor))
        PopupMenu.Instance.ToggleActive();
      else if (ScoreExplorer.Instance.IsActive)
      {
        ScoreExplorer.Instance.SetActive(false);
      }
      else
      {
        if (StateManager.IsState(GameState.MainMenu))
          return;
        StateManager.TransitionTo(GameState.MainMenu);
      }
      SoundManager.Play("click", 0.5f);
    }

    public delegate void StateChangeHandler(AbstractState from, AbstractState to);
  }
}

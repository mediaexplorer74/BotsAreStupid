// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.States.DefaultLevelState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid.States
{
  internal class DefaultLevelState : AbstractLevelWithTextState
  {
    private float autoSaveCooldown;

    public override GameState Type => GameState.InLevel_Default;

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if ((double) this.autoSaveCooldown > 0.0)
      {
        this.autoSaveCooldown -= deltaTime;
      }
      else
      {
        if (SimulationManager.HasMainStarted && !SimulationManager.IsPaused)
          return;
        TextEditor.Instance?.SaveInstructions();
        this.autoSaveCooldown = (float) VarManager.GetInt("autosaveinterval");
      }
    }

    public override void Enter(StateTransition transition)
    {
      base.Enter(transition);
      this.autoSaveCooldown = (float) VarManager.GetInt("autosaveinterval");
    }

    public override void Exit(bool closePopupMenu = true)
    {
      TextEditor.Instance?.SaveInstructions();
      base.Exit(closePopupMenu);
    }

    public override void OnLevelFinish(Simulation simulation, string levelName)
    {
      bool manualControls = VarManager.GetBool("manualControls");
      if (!manualControls && !VarManager.GetBool("silentmode"))
      {
        string fullInstructions = TextEditor.Instance.GetFullInstructions();
        ScoreManager.UploadScore(levelName, fullInstructions, (HttpManager.Callback) (() =>
        {
          if (!VarManager.GetBool("autoaddghost"))
            return;
          SimulationManager.UpdateAutoGhost(true);
        }));
      }
      ScoreData fixedOverview1 = this.GetFixedOverview(levelName);
      ScoreData fixedOverview2 = this.GetFixedOverview(levelName, true);
      float playTime = simulation.Player.PlayTime;
      int lineCount = manualControls ? 999 : simulation.Player.ScriptInterpreter.InstructionCount;
      bool flag = levelName.Contains("Custom");
      bool nextAvailable = !flag && levelName != LevelManager.DefaultLevels[LevelManager.DefaultLevels.Length - 1];
      bool nextLocked = !flag && LevelManager.GetLevelIdx(levelName) + 1 >= VarManager.GetInt("demolock");
      PopupMenu.Instance.ShowSuccess(playTime, lineCount, fixedOverview1, fixedOverview2, manualControls, nextAvailable, nextLocked);
      base.OnLevelFinish(simulation, levelName);
    }

    private ScoreData GetFixedOverview(string levelName, bool personal = false)
    {
      ScoreData overview = ScoreManager.TryGetOverview(levelName, personal);
      if ((double) overview.time <= 0.0)
        overview.time = float.MaxValue;
      if (overview.lineCount <= 0)
        overview.lineCount = int.MaxValue;
      if ((double) overview.averageTime <= 0.0)
        overview.averageTime = 0.0f;
      if (overview.averageLineCount <= 0)
        overview.averageLineCount = 0;
      return overview;
    }
  }
}

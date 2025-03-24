// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.States.WatchLevelState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable enable
namespace BotsAreStupid.States
{
  internal class WatchLevelState : AbstractLevelWithTextState
  {
    public override GameState Type => GameState.InLevel_Watch;

    public override void OnLevelFinish(
    #nullable disable
    Simulation simulation, string levelName)
    {
      base.OnLevelFinish(simulation, levelName);
      this.CheckNext(simulation, levelName);
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (SimulationManager.HasMainStarted || !SimulationManager.AllReady)
        return;
      TextEditor.Instance.StartInstructions(false);
    }

    private async void CheckNext(Simulation simulation, string levelName)
    {
      if (!VarManager.GetBool("playlist"))
        return;
      string nextLevel = "";
      string[] defaultLevels = LevelManager.DefaultLevels;
      for (int i = 0; i < defaultLevels.Length - 1; ++i)
      {
        if (levelName == defaultLevels[i])
          nextLevel = defaultLevels[i + 1];
      }
      if (!string.IsNullOrEmpty(nextLevel))
      {
        ScoreData[] scores = await ScoreManager.GetList(nextLevel, playerName: simulation.PlayerName);
        if (scores != null && scores.Length != 0)
        {
          Utility.PlayScore(scores[0], nextLevel);
          return;
        }
        scores = (ScoreData[]) null;
      }
      VarManager.SetBool("playlist", false);
      StateManager.TransitionTo(GameState.LevelSelection);
      nextLevel = (string) null;
      defaultLevels = (string[]) null;
    }
  }
}

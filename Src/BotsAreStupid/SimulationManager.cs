// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.SimulationManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace BotsAreStupid
{
  internal class SimulationManager
  {
    public static 
    #nullable disable
    SimulationManager Instance = new SimulationManager();
    private static bool isPaused;
    private Simulation mainSimulation;
    private const float fixedStepDeltaTime = 0.005f;
    private ConcurrentDictionary<string, Simulation> simulations = new ConcurrentDictionary<string, Simulation>();

    public static bool IsPaused
    {
      get
      {
        return SimulationManager.isPaused || PopupMenu.Instance.IsActive || ScoreExplorer.Instance.IsActive;
      }
      private set => SimulationManager.isPaused = value;
    }

    public static bool HasMainStarted
    {
      get
      {
        Simulation mainSimulation = SimulationManager.MainSimulation;
        return mainSimulation != null && mainSimulation.HasStarted;
      }
    }

    public static bool HasMainScriptFinished
    {
      get => SimulationManager.HasMainStarted && SimulationManager.MainSimulation.IsScriptFinished;
    }

    public static bool HasLockedFrame
    {
      get
      {
        Simulation mainSimulation = SimulationManager.MainSimulation;
        return mainSimulation != null && mainSimulation.HasLockedFrame;
      }
    }

    public static Simulation MainSimulation
    {
      get
      {
        return SimulationManager.Instance.mainSimulation ?? SimulationManager.CreateSimulation(out bool _, "Main", playerName: VarManager.GetBool("checkmode") ? "check" : VarManager.GetString("username"));
      }
    }

    public static event Action<Simulation> OnSimulationCreated;

    public static event Action<Simulation> OnSimulationRemoved;

    public static event System.Action OnSimulationsCleared;

    public static event SimulationManager.LevelFinishedHandler OnLevelFinished;

    public static event SimulationManager.PlayerDeathHandler OnPlayerDeath;

    public static bool AllReady
    {
      get
      {
        return !Enumerable.Any<KeyValuePair<string, Simulation>>((IEnumerable<KeyValuePair<string, Simulation>>) SimulationManager.Instance.simulations, (Func<KeyValuePair<string, Simulation>, bool>) (s => !s.Value.CanBeStarted));
      }
    }

    public static void Update(float deltaTime)
    {
      if (SimulationManager.IsPaused)
        return;
      float timescale = !VarManager.HasInt("timescale") || VarManager.IsOverclocked || !SimulationManager.MainSimulation.HasStarted ? 1f : (float) VarManager.GetInt("timescale") / 100f;
      SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation => simulation.Update(deltaTime * timescale)));
    }

    public static void SetPaused(bool paused) => SimulationManager.IsPaused = paused;

    public static Simulation CreateSimulation(
      out bool levelLoadSuccess,
      string id = null,
      string[] instructions = null,
      string playerName = null,
      ScoreData scoreData = null,
      string levelName = null)
    {
      string id1 = id ?? "Sim-" + SimulationManager.Instance.simulations.Count.ToString();
      Simulation simulation = new Simulation(id1, instructions, playerName, scoreData);
      bool flag = !string.IsNullOrEmpty(levelName);
      SimulationManager.Instance.simulations.TryAdd(id1, simulation);
      if (simulation.IsMain)
        SimulationManager.Instance.mainSimulation = simulation;
      levelLoadSuccess = !flag || LevelManager.Load(levelName, simulation);
      if (!simulation.IsMain)
      {
        if (!flag)
          SimulationManager.MainSimulation.ForEachGameObject((Action<GameObject>) (g =>
          {
            if (!g.IsEssentialPerSimulation)
              return;
            g.Copy(simulation);
          }), reversedOrder: false);
        simulation.SpawnPipe?.SpawnPlayerASAP();
        Action<Simulation> simulationCreated = SimulationManager.OnSimulationCreated;
        if (simulationCreated != null)
          simulationCreated(simulation);
      }
      simulation.SetTickrate(VarManager.GetInt("defaulttickrate"));
      return simulation;
    }

    public static void RemoveSimulation(Simulation simulation = null, string id = null)
    {
      Simulation simulation1;
      if (!SimulationManager.Instance.simulations.TryRemove(id ?? simulation.ID, out simulation1))
        return;
      simulation = simulation ?? simulation1;
      Action<Simulation> simulationRemoved = SimulationManager.OnSimulationRemoved;
      if (simulationRemoved != null)
        simulationRemoved(simulation);
    }

    public static Simulation GetSimulation(string id)
    {
      Simulation simulation;
      return SimulationManager.Instance.simulations.TryGetValue(id, out simulation) ? simulation : (Simulation) null;
    }

    public static void ClearSimulations()
    {
      foreach (KeyValuePair<string, Simulation> simulation in SimulationManager.Instance.simulations)
      {
        if (simulation.Value.IsMain)
          simulation.Value.SetTickrate(VarManager.GetInt("defaulttickrate"));
        else
          SimulationManager.RemoveSimulation(simulation.Value);
      }
      System.Action simulationsCleared = SimulationManager.OnSimulationsCleared;
      if (simulationsCleared == null)
        return;
      simulationsCleared();
    }

    public static async void UpdateAutoGhost(bool clearCaches = false)
    {
      SimulationManager.RemoveSimulation(id: "AutoGhost");
      ScoreData data;
      string levelName;
      if (!VarManager.GetBool("autoaddghost"))
      {
        data = (ScoreData) null;
        levelName = (string) null;
      }
      else
      {
        data = (ScoreData) null;
        levelName = LevelManager.CurrentLevelName;
        if (clearCaches)
          ScoreManager.ClearLevelCaches(levelName);
        string str1 = VarManager.GetString("autoaddghosttype");
        ScoreData[] personalList;
        ScoreData[] globalList;
        switch (str1)
        {
          case "random":
            data = await ScoreManager.GetRandom(levelName);
            break;
          case "best_personal":
            personalList = await ScoreManager.GetList(levelName, playerName: VarManager.GetString("username"));
            ScoreData[] scoreDataArray1 = personalList;
            data = (scoreDataArray1 != null ? (scoreDataArray1.Length != 0 ? 1 : 0) : 0) != 0 ? personalList[0] : (ScoreData) null;
            break;
          case "best_global":
            globalList = await ScoreManager.GetList(levelName);
            ScoreData[] scoreDataArray2 = globalList;
            data = (scoreDataArray2 != null ? (scoreDataArray2.Length != 0 ? 1 : 0) : 0) != 0 ? globalList[0] : (ScoreData) null;
            break;
        }
        personalList = (ScoreData[]) null;
        globalList = (ScoreData[]) null;
        str1 = (string) null;
        if (data == null)
        {
          data = (ScoreData) null;
          levelName = (string) null;
        }
        else
        {
          if (string.IsNullOrEmpty(data.script))
          {
            if (data.databaseId == -1)
            {
              data = (ScoreData) null;
              levelName = (string) null;
              return;
            }
            ScoreData scoreData = data;
            string str2 = await ScoreManager.GetScriptById(data.databaseId);
            scoreData.script = str2;
            scoreData = (ScoreData) null;
            str2 = (string) null;
          }
          SimulationManager.CreateSimulation(out bool _, "AutoGhost", data.script.Split('|'), data.playerName, data);
          data = (ScoreData) null;
          levelName = (string) null;
        }
      }
    }

    public static void ProgressStep(int amount = 1)
    {
      if (!SimulationManager.IsPaused)
        return;
      for (int index = 0; index < amount; ++index)
      {
        float deltaTime = SimulationManager.MainSimulation.FixedDeltaTime;
        SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation => simulation.Update(deltaTime)));
      }
      TextEditor.Instance?.UpdateView();
    }

    public static void RevertStep(int amount = 1)
    {
      if (!SimulationManager.IsPaused)
        return;
      for (int index = 0; index < amount; ++index)
        SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation => simulation.Update(SimulationManager.MainSimulation.SaveInterval, true)));
      TextEditor.Instance?.UpdateView();
    }

    public static void HandlePlayerDeath(Simulation simulation, bool invokedByButton)
    {
      SimulationManager.PlayerDeathHandler onPlayerDeath = SimulationManager.OnPlayerDeath;
      if (onPlayerDeath != null)
        onPlayerDeath(simulation, invokedByButton);
      if (SimulationManager.MainSimulation.Player == null)
        return;
      SimulationManager.HandleLevelFinish(simulation);
    }

    public static void HandleLevelFinish(Simulation simulation)
    {
      bool allFinished = true;
      SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (s =>
      {
        if (s.IsFinished)
          return;
        allFinished = false;
      }));
      if (!allFinished || SimulationManager.MainSimulation.Player == null)
        return;
      SimulationManager.LevelFinishedHandler onLevelFinished = SimulationManager.OnLevelFinished;
      if (onLevelFinished == null)
        return;
      onLevelFinished(SimulationManager.MainSimulation, LevelManager.CurrentLevelName);
    }

    public static void ForEachSimulation(
      SimulationManager.ForEachHandler handler,
      bool forceMainLast = false)
    {
      foreach (KeyValuePair<string, Simulation> simulation in SimulationManager.Instance.simulations)
      {
        if (!forceMainLast || simulation.Value != SimulationManager.MainSimulation)
          handler(simulation.Value);
      }
      if (!forceMainLast)
        return;
      handler(SimulationManager.MainSimulation);
    }

    public delegate void LevelFinishedHandler(Simulation simulation, string levelName);

    public delegate void PlayerDeathHandler(Simulation simulation, bool invokedByButton);

    public delegate void ForEachHandler(Simulation simulation);
  }
}

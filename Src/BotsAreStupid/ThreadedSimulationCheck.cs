// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ThreadedSimulationCheck
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;
using System.Threading;

#nullable disable
namespace BotsAreStupid
{
  internal class ThreadedSimulationCheck : IDisposable
  {
    private bool exitAsap;
    private bool debug;
    private Thread thread;
    private Simulation simulation;
    private float virtualTimeElapsed;
    private static readonly float fixedDeltaTime = 0.005f;
    private static int checkCount = -1;

    public bool IsFinished => this.thread == null || !this.thread.IsAlive;

    public string ID { get; private set; }

    public string LevelName { get; private set; }

    public bool IsCustom { get; private set; }

    public string[] Instructions { get; private set; }

    public bool SaveSuccessful { get; private set; }

    public bool IsSuccess { get; private set; }

    public string ExitMessage { get; private set; }

    public float ExitTime => this.simulation?.Player?.PlayTime ?? this.virtualTimeElapsed;

    public object Data { get; set; }

    public ThreadedSimulationCheck(
      string levelName,
      string[] instructions,
      bool saveSuccessful = false,
      bool debug = false)
    {
      this.LevelName = levelName;
      this.IsCustom = levelName.Contains("custom");//, StringComparison.InvariantCultureIgnoreCase);
      this.Instructions = instructions;
      this.SaveSuccessful = saveSuccessful;
      this.debug = debug;
      this.ID = "Check#" + (++ThreadedSimulationCheck.checkCount).ToString();
      bool levelLoadSuccess;
      this.simulation = SimulationManager.CreateSimulation(out levelLoadSuccess, this.ID, instructions, levelName: levelName);
      if (!levelLoadSuccess)
      {
        this.Exit(false, "Corrupt Level");
      }
      else
      {
        this.simulation.CheckMode = true;
        this.thread = new Thread(new ThreadStart(this.Run));
      }
    }

    public void Dispose()
    {
      SimulationManager.RemoveSimulation(this.simulation);
      if (this.IsFinished)
        return;
      this.thread?.Abort();
    }

    public void Start() => this.thread?.Start();

    private void Run()
    {
      float num1 = (float) VarManager.GetInt("maxplayerlifetime");
      SimulationManager.OnPlayerDeath += new SimulationManager.PlayerDeathHandler(this.HandlePlayerDeath);
      while (!this.exitAsap)
      {
        if (!this.simulation.HasStarted && this.simulation.CanBeStarted)
          this.simulation.StartASAP();
        this.virtualTimeElapsed += ThreadedSimulationCheck.fixedDeltaTime;
        if ((double) this.virtualTimeElapsed > (double) num1)
          this.Exit(false, "Timeout");
        try
        {
          this.simulation.Update(ThreadedSimulationCheck.fixedDeltaTime);
          if (this.debug)
            Console.WriteLine("Pos: " + this.simulation.Player.X.ToString() + "," + this.simulation.Player.Y.ToString() + " (" + this.simulation.IsFinished.ToString() + ")");
        }
        catch (Exception ex)
        {
          Utility.LogError(ex);
          this.Exit(false, "Error");
        }
        int num2;
        if (this.simulation.IsFinished)
        {
          Player player = this.simulation.Player;
          num2 = player != null ? (player.IsActive ? 1 : 0) : 0;
        }
        else
          num2 = 0;
        if (num2 != 0)
        {
          int num3 = (int) Math.Round((double) this.simulation.Player.PlayTime * 1000.0);
          int instructionCount = this.simulation.Player.ScriptInterpreter.InstructionCount;
          this.Exit(true, num3.ToString() + " " + instructionCount.ToString());
        }
      }
      SimulationManager.OnPlayerDeath -= new SimulationManager.PlayerDeathHandler(this.HandlePlayerDeath);
    }

    private void Exit(bool isSuccess, string exitMessage)
    {
      this.exitAsap = true;
      this.IsSuccess = isSuccess;
      this.ExitMessage = exitMessage;
    }

    private void HandlePlayerDeath(Simulation s, bool button)
    {
      if (s != this.simulation)
        return;
      this.Exit(false, "Death");
    }
  }
}

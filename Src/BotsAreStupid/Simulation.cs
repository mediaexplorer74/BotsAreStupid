// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Simulation
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class Simulation
  {
    public const string MainSimulationID = "Main";
    public const string IntroSimulationID = "Intro";
    private string id;
    private List<string> instructions;
    private float timeSincePhysicsStep = 0.0f;
    private int tickrate;
    private List<GameObject> gameObjects = new List<GameObject>();
    private Dictionary<Type, List<GameObject>> gameObjectDictionary = new Dictionary<Type, List<GameObject>>();
    private List<GameObject> serializableGameObjects = new List<GameObject>();
    private List<BaseObject> particles = new List<BaseObject>();
    private Stack<SimulationState> savedStates = new Stack<SimulationState>();
    private SimulationState? lockedState;
    private List<GameObject> collisionObjects = new List<GameObject>();
    private bool startASAP;
    private bool autoSaveOnStartInstructions;
    private bool enableDraw = false;
    private float timeSinceSave = 0.0f;
    private float timeSinceLoad = 0.0f;

    public string ID
    {
      get => this.id;
      private set
      {
        this.id = value;
        this.IsMain = this.id == "Main";
        this.IsIntro = this.id == "Intro";
      }
    }

    public string PlayerName { get; private set; }

    public ScoreData ScoreData { get; private set; }

    public int UnlockedLevels
    {
      get
      {
        return !this.IsMain || StateManager.IsState(GameState.InLevel_Watch) 
                    || VarManager.GetInt("unlockedlevels") >= VarManager.GetInt("demolock") ? 9999
                    : VarManager.GetInt("unlockedlevels");
      }
    }

    public bool IsMain { get; private set; }

    public bool IsIntro { get; private set; }

    public Player Player { get; private set; }

    public Portal Portal { get; private set; }

    public SpawnPipe SpawnPipe { get; private set; }

    public float FixedDeltaTime => 1f / (float) this.tickrate;

    public float SaveInterval { get; private set; }

    public bool CanBeStarted
    {
      get
      {
        Player player = this.Player;
        return player != null && (double) player.AfterGroundedTime > 0.5;
      }
    }

    public bool HasStarted
    {
      get
      {
        if (this.startASAP || this.IsScriptRunning || this.IsScriptFinished)
          return true;
        Player player = this.Player;
        return player != null && (double) player.PlayTime > 0.0;
      }
    }

    public bool IsScriptRunning
    {
      get
      {
        Player player = this.Player;
        if (player == null)
          return false;
        bool? isInstructing = player.ScriptInterpreter?.IsInstructing;
        bool flag = true;
        return isInstructing.GetValueOrDefault() == flag & isInstructing.HasValue;
      }
    }

    public bool IsScriptFinished
    {
      get
      {
        Player player = this.Player;
        if (player == null)
          return false;
        bool? hasInstructed = player.ScriptInterpreter?.HasInstructed;
        bool flag = true;
        return hasInstructed.GetValueOrDefault() == flag & hasInstructed.HasValue;
      }
    }

    public bool IsFinished { get; private set; }

    public bool HasLockedFrame => this.lockedState.HasValue;

    public bool CanStepBackwards
    {
      get
      {
        if (this.savedStates.Count > 1)
          return true;
        if (this.savedStates.Count != 1)
          return false;
        double playerTime = (double) this.savedStates.Peek().playerTime;
        float? playTime = this.Player?.PlayTime;
        double valueOrDefault = (double) playTime.GetValueOrDefault();
        return !(playerTime == valueOrDefault & playTime.HasValue);
      }
    }

    public bool RenderDefault => this.IsMain || this.ForceRenderAll;

    public bool ForceRenderAll { get; set; }

    public bool CheckMode { get; set; }

    public bool CanSaveScriptTime { get; set; }

    public float SimulationTime { get; private set; }

    private int GameObjectCount => this.gameObjects.Count + this.serializableGameObjects.Count;

    public Simulation(string id, string[] instructions = null, string playerName = null, ScoreData scoreData = null)
    {
      this.ID = id;
      this.PlayerName = playerName;
      this.ScoreData = scoreData;
      this.SetInstructions(instructions);
      if (this.IsIntro)
        this.startASAP = true;
      if (this.IsMain)
        StateManager.OnStateChange += (StateManager.StateChangeHandler) ((from, to) =>
        {
          this.Reset(true);
          GC.Collect();
        });
      VarManager.AddListener("savespersecond", (VarManager.VarChangeHandler) (v => updateSaveInterval()));
      updateSaveInterval();

      void updateSaveInterval()
      {
        this.SaveInterval = 1f / (float) VarManager.GetInt("savespersecond");
      }
    }

    private SimulationState GetCurrentState()
    {
      SimulationState local1 = new SimulationState();
      local1.serializedGameObjects = new GameObject[this.serializableGameObjects.Count];
      local1.serializedParticles = new BaseObject[this.particles.Count];
      local1.tickrate = this.tickrate;
      local1.isFinished = this.IsFinished;
      local1.time = this.SimulationTime;
      //ref SimulationState local1 = ref currentState;
      Player player = this.Player;
      double num = player != null ? (double) player.PlayTime : -1.0;
      local1.playerTime = (float) num;
      local1.needsStart = false;
      if (this.IsMain)
      {
        //ref SimulationState local2 = ref currentState;
        SimulationState local2 = new SimulationState();
        local2 = local1;
        TextEditor instance = TextEditor.Instance;
        int currentInstructionLine = instance != null ? instance.CurrentInstructionLine : 0;
        local2.currentInstructionLine = currentInstructionLine;
        MarkedLine[] instructionLines = TextEditor.Instance?.InstructionLines;
        if (instructionLines != null)
        {
          MarkedLine[] markedLineArray = new MarkedLine[instructionLines.Length];
          for (int index = 0; index < instructionLines.Length; ++index)
            markedLineArray[index] = (MarkedLine) instructionLines[index].Clone();
          local2.currentMarkedLines = markedLineArray;
        }
        else
          local2.currentMarkedLines = (MarkedLine[]) null;
      }
      else
      {
        local1.currentInstructionLine = -1;
        local1.currentMarkedLines = (MarkedLine[]) null;
      }
      for (int index = 0; index < this.serializableGameObjects.Count; ++index)
      {
        GameObject serializableGameObject = this.serializableGameObjects[index];
        if (serializableGameObject != null)
          local1.serializedGameObjects[index] = serializableGameObject.Clone() as GameObject;
      }
      for (int index = 0; index < this.particles.Count; ++index)
      {
        BaseObject particle = this.particles[index];
        if (particle != null)
          local1.serializedParticles[index] = particle.Clone() as BaseObject;
      }
      return local1;
    }

    private void SaveState(bool needsStart = false)
    {
      this.savedStates.Push(this.GetCurrentState() with
      {
        needsStart = needsStart
      });
      this.timeSinceSave = 0.0f;
      this.timeSinceLoad = 0.0f;
    }

    private void LoadState(SimulationState? overrideSave = null)
    {
      if (this.savedStates.Count == 0)
        return;
      SimulationState simulationState = overrideSave ?? this.savedStates.Pop();
      this.DestroyAll(true);
      this.SetTickrate(simulationState.tickrate);
      this.SetFinished(simulationState.isFinished);
      this.SimulationTime = simulationState.time;
      foreach (BaseObject serializedGameObject in simulationState.serializedGameObjects)
        this.RegisterObject(serializedGameObject);
      foreach (BaseObject serializedParticle in simulationState.serializedParticles)
        this.RegisterObject(serializedParticle);
      if (this.IsMain)
      {
        TextEditor.Instance?.SetInstructionLine(simulationState.currentInstructionLine, this.Player?.ScriptInterpreter?.FurthestInstruction ?? -1);
        TextEditor.Instance?.SetInstructionLines(simulationState.currentMarkedLines);
      }
      this.Portal?.UpdateEnergyLevel();
      this.startASAP = simulationState.needsStart;
      if (this.savedStates.Count == 0)
        this.SaveState(true);
      else
        this.timeSinceSave = this.SaveInterval;
      this.timeSinceLoad = 0.0f;
    }

    public int RegisterObject(BaseObject baseObject)
    {
      baseObject.CurrentSimulation = this;
      if (baseObject is Particle || baseObject is ParticleGroup)
      {
        this.particles.Add(baseObject);
        return this.particles.Count;
      }
      GameObject gameObject = baseObject as GameObject;
      switch (gameObject)
      {
        case Player _:
          this.Player?.Destroy();
          this.Player = gameObject as Player;
          break;
        case Portal _:
          this.Portal?.Destroy();
          this.Portal = gameObject as Portal;
          break;
        case SpawnPipe _:
          this.SpawnPipe?.Destroy();
          this.SpawnPipe = gameObject as SpawnPipe;
          break;
      }
      if (gameObject.IsSerializable)
        this.serializableGameObjects.Add(gameObject);
      else
        this.gameObjects.Add(gameObject);
      Type type = gameObject.GetType();
      if (!this.gameObjectDictionary.ContainsKey(type))
        this.gameObjectDictionary.Add(type, new List<GameObject>());
      this.gameObjectDictionary[type].Add(gameObject);
      if (gameObject.HasCollision)
        this.collisionObjects.Add(gameObject);
      gameObject.CurrentSimulation = this;
      return this.GameObjectCount;
    }

    public void RemoveObject(BaseObject baseObject)
    {
      baseObject.CurrentSimulation = (Simulation) null;
      if (baseObject is Particle || baseObject is ParticleGroup)
      {
        this.particles.Remove(baseObject);
      }
      else
      {
        GameObject gameObject = baseObject as GameObject;
        if (gameObject == this.Player)
          this.Player = (Player) null;
        else if (gameObject == this.Portal)
          this.Portal = (Portal) null;
        else if (gameObject == this.SpawnPipe)
          this.SpawnPipe = (SpawnPipe) null;
        if (!this.serializableGameObjects.Remove(gameObject))
          this.gameObjects.Remove(gameObject);
        this.gameObjectDictionary[gameObject.GetType()].Remove(gameObject);
        if (gameObject.HasCollision)
          this.collisionObjects.Remove(gameObject);
        gameObject.CurrentSimulation = (Simulation) null;
      }
    }

    public void Update(float deltaTime, bool reversed = false)
    {
      if (!this.enableDraw)
        this.enableDraw = true;
      this.SimulationTime += deltaTime;
      if (this.startASAP && (this.CanBeStarted || this.IsIntro && this.Player != null))
        this.Start();
      if (reversed || VarManager.GetBool("reversesimulations"))
      {
        this.timeSinceLoad += deltaTime;
        if ((double) this.timeSinceLoad < (double) this.SaveInterval)
          return;
        this.LoadState();
      }
      else
      {
        if (!this.CheckMode)
        {
          this.timeSinceSave += deltaTime;
          if ((double) this.timeSinceSave >= (double) this.SaveInterval && (this.IsScriptRunning || this.IsScriptFinished))
            this.SaveState();
        }
        this.timeSincePhysicsStep += deltaTime;
        if ((double) this.timeSincePhysicsStep >= (double) this.FixedDeltaTime)
        {
          int num = (int) ((double) this.timeSincePhysicsStep / (double) this.FixedDeltaTime);
          for (int index = 0; index < num; ++index)
          {
            this.ForEachGameObject((Action<GameObject>) (g =>
            {
              if (!g.IsActive || this.IsFinished && !(g is ConveyorBelt) && !(g is ParticleSpawningObject))
                return;
              g.Update(this.FixedDeltaTime);
            }));
            this.ForEachParticle((Action<BaseObject>) (p =>
            {
              if (!p.IsActive)
                return;
              p.Update(this.FixedDeltaTime);
            }));
          }
          this.timeSincePhysicsStep = (float) Math.Round((double) this.timeSincePhysicsStep - (double) num * (double) this.FixedDeltaTime, 4);
        }
      }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      if (!this.enableDraw)
        return;
      bool showHitboxes = VarManager.GetBool("showhitboxes");
      this.ForEachGameObject((Action<GameObject>) (g =>
      {
        int num;
        if (g.IsActive)
        {
          if (!this.RenderDefault)
          {
            switch (g)
            {
              case SpawnPipe _:
              case Portal _:
              case Platform _:
              case BackgroundObject _:
                num = 0;
                break;
              default:
                num = !(g is Spike) ? 1 : 0;
                break;
            }
          }
          else
            num = 1;
        }
        else
          num = 0;
        if (num == 0)
          return;
        g.Draw(spriteBatch);
        if (showHitboxes)
          g.DrawHitbox(spriteBatch);
      }));
      this.ForEachParticle((Action<BaseObject>) (p =>
      {
        if (!p.IsActive)
          return;
        p.Draw(spriteBatch);
        if (showHitboxes)
          p.DrawHitbox(spriteBatch);
      }));
    }

    public void OnPlayerDeath(bool invokedByButton)
    {
      this.SetFinished();
      SimulationManager.HandlePlayerDeath(this, invokedByButton);
    }

    public void OnLevelFinish()
    {
      this.SetFinished();
      if (this.IsMain && !VarManager.GetBool("manualControls") || this.CanSaveScriptTime)
        ScriptManager.SaveScriptTime(LevelManager.CurrentLevelName, this.IsMain ? TextEditor.Instance.CurrentScript : this.PlayerName, this.Player.PlayTime);
      SimulationManager.HandleLevelFinish(this);
    }

    public void SetFinished(bool finished = true)
    {
      this.IsFinished = finished;
      if (!finished || !this.IsIntro)
        return;
      SimulationManager.RemoveSimulation(this);
      VarManager.SetBool("hasplayedintro_" + LevelManager.CurrentLevelName, true);
      VarManager.SaveOptions();
    }

    public void Clear()
    {
      this.DestroyAll();
      this.Reset(true);
    }

    public void Reset(bool force = false)
    {
      if (!force && this.lockedState.HasValue)
      {
        this.LoadState(this.lockedState);
        this.lockedState = new SimulationState?(this.GetCurrentState());
        this.savedStates.Clear();
        this.SaveState();
      }
      else
      {
        this.SetTickrate(VarManager.GetInt("defaulttickrate"));
        this.SetAllActive(true);
        this.Player?.Destroy();
        this.SpawnPipe?.SpawnPlayerASAP();
        this.SetFinished(false);
        this.savedStates.Clear();
        if (this.IsIntro)
          this.startASAP = true;
        this.timeSincePhysicsStep = 0.0f;
        this.lockedState = new SimulationState?();
      }
    }

    public void SetTickrate(int newTickrate)
    {
      newTickrate = MathHelper.Clamp(newTickrate, VarManager.GetInt("mintickrate"), VarManager.GetInt("maxtickrate"));
      this.tickrate = newTickrate;
    }

    public void SetPlayerName(string playerName = null) => this.PlayerName = playerName;

    public void SetInstructions(params string[] instructions)
    {
      if (instructions == null || instructions.Length == 0)
        return;
      this.instructions = new List<string>((IEnumerable<string>) instructions);
    }

    public void StartASAP(bool autosave = false)
    {
      this.startASAP = true;
      this.autoSaveOnStartInstructions = autosave;
    }

    public void LockCurrentFrame()
    {
      this.lockedState = new SimulationState?(this.GetCurrentState());
      this.savedStates.Clear();
      this.SaveState();
    }

    public void ResetLockedFrame() => this.lockedState = new SimulationState?();

    private void Start()
    {
      if (!this.CheckMode && this.savedStates.Count == 0)
        this.SaveState(true);
      this.Player.ScriptInterpreter.StartInstructions(this.instructions, this.autoSaveOnStartInstructions && this.IsMain);
      this.timeSinceSave = 0.0f;
      this.timeSincePhysicsStep = 0.0f;
      this.startASAP = false;
    }

    public void DestroyAll(bool serializableOnly = false)
    {
      this.ForEachGameObject((Action<GameObject>) (g => g?.Destroy()), serializableOnly);
      this.ForEachParticle((Action<BaseObject>) (p => p?.Destroy()));
      if (!serializableOnly)
      {
        this.gameObjects.Clear();
        this.gameObjectDictionary.Clear();
        this.collisionObjects.Clear();
      }
      this.serializableGameObjects.Clear();
    }

    public int CountType(Type type, bool requireActive = true, Simulation.CountTypeCondition condition = null)
    {
      if (!this.gameObjectDictionary.ContainsKey(type))
        return 0;
      if (!requireActive)
        return this.gameObjectDictionary[type].Count;
      int count = 0;
      this.ForEachGameObject((Action<GameObject>) (g =>
      {
        int num1 = count;
        int num2;
        if (g.IsActive)
        {
          Simulation.CountTypeCondition countTypeCondition = condition;
          if ((countTypeCondition != null ? (countTypeCondition(g) ? 1 : 0) : 1) != 0)
          {
            num2 = 1;
            goto label_4;
          }
        }
        num2 = 0;
label_4:
        count = num1 + num2;
      }), onlyType: type);
      return count;
    }

    public int CountType(params Type[] types)
    {
      int num = 0;
      foreach (Type type in types)
        num += this.CountType(type);
      return num;
    }

    public List<GameObject> GetAllByType(params Type[] types)
    {
      List<GameObject> allByType = new List<GameObject>();
      foreach (Type type in types)
      {
        if (this.gameObjectDictionary.ContainsKey(type))
          allByType.AddRange((IEnumerable<GameObject>) this.gameObjectDictionary[type]);
      }
      return allByType;
    }

    public List<GameObject> GetCollisionObjects() => this.collisionObjects;

    public void SetAllActive(bool IsActive)
    {
      this.ForEachGameObject((Action<GameObject>) (g => g.SetActive(IsActive)));
    }

    public GameObject GetById(int id)
    {
      GameObject result = (GameObject) null;
      this.ForEachGameObject((Action<GameObject>) (g =>
      {
        if (g.Id != id)
          return;
        result = g;
      }));
      return result;
    }

    public void ForEachGameObject(
      Action<GameObject> handler,
      bool serializableOnly = false,
      bool reversedOrder = true,
      Type onlyType = null)
    {
      if (onlyType != (Type) null)
      {
        if (!this.gameObjectDictionary.ContainsKey(onlyType))
          return;
        forEachGameObject(this.gameObjectDictionary[onlyType], handler, reversedOrder);
      }
      else
      {
        if (!serializableOnly)
          forEachGameObject(this.gameObjects, handler, reversedOrder);
        forEachGameObject(this.serializableGameObjects, handler, reversedOrder);
      }

      void forEachGameObject(List<GameObject> list, Action<GameObject> handler, bool reversed = true)
      {
        if (reversedOrder)
        {
          for (int index = list.Count - 1; index >= 0; index = Math.Min(index - 1, list.Count - 1))
          {
            if (list[index] != null)
              handler(list[index]);
          }
        }
        else
        {
          for (int index = 0; index < list.Count; ++index)
          {
            if (list[index] != null)
              handler(list[index]);
          }
        }
      }
    }

    public void ForEachParticle(Action<BaseObject> handler, bool reversedOrder = true)
    {
      if (reversedOrder)
      {
        for (int index = this.particles.Count - 1; index >= 0; index = Math.Min(index - 1, this.particles.Count - 1))
        {
          if (this.particles[index] != null)
            handler(this.particles[index]);
        }
      }
      else
      {
        for (int index = 0; index < this.particles.Count; ++index)
        {
          if (this.particles[index] != null)
            handler(this.particles[index]);
        }
      }
    }

    public delegate bool CountTypeCondition(GameObject gameObject);
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.StateTransition
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using BotsAreStupid.States;
using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class StateTransition
  {
    private const float defaultDuration = 0.2f;
    private float duration;
    private float timeLeft;

    public AbstractState PreviousState { get; private set; }

    public GameState NextState { get; private set; }

    public string NextLevelName { get; private set; }

    public string NextPlayerName { get; private set; }

    public bool ClosePopupMenu { get; private set; }

    public bool IsNavigatingBack { get; private set; }

    public bool IsDone => (double) this.timeLeft <= 0.0;

    public float Progress => 1f - MathHelper.Clamp(this.timeLeft / this.duration, 0.0f, 1f);

    public StateTransition(
      AbstractState previousState,
      GameState state,
      string levelName,
      string playerName,
      bool closePopupMenu,
      bool isNavigatingBack)
    {
      this.duration = 0.2f;
      this.timeLeft = this.duration;
      this.PreviousState = previousState;
      this.NextState = state;
      this.NextLevelName = levelName;
      this.NextPlayerName = playerName;
      this.ClosePopupMenu = closePopupMenu;
      this.IsNavigatingBack = isNavigatingBack;
    }

    public void Update(float deltaTime, out bool crossedHalfTime)
    {
      float progress = this.Progress;
      this.timeLeft -= deltaTime;
      crossedHalfTime = (double) progress < 0.5 && (double) this.Progress >= 0.5;
    }
  }
}

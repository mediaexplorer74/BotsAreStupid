// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.States.LevelSelectionState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid.States
{
  internal class LevelSelectionState : AbstractState
  {
    public override GameState Type => GameState.LevelSelection;

    protected override bool ScaleUI => false;

    public override void CreateUI(Rectangle fullRect)
    {
      base.CreateUI(fullRect);
      this.MainUIElement = (UIElement) new LevelSelection(fullRect);
    }

    public override void Enter(StateTransition transition)
    {
      base.Enter(transition);
      LevelSelection.Instance.UpdateTeaser();
      ScoreManager.ClearCaches();
      VarManager.SetBool("randomscores", false);
    }
  }
}

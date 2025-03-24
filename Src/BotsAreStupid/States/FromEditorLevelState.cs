// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.States.FromEditorLevelState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid.States
{
  internal class FromEditorLevelState : AbstractLevelWithTextState
  {
    public override GameState Type => GameState.InLevel_FromEditor;

    public override void Enter(StateTransition transition)
    {
      base.Enter(transition);
      PopupMenu.Instance.SetExitButtonText("Back to Editor");
    }
  }
}

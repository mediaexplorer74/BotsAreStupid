// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.States.CreditsState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid.States
{
  internal class CreditsState : AbstractState
  {
    public override GameState Type => GameState.Credits;

    protected override bool ScaleUI => false;

    public override void CreateUI(Rectangle fullRect)
    {
      base.CreateUI(fullRect);
      this.MainUIElement = (UIElement) new Credits(fullRect);
    }
  }
}

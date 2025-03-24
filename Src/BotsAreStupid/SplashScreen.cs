// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.SplashScreen
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal class SplashScreen : UIElement
  {
    private const float startTimeLeft = 2.5f;
    private float timeLeft;
    private bool playedSound;

    public SplashScreen(Rectangle rectangle)
      : base(rectangle)
    {
      this.timeLeft = 2.5f;
      Texture2D texture = TextureManager.GetTexture("leleg");
      this.AddChild(new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => parentRect)));
    }

    public override void Update(float deltaTime)
    {
      if (!this.IsActive || !this.IsCurrentState() || (double) this.timeLeft <= 0.0)
        return;
      this.timeLeft -= deltaTime;
      if (!this.playedSound && (double) this.timeLeft <= 2.0)
      {
        SoundManager.Play("lelegsound");
        this.playedSound = true;
      }
      if ((double) this.timeLeft <= 0.0)
      {
        StateManager.TransitionTo(GameState.MainMenu, closePopupMenu: false);
        SoundManager.Instance.StartMusic();
        HttpManager.Instance.CheckConnection(new HttpManager.Callback(VarManager.Instance.AfterLoadCheck));
      }
    }
  }
}

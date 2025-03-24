// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.EnergyOrb
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal class EnergyOrb : ParticleSpawningObject
  {
    private static Color hitboxColor = ColorManager.GetColor("red");
    private const int defaultWidth = 10;
    private const int maxParticles = 16;

    protected override bool ShowHitboxEnabled
    {
      get => base.ShowHitboxEnabled && VarManager.GetBool("showhitboxes_special");
    }

    protected override Color HitboxColor => EnergyOrb.hitboxColor;

    public EnergyOrb(Simulation simulation, float x, float y)
      : base(simulation, x, y, 10, 10, new Color?(Color.White), TextureManager.GetTexture("tileset"), new Rectangle?(TextureManager.GetSpritePos("energyorb")), true, 16)
    {
      this.IsScalable = false;
      this.checkPlayerCollision = true;
      this.defaultSize = new Vector2?(new Vector2(10f, 10f));
      this.particleConfig = new ParticleConfig()
      {
        texture = this.texture,
        spritePos = this.spritePos,
        randomLifeTime = new Vector2?(new Vector2(3.4f, 1f)),
        color = Color.White,
        startDir = Vector2.UnitY,
        startDirRandomness = 180f,
        randomStartOffset = new Vector2?(new Vector2((float) (this.Width / 3 * 2), (float) (this.Width / 6))),
        fadeAway = true,
        fadeIn = true,
        randomStartSize = new Vector2?(new Vector2((float) this.Width, (float) (this.Width / 4))),
        randomStartSpeed = new Vector2?(new Vector2(1.5f, 0.4f))
      };
    }

    protected override void UpdateParticleConfig()
    {
      base.UpdateParticleConfig();
      this.particleConfig.startPos -= Vector2.One * 2f;
    }

    protected override void UpdateRectangle()
    {
      Rectangle r = new Rectangle((int) this.X, (int) this.Y, this.Width, this.Height);
      r.X -= r.Width / 2;
      r.Y -= r.Height / 2;
      r.Width *= 2;
      r.Height *= 2;
      this.CacheRectangle(r);
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new EnergyOrb(intoSimulation ?? this.CurrentSimulation, this.X, this.Y);
    }

    protected override void OnPlayerCollision(Player p)
    {
      if (this.IsDisabled || StateManager.IsState(GameState.LevelEditor))
        return;
      base.OnPlayerCollision(p);
      ParticleTrail particleTrail = new ParticleTrail(this.CurrentSimulation, 6.5f, 8, Color.White, this.Center, this.CurrentSimulation.Portal.Center, TextureManager.GetTexture("tileset"), new Rectangle?(TextureManager.GetSpritePos("energyorb")), true);
      this.CurrentSimulation.Player.HasPickedUpOrb = true;
      SoundManager.Play("orbpickup", 0.6f);
      this.Disable();
    }
  }
}

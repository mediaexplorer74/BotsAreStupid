// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Booster
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class Booster : ParticleSpawningObject
  {
    private static Color hitboxColor = ColorManager.GetColor("orange");
    private const int defaultBoostSpeed = 200;
    private const int defaultWidth = 10;
    private float speedMultiplier = 1f;

    public int BoostSpeed { private set; get; }

    public float AngularBoostSpeed => (float) this.BoostSpeed / 100f;

    protected override bool ShowHitboxEnabled
    {
      get => base.ShowHitboxEnabled && VarManager.GetBool("showhitboxes_special");
    }

    protected override Color HitboxColor => Booster.hitboxColor;

    public Booster(Simulation simulation, float x, float y, int? boostSpeed = null)
      : base(simulation, x, y, 10, 10, new Color?(Color.White), TextureManager.GetTexture("tileset"), new Rectangle?(TextureManager.GetSpritePos("booster")), true)
    {
      this.IsScalable = false;
      this.checkPlayerCollision = true;
      this.BoostSpeed = boostSpeed ?? 200;
      this.defaultSize = new Vector2?(new Vector2(10f, 10f));
      this.availableParameters.Add(new ObjectParameter()
      {
        name = "Boost:",
        min = 50,
        max = 500,
        propertyName = nameof (BoostSpeed),
        gameObject = (GameObject) this,
        changeHandler = (GameObject.ParameterChangeHandler) (v => this.CalculateSpeedMultiplier())
      });
      this.particleConfig = new ParticleConfig()
      {
        texture = this.texture,
        spritePos = this.spritePos,
        randomLifeTime = new Vector2?(new Vector2(1.3f, 1f)),
        color = Color.White,
        startDir = Vector2.UnitY,
        startDirRandomness = 180f,
        fadeAway = true,
        randomSpinSpeedAllowNegative = true,
        fadeIn = true,
        randomizeColor = true,
        randomColorRange = 32,
        randomColorIndividualRange = 4,
        randomStartSize = new Vector2?(new Vector2((float) (this.Width / 5 * 2), (float) (this.Width / 5)))
      };
      this.CalculateSpeedMultiplier();
    }

    protected override void UpdateParticleConfig()
    {
      base.UpdateParticleConfig();
      this.particleConfig.randomSpinSpeed = new Vector2?(new Vector2(1200f * this.speedMultiplier, 300f * this.speedMultiplier));
      this.particleConfig.randomStartSpeed = new Vector2?(new Vector2(8f * this.speedMultiplier, 0.3f * this.speedMultiplier));
    }

    protected override void UpdateRectangle()
    {
      RectangleF r = new RectangleF(this.X, this.Y, (float) this.Width, (float) this.Height);
      r.X -= (float) this.Width * this.speedMultiplier;
      r.Y -= (float) this.Height * this.speedMultiplier;
      r.Width *= (float) (1.0 + 2.0 * (double) this.speedMultiplier);
      r.Height *= (float) (1.0 + 2.0 * (double) this.speedMultiplier);
      this.CacheRectangle((Rectangle) r);
      this.CacheRectanglePrecise(new RectangleF?(r));
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new Booster(intoSimulation ?? this.CurrentSimulation, this.X, this.Y, new int?(this.BoostSpeed));
    }

    protected override void OnPlayerCollision(Player p)
    {
      if (this.IsDisabled || StateManager.IsState(GameState.LevelEditor))
        return;
      base.OnPlayerCollision(p);
      p.Boost(this);
      SoundManager.Play("boostpickup", 0.5f);
      this.Disable();
    }

    private void CalculateSpeedMultiplier()
    {
      float num = (float) this.BoostSpeed / 200f;
      this.speedMultiplier = (float) (Math.Pow(((double) num - 1.0) / 2.0, 3.0) * 2.0 + 1.0);
      this.particleStartDelay = 0.01f / this.speedMultiplier;
      this.UpdateParticleConfig();
      this.UpdateRectangle();
      this.SetMaxParticles((int) ((double) num * 64.0 * 2.0));
    }
  }
}

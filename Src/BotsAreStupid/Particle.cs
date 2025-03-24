// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Particle
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace BotsAreStupid
{
  [Serializable]
  internal class Particle : BaseObject
  {
    private static Microsoft.Xna.Framework.Color hitboxColor = ColorManager.GetColor("white");
    private ParticleConfig config;
    private ParticleConfig trailConfig;
    private float trailCooldown;
    private Vector2 acceleration;
    private Microsoft.Xna.Framework.Color color;
    private float maxLifeTime;
    private float sinOffset;
    private float spinSpeed;
    private float trailLifeTime;

    public int OwnerId { get; private set; } = -1;

    public override float LayerDepth => this.config.layerDepth ?? LayerDepths.Particle_Default;

    protected override Texture2D RenderTexture => this.config.texture;

    protected override Microsoft.Xna.Framework.Color RenderColor => this.color;

    public override Microsoft.Xna.Framework.Rectangle? RenderSpritePos => this.config.spritePos;

    protected override Vector2 RenderScale
    {
      get
      {
        return Utility.CalculateRenderScale(this.Width, this.Height, this.SpriteSize, this.config.fadeIn, this.config.fadeAway, this.lifeTime / this.maxLifeTime);
      }
    }

    protected override bool ShowHitboxEnabled => VarManager.GetBool("showhitboxes_particles");

    protected override Microsoft.Xna.Framework.Color HitboxColor => Particle.hitboxColor;

    public Particle(Simulation simulation, ParticleConfig config, GameObject owner = null)
      : base(simulation, config.startPos.X, config.startPos.Y, (int) Utility.TryGetRandom((float) config.startSize, config.randomStartSize))
    {
      if (VarManager.IsOverclocked)
      {
        this.Destroy();
      }
      else
      {
        Random r = new Random();
        config.startDir.Normalize();
        this.sinOffset = (float) (r.NextDouble() * (2.0 * Math.PI));
        this.SetVelocity(Utility.RandomizeVector(config.startDir, config.startDirRandomness, r) * Utility.TryGetRandom(config.startSpeed, config.randomStartSpeed));
        this.Translate(config.startDir * Utility.TryGetRandom(config.startOffset, config.randomStartOffset));
        this.maxLifeTime = Utility.TryGetRandom(config.lifeTime, config.randomLifeTime);
        this.spinSpeed = Utility.TryGetRandom(0.0f, config.randomSpinSpeed) * (!config.randomSpinSpeedAllowNegative || !Utility.GetBool() ? 1f : -1f);
        this.trailLifeTime = Utility.TryGetRandom(0.0f, config.randomTrailLifeTime);
        this.acceleration = new Vector2(Utility.TryGetRandom(0.0f, config.randomAccelerationX), Utility.TryGetRandom(config.accelerationY, config.randomAccelerationY));
        if (config.color != new Microsoft.Xna.Framework.Color())
        {
          this.color = config.color;
          if (config.randomizeColor)
          {
            int number = (int) Utility.RandomizeNumber(0.0f, (float) Math.Max(config.randomColorRange, 1));
            this.color.R = (byte) MathHelper.Clamp((float) this.color.R + Utility.RandomizeNumber((float) number, (float) config.randomColorIndividualRange), 0.0f, (float) byte.MaxValue);
            this.color.G = (byte) MathHelper.Clamp((float) this.color.G + Utility.RandomizeNumber((float) number, (float) config.randomColorIndividualRange), 0.0f, (float) byte.MaxValue);
            this.color.B = (byte) MathHelper.Clamp((float) this.color.B + Utility.RandomizeNumber((float) number, (float) config.randomColorIndividualRange), 0.0f, (float) byte.MaxValue);
          }
        }
        else
        {
          float random = Utility.GetRandom(0.0f, 0.5f);
          this.color = new Microsoft.Xna.Framework.Color(random, random, random);
        }
        if ((double) config.lifeTime == 0.0)
          config.lifeTime = 1f;
        if (config.texture == null)
          config.texture = !config.round ? TextureManager.GetRectangleTexture(config.startSize, config.startSize, Microsoft.Xna.Framework.Color.White) : TextureManager.GetCircleTexture(config.startSize);
        this.config = config;
        if (owner != null)
          this.OwnerId = owner.Id;
        if (!config.trail)
          return;
        this.UpdateTrailConfig();
      }
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      this.Accelerate(new Vector2((float) Math.Sin((double) this.lifeTime + (double) this.sinOffset), (float) Math.Cos((double) this.lifeTime + (double) this.sinOffset)) * this.config.randomness * deltaTime * 100f);
      this.Accelerate(this.acceleration * deltaTime * 100f);
      this.Translate(this.Velocity * deltaTime);
      if (this.config.trail)
      {
        Vector2 renderScale = this.RenderScale;
        if ((double) renderScale.X > 0.30000001192092896)
        {
          if ((double) this.trailCooldown <= 0.0)
          {
            this.UpdateTrailConfig();
            this.trailConfig.startSize = (int) ((double) this.config.startSize * (double) renderScale.X * ((double) this.config.trailSize != 0.0 ? (double) this.config.trailSize : 0.5)) + 1;
            this.trailConfig.startPos = this.Center - Vector2.One * (float) this.trailConfig.startSize;
            this.trailConfig.startDir = -this.Velocity;
            Particle particle = new Particle(this.CurrentSimulation, this.trailConfig);
            this.trailCooldown = (double) this.config.trailInterval > 0.0 ? this.config.trailInterval : 0.01f;
          }
          else
            this.trailCooldown -= deltaTime;
        }
      }
      this.Rotate(this.spinSpeed * deltaTime);
      if ((double) this.maxLifeTime <= 0.0 || (double) this.lifeTime <= (double) this.maxLifeTime)
        return;
      this.Destroy();
    }

    public static void SpawnGroup(
      Simulation simulation,
      int amount,
      float angleBetween,
      ParticleConfig config,
      float lifeTimeRan = 0.0f)
    {
      if (VarManager.IsOverclocked || amount == 0)
        return;
      float lifeTime = config.lifeTime;
      float num1 = 2f;
      float num2 = 0.0f;
      int num3 = 0;
      while (num3 < amount)
      {
        double num4 = 2.0 * Math.PI * ((double) num2 / 360.0);
        double x = (double) num1 * Math.Cos(num4);
        double y = (double) num1 * Math.Sin(num4);
        config.randomLifeTime = new Vector2?(new Vector2(lifeTime, lifeTimeRan));
        config.startDir = new Vector2((float) x, (float) y);
        Particle particle = new Particle(simulation, config);
        ++num3;
        num2 += angleBetween;
      }
    }

    private void UpdateTrailConfig()
    {
      if (this.trailConfig != null)
        return;
      this.trailConfig = new ParticleConfig()
      {
        layerDepth = new float?(this.LayerDepth - 0.05f),
        lifeTime = (double) this.trailLifeTime > 0.0 ? this.trailLifeTime : 0.6f,
        color = this.config.trailColor,
        round = this.config.round,
        fadeAway = true,
        randomizeColor = true,
        randomColorIndividualRange = 3,
        randomColorRange = this.config.trailRandomColorRange != 0 ? this.config.trailRandomColorRange : 20
      };
    }
  }
}

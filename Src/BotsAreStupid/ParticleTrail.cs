// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ParticleTrail
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal class ParticleTrail : GameObject
  {
    private NoisePath path;
    private float speed;
    private float distance = 0.0f;
    private ParticleConfig trailConfig;
    private ParticleConfig explosionConfig;
    private float cooldownTime = 0.0f;
    private const float cooldownTimeMax = 0.04f;

    public bool IsFromPickup { get; private set; }

    protected override bool ShowHitboxEnabled => VarManager.GetBool("showhitboxes_particles");

    protected override Microsoft.Xna.Framework.Color HitboxColor => Microsoft.Xna.Framework.Color.Magenta;

    public ParticleTrail(
      Simulation simulation,
      float speed,
      int size,
      Microsoft.Xna.Framework.Color color,
      Vector2 start,
      Vector2 end,
      Texture2D texture = null,
      Microsoft.Xna.Framework.Rectangle? spritePos = null,
      bool IsFromPickup = false,
      float scale = 1f)
      : base(simulation, start.X, start.Y, size, size, new Microsoft.Xna.Framework.Color?(color), texture, spritePos)
    {
      this.speed = (float) ((double) speed / (double) (end - start).Length() * 100.0);
      this.IsSelectable = false;
      this.path = new NoisePath(start, end, scale);
      this.layerDepth = LayerDepths.ParticleTrail;
      this.trailConfig = new ParticleConfig()
      {
        startPos = this.Center,
        startSize = size / 3 * 2,
        color = ColorManager.GetColor("red"),
        startSpeed = 10f,
        accelerationY = 3f,
        randomness = 4f,
        lifeTime = 0.2f,
        fadeAway = true,
        round = true
      };
      this.explosionConfig = new ParticleConfig()
      {
        texture = texture,
        spritePos = spritePos,
        startPos = this.path.Last,
        startSize = this.Width / 2,
        color = color,
        startSpeed = 5f,
        randomness = 4f,
        accelerationY = 3f,
        lifeTime = 0.5f,
        fadeAway = true
      };
      this.IsFromPickup = IsFromPickup;
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      this.distance += deltaTime * this.speed;
      int num = VarManager.GetInt("particleamount");
      if ((double) this.distance >= 1.0)
      {
        this.Destroy();
        Particle.SpawnGroup(this.CurrentSimulation, 5 * num, 30f, this.explosionConfig, 0.2f);
        if (!this.IsFromPickup)
          return;
        SoundManager.Play("chargeportal", 0.2f);
      }
      else
      {
        if (VarManager.IsOverclocked)
          return;
        Vector2 currentDir;
        this.SetPosition(this.path.GetPoint(this.distance, out currentDir));
        if (num > 0)
        {
          if ((double) this.cooldownTime <= 0.0)
          {
            this.trailConfig.startDir = -currentDir;
            this.trailConfig.startPos = this.Center;
            Particle particle = new Particle(this.CurrentSimulation, this.trailConfig);
            this.cooldownTime = 0.04f / (float) num;
          }
          else
            this.cooldownTime -= deltaTime;
        }
      }
    }
  }
}

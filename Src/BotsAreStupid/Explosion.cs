// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Explosion
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal class Explosion : GameObject
  {
    private const int bigParts = 1;
    private const int mediumParts = 4;
    private const int smallParts = 2;
    private const int trails = 2;

    public Explosion(
      Simulation simulation,
      Vector2 pos,
      Rectangle? playerRect = null,
      Rectangle? spritePos = null)
      : base(simulation, pos.X, pos.Y, 5, 5, new Color?(Color.Transparent))
    {
      int num1 = VarManager.GetInt("particleamount");
      ParticleConfig config = new ParticleConfig()
      {
        startPos = pos
      };
      this.IsSelectable = false;
      int rayAmount = 8;
      Vector2[] possibleDirections = Utility.SphereCast(this.CurrentSimulation, this.Center, 50, rayAmount);
      if (possibleDirections.Length == 0)
      {
        Vector2 vector = Vector2.UnitY;
        possibleDirections = new Vector2[rayAmount];
        for (int index = 0; index < rayAmount; ++index)
        {
          vector = Utility.RotateVector(vector, 360f / (float) rayAmount);
          possibleDirections[index] = vector;
        }
      }
      int amount = rayAmount / possibleDirections.Length * num1;
      int num2 = 90 / possibleDirections.Length;
      float probability = (float) (0.75 - 0.5 / (double) amount);
      foreach (Vector2 dir in possibleDirections)
        this.SpawnParticles(dir, config, amount, num2, probability);
      for (int index = 0; index < 2 * num1; ++index)
      {
        Vector2 vector = possibleDirections[Utility.GetRandom(possibleDirections.Length - 1)];
        ParticleTrail particleTrail = new ParticleTrail(this.CurrentSimulation, Utility.RandomizeNumber(3f, 1f), 5, ColorManager.GetColor("red"), this.Center, this.Center + Utility.RandomizeVector(vector, (float) num2) * Utility.RandomizeNumber(100f, 50f), TextureManager.GetCircleTexture(6));
      }
      this.SpawnFragments(playerRect, spritePos, possibleDirections);
      SoundManager.PlayRandom("explosion");
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if ((double) this.lifeTime <= 5.0)
        return;
      this.Destroy();
    }

    private void SpawnParticles(
      Vector2 dir,
      ParticleConfig config,
      int amount,
      int randomness,
      float probability)
    {
      dir.Normalize();
      config.randomness = 0.0f;
      config.fadeAway = true;
      config.round = true;
      config.trail = true;
      config.trailColor = new Color(50, 50, 50);
      config.startDir = dir;
      config.startDirRandomness = (float) randomness;
      for (int index = 0; index < amount; ++index)
      {
        if (Utility.GetBool(probability))
        {
          config.startSize = 16;
          config.randomStartSpeed = new Vector2?(new Vector2(130f, 70f));
          config.randomLifeTime = new Vector2?(new Vector2(0.7f, 0.6f));
          config.randomizeColor = true;
          config.randomColorRange = 80;
          config.randomColorIndividualRange = 5;
          config.color = Color.Orange;
          config.accelerationY = 3f;
          Particle particle = new Particle(this.CurrentSimulation, config);
        }
      }
      for (int index = 0; index < 4 * amount; ++index)
      {
        if (Utility.GetBool(probability))
        {
          config.startSize = 10;
          config.randomStartSpeed = new Vector2?(new Vector2(100f, 50f));
          config.randomLifeTime = new Vector2?(new Vector2(0.7f, 0.6f));
          config.color = ColorManager.GetColor("lightslate");
          config.trail = false;
          config.randomColorRange = 30;
          config.randomColorIndividualRange = 0;
          Particle particle = new Particle(this.CurrentSimulation, config);
        }
      }
      for (int index = 0; index < 4 * amount / 4; ++index)
      {
        if (Utility.GetBool(probability))
        {
          config.startSize = 7;
          config.randomStartSpeed = new Vector2?(new Vector2(100f, 50f));
          config.randomLifeTime = new Vector2?(new Vector2(0.7f, 0.6f));
          config.color = ColorManager.GetColor("white");
          Particle particle = new Particle(this.CurrentSimulation, config);
        }
      }
      for (int index = 0; index < 2 * amount; ++index)
      {
        if (Utility.GetBool(probability))
        {
          config.startSize = 5;
          config.randomStartSpeed = new Vector2?(new Vector2(60f, 50f));
          config.randomLifeTime = new Vector2?(new Vector2(1.3f, 0.3f));
          config.color = ColorManager.GetColor("red");
          config.accelerationY = 3f;
          Particle particle = new Particle(this.CurrentSimulation, config);
        }
      }
    }

    private void SpawnFragments(
      Rectangle? playerRect,
      Rectangle? spritePos,
      Vector2[] possibleDirections)
    {
      if (!playerRect.HasValue)
        return;
      Rectangle rectangle1 = playerRect.Value;
      ParticleConfig config1 = new ParticleConfig()
      {
        startPos = new Vector2((float) rectangle1.X, (float) rectangle1.Y),
        startSize = rectangle1.Width / 2,
        texture = TextureManager.GetTexture("tileset"),
        fadeAway = true,
        trailSize = 0.3f,
        trail = true,
        trailColor = new Color(70, 70, 70, 100),
        accelerationY = 4f,
        color = Color.White,
        randomStartSpeed = new Vector2?(new Vector2(200f, 60f)),
        randomLifeTime = new Vector2?(new Vector2(0.8f, 0.4f)),
        randomSpinSpeed = new Vector2?(new Vector2(0.0f, 500f))
      };
      config1.startDirRandomness = (float) (90 / possibleDirections.Length);
      Rectangle rectangle2 = spritePos.Value;
      config1.startDir = possibleDirections[Utility.GetRandom(possibleDirections.Length - 1)];
      config1.spritePos = new Rectangle?(new Rectangle(rectangle2.X, rectangle2.Y, rectangle2.Width / 2, rectangle2.Height / 2));
      Particle particle1 = new Particle(this.CurrentSimulation, config1);
      ParticleConfig config2 = config1.Clone();
      config2.startDir = possibleDirections[Utility.GetRandom(possibleDirections.Length - 1)];
      config2.startPos += new Vector2((float) (rectangle1.Width / 2), 0.0f);
      config2.spritePos = new Rectangle?(new Rectangle(rectangle2.X + rectangle2.Width / 2, rectangle2.Y, rectangle2.Width / 2, rectangle2.Height / 2));
      Particle particle2 = new Particle(this.CurrentSimulation, config2);
      ParticleConfig config3 = config2.Clone();
      config3.startDir = possibleDirections[Utility.GetRandom(possibleDirections.Length - 1)];
      config3.startPos += new Vector2(0.0f, (float) (rectangle1.Height / 2));
      config3.spritePos = new Rectangle?(new Rectangle(rectangle2.X + rectangle2.Width / 2, rectangle2.Y + rectangle2.Height / 2, rectangle2.Width / 2, rectangle2.Height / 2));
      Particle particle3 = new Particle(this.CurrentSimulation, config3);
      ParticleConfig config4 = config3.Clone();
      config4.startDir = possibleDirections[Utility.GetRandom(possibleDirections.Length - 1)];
      config4.startPos -= new Vector2((float) (rectangle1.Width / 2), 0.0f);
      config4.spritePos = new Rectangle?(new Rectangle(rectangle2.X, rectangle2.Y + rectangle2.Height / 2, rectangle2.Width / 2, rectangle2.Height / 2));
      Particle particle4 = new Particle(this.CurrentSimulation, config4);
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ParticleSpawningObject
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal abstract class ParticleSpawningObject : GameObject
  {
    protected float particleStartDelay = 0.05f;
    protected bool particlesEnabled = true;
    private float disableTime = -1f;
    private bool hideInParticles;
    protected const int defaultMaxParticles = 64;
    private int maxParticles;
    private List<PrimitiveParticle> particles = new List<PrimitiveParticle>();
    protected ParticleConfig particleConfig;
    private float particleRandomOffset;
    protected bool particleIndividualRandomOffsets;
    private const float fadeOutTime = 0.15f;
    private float speedMultiplier;

    public bool IsDisabled => (double) this.disableTime != -1.0;

    protected virtual float ParticleTime => this.lifeTime;

    protected override bool ShowHitboxEnabled => !this.IsDisabled;

    public ParticleSpawningObject(
      Simulation simulation,
      float x,
      float y,
      int width,
      int height,
      Microsoft.Xna.Framework.Color? color = null,
      Texture2D texture = null,
      Microsoft.Xna.Framework.Rectangle? spritePos = null,
      bool hideInParticles = false,
      int maxParticles = 64,
      float speedMultiplier = 1f)
      : base(simulation, x, y, width, height, color, texture, spritePos)
    {
      this.hideInParticles = hideInParticles;
      this.maxParticles = maxParticles;
      this.speedMultiplier = speedMultiplier;
      this.particleRandomOffset = Utility.RandomizeNumber(0.0f, 1000f);
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (this.particles.Count == 0 && this.CurrentSimulation.RenderDefault)
        this.SetMaxParticles(this.maxParticles);
      if (!this.IsDisabled || (double) this.ParticleTime - (double) this.disableTime <= 0.15000000596046448)
        return;
      this.SetActive(false);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if (this.CurrentSimulation != null && this.CurrentSimulation.RenderDefault)
      {
        this.UpdateParticleConfig();
        int num = 0;
        float fadeMultiplier = this.IsDisabled ? 1f - MathHelper.Clamp((float) (((double) this.ParticleTime - (double) this.disableTime) / 0.15000000596046448), 0.0f, 1f) : 1f;
        float offsetMultiplier = this.IsDisabled ? (float) (1.0 + (1.0 - (double) fadeMultiplier) * 3.0) : 1f;
        for (int index = 0; index < this.particles.Count; ++index)
        {
          if (this.particles[index].TryDraw(spriteBatch, this.CurrentSimulation, this.ParticleTime, this.particleConfig.startPos, LayerDepths.Particle_Default + (float) index * 0.0001f, fadeMultiplier, offsetMultiplier))
            ++num;
        }
      }
      if (this.hideInParticles && !StateManager.IsState(GameState.LevelEditor) && !VarManager.IsOverclocked && VarManager.GetInt("particleamount") != 0)
        return;
      base.Draw(spriteBatch);
    }

    public override void SetActive(bool active)
    {
      if (active)
        this.disableTime = -1f;
      base.SetActive(active);
    }

    public void Disable()
    {
      if (this.IsDisabled)
        return;
      this.disableTime = this.ParticleTime;
    }

    protected virtual void UpdateParticleConfig() => this.particleConfig.startPos = this.Center;

    protected void SetMaxParticles(int maxParticles)
    {
      maxParticles *= VarManager.GetInt("particleamount");
      if (this.particles.Count == maxParticles || !this.CurrentSimulation.RenderDefault)
        return;
      this.maxParticles = maxParticles;
      if (this.particles.Count < maxParticles)
      {
        for (int count = this.particles.Count; count < maxParticles; ++count)
        {
          float randomOffset = this.particleIndividualRandomOffsets ? Utility.RandomizeNumber(0.0f, 1000f) : this.particleRandomOffset;
          this.particles.Add(new PrimitiveParticle(this.particleConfig, this.ParticleTime + (float) count * this.particleStartDelay, randomOffset, this.speedMultiplier));
        }
      }
      else
        this.particles.RemoveRange(maxParticles, this.particles.Count - maxParticles - 1);
    }
  }
}

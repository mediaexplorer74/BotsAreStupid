// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.PrimitiveParticle
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class PrimitiveParticle
  {
    public ParticleConfig config;
    public float startTime;
    public float maxLifeTime;
    private float randomOffset;
    private float speedMultiplier;

    public PrimitiveParticle(
      ParticleConfig config,
      float startTime,
      float randomOffset,
      float speedMultiplier = 1f)
    {
      this.config = config;
      this.startTime = startTime;
      this.maxLifeTime = Utility.TryGetRandom(config.lifeTime, config.randomLifeTime);
      this.randomOffset = randomOffset;
      this.speedMultiplier = speedMultiplier;
    }

    public bool TryDraw(
      SpriteBatch spriteBatch,
      Simulation simulation,
      float time,
      Vector2 basePosition,
      float layerDepth,
      float fadeMultiplier = 1f,
      float offsetMultiplier = 1f)
    {
      if (!this.IsActive(time))
        return false;
      int size1 = this.GetSize(time);
      Vector2 size2 = new Vector2((float) size1, (float) size1) * fadeMultiplier;
      Vector2 renderScale = Utility.CalculateRenderScale(size1, size1, this.config.SpriteSize, this.config.fadeIn, this.config.fadeAway, this.GetLifePercentage(time), fadeMultiplier);
      Utility.DrawTexture(spriteBatch, simulation, this.config.texture, this.GetPosition(basePosition, time, offsetMultiplier) - size2 * renderScale / 2f, this.config.spritePos, this.GetColor(time), layerDepth, size2, renderScale, this.GetRotation(time));
      return true;
    }

    public bool IsActive(float time)
    {
      if ((double) time < (double) this.startTime)
        return false;
      return this.config.maxRepeats == -1 || this.GetLifeIdx(time) < this.config.maxRepeats;
    }

    public float GetLifePercentage(float time)
    {
      return MathHelper.Clamp(this.NormalizedTime(time) / this.maxLifeTime, 0.0f, 1f);
    }

    public Vector2 GetPosition(Vector2 basePos, float time, float offsetMultiplier = 1f)
    {
      return basePos + Utility.RotateVector(this.config.startDir, this.config.startDirRandomness * 2f * Utility.StaticRandom(this.RandomX(time))) * (this.GetStartOffset(time) + this.GetSpeed(time) * this.NormalizedTime(time) * offsetMultiplier);
    }

    public float GetRotation(float time)
    {
      if (!this.config.randomSpinSpeed.HasValue)
        return 0.0f;
      float x = this.RandomX(time);
      float num = Utility.StaticTryGetRandom(0.0f, this.config.randomSpinSpeed, x, 0.1f) * (!this.config.randomSpinSpeedAllowNegative || (double) Utility.StaticRandom(x, 0.2f) <= 0.5 ? 1f : -1f);
      return time * num;
    }

    public int GetSize(float time)
    {
      return (int) Utility.StaticTryGetRandom((float) this.config.startSize, this.config.randomStartSize, this.RandomX(time), 0.3f);
    }

    private float NormalizedTime(float time) => (time - this.startTime) % this.maxLifeTime;

    private int GetLifeIdx(float time)
    {
      return (int) (((double) time - (double) this.startTime) / (double) this.maxLifeTime);
    }

    private float RandomX(float time)
    {
      return this.startTime + this.randomOffset + (float) this.GetLifeIdx(time);
    }

    public float GetSpeed(float time)
    {
      return Utility.StaticTryGetRandom(this.config.startSpeed, this.config.randomStartSpeed, this.RandomX(time), 0.4f) * this.speedMultiplier;
    }

    public float GetStartOffset(float time)
    {
      return Utility.StaticTryGetRandom(this.config.startOffset, this.config.randomStartOffset, this.RandomX(time), 0.5f);
    }

    private Color GetColor(float time)
    {
      Color color = this.config.color;
      if (this.config.randomizeColor)
      {
        float x = this.RandomX(time);
        int number = (int) Utility.StaticRandomizeNumber(0.0f, (float) Math.Max(this.config.randomColorRange, 1), x, 0.6f);
        color.R = (byte) MathHelper.Clamp((float) color.R + Utility.StaticRandomizeNumber((float) number, (float) this.config.randomColorIndividualRange, x, 0.7f), 0.0f, (float) byte.MaxValue);
        color.G = (byte) MathHelper.Clamp((float) color.G + Utility.StaticRandomizeNumber((float) number, (float) this.config.randomColorIndividualRange, x, 0.8f), 0.0f, (float) byte.MaxValue);
        color.B = (byte) MathHelper.Clamp((float) color.B + Utility.StaticRandomizeNumber((float) number, (float) this.config.randomColorIndividualRange, x, 0.9f), 0.0f, (float) byte.MaxValue);
      }
      return color;
    }
  }
}

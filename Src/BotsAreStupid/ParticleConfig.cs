// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ParticleConfig
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal class ParticleConfig
  {
    public Vector2 startPos;
    public Texture2D texture;
    public Rectangle? spritePos;
    public int startSize;
    public Vector2? randomStartSize;
    public bool fadeAway;
    public bool fadeIn;
    public bool trail;
    public Color trailColor;
    public float trailSize;
    public float trailInterval;
    public Vector2? randomTrailLifeTime;
    public int trailRandomColorRange;
    public bool round;
    public Vector2 startDir;
    public float startDirRandomness;
    public float startSpeed;
    public Vector2? randomStartSpeed;
    public Vector2? randomAccelerationX;
    public float accelerationY;
    public Vector2? randomAccelerationY;
    public float startOffset;
    public Vector2? randomStartOffset;
    public float randomness;
    public Color color;
    public bool randomizeColor;
    public int randomColorRange;
    public int randomColorIndividualRange;
    public float lifeTime;
    public Vector2? randomLifeTime;
    public Vector2? randomSpinSpeed;
    public bool randomSpinSpeedAllowNegative;
    public float? layerDepth;
    public int maxRepeats = -1;

    public Vector2 SpriteSize
    {
      get
      {
        ref Rectangle? local1 = ref this.spritePos;
        double x = local1.HasValue ? (double) local1.GetValueOrDefault().Width : (double) this.texture.Width;
        ref Rectangle? local2 = ref this.spritePos;
        double y = local2.HasValue ? (double) local2.GetValueOrDefault().Height : (double) this.texture.Height;
        return new Vector2((float) x, (float) y);
      }
    }

    public ParticleConfig Clone() => this.MemberwiseClone() as ParticleConfig;
  }
}

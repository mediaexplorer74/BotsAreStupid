// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Animation
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal class Animation
  {
    public AnimationFrame[] frames;
    public bool loop;
    public Vector2? renderOffset;

    public Animation(bool loop, Vector2? renderOffset, params AnimationFrame[] frames)
    {
      this.loop = loop;
      this.renderOffset = renderOffset;
      this.frames = frames;
    }
  }
}

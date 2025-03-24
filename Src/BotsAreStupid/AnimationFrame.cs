// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.AnimationFrame
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal struct AnimationFrame(
    float duration,
    Texture2D texture,
    Rectangle? spritePos,
    AnimationFrame.FrameFinishHandler finishHandler = null)
  {
    public float duration = duration;
    public Texture2D texture = texture;
    public Rectangle? spritePos = spritePos;
    public AnimationFrame.FrameFinishHandler FinishHandler = finishHandler;

    public delegate void FrameFinishHandler(GameObject obj);
  }
}

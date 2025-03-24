// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Animator
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class Animator : ICloneable
  {
    private Animation currentAnimation;
    private GameObject gameObject;
    private float time = 0.0f;
    private int currentFrameIndex = 0;

    public Animator(GameObject gameObject) => this.gameObject = gameObject;

    public object Clone() => (object) (Animator) this.MemberwiseClone();

    public void SetGameObject(GameObject gameObject) => this.gameObject = gameObject;

    public void Update(float deltaTime)
    {
      if (this.currentAnimation == null)
        return;
      AnimationFrame frame = this.currentAnimation.frames[this.currentFrameIndex];
      if (this.currentAnimation.loop || this.currentFrameIndex < this.currentAnimation.frames.Length - 1)
        this.time += deltaTime;
      if ((double) this.time <= (double) frame.duration)
        return;
      AnimationFrame.FrameFinishHandler finishHandler = frame.FinishHandler;
      if (finishHandler != null)
        finishHandler(this.gameObject);
      this.NextFrame();
    }

    public void Reset()
    {
      this.time = 0.0f;
      this.currentFrameIndex = 0;
      this.currentAnimation = (Animation) null;
    }

    public void SetAnimation(Animation animation) => this.currentAnimation = animation;

    public bool InAnimation(Animation animation) => animation == this.currentAnimation;

    private void NextFrame()
    {
      ++this.currentFrameIndex;
      if (this.currentFrameIndex == this.currentAnimation.frames.Length)
        this.currentFrameIndex = 0;
      this.time = 0.0f;
    }

    public Texture2D GetCurrentTexture()
    {
      return this.currentAnimation?.frames[this.currentFrameIndex].texture;
    }

    public Rectangle? GetCurrentSpritePos()
    {
      return (Rectangle?) this.currentAnimation?.frames[this.currentFrameIndex].spritePos;
    }

    public Vector2? GetCurrentRenderOffset() => (Vector2?) this.currentAnimation?.renderOffset;
  }
}

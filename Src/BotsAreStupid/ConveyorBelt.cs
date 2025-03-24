// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ConveyorBelt
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
  internal class ConveyorBelt : Platform
  {
    public const float defaultAnimationDuration = 0.1f;
    public const int maxSpeed = 10;
    private static Color hitboxColor = ColorManager.GetColor("orange");
    private const int defaultWidth = 168;
    private const int defaultHeight = 28;

    public int MoveSpeed { private set; get; }

    protected override bool ShowHitboxEnabled => VarManager.GetBool("showhitboxes_special");

    protected override Color HitboxColor
    {
      get
      {
        Player player = this.CurrentSimulation.Player;
        return (player != null ? (player.IsStandingOn((GameObject) this) ? 1 : 0) : 0) == 0 ? ConveyorBelt.hitboxColor : Utility.ChangeColor(ConveyorBelt.hitboxColor, -0.4f);
      }
    }

    public ConveyorBelt(
      Simulation simulation,
      float x,
      float y,
      int? speed = null,
      int width = 168,
      int height = 28)
      : base(simulation, x, y, width, height, Color.White, TextureManager.GetTexture("tileset"), new Rectangle?(new Rectangle(0, 128, 64, 10)))
    {
      this.MoveSpeed = speed ?? 2;
      this.defaultSize = new Vector2?(new Vector2(168f, 28f));
      this.CreateAnimation();
      this.availableParameters.Add(new ObjectParameter()
      {
        name = "Speed:",
        min = -10,
        max = 10,
        propertyName = nameof (MoveSpeed),
        gameObject = (GameObject) this,
        changeHandler = (GameObject.ParameterChangeHandler) (s => this.CreateAnimation())
      });
    }

    private void CreateAnimation()
    {
      Texture2D texture = TextureManager.GetTexture("tileset");
      float duration = 0.1f / (float) Math.Abs(this.MoveSpeed);
      List<AnimationFrame> animationFrameList = new List<AnimationFrame>();
      animationFrameList.Add(new AnimationFrame(duration, texture, new Rectangle?(new Rectangle(0, 128, 64, 10))));
      animationFrameList.Add(new AnimationFrame(duration, texture, new Rectangle?(new Rectangle(0, 144, 64, 10))));
      animationFrameList.Add(new AnimationFrame(duration, texture, new Rectangle?(new Rectangle(0, 160, 64, 10))));
      if (this.MoveSpeed < 0)
        animationFrameList.Reverse();
      this.SetAnimation(new Animation(true, new Vector2?(), animationFrameList.ToArray()));
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new ConveyorBelt(intoSimulation ?? this.CurrentSimulation, this.X, this.Y, new int?(this.MoveSpeed), this.Width, this.Height);
    }
  }
}

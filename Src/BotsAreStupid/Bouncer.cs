// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Bouncer
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class Bouncer : GameObject
  {
    private static Microsoft.Xna.Framework.Color hitboxColor = ColorManager.GetColor("white");
    private const int defaultBounceSpeed = 50;
    private const int defaultWidth = 28;
    private const int defaultHeight = 28;
    private float extendDuration = 0.15f;
    private const float baseExtendDuration = 0.15f;
    private const float retractDuration = 0.25f;
    private Microsoft.Xna.Framework.Rectangle padSpritePos;
    private float padStateTimeLeft;
    private Bouncer.PadState padState;
    private Vector2 forwardDirection;
    private const float previewDuration = 2f;
    private const float previewFadeTime = 0.25f;
    private float previewTimeLeft;
    private float previewTime;
    private float currentExtensionT;

    public int BounceSpeed { get; private set; }

    public bool CanDoBounce { get; private set; }

    public override bool HasCollision => true;

    protected override bool ShowHitboxEnabled => VarManager.GetBool("showhitboxes_special");

    protected override Microsoft.Xna.Framework.Color HitboxColor
    {
      get
      {
        Player player = this.CurrentSimulation.Player;
        return (player != null ? (player.IsStandingOn((GameObject) this) ? 1 : 0) : 0) == 0 ? Bouncer.hitboxColor : Utility.ChangeColor(Bouncer.hitboxColor, -0.4f);
      }
    }

    private float bounceMultiplier => (float) this.BounceSpeed / 100f;

    private float scaleMultiplier
    {
      get
      {
        return Utility.Remap(this.IsTipped ? (float) this.Width / 28f : (float) this.Height / 28f, 1f, 3f, 0.5f, 1f);
      }
    }

    private float extendDurationMultiplier
    {
      get
      {
        return Utility.Remap(this.scaleMultiplier * (1f - this.bounceMultiplier), 0.0f, 1f, 0.75f, 1.25f) * this.scaleMultiplier;
      }
    }

    private float bounceExtension
    {
      get
      {
        return (this.IsTipped ? (float) this.Width : (float) this.Height) * (float) (0.20000000298023224 + (double) this.bounceMultiplier * 0.60000002384185791);
      }
    }

    public Bouncer(
      Simulation simulation,
      float x,
      float y,
      int width = 28,
      int height = 28,
      float rotation = 0.0f,
      int? bounceSpeed = null)
      : base(simulation, x, y, width, height, new Microsoft.Xna.Framework.Color?(Microsoft.Xna.Framework.Color.White), TextureManager.GetTexture("tileset"), new Microsoft.Xna.Framework.Rectangle?(TextureManager.GetSpritePos("bouncer_base")), rotation)
    {
      this.layerDepth = LayerDepths.Bouncer_Base;
      this.defaultSize = new Vector2?(new Vector2(28f, 28f));
      this.MinScale = 1f;
      this.MaxScale = new float?(3f);
      this.padSpritePos = TextureManager.GetSpritePos("bouncer_pad");
      this.BounceSpeed = bounceSpeed ?? 50;
      this.availableParameters.Add(new ObjectParameter()
      {
        name = "Power:",
        min = 10,
        max = 100,
        propertyName = nameof (BounceSpeed),
        gameObject = (GameObject) this,
        changeHandler = (GameObject.ParameterChangeHandler) (v => this.previewTimeLeft = 2f)
      });
      this.OnRotate += new System.Action(this.UpdateForwardDirection);
      this.UpdateForwardDirection();
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new Bouncer(intoSimulation, this.X, this.Y, this.Width, this.Height, this.Rotation, new int?(this.BounceSpeed));
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      this.CanDoBounce = true;
      if ((double) this.previewTimeLeft > 0.0)
      {
        this.previewTimeLeft -= deltaTime;
        this.previewTime += deltaTime;
      }
      else
        this.previewTime = 0.0f;
      this.currentExtensionT = 0.0f;
      if (this.padState == 0)
        return;
      this.padStateTimeLeft -= deltaTime;
      if ((double) this.padStateTimeLeft <= 0.0)
      {
        if (this.padState == Bouncer.PadState.Extending)
        {
          this.padState = Bouncer.PadState.Retracting;
          this.padStateTimeLeft = 0.25f;
        }
        else if (this.padState == Bouncer.PadState.Retracting)
          this.padState = Bouncer.PadState.Idle;
      }
      if (this.padState == Bouncer.PadState.Extending)
        this.currentExtensionT = (float) (1.0 - (double) this.padStateTimeLeft / ((double) this.extendDuration * (double) this.extendDurationMultiplier));
      else if (this.padState == Bouncer.PadState.Retracting)
        this.currentExtensionT = this.padStateTimeLeft / 0.25f;
    }

    public bool TryBounce(bool vertical, PhysicsObject obj)
    {
      if (!this.CanBounce(vertical, obj))
        return false;
      this.Bounce(obj);
      return true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if (this.CurrentSimulation == null || this.CurrentSimulation.IsMain)
        base.Draw(spriteBatch);
      if (!this.enableDraw)
        return;
      Vector2 position1 = this.Position - this.forwardDirection * Utility.Interpolate(1f, this.bounceExtension, this.currentExtensionT);
      Vector2 spriteSize = this.SpriteSize;
      Utility.DrawTexture(spriteBatch, this.CurrentSimulation, this.texture, position1, new Microsoft.Xna.Framework.Rectangle?(this.padSpritePos), this.color, this.ScrambledLayerDepth - 0.01f, spriteSize, this.renderScale, this.Rotation, this.flipTexture);
      if ((double) this.previewTimeLeft > 0.0)
      {
        float num = -0.35f;
        if ((double) this.previewTime < 0.25)
          num = Utility.Lerp(-1f, num, this.previewTime / 0.25f);
        else if ((double) this.previewTimeLeft < 0.25)
          num = Utility.Lerp(-1f, num, this.previewTimeLeft / 0.25f);
        Vector2 position2 = this.Position - this.forwardDirection * Utility.Interpolate(1f, this.bounceExtension, 1f);
        Utility.DrawTexture(spriteBatch, this.CurrentSimulation, this.texture, position2, new Microsoft.Xna.Framework.Rectangle?(this.padSpritePos), Utility.ChangeColor(this.color, num, 0.1f), this.ScrambledLayerDepth - 0.02f, spriteSize, this.renderScale, this.Rotation, this.flipTexture);
      }
      if (VarManager.GetBool("showhitboxes_bouncer") && this.CurrentSimulation?.Player != null)
        Utility.DrawOutline(spriteBatch, this.GetCheckRectangle((GameObject) this.CurrentSimulation.Player), 1, Microsoft.Xna.Framework.Color.Magenta, 1f);
    }

    public override void DrawHitbox(SpriteBatch spriteBatch)
    {
      if (!this.ShowHitboxEnabled)
        return;
      base.DrawHitbox(spriteBatch);
      BorderInfo none = BorderInfo.None;
      float rotation = this.Rotation;
      if ((double) rotation != 90.0)
      {
        if ((double) rotation != 180.0)
        {
          if ((double) rotation == 270.0)
            none.left = true;
          else
            none.top = true;
        }
        else
          none.bottom = true;
      }
      else
        none.right = true;
      Utility.DrawOutline(spriteBatch, this.CollisionRectangle, 2, ColorManager.GetColor("orange"), this.ScrambledLayerDepth + 0.02f, new BorderInfo?(none));
    }

    public Microsoft.Xna.Framework.Rectangle GetCheckRectangle(GameObject forObj)
    {
      Microsoft.Xna.Framework.Rectangle collisionRectangle = this.CollisionRectangle;
      Vector2 vector2 = this.forwardDirection * (this.IsTipped ? (float) this.Width + (float) forObj.Width * 0.75f : (float) this.Height + (float) forObj.Height * 0.75f);
      collisionRectangle.X -= (int) vector2.X;
      collisionRectangle.Y -= (int) vector2.Y;
      int num = 4;
      if (this.IsTipped)
      {
        collisionRectangle.Height += 2 * num;
        collisionRectangle.Y -= num;
      }
      else
      {
        collisionRectangle.Width += 2 * num;
        collisionRectangle.X -= num;
      }
      return collisionRectangle;
    }

    public bool CanBounce(bool vertical, PhysicsObject obj)
    {
      if (vertical == this.IsTipped || !this.CanDoBounce || !obj.IntersectsRectangle(this.GetCheckRectangle((GameObject) obj)))
        return false;
      float rotation = this.Rotation;
      if ((double) rotation <= 90.0)
      {
        if ((double) rotation == 0.0)
          return (double) obj.Y + (double) obj.Height < (double) this.Y + 8.0;
        if ((double) rotation == 90.0)
          return (double) obj.X > (double) this.X + (double) this.Width - 8.0;
      }
      else
      {
        if ((double) rotation == 180.0)
          return (double) obj.Y > (double) this.Y + (double) this.Height - 8.0;
        if ((double) rotation == 270.0)
          return (double) obj.X + (double) obj.Width < (double) this.X + 8.0;
      }
      return false;
    }

    private void Bounce(PhysicsObject bouncedObject)
    {
      if (!this.CanDoBounce)
        return;
      float t = Utility.Remap(this.scaleMultiplier * this.bounceMultiplier, 0.0f, 1f, 0.2f, 0.8f);
      float num1 = Utility.Lerp(0.6f, 0.9f, this.bounceMultiplier);
      float num2 = 1300f * t;
      Vector2 velocity = bouncedObject.Velocity;
      if (this.IsTipped)
      {
        velocity.X *= -num1;
        float num3 = num2 - Math.Abs(velocity.X);
        if ((double) num3 > 0.0)
          velocity -= this.forwardDirection * num3;
      }
      else
      {
        velocity.Y *= -num1;
        float num4 = num2 - Math.Abs(velocity.Y);
        if ((double) num4 > 0.0)
          velocity -= this.forwardDirection * num4;
      }
      if (bouncedObject.IsHooked)
        bouncedObject.HookObject.SetAngularVelocity(velocity);
      else
        bouncedObject.SetVelocity(velocity);
      this.padState = Bouncer.PadState.Extending;
      this.padStateTimeLeft = this.extendDurationMultiplier * this.extendDuration;
      this.CanDoBounce = false;
      SoundManager.Play("bouncer", pitch: Utility.LerpClamped(-0.1f, 0.1f, t));
    }

    private void UpdateForwardDirection()
    {
      this.forwardDirection = Utility.RotateVector(Vector2.UnitY, this.Rotation);
    }

    private enum PadState
    {
      Idle,
      Extending,
      Retracting,
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Hook
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class Hook : ICloneable
  {
    private const float hookFriction = 0.06f;
    private const float extensionSpeed = 1697.05627f;
    private const float hookpullSpeed = 150f;
    private const float volume = 0.25f;
    private static readonly Microsoft.Xna.Framework.Rectangle hookSpritePos = new Microsoft.Xna.Framework.Rectangle(294, 50, 4, 7);
    private const float hookScale = 1.5f;
    private PhysicsObject gameObject;
    private float angularVelocity;
    private Vector2 hookPoint;
    private bool isExtending;
    private bool isRetracting;
    private bool isExtendingRight;
    private bool isExtendingDown;
    private Vector2 extension;
    private Vector2 extensionDirection;
    private Microsoft.Xna.Framework.Color color;
    private bool isWallJumping;
    private bool missed;
    private float rangeMultiplier = 1f;
    private bool isGhost;
    private static ParticleConfig attachParticleConfig = new ParticleConfig()
    {
      texture = TextureManager.GetCircleTexture(5),
      randomLifeTime = new Vector2?(new Vector2(0.5f, 0.3f)),
      randomStartSize = new Vector2?(new Vector2(6f, 2f)),
      startDir = Vector2.UnitY,
      startDirRandomness = 180f,
      color = ColorManager.GetColor("red"),
      fadeAway = true,
      randomStartSpeed = new Vector2?(new Vector2(60f, 40f)),
      maxRepeats = 1
    };

    public Simulation CurrentSimulation { get; private set; }

    public bool IsAttached { get; private set; }

    public float AngularVelocity => this.angularVelocity;

    public float ExtensionLength => this.MeasurementVector.Length();

    public double Angle => (this.MeasurementVector * new Vector2(1f, -1f)).GetDegrees();

    private Vector2 MeasurementVector
    {
      get => !this.IsAttached ? this.extension : this.hookPoint - this.gameObject.Center;
    }

    private float MaxExtension => (float) VarManager.GetInt("hookrange") * this.rangeMultiplier;

    public Hook(PhysicsObject gameObject)
    {
      this.SetGameObject(gameObject);
      this.color = ColorManager.GetColor("red");
      if (!this.isGhost)
        return;
      this.color = Utility.ChangeColor(this.color, -0.4f);
    }

    public void SetGameObject(PhysicsObject gameObject)
    {
      this.gameObject = gameObject;
      this.CurrentSimulation = gameObject.CurrentSimulation;
      this.isGhost = this.CurrentSimulation != null && !this.CurrentSimulation.IsMain && !this.CurrentSimulation.IsIntro;
    }

    public object Clone() => this.MemberwiseClone();

    public void Update(float deltaTime)
    {
      if (this.isExtending)
      {
        if ((double) this.ExtensionLength < (double) this.MaxExtension)
        {
          this.extension += this.extensionDirection * 1697.05627f * this.rangeMultiplier * deltaTime;
        }
        else
        {
          this.missed = true;
          this.Retract();
        }
      }
      else if (this.isRetracting)
      {
        Vector2 vector2 = Vector2.Normalize(this.extension) * 1697.05627f * 2f * deltaTime;
        float extensionLength = this.ExtensionLength;
        if ((double) extensionLength > 30.0 && (double) extensionLength > (double) vector2.Length())
        {
          this.extension -= vector2;
        }
        else
        {
          this.isRetracting = false;
          this.extension = Vector2.Zero;
        }
      }
      if (this.IsAttached || !this.isExtending && (!this.isRetracting || !this.missed))
        return;
      float? minDistToCollision = Utility.GetMinDistToCollision(this.CurrentSimulation, this.gameObject.Center, Vector2.Normalize(this.extension), typeof (Spike));
      int num;
      if (minDistToCollision.HasValue)
      {
        float? nullable = minDistToCollision;
        float extensionLength = this.ExtensionLength;
        num = (double) nullable.GetValueOrDefault() <= (double) extensionLength & nullable.HasValue ? 1 : 0;
      }
      else
        num = 0;
      if (num != 0)
        this.Attach(minDistToCollision.Value);
    }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.isExtending && (double)this.ExtensionLength > 0.0 && VarManager.GetBool("showhookpreview"))
            {
                Vector2 dir = Vector2.Normalize(this.extension);
                float? minDistToCollision = Utility.GetMinDistToCollision(this.CurrentSimulation, this.gameObject.Center, dir, typeof(Spike));
                Utility.DrawLine(spriteBatch, this.gameObject.Center, this.gameObject.Center + dir
                    * Math.Min((float)(minDistToCollision ?? float.MaxValue), this.MaxExtension), 2, new Microsoft.Xna.Framework.Color?(Utility.ChangeColor(Microsoft.Xna.Framework.Color.Magenta, -0.6f)), LayerDepths.HookDebug - 1f / 1000f, 0);
                if (minDistToCollision.HasValue && (double)minDistToCollision.Value <= (double)this.MaxExtension)
                {
                    Vector2 vector2 = this.gameObject.Center + dir * minDistToCollision.Value;
                    Utility.DrawLine(spriteBatch, vector2 + Vector2.UnitY * 3f, vector2 - Vector2.UnitY * 4f, color: new Microsoft.Xna.Framework.Color?(Microsoft.Xna.Framework.Color.Magenta), layerDepth: LayerDepths.HookDebug);
                    Utility.DrawLine(spriteBatch, vector2 + Vector2.UnitX * 4f, vector2 - Vector2.UnitX * 3f, color: new Microsoft.Xna.Framework.Color?(Microsoft.Xna.Framework.Color.Magenta), layerDepth: LayerDepths.HookDebug);
                }
            }
            if (this.IsAttached)
            {
                Utility.DrawLine(spriteBatch, this.gameObject.Center, this.hookPoint, 3, new Microsoft.Xna.Framework.Color?(this.color), LayerDepths.HookRope - (this.isGhost ? 0.01f : 0.0f), (int)((double)Hook.hookSpritePos.Height * 1.5));
                this.DrawHook(spriteBatch, this.hookPoint);
            }
            else
            {
                if (!this.isExtending && !this.isRetracting || (double)this.ExtensionLength <= 0.0)
                    return;
                Vector2 center = this.gameObject.Center;
                Utility.DrawLine(spriteBatch, center, center + this.extension, 3, new Microsoft.Xna.Framework.Color?(this.color), LayerDepths.HookRope - (this.isGhost ? 0.01f : 0.0f), 0);
                this.DrawHook(spriteBatch, center + this.extension);
            }
        }

    private void DrawHook(SpriteBatch spriteBatch, Vector2 pos)
    {
      Vector2 vector2 = this.gameObject.Center - pos;
      float num = (float) (Math.Atan2((double) vector2.Y, (double) vector2.X) + Math.PI / 2.0);
      spriteBatch.Draw(TextureManager.GetTexture("tileset"), pos, new Microsoft.Xna.Framework.Rectangle?(Hook.hookSpritePos), Microsoft.Xna.Framework.Color.White, num, new Vector2((float) Hook.hookSpritePos.Width, (float) (Hook.hookSpritePos.Height + 2)) / 2f * 1.5f, Vector2.One * 1.5f, SpriteEffects.None, LayerDepths.HookEnd);
    }

    public void UpdatePosition(
      float deltaTime,
      out float newX,
      out float newY,
      float oldX,
      float oldY)
    {
      Vector2 vector2_1 = this.hookPoint - this.gameObject.Center;
      this.AddAngularVelocity(new Vector2(0.0f, (float) VarManager.GetInt("gravity") * deltaTime));
      this.angularVelocity -= this.angularVelocity * 0.06f * deltaTime;
      Vector2 vector2_2 = this.hookPoint + Utility.RotateVector(this.gameObject.Center - this.hookPoint, (float) ((double) this.angularVelocity * 180.0 / Math.PI) * deltaTime) - new Vector2((float) this.gameObject.Width, (float) this.gameObject.Height) / 2f;
      newX = vector2_2.X;
      newY = vector2_2.Y;
      this.gameObject.SetVelocity((newX - oldX) / deltaTime, (newY - oldY) / deltaTime);
      if (!this.isWallJumping || !(this.gameObject is Player))
        return;
      (this.gameObject as Player).WallJump();
      this.isWallJumping = false;
    }

    public void AddAngularVelocity(Vector2 velocity)
    {
      Vector2 vector2 = this.hookPoint - this.gameObject.Center;
      this.angularVelocity += (float) ((-(double) vector2.X * (double) velocity.Y - -(double) vector2.Y * (double) velocity.X) / ((double) vector2.X * (double) vector2.X + (double) vector2.Y * (double) vector2.Y));
    }

    public void SetAngularVelocity(Vector2 velocity)
    {
      Vector2 vector2 = this.hookPoint - this.gameObject.Center;
      this.angularVelocity = (float) ((-(double) vector2.X * (double) velocity.Y - -(double) vector2.Y * (double) velocity.X) / ((double) vector2.X * (double) vector2.X + (double) vector2.Y * (double) vector2.Y));
    }

    public void FixPosition(
      ref float v,
      bool isX,
      GameObject collision,
      Vector2 startVelocity,
      bool doBounce)
    {
      Microsoft.Xna.Framework.Rectangle collisionRectangle = collision.CollisionRectangle;
      v = !(isX ? (double) startVelocity.X > 0.0 : (double) startVelocity.Y > 0.0) ? (isX ? (float) (collisionRectangle.X + collisionRectangle.Width) : (float) (collisionRectangle.Y + collisionRectangle.Height)) : (isX ? (float) (collisionRectangle.X - this.gameObject.Width) : (float) (collisionRectangle.Y - this.gameObject.Height));
      if (doBounce)
      {
        if (!(collision is Bouncer bouncer))
          return;
        bouncer.TryBounce(!isX, this.gameObject);
      }
      else
      {
        this.angularVelocity = 0.0f;
        if (isX)
          this.isWallJumping = true;
        else if ((double) startVelocity.Y > 0.0 && (double) Math.Abs(this.angularVelocity) < 25.0)
        {
          this.Use(retract: true);
          Vector2 vector2 = this.hookPoint - this.gameObject.Position;
          vector2.Normalize();
          this.gameObject.Accelerate(vector2 * 150f);
        }
      }
    }

    public void Use(bool? right = null, bool retract = false, bool down = false)
    {
      if (this.IsAttached)
      {
        this.IsAttached = false;
        this.gameObject.HasGravity = true;
        this.missed = false;
        this.Retract(new Vector2?(this.hookPoint));
      }
      else if (this.isExtending)
      {
        this.missed = true;
        this.Retract();
      }
      else
      {
        if (retract)
          return;
        this.missed = false;
        this.Extend(right, down);
      }
    }

    private void Extend(bool? right = null, bool down = false)
    {
      this.isExtending = true;
      this.isExtendingRight = right.HasValue ? right.Value : (double) this.gameObject.Velocity.X >= 0.0;
      this.isExtendingDown = down;
      this.extensionDirection = new Vector2(this.isExtendingRight ? 1f : -1f, this.isExtendingDown ? 1f : -1f);
      this.extensionDirection.Normalize();
      this.extension = Vector2.Zero;
    }

    private void Retract(Vector2? from = null)
    {
      if (from.HasValue)
      {
        this.extension = from.Value - this.gameObject.Center;
        SoundManager.Play("hookdetach", 0.25f);
      }
      this.isExtending = false;
      this.isRetracting = true;
      this.isWallJumping = false;
    }

    private void Attach(float hookDistance)
    {
      if ((double) hookDistance <= 0.0)
        hookDistance = 0.01f;
      Vector2 vector2 = this.extensionDirection * hookDistance;
      this.hookPoint = this.gameObject.Center + vector2;
      this.angularVelocity = (float) (((double) vector2.X * (double) this.gameObject.Velocity.X - (double) vector2.Y * (double) this.gameObject.Velocity.Y) / ((double) vector2.X * (double) vector2.X + (double) vector2.Y * (double) vector2.Y)) * (float) Math.Sign(this.extensionDirection.X) * (float) Math.Sign(this.extensionDirection.Y);
      this.IsAttached = true;
      this.isExtending = false;
      this.gameObject.HasGravity = false;
      SoundManager.Play("hookattach", 0.25f);
      if (this.CurrentSimulation.CheckMode)
        return;
      int num = VarManager.GetInt("particleamount");
      Hook.attachParticleConfig.startPos = this.hookPoint;
      if (num > 0)
      {
        ParticleGroup particleGroup = new ParticleGroup(this.CurrentSimulation, this.hookPoint, Hook.attachParticleConfig, 8 * num);
      }
    }

    public void Boost(float amount)
    {
      this.angularVelocity += (double) this.angularVelocity > 0.0 ? amount : -amount;
    }

    public void SetRangeMultiplier(float value) => this.rangeMultiplier = value;
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.PhysicsObject
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
  internal class PhysicsObject : GameObject
  {
    private Vector2 previousVelocityDir;
    private int previousHookVelocitySign;
    protected Hook hook;
    protected bool isBeeingMoved;
    private static ParticleConfig boostParticleConfig = new ParticleConfig()
    {
      randomStartSize = new Vector2?(new Vector2(7f, 1f)),
      lifeTime = 1.6f,
      spritePos = new Microsoft.Xna.Framework.Rectangle?(TextureManager.GetSpritePos("booster")),
      fadeAway = true,
      texture = TextureManager.GetTexture("tileset"),
      color = Microsoft.Xna.Framework.Color.White,
      randomSpinSpeed = new Vector2?(new Vector2(1200f, 300f)),
      randomSpinSpeedAllowNegative = true,
      randomizeColor = true,
      randomColorRange = 32,
      randomColorIndividualRange = 4,
      maxRepeats = 1
    };
    private List<(Vector2, PrimitiveParticle)> boostParticles = new List<(Vector2, PrimitiveParticle)>();
    private float boostTime = 0.0f;
    private const float boostTimeMax = 0.6f;
    private float particleCooldown;
    private const float particleCooldownMax = 0.02f;
    private const float friction = 0.1f;
    private GameObject standingOn;
    protected float fallTime = 0.0f;

    public bool IsGrounded { get; set; }

    public bool HasGravity { get; set; } = true;

    public bool IsBoosted => (double) this.boostTime > 0.0;

    public bool IsHooked => this.hook.IsAttached;

    public Hook HookObject => this.hook;

    public float RoundedVelocityMagnitude => (float) Math.Round((double) this.Velocity.Length(), 2);

    public float RealVelocityMagnitude
    {
      get
      {
        return (float) Math.Round((double) this.Velocity.Length() * (double) Utility.pixelsToMeters, 3);
      }
    }

    public Vector2 RealVelocity => this.Velocity * Utility.pixelsToMeters;

    public bool HasFlippedVelocity
    {
      get
      {
        return this.previousHookVelocitySign != 0 && Math.Sign(this.hook.AngularVelocity) + this.previousHookVelocitySign == 0 || (double) Vector2.Dot(this.previousVelocityDir, Vector2.Normalize(this.Velocity)) < -0.949999988079071;
      }
    }

    protected int ConveyorSpeed
    {
      get
      {
        return !this.IsGrounded || !(this.standingOn is ConveyorBelt) ? 0 : ((ConveyorBelt) this.standingOn).MoveSpeed * VarManager.GetInt("conveyormultiplier");
      }
    }

    public PhysicsObject(
      Simulation simulation,
      float x,
      float y,
      int width,
      int height,
      Microsoft.Xna.Framework.Color color,
      Texture2D texture = null,
      Microsoft.Xna.Framework.Rectangle? spritePos = null)
      : base(simulation, x, y, width, height, new Microsoft.Xna.Framework.Color?(color), texture, spritePos)
    {
      this.hook = new Hook(this);
    }

    public override object Clone()
    {
      PhysicsObject gameObject = (PhysicsObject) base.Clone();
      Hook hook = (Hook) this.hook.Clone();
      gameObject.hook = hook;
      hook.SetGameObject(gameObject);
      gameObject.boostParticles = new List<(Vector2, PrimitiveParticle)>((IEnumerable<(Vector2, PrimitiveParticle)>) this.boostParticles);
      return (object) gameObject;
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      this.previousVelocityDir = Vector2.Normalize(this.Velocity);
      this.previousHookVelocitySign = this.hook.IsAttached ? Math.Sign(this.hook.AngularVelocity) : 0;
      if (this.HasGravity)
        this.Accelerate(0.0f, (float) VarManager.GetInt("gravity") * deltaTime);
      if (!this.isBeeingMoved && this.IsGrounded)
        this.SetVelocity(this.Velocity.X * (float) (1.0 - 0.10000000149011612 * (double) deltaTime * 128.0), this.Velocity.Y);
      if ((double) this.particleCooldown > 0.0)
        this.particleCooldown -= deltaTime;
      if ((double) this.boostTime > 0.0)
      {
        this.boostTime -= deltaTime;
        if ((double) this.particleCooldown <= 0.0)
        {
          if (!this.CurrentSimulation.CheckMode)
            this.boostParticles.Add((this.Position + new Vector2((float) (this.Width / 2), (float) (this.Height / 5 * 4)), new PrimitiveParticle(PhysicsObject.boostParticleConfig, this.CurrentSimulation.SimulationTime, this.lifeTime * this.lifeTime)));
          this.particleCooldown = 0.02f / (float) VarManager.GetInt("particleamount");
        }
      }
      this.hook.Update(deltaTime);
      bool isGrounded = this.IsGrounded;
      this.UpdatePosition(deltaTime);
      if (isGrounded)
        return;
      if (this.IsGrounded)
      {
        this.OnLanding(this.fallTime);
        this.fallTime = 0.0f;
      }
      else if ((double) this.Velocity.Y > 0.0)
        this.fallTime += deltaTime;
      else
        this.fallTime = 0.0f;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      this.hook.Draw(spriteBatch);
      for (int index = 0; index < this.boostParticles.Count; ++index)
      {
        (Vector2, PrimitiveParticle) boostParticle = this.boostParticles[index];
        if (!boostParticle.Item2.TryDraw(spriteBatch, this.CurrentSimulation, this.CurrentSimulation.SimulationTime, boostParticle.Item1, LayerDepths.Particle_BoostTrail + (float) index * 0.0001f))
          this.boostParticles.Remove(boostParticle);
      }
      base.Draw(spriteBatch);
    }

    public void Boost(Booster b)
    {
      if (this.hook.IsAttached)
        this.hook.Boost(b.AngularBoostSpeed);
      else if ((double) this.Velocity.Length() > 1.0)
      {
        Vector2 velocity = this.Velocity;
        velocity.Normalize();
        this.Accelerate(velocity * (float) b.BoostSpeed);
      }
      this.boostTime = 0.6f;
    }

    public bool IsStandingOn(GameObject obj) => this.standingOn == obj;

    protected virtual void OnLanding(float fallTime) => SoundManager.Play("landing", 0.3f);

    protected GameObject GetCollidingObject(bool vertical, out bool doBounce, bool debug)
    {
      doBounce = false;
      GameObject collidingObject = (GameObject) null;
      float num = -1f;
      if (debug)
        Console.WriteLine("Starting check...");
      List<GameObject> allByType = this.CurrentSimulation.GetAllByType(typeof (Bouncer));
      for (int index = allByType.Count - 1; index >= 0; --index)
      {
        GameObject gameObject = allByType[index];
        float penetrationDepth;
        if (gameObject.IsActive && gameObject is Bouncer && gameObject.IsCollidingWith((GameObject) this, out penetrationDepth))
        {
          if (debug)
            Console.WriteLine("Found: " + gameObject.GetType()?.ToString() + "#" + gameObject.Id.ToString() + " [" + penetrationDepth.ToString() + "/" + num.ToString() + "] b:" + ((gameObject as Bouncer).CanBounce(vertical, this) ? "yes" : "no"));
          if ((double) penetrationDepth > (double) num && (gameObject as Bouncer).CanBounce(vertical, this))
          {
            collidingObject = gameObject;
            num = penetrationDepth;
          }
        }
      }
      foreach (GameObject collisionObject in this.CurrentSimulation.GetCollisionObjects())
      {
        float penetrationDepth;
        if (collisionObject.IsActive && collisionObject.IsCollidingWith((GameObject) this, out penetrationDepth))
        {
          if (debug)
            Console.WriteLine("Found: " + collisionObject.GetType()?.ToString() + "#" + collisionObject.Id.ToString() + " [" + penetrationDepth.ToString() + "/" + num.ToString() + "]");
          if ((double) penetrationDepth > (double) num)
          {
            if (debug)
              Console.WriteLine("=> Choice: " + collisionObject.GetType()?.ToString() + "#" + collisionObject.Id.ToString());
            return collisionObject;
          }
        }
      }
      if (collidingObject == null)
        return (GameObject) null;
      doBounce = true;
      if (debug)
        Console.WriteLine("=> Choice: " + collidingObject.GetType()?.ToString() + "#" + collidingObject.Id.ToString());
      return collidingObject;
    }

    private void UpdatePosition(float deltaTime)
    {
      float x = this.X;
      float y = this.Y;
      int conveyorSpeed = this.ConveyorSpeed;
      if (conveyorSpeed > 0 && (double) this.Velocity.X < (double) conveyorSpeed || conveyorSpeed < 0 && (double) this.Velocity.X > (double) conveyorSpeed)
        this.Accelerate((float) (conveyorSpeed * VarManager.GetInt("conveyoracceleration")) * deltaTime, 0.0f);
      float newX;
      float newY;
      if (this.hook.IsAttached)
      {
        this.hook.UpdatePosition(deltaTime, out newX, out newY, x, y);
        this.flipTexture = (double) this.Velocity.X < 0.0;
      }
      else
      {
        newX = this.X + this.Velocity.X * deltaTime;
        newY = this.Y + this.Velocity.Y * deltaTime;
      }
      this.SetAndCheckPosition(newX, newY);
    }

    private void SetAndCheckPosition(float newX, float newY)
    {
      Vector2 velocity = this.Velocity;
      bool debug = false;
      this.X = newX;
      float v1 = this.X;
      bool doBounce;
      GameObject collidingObject1 = this.GetCollidingObject(false, out doBounce, debug);
      bool flag = false;
      if (collidingObject1 != null)
      {
        if (debug)
          Console.WriteLine("[Sim:" + this.CurrentSimulation.PlayerName + "] Horizontal Collision with " + collidingObject1.GetType()?.ToString() + "#" + collidingObject1.Id.ToString() + " - [" + velocity.X.ToString() + "/" + velocity.Y.ToString() + "] => [" + this.Velocity.X.ToString() + "/" + this.Velocity.Y.ToString() + "]");
        Microsoft.Xna.Framework.Rectangle collisionRectangle = collidingObject1.CollisionRectangle;
        if (this.hook.IsAttached)
        {
          this.hook.FixPosition(ref v1, true, collidingObject1, velocity, doBounce);
        }
        else
        {
          if (this.IsGrounded && (double) this.Y + (double) this.Height - (double) collisionRectangle.Y <= (double) (this.Height / 3))
          {
            newY = (float) (collisionRectangle.Y - this.Height);
            flag = true;
            if (debug)
              Console.WriteLine("[Sim:" + this.CurrentSimulation.PlayerName + "] Performed a step");
          }
          v1 = (double) velocity.X <= 0.0 ? (float) (collisionRectangle.X + collisionRectangle.Width) : (float) (collisionRectangle.X - this.Width);
          if (doBounce)
          {
            if (collidingObject1 is Bouncer bouncer)
              bouncer.TryBounce(false, this);
          }
          else if (!flag)
            this.SetVelocity(0.0f, this.Velocity.Y);
        }
      }
      if (!flag)
        this.X = v1;
      this.IsGrounded = flag;
      this.standingOn = flag ? collidingObject1 : (GameObject) null;
      float y = this.Y;
      this.Y = newY;
      float v2 = this.Y;
      velocity = this.Velocity;
      GameObject collidingObject2 = this.GetCollidingObject(true, out doBounce, debug);
      if (collidingObject2 != null)
      {
        if (debug)
          Console.WriteLine("[Sim:" + this.CurrentSimulation.PlayerName + "] Vertical Collision with " + collidingObject2.GetType()?.ToString() + "#" + collidingObject2.Id.ToString() + " - [" + velocity.X.ToString() + "/" + velocity.Y.ToString() + "] => [" + this.Velocity.X.ToString() + "/" + this.Velocity.Y.ToString() + "]");
        Microsoft.Xna.Framework.Rectangle collisionRectangle = collidingObject2.CollisionRectangle;
        if (this.hook.IsAttached)
        {
          this.hook.FixPosition(ref v2, false, collidingObject2, velocity, doBounce);
        }
        else
        {
          if ((double) velocity.Y > 0.0)
          {
            if (flag)
            {
              this.X = v1;
              this.SetVelocity(0.0f, 0.0f);
              v2 = y;
            }
            else
            {
              v2 = (float) (collisionRectangle.Y - this.Height);
              this.IsGrounded = true;
              this.standingOn = collidingObject2;
            }
          }
          else
            v2 = (float) (collisionRectangle.Y + collisionRectangle.Height);
          if (doBounce)
          {
            if (collidingObject2 is Bouncer bouncer)
              bouncer.TryBounce(true, this);
          }
          else
            this.SetVelocity(this.Velocity.X, 0.0f);
        }
      }
      this.Y = v2;
      if (this.IsHooked || (double) this.Y <= (double) (Utility.VirtualHeight + this.Height))
        return;
      if (this is Player)
        (this as Player).Kill();
      else
        this.Destroy();
    }
  }
}

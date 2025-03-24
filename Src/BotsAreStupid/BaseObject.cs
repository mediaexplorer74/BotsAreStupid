// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.BaseObject
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace BotsAreStupid
{
  [Serializable]
  internal abstract class BaseObject : ICloneable
  {
    public Simulation CurrentSimulation;
    protected Rectangle savedRectangle;
    protected RectangleF? savedRectanglePrecise;
    private Vector2 position;
    protected (int, int) actualSize;
    private float rotation = 0.0f;
    protected float lifeTime;
    private bool isDead = false;
    private Simulation preDestroyedSimulation;

    public int Id { get; protected set; }

    public bool IsActive { private set; get; } = true;

    public float LifeTime => this.lifeTime;

    public bool IsDead => this.isDead;

    public float X
    {
      get => this.position.X;
      protected set
      {
        this.position.X = value;
        this.UpdateRectangle();
      }
    }

    public float Y
    {
      get => this.position.Y;
      protected set
      {
        this.position.Y = value;
        this.UpdateRectangle();
      }
    }

    public Vector2 Velocity { private set; get; }

    public Vector2 Position => new Vector2(this.X, this.Y);

    public Vector2 Size => new Vector2((float) this.Width, (float) this.Height);

    public Vector2 Center => this.Position + this.Size / 2f;

    public virtual Rectangle Rectangle => this.savedRectangle;

    public RectangleF RectanglePrecise => this.savedRectanglePrecise ?? (RectangleF) this.Rectangle;

    public virtual int Width
    {
      get => this.actualSize.Item1;
      set
      {
        this.actualSize.Item1 = value;
        this.UpdateRectangle();
      }
    }

    public virtual int Height
    {
      get => this.actualSize.Item2;
      set
      {
        this.actualSize.Item2 = value;
        this.UpdateRectangle();
      }
    }

    public float Rotation
    {
      protected set
      {
        this.rotation = (float) (((double) value + 360.0) % 360.0);
        this.IsTipped = (double) this.rotation == 90.0 || (double) this.rotation == 270.0;
        this.UpdateRectangle();
      }
      get => this.rotation;
    }

    public virtual bool IsRotatable => true;

    public bool IsTipped { get; private set; }

    public virtual float LayerDepth => 0.5f;

    public float ScrambledLayerDepth
    {
      get
      {
        return this.LayerDepth + MathHelper.Clamp((float) ((double) this.lifeTime / 1000000.0 
            + (double) this.Id / 1000000.0), 0.0f, 0.1f);
      }
    }

    protected abstract Texture2D RenderTexture { get; }

    protected abstract Color RenderColor { get; }

    public abstract Rectangle? RenderSpritePos { get; }

    protected virtual Vector2 RenderPosition => this.Position;

    protected virtual Vector2 SpriteSize
    {
      get
      {
        Rectangle? renderSpritePos = this.RenderSpritePos;
        ref Rectangle? local1 = ref renderSpritePos;
        double x = local1.HasValue ? (double) local1.GetValueOrDefault().Width : (double) this.RenderTexture.Width;
        renderSpritePos = this.RenderSpritePos;
        ref Rectangle? local2 = ref renderSpritePos;
        double y = local2.HasValue ? (double) local2.GetValueOrDefault().Height : (double) this.RenderTexture.Height;
        return new Vector2((float) x, (float) y);
      }
    }

    protected virtual Vector2 RenderScale => Vector2.One;

    protected virtual bool RenderFlipped => false;

    protected virtual bool ShowHitboxEnabled => false;

    protected virtual Color HitboxColor => Color.Transparent;

    protected virtual Rectangle HitboxRectangle => this.Rectangle;

    public event System.Action OnDestroy;

    public BaseObject(
      Simulation simulation,
      float x,
      float y,
      int width,
      int height = -1,
      float rotation = 0.0f)
    {
      this.CurrentSimulation = simulation ?? SimulationManager.MainSimulation;
      this.Id = this.CurrentSimulation.RegisterObject(this);
      this.X = x;
      this.Y = y;
      this.Rotation = rotation;
      this.Width = width;
      this.Height = height == -1 ? width : height;
    }

    public virtual object Clone() => this.MemberwiseClone();

    public virtual void Destroy()
    {
      if (this.isDead)
        return;
      this.SetActive(false);
      this.isDead = true;
      this.preDestroyedSimulation = this.CurrentSimulation;
      this.CurrentSimulation.RemoveObject(this);
      System.Action onDestroy = this.OnDestroy;
      if (onDestroy != null)
        onDestroy();
    }

    public void Revive()
    {
      if (!this.isDead)
        return;
      this.Id = this.preDestroyedSimulation.RegisterObject(this);
      this.SetActive(true);
      this.isDead = false;
      this.lifeTime = 0.0f;
    }

    public virtual void SetActive(bool value) => this.IsActive = value;

    public virtual void Update(float deltaTime) => this.lifeTime += deltaTime;

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      if (this.RenderTexture == null)
        return;
      Utility.DrawTexture(spriteBatch, this.CurrentSimulation, this.RenderTexture, this.RenderPosition, this.RenderSpritePos, this.RenderColor, this.ScrambledLayerDepth, this.SpriteSize, this.RenderScale, this.Rotation, this.RenderFlipped);
    }

    public virtual void DrawHitbox(SpriteBatch spriteBatch)
    {
      if (!this.ShowHitboxEnabled || this.RenderColor == Color.Transparent)
        return;
      Utility.DrawOutline(spriteBatch, this.HitboxRectangle, 1, this.HitboxColor, this.ScrambledLayerDepth + 0.01f);
    }

    protected virtual void UpdateRectangle()
    {
      this.CacheRectangle(new Rectangle((int) this.X, (int) this.Y, this.Width, this.Height));
    }

    protected virtual void CacheRectangle(Rectangle r) => this.savedRectangle = r;

    protected void CacheRectanglePrecise(RectangleF? r = null)
    {
      this.savedRectanglePrecise = new RectangleF?(r ?? new RectangleF(this.X, this.Y, (float) this.Width, (float) this.Height));
    }

    public void SetPosition(Vector2 pos)
    {
      this.X = pos.X;
      this.Y = pos.Y;
    }

    public void SetPosition(float x, float y)
    {
      this.X = x;
      this.Y = y;
    }

    public void Translate(Vector2 v)
    {
      if (!float.IsNaN(v.X))
        this.X += v.X;
      if (float.IsNaN(v.Y))
        return;
      this.Y += v.Y;
    }

    public void SetVelocity(Vector2 velocity) => this.Velocity = velocity;

    public void SetVelocity(float x, float y) => this.Velocity = new Vector2(x, y);

    public void Accelerate(Vector2 amount) => this.Velocity += amount;

    public void Accelerate(float x, float y) => this.Velocity += new Vector2(x, y);

    public event System.Action OnRotate;

    public void SetRotation(float rotation)
    {
      this.Rotation = rotation;
      System.Action onRotate = this.OnRotate;
      if (onRotate == null)
        return;
      onRotate();
    }

    public void Rotate(float angle)
    {
      if (!this.IsRotatable)
        return;
      this.Rotation += angle;
      if ((double) this.Rotation > 360.0)
        this.Rotation -= 360f;
      if ((double) this.Rotation < 0.0)
        this.Rotation += 360f;
      System.Action onRotate = this.OnRotate;
      if (onRotate != null)
        onRotate();
    }

    public void RotateAround(Vector2 pivot, float angle)
    {
      Vector2 vector2 = Utility.RotateVector(this.Center - pivot, angle);
      this.SetPosition(pivot + vector2 - new Vector2((float) this.Width, (float) this.Height) / 2f);
      this.Rotate(angle);
    }
  }
}

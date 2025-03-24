// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.GameObject
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class GameObject : BaseObject
  {
    protected float layerDepth = LayerDepths.GameObject_Default;
    protected Texture2D texture;
    protected Microsoft.Xna.Framework.Color color;
    protected Microsoft.Xna.Framework.Rectangle? spritePos;
    protected Vector2 renderScale = Vector2.One;
    protected bool flipTexture;
    protected bool isRotatable = true;
    protected Vector2? defaultSize;
    protected Animator animator;
    protected bool enableDraw;
    private float collisionHeightPercent = 1f;
    private Microsoft.Xna.Framework.Rectangle collisionRect;
    protected bool checkPlayerCollision;

    public override float LayerDepth => this.layerDepth;

    protected override Texture2D RenderTexture
    {
      get => this.animator?.GetCurrentTexture() ?? this.texture;
    }

    protected override Microsoft.Xna.Framework.Color RenderColor => this.color;

    public override Microsoft.Xna.Framework.Rectangle? RenderSpritePos
    {
      get => (Microsoft.Xna.Framework.Rectangle?) this.animator?.GetCurrentSpritePos() ?? this.spritePos;
    }

    protected override Vector2 RenderPosition
    {
      get
      {
        return base.RenderPosition + ((Vector2?) this.animator?.GetCurrentRenderOffset() ?? Vector2.Zero);
      }
    }

    protected override Vector2 RenderScale => this.renderScale;

    protected override bool RenderFlipped => this.flipTexture;

    public override bool IsRotatable => this.isRotatable;

    protected override Microsoft.Xna.Framework.Rectangle HitboxRectangle => this.CollisionRectangle;

    public override int Width
    {
      get => !this.IsTipped ? base.Width : base.Height;
      set
      {
        if (this.IsTipped)
          base.Height = value;
        else
          base.Width = value;
      }
    }

    public override int Height
    {
      get => !this.IsTipped ? base.Height : base.Width;
      set
      {
        if (this.IsTipped)
          base.Width = value;
        else
          base.Height = value;
      }
    }

    public Microsoft.Xna.Framework.Rectangle CollisionRectangle => this.collisionRect;

    public virtual bool HasCollision => false;

    public bool HasGeneratedTexture { private set; get; }

    public float CollisionHeightPercent
    {
      get => this.collisionHeightPercent;
      protected set
      {
        this.collisionHeightPercent = value;
        this.UpdateRectangle();
      }
    }

    public bool IsScalable { get; protected set; } = true;

    public bool IsSelectable { get; protected set; } = true;

    public bool IsSerializable
    {
      get
      {
        switch (this)
        {
          case Player _:
          case Booster _:
          case EnergyOrb _:
          case TextObject _:
          case ParticleTrail _:
          case Bouncer _:
            return true;
          default:
            return this is MovingPlatform;
        }
      }
    }

    public bool IsEssentialPerSimulation
    {
      get
      {
        switch (this)
        {
          case EnergyOrb _:
          case SpawnPipe _:
          case Booster _:
          case Portal _:
          case Spike _:
          case Platform _:
            return true;
          default:
            return this is Bouncer;
        }
      }
    }

    public Vector2 DefaultSize => this.defaultSize ?? new Vector2(20f, 20f);

    public float MinScale { get; protected set; } = 0.5f;

    public float? MaxScale { get; protected set; }

    protected List<ObjectParameter> availableParameters { get; } = new List<ObjectParameter>();

    public GameObject(
      Simulation simulation,
      float x,
      float y,
      int width,
      int height,
      Microsoft.Xna.Framework.Color? color = null,
      Texture2D texture = null,
      Microsoft.Xna.Framework.Rectangle? spritePos = null,
      float rotation = 0.0f,
      float collisionHeightPercent = 1f)
      : base(simulation, x, y, width, height, rotation)
    {
      Microsoft.Xna.Framework.Color? nullable = color;
      this.color = nullable ?? Microsoft.Xna.Framework.Color.White;
      this.texture = texture;
      this.spritePos = spritePos;
      this.enableDraw = true;
      if (texture == null)
      {
        if (this.color != Microsoft.Xna.Framework.Color.Transparent)
        {
          this.texture = TextureManager.GetRectangleTexture(width, height, Microsoft.Xna.Framework.Color.White, new Microsoft.Xna.Framework.Color?(new Microsoft.Xna.Framework.Color(180, 180, 180)));
          this.HasGeneratedTexture = true;
        }
        else
        {
          int width1 = width;
          int height1 = height;
          Microsoft.Xna.Framework.Color transparent = Microsoft.Xna.Framework.Color.Transparent;
          nullable = new Microsoft.Xna.Framework.Color?();
          Microsoft.Xna.Framework.Color? fadeTo = nullable;
          this.texture = TextureManager.GetRectangleTexture(width1, height1, transparent, fadeTo);
          this.HasGeneratedTexture = true;
        }
      }
      else
        this.CalculateRenderScale();
    }

    public Microsoft.Xna.Framework.Rectangle GetRectangle(bool collisionOnly = true)
    {
      return collisionOnly ? this.collisionRect : this.Rectangle;
    }

    protected override void CacheRectangle(Microsoft.Xna.Framework.Rectangle rect)
    {
      base.CacheRectangle(rect);
      Microsoft.Xna.Framework.Rectangle rectangle = rect;
      if ((double) this.CollisionHeightPercent != 1.0)
        rectangle.Height = (int) ((double) rectangle.Height * (double) this.CollisionHeightPercent);
      this.collisionRect = rectangle;
    }

    public void CalculateRenderScale()
    {
      Vector2 spriteSize = this.SpriteSize;
      this.renderScale.X = (float) this.actualSize.Item1 / spriteSize.X;
      this.renderScale.Y = (float) this.actualSize.Item2 / spriteSize.Y;
    }

    public void SetScaleWidth(int width)
    {
      this.renderScale.X = (float) width / (this.spritePos.HasValue ? (float) this.spritePos.Value.Width : (float) this.texture.Width);
    }

    public void SetScaleHeight(int height)
    {
      this.renderScale.Y = (float) height / (this.spritePos.HasValue ? (float) this.spritePos.Value.Height : (float) this.texture.Height);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if (!this.enableDraw)
        return;
      base.Draw(spriteBatch);
    }

    public bool IsCollidingWith(GameObject other, out float penetrationDepth)
    {
      penetrationDepth = 0.0f;
      if (other == null || other == this)
        return false;
      Microsoft.Xna.Framework.Rectangle collisionRectangle = this.CollisionRectangle;
      bool flag = (double) (collisionRectangle.X + collisionRectangle.Width) > (double) other.X && (double) (collisionRectangle.Y + collisionRectangle.Height) > (double) other.Y && (double) collisionRectangle.X < (double) other.X + (double) other.Width && (double) collisionRectangle.Y < (double) other.Y + (double) other.Height;
      if (flag)
        penetrationDepth = Utility.GetMinAxisOverlap(collisionRectangle, other.CollisionRectangle);
      return flag;
    }

    public bool IntersectsRectangle(Microsoft.Xna.Framework.Rectangle otherRect, bool collisionOnly = true)
    {
      return (collisionOnly ? this.CollisionRectangle : this.Rectangle).Intersects(otherRect);
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      this.animator?.Update(deltaTime);
      int num;
      if (this.checkPlayerCollision)
      {
        Player player = this.CurrentSimulation.Player;
        if ((player != null ? (player.IsActive ? 1 : 0) : 0) != 0)
        {
          num = this.IsCollidingWith((GameObject) this.CurrentSimulation.Player, out float _) ? 1 : 0;
          goto label_6;
        }
      }
      num = 0;
label_6:
      if (num == 0)
        return;
      this.OnPlayerCollision(this.CurrentSimulation.Player);
    }

    public virtual GameObject Copy(Simulation intoSimulation = null)
    {
      bool flag = (double) this.Rotation == 90.0 || (double) this.Rotation == 270.0;
      return new GameObject(intoSimulation ?? this.CurrentSimulation, this.X, this.Y, flag ? this.Height : this.Width, flag ? this.Width : this.Height, new Microsoft.Xna.Framework.Color?(this.color), this.HasGeneratedTexture ? (Texture2D) null : this.texture, this.spritePos, this.Rotation, this.CollisionHeightPercent);
    }

    public override object Clone()
    {
      object obj = base.Clone();
      if (this.animator != null)
      {
        Animator animator = (Animator) this.animator.Clone();
        GameObject gameObject = obj as GameObject;
        gameObject.animator = animator;
        animator.SetGameObject(gameObject);
      }
      return obj;
    }

    public List<ObjectParameter> GetParameters() => this.availableParameters;

    protected void SetAnimation(Animation animation)
    {
      if (this.animator == null)
        this.animator = new Animator(this);
      this.animator.Reset();
      this.animator.SetAnimation(animation);
    }

    protected virtual void OnPlayerCollision(Player p)
    {
    }

    public delegate void ParameterChangeHandler(int value);
  }
}

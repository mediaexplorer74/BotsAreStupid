// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.MovingPlatform
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
  internal class MovingPlatform : GameObject
  {
    private static Color hitboxColor = ColorManager.GetColor("orange");
    private const int defaultOffsetX = 0;
    private const int defaultOffsetY = -100;
    private const int defaultSpeed = 5;
    private const int defaultWidth = 84;
    private const int defaultHeight = 28;
    private Vector2 startPosition;
    private float currentT;

    public override bool HasCollision => true;

    protected override bool ShowHitboxEnabled => VarManager.GetBool("showhitboxes_special");

    protected override Color HitboxColor
    {
      get
      {
        Player player = this.CurrentSimulation.Player;
        return (player != null ? (player.IsStandingOn((GameObject) this) ? 1 : 0) : 0) == 0 ? MovingPlatform.hitboxColor : Utility.ChangeColor(MovingPlatform.hitboxColor, -0.4f);
      }
    }

    public int OffsetY { get; private set; }

    public int OffsetX { get; private set; }

    public int Speed { get; private set; }

    public MovingPlatform(
      Simulation simulation,
      float x,
      float y,
      int width = 84,
      int height = 28,
      int? offsetX = null,
      int? offsetY = null,
      int? speed = null)
      : base(simulation, x, y, width, height, new Color?(Color.White), TextureManager.GetTexture("tileset"), new Rectangle?(TextureManager.GetSpritePos("movingplatform")))
    {
      this.OffsetX = offsetX.GetValueOrDefault();
      int? nullable = offsetY;
      this.OffsetY = nullable ?? -100;
      nullable = speed;
      this.Speed = nullable ?? 5;
      this.startPosition = new Vector2(x, y);
      List<ObjectParameter> availableParameters1 = this.availableParameters;
      ObjectParameter objectParameter1 = new ObjectParameter();
      objectParameter1.name = "X Offset:";
      objectParameter1.min = -1000;
      objectParameter1.max = 1000;
      objectParameter1.propertyName = nameof (OffsetX);
      objectParameter1.gameObject = (GameObject) this;
      ObjectParameter objectParameter2 = objectParameter1;
      availableParameters1.Add(objectParameter2);
      List<ObjectParameter> availableParameters2 = this.availableParameters;
      objectParameter1 = new ObjectParameter();
      objectParameter1.name = "Y Offset:";
      objectParameter1.min = -500;
      objectParameter1.max = 500;
      objectParameter1.propertyName = nameof (OffsetY);
      objectParameter1.gameObject = (GameObject) this;
      ObjectParameter objectParameter3 = objectParameter1;
      availableParameters2.Add(objectParameter3);
      List<ObjectParameter> availableParameters3 = this.availableParameters;
      objectParameter1 = new ObjectParameter();
      objectParameter1.name = "Speed:";
      objectParameter1.min = 1;
      objectParameter1.max = 10;
      objectParameter1.propertyName = nameof (Speed);
      objectParameter1.gameObject = (GameObject) this;
      ObjectParameter objectParameter4 = objectParameter1;
      availableParameters3.Add(objectParameter4);
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new MovingPlatform(intoSimulation, this.X, this.Y, this.Width, this.Height, new int?(this.OffsetX), new int?(this.OffsetY), new int?(this.Speed));
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if (!this.CurrentSimulation.HasStarted)
        return;
      this.currentT = (float) (((double) this.currentT + (double) deltaTime * (double) this.Speed / 5.0) % 6.2831854820251465);
      this.SetPosition(Vector2.Lerp(this.startPosition + new Vector2((float) this.OffsetX, (float) this.OffsetY), this.startPosition, (float) (Math.Cos((double) this.currentT) / 2.0 + 0.5)));
    }

    public override void SetActive(bool value)
    {
      base.SetActive(value);
      if (value)
        this.SetPosition(this.startPosition);
      this.currentT = 0.0f;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if (this.CurrentSimulation == null || this.CurrentSimulation.IsMain)
        base.Draw(spriteBatch);
      if (this.CurrentSimulation == null || !StateManager.IsState(GameState.LevelEditor))
        return;
      Vector2 vector2 = new Vector2((float) this.OffsetX, (float) this.OffsetY);
      Vector2 center = this.Center;
      Vector2 end = center + vector2;
      Color color = Utility.ChangeColor(ColorManager.GetColor("white"), -0.5f);
      Utility.DrawLine(spriteBatch, center, end, 6, new Color?(color), 0.0f, 0);
      Utility.DrawTexture(spriteBatch, this.CurrentSimulation, this.RenderTexture, this.RenderPosition + vector2, this.RenderSpritePos, Utility.ChangeColor(this.RenderColor, -0.3f), this.ScrambledLayerDepth, this.SpriteSize, this.RenderScale, this.Rotation, this.RenderFlipped);
    }

    private enum MoveState
    {
      Idle,
      Extending,
      Retracting,
    }
  }
}

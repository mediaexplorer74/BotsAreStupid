// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Platform
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal class Platform : GameObject
  {
    private static Microsoft.Xna.Framework.Color hitboxColor = ColorManager.GetColor("white");
    private static Microsoft.Xna.Framework.Color stoodOnColor = Microsoft.Xna.Framework.Color.Green;

    public override bool HasCollision => true;

    protected override bool ShowHitboxEnabled => VarManager.GetBool("showhitboxes_platforms");

    protected override Microsoft.Xna.Framework.Color HitboxColor
    {
      get
      {
        Player player = this.CurrentSimulation.Player;
        return (player != null ? (player.IsStandingOn((GameObject) this) ? 1 : 0) : 0) == 0 ? Platform.hitboxColor : Utility.ChangeColor(Platform.hitboxColor, -0.4f);
      }
    }

    public Platform(
      Simulation simulation,
      float x,
      float y,
      int width,
      int height,
      Microsoft.Xna.Framework.Color color,
      Texture2D texture = null,
      Microsoft.Xna.Framework.Rectangle? spritePos = null,
      float collisionHeightPercent = 1f,
      float rotation = 0.0f)
      : base(simulation, x, y, width, height, new Microsoft.Xna.Framework.Color?(color), texture, spritePos)
    {
      this.CollisionHeightPercent = collisionHeightPercent;
      this.isRotatable = false;
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new Platform(intoSimulation ?? this.CurrentSimulation, this.X, this.Y, this.Width, this.Height, this.color, this.HasGeneratedTexture ? (Texture2D) null : this.texture, this.spritePos, this.CollisionHeightPercent, this.Rotation);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
      if (!this.HasGeneratedTexture)
        return;
      Utility.DrawLine(spriteBatch, new Vector2(this.X, this.Y), new Vector2(this.X + (float) this.Width * (this.CurrentSimulation == null ? this.renderScale.X : 1f), this.Y), 3, new Microsoft.Xna.Framework.Color?(ColorManager.GetColor("gray")), LayerDepths.PlatformTop, 0);
    }
  }
}

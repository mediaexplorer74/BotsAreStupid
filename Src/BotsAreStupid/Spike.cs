// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Spike
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal class Spike : GameObject
  {
    private const int defaultWidth = 30;
    private const int defaultHeight = 8;

    protected override bool ShowHitboxEnabled => VarManager.GetBool("showhitboxes_special");

    protected override Microsoft.Xna.Framework.Color HitboxColor => Microsoft.Xna.Framework.Color.Red;

    public Spike(Simulation simulation, float x, float y, int width = 30, int height = 8, float rotation = 0.0f)
      : base(simulation, x, y, width, height, new Microsoft.Xna.Framework.Color?(Microsoft.Xna.Framework.Color.White), TextureManager.GetTexture("tileset"), new Microsoft.Xna.Framework.Rectangle?(TextureManager.GetSpritePos("spike")), rotation)
    {
      this.layerDepth = LayerDepths.Spike;
      this.defaultSize = new Vector2?(new Vector2(30f, 8f));
      this.checkPlayerCollision = true;
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new Spike(intoSimulation, this.X, this.Y, this.Width, this.Height, this.Rotation);
    }

    protected override void OnPlayerCollision(Player p)
    {
      base.OnPlayerCollision(p);
      p.Kill();
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.BackgroundObject
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal class BackgroundObject : GameObject
  {
    protected override bool ShowHitboxEnabled => VarManager.GetBool("showhitboxes_background");

    protected override Microsoft.Xna.Framework.Color HitboxColor => Microsoft.Xna.Framework.Color.Gray;

    public int zIndex
    {
      set => this.layerDepth = (float) value / 100f;
      get => (int) ((double) this.LayerDepth * 100.0);
    }

    public BackgroundObject(
      float x,
      float y,
      int width,
      int height,
      Texture2D texture = null,
      Microsoft.Xna.Framework.Rectangle? spritePos = null,
      float rotation = 0.0f,
      float? layerDepth = null,
      Simulation intoSimulation = null)
      : base(intoSimulation, x, y, width, height, new Microsoft.Xna.Framework.Color?(Microsoft.Xna.Framework.Color.White), texture, spritePos, rotation)
    {
      this.layerDepth = layerDepth.HasValue ? layerDepth.Value : LayerDepths.BackgroundObject_Default;
      if (spritePos.HasValue)
      {
        int? nullable1 = spritePos?.Width;
        double x1 = (double) (nullable1.Value * 2);
        int? nullable2;
        if (!spritePos.HasValue)
        {
          nullable1 = new int?();
          nullable2 = nullable1;
        }
        else
          nullable2 = new int?(spritePos.GetValueOrDefault().Height);
        nullable1 = nullable2;
        double y1 = (double) (nullable1.Value * 2);
        this.defaultSize = new Vector2?(new Vector2((float) x1, (float) y1));
      }
      this.availableParameters.Add(new ObjectParameter()
      {
        name = "Z-Index:",
        min = 1,
        max = 99,
        gameObject = (GameObject) this,
        propertyName = nameof (zIndex),
        tooltip = "Objects with a higher value will be drawn in front."
      });
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new BackgroundObject(this.X, this.Y, this.Width, this.Height, this.texture, this.spritePos, this.Rotation, new float?(this.LayerDepth));
    }
  }
}

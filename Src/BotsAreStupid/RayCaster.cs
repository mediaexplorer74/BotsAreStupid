// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.RayCaster
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal class RayCaster : GameObject
  {
    private int range;
    private int amount;

    public RayCaster(float x, float y, int range = 200, int amount = 32)
      : base((Simulation) null, x - (float) range, y - (float) range, range * 2, range * 2, new Color?(Color.Green))
    {
      this.layerDepth = 1f;
      this.range = range;
      this.amount = amount;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
      Vector2 vector2 = Utility.RotateVector(Vector2.UnitY, 180f / (float) this.amount);
      for (int index = 0; index < this.amount; ++index)
      {
        vector2 = Utility.RotateVector(vector2, 360f / (float) this.amount);
        SpriteBatch spriteBatch1 = spriteBatch;
        Vector2 center = this.Center;
        Vector2 end = this.Center + vector2 * (float) this.range;
        float? minDistToCollision = Utility.GetMinDistToCollision(this.CurrentSimulation, this.Center, vector2);
        float range = (float) this.range;
        Color? color = new Color?((double) minDistToCollision.GetValueOrDefault() < (double) range & minDistToCollision.HasValue ? Color.Red : Color.Green);
        double layerDepth = 1.0 / (double) index;
        Utility.DrawLine(spriteBatch1, center, end, 2, color, (float) layerDepth, 0);
      }
    }
  }
}

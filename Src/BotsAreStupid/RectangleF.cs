// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.RectangleF
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal struct RectangleF(float x, float y, float w, float h)
  {
    public float X = x;
    public float Y = y;
    public float Width = w;
    public float Height = h;

    public static implicit operator RectangleF(Rectangle r)
    {
      return new RectangleF((float) r.X, (float) r.Y, (float) r.Width, (float) r.Height);
    }

    public static explicit operator Rectangle(RectangleF r)
    {
      return new Rectangle((int) r.X, (int) r.Y, (int) r.Width, (int) r.Height);
    }

    public override string ToString()
    {
      return string.Format("({0}, {1}, {2}, {3})", (object) this.X, (object) this.Y, (object) this.Width, (object) this.Height);
    }
  }
}

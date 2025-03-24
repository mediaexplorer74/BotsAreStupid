// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.MarkedLine
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;

#nullable disable
namespace BotsAreStupid
{
  internal class MarkedLine : ICloneable
  {
    public int line;
    public float timeLeft;
    public float lifeTime;

    public MarkedLine(int line, float lifeTime)
    {
      this.line = line;
      this.lifeTime = lifeTime;
      this.timeLeft = lifeTime;
    }

    public virtual object Clone() => this.MemberwiseClone();
  }
}

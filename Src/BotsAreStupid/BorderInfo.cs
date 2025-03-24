// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.BorderInfo
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal struct BorderInfo(bool top = false, bool right = false, bool bottom = false, bool left = false)
  {
    public bool top = top;
    public bool right = right;
    public bool bottom = bottom;
    public bool left = left;
    public static BorderInfo All = new BorderInfo(true, true, true, true);
    public static BorderInfo None = new BorderInfo();
  }
}

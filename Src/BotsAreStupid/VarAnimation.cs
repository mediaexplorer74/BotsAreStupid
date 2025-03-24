// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.VarAnimation
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal class VarAnimation
  {
    public string varName;
    public int startValue;
    public int endValue;
    public float duration;
    public float timeLeft;

    public VarAnimation(string varName, int startValue, int endValue, float duration)
    {
      this.varName = varName;
      this.startValue = startValue;
      this.endValue = endValue;
      this.duration = duration;
      this.timeLeft = duration;
    }
  }
}

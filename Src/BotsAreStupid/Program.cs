// Decompiled with JetBrains decompiler
// Type: Program
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using BotsAreStupid;
using System;

#nullable disable
public static class Program
{
  [STAThread]
  private static void Main(string[] args)
  {
    if (ScoreChecker.EvaluateArgs(args))
      return;
    using (Game1 game1 = new Game1(args))
      game1.Run();
  }
}

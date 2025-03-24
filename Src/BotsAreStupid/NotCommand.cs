// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.NotCommand
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal class NotCommand : ScriptCommand
  {
    public override ScriptCommand[] Parameters => this.parent.Parameters;

    public NotCommand()
      : base("not")
    {
    }
  }
}

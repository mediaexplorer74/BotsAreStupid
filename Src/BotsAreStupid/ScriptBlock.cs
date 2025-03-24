// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScriptBlock
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal class ScriptBlock : ScriptCommand
  {
    public ScriptCommand ClosingCommand { get; private set; }

    public bool RequireWait { get; private set; }

    public ScriptBlock(
      string command,
      ScriptCommand closingCommand,
      bool requireWait,
      params ScriptCommand[] parameters)
      : base(command, parameters)
    {
      this.ClosingCommand = closingCommand;
      this.RequireWait = requireWait;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScriptText
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal class ScriptText : ScriptCommand
  {
    public override string ContextName
    {
      get => "%{smallred}Any: %" + base.ContextName;
      set => base.ContextName = value;
    }

    public ScriptText()
      : base("$text")
    {
      this.EnableAutoComplete = false;
    }

    public override bool CheckLine(string line, out ScriptError error, int pos)
    {
      error = new ScriptError(pos);
      if (!string.IsNullOrEmpty(line))
        return line.Split(' ').Length != 0;
      error.message = "No Text";
      return false;
    }
  }
}

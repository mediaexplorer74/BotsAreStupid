// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScriptError
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal struct ScriptError(int parameter, string message = null, int line = -1, bool noLineCount = false)
  {
    public string message = message;
    public int parameter = parameter;
    public int line = line;
    public bool noLineCount = noLineCount;

    public new string ToString()
    {
      return "Error in Line " + this.line.ToString() + ": " + this.message + (this.parameter == 0 ? "" : " at argument " + this.parameter.ToString());
    }
  }
}

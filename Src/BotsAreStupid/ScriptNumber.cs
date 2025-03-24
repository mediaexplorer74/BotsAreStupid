// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScriptNumber
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;
using System.Globalization;

#nullable disable
namespace BotsAreStupid
{
  internal class ScriptNumber : ScriptCommand
  {
    private int maxDecimals;
    private string suffix;
    private float minValue = -1f;
    private float maxValue = -1f;

    public override string Command => "Number";

    public override string ContextName
    {
      get => "%{smallred}Number: %" + base.ContextName;
      set => base.ContextName = value;
    }

    public int Decimals => this.maxDecimals;

    public string Suffix => this.suffix;

    public ScriptNumber(int maxDecimals = 0, string suffix = "", params ScriptCommand[] parameters)
      : base("$number", parameters)
    {
      this.maxDecimals = maxDecimals;
      this.suffix = suffix;
      this.EnableAutoComplete = false;
    }

    public ScriptNumber(
      float minValue,
      float maxValue,
      int maxDecimals = 0,
      string suffix = "",
      params ScriptCommand[] parameters)
      : base("$number", parameters)
    {
      this.maxDecimals = maxDecimals;
      this.suffix = suffix;
      this.minValue = minValue;
      this.maxValue = maxValue;
    }

    public override bool CheckLine(string line, out ScriptError error, int pos)
    {
      line.Replace(',', '.');
      string[] args = line.Trim().Split(' ');
      if (args.Length != 0)
      {
        if (string.IsNullOrEmpty(this.suffix))
        {
          float result;
          if (float.TryParse(args[0], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
          {
            if ((double) this.minValue != -1.0 && (double) result < (double) this.minValue)
            {
              error = new ScriptError(pos, "Value too small (min: " + this.minValue.ToString() + ")");
              return false;
            }
            if ((double) this.maxValue != -1.0 && (double) result > (double) this.maxValue)
            {
              error = new ScriptError(pos, "Value too big (max: " + this.maxValue.ToString() + ")");
              return false;
            }
            string str = result.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            int num = str.IndexOf('.');
            if (num == -1)
              return this.CheckNext(line, args, out error, pos);
            if (str.Length - num - 1 <= this.maxDecimals)
              return this.CheckNext(line, args, out error, pos);
          }
        }
        else
        {
          if (args[0] == this.suffix)
            return this.CheckNext(line, args, out error, pos);
          if (args[0].EndsWith(this.suffix))
          {
            int result;
            if (int.TryParse(args[0].Substring(0, args[0].IndexOf(this.suffix)), out result))
            {
              if ((double) this.minValue != -1.0 && (double) result < (double) this.minValue)
              {
                error = new ScriptError(pos, "Value too small (min: " + this.minValue.ToString() + ")");
                return false;
              }
              if ((double) this.maxValue == -1.0 || (double) result <= (double) this.maxValue)
                return this.CheckNext(line, args, out error, pos);
              error = new ScriptError(pos, "Value too big (max: " + this.maxValue.ToString() + ")");
              return false;
            }
          }
          else
          {
            error = new ScriptError(pos, "Invalid Suffix");
            return false;
          }
        }
      }
      error = new ScriptError(pos, "Invalid Number");
      return false;
    }
  }
}

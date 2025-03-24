// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScriptCommand
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

#nullable disable
namespace BotsAreStupid
{
  internal class ScriptCommand
  {
    private ScriptCommand[] parameters;
    protected ScriptCommand parent;
    protected string contextName;
    private const string restLinePattern = "(?<=(^\\s*(\\S+\\s+){0}\\S+))\\s+.*";

    public virtual string Command { private set; get; }

    public virtual ScriptCommand[] Parameters
    {
      private set => this.parameters = value;
      get => this.parameters;
    }

    public bool HideBluePrint { set; get; }

    public virtual string ContextName
    {
      set => this.contextName = value;
      get => string.IsNullOrEmpty(this.contextName) ? this.Command : this.contextName;
    }

    public string Tooltip { get; set; }

    public bool EnableContext { get; set; } = true;

    public bool EnableAutoComplete { get; protected set; } = true;

    public bool AllowInIntroOnly { get; set; }

    public bool AllowAtBeginningOnly { get; set; }

    public int UnlockedInLevel { get; set; } = 0;

    public ScriptCommand(string command, params ScriptCommand[] parameters)
    {
      this.Command = command.ToLower();
      this.parameters = new ScriptCommand[parameters.Length];
      for (int index = 0; index < parameters.Length; ++index)
      {
        this.parameters[index] = parameters[index].Clone();
        this.parameters[index].parent = this;
      }
    }

    public virtual ScriptCommand Clone()
    {
      object obj = this.MemberwiseClone();
      for (int index = 0; index < this.parameters.Length; ++index)
        this.parameters[index] = this.parameters[index].Clone();
      return (ScriptCommand) obj;
    }

    public virtual bool CheckLine(string line, out ScriptError error, int pos = 0)
    {
      error = new ScriptError(pos);
      string[] args = ScriptParser.GetArgs(line);
      return args.Length != 0 && args[0].ToLower() == this.Command && this.CheckNext(line, args, out error, pos);
    }

    protected bool CheckNext(string line, string[] args, out ScriptError error, int pos)
    {
      error = new ScriptError(pos);
      if (args.Length == 1)
      {
        if (this.Parameters.Length == 0 || Enumerable.Any<ScriptCommand>((IEnumerable<ScriptCommand>) this.Parameters, (Func<ScriptCommand, bool>) (x => x == null)))
          return true;
        error.message = "Missing Parameter";
        return false;
      }
      if (this.Parameters.Length != 0)
      {
        string line1 = ((Capture) Regex.Match(line, "(?<=(^\\s*(\\S+\\s+){0}\\S+))\\s+.*")).Value;
        for (int index = this.Parameters.Length - 1; index >= 0; --index)
        {
          ScriptError error1;
          if (this.Parameters[index].CheckLine(line1, out error1, pos + 1))
            return true;
          if (!string.IsNullOrEmpty(error1.message))
            error = error1;
        }
        if (string.IsNullOrEmpty(error.message))
          error = new ScriptError(pos + 1, "Invalid Parameter '" + args[1] + "'");
      }
      else if (args.Length > 1)
        error.message = "Redundant Parameter '" + args[1] + "'";
      return false;
    }

    protected string GetError(int pos, string error) => error + " at parameter " + pos.ToString();

    public new string ToString()
    {
      return this.Parameters.Length == 0 ? this.Command : this.Command + " " + this.GetParameterString();
    }

    public bool IsUnlockedIn(Simulation simulation)
    {
      return this.IsAvailableIn(simulation) && simulation.UnlockedLevels >= this.UnlockedInLevel;
    }

    public bool IsAvailableIn(Simulation simulation)
    {
      return VarManager.GetBool("godmode") || !this.AllowInIntroOnly || simulation.IsIntro;
    }

    private string GetParameterString()
    {
      bool flag1 = this.Parameters.Length > 1;
      string parameterString = flag1 ? "[" : "";
      for (int index = 0; index < this.Parameters.Length; ++index)
      {
        bool flag2 = index == this.Parameters.Length - 1;
        parameterString = parameterString + this.Parameters[index].ToString() + (flag2 ? "" : " ") + (!flag1 || flag2 ? "" : "| ");
      }
      if (flag1)
        parameterString += "]";
      return parameterString;
    }

    public virtual string AutoComplete(string input)
    {
      string[] args = ScriptParser.GetArgs(input);
      if (args.Length != 0)
      {
        if (args[0] == this.Command)
        {
          if (args.Length == 1 && this.Parameters.Length != 0)
          {
            IEnumerable<ScriptCommand> scriptCommands = Enumerable.Where<ScriptCommand>((IEnumerable<ScriptCommand>) this.Parameters, (Func<ScriptCommand, bool>) (p => p.EnableAutoComplete));
            return args[0] + " " + (Enumerable.Count<ScriptCommand>(scriptCommands) > 0 ? Enumerable.First<ScriptCommand>(scriptCommands).Command : "");
          }
          if (args.Length > 1)
          {
            int length = input.IndexOf(' ') + 1;
            string input1 = input.Substring(input.IndexOf(' ') + 1);
            foreach (ScriptCommand parameter in this.Parameters)
            {
              string str = parameter.AutoComplete(input1);
              if (input1 != str)
                return input.Substring(0, length) + str;
            }
          }
        }
        else if (args.Length == 1 && this.Command.StartsWith(args[0]))
          return this.Command;
      }
      return input;
    }

    public ScriptCommand FindParameter(string searchValue)
    {
      foreach (ScriptCommand parameter in this.Parameters)
      {
        if (parameter.Command == searchValue)
          return parameter;
      }
      foreach (ScriptCommand parameter in this.Parameters)
      {
        if (parameter is ScriptNumber)
        {
          ScriptNumber scriptNumber = parameter as ScriptNumber;
          bool flag = !string.IsNullOrEmpty(scriptNumber.Suffix);
          float result;
          if (!flag && float.TryParse(searchValue, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || flag && searchValue.EndsWith(scriptNumber.Suffix) && float.TryParse(searchValue.Replace(scriptNumber.Suffix, ""), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
            return parameter;
        }
      }
      return (ScriptCommand) null;
    }

    public List<ScriptCommand> GetAvailableParameters(
      string query,
      Simulation simulation,
      bool checkContains = false)
    {
      List<ScriptCommand> availableParameters = new List<ScriptCommand>();
      foreach (ScriptCommand parameter in this.Parameters)
      {
        if (parameter.IsUnlockedIn(simulation) && parameter.ContextName != query && (checkContains ? parameter.ContextName.Contains(query) : parameter.ContextName.StartsWith(query)))
          availableParameters.Add(parameter);
      }
      return availableParameters;
    }
  }
}

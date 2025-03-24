// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScriptParser
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
  internal class ScriptParser
  {
    public static ScriptParser Instance = new ScriptParser();
    private List<ScriptCommand> availableCommands = new List<ScriptCommand>();
    private const string argPattern = "(?<=\\s*)\\S+(?=\\s*)";

    public ScriptCommand[] AvailableCommands => this.availableCommands.ToArray();

    public ScriptParser()
    {
      ScriptCommand scriptCommand1 = new ScriptCommand("left", Array.Empty<ScriptCommand>());
      ScriptCommand scriptCommand2 = new ScriptCommand("right", Array.Empty<ScriptCommand>());
      ScriptCommand scriptCommand3 = new ScriptCommand("down", new ScriptCommand[2]
      {
        scriptCommand1,
        scriptCommand2
      })
      {
        UnlockedInLevel = 6
      };
      ScriptCommand scriptCommand4 = new ScriptCommand("up", new ScriptCommand[2]
      {
        scriptCommand1,
        scriptCommand2
      })
      {
        HideBluePrint = true,
        UnlockedInLevel = 6
      };
      ScriptCommand scriptCommand5 = new ScriptCommand("grounded", Array.Empty<ScriptCommand>());
      ScriptCommand scriptCommand6 = new ScriptCommand("boosted", Array.Empty<ScriptCommand>());
      ScriptCommand scriptCommand7 = new ScriptCommand("hooked", Array.Empty<ScriptCommand>())
      {
        UnlockedInLevel = 5
      };
      ScriptCommand scriptCommand8 = new ScriptCommand("stopped", Array.Empty<ScriptCommand>());
      ScriptCommand scriptCommand9 = new ScriptCommand("falling", Array.Empty<ScriptCommand>());
      ScriptCommand scriptCommand10 = new ScriptCommand("orbpickup", Array.Empty<ScriptCommand>());
      ScriptCommand scriptCommand11 = new ScriptCommand("true", Array.Empty<ScriptCommand>());
      ScriptCommand scriptCommand12 = new ScriptCommand("false", Array.Empty<ScriptCommand>());
      NotCommand notCommand = new NotCommand();
      ScriptCommand[] scriptCommandArray1 = new ScriptCommand[7]
      {
        scriptCommand5,
        scriptCommand6,
        scriptCommand7,
        scriptCommand8,
        scriptCommand9,
        scriptCommand10,
        (ScriptCommand) notCommand
      };
      ScriptCommand scriptCommand13 = new ScriptCommand("until", Enumerable.ToArray<ScriptCommand>((IEnumerable<ScriptCommand>) scriptCommandArray1))
      {
        UnlockedInLevel = 3
      };
      ScriptCommand scriptCommand14 = new ScriptCommand("while", Enumerable.ToArray<ScriptCommand>((IEnumerable<ScriptCommand>) scriptCommandArray1))
      {
        UnlockedInLevel = 3
      };
      ScriptCommand[] scriptCommandArray2 = new ScriptCommand[8]
      {
        scriptCommand5,
        scriptCommand6,
        scriptCommand7,
        scriptCommand8,
        scriptCommand9,
        scriptCommand11,
        scriptCommand12,
        (ScriptCommand) notCommand
      };
      ScriptCommand scriptCommand15 = new ScriptCommand("until", Enumerable.ToArray<ScriptCommand>((IEnumerable<ScriptCommand>) scriptCommandArray2));
      ScriptCommand scriptCommand16 = new ScriptCommand("while", Enumerable.ToArray<ScriptCommand>((IEnumerable<ScriptCommand>) scriptCommandArray2));
      this.availableCommands.Add(new ScriptCommand("start", new ScriptCommand[2]
      {
        scriptCommand1,
        scriptCommand2
      })
      {
        HideBluePrint = true,
        EnableContext = false
      });
      this.availableCommands.Add(new ScriptCommand("move", new ScriptCommand[2]
      {
        scriptCommand1,
        scriptCommand2
      })
      {
        Tooltip = "Start moving in a direction until 'stop' is called or the direction is changed.\nAlias: 'start'\nSyntax:\nmove [left | right]"
      });
      List<ScriptCommand> availableCommands1 = this.availableCommands;
      ScriptCommand[] scriptCommandArray3 = new ScriptCommand[4];
      ScriptNumber scriptNumber1 = new ScriptNumber(1f / 1000f, float.MaxValue, 3, "", Array.Empty<ScriptCommand>());
      scriptNumber1.ContextName = "[seconds]";
      scriptCommandArray3[0] = (ScriptCommand) scriptNumber1;
      ScriptNumber scriptNumber2 = new ScriptNumber(1f, (float) int.MaxValue, 0, "f", Array.Empty<ScriptCommand>());
      scriptNumber2.ContextName = "[frames]f";
      scriptNumber2.UnlockedInLevel = 10;
      scriptCommandArray3[1] = (ScriptCommand) scriptNumber2;
      scriptCommandArray3[2] = scriptCommand13;
      scriptCommandArray3[3] = scriptCommand14;
      availableCommands1.Add(new ScriptCommand("wait", scriptCommandArray3)
      {
        Tooltip = "Pause script execution until the wait is over.\nSyntax:\nwait [time | frames | until (not) [grounded | boosted | hooked | stopped | falling | orbpickup]]\n\nExamples:\nwait 1.55\nwait 10f\nwait until orbpickup\nwait until not grounded\n\n(More than 3 decimals in time wont have any effect)"
      });
      this.availableCommands.Add(new ScriptCommand("stop", new ScriptCommand[3]
      {
        scriptCommand1,
        scriptCommand2,
        new ScriptCommand("hook", Array.Empty<ScriptCommand>())
        {
          UnlockedInLevel = 5,
          EnableContext = false
        }
      })
      {
        HideBluePrint = true,
        EnableContext = false
      });
      this.availableCommands.Add(new ScriptCommand("stop", Array.Empty<ScriptCommand>())
      {
        Tooltip = "Stop moving/accelerating.\nThe bot will only slow down once it's grounded, since there is no friction while in the air."
      });
      this.availableCommands.Add(new ScriptCommand("jump", Array.Empty<ScriptCommand>())
      {
        Tooltip = "Execute a single jump.\nNothing will happen if the bot isn't grounded.",
        UnlockedInLevel = 3
      });
      this.availableCommands.Add(new ScriptCommand("hook", new ScriptCommand[4]
      {
        scriptCommand1,
        scriptCommand2,
        scriptCommand3,
        scriptCommand4
      })
      {
        Tooltip = "Extend the hook in one of 4 directions.\nSyntax:\nhook (down) [left | right]",
        UnlockedInLevel = 5
      });
      this.availableCommands.Add(new ScriptCommand("unhook", Array.Empty<ScriptCommand>())
      {
        Tooltip = "Detach the hook if it's attached or else stop extending it.\nAlias: 'stop hook'",
        UnlockedInLevel = 5
      });
      List<ScriptCommand> availableCommands2 = this.availableCommands;
      ScriptCommand[] scriptCommandArray4 = new ScriptCommand[1];
      ScriptCommand[] scriptCommandArray5 = new ScriptCommand[1];
      ScriptText scriptText = new ScriptText();
      scriptText.ContextName = "[text]";
      scriptCommandArray5[0] = (ScriptCommand) scriptText;
      ScriptNumber scriptNumber3 = new ScriptNumber(3, "", scriptCommandArray5);
      scriptNumber3.ContextName = "[seconds]";
      scriptCommandArray4[0] = (ScriptCommand) scriptNumber3;
      availableCommands2.Add(new ScriptCommand("say", scriptCommandArray4)
      {
        Tooltip = "Make the bot spell out something.\nSyntax:\nsay [seconds] [text]\n\nExample:\nsay 5 Hello World!",
        UnlockedInLevel = 8
      });
      ScriptCommand scriptCommand17 = new ScriptCommand("repeatend", Array.Empty<ScriptCommand>())
      {
        Tooltip = "End repeat clause.\n\nExample:\nrepeat 5\njump\nwait 0.75\nrepeatend",
        UnlockedInLevel = 16
      };
      List<ScriptCommand> availableCommands3 = this.availableCommands;
      ScriptCommand closingCommand = scriptCommand17;
      ScriptCommand[] scriptCommandArray6 = new ScriptCommand[3];
      ScriptNumber scriptNumber4 = new ScriptNumber(0, "", Array.Empty<ScriptCommand>());
      scriptNumber4.ContextName = "[amount]";
      scriptCommandArray6[0] = (ScriptCommand) scriptNumber4;
      scriptCommandArray6[1] = scriptCommand15;
      scriptCommandArray6[2] = scriptCommand16;
      ScriptBlock scriptBlock = new ScriptBlock("repeat", closingCommand, true, scriptCommandArray6);
      scriptBlock.UnlockedInLevel = 16;
      scriptBlock.Tooltip = "Repeat the succeeding lines.\nSyntax:\nrepeat [number | [while | until] (not) [grounded | boosted | hooked | stopped | falling]]\n\nExamples:\nrepeat 10\nrepeat while not grounded\nrepeat until boosted\n\nThe block has to contain at least 1 'wait' command and needs to be closed by 'repeatend'.";
      availableCommands3.Add((ScriptCommand) scriptBlock);
      this.availableCommands.Add(scriptCommand17);
      this.availableCommands.Add(new ScriptCommand("-tickrate", new ScriptCommand[3]
      {
        new ScriptCommand("low", Array.Empty<ScriptCommand>()),
        new ScriptCommand("legacy", Array.Empty<ScriptCommand>()),
        new ScriptCommand("high", Array.Empty<ScriptCommand>())
      })
      {
        UnlockedInLevel = 10,
        AllowAtBeginningOnly = true,
        Tooltip = "Change how often the simulation is updated per second.\nSyntax:\n-tickrate [low (50/s) | legacy (128/s) | default (200/s) | high (1000/s)]"
      });
      this.availableCommands.Add(new ScriptCommand("text", new ScriptCommand[1]
      {
        (ScriptCommand) new ScriptNumber(0, "", new ScriptCommand[1]
        {
          (ScriptCommand) new ScriptNumber(0, "", new ScriptCommand[1]
          {
            (ScriptCommand) new ScriptNumber(3, "", new ScriptCommand[1]
            {
              (ScriptCommand) new ScriptText()
            })
          })
        })
      })
      {
        AllowInIntroOnly = true,
        HideBluePrint = true
      });
      this.availableCommands.Add(new ScriptCommand("-teleport", new ScriptCommand[1]
      {
        (ScriptCommand) new ScriptNumber(0, "", new ScriptCommand[1]
        {
          (ScriptCommand) new ScriptNumber(0, "", Array.Empty<ScriptCommand>())
        })
      })
      {
        AllowInIntroOnly = true,
        HideBluePrint = true
      });
      this.availableCommands.Add(new ScriptCommand("-introwait", new ScriptCommand[3]
      {
        new ScriptCommand("main", new ScriptCommand[2]
        {
          scriptCommand13,
          scriptCommand14
        }),
        new ScriptCommand("maxorbs", new ScriptCommand[1]
        {
          (ScriptCommand) new ScriptNumber(0, "", Array.Empty<ScriptCommand>())
        }),
        new ScriptCommand("maininrect", new ScriptCommand[1]
        {
          (ScriptCommand) new ScriptNumber(0, "", new ScriptCommand[1]
          {
            (ScriptCommand) new ScriptNumber(0, "", new ScriptCommand[1]
            {
              (ScriptCommand) new ScriptNumber(0, "", new ScriptCommand[1]
              {
                (ScriptCommand) new ScriptNumber(0, "", Array.Empty<ScriptCommand>())
              })
            })
          })
        })
      })
      {
        AllowInIntroOnly = true,
        HideBluePrint = true
      });
      this.availableCommands.Add(new ScriptCommand("-cleartext", Array.Empty<ScriptCommand>())
      {
        AllowInIntroOnly = true,
        HideBluePrint = true
      });
      this.availableCommands.Add(new ScriptCommand("-stopsay", Array.Empty<ScriptCommand>())
      {
        AllowInIntroOnly = true,
        HideBluePrint = true
      });
      this.availableCommands.Add(new ScriptCommand("-destroylasttext", Array.Empty<ScriptCommand>())
      {
        AllowInIntroOnly = true,
        HideBluePrint = true
      });
      this.availableCommands.Add(new ScriptCommand("-emote", new ScriptCommand[1]
      {
        new ScriptCommand("wave", Array.Empty<ScriptCommand>())
      })
      {
        AllowInIntroOnly = true,
        HideBluePrint = true
      });
    }

    public static void CheckScript(
      string[] script,
      Simulation simulation,
      out ScriptError[] errors,
      out int lineCount)
    {
      lineCount = 0;
      List<ScriptError> scriptErrorList = new List<ScriptError>();
      bool flag = false;
      int currentIdx = 0;
      while (true)
      {
        int num = currentIdx;
        int? length = script?.Length;
        int valueOrDefault = length.GetValueOrDefault();
        if (num < valueOrDefault & length.HasValue)
        {
          string line = script[currentIdx];
          if (flag)
          {
            if (ScriptParser.IsBlockCommentEnd(line))
              flag = false;
          }
          else if (ScriptParser.IsBlockCommentStart(line))
          {
            if (!ScriptParser.IsBlockCommentEnd(line))
              flag = true;
          }
          else
          {
            ScriptError error;
            if (ScriptParser.IsExecutableLine(line, out error, currentIdx, simulation, script))
            {
              if (!error.noLineCount)
                ++lineCount;
            }
            else if (error.message != "empty")
            {
              error.line = currentIdx + 1;
              scriptErrorList.Add(error);
            }
          }
          ++currentIdx;
        }
        else
          break;
      }
      errors = scriptErrorList.ToArray();
    }

    public static bool ParseBool(Player player, string[] args)
    {
      if (args.Length == 0)
        return false;
      switch (args[0])
      {
        case "boosted":
          if (player.IsBoosted)
            return true;
          break;
        case "falling":
          if (Math.Round((double) player.Velocity.Y, 2) > 0.0)
            return true;
          break;
        case "grounded":
          if (player.IsGrounded)
            return true;
          break;
        case "hooked":
          if (player.IsHooked)
            return true;
          break;
        case "not":
          List<string> stringList = new List<string>();
          for (int index = 1; index < args.Length; ++index)
            stringList.Add(args[index]);
          return !ScriptParser.ParseBool(player, stringList.ToArray());
        case "orbpickup":
          if (player.HasPickedUpOrb)
            return true;
          break;
        case "stopped":
          if ((double) player.RoundedVelocityMagnitude == 0.0 || player.HasFlippedVelocity)
            return true;
          break;
        case "true":
          return true;
        default:
          return false;
      }
      return false;
    }

    public static string AutoComplete(string input)
    {
      foreach (ScriptCommand availableCommand in ScriptParser.Instance.availableCommands)
      {
        string str = availableCommand.AutoComplete(input);
        if (str != input)
          return str;
      }
      return input;
    }

    public static bool IsExecutableLine(
      string line,
      out ScriptError error,
      int currentIdx,
      Simulation simulation,
      string[] fullScript,
      bool isIntro = false)
    {
      error = new ScriptError(0);
      string str1 = ScriptParser.GetArg(line, 0);
      if (!string.IsNullOrEmpty(str1) && !ScriptParser.IsSingleComment(line))
      {
        if (str1 == "-tickrate")
          error.noLineCount = true;
        foreach (ScriptCommand availableCommand in ScriptParser.Instance.availableCommands)
        {
          ScriptError error1;
          if (availableCommand.CheckLine(line, out error1))
          {
            if (!availableCommand.IsAvailableIn(simulation))
            {
              error.message = "Command not allowed";
              return false;
            }
            if (availableCommand is ScriptBlock)
            {
              ScriptBlock scriptBlock = (ScriptBlock) availableCommand;
              string str2 = "At least one wait command is required inside the " + scriptBlock.Command + " block";
              bool flag1 = false;
              bool flag2 = false;
              for (int index = currentIdx++; index < fullScript.Length; ++index)
              {
                string line1 = fullScript[index];
                if (flag2)
                {
                  if (ScriptParser.IsBlockCommentEnd(line1))
                    flag2 = false;
                }
                else if (ScriptParser.IsBlockCommentStart(line1))
                {
                  if (!ScriptParser.IsBlockCommentEnd(line1))
                    flag2 = true;
                }
                else
                {
                  if (!flag1 && line1.ToLower().Contains("wait"))
                    flag1 = true;
                  if (fullScript[index].Contains(scriptBlock.ClosingCommand.Command))
                  {
                    if (!scriptBlock.RequireWait | flag1)
                      return true;
                    error1.message = str2;
                    break;
                  }
                }
              }
              if (error1.message != str2)
                error1.message = "Block has to be closed by '" + scriptBlock.ClosingCommand.Command + "'";
            }
            else
            {
              if (!availableCommand.AllowAtBeginningOnly)
                return true;
              for (int currentIdx1 = currentIdx - 1; currentIdx1 >= 0; --currentIdx1)
              {
                if (ScriptParser.IsExecutableLine(fullScript[currentIdx1], out ScriptError _, currentIdx1, simulation, fullScript, isIntro))
                {
                  error.parameter = 0;
                  error.message = "Command is only allowed at the beginning of the script";
                  return false;
                }
              }
              return true;
            }
          }
          if (!string.IsNullOrEmpty(error1.message))
            error = error1;
        }
        if (string.IsNullOrEmpty(error.message))
          error.message = "Invalid Command '" + str1 + "'";
      }
      else
        error.message = "empty";
      return false;
    }

    public static bool IsCompleteLine(
      string line,
      out ScriptError error,
      out ScriptCommand command,
      Simulation simulation)
    {
      error = new ScriptError(0);
      string str = ScriptParser.GetArg(line, 0);
      command = (ScriptCommand) null;
      if (string.IsNullOrEmpty(str) || line.StartsWith("//"))
        return true;
      foreach (ScriptCommand availableCommand in ScriptParser.Instance.availableCommands)
      {
        if (simulation == null || availableCommand.IsAvailableIn(simulation))
        {
          ScriptError error1;
          if (availableCommand.CheckLine(line, out error1))
          {
            command = availableCommand;
            return true;
          }
          if (!string.IsNullOrEmpty(error1.message))
          {
            error = error1;
            command = availableCommand;
          }
        }
      }
      return false;
    }

    public static string[] GetArgs(string line)
    {
      MatchCollection matchCollection = Regex.Matches(line, "(?<=\\s*)\\S+(?=\\s*)");
      string[] args = new string[matchCollection.Count];
      for (int index = 0; index < matchCollection.Count; ++index)
        args[index] = ((Capture) matchCollection[index]).Value;
      return args;
    }

    private static string GetArgPattern(int idx, bool fromBack)
    {
      return !fromBack ? "(?<=(^\\s*(\\S+\\s+){" + idx.ToString() + "}))\\S+" : "\\S+(?=(\\s+\\S+){" + idx.ToString() + "}\\s*$)";
    }

    public static string GetArg(string line, int idx, bool fromBack = false)
    {
      Match match = Regex.Match(line, ScriptParser.GetArgPattern(idx, fromBack));
      return ((Group) match).Success ? ((Capture) match).Value : (string) null;
    }

    public static int GetArgPosition(string line, int idx, bool fromBack = false)
    {
      return ((Capture) Regex.Match(line, ScriptParser.GetArgPattern(idx, fromBack))).Index;
    }

    public static int GetArgIndex(string line, int position)
    {
      string[] args = ScriptParser.GetArgs(line);
      if (args.Length == 0)
        return -1;
      int num = ScriptParser.GetArgPosition(line, 0);
      for (int idx = 1; idx < args.Length; ++idx)
      {
        int argPosition = ScriptParser.GetArgPosition(line, idx);
        if (num <= position && position < argPosition)
          return idx - 1;
        if (idx == args.Length - 1)
          return idx;
        num = argPosition;
      }
      return 0;
    }

    public static string SetArg(string line, int idx, string value, bool fromBack = false)
    {
      return Regex.Replace(line, ScriptParser.GetArgPattern(idx, fromBack), value);
    }

    public static ScriptCommand[] GetCommandsStartingWith(string start, Simulation unlockedIn = null)
    {
      return Enumerable.ToArray<ScriptCommand>(Enumerable.Where<ScriptCommand>((IEnumerable<ScriptCommand>) ScriptParser.Instance.AvailableCommands, (Func<ScriptCommand, bool>) (cmd => cmd.EnableContext && cmd.Command != start && (unlockedIn == null || cmd.IsUnlockedIn(unlockedIn)) && cmd.Command.StartsWith(start))));
    }

    public static ScriptCommand FindCommand(string commandString, Simulation unlockedIn = null)
    {
      return Enumerable.FirstOrDefault<ScriptCommand>((IEnumerable<ScriptCommand>) ScriptParser.Instance.AvailableCommands, (Func<ScriptCommand, bool>) (cmd => cmd.EnableContext && (unlockedIn == null || cmd.IsUnlockedIn(unlockedIn)) && cmd.Command.StartsWith(commandString, true, CultureInfo.InvariantCulture)));
    }

    public static ScriptCommand FindCommandQuick(string firstWord)
    {
      return Enumerable.FirstOrDefault<ScriptCommand>((IEnumerable<ScriptCommand>) ScriptParser.Instance.AvailableCommands, (Func<ScriptCommand, bool>) (cmd => cmd.Command.Equals(firstWord, StringComparison.InvariantCultureIgnoreCase)));
    }

    public static bool IsBlockStart(string commandString)
    {
      if (ScriptParser.IsBlockCommentStart(commandString))
        return true;
      commandString = commandString.Trim();
      int length = commandString.IndexOf(' ');
      if (length != -1)
        commandString = commandString.Substring(0, length);
      return ScriptParser.FindCommandQuick(commandString) is ScriptBlock;
    }

    public static bool IsBlockEnd(string commandString)
    {
      if (ScriptParser.IsBlockCommentEnd(commandString))
        return true;
      commandString = commandString.Trim();
      int length = commandString.IndexOf(' ');
      if (length != -1)
        commandString = commandString.Substring(0, length);
      return ScriptParser.FindCommandQuick(commandString)?.Command == "repeatend";
    }

    public static bool IsSingleComment(string line) => line.Trim().StartsWith("//");

    public static bool IsBlockCommentStart(string line) => line.Trim().StartsWith("/*");

    public static bool IsBlockCommentEnd(string line) => line.Trim().EndsWith("*/");
  }
}

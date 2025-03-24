// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScoreChecker
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

#nullable disable
namespace BotsAreStupid
{
  internal class ScoreChecker
  {
    private List<ThreadedSimulationCheck> checks = new List<ThreadedSimulationCheck>();
    private static string commandsToFilePath;

    public ScoreChecker()
    {
      if (VarManager.Instance == null)
      {
        VarManager varManager = new VarManager(false);
      }
      CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
      VarManager.SetBool("simplelogs", true);
      VarManager.SetBool("checkmode", true);
      VarManager.SetInt("overclock", 99);
      VarManager.SetInt("unlockedlevels", 99);
      VarManager.SetString("appdirectory", VarManager.GetString("tempdirectory"));
      VarManager.SetString("defaultlevelsdirectory", VarManager.GetString("appdirectory") + "Content/Levels/");
      VarManager.SetString("customlevelsdirectory", "C:\\Users\\leleh\\Desktop\\FH\\Projects\\BotsAreStupid\\Web\\Levels\\Custom\\");
    }

    public static bool EvaluateArgs(string[] args)
    {
      if (args.Length < 2)
        return false;
      ScoreChecker scoreChecker = new ScoreChecker();
      string levelName = args[0];
      if (args[1].ToLower() == "random")
      {
        int result;
        int amount = args.Length < 3 || !int.TryParse(args[2], out result) ? 100 : result;
        scoreChecker.AddRandomChecks(levelName, amount);
      }
      else
      {
        List<(string, string)> valueTupleList = new List<(string, string)>();
        for (int index = 0; index < args.Length; index += 2)
          valueTupleList.Add((args[index], args[index + 1]));
        scoreChecker.AddChecks(valueTupleList.ToArray());
      }
      scoreChecker.Run();
      return true;
    }

    private void AddCheck(
      string levelName,
      string[] instructions,
      bool start = false,
      bool saveSuccessful = false)
    {
      ThreadedSimulationCheck threadedSimulationCheck = new ThreadedSimulationCheck(levelName, instructions, saveSuccessful);
      if (start)
        threadedSimulationCheck.Start();
      this.checks.Add(threadedSimulationCheck);
    }

    public void AddChecks(params (string, string)[] data)
    {
      Console.WriteLine("Creating " + data.Length.ToString() + " Check" + (data.Length == 1 ? "" : "s") + "...");
      foreach ((string, string) tuple in data)
        this.AddCheck(tuple.Item1, tuple.Item2.Split('|'));
    }

    public void AddRandomChecks(string levelName, int amount)
    {
      Console.WriteLine("Creating " + amount.ToString() + " Random Check" + (amount == 1 ? "" : "s") + "...");
      for (int index = 0; index < amount; ++index)
        this.AddCheck(levelName, ScriptInterpreter.GetRandomInstructions(), saveSuccessful: true);
    }

    public void Run()
    {
      Console.WriteLine("Starting " + this.checks.Count.ToString() + " Check" + (this.checks.Count == 1 ? "" : "s") + "...");
      foreach (ThreadedSimulationCheck check in this.checks)
        check.Start();
      bool flag = this.checks.Count > 1;
      int num = 0;
      string str1 = VarManager.GetString("basedirectory");
      List<string> stringList1 = new List<string>(this.checks.Count);
      while (this.checks.Count > 0)
      {
        for (int index = this.checks.Count - 1; index >= 0; index = Math.Min(index - 1, this.checks.Count - 1))
        {
          ThreadedSimulationCheck check = this.checks[index];
          if (check.IsFinished)
          {
            string str2 = flag ? check.ID + " => " : "";
            string str3 = check.IsSuccess ? "" : " after " + check.ExitTime.ToString() + "s";
            string str4 = str2 + check.IsSuccess.ToString() + " " + check.ExitMessage + str3;
            stringList1.Add(str4);
            Console.WriteLine(str4);
            if (check.IsSuccess)
            {
              ++num;
              if (check.SaveSuccessful)
              {
                List<string> stringList2 = new List<string>();
                stringList2.Add(check.LevelName + ":");
                stringList2.AddRange((IEnumerable<string>) check.Instructions);
                stringList2.Add(new string('=', 69));
                File.AppendAllLines(str1 + "randomscripts.txt", (IEnumerable<string>) stringList2);
              }
            }
            this.checks.Remove(check);
            check.Dispose();
          }
        }
        if (this.checks.Count > 0)
          Thread.Sleep(100);
      }
      File.WriteAllLines(str1 + "results.txt", (IEnumerable<string>) stringList1);
      if (!flag)
        return;
      Console.WriteLine("All done! [" + num.ToString() + " successful]");
    }
  }
}

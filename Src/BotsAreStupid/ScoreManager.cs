// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScoreManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
namespace BotsAreStupid
{
  internal class ScoreManager
  {
    public static 
    #nullable disable
    ScoreManager Instance;
    private readonly ConcurrentDictionary<string, ScoreData> overviews = new ConcurrentDictionary<string, ScoreData>();
    private static readonly ConcurrentDictionary<string, ScoreData[]> cachedLists = new ConcurrentDictionary<string, ScoreData[]>();
    private const string personalKeyPrefix = "personal%";

    public ScoreManager() => ScoreManager.Instance = this;

    public static async void UploadScore(
      string levelName,
      string script,
      HttpManager.Callback callback = null)
    {
      Dictionary<string, string> values;
      string response;
      string[] args;
      if (script.Length > 8000)
      {
        PopupMenu.Instance.UpdateSuccess(isTooLong: true);
        values = (Dictionary<string, string>) null;
        response = (string) null;
        args = (string[]) null;
      }
      else
      {
        PopupMenu.Instance.SetSuccessInfoLoading();
        values = new Dictionary<string, string>()
        {
          {
            "uuid",
            VarManager.GetString("uuid")
          },
          {
            "levelname",
            levelName
          },
          {
            nameof (script),
            script
          }
        };
        response = await HttpManager.Post(VarManager.GetString("baseurl") + "Scores/Add.php", values);
        args = response.Split(' ');
        int timePosition;
        int linecountPosition;
        int totalScores;
        int personalTimePosition;
        int personalLinecountPosition;
        int totalPersonalScores;
        if (args.Length >= 6 && int.TryParse(args[0], out timePosition)
                    && int.TryParse(args[1], out linecountPosition) 
                    && int.TryParse(args[2], out totalScores) && int.TryParse(args[3], out personalTimePosition) 
                    && int.TryParse(args[4], out personalLinecountPosition)
                    && int.TryParse(args[5], out totalPersonalScores))
        {
          bool isDuplicate = false;
          if (args.Length >= 7 && args[6] == "duplicate")
            isDuplicate = true;
          PopupMenu.Instance.UpdateSuccess(timePosition, linecountPosition, totalScores,
              personalTimePosition, personalLinecountPosition, totalPersonalScores, isDuplicate);
        }
        else
          PopupMenu.Instance.UpdateSuccess();
        HttpManager.Callback callback1 = callback;
        if (callback1 == null)
        {
          values = (Dictionary<string, string>) null;
          response = (string) null;
          args = (string[]) null;
        }
        else
        {
          callback1();
          values = (Dictionary<string, string>) null;
          response = (string) null;
          args = (string[]) null;
        }
      }
    }

    public static async Task UpdateOverview(string levelName = null)
    {
      ScoreData overview;
      string personalKey;
      ScoreData personalOverview;
      if (ScoreManager.Instance == null)
      {
        overview = (ScoreData) null;
        personalKey = (string) null;
        personalOverview = (ScoreData) null;
      }
      else
      {
        levelName = levelName ?? LevelManager.CurrentLevelName;
        overview = await ScoreManager.GetOverview(levelName);
        ConcurrentDictionary<string, ScoreData> overviews1 = ScoreManager.Instance.overviews;
        if (overviews1 != null && overviews1.ContainsKey(levelName))
          ScoreManager.Instance.overviews[levelName] = overview;
        else
          ScoreManager.Instance.overviews.TryAdd(levelName, overview);
        personalKey = "personal%" + levelName;
        personalOverview = await ScoreManager.GetOverview(levelName, true);
        ConcurrentDictionary<string, ScoreData> overviews2 = ScoreManager.Instance.overviews;
        if (overviews2 != null && overviews2.ContainsKey(personalKey))
        {
          ScoreManager.Instance.overviews[personalKey] = personalOverview;
          overview = (ScoreData) null;
          personalKey = (string) null;
          personalOverview = (ScoreData) null;
        }
        else
        {
          ScoreManager.Instance.overviews.TryAdd(personalKey, personalOverview);
          overview = (ScoreData) null;
          personalKey = (string) null;
          personalOverview = (ScoreData) null;
        }
      }
    }

    public static void ClearCaches()
    {
      if (ScoreManager.Instance == null)
        return;
      ScoreManager.cachedLists.Clear();
    }

    public static void ClearLevelCaches(string levelname)
    {
      if (ScoreManager.Instance == null)
        return;
      foreach (KeyValuePair<string, ScoreData[]> cachedList in ScoreManager.cachedLists)
      {
        if (cachedList.Key.Contains(levelname))
        {
          ScoreData[] scoreDataArray;
          ScoreManager.cachedLists.TryRemove(cachedList.Key, out scoreDataArray);
        }
      }
    }

    public static async Task<ScoreData[]> GetList(
      string levelName,
      bool minSpeed = true,
      string playerName = "",
      bool descending = false)
    {
      string listID = levelName + "%" + minSpeed.ToString() + "%" + playerName + "%" + descending.ToString();
      ScoreData[] cachedList;
      if (ScoreManager.cachedLists.TryGetValue(listID, out cachedList))
        return cachedList;
      string response;
      try
      {
        response = await HttpManager.GetStringAsync(VarManager.GetString("baseurl") 
            + "Scores/List.php?search=" + (minSpeed ? nameof (minSpeed) : "minLines")
            + "&levelname=" + levelName + (!string.IsNullOrEmpty(playerName) 
            ? "&playername=" + playerName : "") + (descending ? "&descending=true" : ""));
      }
      catch
      {
        response = "";
      }
      if (!string.IsNullOrEmpty(response))
      {
        string[] parts = response.Split('*');
        ScoreData[] list = new ScoreData[parts.Length];
        for (int i = 0; i < parts.Length; ++i)
          list[i] = ScoreManager.ParseScoreData(parts[i]);
        ScoreManager.cachedLists.TryAdd(listID, list);
        return list;
      }
      response = (string) null;
      return (ScoreData[]) null;
    }

    public static ScoreData TryGetOverview(string levelName, bool personal = false)
    {
      ScoreData overview;
      if (!ScoreManager.Instance.overviews.TryGetValue((personal ? "personal%" : "") + levelName, out overview))
        Console.WriteLine("Failed to get " + (personal ? "personal " : "") + "Overview for " + levelName);
      return overview;
    }

    private static async Task<ScoreData> GetOverview(string levelName, bool personal = false)
    {
      ScoreData data = new ScoreData();
      string response;
      try
      {
        response = await HttpManager.GetStringAsync(VarManager.GetString("baseurl") + "Scores/Overview.php?levelname=" + levelName + (personal ? "&uuid=" + VarManager.GetString("uuid") : ""));
      }
      catch
      {
        response = "";
      }
      if (!string.IsNullOrEmpty(response))
      {
        string[] val = response.Split('%');
        int time;
        data.time = int.TryParse(val[0], out time) ? (float) time / 1000f : -1f;
        int lineCount;
        data.lineCount = int.TryParse(val[1], out lineCount) ? lineCount : -1;
        int averageTime;
        data.averageTime = int.TryParse(val[2], out averageTime) ? (float) averageTime / 1000f : -1f;
        int averageLineCount;
        data.averageLineCount = int.TryParse(val[3], out averageLineCount) ? averageLineCount : -1;
        val = (string[]) null;
      }
      ScoreData overview = data;
      response = (string) null;
      data = (ScoreData) null;
      return overview;
    }

    public static async Task<ScoreData> GetRandom(string levelName)
    {
      string response;
      try
      {
        response = await HttpManager.GetStringAsync(VarManager.GetString("baseurl") + "Scores/Random.php?levelname=" + levelName);
      }
      catch
      {
        response = "";
      }
      ScoreData scoreData = ScoreManager.ParseScoreData(response, true);
      response = (string) null;
      return scoreData;
    }

    public static async Task<string> GetScriptById(int databaseId)
    {
      string response;
      try
      {
        response = await HttpManager.GetStringAsync(VarManager.GetString("baseurl") + "Scores/Get.php?id=" + databaseId.ToString() + (VarManager.GetBool("silentmode") ? "&silent=1" : ""));
      }
      catch
      {
        response = "";
      }
      string scriptById = response;
      response = (string) null;
      return scriptById;
    }

    private static ScoreData ParseScoreData(string dataString, bool containsScript = false)
    {
      ScoreData scoreData = new ScoreData();
      if (!string.IsNullOrEmpty(dataString))
      {
        string[] strArray = dataString.Split('%');
        int result1;
        scoreData.time = int.TryParse(strArray[0], out result1) ? (float) result1 / 1000f : -1f;
        int result2;
        scoreData.lineCount = int.TryParse(strArray[1], out result2) ? result2 : -1;
        scoreData.playerName = strArray[2];
        if (containsScript)
        {
          scoreData.script = strArray[3];
        }
        else
        {
          int result3;
          scoreData.databaseId = int.TryParse(strArray[3], out result3) ? result3 : -1;
        }
        if (strArray.Length == 5)
        {
          uint result4;
          scoreData.publishDuration = uint.TryParse(strArray[4], out result4) ? result4 : 0U;
        }
      }
      return scoreData;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.VarManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

#nullable enable
namespace BotsAreStupid
{
  internal class VarManager
  {
    public static 
    #nullable disable
    VarManager Instance;
    private static string[] protectedVars = new string[6]
    {
      "uuid",
      "username",
      "baseurl",
      "authkey",
      "windowsize",
      "demolock"
    };
    private static List<Tuple<string, VarManager.VarChangeHandler>> changeHandlers;
    private Dictionary<string, int> ints = new Dictionary<string, int>();
    private List<VarAnimation> animations = new List<VarAnimation>();
    private Dictionary<string, bool> bools = new Dictionary<string, bool>();
    private Dictionary<string, string> strings = new Dictionary<string, string>();

    public static bool IsOverclocked
    {
      get => VarManager.HasInt("overclock") && VarManager.GetInt("overclock") > 10;
    }

    public VarManager(bool loadOptions = true)
    {
      VarManager.Instance = this;
      VarManager.SetString("version", "1.6");
      VarManager.SetInt("demolock", 6);
      VarManager.SetInt("gravity", 580);
      VarManager.SetInt("hookrange", 300);
      VarManager.SetString("baseurl", "http://bas.leleg.de/game/");
      VarManager.SetInt("overclock", 1);
      VarManager.SetInt("randombotspercent", 50);
      VarManager.SetInt("skipspeed", 20);
      VarManager.SetInt("conveyormultiplier", 70);
      VarManager.SetInt("conveyoracceleration", 10);
      VarManager.SetString("forceflag", "--f");
      VarManager.SetInt("linefadetime", 200);
      VarManager.SetInt("stepheight", 50);
      VarManager.SetInt("slowmotion", 1);
      VarManager.SetInt("defaulttickrate", 200);
      VarManager.SetInt("mintickrate", 50);
      VarManager.SetInt("maxtickrate", 1000);
      VarManager.SetInt("maxplayerlifetime", 999);
      VarManager.SetInt("viewscale", 100);
      VarManager.SetInt("viewoffsetx", 0);
      VarManager.SetInt("viewoffsety", 0);
      VarManager.SetString("basedirectory", Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "/");
      VarManager.SetString("tempdirectory", AppContext.BaseDirectory);
      VarManager.SetString("appdirectory", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/BotsAreStupid/");
      VarManager.SetInt("levelwidth", 940);
      VarManager.SetInt("levelheight", 800);
      VarManager.SetInt("minleveluiwidth", 340);
      VarManager.SetInt("minsidebarwidth", 140);
      VarManager.SetInt("leveluiwidth", VarManager.GetInt("minleveluiwidth"));
      VarManager.SetInt("leveluiheight", 800);
      VarManager.SetString("contentdirectory", VarManager.GetString("tempdirectory") + "Content/");
      VarManager.SetString("defaultlevelsdirectory", VarManager.GetString("contentdirectory") + "Levels/");
      VarManager.SetString("customlevelsdirectory", VarManager.GetString("appdirectory") + "CustomLevels/");
      VarManager.SetString("scriptdirectory", VarManager.GetString("appdirectory") + "Scripts/");
      VarManager.SetString("envversion", Environment.Version.ToString());
      VarManager.SetInt("autosaveinterval", 10);
      if (!loadOptions)
        return;
      this.LoadOptions();
    }

    public void Update(float deltaTime)
    {
      for (int index = this.animations.Count - 1; index >= 0; --index)
      {
        VarAnimation animation = this.animations[index];
        animation.timeLeft -= deltaTime;
        if ((double) animation.timeLeft <= 0.0)
        {
          VarManager.SetInt(animation.varName, animation.endValue);
          this.animations.Remove(animation);
        }
        else
        {
          float px = (float) (1.0 - (double) animation.timeLeft / (double) animation.duration);
          int num = (int) Utility.Interpolate((float) animation.startValue, (float) animation.endValue, px);
          VarManager.SetInt(animation.varName, num);
        }
      }
    }

    public static void SetInt(
      string name,
      int value,
      bool ignoreProtection = true,
      bool ignoreListeners = false,
      bool addToValue = false)
    {
      if (!ignoreProtection && VarManager.IsProtected(name))
        return;
      if (VarManager.Instance.ints.ContainsKey(name))
      {
        if (addToValue)
          VarManager.Instance.ints[name] += value;
        else
          VarManager.Instance.ints[name] = value;
      }
      else
        VarManager.Instance.ints.Add(name, value);
      if (ignoreListeners)
        return;
      VarManager.CheckListeners(name, value.ToString());
    }

    public static int GetInt(string name)
    {
      int num;
      VarManager.Instance.ints.TryGetValue(name, out num);
      return num;
    }

    public static bool HasInt(string name) => VarManager.Instance.ints.ContainsKey(name);

    public static void DeleteInt(string name)
    {
      if (!VarManager.Instance.ints.ContainsKey(name))
        return;
      VarManager.Instance.ints.Remove(name);
    }

    public static void AnimateInt(string name, int targetValue, float duration = 1f)
    {
      VarManager.Instance.animations.RemoveAll((Predicate<VarAnimation>) (animation => animation.varName == name));
      VarManager.Instance.animations.Add(new VarAnimation(name, VarManager.GetInt(name), targetValue, duration));
    }

    public static void SetBool(
      string name,
      bool value,
      bool ignoreProtection = true,
      bool ignoreListeners = false)
    {
      if (!ignoreProtection && VarManager.IsProtected(name))
        return;
      if (VarManager.Instance.bools.ContainsKey(name))
        VarManager.Instance.bools[name] = value;
      else
        VarManager.Instance.bools.Add(name, value);
      if (ignoreListeners)
        return;
      VarManager.CheckListeners(name, value.ToString());
    }

    public static void ToggleBool(string name, bool ignoreProtection = true, bool ignoreListeners = false)
    {
      if (!ignoreProtection && VarManager.IsProtected(name))
        return;
      VarManager.Instance.bools[name] = !VarManager.Instance.bools.ContainsKey(name) || !VarManager.Instance.bools[name];
      if (ignoreListeners)
        return;
      VarManager.CheckListeners(name, VarManager.Instance.bools[name].ToString());
    }

    public static bool GetBool(string name)
    {
      bool flag;
      VarManager.Instance.bools.TryGetValue(name, out flag);
      return flag;
    }

    public static bool HasBool(string name) => VarManager.Instance.bools.ContainsKey(name);

    public static void SetString(
      string name,
      string value,
      bool ignoreProtection = true,
      bool ignoreListeners = false)
    {
      if (!ignoreProtection && VarManager.IsProtected(name))
        return;
      if (VarManager.Instance.strings.ContainsKey(name))
        VarManager.Instance.strings[name] = value;
      else
        VarManager.Instance.strings.Add(name, value);
      if (ignoreListeners)
        return;
      VarManager.CheckListeners(name, value);
    }

    public static string GetString(string name)
    {
      string str;
      if (!VarManager.Instance.strings.TryGetValue(name, out str))
        Console.WriteLine("[VarManager] Tried to get unknown string '" + name + "'");
      return str;
    }

    public static bool HasString(string name) => VarManager.Instance.strings.ContainsKey(name);

    public static void AddListener(string name, VarManager.VarChangeHandler changeHandler)
    {
      VarManager.CheckListeners((string) null, (string) null);
      VarManager.changeHandlers.Add(Tuple.Create<string, VarManager.VarChangeHandler>(name, changeHandler));
    }

    public void AfterLoadCheck()
    {
      if (VarManager.GetString("uuid") == null)
        VarManager.SetString("uuid", Guid.NewGuid().ToString());
      if (VarManager.HasString("levelkey"))
        VarManager.SetString("authkey", VarManager.GetString("levelkey"));
      if (VarManager.GetString("authkey") == null)
        VarManager.SetString("authkey", Guid.NewGuid().ToString());
      if (VarManager.GetString("autoaddghosttype") == null)
        VarManager.SetString("autoaddghosttype", "random");
      if (VarManager.GetBool("keyrequested"))
      {
        HttpManager.Post(VarManager.GetString("baseurl") + "Players/RegisterKey.php", new Dictionary<string, string>()
        {
          {
            "uuid",
            VarManager.GetString("uuid")
          },
          {
            "authkey",
            VarManager.GetString("authkey")
          }
        });
        VarManager.SetBool("keyrequested", false);
      }
      string str = VarManager.GetString("username");
      if (str == null)
      {
        this.PromptUsername("Welcome!   Enter your username:");
      }
      else
      {
        string errorMessage;
        if (!this.UsernameValidator(str, out errorMessage))
          this.PromptUsername("Hey!   Enter a new username:", str, errorMessage);
        else
          this.TryRegisterPlayer();
      }
      if (!VarManager.HasInt("mastervolume"))
        VarManager.SetInt("mastervolume", 50);
      if (!VarManager.HasInt("sfxvolume"))
        VarManager.SetInt("sfxvolume", 50);
      if (!VarManager.HasInt("musicvolume"))
        VarManager.SetInt("musicvolume", 25);
      if (!VarManager.HasInt("particleamount"))
        VarManager.SetInt("particleamount", 2);
      if (!VarManager.HasInt("unlockedlevels"))
        VarManager.SetInt("unlockedlevels", 1);
      if (!VarManager.HasBool("fullscreen"))
        VarManager.SetBool("fullscreen", true);
      if (VarManager.GetString("windowsize") == null)
        VarManager.SetString("windowsize", "1280x800");
      if (!VarManager.HasInt("textsize"))
        VarManager.SetInt("textsize", 2);
      if (!VarManager.HasInt("savespersecond"))
        VarManager.SetInt("savespersecond", 20);
      if (!VarManager.HasBool("livescriptchanges"))
        VarManager.SetBool("livescriptchanges", true);
      if (!VarManager.HasInt("editoruiscale"))
        VarManager.SetInt("editoruiscale", 100);
      if (!VarManager.HasString("colorscheme"))
        VarManager.SetString("colorscheme", "default");
      this.CheckMigrations();
      VarManager.SaveOptions();
    }

    public void PromptUsername(string msgOverwrite = null, string fieldValue = null, string errorMsg = null)
    {
      PopupMenu.Instance?.Prompt(msgOverwrite ?? "Enter username:", (PromptMenu.InteractionHandler) null, (PromptMenu.InteractionHandlerString) (s =>
      {
        VarManager.SetString("username", s);
        this.TryRegisterPlayer();
        PopupMenu.Instance.SetActive(false);
        LevelManager.UpdateCustomCreatorNames(s);
        VarManager.SaveOptions();
      }), new PromptMenu.Validator(this.UsernameValidator), fieldValue, errorMsg);
    }

    private async void TryRegisterPlayer()
    {
      string str = await HttpManager.Post(VarManager.GetString("baseurl") + "Players/Register.php", new Dictionary<string, string>()
      {
        {
          "name",
          VarManager.GetString("username")
        },
        {
          "uuid",
          VarManager.GetString("uuid")
        },
        {
          "authkey",
          VarManager.GetString("authkey")
        }
      });
    }

    private void LoadOptions()
    {
      string str1 = VarManager.GetString("appdirectory") + "data";
      if (File.Exists(str1))
      {
        foreach (string str2 in Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(str1))).Split('\n'))
        {
          if (!string.IsNullOrEmpty(str2))
          {
            string[] strArray = str2.Split('=');
            int result1;
            if (int.TryParse(strArray[1], out result1))
            {
              VarManager.SetInt(strArray[0], result1);
            }
            else
            {
              bool result2;
              if (bool.TryParse(strArray[1], out result2))
                VarManager.SetBool(strArray[0], result2);
              else
                VarManager.SetString(strArray[0], strArray[1]);
            }
          }
        }
      }
      this.AfterLoadCheck();
    }

    private bool UsernameValidator(string name, out string errorMessage)
    {
      if (name.Length < 3)
      {
        errorMessage = "Name too short! (min 3 symbols)";
        return false;
      }
      if (name.Length > 14)
      {
        errorMessage = "Name too long! (max 14 symbols)";
        return false;
      }
      bool result;
      if (bool.TryParse(HttpManager.GetString(VarManager.GetString("baseurl") + "Players/CheckName.php?name=" + name + "&uuid=" + VarManager.GetString("uuid")), out result) & result)
      {
        errorMessage = "Already taken!";
        return false;
      }
      string errorMessage1;
      if (!Utility.CussWordValidator(name, out errorMessage1))
      {
        errorMessage = errorMessage1;
        return false;
      }
      errorMessage = "null";
      return true;
    }

    private static bool IsProtected(string name)
    {
      if (VarManager.protectedVars == null)
        return false;
      foreach (string protectedVar in VarManager.protectedVars)
      {
        if (protectedVar == name.ToLower())
          return true;
      }
      return false;
    }

    public static void SaveOptions()
    {
      string str1 = "" + "dataversion=" + VarManager.GetString("version") + "\n" + "mastervolume=" + VarManager.GetInt("mastervolume").ToString() + "\n" + "sfxvolume=" + VarManager.GetInt("sfxvolume").ToString() + "\n" + "musicvolume=" + VarManager.GetInt("musicvolume").ToString() + "\n" + "username=" + VarManager.GetString("username") + "\n" + "uuid=" + VarManager.GetString("uuid") + "\n" + "authkey=" + VarManager.GetString("authkey") + "\n" + "fullscreen=" + VarManager.GetBool("fullscreen").ToString() + "\n" + "windowsize=" + VarManager.GetString("windowsize") + "\n" + "particleamount=" + VarManager.GetInt("particleamount").ToString() + "\n" + "unlockedlevels=" + VarManager.GetInt("unlockedlevels").ToString() + "\n" + "hasplayedintro=" + VarManager.GetBool("hasplayedintro").ToString() + "\n" + "textsize=" + VarManager.GetInt("textsize").ToString() + "\n" + "autoaddghost=" + VarManager.GetBool("autoaddghost").ToString() + "\n" + "autoaddghosttype=" + VarManager.GetString("autoaddghosttype") + "\n" + "successpersonalinfo=" + VarManager.GetBool("successpersonalinfo").ToString() + "\n" + "livescriptchanges=" + VarManager.GetBool("livescriptchanges").ToString() + "\n" + "savespersecond=" + VarManager.GetInt("savespersecond").ToString() + "\n" + "editoruiscale=" + VarManager.GetInt("editoruiscale").ToString() + "\n" + "colorscheme=" + VarManager.GetString("colorscheme") + "\n";
      if (VarManager.HasBool("silentmode"))
        str1 = str1 + "silentmode=" + VarManager.GetBool("silentmode").ToString() + "\n";
      if (VarManager.HasBool("godmode"))
        str1 = str1 + "godmode=" + VarManager.GetBool("godmode").ToString() + "\n";
      string str2 = str1 + VarManager.Instance.AutoGetSaveBools();
      Directory.CreateDirectory(VarManager.GetString("appdirectory"));
      File.WriteAllText(VarManager.GetString("appdirectory") + "data", Convert.ToBase64String(Encoding.UTF8.GetBytes(str2)));
    }

    public string AutoGetSaveBools()
    {
      string saveBools = "";
      foreach (KeyValuePair<string, bool> keyValuePair in this.bools)
      {
        if (keyValuePair.Key.StartsWith("hasplayedintro_"))
          saveBools = saveBools + keyValuePair.Key + "=" + keyValuePair.Value.ToString() + "\n";
      }
      return saveBools;
    }

    private static void CheckListeners(string name, string value)
    {
      if (VarManager.changeHandlers == null)
        VarManager.changeHandlers = new List<Tuple<string, VarManager.VarChangeHandler>>();
      if (name == null && value == null)
        return;
      foreach (Tuple<string, VarManager.VarChangeHandler> changeHandler in VarManager.changeHandlers)
      {
        if (name == changeHandler.Item1)
        {
          VarManager.VarChangeHandler varChangeHandler = changeHandler.Item2;
          if (varChangeHandler != null)
            varChangeHandler(value);
        }
      }
    }

    private void CheckMigrations()
    {
      string version1 = VarManager.GetString("version");
      if (!VarManager.HasString("dataversion"))
      {
        VarManager.SetString("dataversion", version1);
      }
      else
      {
        Version version = new Version(version1);
        Version dataVersion = new Version(VarManager.GetString("dataversion"));
        if (dataVersion.CompareTo(version) < 0)
        {
          checkMigration("1.4", (System.Action) (() =>
          {
            ScriptManager.RenameSaveFileById("9b95c449-5ad7-46a4-b0c1-fe59ca7b3807", "Basic4");
            ScriptManager.RenameSaveFileById("ee8e8b40-37f8-4ed3-863d-23cb16e0d42f", "Basic5");
            ScriptManager.RenameSaveFileById("cff339a0-9f1e-49b1-a797-343b8baaa111", "Easy4");
            ScriptManager.RenameSaveFileById("3b75a2bf-626a-4b71-a0f3-1d4cc55c86d3", "Medium3");
            ScriptManager.RenameSaveFileById("1800c1d0-4255-418c-a89d-df896fc9a154", "Medium2");
          }));
          checkMigration("1.5", (System.Action) (() => ScriptManager.ReplaceInAllScripts(("-tickrate\\s+50", "-tickrate low"), ("-tickrate\\s+128", "-tickrate legacy"), ("-tickrate\\s+1000", "-tickrate high"))));
        }
        VarManager.SetString("dataversion", version1);

        void checkMigration(string checkVersionString, System.Action action)
        {
          Version version = new Version(checkVersionString);
          if (dataVersion.CompareTo(version) >= 0 || version.CompareTo(version) < 0)
            return;
          Console.WriteLine("Performing " + checkVersionString + " Migration");
          if (action != null)
            action();
        }
      }
    }

    public delegate void VarChangeHandler(string value);
  }
}

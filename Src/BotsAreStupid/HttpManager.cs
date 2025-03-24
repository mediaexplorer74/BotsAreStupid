// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.HttpManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

#nullable enable
namespace BotsAreStupid
{
  internal class HttpManager
  {
    public static 
    #nullable disable
    HttpManager Instance = new HttpManager();
    private readonly HttpClient client = new HttpClient();

    public static bool HasConnection { private set; get; } = false;

    public async void CheckConnection(HttpManager.Callback callback = null)
    {
      try
      {
        string response = await HttpManager.Instance.client.GetStringAsync(VarManager.GetString("baseurl") + "Check.php?name=" + VarManager.GetString("username") + "&uuid=" + VarManager.GetString("uuid") + (VarManager.HasBool("silentmode") ? "&silent=1" : ""));
        if (!string.IsNullOrEmpty(response))
        {
          string[] args = response.Split(' ');
          if (args.Length > 3 && args[2] == "Info:")
          {
            string info = "";
            for (int i = 3; i < args.Length; ++i)
              info = info + args[i] + " ";
            PopupMenu.Instance.Inform(info, new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), false);
            info = (string) null;
          }
          if (args.Length == 3 && args[2] == "--requestkey")
            VarManager.SetBool("keyrequested", true);
          bool value;
          if (bool.TryParse(args[0], out value) & value)
          {
            HttpManager.HasConnection = true;
            string currentVersion = VarManager.GetString("version");
            if (args[1] != currentVersion)
              PopupMenu.Instance?.Inform("An update is available:*" + currentVersion + " > " + args[1], new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), false);
            HttpManager.Callback callback1 = callback;
            if (callback1 != null)
              callback1();
            MainMenu.Instance.SetConnectedToServer(true);
            return;
          }
          args = (string[]) null;
        }
        response = (string) null;
      }
      catch
      {
        HttpManager.HasConnection = false;
      }
      HttpManager.HasConnection = false;
      MainMenu.Instance.SetConnectedToServer(false);
      HttpManager.Callback callback2 = callback;
      if (callback2 == null)
        return;
      callback2();
    }

    public static async Task<string> GetStringAsync(string url)
    {
      int num = 0;
      if (num != 0 && !HttpManager.HasConnection)
        return "";
      string response;
      try
      {
        response = await HttpManager.Instance.client.GetStringAsync(url);
      }
      catch
      {
        response = "";
      }
      return response;
    }

    public static string GetString(string url)
    {
      if (!HttpManager.HasConnection)
        return "";
      string str;
      try
      {
        str = HttpManager.Instance.client.GetStringAsync(url).Result;
      }
      catch
      {
        str = "";
      }
      return str;
    }

    public static async Task<string> Post(
      string url,
      Dictionary<string, string> content,
      HttpManager.Callback callback = null)
    {
      if (!HttpManager.HasConnection)
        return "Error: Cant connect to server";
      try
      {
        HttpResponseMessage response = await HttpManager.Instance.client.PostAsync(url, (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) content));
        HttpManager.Callback callback1 = callback;
        if (callback1 != null)
          callback1();
        string str = await response.Content.ReadAsStringAsync();
        return str;
      }
      catch
      {
        return "Error: Cant connect to server";
      }
    }

    public delegate void Callback();
  }
}

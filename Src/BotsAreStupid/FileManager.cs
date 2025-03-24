// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.FileManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Globalization;
using System.Net;
using System.Text;

#nullable disable
namespace BotsAreStupid
{
  internal class FileManager
  {
    public static FileManager Instance = new FileManager();
    private readonly WebClient client = new WebClient();

    public static void DownloadLevel(LevelInfo levelInfo)
    {
      try
      {
        FileManager.Instance.client.DownloadFile(VarManager.GetString("baseurl") + "Levels/Download.php?levelname=" + levelInfo.name + "&uuid=" + VarManager.GetString("uuid"), levelInfo.path);
      }
      catch
      {
        PopupMenu.Instance.Inform("Error: Unexpectedly failed download", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)));
      }
    }

    public static void UploadLevel(LevelInfo levelInfo)
    {
      try
      {
        string info = "Error: Not connected to server";
        if (HttpManager.HasConnection)
        {
          byte[] numArray = FileManager.Instance.client.UploadFile(VarManager.GetString("baseurl") + "Levels/Upload.php", "POST", levelInfo.path);
          info = Encoding.UTF8.GetString(numArray, 0, numArray.Length);
        }
        if (!info.StartsWith("error", true, (CultureInfo) null) && !info.StartsWith("success", true, (CultureInfo) null))
          return;
        PopupMenu.Instance.Inform(info, (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)));
      }
      catch
      {
        PopupMenu.Instance.Inform("Error: Cant connect to server", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)));
      }
    }

    public static void TryGetThumbnail(string levelName, Action<Texture2D> callback)
    {
    }
  }
}

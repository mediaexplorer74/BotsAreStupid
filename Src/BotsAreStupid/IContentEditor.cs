// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.IContentEditor
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;

#nullable disable
namespace BotsAreStupid
{
  internal interface IContentEditor
  {
    event IContentEditor.ViewUpdateHandler OnViewUpdate;

    void UpdateView(bool ignoreContext = false);

    object AddLine(string lineContent, Vector2? position = null, bool force = false);

    void AddContent(string[] newContent);

    string[] GetContent();

    string GetLineString(int index);

    int LineCount { get; }

    void SetFirstEditableLine(int index);

    Rectangle GetGlobalContentRect();

    void Clear(bool addEmpty = true);

    void ShowLine(int index, bool loadSaved);

    int GetLineFromPos(
      Vector2? position = null,
      bool clampToCount = true,
      bool rounded = false,
      int minFirstEditableLineOffset = -1);

    void SaveScroll();

    void LoadScroll();

    delegate void ViewUpdateHandler();
  }
}

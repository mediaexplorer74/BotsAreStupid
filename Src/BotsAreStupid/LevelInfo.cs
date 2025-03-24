// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.LevelInfo
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal class LevelInfo : IListElement
  {
    public string name;
    public string path;
    public bool local;
    public int downloads;
    public string creator;
    public ListElementActionType[] availableActions;

    public object GetValue(int id)
    {
      switch (id)
      {
        case 0:
          return (object) this.name;
        case 1:
          return (object) this.creator;
        case 2:
          return (object) this.downloads;
        case 3:
          return (object) this.path;
        case 4:
          return (object) this.local;
        default:
          return (object) null;
      }
    }

    public ListElementAction[] GetAvailableActions()
    {
      ListElementAction[] availableActions = new ListElementAction[this.availableActions.Length];
      for (int index = 0; index < this.availableActions.Length; ++index)
        availableActions[index] = new ListElementAction(this.availableActions[index]);
      return availableActions;
    }
  }
}

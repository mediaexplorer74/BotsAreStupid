// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScoreData
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class ScoreData : IListElement
  {
    public float time;
    public int lineCount;
    public string playerName;
    public int databaseId = -1;
    public uint publishDuration;
    public float averageTime;
    public int averageLineCount;
    public string script;
    public ListElementActionType[] availableActions = new ListElementActionType[2]
    {
      ListElementActionType.Watch,
      ListElementActionType.More
    };
    private bool canBeWatched = true;

    public object GetValue(int id)
    {
      switch (id)
      {
        case 0:
          return (object) this.playerName;
        case 1:
          return (object) this.time;
        case 2:
          return (object) this.lineCount;
        case 3:
          return (object) this.publishDuration;
        case 4:
          return (object) this.databaseId;
        case 5:
          return (object) this.averageTime;
        case 6:
          return (object) this.averageLineCount;
        default:
          return (object) null;
      }
    }

    public ListElementAction[] GetAvailableActions()
    {
      if (!this.canBeWatched)
        return new ListElementAction[1]
        {
          new ListElementAction(ListElementActionType.More)
        };
      List<ListElementAction> listElementActionList = new List<ListElementAction>();
      for (int index = 0; index < this.availableActions.Length; ++index)
        listElementActionList.Add(new ListElementAction(this.availableActions[index]));
      if (this.playerName.Equals(VarManager.GetString("username"), StringComparison.InvariantCultureIgnoreCase))
        listElementActionList.Add(new ListElementAction(ListElementActionType.Remove));
      return listElementActionList.ToArray();
    }

    public void SetCanBeWatched(bool enabled) => this.canBeWatched = enabled;
  }
}

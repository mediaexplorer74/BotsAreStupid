// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Script
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class Script : IListElement
  {
    public string name;
    public string[] instructions;
    public DateTime lastSaved;
    public string finishTime;
    public bool isCurrent;
    private bool canBeLoaded = true;
    public ListElementActionType[] availableActions = new ListElementActionType[3]
    {
      ListElementActionType.LoadScript,
      ListElementActionType.Rename,
      ListElementActionType.Delete
    };

    public object GetValue(int id)
    {
      switch (id)
      {
        case 0:
          return (object) this.name;
        case 1:
          return (object) this.finishTime;
        case 2:
          return (object) this.lastSaved;
        case 3:
          return (object) this.instructions;
        default:
          return (object) null;
      }
    }

    public void SetCanBeLoaded(bool canBeLoaded) => this.canBeLoaded = canBeLoaded;

    public ListElementAction[] GetAvailableActions()
    {
      if (this.isCurrent)
        return new ListElementAction[1]
        {
          new ListElementAction(ListElementActionType.Rename)
        };
      List<ListElementAction> listElementActionList = new List<ListElementAction>();
      listElementActionList.Add(new ListElementAction(ListElementActionType.Rename));
      if (!this.isCurrent && this.canBeLoaded)
      {
        listElementActionList.Add(new ListElementAction(ListElementActionType.LoadScript));
        listElementActionList.Add(new ListElementAction(ListElementActionType.Add));
      }
      if (!this.isCurrent)
        listElementActionList.Add(new ListElementAction(ListElementActionType.Delete));
      return listElementActionList.ToArray();
    }
  }
}

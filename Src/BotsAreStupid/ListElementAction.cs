// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ListElementAction
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal struct ListElementAction(ListElementActionType type, ListElementAction.Action action = null)
  {
    public ListElementActionType type = type;
    public ListElementAction.Action action = action;

    public delegate void Action(IListElement element);
  }
}

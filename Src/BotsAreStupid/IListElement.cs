// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.IListElement
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal interface IListElement
  {
    object GetValue(int id);

        /*
    bool GetValue<T>(int id, out T value)
    {
      object obj = this.GetValue(id);
      if (obj.GetType() == typeof (T))
      {
        value = (T) obj;
        return true;
      }
      value = default (T);
      return false;
    }*/

    ListElementAction[] GetAvailableActions();
  }
}

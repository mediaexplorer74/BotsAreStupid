// Decompiled with JetBrains decompiler
// Type: ColumnInfo
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
internal struct ColumnInfo(
  string name,
  float widthPercentage,
  bool isSearchable,
  bool isSortDisplayReversed,
  bool isInteractable = true)
{
  public string name = name;
  public float widthPercentage = widthPercentage;
  public bool isSearchable = isSearchable;
  public bool isInteractable = isInteractable;
  public bool isSortDisplayReversed = isSortDisplayReversed;
}

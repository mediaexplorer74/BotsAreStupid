// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.LevelMenu
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System.Collections.Generic;
using System.IO;

#nullable disable
namespace BotsAreStupid
{
  internal class LevelMenu : UIElement
  {
    private LevelList levelList;
    private UIElement header;
    private const int padding = 30;
    private const int headerHeight = 58;
    private const int shiftAddInfoHeight = 50;
    private static readonly ColumnInfo[] columnInfos = new ColumnInfo[3]
    {
      new ColumnInfo("No.", 0.1f, false, false),
      new ColumnInfo("Name", 0.65f, true, false),
      new ColumnInfo("Actions", 0.25f, false, false)
    };

    public LevelMenu(Microsoft.Xna.Framework.Rectangle rectangle)
      : base(rectangle)
    {
      Microsoft.Xna.Framework.Color color = ColorManager.GetColor("white");
      this.AddChild(this.header = new UIElement(
          new Microsoft.Xna.Framework.Rectangle(58, 0, rectangle.Width - 116, 58)));
      this.AddChild((UIElement) (this.levelList 
          = new LevelList(new Microsoft.Xna.Framework.Rectangle
          (0, 58, rectangle.Width, rectangle.Height - 58 - 4), 
          new ListElementActionType[2]
      {
        ListElementActionType.Load,
        ListElementActionType.Delete
      }, "Nothing here yet...", rowAmount: 7, columnInfosOverride: LevelMenu.columnInfos)));
      this.levelList.Initialize();
    }

    public void ShowList()
    {
      Directory.CreateDirectory(VarManager.GetString("customlevelsdirectory"));
      string[] files = Directory.GetFiles(VarManager.GetString("customlevelsdirectory"));
      List<IListElement> listElementList = new List<IListElement>();
      foreach (string path in files)
      {
        string rawLevelName = Utility.GetRawLevelName(path);
        string levelCreatorName = Utility.GetLevelCreatorName(path);
        if (Utility.GetLevelCreatorUUID(path) == VarManager.GetString("uuid"))
        {
          ListElementActionType[] elementActionTypeArray1;
          if (!(rawLevelName == LevelManager.CurrentLevelNameSimple))
            elementActionTypeArray1 = new ListElementActionType[2]
            {
              ListElementActionType.Load,
              ListElementActionType.Delete
            };
          else
            elementActionTypeArray1 = new ListElementActionType[0];
          ListElementActionType[] elementActionTypeArray2 = elementActionTypeArray1;
          listElementList.Add((IListElement) new LevelInfo()
          {
            name = rawLevelName,
            local = true,
            creator = levelCreatorName,
            path = path,
            availableActions = elementActionTypeArray2
          });
        }
      }
      this.levelList.SetElements(listElementList.ToArray());
      this.levelList.UpdateView();
    }
  }
}

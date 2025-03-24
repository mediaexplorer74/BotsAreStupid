// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.LevelSelection
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;
using System.IO;

#nullable enable
namespace BotsAreStupid
{
  internal class LevelSelection : BasicTabMenu
  {
    private const int mainOuterMargin = 70;
    private const int horizontalSpacing = 70;
    private const int verticalSpacing = 60;
    private const int customLevelPadding = 20;
    private const int customLevelTabHeight = 60;
    private Rectangle mainRect;
    private bool localCustomLevelListActive = true;
    private 
    #nullable disable
    Button localTabButton;
    private Button onlineTabButton;
    private LevelList localCustomLevelList;
    private LevelList onlineCustomLevelList;

    public static LevelSelection Instance { get; private set; }

    public LevelSelection(Rectangle rectangle)
      : base(rectangle, GameState.LevelSelection, new Styling?(), (Button.OnClick) null, "Default", "Custom")
    {
      LevelSelection.Instance = this;
      this.CreateChildren();
      this.UpdateTeaser();
    }

    public void UpdateTeaser()
    {
      this.UpdateCustomLevels();
      foreach (LevelTeaser levelTeaser in UIElement.FindAll(type: typeof (LevelTeaser), descendantOf: this.main))
        levelTeaser.UpdateView();
      this.SetCustomLevelTab(this.localCustomLevelListActive);
    }

    private void CreateChildren()
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = ColorManager.GetColor("white");
      ColorManager.GetColor("lightslate");
      Color color3 = ColorManager.GetColor("darkslate");
      BitmapFont font = TextureManager.GetFont("megaMan2");
      this.mainRect = this.main.GetRectangle();
      List<LevelTeaser> levelTeaserList = new List<LevelTeaser>();
      foreach (string defaultLevel in LevelManager.DefaultLevels)
        levelTeaserList.Add(new LevelTeaser(defaultLevel));
      GridContainer gridContainer;
      this.main.AddChild((UIElement) (gridContainer = new GridContainer(new Rectangle(0, 70, this.mainRect.Width, this.mainRect.Height - 140), 330, 220, 70, 60, -25, 35, 1f)));
      gridContainer.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 70, parentRect.Width, parentRect.Height - 140)));
      gridContainer.SetElements((UIElement[]) levelTeaserList.ToArray());
      Styling style = new Styling()
      {
        centerText = true,
        //font = font,
        defaultTextColor = color2,
        defaultColor = color3,
        hoverColor = color3,
        clickColor = color1,
        borderWidth = 4,
        borderColor = color2
      };
      int num = this.mainRect.Width / 2;
      style.text = "Local";
      this.contentElements[1].AddChild((UIElement) (this.localTabButton = new Button(new Rectangle(0, 0, num, 60), (Button.OnClick) (() => this.SetCustomLevelTab(true)), style)));
      this.localTabButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width / 2, 60)));
      style.text = "Online";
      this.contentElements[1].AddChild((UIElement) (this.onlineTabButton = new Button(new Rectangle(num, 0, num, 60), (Button.OnClick) (() => this.SetCustomLevelTab(false)), style)));
      this.onlineTabButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2, 0, parentRect.Width / 2, 60)));
    
            //RnD
      /*this.contentElements[1].AddChild((UIElement) (this.localCustomLevelList
        = new LevelList(Rectangle.Empty, new ListElementActionType[4]
      {
        ListElementActionType.Play,
        ListElementActionType.Edit,
        ListElementActionType.Upload,
        ListElementActionType.Delete
      }, "Nothing here yet...\nCheck online levels?", (Button.OnClick) (() => this.SetCustomLevelTab(false)))));
      */
      
      this.localCustomLevelList.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => listRect(parentRect)));
      
      /*      
      this.contentElements[1].AddChild((UIElement) (this.onlineCustomLevelList = new LevelList(Rectangle.Empty, new ListElementActionType[3]
      {
        ListElementActionType.Download,
        ListElementActionType.Scores,
        ListElementActionType.Remove
      }, "Not connected to server...\nRetry connection?", (Button.OnClick) (() => HttpManager.Instance.CheckConnection((HttpManager.Callback) (() =>
      {
        if (!HttpManager.HasConnection)
          return;
        this.UpdateCustomLevels();
      }))))));
      */

      this.onlineCustomLevelList.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => listRect(parentRect)));
      this.onlineCustomLevelList.SetActive(false);
      this.localCustomLevelList.SetActive(false);

      static Rectangle listRect(Rectangle parentRect)
      {
        return new Rectangle(70, 130, parentRect.Width - 140 - 1, parentRect.Height - 200);
      }
    }

    public async void UpdateCustomLevels()
    {
      Directory.CreateDirectory(VarManager.GetString("customlevelsdirectory"));
      string[] localLevelPaths = Directory.GetFiles(VarManager.GetString("customlevelsdirectory"));
      string onlineLevelsResponse = await HttpManager.GetStringAsync(VarManager.GetString("baseurl") + "Levels/List.php");
      string[] onlineLevelsInfos = onlineLevelsResponse.Split('%');
      List<IListElement> onlineLevels = new List<IListElement>();
      string[] strArray1 = onlineLevelsInfos;
      for (int index1 = 0; index1 < strArray1.Length; ++index1)
      {
        string levelInfo = strArray1[index1];
        if (!string.IsNullOrEmpty(levelInfo))
        {
          string[] args = levelInfo.Split('|');
          if (args.Length != 0)
          {
            List<ListElementActionType> availableActions = new List<ListElementActionType>()
            {
              ListElementActionType.Scores
            };
            if (args[3] == VarManager.GetString("uuid"))
              availableActions.Add(ListElementActionType.Remove);
            bool existsLocally = false;
            string[] strArray2 = localLevelPaths;
            for (int index2 = 0; index2 < strArray2.Length; ++index2)
            {
              string path = strArray2[index2];
              if (Utility.GetRawLevelName(path) == args[0])
                existsLocally = true;
              path = (string) null;
            }
            strArray2 = (string[]) null;
            if (!existsLocally)
              availableActions.Add(ListElementActionType.Download);
            onlineLevels.Add((IListElement) new LevelInfo()
            {
              name = args[0],
              local = false,
              creator = args[1],
              downloads = int.Parse(args[2]),
              path = (VarManager.GetString("customlevelsdirectory") + args[0] + ".xml"),
              availableActions = availableActions.ToArray()
            });
            availableActions = (List<ListElementActionType>) null;
          }
          args = (string[]) null;
          levelInfo = (string) null;
        }
      }
      strArray1 = (string[]) null;
      List<IListElement> localLevels = new List<IListElement>();
      string[] strArray3 = localLevelPaths;
      for (int index = 0; index < strArray3.Length; ++index)
      {
        string levelPath = strArray3[index];
        string name = Utility.GetRawLevelName(levelPath);
        int downloads = 0;
        string creatorName = Utility.GetLevelCreatorName(levelPath);
        string creatorUUID = Utility.GetLevelCreatorUUID(levelPath);
        bool isCreator = creatorUUID == VarManager.GetString("uuid");
        List<ListElementActionType> availableActions = new List<ListElementActionType>()
        {
          ListElementActionType.Play,
          ListElementActionType.Delete
        };
        if (!this.LevelExists(onlineLevels, name) & isCreator)
          availableActions.Add(ListElementActionType.Upload);
        else
          downloads = this.GetDownloadCount(onlineLevels, name);
        if (isCreator)
          availableActions.Add(ListElementActionType.Edit);
        localLevels.Add((IListElement) new LevelInfo()
        {
          name = name,
          local = true,
          downloads = downloads,
          creator = creatorName,
          path = levelPath,
          availableActions = availableActions.ToArray()
        });
        name = (string) null;
        creatorName = (string) null;
        creatorUUID = (string) null;
        availableActions = (List<ListElementActionType>) null;
        levelPath = (string) null;
      }
      strArray3 = (string[]) null;
      this.localCustomLevelList.SetElements(localLevels.ToArray());
      this.localCustomLevelList.UpdateView();
      this.onlineCustomLevelList.SetElements(onlineLevels.ToArray());
      this.onlineCustomLevelList.UpdateView();
      localLevelPaths = (string[]) null;
      onlineLevelsResponse = (string) null;
      onlineLevelsInfos = (string[]) null;
      onlineLevels = (List<IListElement>) null;
      localLevels = (List<IListElement>) null;
    }

    private void SetCustomLevelTab(bool local)
    {
      this.localCustomLevelListActive = local;
      Color color = ColorManager.GetColor("lightslate");
      Styling newStyle1 = new Styling()
      {
        defaultColor = color,
        borderWidth = -1,
        clickColor = color,
        hoverColor = color,
        hoverCursor = new bool?(false)
      };
      Styling newStyle2 = new Styling()
      {
        defaultColor = ColorManager.GetColor("darkslate"),
        borderWidth = 4,
        clickColor = ColorManager.GetColor("red"),
        hoverCursor = new bool?(true)
      };
      if (local)
      {
        this.localCustomLevelList.SetActive(true);
        this.onlineCustomLevelList.SetActive(false);
        this.localTabButton.ChangeStyle(newStyle1);
        this.onlineTabButton.ChangeStyle(newStyle2);
      }
      else
      {
        this.localCustomLevelList.SetActive(false);
        this.onlineCustomLevelList.SetActive(true);
        this.localTabButton.ChangeStyle(newStyle2);
        this.onlineTabButton.ChangeStyle(newStyle1);
      }
    }

    private bool LevelExists(List<IListElement> collection, string name)
    {
      foreach (LevelInfo levelInfo in collection)
      {
        if (levelInfo.name == name)
          return true;
      }
      return false;
    }

    private int GetDownloadCount(List<IListElement> collection, string name)
    {
      foreach (LevelInfo levelInfo in collection)
      {
        if (levelInfo.name == name)
          return levelInfo.downloads;
      }
      return 0;
    }
  }
}

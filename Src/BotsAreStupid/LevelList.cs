// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.LevelList
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;

#nullable enable
namespace BotsAreStupid
{
  internal partial class LevelList : ListTable
  {
    private static readonly 
    #nullable disable
    ColumnInfo[] columnInfos = new ColumnInfo[5]
    {
      new ColumnInfo("No.", 0.05f, false, false),
      new ColumnInfo("Name", 0.275f, true, false),
      new ColumnInfo("Creator", 0.275f, true, false),
      new ColumnInfo("Downloads", 0.15f, false, true),
      new ColumnInfo("Actions", 0.25f, false, false)
    };
        private ListElementActionType[] listElementActionTypes;
        private string v;
        private ColumnInfo[] columnInfosOverride;

        // Fix for CS7036: Adding missing parameters to the constructor call
        public LevelList(Rectangle rectangle, ListElementActionType[] listElementActionTypes, string v, int rowAmount, ColumnInfo[] columnInfosOverride)
            : base(rectangle, columnInfosOverride ?? LevelList.columnInfos, LevelList.GetActions(listElementActionTypes), v, rowAmount, true, 0, true, new Styling?(), 0, true, null)
        {
            this.rectangle = rectangle;
            this.listElementActionTypes = listElementActionTypes;
            this.v = v;
            this.rowAmount = rowAmount;
            this.columnInfosOverride = columnInfosOverride;
        }

        
        public LevelList(Rectangle rectangle, ColumnInfo[] columnInfos, 
            ListElementAction[] actions, string emptyListInfoText = "Nothing here yet...", int secondSortParameter = 0, bool secondSortReversed = true, int rowAmount = -1, bool proportionalButtonSizes = false, Styling? style = null, int sortIndicatorHeight = -1, bool scrollbarEnabled = true, Button.OnClick emptyListInfoClickAction = null) : base(rectangle, columnInfos, actions, emptyListInfoText, secondSortParameter, secondSortReversed, rowAmount, proportionalButtonSizes, style, sortIndicatorHeight, scrollbarEnabled, emptyListInfoClickAction)
        {
        }

        // public LevelList()
        //     : this(new Rectangle(), new ListElementActionType[0], string.Empty)
        // {
        // }

        /*
        public LevelList(
          Rectangle rectangle,
          ListElementActionType[] actions,
          string emptyListInfoText,
          Button.OnClick emptyListInfoClickAction = null,
          int rowAmount = -1,
          ColumnInfo[] columnInfosOverride = null) //: this(new Rectangle(), new ListElementActionType[0], string.Empty)
        {
            Microsoft.Xna.Framework.Rectangle rectangle1 = rectangle;
            ColumnInfo[] columnInfos = columnInfosOverride ?? LevelList.columnInfos;
            ListElementAction[] actions1 = LevelList.GetActions(actions);
            string emptyListInfoText1 = emptyListInfoText;
            int rowAmount1 = rowAmount;
            Button.OnClick onClick = emptyListInfoClickAction;
            Styling? style = new Styling?();
            Button.OnClick emptyListInfoClickAction1 = onClick;

            // ISSUE: explicit constructor call
            //base.\u002Ector(rectangle1, columnInfos, actions1, emptyListInfoText1,
            //rowAmount: rowAmount1, proportionalButtonSizes: true, style: style, emptyListInfoClickAction: emptyListInfoClickAction1);

            //     
             public LevelList(
                Microsoft.Xna.Framework.Rectangle rectangle,
                ListElementActionType[] actions,
                string emptyListInfoText,
                Button.OnClick emptyListInfoClickAction = null,
                int rowAmount = -1,
                ColumnInfo[] columnInfosOverride = null)
                : base(rectangle, columnInfosOverride ?? LevelList.columnInfos, LevelList.GetActions(actions), emptyListInfoText, rowAmount, true, 0, true, new Styling?(), 0, true, emptyListInfoClickAction)
              {
                  this.SortBy("Name", new bool?(false));
              }
              //


             // public LevelList(
                Microsoft.Xna.Framework.Rectangle rectangle,
                ListElementActionType[] actions,
                string emptyListInfoText,
                Button.OnClick emptyListInfoClickAction = null,
                int rowAmount = -1,
                ColumnInfo[] columnInfosOverride = null)
                : base(rectangle, columnInfosOverride ?? LevelList.columnInfos, LevelList.GetActions(actions), emptyListInfoText, rowAmount, true, new Styling?(), emptyListInfoClickAction)
              {
                  this.SortBy("Name", new bool?(false));
              }//
            this.SortBy("Name", new bool?(false));
          //
        }*/


        private static ListElementAction[] GetActions(ListElementActionType[] actionTypes)
    {
      ListElementAction[] actions = new ListElementAction[actionTypes.Length];
      for (int index = 0; index < actions.Length; ++index)
      {
        ListElementActionType actionType = actionTypes[index];
        actions[index] = new ListElementAction(actionType, LevelList.GetClickHandler(actionType));
      }
      return actions;
    }

    private static ListElementAction.Action GetClickHandler(ListElementActionType action)
    {
      switch (action)
      {
        case ListElementActionType.Play:
          return (ListElementAction.Action) (element =>
          {
            LevelInfo levelInfo = (LevelInfo) element;
            if (levelInfo == null)
              return;
            StateManager.TransitionTo(GameState.InLevel_Default, Utility.TranslateLevelname(levelInfo.name));
          });
        case ListElementActionType.Delete:
          return (ListElementAction.Action) (element => PopupMenu.Instance.Confirm("Are you sure?*This will irreversibly delete*the level from your computer!", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), (PromptMenu.InteractionHandler) (() =>
          {
            LevelInfo levelInfo = (LevelInfo) element;
            if (levelInfo == null)
              return;
            File.Delete(levelInfo.path);
            LevelSelection.Instance.UpdateCustomLevels();
            PopupMenu.Instance.SetActive(false);
          })));
        case ListElementActionType.Edit:
          return (ListElementAction.Action) (element =>
          {
            LevelInfo levelInfo = (LevelInfo) element;
            if (levelInfo == null)
              return;
            StateManager.TransitionTo(GameState.LevelEditor, Utility.TranslateLevelname(levelInfo.name));
          });
        case ListElementActionType.Upload:
          return (ListElementAction.Action) (element =>
          {
            LevelInfo levelInfo = (LevelInfo) element;
            if (levelInfo == null)
              return;
            FileManager.UploadLevel(levelInfo);
            LevelSelection.Instance.UpdateCustomLevels();
          });
        case ListElementActionType.Download:
          return (ListElementAction.Action) (element =>
          {
            LevelInfo levelInfo = (LevelInfo) element;
            if (levelInfo == null)
              return;
            FileManager.DownloadLevel(levelInfo);
            LevelSelection.Instance.UpdateCustomLevels();
          });
        case ListElementActionType.Remove:
          return (ListElementAction.Action) (element => PopupMenu.Instance.Confirm("Are you sure?*This will remove the level from*the database!", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), (PromptMenu.InteractionHandler) (async () =>
          {
            LevelInfo levelInfo = (LevelInfo) element;
            if (levelInfo == null)
            {
              levelInfo = (LevelInfo) null;
            }
            else
            {
              string url = VarManager.GetString("baseurl") + "Levels/Remove.php";
              Dictionary<string, string> content = new Dictionary<string, string>();
              content.Add("levelname", levelInfo.name);
              content.Add("uuid", VarManager.GetString("uuid"));
              content.Add("username", VarManager.GetString("username"));
              content.Add("levelkey", VarManager.GetString("authkey"));
              HttpManager.Callback callback = new HttpManager.Callback(LevelSelection.Instance.UpdateCustomLevels);
              string response = await HttpManager.Post(url, content, callback);
              if (response.StartsWith("error:", true, (CultureInfo) null))
                PopupMenu.Instance.Inform(response, (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)));
              else
                PopupMenu.Instance.SetActive(false);
              response = (string) null;
              levelInfo = (LevelInfo) null;
            }
          })));
        case ListElementActionType.Scores:
          return (ListElementAction.Action) (async element =>
          {
            LevelInfo levelInfo = (LevelInfo) element;
            if (levelInfo == null)
            {
              levelInfo = (LevelInfo) null;
            }
            else
            {
              string levelName = "Custom/" + levelInfo.name;
              await ScoreManager.UpdateOverview(levelName);
              ScoreExplorer.Instance.ShowList(levelName, sortBy: "time", sortDescending: new bool?(false));
              levelName = (string) null;
              levelInfo = (LevelInfo) null;
            }
          });
        case ListElementActionType.Load:
          return (ListElementAction.Action) (element =>
          {
            LevelInfo levelInfo = (LevelInfo) element;
            if (levelInfo == null)
              return;
            if (LevelManager.CurrentLevelName != "Editor")
              LevelManager.SaveCurrent();
            string name = "Custom/" + levelInfo.name;
            LevelManager.Load(name);
            LevelEditor.Instance.SetLevelNameDisplay(name);
            LevelEditor.Instance.ClearHistory();
            PopupMenu.Instance.SetActive(false);
          });
        default:
          return (ListElementAction.Action) null;
      }
    }
  }
}

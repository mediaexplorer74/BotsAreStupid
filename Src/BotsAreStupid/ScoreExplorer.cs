// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScoreExplorer
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;

#nullable enable
namespace BotsAreStupid
{
  internal class ScoreExplorer : UIElement
  {
    public static 
    #nullable disable
    ScoreExplorer Instance;
    private static readonly ColumnInfo[] columnInfos = new ColumnInfo[6]
    {
      new ColumnInfo("No.", 0.1f, false, false),
      new ColumnInfo("Player", 0.3375f, true, false),
      new ColumnInfo("Time", 0.1375f, false, true),
      new ColumnInfo("Lines", 0.1f, false, true),
      new ColumnInfo("Upload", 0.15f, false, true),
      new ColumnInfo("Actions", 0.175f, false, false)
    };
    private const int padding = 0;
    private const int headerHeight = 100;
    private const int shiftAddInfoHeight = 40;
    private UIElement window;
    private UIElement header;
    private ListTable scoreList;
    private ListTable playerScoreList;
    private Button backButton;
    private Button randomButton;
    private UIElement shiftAddInfo;
    private string levelName;
    private bool minSpeed;
    private string playerName;
    private bool createGhostOnWatch;
    private ScoreData[] scores;

    public ScoreExplorer(Microsoft.Xna.Framework.Rectangle rectangle)
      : base(rectangle)
    {
      ScoreExplorer.Instance = this;
      this.AddChild(this.window = new UIElement(Microsoft.Xna.Framework.Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(parentRect.Width / 20, parentRect.Height / 20, parentRect.Width / 20 * 18, parentRect.Height / 20 * 18))));
      this.SetActive(false);
      Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() =>
      {
        if (!this.IsActive || this.window.MouseOnRectangle())
          return;
        List<UIElement> childrenOfType = UIElement.FindChildrenOfType<Scrollbar>((UIElement) this);
        if (childrenOfType.Count > 0)
        {
          Vector2 pos = this.LocalMousePos();
          foreach (UIElement uiElement in childrenOfType)
          {
            if (Utility.PointInside(pos, ((Scrollbar) uiElement).GlobalSliderRect))
              return;
          }
        }
        this.SetActive(false);
      }));
      SimulationManager.OnSimulationRemoved += (Action<Simulation>) (simulation => simulation.ScoreData?.SetCanBeWatched(true));
      this.CreateChildren();
    }

    private void CreateChildren()
    {
      Microsoft.Xna.Framework.Color color1 = ColorManager.GetColor("white");
      ColorManager.GetColor("darkslate");
      Microsoft.Xna.Framework.Color color2 = ColorManager.GetColor("lightslate");
      BitmapFont font = TextureManager.GetFont("megaMan2Big");
      Vector2 vector2 = (Vector2) font.MeasureStringHalf("a");
      Styling styling1 = new Styling()
      {
        defaultTextColor = color1,
        textOffset = 0,
        //font = font,
        text = "Highscores",
        centerText = true,
        borderColor = ColorManager.GetColor("white"),
        borderWidth = 4,
        defaultColor = color2,
        enabledBorders = new BorderInfo()
        {
          top = true,
          left = true,
          right = true
        }
      };
      Styling defaultButtonStyling = Styling.DefaultButtonStyling;
      Styling styling2 = new Styling()
      {
        defaultColor = color2
      };
      this.window.AddChild(this.header = new UIElement(Microsoft.Xna.Framework.Rectangle.Empty, new Styling?(styling1)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(100, 0, parentRect.Width - 200, 100))));
      UIElement window = this.window;
      Microsoft.Xna.Framework.Rectangle empty1 = Microsoft.Xna.Framework.Rectangle.Empty;
      Styling styling3 = new Styling();
      styling3.defaultTextColor = color1;
      //styling3.font = TextureManager.GetFont("courier");
      styling3.centerText = true;
      styling3.defaultColor = color2;
      styling3.text = "( Hold shift to add multiple ) ";
      Styling? style1 = new Styling?(styling3);
      UIElement child1 = this.shiftAddInfo = new UIElement(empty1, style1).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(100, 60, parentRect.Width - 200, 40)));
      window.AddChild(child1);
      this.shiftAddInfo.SetActive(false);
      UIElement child2 = new Button(Microsoft.Xna.Framework.Rectangle.Empty, new Button.OnClick(((UIElement) this).ToggleActive), defaultButtonStyling).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(parentRect.Width - 100, 0, 100, 100)));
      UIElement uiElement = child2;
      Microsoft.Xna.Framework.Rectangle empty2 = Microsoft.Xna.Framework.Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultColor = color1;
      styling3.spritePos = TextureManager.GetSpritePos("ui_cross");
      Styling? style2 = new Styling?(styling3);
      UIElement child3 = new UIElement(empty2, style2).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p, 0.4f)));
      uiElement.AddChild(child3);
      this.window.AddChild(child2);
      this.window.AddChild((UIElement) (this.backButton = new Button(new Microsoft.Xna.Framework.Rectangle(0, 0, 100, 100), (Button.OnClick) (() =>
      {
        string levelName = this.levelName;
        int num1 = this.minSpeed ? 1 : 0;
        bool createGhostOnWatch = this.createGhostOnWatch;
        bool? sortDescending = new bool?();
        int num2 = createGhostOnWatch ? 1 : 0;
        this.ShowList(levelName, num1 != 0, sortDescending: sortDescending, createGhostOnWatch: num2 != 0);
      }), defaultButtonStyling)));
      Button backButton = this.backButton;
      Microsoft.Xna.Framework.Rectangle empty3 = Microsoft.Xna.Framework.Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultColor = color1;
      styling3.spritePos = TextureManager.GetSpritePos("ui_left");
      Styling? style3 = new Styling?(styling3);
      UIElement child4 = new UIElement(empty3, style3).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p)));
      backButton.AddChild(child4);
      this.backButton.SetActive(false);
      this.window.AddChild((UIElement) (this.randomButton = new Button(new Microsoft.Xna.Framework.Rectangle(0, 0, 100, 100), (Button.OnClick) (() => this.PlayRandomScore()), defaultButtonStyling)));
      Button randomButton = this.randomButton;
      Microsoft.Xna.Framework.Rectangle empty4 = Microsoft.Xna.Framework.Rectangle.Empty;
      styling3 = new Styling();
      styling3.defaultColor = color1;
      styling3.spritePos = TextureManager.GetSpritePos("ui_shuffle");
      Styling? style4 = new Styling?(styling3);
      UIElement child5 = new UIElement(empty4, style4).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p)));
      randomButton.AddChild(child5);
      ListElementAction listElementAction = new ListElementAction(ListElementActionType.Watch, (ListElementAction.Action) (element =>
      {
        ScoreData data = (ScoreData) element;
        if (this.PlayHighscore(data, true))
          data.SetCanBeWatched(false);
        this.scoreList.UpdateView();
      }));
      ListElementAction[] actions1 = new ListElementAction[2]
      {
        listElementAction,
        new ListElementAction(ListElementActionType.More, (ListElementAction.Action) (element =>
        {
          ScoreData scoreData = (ScoreData) element;
          string levelName = this.levelName;
          int num3 = this.minSpeed ? 1 : 0;
          string playerName = scoreData.playerName;
          bool createGhostOnWatch = this.createGhostOnWatch;
          bool? sortDescending = new bool?();
          int num4 = createGhostOnWatch ? 1 : 0;
          this.ShowList(levelName, num3 != 0, playerName, sortDescending: sortDescending, createGhostOnWatch: num4 != 0);
        }))
      };
      this.scoreList = new ListTable(Microsoft.Xna.Framework.Rectangle.Empty, ScoreExplorer.columnInfos, actions1, secondSortParameter: 3, proportionalButtonSizes: true, style: new Styling?(styling2));
      this.scoreList.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(0, 100, parentRect.Width, parentRect.Height - 100)));
      this.scoreList.SetElements((IListElement[]) this.scores);
      this.scoreList.OnSortChange += new ListTable.SortChangeHandler(this.HandleSortChange);
      this.window.AddChild((UIElement) this.scoreList);
      ListElementAction[] actions2 = new ListElementAction[2]
      {
        listElementAction,
        new ListElementAction(ListElementActionType.Remove, (ListElementAction.Action) (element => PopupMenu.Instance.Confirm("Are you sure?*This will remove your script*from the database!", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), (PromptMenu.InteractionHandler) (() =>
        {
          ScoreData scoreData = (ScoreData) element;
          Dictionary<string, string> content = new Dictionary<string, string>()
          {
            {
              "playerid",
              VarManager.GetString("uuid")
            },
            {
              "scriptid",
              scoreData.databaseId.ToString()
            },
            {
              "authkey",
              VarManager.GetString("authkey")
            }
          };
          HttpManager.Post(VarManager.GetString("baseurl") + "Scores/Remove.php", content);
          this.playerScoreList.RemoveElement(element);
          ScoreManager.ClearLevelCaches(this.levelName);
          PopupMenu.Instance.ToggleActive();
        }))))
      };
      this.playerScoreList = new ListTable(Microsoft.Xna.Framework.Rectangle.Empty, ScoreExplorer.columnInfos, actions2, secondSortParameter: 3, proportionalButtonSizes: true, style: new Styling?(styling2));
      this.playerScoreList.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(0, 100, parentRect.Width, parentRect.Height - 100)));
      this.playerScoreList.SetElements((IListElement[]) this.scores);
      this.playerScoreList.OnSortChange += new ListTable.SortChangeHandler(this.HandleSortChange);
      this.window.AddChild((UIElement) this.playerScoreList);
    }

    private bool PlayHighscore(ScoreData data, bool minSpeed)
    {
      if (LevelManager.Exists(this.levelName))
      {
        Utility.PlayScore(data, this.levelName, this.createGhostOnWatch);
        if (!this.createGhostOnWatch || !Input.IsDown(KeyBind.Shift))
          ScoreExplorer.Instance.SetActive(false);
        return true;
      }
      PopupMenu.Instance.Inform("The level needs to be*downloaded first!", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), false);
      return false;
    }

    private void HandleSortChange(string column, bool reversed)
    {
      column = column.ToLower();
      switch (column)
      {
        case "time":
          this.ShowList(this.levelName, playerName: this.playerName, sortDescending: new bool?(!reversed), createGhostOnWatch: this.createGhostOnWatch);
          break;
        case "lines":
          this.ShowList(this.levelName, false, this.playerName, sortDescending: new bool?(!reversed), createGhostOnWatch: this.createGhostOnWatch);
          break;
      }
    }

    private void PlayRandomScore(bool allowPrompt = true)
    {
      this.SetActive(false);
      if (!this.createGhostOnWatch)
        VarManager.SetBool("randomscores", true);
      else if (allowPrompt && Input.IsDown(KeyBind.Shift))
      {
        PopupMenu.Instance.Prompt("Amount:", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), (PromptMenu.InteractionHandlerString) (s =>
        {
          int result;
          if (int.TryParse(s, out result))
          {
            for (int index = 0; index < result; ++index)
              this.PlayRandomScore(false);
          }
          PopupMenu.Instance.SetActive(false);
        }), (PromptMenu.Validator) ((string input, out string msg) => Utility.NumberValidator(input, out msg, 50, 1)), "1");
        return;
      }
      Utility.PlayRandomScore(this.levelName, this.createGhostOnWatch);
    }

    public async void ShowList(
      string levelName,
      bool minSpeed = true,
      string playerName = "",
      string sortBy = "",
      bool? sortDescending = null,
      bool createGhostOnWatch = false)
    {
      this.playerName = playerName;
      ScoreData[] scoreDataArray = await ScoreManager.GetList(levelName, minSpeed, playerName, sortDescending.GetValueOrDefault());
      this.scores = scoreDataArray;
      scoreDataArray = (ScoreData[]) null;
      this.levelName = levelName;
      this.minSpeed = minSpeed;
      this.createGhostOnWatch = createGhostOnWatch;
      bool isPlayerList = !string.IsNullOrEmpty(playerName);
      this.playerScoreList.SetActive(isPlayerList);
      this.scoreList.SetActive(!isPlayerList);
      ListTable currentList = isPlayerList ? this.playerScoreList : this.scoreList;
      currentList.SetButtonLabels(0, createGhostOnWatch ? "Add" : "Watch");
      this.randomButton.SetTooltip(createGhostOnWatch ? "Add random script" : "Watch random scripts for this level");
      currentList.SetElements((IListElement[]) this.scores);
      if (!string.IsNullOrEmpty(sortBy))
        currentList.SortBy(sortBy, sortDescending);
      currentList.ResetView();
      this.backButton.SetActive(isPlayerList);
      this.randomButton.SetActive(!isPlayerList && !currentList.IsEmpty);
      this.header.SetText(isPlayerList ? "Scores:  " + playerName : "Highscores");
      this.shiftAddInfo.SetActive(createGhostOnWatch);
      this.header.SetSize(this.header.Width, createGhostOnWatch ? 80 : 100);
      this.SetActive(true);
      currentList = (ListTable) null;
    }
  }
}

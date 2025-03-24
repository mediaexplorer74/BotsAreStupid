// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.PopupMenu
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System.IO;

#nullable disable
namespace BotsAreStupid
{
  internal class PopupMenu : UIElement
  {
    private const int width = 580;
    private const int height = 440;
    private const int buttonHeight = 50;
    private const int paddingLeft = 50;
    private UIElement levelMenu;
    private UIElement editorMenu;
    private PromptMenu promptMenu;
    private SuccessMenu successMenu;
    private ScriptMenu scriptMenu;
    private UIElement quickOptionsMenu;
    private LevelMenu loadLevelMenu;
    private Button levelExitButton;
    private Button editorExitButton;
    private ToggleButton manualControlsButton;

    public static PopupMenu Instance { get; private set; }

    public PopupMenu(Rectangle rectangle)
      : base(new Rectangle(rectangle.X + 340 + (rectangle.Width - 340) / 2 - 290, rectangle.Y + rectangle.Height / 2 - 220, 580, 440))
    {
      PopupMenu.Instance = this;
      this.CreateChildren(rectangle);
      this.SetActive(false);
    }

    private void CreateChildren(Rectangle fullRect)
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = ColorManager.GetColor("white");
      Color color3 = ColorManager.GetColor("lightslate");
      Color color4 = ColorManager.GetColor("darkslate");
      Styling styling = new Styling()
      {
        defaultColor = color4,
        hoverColor = color3,
        clickColor = color1,
        centerText = true,
        defaultTextColor = color2,
        //font = TextureManager.GetFont("megaman2"),
        borderColor = color2,
        borderWidth = 4
      };
      Rectangle rectangle1 = new Rectangle(0, 0, this.rectangle.Width, this.rectangle.Height);
      this.levelMenu = new UIElement(rectangle1);
      this.editorMenu = new UIElement(rectangle1);
      this.promptMenu = new PromptMenu(rectangle1);
      this.successMenu = new SuccessMenu(rectangle1);
      this.scriptMenu = new ScriptMenu(rectangle1);
      this.quickOptionsMenu = (UIElement) new QuickOptionsMenu(rectangle1, styling, (Button.OnClick) (() => this.SetMenu(this.levelMenu)));
      this.loadLevelMenu = new LevelMenu(rectangle1);
      Rectangle rectangle2 = new Rectangle(0, 0, this.rectangle.Width / 3, 50);
      Rectangle rectangle3 = new Rectangle(0, this.rectangle.Height - 50, this.rectangle.Width, 50);
      int num1 = (this.rectangle.Height - 100) / 3;
      styling.text = "Resume";
      this.levelMenu.AddChild((UIElement) new Button(rectangle2, (Button.OnClick) (() => this.SetActive(false)), styling));
      styling.text = "Exit to Levels";
      styling.tooltip = "Your current script will be saved.";
      this.levelMenu.AddChild((UIElement) (this.levelExitButton = new Button(rectangle3, new Button.OnClick(this.OnLevelExitButtonPress), styling)));
      styling.tooltip = (string) null;
      styling.text = "New Script";
      this.levelMenu.AddChild((UIElement) new Button(new Rectangle(50, 50 + num1 - 25, this.rectangle.Width / 2 - 50 - 25, 50), (Button.OnClick) (() => this.Prompt("Enter a name for the new script:", (PromptMenu.InteractionHandler) (() => this.ShowDefault()), (PromptMenu.InteractionHandlerString) (name =>
      {
        name = name.ToLower();
        TextEditor.Instance.SaveInstructions();
        ScriptManager.SaveScript(LevelManager.CurrentLevelName, name, (string[]) null);
        TextEditor.Instance.LoadInstructions((string[]) null, name);
        this.SetActive(false);
      }), new PromptMenu.Validator(ScriptManager.ScriptNameValidator))), styling));
      styling.tooltip = (string) null;
      styling.text = "Load Script";
      this.levelMenu.AddChild((UIElement) new Button(new Rectangle(this.rectangle.Width / 2 + 25, 50 + num1 - 25, this.rectangle.Width / 2 - 50 - 25, 50), (Button.OnClick) (() => this.ShowLoadInstructionsMenu()), styling));
      styling.tooltip = (string) null;
      styling.text = "Options";
      this.levelMenu.AddChild((UIElement) new Button(new Rectangle(50, 50 + num1 * 2 - 25, this.rectangle.Width - 100, 50), (Button.OnClick) (() => this.SetMenu(this.quickOptionsMenu)), styling));
      styling.tooltip = (string) null;
      styling.text = "";
      Button child1 = new Button(new Rectangle(0, 0, 58, 58), (Button.OnClick) (() =>
      {
        if (this.scriptMenu.IsGhostMenu)
          this.ToggleActive();
        else
          this.SetMenu(this.levelMenu);
      }), styling);
      child1.AddChild(new UIElement(new Rectangle(child1.Width / 2 - 17, child1.Height / 2 - 17, 35, 35)));
      this.scriptMenu.AddChild((UIElement) child1);
      Button child2 = new Button(new Rectangle(0, 0, 58, 58), new Button.OnClick(((UIElement) this).ToggleActive), styling);
      child2.AddChild(new UIElement(new Rectangle(child1.Width / 2 - 17, child1.Height / 2 - 17, 35, 35)));
      this.loadLevelMenu.AddChild((UIElement) child2);
      styling.tooltip = "Search for legacy scripts of older versions";
      Button child3 = new Button(new Rectangle(522, 0, 58, 58), (Button.OnClick) (() => this.Confirm("Search your computer for legacy*scripts saved in the level files*of older versions of the game?", (PromptMenu.InteractionHandler) (() => this.ShowLoadInstructionsMenu()), (PromptMenu.InteractionHandler) (() => ScriptManager.FindLegacyScripts(LevelManager.CurrentLevelName)))), styling);
      child3.AddChild(new UIElement(new Rectangle(child3.Width / 2 - 12, child3.Height / 2 - 12, 25, 25)));
      this.scriptMenu.AddChild((UIElement) child3);
      int num2 = (this.rectangle.Height - 100) / 3;
      styling.tooltip = (string) null;
      styling.text = "Resume";
      this.editorMenu.AddChild((UIElement) new Button(rectangle2, (Button.OnClick) (() => this.SetActive(false)), styling));
      styling.text = "Back to Menu";
      this.editorMenu.AddChild((UIElement) (this.editorExitButton = new Button(rectangle3, (Button.OnClick) (() =>
      {
        if (LevelEditor.Instance.HasChanges)
          this.Confirm("You have unsaved changes!*Are you sure?", (PromptMenu.InteractionHandler) (() => this.SetMenu(this.editorMenu)), (PromptMenu.InteractionHandler) (() => StateManager.TransitionBack()));
        else
          StateManager.TransitionBack();
      }), styling)));
      styling.text = "Rename Level";
      this.editorMenu.AddChild((UIElement) new Button(new Rectangle(50, 50 + num2 - 25, this.rectangle.Width - 100, 50), new Button.OnClick(this.OnRenameLevelButtonPress), styling));
      styling.text = "Clear level";
      this.editorMenu.AddChild((UIElement) new Button(new Rectangle(50, 50 + num2 * 2 - 25, this.rectangle.Width - 100, 50), (Button.OnClick) (() => this.Confirm("Confirm clearing the level:", (PromptMenu.InteractionHandler) (() => this.SetMenu(this.editorMenu)), (PromptMenu.InteractionHandler) (() => this.OnClearLevelButtonPress()))), styling));
      this.AddChild(this.levelMenu);
      this.AddChild(this.editorMenu);
      this.AddChild((UIElement) this.promptMenu);
      this.AddChild((UIElement) this.successMenu);
      this.AddChild((UIElement) this.scriptMenu);
      this.AddChild(this.quickOptionsMenu);
      this.AddChild((UIElement) this.loadLevelMenu);
    }

    public override void SetActive(bool active)
    {
      base.SetActive(active);
      bool inLevel = StateManager.IsState(GameState.InLevel_AnyWithText);
      if (active)
        this.ShowDefault(inLevel);
      if (!inLevel)
        return;
      TextEditor.Instance.UpdateView();
    }

    public void SetExitButtonText(string text)
    {
      this.levelExitButton.SetText(text);
      this.editorExitButton.SetText(text);
    }

    private void ShowDefault(bool inLevel = true)
    {
      if (!inLevel && !StateManager.IsState(GameState.LevelEditor))
        return;
      this.SetMenu(inLevel ? this.levelMenu : this.editorMenu);
    }

    private void SetMenu(UIElement menu)
    {
      if (menu.Parent != this)
        return;
      foreach (UIElement child in this.Children)
        child.SetActive(false);
      menu.SetActive(true);
    }

    private void OnClearLevelButtonPress()
    {
      LevelManager.Load("Editor");
      LevelEditor.Instance.ClearHistory();
      this.SetActive(false);
    }

    private void OnRenameLevelButtonPress()
    {
      this.Prompt("Rename Level*New name:", (PromptMenu.InteractionHandler) (() => this.SetMenu(this.editorMenu)), (PromptMenu.InteractionHandlerString) (name =>
      {
        string str = Utility.TranslateLevelname(name);
        ScriptManager.RenameSaveFile(LevelManager.CurrentLevelName, str);
        if (!LevelManager.CurrentLevelNameSimple.Contains("AutoSave") && !LevelManager.CurrentLevelNameSimple.Contains("Editor"))
          File.Delete(LevelManager.CurrentLevelPath);
        LevelManager.SaveAs(str);
        this.SetActive(false);
      }), new PromptMenu.Validator(Utility.LevelNameValidator), LevelManager.CurrentLevelNameSimple);
    }

    private void EnableManualControls()
    {
      VarManager.SetBool("manualControls", true);
      Utility.MainPlayer?.Kill();
      this.Inform("Move:   WASD / Arrow Keys*Hook:   F*Reset:   R", (PromptMenu.InteractionHandler) (() =>
      {
        this.SetActive(false);
        Utility.MainPlayer?.EnableManualControls();
      }), false);
    }

    public void DisableManualControls()
    {
      VarManager.SetBool("manualControls", false);
      Utility.MainPlayer?.Kill();
      this.SetActive(false);
    }

    private void OnLevelExitButtonPress()
    {
      if (StateManager.IsState(GameState.InLevel_Default) || StateManager.IsState(GameState.InLevel_FromEditor))
        TextEditor.Instance.SaveInstructions();
      StateManager.TransitionBack();
    }

    public void ShowLoadInstructionsMenu(bool isGhostMenu = false)
    {
      this.SetActive(true);
      this.scriptMenu.ShowList(isGhostMenu);
      this.SetMenu((UIElement) this.scriptMenu);
    }

    public void ShowLoadLevelMenu()
    {
      this.SetActive(true);
      this.loadLevelMenu.ShowList();
      this.SetMenu((UIElement) this.loadLevelMenu);
    }

    public void Prompt(
      string question,
      PromptMenu.InteractionHandler backButtonHandler,
      PromptMenu.InteractionHandlerString okButtonHandler,
      PromptMenu.Validator validator = null,
      string fieldValue = null,
      string errorMsg = null)
    {
      this.SetActive(true);
      this.promptMenu.Reset();
      this.promptMenu.SetInformation(question, true);
      this.promptMenu.SetBackButtonHandler(backButtonHandler);
      this.promptMenu.SetOkButtonHandlerString(okButtonHandler);
      this.promptMenu.SetValidator(validator);
      this.SetMenu((UIElement) this.promptMenu);
      this.promptMenu.ResetView();
      if (fieldValue != null)
        this.promptMenu.SetFieldValue(fieldValue);
      if (errorMsg == null)
        return;
      this.promptMenu.SetError(errorMsg);
    }

    public void Inform(string info, PromptMenu.InteractionHandler backButtonHandler, bool useBack = true)
    {
      this.SetActive(true);
      this.promptMenu.Reset();
      this.promptMenu.SetInformation(info);
      if (useBack)
        this.promptMenu.SetBackButtonHandler(backButtonHandler);
      else
        this.promptMenu.SetOkButtonHandler(backButtonHandler);
      this.SetMenu((UIElement) this.promptMenu);
      this.promptMenu.ResetView();
    }

    public void Confirm(
      string info,
      PromptMenu.InteractionHandler backButtonHandler,
      PromptMenu.InteractionHandler okButtonHandler)
    {
      this.SetActive(true);
      this.promptMenu.Reset();
      this.promptMenu.SetInformation(info);
      this.promptMenu.SetBackButtonHandler(backButtonHandler);
      this.promptMenu.SetOkButtonHandler(okButtonHandler);
      this.SetMenu((UIElement) this.promptMenu);
      this.promptMenu.ResetView();
    }

    public void ShowSuccess(
      float time,
      int lineCount,
      ScoreData overview,
      ScoreData personalOverview,
      bool manualControls,
      bool nextAvailable,
      bool nextLocked)
    {
      this.SetActive(true);
      this.successMenu.SetScore(time, lineCount, overview, personalOverview);
      this.successMenu.SetManualControlsInfoActive(manualControls);
      string currentLevelName = LevelManager.CurrentLevelName;
      this.successMenu.UpdateNextButton(nextAvailable, nextLocked);
      this.SetMenu((UIElement) this.successMenu);
    }

    public void UpdateSuccess(
      int timeRank = -1,
      int lineCountRank = -1,
      int totalScores = -1,
      int personalTimeRank = -1,
      int personalLineCountRank = -1,
      int totalPersonalScores = -1,
      bool isDuplicate = false,
      bool isTooLong = false)
    {
      this.successMenu.SetInfo(timeRank, lineCountRank, totalScores, personalTimeRank, personalLineCountRank, totalPersonalScores, isDuplicate, isTooLong);
    }

    public void SetSuccessInfoLoading() => this.successMenu.SetInfoLoading();
  }
}

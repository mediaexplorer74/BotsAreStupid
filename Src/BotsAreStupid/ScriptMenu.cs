// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScriptMenu
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal class ScriptMenu : UIElement
  {
    private ListTable scriptList;
    private UIElement header;
    private UIElement shiftAddInfo;
    private const int padding = 30;
    private const int headerHeight = 58;
    private const int shiftAddInfoHeight = 50;
    private static readonly ColumnInfo[] columnInfos = new ColumnInfo[4]
    {
      new ColumnInfo("No.", 0.1f, false, false),
      new ColumnInfo("Name", 0.3f, true, false),
      new ColumnInfo("Time", 0.2f, false, false),
      new ColumnInfo("Actions", 0.4f, false, false)
    };

    public bool IsGhostMenu { get; private set; }

    public ScriptMenu(Microsoft.Xna.Framework.Rectangle rectangle)
      : base(rectangle)
    {
      Microsoft.Xna.Framework.Color color = ColorManager.GetColor("white");
      this.AddChild(this.header = new UIElement(new Microsoft.Xna.Framework.Rectangle(58, 0, rectangle.Width - 116, 58)));
      this.AddChild(this.shiftAddInfo = new UIElement(new Microsoft.Xna.Framework.Rectangle(0, rectangle.Height, rectangle.Width, 50)));
      this.shiftAddInfo.SetActive(false);
    }

    public void ShowList(bool isGhostMenu = false)
    {
      this.IsGhostMenu = isGhostMenu;
      this.scriptList?.Destroy();
      ListElementAction[] actions;
      if (isGhostMenu)
        actions = new ListElementAction[1]
        {
          new ListElementAction(ListElementActionType.Add, (ListElementAction.Action) (element =>
          {
            Script script = (Script) element;
            SimulationManager.CreateSimulation(out bool _, instructions: script.instructions, playerName: script.name).CanSaveScriptTime = true;
            script.SetCanBeLoaded(false);
            this.scriptList.UpdateView();
            if (Input.IsDown(KeyBind.Shift))
              return;
            PopupMenu.Instance.SetActive(false);
          }))
        };
      else
        actions = new ListElementAction[3]
        {
          new ListElementAction(ListElementActionType.LoadScript, (ListElementAction.Action) (element =>
          {
            Script script = (Script) element;
            StateManager.TransitionTo(GameState.InLevel_Default);
            TextEditor.Instance.SaveInstructions();
            TextEditor.Instance.LoadInstructions(script.instructions, script.name);
            PopupMenu.Instance.SetActive(false);
            Utility.MainPlayer?.Kill();
          })),
          new ListElementAction(ListElementActionType.Rename, (ListElementAction.Action) (element =>
          {
            Script script = (Script) element;
            PopupMenu.Instance.Prompt("Enter a new name for:*" + script.name, (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.ShowLoadInstructionsMenu()), (PromptMenu.InteractionHandlerString) (name =>
            {
              name = name.ToLower();
              ScriptManager.RenameScript(LevelManager.CurrentLevelName, script.name, name);
              PopupMenu.Instance.ShowLoadInstructionsMenu();
              TextEditor.Instance.SetCurrentScriptName(name, script.name);
            }), new PromptMenu.Validator(ScriptManager.ScriptNameValidator));
          })),
          new ListElementAction(ListElementActionType.Delete, (ListElementAction.Action) (element =>
          {
            Script script = (Script) element;
            PopupMenu.Instance.Confirm("Confirm  deletion  of:*" + script.name + "*This is irreversible!", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.ShowLoadInstructionsMenu()), (PromptMenu.InteractionHandler) (() =>
            {
              ScriptManager.DeleteScript(LevelManager.CurrentLevelName, script.name);
              PopupMenu.Instance.ShowLoadInstructionsMenu();
            }));
          }))
        };
      this.AddChild((UIElement) (this.scriptList = new ListTable(new Microsoft.Xna.Framework.Rectangle(0, 58, this.rectangle.Width, this.rectangle.Height - 58 - 4), ScriptMenu.columnInfos, actions, rowAmount: 7, proportionalButtonSizes: true)));
      this.scriptList.SetElements((IListElement[]) ScriptManager.GetScripts(LevelManager.CurrentLevelName, out Script _));
      this.scriptList.Initialize();
      this.shiftAddInfo.SetActive(isGhostMenu);
    }
  }
}

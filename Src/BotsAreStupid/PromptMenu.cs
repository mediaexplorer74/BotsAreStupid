// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.PromptMenu
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;

#nullable disable
namespace BotsAreStupid
{
  internal class PromptMenu : UIElement
  {
    private PromptMenu.InteractionHandler backButtonHandler;
    private PromptMenu.InteractionHandler okButtonHandler;
    private PromptMenu.InteractionHandlerString okButtonHandlerString;
    private PromptMenu.Validator validator;
    private const int padding = 30;
    private const int elementHeight = 30;
    private const int buttonWidth = 120;
    private UIElement text1;
    private UIElement text2;
    private UIElement text3;
    private UIElement errorDisplay;
    private Textfield textfield;
    private Button backButton;
    private Button okButton;
    private GameState gameState;
    private bool hasQuestion = false;
    private bool backButtonEnabled = false;
    private bool okButtonEnabled = false;

    public PromptMenu(Rectangle rectangle, GameState gameState = GameState.Any)
      : base(rectangle)
    {
      this.gameState = gameState;
      this.CreateChildren();
      Input.RegisterOnDown(KeyBind.Enter, new InputAction.InputHandler(this.OnEnterPress), longPressDelay: 1f, ignoreTextLock: true);
    }

    public override void SetActive(bool active)
    {
      base.SetActive(active);
      if (!active)
        return;
      this.ResetView();
    }

    public void ResetView()
    {
      this.textfield.SetActive(this.hasQuestion);
      if (this.hasQuestion)
        this.textfield.Reset();
      this.backButton.SetActive(this.backButtonEnabled);
      this.okButton.SetActive(this.okButtonEnabled);
      this.errorDisplay.SetActive(false);
    }

    public void Reset()
    {
      this.backButtonHandler = (PromptMenu.InteractionHandler) null;
      this.okButtonHandler = (PromptMenu.InteractionHandler) null;
      this.okButtonHandlerString = (PromptMenu.InteractionHandlerString) null;
      this.validator = (PromptMenu.Validator) null;
      this.hasQuestion = false;
      this.backButtonEnabled = false;
      this.okButtonEnabled = false;
    }

    public void SetInformation(string information, bool hasQuestion = false)
    {
      this.hasQuestion = hasQuestion;
      string text1 = " ";
      string text2 = " ";
      string text3 = " ";
      if (information.Contains("*"))
      {
        int length1 = information.IndexOf('*');
        if (length1 > 0)
          text1 = information.Substring(0, length1);
        text2 = information.Substring(length1 + 1);
        if (text2.Contains("*"))
        {
          int length2 = text2.IndexOf('*');
          text3 = text2.Substring(length2 + 1);
          if (length2 > 0)
            text2 = text2.Substring(0, length2);
        }
      }
      else
        text1 = information;
      this.text1.SetText(text1);
      this.text2.SetText(text2);
      this.text3.SetText(text3);
    }

    public void SetBackButtonHandler(PromptMenu.InteractionHandler handler)
    {
      if (handler != null)
        this.backButtonEnabled = true;
      this.backButtonHandler = handler;
    }

    public void SetOkButtonHandlerString(PromptMenu.InteractionHandlerString handler)
    {
      this.okButtonEnabled = true;
      this.okButtonHandlerString = handler;
    }

    public void SetOkButtonHandler(PromptMenu.InteractionHandler handler)
    {
      this.okButtonEnabled = true;
      this.okButtonHandler = handler;
    }

    public void SetValidator(PromptMenu.Validator validator) => this.validator = validator;

    private void CreateChildren()
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = ColorManager.GetColor("white");
      Color color3 = ColorManager.GetColor("lightslate");
      Color color4 = ColorManager.GetColor("darkslate");
      BitmapFont font1 = TextureManager.GetFont("megaman2");
      BitmapFont font2 = TextureManager.GetFont("courier");
      int num = this.rectangle.Height / 4;
      Styling styling = new Styling()
      {
        defaultTextColor = color2,
        //font = font1
      };
      this.text1 = new UIElement(new Rectangle(30, num - 15, this.rectangle.Width - 60, 30), new Styling?(styling));
      this.AddChild(this.text1);
      this.text2 = new UIElement(new Rectangle(30, (int) ((double) num * 1.5) - 15, this.rectangle.Width - 60, 30), new Styling?(styling));
      this.AddChild(this.text2);
      this.text3 = new UIElement(new Rectangle(30, num * 2 - 15, this.rectangle.Width - 60, 30), new Styling?(styling));
      this.AddChild(this.text3);
      this.textfield = new Textfield(new Rectangle(30, num * 2 - 15, this.rectangle.Width - 60, 30), this.gameState);
      this.AddChild((UIElement) this.textfield);
      this.errorDisplay = new UIElement(new Rectangle(30, num + num / 2 - 7, this.rectangle.Width - 60, 15));
      this.AddChild(this.errorDisplay);
      int x = (this.rectangle.Width - 240) / 3;
      Styling style = new Styling()
      {
        defaultColor = color4,
        hoverColor = color3,
        clickColor = color1,
        //font = font1,
        defaultTextColor = color2,
        centerText = true,
        borderColor = color2,
        borderWidth = 4
      } with
      {
        text = "Back"
      };
      this.backButton = new Button(new Rectangle(x, num * 3 - 15, 120, 30), (Button.OnClick) (() =>
      {
        PromptMenu.InteractionHandler backButtonHandler = this.backButtonHandler;
        if (backButtonHandler == null)
          return;
        backButtonHandler();
      }), style);
      this.AddChild((UIElement) this.backButton);
      style.text = "OK";
      this.okButton = new Button(new Rectangle(x * 2 + 120, num * 3 - 15, 120, 30), new Button.OnClick(this.OnOkButtonClick), style);
      this.AddChild((UIElement) this.okButton);
    }

    private void OnOkButtonClick()
    {
      string str = this.textfield.GetValue();
      string errorMessage = "Error: null";
      if (this.validator == null || this.validator(str, out errorMessage))
      {
        PromptMenu.InteractionHandlerString buttonHandlerString = this.okButtonHandlerString;
        if (buttonHandlerString != null)
          buttonHandlerString(str.Replace(VarManager.GetString("forceflag"), ""));
        PromptMenu.InteractionHandler okButtonHandler = this.okButtonHandler;
        if (okButtonHandler == null)
          return;
        okButtonHandler();
      }
      else
        this.SetError(errorMessage);
    }

    private void OnEnterPress()
    {
      if (!this.IsActive)
        return;
      if (this.okButtonEnabled)
      {
        this.OnOkButtonClick();
      }
      else
      {
        PromptMenu.InteractionHandler backButtonHandler = this.backButtonHandler;
        if (backButtonHandler != null)
          backButtonHandler();
      }
      this.PlayInteractionSound();
    }

    public void SetFieldValue(string value) => this.textfield.SetValue(value);

    public void SetError(string errorMessage)
    {
      this.errorDisplay.SetActive(true);
      this.errorDisplay.ChangeStyle(new Styling()
      {
        text = "Error: " + errorMessage,
        defaultColor = ColorManager.GetColor("red")
      });
    }

    public delegate void InteractionHandler();

    public delegate void InteractionHandlerString(string value);

    public delegate bool Validator(string value, out string errorMessage);
  }
}

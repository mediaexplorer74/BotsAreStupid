// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Credits
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;

#nullable enable
namespace BotsAreStupid
{
  internal class Credits : BasicMenu
  {
    private const int spacing = 50;
    private const int buttonHeight = 50;

    public Credits(Rectangle rectangle)
      : base(rectangle, GameState.Credits)
    {
      this.CreateChildren();
    }

    private void CreateChildren()
    {
      BitmapFont font = TextureManager.GetFont("megaMan2Big");
      Color color = ColorManager.GetColor("white");
      Styling styling = new Styling()
      {
        defaultTextColor = color,
        centerText = true,
        //font = TextureManager.GetFont("megaMan2")
      };
            int height = 24;//(int) font.MeasureStringHalf("a").Height;
      int heightTotal = 6 * (height + 50) - 50;
      styling.text = "developed by";
      this.main.AddChild(new UIElement(Rectangle.Empty, new Styling?(styling)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 3 - heightTotal / 2 + height / 2, this.main.Width, height / 2))));
      styling.text = "tileset by";
      this.main.AddChild(new UIElement(Rectangle.Empty, new Styling?(styling)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 3 - heightTotal / 2 + 4 * (height + 50) + height / 2, parentRect.Width, height / 2))));
      styling.text = "V " + VarManager.GetString("version");
      this.main.AddChild(new UIElement(Rectangle.Empty, new Styling?(styling)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height - 50 - 50, parentRect.Width, 50))));
      //styling.font = font;
      styling.text = "Leander Edler-Golla";
      this.main.AddChild(new UIElement(Rectangle.Empty, new Styling?(styling)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 3 - heightTotal / 2 + height + 50, parentRect.Width, height))));
      Styling style = new Styling()
      {
        defaultColor = color,
        hoverColor = Color.Gray,
        tooltip = "Follow me on Twitter :)\n(-> https://twitter.com/lelegolla)",
        clickColor = ColorManager.GetColor("red"),
        round = true,
        spritePos = TextureManager.GetSpritePos("ui_twitter")
      };
            int nameWidth = 24;//(int) font.MeasureStringHalf(styling.text).Width;
      int twitterButtonWidth = (int) ((double) height * ((double) style.spritePos.Width / (double) style.spritePos.Height));
      this.main.AddChild(new Button(Rectangle.Empty, (Button.OnClick) (() => Utility.OpenURL("https://twitter.com/lelegolla")), style).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 + nameWidth / 2 + 20, parentRect.Height / 3 - heightTotal / 2 + height + 50 + 2, twitterButtonWidth, height))));
      styling.text = "at FH Salzburg - MultiMediaTechnology";
      this.main.AddChild(new UIElement(Rectangle.Empty, new Styling?(styling)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 3 - heightTotal / 2 + 2 * (height + 50), parentRect.Width, height))));
      styling.text = "as MMP1";
      this.main.AddChild(new UIElement(Rectangle.Empty, new Styling?(styling)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 3 - heightTotal / 2 + 3 * (height + 50), parentRect.Width, height))));
      styling.text = "0x72 - Robert Norenberg";
      this.main.AddChild(new UIElement(Rectangle.Empty, new Styling?(styling)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 3 - heightTotal / 2 + 5 * (height + 50), parentRect.Width, height))));
      Styling defaultButtonStyling = Styling.DefaultButtonStyling with
      {
        text = "Send Feedback"
      };
      this.main.AddChild(new Button(Rectangle.Empty, (Button.OnClick) (() => PopupMenu.Instance.Prompt("Message:", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)), (PromptMenu.InteractionHandlerString) (async s =>
      {
        string str = await HttpManager.Post(VarManager.GetString("baseurl") + "Messages/Feedback.php", new Dictionary<string, string>()
        {
          {
            "message",
            s
          },
          {
            "uuid",
            VarManager.GetString("uuid")
          },
          {
            "name",
            VarManager.GetString("username")
          }
        });
        PopupMenu.Instance.Inform("Feedback sent!", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)));
      }))), defaultButtonStyling).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 - parentRect.Width / 3 - parentRect.Width / 3 / 4, parentRect.Height - 50 - 50, parentRect.Width / 3, 50))));
      defaultButtonStyling.text = "Report Bug";
      this.main.AddChild(new Button(Rectangle.Empty, (Button.OnClick) (() => PopupMenu.Instance.Prompt("Please describe the bug*as accurately as possible:", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)), (PromptMenu.InteractionHandlerString) (async s =>
      {
        string str = await HttpManager.Post(VarManager.GetString("baseurl") + "Messages/Bugreport.php", new Dictionary<string, string>()
        {
          {
            "message",
            s
          },
          {
            "uuid",
            VarManager.GetString("uuid")
          },
          {
            "name",
            VarManager.GetString("username")
          }
        });
        PopupMenu.Instance.Inform("Bugreport sent!", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.SetActive(false)));
      }))), defaultButtonStyling).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 + parentRect.Width / 3 / 4, parentRect.Height - 50 - 50, parentRect.Width / 3, 50))));
    }
  }
}

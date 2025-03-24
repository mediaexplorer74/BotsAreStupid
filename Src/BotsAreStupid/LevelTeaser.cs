// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.LevelTeaser
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;
using System;

#nullable enable
namespace BotsAreStupid
{
  internal class LevelTeaser : UIElement
  {
    private const int sidebarPadding = 10;
    private const int headerHeight = 50;
    private const int buttonHeight = 50;
    private const int buttonIconWidth = 20;
    private const int buttonPadding = 30;
    private 
    #nullable disable
    string levelName;
    private UIElement lockedContainer;
    private UIElement unlockedContainer;
    private Button bestTime;
    private Button bestScript;
    private Button playButton;
    private ScoreData scoreOverview;
    private bool isUnlocked;
    private bool isDemoLocked;

    public LevelTeaser(string levelName)
      : base(new Rectangle(0, 0, 1, 1))
    {
      this.levelName = levelName;
      this.CreateChildren();
    }

    private void CreateChildren()
    {
      string str1 = this.levelName;
      char ch = this.levelName[this.levelName.Length - 1];
      if (char.IsNumber(ch))
        str1 = str1.TrimEnd(ch) + " " + Utility.IntToRoman(int.Parse(ch.ToString()));
      Color color = ColorManager.GetColor("white");
      BitmapFont font1 = TextureManager.GetFont("megaMan2");
      BitmapFont font2 = TextureManager.GetFont("megaMan2Small");
      BitmapFont font3 = TextureManager.GetFont("megaMan2Big");
      Styling imageStyling = new Styling()
      {
        defaultColor = Color.White,
        texture = TextureManager.GetTexture("thumbnail_" + this.levelName.ToLower()),
        borderColor = color,
        borderWidth = 4
      };
      Styling styling1 = new Styling()
      {
        borderColor = color,
        borderWidth = 4
      };
      Styling styling2 = new Styling()
      {
        text = str1,
        //font = font1,
        defaultTextColor = color,
        centerText = true,
        borderColor = color,
        borderWidth = 4
      };
      Styling clickableRedTextStyling = Styling.ClickableRedTextStyling;
      Vector2 vector2 = (Vector2) font1.MeasureStringHalf("a");
      Vector2 megaMan2SmallSize = (Vector2) font2.MeasureStringHalf("a");
      Vector2 megaMan2BigSize = (Vector2) font3.MeasureStringHalf("a");
      string str2 = "null";
      for (int index = 0; index < LevelManager.DefaultLevels.Length - 1; ++index)
      {
        if (LevelManager.DefaultLevels[index + 1] == this.levelName)
          str2 = LevelManager.DefaultLevels[index];
      }
      Rectangle empty1 = Rectangle.Empty;
      Styling styling3 = new Styling();
      styling3.tooltip = "Finish '" + str2 + "' to unlock.";
      Styling? style1 = new Styling?(styling3);
      this.lockedContainer = new UIElement(empty1, style1);
      this.lockedContainer.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, parentRect.Height)));
      this.unlockedContainer = new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, parentRect.Height)));
      UIElement child1 = new UIElement(Rectangle.Empty, new Styling?(styling1));
      child1.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2, 0, parentRect.Width / 2, parentRect.Height)));
      if (imageStyling.texture != null)
        child1.AddChild(new UIElement(Rectangle.Empty, new Styling?(imageStyling)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, thumbnailHeight(parentRect)))));
      UIElement child2 = new UIElement(Rectangle.Empty, new Styling?(styling1));
      child2.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width / 2, parentRect.Height)));
      UIElement child3 = new UIElement(Rectangle.Empty, new Styling?(styling2)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, 50)));
      child2.AddChild(child3);
      UIElement child4 = new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(10, 60, parentRect.Width - 20, parentRect.Height - 20 - 50)));
      UIElement uiElement1 = child4;
      Rectangle empty2 = Rectangle.Empty;
      styling3 = new Styling();
      //styling3.font = font2;
      styling3.defaultTextColor = color;
      styling3.text = "Global Highscores:";
      Styling? style2 = new Styling?(styling3);
      UIElement child5 = new UIElement(empty2, style2).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, -(int) megaMan2SmallSize.Y / 2, parentRect.Width, parentRect.Height / 3)));
      uiElement1.AddChild(child5);
      UIElement uiElement2 = child4;
      Rectangle empty3 = Rectangle.Empty;
      styling3 = new Styling();
      //styling3.font = font2;
      styling3.defaultTextColor = color;
      styling3.text = "Best Time";
      Styling? style3 = new Styling?(styling3);
      UIElement child6 = new UIElement(empty3, style3).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 6 * 2 - (int) megaMan2SmallSize.Y / 2, parentRect.Width, parentRect.Height / 6)));
      uiElement2.AddChild(child6);
      child4.AddChild((UIElement) (this.bestTime = new Button(Rectangle.Empty, (Button.OnClick) (() => ScoreExplorer.Instance.ShowList(this.levelName, sortBy: "time", sortDescending: new bool?(false))), clickableRedTextStyling)));
      this.bestTime.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 6 * 3 - (int) megaMan2SmallSize.Y / 2, parentRect.Width, parentRect.Height / 6)));
      UIElement uiElement3 = child4;
      Rectangle empty4 = Rectangle.Empty;
      styling3 = new Styling();
      //styling3.font = font2;
      styling3.defaultTextColor = color;
      styling3.text = "Best Script";
      Styling? style4 = new Styling?(styling3);
      UIElement child7 = new UIElement(empty4, style4).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 6 * 4 - (int) megaMan2SmallSize.Y / 2, parentRect.Width, parentRect.Height / 6)));
      uiElement3.AddChild(child7);
      child4.AddChild((UIElement) (this.bestScript = new Button(Rectangle.Empty, (Button.OnClick) (() => ScoreExplorer.Instance.ShowList(this.levelName, false, sortBy: "lines", sortDescending: new bool?(false))), clickableRedTextStyling)));
      this.bestScript.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 6 * 5 - (int) megaMan2SmallSize.Y / 2, parentRect.Width, parentRect.Height / 6)));
      child2.AddChild(child4);
      this.playButton = new Button(Rectangle.Empty, (Button.OnClick) (() => StateManager.TransitionTo(GameState.InLevel_Default, this.levelName)), Styling.RedButtonStyling with
      {
        text = "Play",
        textOffset = 30,
        centerText = false
      });
      this.playButton.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, thumbnailHeight(parentRect), parentRect.Width, parentRect.Height - thumbnailHeight(parentRect))));
      Button playButton = this.playButton;
      Rectangle empty5 = Rectangle.Empty;
      styling3 = new Styling();
      styling3.spritePos = TextureManager.GetSpritePos("ui_hollowstart");
      styling3.defaultColor = color;
      Styling? style5 = new Styling?(styling3);
      UIElement child8 = new UIElement(empty5, style5).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width - 30 - 20, parentRect.Height / 2 - 10, 20, 20)));
      playButton.AddChild(child8);
      child1.AddChild((UIElement) this.playButton);
      this.unlockedContainer.AddChild(child1);
      this.unlockedContainer.AddChild(child2);
      this.AddChild(this.unlockedContainer);
      UIElement lockedContainer = this.lockedContainer;
      Rectangle empty6 = Rectangle.Empty;
      styling3 = new Styling();
      //styling3.font = font3;
      styling3.text = "?";
      styling3.centerText = true;
      styling3.defaultTextColor = color;
      Styling? style6 = new Styling?(styling3);
      UIElement child9 = new UIElement(empty6, style6).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, 0, parentRect.Width, parentRect.Height)));
      lockedContainer.AddChild(child9);
      this.lockedContainer.AddChild(new UIElement(Rectangle.Empty, new Styling?(styling1)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(0, parentRect.Height / 2, parentRect.Width / 2 - (int) megaMan2BigSize.X, 1))));
      this.lockedContainer.AddChild(new UIElement(Rectangle.Empty, new Styling?(styling1)).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width / 2 + (int) megaMan2BigSize.X, parentRect.Height / 2, parentRect.Width / 2 - (int) megaMan2BigSize.X, 1))));
      this.AddChild(this.lockedContainer);
      this.UpdateView();

      int thumbnailHeight(Rectangle parentRect)
      {
        int val2 = parentRect.Height - 50;
        float num = imageStyling.texture != null ? (float) imageStyling.texture.Height / (float) imageStyling.texture.Width : 1f;
        return Math.Min((int) ((double) parentRect.Width * (double) num), val2);
      }
    }

    public async void UpdateView()
    {
      if (!this.isUnlocked)
      {
        int unlocked = VarManager.GetInt("unlockedlevels");
        int demoLock = VarManager.GetInt("demolock");
        if (unlocked >= demoLock)
          unlocked = 99;
        for (int i = 0; i < unlocked; ++i)
        {
          if (i < LevelManager.DefaultLevels.Length)
          {
            if (LevelManager.DefaultLevels[i] == this.levelName)
            {
              this.isUnlocked = true;
              if (i >= demoLock)
                this.isDemoLocked = true;
            }
          }
          else
            this.isUnlocked = true;
        }
      }
      this.unlockedContainer.SetActive(this.isUnlocked);
      this.lockedContainer.SetActive(!this.isUnlocked);
      this.bestTime.SetInteractable(!this.isDemoLocked);
      this.bestScript.SetInteractable(!this.isDemoLocked);
      this.playButton.SetInteractable(!this.isDemoLocked);
      this.SetTooltip(this.isDemoLocked ? "This Level is not available in the Demo." : "");
      await ScoreManager.UpdateOverview(this.levelName);
      this.scoreOverview = ScoreManager.TryGetOverview(this.levelName);
      bool hasBestTime = (double) this.scoreOverview.time > 0.0;
      bool hasBestScript = this.scoreOverview.lineCount > 0;
      string scriptAvailableTooltip = "Open Leaderboard";
      string scriptUnavailableTooltip = "Open Leaderboard (No highscore found)";
      this.bestTime.SetText(hasBestTime ? this.scoreOverview.time.ToString() + "s" : "-");
      this.bestTime.SetTooltip(hasBestTime ? scriptAvailableTooltip : scriptUnavailableTooltip);
      this.bestScript.SetText(hasBestScript ? this.scoreOverview.lineCount.ToString() + " Lines" : "-");
      this.bestScript.SetTooltip(hasBestScript ? scriptAvailableTooltip : scriptUnavailableTooltip);
      scriptAvailableTooltip = (string) null;
      scriptUnavailableTooltip = (string) null;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.SuccessMenu
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;
using System;
using System.Globalization;

#nullable enable
namespace BotsAreStupid
{
  internal class SuccessMenu : UIElement
  {
    private const int buttonHeight = 50;
    private const int headerHeight = 50;
    private const int padding = 15;
    private 
    #nullable disable
    UIElement timeScore;
    private UIElement scriptScore;
    private UIElement dataContainer;
    private UIElement personalDataContainer;
    private InfoLine timeHighscore;
    private UIElement timeNewHighscoreDisplay;
    private InfoLine timeAverage;
    private InfoLine timeRankDisplay;
    private InfoLine personalTimeHighscore;
    private UIElement personalTimeNewHighscoreDisplay;
    private InfoLine personalTimeAverage;
    private InfoLine personalTimeRankDisplay;
    private InfoLine scriptHighscore;
    private UIElement scriptNewHighscoreDisplay;
    private InfoLine scriptAverage;
    private InfoLine scriptRankDisplay;
    private InfoLine personalScriptHighscore;
    private UIElement personalScriptNewHighscoreDisplay;
    private InfoLine personalScriptAverage;
    private InfoLine personalScriptRankDisplay;
    private UIElement nextButton;
    private UIElement nextLockedInfo;
    private UIElement headerInfo;

    public SuccessMenu(Rectangle rectangle)
      : base(rectangle)
    {
      this.CreateChildren();
      Input.RegisterOnDown(KeyBind.Enter, (InputAction.InputHandler) (() =>
      {
        if (!this.IsActive || !this.nextButton.IsInteractable)
          return;
        this.LoadNext();
      }), GameState.InLevel_Default);
    }

    public void SetScore(
      float time,
      int linecount,
      ScoreData overview,
      ScoreData personalOverview)
    {
      this.timeScore.SetText(time.ToString((IFormatProvider) CultureInfo.InvariantCulture) + " s");
      this.SetTimeScore(time, overview.time, overview.averageTime, this.timeNewHighscoreDisplay, this.timeHighscore, this.timeAverage);
      this.SetTimeScore(time, personalOverview.time, personalOverview.averageTime, this.personalTimeNewHighscoreDisplay, this.personalTimeHighscore, this.personalTimeAverage);
      this.scriptScore.SetText(linecount.ToString() + " Lines");
      this.SetScriptScore(linecount, overview.lineCount, (float) overview.averageLineCount, this.scriptNewHighscoreDisplay, this.scriptHighscore, this.scriptAverage);
      this.SetScriptScore(linecount, personalOverview.lineCount, (float) personalOverview.averageLineCount, this.personalScriptNewHighscoreDisplay, this.personalScriptHighscore, this.personalScriptAverage);
    }

    private void SetTimeScore(
      float current,
      float highscore,
      float average,
      UIElement newHighscore,
      InfoLine highscoreInfo,
      InfoLine averageInfo)
    {
      if ((double) current < (double) highscore)
      {
        newHighscore.SetActive(true);
        highscoreInfo.SetActive(false);
      }
      else
      {
        newHighscore.SetActive(false);
        highscoreInfo.SetActive(true);
        highscoreInfo.SetInfoText(highscore.ToString((IFormatProvider) CultureInfo.InvariantCulture) + " s");
      }
      if ((double) average == 0.0)
        averageInfo.SetInfoText("-");
      else
        averageInfo.SetInfoText(average.ToString((IFormatProvider) CultureInfo.InvariantCulture) + " s");
    }

    private void SetScriptScore(
      int current,
      int highscore,
      float average,
      UIElement newHighscore,
      InfoLine highscoreInfo,
      InfoLine averageInfo)
    {
      if (current < highscore)
      {
        newHighscore.SetActive(true);
        highscoreInfo.SetActive(false);
      }
      else
      {
        newHighscore.SetActive(false);
        highscoreInfo.SetActive(true);
        highscoreInfo.SetInfoText(highscore.ToString() + " Lines");
      }
      if ((double) average == 0.0)
        averageInfo.SetInfoText("-");
      else
        averageInfo.SetInfoText(average.ToString() + " Lines");
    }

    private void LoadNext()
    {
      TextEditor.Instance?.SaveInstructions();
      LevelManager.LoadNext();
    }

    public void UpdateNextButton(bool isAvailable, bool isLocked)
    {
      this.nextButton.SetInteractable(isAvailable && !isLocked);
    }

    public void SetManualControlsInfoActive(bool value)
    {
      if (value)
        this.SetInfo(-1, -1, -1, -1, -1, -1, false);
      this.headerInfo.SetText(value ? "(Wont upload score because of manual controls)" : " ");
    }

    public void SetInfo(
      int timeRank,
      int lineCountRank,
      int totalScores,
      int personalTimeRank,
      int personalLineCountRank,
      int totalPersonalScores,
      bool isDuplicateScript,
      bool isTooLong = false)
    {
      if (timeRank == -1 && lineCountRank == -1 && totalScores == -1)
      {
        this.timeRankDisplay.SetInfoText("-");
        this.scriptRankDisplay.SetInfoText("-");
        this.personalTimeRankDisplay.SetInfoText("-");
        this.personalScriptRankDisplay.SetInfoText("-");
        this.headerInfo.SetText(isTooLong ? "(Couldn't upload script because it is longer than 8000 chars)" : " ");
      }
      else
      {
        this.headerInfo.SetText(isDuplicateScript ? "(Didn't upload script because it exists already)" : " ");
        this.timeRankDisplay.SetInfoText(timeRank.ToString() + "/" + totalScores.ToString());
        this.scriptRankDisplay.SetInfoText(lineCountRank.ToString() + "/" + totalScores.ToString());
        this.personalTimeRankDisplay.SetInfoText(personalTimeRank.ToString() + "/" + totalPersonalScores.ToString());
        this.personalScriptRankDisplay.SetInfoText(personalLineCountRank.ToString() + "/" + totalPersonalScores.ToString());
      }
    }

    public void SetInfoLoading()
    {
      this.timeRankDisplay.SetInfoText("Loading...");
      this.scriptRankDisplay.SetInfoText("Loading...");
    }

    private void CreateChildren()
    {
      Color color1 = ColorManager.GetColor("red");
      Color color2 = ColorManager.GetColor("white");
      Color color3 = ColorManager.GetColor("lightslate");
      Color color4 = ColorManager.GetColor("darkslate");
      BitmapFont font1 = TextureManager.GetFont("megaman2");
      BitmapFont font2 = TextureManager.GetFont("megaman2Big");
      int megaMan2Height = 24;//(int) font1.MeasureStringHalf("a").Height;
      //BitmapFont font3 = TextureManager.GetFont("courier");
      int height1 = 24;//(int) font3.MeasureStringHalf("a").Height;
      Styling style = new Styling()
      {
        defaultColor = color4,
        hoverColor = color3,
        clickColor = color1,
        centerText = true,
        defaultTextColor = color2,
        //font = font1,
        borderColor = color2,
        borderWidth = 4
      };
      Styling styling1 = new Styling()
      {
        defaultTextColor = color2,
        text = "Success!",
        //font = TextureManager.GetFont("megaman2Big")
      };
      Styling styling2 = new Styling()
      {
        defaultTextColor = color2,
        //font = font1
      };
      Styling styling3 = new Styling()
      {
        defaultTextColor = color1,
        //font = font2
      };
      Styling highscoreStyling = new Styling()
      {
        defaultTextColor = color2,
        //font = font1,
        rightText = true
      };
      Styling newHighscoreStyling = new Styling()
      {
        defaultTextColor = color2,
        //font = font1,
        text = "New Highscore!"
      };
      this.AddChild(new UIElement(new Rectangle(15, 15, this.rectangle.Width - 30, 50), new Styling?(styling1)));
      int y1 = 80;
      int width1 = this.rectangle.Width - 30;
      this.AddChild(this.headerInfo = new UIElement(new Rectangle(15, y1, width1, 0)));
      int width2 = (this.rectangle.Width - 30) / 2;
      bool flag = VarManager.HasBool("successpersonalinfo") && VarManager.GetBool("successpersonalinfo");
      this.AddChild((UIElement) new ToggleLine(new Rectangle(this.rectangle.Width - 15 - width2, 40 - megaMan2Height / 2, width2, megaMan2Height), "Show Info:", (Button.OnClick) (() =>
      {
        this.dataContainer.SetActive(false);
        this.personalDataContainer.SetActive(true);
        VarManager.SetBool("successpersonalinfo", true);
        VarManager.SaveOptions();
      }), (Button.OnClick) (() =>
      {
        this.dataContainer.SetActive(true);
        this.personalDataContainer.SetActive(false);
        VarManager.SetBool("successpersonalinfo", false);
        VarManager.SaveOptions();
      }), "Global", "Personal", flag));
      UIElement content = new UIElement(new Rectangle(15, y1 + 15 + height1, width1, this.rectangle.Height - 50 - y1 - 30 - height1));
      int height2 = content.Height / 2 - 45 - megaMan2Height;
      int y2 = 30 + megaMan2Height;
      int statsCenterY = y2 + height2 / 2;
      styling2.text = "Your Time:";
      content.AddChild(new UIElement(new Rectangle(0, 15, content.Width, 0), new Styling?(styling2)));
      content.AddChild(this.timeScore = new UIElement(new Rectangle(0, y2, content.Width / 2, height2), new Styling?(styling3)));
      styling2.text = "Your Lines:";
      content.AddChild(new UIElement(new Rectangle(0, content.Height / 2 + 15, content.Width, 0), new Styling?(styling2)));
      content.AddChild(this.scriptScore = new UIElement(new Rectangle(0, content.Height / 2 + y2, content.Width / 2, height2), new Styling?(styling3)));
      content.AddChild(this.dataContainer = createContainer(ref this.timeRankDisplay, ref this.timeHighscore, ref this.timeNewHighscoreDisplay, ref this.timeAverage, ref this.scriptRankDisplay, ref this.scriptHighscore, ref this.scriptNewHighscoreDisplay, ref this.scriptAverage));
      content.AddChild(this.personalDataContainer = createContainer(ref this.personalTimeRankDisplay, ref this.personalTimeHighscore, ref this.personalTimeNewHighscoreDisplay, ref this.personalTimeAverage, ref this.personalScriptRankDisplay, ref this.personalScriptHighscore, ref this.personalScriptNewHighscoreDisplay, ref this.personalScriptAverage));
      this.dataContainer.SetActive(!flag);
      this.personalDataContainer.SetActive(flag);
      this.AddChild(content);
      int num = this.rectangle.Width / 3;
      style.text = "Resume";
      this.AddChild((UIElement) new Button(new Rectangle(0, this.rectangle.Height - 50, num, 50), (Button.OnClick) (async () =>
      {
        PopupMenu.Instance.SetActive(false);
        await ScoreManager.UpdateOverview();
      }), style));
      style.text = "Exit";
      this.AddChild((UIElement) new Button(new Rectangle(num, this.rectangle.Height - 50, num, 50), (Button.OnClick) (() => StateManager.TransitionBack()), style));
      Styling redButtonStyling = Styling.RedButtonStyling with
      {
        text = "Next"
      };
      this.AddChild(this.nextButton = (UIElement) new Button(new Rectangle(num * 2, this.rectangle.Height - 50, num, 50), new Button.OnClick(this.LoadNext), redButtonStyling));

      UIElement createContainer(
        ref InfoLine timeRankDisplay,
        ref InfoLine timeHighscore,
        ref UIElement timeNewHighscoreDisplay,
        ref InfoLine timeAverage,
        ref InfoLine scriptRankDisplay,
        ref InfoLine scriptHighscore,
        ref UIElement scriptNewHighscoreDisplay,
        ref InfoLine scriptAverage)
      {
        UIElement container = new UIElement(new Rectangle(0, 0, content.Width, content.Height));
        UIElement child1 = new UIElement(new Rectangle(0, 0, content.Width, content.Height / 2));
        child1.AddChild((UIElement) (timeRankDisplay = new InfoLine(new Rectangle(content.Width / 2, statsCenterY - megaMan2Height / 2 - 15, child1.Width / 2, 0), "Rank:", "Loading ...", new Styling?(highscoreStyling))));
        child1.AddChild((UIElement) (timeHighscore = new InfoLine(new Rectangle(child1.Width / 2, statsCenterY, child1.Width / 2, 0), "Highscore:", "Loading ...", new Styling?(highscoreStyling))));
        child1.AddChild(timeNewHighscoreDisplay = new UIElement(new Rectangle(child1.Width / 2, statsCenterY, child1.Width / 2, 0), new Styling?(newHighscoreStyling)));
        timeNewHighscoreDisplay.SetActive(false);
        child1.AddChild((UIElement) (timeAverage = new InfoLine(new Rectangle(child1.Width / 2, statsCenterY + megaMan2Height / 2 + 15, child1.Width / 2, 0), "Average:", "Loading ...", new Styling?(highscoreStyling))));
        container.AddChild(child1);
        UIElement child2 = new UIElement(new Rectangle(0, content.Height / 2, content.Width, content.Height / 2));
        child2.AddChild((UIElement) (scriptRankDisplay = new InfoLine(new Rectangle(child2.Width / 2, statsCenterY - megaMan2Height / 2 - 15, child2.Width / 2, 0), "Rank:", "Loading ...", new Styling?(highscoreStyling))));
        child2.AddChild((UIElement) (scriptHighscore = new InfoLine(new Rectangle(child2.Width / 2, statsCenterY, child2.Width / 2, 0), "Highscore:", "Loading ...", new Styling?(highscoreStyling))));
        child2.AddChild(scriptNewHighscoreDisplay = new UIElement(new Rectangle(child2.Width / 2, statsCenterY, child2.Width / 2, 0), new Styling?(newHighscoreStyling)));
        scriptNewHighscoreDisplay.SetActive(false);
        child2.AddChild((UIElement) (scriptAverage = new InfoLine(new Rectangle(child2.Width / 2, statsCenterY + megaMan2Height / 2 + 15, child2.Width / 2, 0), "Average:", "Loading ...", new Styling?(highscoreStyling))));
        container.AddChild(child2);
        return container;
      }
    }
  }
}

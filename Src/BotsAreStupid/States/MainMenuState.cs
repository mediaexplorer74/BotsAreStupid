// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.States.MainMenuState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace BotsAreStupid.States
{
  internal class MainMenuState : AbstractState
  {
    public override GameState Type => GameState.MainMenu;

    public override Rectangle LevelRect => new Rectangle(0, 175, 1280, 525);

    public override void CreateUI(Rectangle fullRect)
    {
      base.CreateUI(fullRect);
      this.MainUIElement = (UIElement) new MainMenu(fullRect);
    }

    public override void Enter(StateTransition transition)
    {
      base.Enter(transition);
      LevelManager.Load("MainMenu");
    }

    public override void UpdateMatrices(float windowWidth, float windowHeight)
    {
      base.UpdateMatrices(windowWidth, windowHeight);
      Vector2 virtualSize = Utility.VirtualSize;
      float num1 = windowHeight / virtualSize.Y;
      Rectangle levelRect = this.LevelRect;
      float val2_1 = (float) ((double) windowHeight - 150.0 * (double) num1 - 100.0 * (double) num1);
      float val2_2 = windowWidth;
      float num2 = val2_2;
      float num3 = val2_1;
      float num4 = num2 / (float) levelRect.Width;
      if ((double) levelRect.Height * (double) num4 > (double) val2_1)
        num4 = num3 / (float) levelRect.Height;
      Math.Min((float) levelRect.Width * num4, val2_2);
      float num5 = Math.Min((float) levelRect.Height * num4, val2_1);
      MainMenu.headerHeight = (int) (((double) windowHeight - (double) num5) / 3.0 * 2.0 * 1.0 / (double) num1);
      MainMenu.footerHeight = (int) (((double) windowHeight - (double) num5) / 3.0 * 1.0 / (double) num1);
      float xPosition = windowWidth / 2f;
      float yPosition = (float) ((double) MainMenu.headerHeight * (double) num1 + (double) num5 / 2.0);
      this.LevelViewMatrix = new Matrix?(Matrix.CreateTranslation((float) (-levelRect.X - levelRect.Width / 2), (float) (-levelRect.Y - levelRect.Height / 2), 0.0f) * Matrix.CreateScale(num4, num4, 1f) * Matrix.CreateTranslation(xPosition, yPosition, 0.0f));
      MainMenu.Instance.SetSize((int) ((double) windowWidth * (1.0 / (double) num1)), MainMenu.Instance.Height, true);
      Vector2 vector2 = new Vector2((float) PopupMenu.Instance.Width, (float) PopupMenu.Instance.Height);
      PopupMenu.Instance.SetPosition(new Vector2((float) levelRect.Width, (float) levelRect.Height) / 2f - vector2 / 2f);
    }
  }
}

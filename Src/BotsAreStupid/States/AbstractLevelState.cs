// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.States.AbstractLevelState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace BotsAreStupid.States
{
  internal abstract class AbstractLevelState : AbstractState
  {
    private Vector2 currentCamPos;

    public override Rectangle LevelRect
    {
      get
      {
        return new Rectangle(340, 0, VarManager.GetInt("levelwidth"), VarManager.GetInt("levelheight"));
      }
    }

    protected virtual bool EnableSidebar => false;

    protected virtual bool FixedCamPosition => false;

    protected virtual bool CanMouseZoom => false;

    public override void Enter(StateTransition transition)
    {
      base.Enter(transition);
      VarManager.SetInt("overclock", 1);
      VarManager.SetBool("manualControls", false);
      if (this.CanMouseZoom)
        Input.RegisterOnScroll(new Input.MouseScrollHandler(this.HandleMouseScroll));
      SimulationManager.UpdateAutoGhost();
    }

    public override void Exit(bool closePopupMenu = true)
    {
      base.Exit(closePopupMenu);
      if (!this.CanMouseZoom)
        return;
      Input.UnRegisterOnScroll(new Input.MouseScrollHandler(this.HandleMouseScroll));
    }

    private void HandleMouseScroll(float scroll)
    {
      if (!Utility.MouseInside(this.LevelRect, true) || PopupMenu.Instance.IsActive)
        return;
      int num = VarManager.GetInt("camzoom");
      int targetValue = (int) MathHelper.Clamp((float) num + scroll * 0.5f, 0.0f, 1000f);
      if (num != targetValue)
        VarManager.AnimateInt("camzoom", targetValue, 0.03f);
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      Simulation mainSimulation = SimulationManager.MainSimulation;
      bool flag = this.FixedCamPosition || VarManager.GetBool("camfixedposition");
      if (flag)
        this.currentCamPos = new Vector2((float) VarManager.GetInt("camposx"), (float) VarManager.GetInt("camposy"));
      else if (mainSimulation.Player != null)
      {
        Vector2 vector2 = mainSimulation.Player.Center - this.currentCamPos;
        float num = VarManager.HasInt("camspeed") ? (float) VarManager.GetInt("camspeed") : 10f;
        this.currentCamPos += vector2 * deltaTime * num;
      }
      float t;
      float zoom = Utility.GetZoom(out t);
      Vector2 currentCamPos = this.currentCamPos;
      Vector2 levelSize = Utility.LevelSize;
      Vector2 vector2_1 = new Vector2((float) (340.0 + (double) levelSize.X / 2.0), levelSize.Y / 2f);
      Vector2 vector2_2 = (currentCamPos - vector2_1) * zoom;
      Vector2 vector2_3 = levelSize / 2f * Utility.Lerp(0.0f, 2f, t);
      vector2_2.X = MathHelper.Clamp(vector2_2.X, -vector2_3.X, vector2_3.X);
      vector2_2.Y = MathHelper.Clamp(vector2_2.Y, -vector2_3.Y, vector2_3.Y);
      this.currentCamPos = vector2_2 / zoom + vector2_1;
      if (flag)
      {
        if ((double) currentCamPos.X != (double) this.currentCamPos.X)
          VarManager.SetInt("camposx", (int) this.currentCamPos.X);
        if ((double) currentCamPos.Y != (double) this.currentCamPos.Y)
          VarManager.SetInt("camposy", (int) this.currentCamPos.Y);
      }
      Matrix translation = Matrix.CreateTranslation((float) (-340.0 - (double) levelSize.X / 2.0), (float) (-(double) levelSize.Y / 2.0), 0.0f);
      this.LevelCameraMatrix = new Matrix?(translation * Matrix.CreateScale(zoom, zoom, 1f) * Matrix.CreateTranslation(-vector2_2.X, -vector2_2.Y, 0.0f) * Matrix.Invert(translation));
    }

    public override void UpdateMatrices(float windowWidth, float windowHeight)
    {
      if ((double) (windowWidth / windowHeight) < 0.60000002384185791)
        this.CalculatePortrait(windowWidth, windowHeight);
      else
        this.CalculateLandscape(windowWidth, windowHeight);
    }

    private void CalculateLandscape(float windowWidth, float windowHeight)
    {
      int width = this.LevelRect.Width;
      int height = this.LevelRect.Height;
      float num1 = 30f;
      float num2 = windowWidth / (float) Utility.VirtualWidth;
      float num3 = 1f;
      if ((double) num2 >= 1.5)
        num3 = 1.6f;
      if ((double) num2 >= 2.0)
        num3 = 2f;
      if ((double) num2 >= 2.5)
        num3 = 2.6f;
      if (VarManager.HasInt("editoruiscale"))
        num3 *= (float) VarManager.GetInt("editoruiscale") / 100f;
      float num4 = this.EnableSidebar ? MathHelper.Clamp((float) VarManager.GetInt("minsidebarwidth") * num3, 140f, 280f) : 0.0f;
      float num5 = MathHelper.Clamp((float) VarManager.GetInt("minleveluiwidth") * num3, 340f, windowWidth - (float) (width / 2) * num3 - num4);
      float val2_1 = windowHeight - (float) ((double) num1 * (double) num3 * 2.0);
      float val2_2 = windowWidth - num5 - num4;
      float num6 = Math.Max(val2_1 / (float) height, 0.1f);
      if ((double) width * (double) num6 > (double) val2_2)
        num6 = Math.Max(val2_2 / (float) width, 0.1f);
      float num7 = num6 * ((float) VarManager.GetInt("viewscale") / 100f);
      float num8 = Math.Min((float) width * num7, val2_2);
      Math.Min((float) height * num7, val2_1);
      this.LevelViewMatrix = new Matrix?(Matrix.CreateTranslation((float) (-340 - width / 2), (float) (-height / 2), 0.0f) * Matrix.CreateScale(num7, num7, 1f) * Matrix.CreateTranslation((float) ((double) windowWidth - (double) num8 / 2.0 - (double) num4 + 2.0) + (float) VarManager.GetInt("viewoffsetx"), windowHeight / 2f + (float) VarManager.GetInt("viewoffsety"), 0.0f));
      Game1.Instance.WindowMatrix = Matrix.CreateScale(num3, num3, 1f);
      VarManager.SetInt("leveluiwidth", (int) (((double) windowWidth - (double) num8 - (double) num4) * (1.0 / (double) num3)));
      VarManager.SetInt("leveluiheight", (int) ((double) windowHeight * (1.0 / (double) num3)));
      VarManager.SetInt("sidebarwidth", (int) ((double) num4 * (1.0 / (double) num3)));
      VarManager.SetInt("sidebarx", (int) (((double) windowWidth - (double) num4 + 4.0) * (1.0 / (double) num3)));
      ScoreExplorer.Instance.SetPosition(new int?(340), new int?(0));
      ScoreExplorer.Instance.SetSize(width, height, true);
      PopupMenu.Instance.SetPosition(new int?(340 + width / 2 - PopupMenu.Instance.Width / 2), new int?(height / 2 - PopupMenu.Instance.Height / 2));
    }

    private void CalculatePortrait(float windowWidth, float windowHeight)
    {
      int width = this.LevelRect.Width;
      int height = this.LevelRect.Height;
      Vector2 virtualSize = Utility.VirtualSize;
      float num1 = 1.5f;
      float num2 = 26f * num1;
      float num3 = 500f;
      float val2_1 = windowWidth - num2 * 2f;
      float val2_2 = windowHeight - num3;
      float num4 = val2_1;
      float num5 = val2_2;
      float num6 = Math.Max(num4 / (float) width, 0.1f);
      if ((double) height * (double) num6 > (double) val2_1)
        num6 = Math.Max(num5 / (float) height, 0.1f);
      Math.Min((float) width * num6, val2_1);
      float num7 = Math.Min((float) height * num6, val2_2);
      float xPosition = windowWidth / 2f;
      float num8 = num7 / 2f;
      this.LevelViewMatrix = new Matrix?(Matrix.CreateTranslation((float) (-340 - width / 2), (float) (-height / 2), 0.0f) * Matrix.CreateScale(num6, num6, 1f) * Matrix.CreateTranslation(xPosition, num8 + 40f, 0.0f));
      Game1.Instance.WindowMatrix = Matrix.CreateScale(num1, num1, 1f) * Matrix.CreateTranslation(0.0f, (float) (height + 130), 0.0f);
      VarManager.SetInt("leveluiwidth", (int) ((double) windowWidth * 1.0 / (double) num1));
      VarManager.SetInt("leveluiheight", (int) (((double) windowHeight - (double) num7) * 1.0 / (double) num1) - 50);
      ScoreExplorer.Instance.SetPosition(new int?(340), new int?(0));
      ScoreExplorer.Instance.SetSize(width, height, true);
      PopupMenu.Instance.SetPosition(new int?(340 + width / 2 - PopupMenu.Instance.Width / 2), new int?(height / 2 - PopupMenu.Instance.Height / 2));
    }
  }
}

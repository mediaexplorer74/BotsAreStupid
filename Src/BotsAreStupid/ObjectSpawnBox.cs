// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ObjectSpawnBox
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal class ObjectSpawnBox : UIElement
  {
    private const int padding = 15;
    private GameObject gameObject;

    public bool IsLocked { get; private set; }

    public ObjectSpawnBox(Rectangle rectangle, GameObject gameObject)
      : base(rectangle)
    {
      ObjectSpawnBox objectSpawnBox = this;
      gameObject.CurrentSimulation.RemoveObject((BaseObject) gameObject);
      this.gameObject = gameObject;
      Input.RegisterMouse(InputEvent.OnDown, new InputAction.InputHandler(this.OnMouseDown), GameState.LevelEditor);
      this.ChangeStyle(new Styling()
      {
        hoverCursor = new bool?(true)
      });
      this.AddResponsiveAction((UIElement.ResponsiveActionHandler) (parentRect =>
      {
        int num = objectSpawnBox.GlobalRect.Width - 30;
        if (gameObject.Width > gameObject.Height)
        {
          gameObject.SetScaleWidth(num);
          int height = num * gameObject.Height / gameObject.Width;
          gameObject.SetScaleHeight(height);
          gameObject.SetPosition(new Vector2((float) (objectSpawnBox.GlobalRect.X + objectSpawnBox.GlobalRect.Width / 2 - num / 2), (float) (objectSpawnBox.GlobalRect.Y + objectSpawnBox.GlobalRect.Height / 2 - height / 2)));
        }
        else
        {
          gameObject.SetScaleHeight(num);
          int width = num * gameObject.Width / gameObject.Height;
          gameObject.SetScaleWidth(width);
          gameObject.SetPosition(new Vector2((float) (objectSpawnBox.GlobalRect.X + objectSpawnBox.GlobalRect.Width / 2 - width / 2), (float) (objectSpawnBox.GlobalRect.Y + objectSpawnBox.GlobalRect.Height / 2 - num / 2)));
        }
      }));
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);
      this.gameObject.Draw(spriteBatch);
      if (!this.IsLocked)
        return;
      Rectangle globalRect = this.GlobalRect;
      int y = globalRect.Y + globalRect.Height / 2;
      Utility.DrawLine(spriteBatch, new Vector2((float) globalRect.X, (float) y), new Vector2((float) (globalRect.X + globalRect.Width), (float) y), this.style.borderWidth, new Color?(this.style.borderColor), 0.0f, 0);
      int num1 = globalRect.Width / 2;
      int num2 = globalRect.Width / 3;
      spriteBatch.Draw(TextureManager.GetCircleTexture(num1 * 2), new Rectangle(globalRect.X + globalRect.Width / 2 - num1 / 2, globalRect.Y + globalRect.Height / 2 - num1 / 2, num1, num1), new Rectangle?(), this.style.borderColor);
      spriteBatch.Draw(TextureManager.GetTexture("uisheet"), new Rectangle(globalRect.X + globalRect.Width / 2 - num2 / 2, globalRect.Y + globalRect.Height / 2 - num2 / 2, num2, num2), new Rectangle?(TextureManager.GetSpritePos("ui_locked")), ColorManager.GetColor("red"));
    }

    public void SetLocked()
    {
      this.IsLocked = true;
      this.SetTooltip("This object is not available in the Demo.");
      this.style.borderColor = Utility.ChangeColor(this.style.borderColor, -0.35f);
      this.style.hoverColor = this.style.defaultColor;
      this.style.hoverCursor = new bool?(false);
    }

    private void OnMouseDown()
    {
      if (!this.IsActive || !this.MouseOnRectangle() || this.IsLocked)
        return;
      GameObject gameObject = this.gameObject.Copy();
      Vector2 vector2 = this.LocalMousePos(forceTransformedLevelSpace: true);
      LevelEditor.Instance.SpawnObject(gameObject);
      gameObject.SetPosition(vector2.X - (float) (gameObject.Width / 2), vector2.Y - (float) (gameObject.Height / 2));
      Input.CancelCurrentAction = true;
    }
  }
}

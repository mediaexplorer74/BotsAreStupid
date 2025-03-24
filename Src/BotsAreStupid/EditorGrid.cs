// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.EditorGrid
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal class EditorGrid
  {
    public static EditorGrid Instance = new EditorGrid();
    public static int CellSize = 28;
    private Color color = Utility.ChangeColor(Color.White, -0.75f);

    public bool IsActive { get; private set; } = true;

    public bool SnappingEnabled { get; private set; } = false;

    public void TryDraw(SpriteBatch spriteBatch)
    {
      if (!this.IsActive || !StateManager.IsState(GameState.LevelEditor))
        return;
      Texture2D gridTexture = TextureManager.GetGridTexture(Utility.VirtualWidth, Utility.VirtualHeight, EditorGrid.CellSize, 1);
      spriteBatch.Draw(gridTexture, new Vector2(340f, 0.0f), new Rectangle?(), this.color);
    }

    public static void ToggleActive()
    {
      EditorGrid.Instance.IsActive = !EditorGrid.Instance.IsActive;
    }

    public static void SetSnappingEnabled(bool value)
    {
      EditorGrid.Instance.SnappingEnabled = value;
    }
  }
}

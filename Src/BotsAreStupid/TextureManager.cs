// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.TextureManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class TextureManager
  {
    private Dictionary<string, Texture2D> textures;
    private Dictionary<string, Rectangle> spritePositions;
    private Dictionary<string, BitmapFont> fonts;

    public static TextureManager Instance { get; private set; }

    public TextureManager(ContentManager Content)
    {
      TextureManager.Instance = this;
      this.textures = new Dictionary<string, Texture2D>();
      this.spritePositions = new Dictionary<string, Rectangle>();
      this.fonts = new Dictionary<string, BitmapFont>();
      //this.textures.TryAdd("tileset", Content.Load<Texture2D>("Textures/tileset"));
      //this.textures.TryAdd("uisheet", Content.Load<Texture2D>("Textures/UI"));
      //this.textures.TryAdd("logo", Content.Load<Texture2D>("Textures/logo"));
      //this.textures.TryAdd("leleg", Content.Load<Texture2D>("Textures/leleg"));
      foreach (string defaultLevel in LevelManager.DefaultLevels)
      {
        string lower = defaultLevel.ToLower();
        try
        {
          //this.textures.TryAdd("thumbnail_" + lower, Content.Load<Texture2D>("Textures/Thumbnails/" + lower));
        }
        catch
        {
          Console.WriteLine("[TextureManager] No Thumbnail found for " + defaultLevel);
        }
      }
      Texture2D texture2D = new Texture2D(Game1.Instance.GraphicsDM.GraphicsDevice, 1, 1);
      //texture2D.SetData<Color>(new Color[1]{ Color.White });
      //this.textures.TryAdd("line", texture2D);
      //this.spritePositions.TryAdd("booster", new Rectangle(441, 25, 43, 45));
      //this.spritePositions.TryAdd("spike", new Rectangle(193, 45, 15, 4));
      //this.spritePositions.TryAdd("energyorb", new Rectangle(209, 257, 14, 14));
      //this.spritePositions.TryAdd("portal", new Rectangle(78, 206, 36, 36));
      //this.spritePositions.TryAdd("bouncer_base", new Rectangle(16, 80, 16, 16));
      //this.spritePositions.TryAdd("bouncer_pad", new Rectangle(32, 80, 16, 16));
      //this.spritePositions.TryAdd("spawnpipe", new Rectangle(227, 192, 10, 16));
      //this.spritePositions.TryAdd("movingplatform", new Rectangle(352, 48, 32, 5));
      //this.fonts.TryAdd("courier", Content.Load<BitmapFont>("Fonts/Courier"));
      //this.fonts.TryAdd("couriermedium", Content.Load<BitmapFont>("Fonts/CourierMedium"));
      //this.fonts.TryAdd("courierbig", Content.Load<BitmapFont>("Fonts/CourierBig"));
      //this.fonts.TryAdd("megaman2", Content.Load<BitmapFont>("Fonts/MegaMan2"));
      //this.fonts.TryAdd("megaman2big", Content.Load<BitmapFont>("Fonts/MegaMan2Big"));
      //this.fonts.TryAdd("megaman2small", Content.Load<BitmapFont>("Fonts/MegaMan2Small"));
      //this.spritePositions.TryAdd("ui_leveleditor", new Rectangle(0, 0, 418, 45));
      //this.spritePositions.TryAdd("ui_options", new Rectangle(0, 46, 270, 45));
      //this.spritePositions.TryAdd("ui_credits", new Rectangle(0, 92, 256, 45));
      //this.spritePositions.TryAdd("ui_play", new Rectangle(0, 138, 164, 45));
      //this.spritePositions.TryAdd("ui_quit", new Rectangle(0, 184, 146, 60));
      //this.spritePositions.TryAdd("ui_more", new Rectangle(320, 64, 64, 64));
      //this.spritePositions.TryAdd("ui_step", new Rectangle(384, 64, 64, 64));
      //this.spritePositions.TryAdd("ui_cross", new Rectangle(256, 128, 64, 64));
      //this.spritePositions.TryAdd("ui_roundcross", new Rectangle(320, 128, 64, 64));
      //this.spritePositions.TryAdd("ui_left", new Rectangle(384, 128, 64, 64));
      //this.spritePositions.TryAdd("ui_right", new Rectangle(448, 128, 64, 64));
      //this.spritePositions.TryAdd("ui_discord", new Rectangle(192, 192, 64, 64));
      //this.spritePositions.TryAdd("ui_twitter", new Rectangle(256, 192, 64, 64));
      //this.spritePositions.TryAdd("ui_help", new Rectangle(320, 192, 64, 64));
      //this.spritePositions.TryAdd("ui_tools", new Rectangle(384, 192, 64, 64));
      //this.spritePositions.TryAdd("ui_import", new Rectangle(448, 192, 64, 64));
      //this.spritePositions.TryAdd("ui_hollowstart", new Rectangle(0, 256, 128, 128));
      //this.spritePositions.TryAdd("ui_start", new Rectangle(0, 384, 128, 128));
      //this.spritePositions.TryAdd("ui_pause", new Rectangle(128, 256, 128, 128));
      //this.spritePositions.TryAdd("ui_reset", new Rectangle(128, 384, 128, 128));
      //this.spritePositions.TryAdd("ui_delete", new Rectangle(256, 256, 128, 128));
      //this.spritePositions.TryAdd("ui_duplicate", new Rectangle(256, 384, 128, 128));
      //this.spritePositions.TryAdd("ui_rotate", new Rectangle(384, 256, 128, 128));
      //this.spritePositions.TryAdd("ui_shuffle", new Rectangle(384, 384, 128, 128));
      //this.spritePositions.TryAdd("ui_locked", new Rectangle(448, 0, 64, 64));
      //this.spritePositions.TryAdd("ui_unlocked", new Rectangle(448, 64, 64, 64));

            for (int width = 1; width <= 20; ++width)
            {
                //this.textures.TryAdd("circle" + width.ToString(), TextureManager.GetCircleTexture(width));
            }
    }

    public static Texture2D GetTexture(string name)
    {
      if (TextureManager.Instance == null)
        return (Texture2D) null;
      Texture2D texture;
      TextureManager.Instance.textures.TryGetValue(name.ToLower(), out texture);
      return texture;
    }

    public static Rectangle GetSpritePos(string name)
    {
      if (TextureManager.Instance == null)
        return new Rectangle();
      Rectangle spritePos;
      if (!TextureManager.Instance.spritePositions.TryGetValue(name.ToLower(), out spritePos))
        Console.WriteLine("[TextureManager] Didnt find spritepos: " + name);
      return spritePos;
    }

    public static BitmapFont GetFont(string name)
    {
      BitmapFont font;
      if (!TextureManager.Instance.fonts.TryGetValue(name.ToLower(), out font))
        Console.WriteLine("[TextureManager] Didnt find font: " + name);
      return font;
    }

    public static Texture2D GetRectangleTexture(int width, int height, Color color, Color? fadeTo = null)
    {
      if (TextureManager.Instance == null)
        return (Texture2D) null;
      string str = "rectangle" + width.ToString() + "x" + height.ToString();
      Texture2D texture = TextureManager.GetTexture(str);
      if (color == Color.White && texture != null)
        return texture;
      Color[] data = new Color[width * height];
      Texture2D rectangleTexture = new Texture2D(Game1.Instance.GraphicsDM.GraphicsDevice, width, height);
      if (fadeTo.HasValue)
      {
        Color color1 = fadeTo.Value;
        for (int index = 0; index < data.Length; ++index)
        {
          float amount = (float) (index / width) / (float) height;
          data[index] = Color.Lerp(color, color1, amount);
        }
      }
      else
      {
        for (int index = 0; index < data.Length; ++index)
          data[index] = color;
      }
      rectangleTexture.SetData<Color>(data);

        if (color == Color.White)
        {
            //TextureManager.Instance.textures.TryAdd(str, rectangleTexture);
        }

      return rectangleTexture;
    }

    public static Texture2D GetCircleTexture(int width, int hollowSize = 0)
    {
      if (TextureManager.Instance == null)
        return (Texture2D) null;
      string str = "circle" + width.ToString() + "h" + hollowSize.ToString();
      Texture2D texture = TextureManager.GetTexture(str);
      if (texture != null)
        return texture;
      Color[] data = new Color[width * width];
      Texture2D circleTexture = new Texture2D(Game1.Instance.GraphicsDM.GraphicsDevice, width, width);
      Vector2 vector2 = new Vector2((float) (width / 2), (float) (width / 2));
      for (int index = 0; index < data.Length; ++index)
      {
        int x = index % width;
        int y = index / width;
        float num1 = Vector2.Distance(vector2, new Vector2((float) x, (float) y));
        int num2 = (double) num1 > (double) (width / 2) ? 0 : (hollowSize <= 0 ? 1 : ((double) num1 >= (double) hollowSize ? 1 : 0));
        data[index] = num2 == 0 ? Color.Transparent : Color.White;
      }
      //TextureManager.Instance.textures.TryAdd(str, circleTexture);
      circleTexture.SetData<Color>(data);
      return circleTexture;
    }

    public static Texture2D GetSineTexture(int height, int width, float treshold = 0.1f)
    {
      if (TextureManager.Instance == null)
        return (Texture2D) null;
      string str = "sine" + height.ToString() + "x" + width.ToString() + "t" + treshold.ToString();
      Texture2D texture = TextureManager.GetTexture(str);
      if (texture != null)
        return texture;
      Color[] data = new Color[width * height];
      Texture2D sineTexture = new Texture2D(Game1.Instance.GraphicsDM.GraphicsDevice, width, height);
      for (int index1 = 0; index1 < height; ++index1)
      {
        for (int index2 = 0; index2 < width; ++index2)
        {
          float num1 = (float) index2 / (float) width;
          float num2 = (float) index1 / (float) height;
          bool flag = (double) Math.Abs((float) (Math.Sin((double) num1 * Math.PI * 2.0) + 1.0) / 2f - num2) < (double) treshold;
          data[index1 * width + index2] = flag ? Color.White : Color.Transparent;
        }
      }
      sineTexture.SetData<Color>(data);
      //TextureManager.Instance.textures.TryAdd(str, sineTexture);
      return sineTexture;
    }

    public static Texture2D GetGridTexture(int width, int height, int cellSize, int lineWidth)
    {
      if (TextureManager.Instance == null)
        return (Texture2D) null;
      string str = "grid" + width.ToString() + "x" + height.ToString() + "c" + cellSize.ToString() + "l" + lineWidth.ToString();
      Texture2D texture = TextureManager.GetTexture(str);
      if (texture != null)
        return texture;
      Color[] data = new Color[width * height];
      Texture2D gridTexture = new Texture2D(Game1.Instance.GraphicsDM.GraphicsDevice, width, height);
      for (int index1 = 0; index1 < height; ++index1)
      {
        for (int index2 = 0; index2 < width; ++index2)
        {
          bool flag = index2 % cellSize < lineWidth || index1 % cellSize < lineWidth;
          data[index1 * width + index2] = flag ? Color.White : Color.Transparent;
        }
      }
      gridTexture.SetData<Color>(data);
      //TextureManager.Instance.textures.TryAdd(str, gridTexture);
      return gridTexture;
    }
  }
}

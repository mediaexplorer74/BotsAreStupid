// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Utility
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using MonoGame.Extended;
//using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

#nullable enable
namespace BotsAreStupid
{
  internal static class Utility
  {
    public static 
    #nullable disable
    string clipboard;
    public const int legacyUiWidth = 340;
    public const int minSidebarWidth = 140;
    public static float pixelsToMeters = 0.0357142873f;
    public static Random random = new Random();
    private const int virtualWidth = 1280;
    private const int virtualHeight = 800;
    public static Vector2 VirtualSize = new Vector2((float) Utility.VirtualWidth, (float) Utility.VirtualHeight);
    public static Vector2 LevelSize = new Vector2((float) VarManager.GetInt("levelwidth"), (float) VarManager.GetInt("levelheight"));
    public static readonly Rectangle LevelConstraints = new Rectangle(340, 0, 1280, 800);
    private const string fullContentPattern = "[^{}%~]+(?![^{]*})";
    private const string searchPattern = "%{\\w+}.*?(%|$)";
    private const string specialModifierPattern = "(?<=%{)\\w+";
    private const string specialContentPattern = "(?<=%{\\w+})[^%]+";
    private static string[] cussWords = new string[15]
    {
      "anal", "anus", "ass", "cock", "cunt", "dick", "dyke", "faggot",
      "fuck", "kike",  "nigger",  "nigga", "penis",  "pussy", "fortnite"
    };

    public static int VirtualWidth => 1280;

    public static int VirtualHeight => 800;

    public static Player MainPlayer => SimulationManager.MainSimulation.Player;

    public static Rectangle WindowConstraints
    {
      get => new Rectangle(0, 0, Game1.GetWindowSize(true).Item1, Game1.GetWindowSize(true).Item2);
    }

    public static bool PointInside(Vector2 pos, Rectangle rec)
    {
      return (double) pos.X >= (double) rec.X && (double) pos.X <= (double) (rec.X + rec.Width) 
                && (double) pos.Y >= (double) rec.Y && (double) pos.Y <= (double) (rec.Y + rec.Height);
    }

    public static int GetRandom(int max, Random r = null) => (r ?? Utility.random).Next(max + 1);

    public static int GetRandom(int min, int max, bool canBeNegative = false, Random r = null)
    {
      return (min + (r ?? Utility.random).Next(max - min + 1)) * (!canBeNegative || !Utility.GetBool() ? 1 : -1);
    }

    public static float GetRandom(float min, float max, bool canBeNegative = false, Random r = null)
    {
      return min + (float) ((r ?? Utility.random).NextDouble() * ((double) max - (double) min) * (!canBeNegative 
                || !Utility.GetBool() ? 1.0 : -1.0));
    }

    public static float RandomizeNumber(float number, float maxAmount, Random r = null)
    {
      return (float) ((r ?? Utility.random).NextDouble() * (double) maxAmount * 2.0) + number - maxAmount;
    }

    public static float TryGetRandom(float value, Vector2? range)
    {
      return range.HasValue ? Utility.RandomizeNumber(range.Value.X, range.Value.Y) : value;
    }

    public static float StaticRandom(float x, float y = 0.0f)
    {
      return Utility.StaticRandom(new Vector2(x, y));
    }

    public static float StaticRandom(Vector2 pos)
    {
      double d = Math.Sin((double) Vector2.Dot(pos, new Vector2(12.9898f, 78.233f))) * 43758.5453;
      return (float) (d - Math.Truncate(d));
    }

    public static float StaticRandomizeNumber(float number, float maxAmount, float x, float y = 0.0f)
    {
      return (float) ((double) Utility.StaticRandom(x, y) * (double) maxAmount * 2.0) + number - maxAmount;
    }

    public static float StaticTryGetRandom(float value, Vector2? range, float x, float y = 0.0f)
    {
      return range.HasValue ? Utility.StaticRandomizeNumber(range.Value.X, range.Value.Y, x, y) : value;
    }

    public static Vector2 RotateVector(Vector2 vector, float amount)
    {
      double num1 = vector.GetDegrees() + (double) amount;
      if (num1 > 360.0)
        num1 -= 360.0;
      if (num1 < 0.0)
        num1 += 360.0;
      double num2 = 2.0 * Math.PI * (num1 / 360.0);
      double num3 = (double) vector.Length();
      return new Vector2((float) (num3 * Math.Cos(num2)), (float) (num3 * Math.Sin(num2)));
    }

    public static double GetDegrees(this Vector2 vector)
    {
      return Math.Atan2((double) vector.Y, (double) vector.X) * (180.0 / Math.PI);
    }

    public static Vector2 RandomizeVector(Vector2 vector, float maxAmount, Random r = null)
    {
      return Utility.RotateVector(vector, (float) ((r ?? Utility.random).NextDouble() 
          * (double) maxAmount * 2.0) - maxAmount);
    }

    public static void DrawLine(
      SpriteBatch spriteBatch,
      Vector2 start,
      Vector2 end,
      int width = 1,
      Color? color = null,
      float layerDepth = 0.0f,
      int shortenedBy = 0)
    {
      Vector2 vector2 = end - start;
      float num = (float) Math.Atan2((double) vector2.Y, (double) vector2.X);
      spriteBatch.Draw(TextureManager.GetTexture("line"), new Rectangle((int) start.X, (int) start.Y, (int) vector2.Length() - shortenedBy, width), new Rectangle?(), color ?? Color.Green, num, Vector2.Zero, SpriteEffects.None, layerDepth);
    }

    public static void DrawLine(
      SpriteBatch spriteBatch,
      Vector2 start,
      Vector2 dir,
      int length,
      int width = 1,
      Color? color = null,
      float layerDepth = 0.0f)
    {
      float num = (float) Math.Atan2((double) dir.Y, (double) dir.X);
      spriteBatch.Draw(TextureManager.GetTexture("line"), new Rectangle((int) start.X, (int) start.Y, length, width), new Rectangle?(), color ?? Color.Green, num, Vector2.Zero, SpriteEffects.None, layerDepth);
    }

    public static void DrawSquigglyLine(
      SpriteBatch spriteBatch,
      Vector2 start,
      Vector2 end,
      int height = 10,
      Color? color = null,
      float sineTreshold = 0.1f,
      int segmentWidth = 10,
      float layerDepth = 0.0f)
    {
      Vector2 vector2_1 = end - start;
      Vector2 vector2_2 = Vector2.Normalize(vector2_1);
      float num1 = (float) Math.Atan2((double) vector2_2.Y, (double) vector2_2.X);
      Texture2D sineTexture = TextureManager.GetSineTexture(segmentWidth, height, sineTreshold);
      int num2 = (int) ((double) vector2_1.Length() / (double) segmentWidth) + 1;
      for (int index = 0; index < num2; ++index)
      {
        Vector2 vector2_3 = start + vector2_2 * (float) segmentWidth * (float) index;
        Rectangle rectangle = new Rectangle((int) vector2_3.X, (int) vector2_3.Y, segmentWidth, height);
        spriteBatch.Draw(sineTexture, rectangle, new Rectangle?(), !color.HasValue ? Color.Green : color.Value, num1, Vector2.Zero, SpriteEffects.None, layerDepth);
      }
    }

    public static void DrawOutline(
      SpriteBatch spriteBatch,
      Rectangle rectangle,
      int size,
      Color color,
      float layerDepth = 0.0f,
      BorderInfo? enabledBorders = null)
    {
      Utility.DrawOutline(spriteBatch, (RectangleF) rectangle, size, color, layerDepth, enabledBorders);
    }

    public static void DrawOutline(
      SpriteBatch spriteBatch,
      RectangleF rectangle,
      int size,
      Color color,
      float layerDepth = 0.0f,
      BorderInfo? enabledBorders = null)
    {
      BorderInfo borderInfo = !enabledBorders.HasValue || enabledBorders.Equals((object) BorderInfo.None) ? BorderInfo.All : enabledBorders.Value;
      float num = (float) (size / 2);
      RectangleF rectangleF = new RectangleF(rectangle.X + num, rectangle.Y + num, rectangle.Width - (float) size, rectangle.Height - (float) size);
      if (borderInfo.left)
        Utility.DrawLine(spriteBatch, new Vector2(rectangleF.X, rectangleF.Y - (float) size), new Vector2(rectangleF.X, rectangleF.Y + rectangleF.Height + (float) size), size, new Color?(color), layerDepth, 0);
      if (borderInfo.bottom)
        Utility.DrawLine(spriteBatch, new Vector2(rectangleF.X - (float) size, rectangleF.Y + rectangleF.Height), new Vector2(rectangleF.X + rectangleF.Width + (float) size, rectangleF.Y + rectangleF.Height), size, new Color?(color), layerDepth, 0);
      if (borderInfo.right)
        Utility.DrawLine(spriteBatch, new Vector2(rectangleF.X + rectangleF.Width, rectangleF.Y + rectangleF.Height + (float) size), new Vector2(rectangleF.X + rectangleF.Width, rectangleF.Y - (float) size), size, new Color?(color), layerDepth, 0);
      if (!borderInfo.top)
        return;
      Utility.DrawLine(spriteBatch, new Vector2(rectangleF.X + rectangleF.Width + (float) size, rectangleF.Y), new Vector2(rectangleF.X - (float) size, rectangleF.Y), size, new Color?(color), layerDepth, 0);
    }

    private static string GetFullContent(string input)
    {
      string fullContent = "";
      MatchCollection matchCollection = Regex.Matches(input.Trim(), "[^{}%~]+(?![^{]*})");
      for (int index = 0; index < matchCollection.Count; ++index)
        fullContent += ((Capture) matchCollection[index]).Value;
      return fullContent;
    }

        private static int GetFullContentWidth(string input, BitmapFont defaultFont)
        {
            int num = 0;
            for (Match match1 = Regex.Match(input, "%{\\w+}.*?(%|$)"); ((Group)match1).Success; match1 = Regex.Match(input, "%{\\w+}.*?(%|$)"))
            {
                if (((Capture)match1).Index > 0)
                {
                    string text = input.Substring(0, ((Capture)match1).Index);
                    if (text.Length > 0)
                    {
                        var measuredString = defaultFont.MeasureStringHalf(text);
                        num += 24;//(int)measuredString.Width;
                    }
                }
                Match match2 = Regex.Match(input, "(?<=%{)\\w+");
                Match match3 = Regex.Match(input, "(?<=%{\\w+})[^%]+");
                bool flag = num == 0;
                float measureMultiplier;
                Styling specialTextStyling = Utility.GetSpecialTextStyling(((Capture)match2).Value, ((Capture)match3).Value, out Action<string, Rectangle> _, out measureMultiplier, out bool _);
                string measuredSpecialString = "";//specialTextStyling.font.MeasureStringHalf(((Capture)match3).Value);
                num += /*(int)(measuredSpecialString).Width*/24 
                    * (flag ? 1 : 2) * specialTextStyling.margin + (int)(2 * specialTextStyling.padding);
                input = input.Substring(((Capture)match1).Index + ((Capture)match1).Length);
            }
            var measuredRemainingString = defaultFont.MeasureStringHalf(input);
            num += 24;//(int)(measuredRemainingString).Width;
            return num;
        }

    public static void DrawTexture(
      SpriteBatch spriteBatch,
      Simulation simulation,
      Texture2D texture,
      Vector2 position,
      Rectangle? spritePos,
      Color color,
      float layerDepth,
      Vector2 size,
      Vector2 renderScale,
      float rotation = 0.0f,
      bool flip = false,
      float? alphaOverride = null)
    {
      SpriteEffects spriteEffects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
      Vector2 vector2_1 = new Vector2(size.X / 2f, size.Y / 2f);
      Vector2 vector2_2 = (double) rotation != 90.0 && (double) rotation != 270.0 ? new Vector2(position.X + vector2_1.X * renderScale.X, position.Y + vector2_1.Y * renderScale.Y) : new Vector2(position.X + vector2_1.Y * renderScale.Y, position.Y + vector2_1.X * renderScale.X);
      bool flag1 = simulation == null || simulation.RenderDefault;
      bool flag2 = simulation != null && simulation.IsIntro;
      float alpha = alphaOverride.HasValue ? alphaOverride.Value : (flag1 ? 1.0f : 0.05000000074505806f);
      Color color1 = new Color(VarManager.GetBool("phaseIV") ? Color.Yellow : color, alpha);
      if (!(flag1 | flag2))
        color1 = Utility.ChangeColor(color1, -0.4f);
      spriteBatch.Draw(texture, vector2_2, spritePos, color1, (float) ((double) rotation / 360.0 * 6.2831854820251465), vector2_1, renderScale, spriteEffects, layerDepth);
    }

    private static Styling GetSpecialTextStyling(
      string modifier,
      string content1,
      out Action<string, Rectangle> onClick,
      out float measureMultiplier,
      out bool hidden,
      Styling? baseStyle = null)
    {
      Styling specialTextStyling = (baseStyle ?? Styling.Null) with
      {
        margin = 1
      };
      measureMultiplier = 1f;
      hidden = false;
      onClick = (Action<string, Rectangle>) null;
      switch (modifier)
      {
        case "code":
          specialTextStyling.defaultTextColor = Color.Black;
          specialTextStyling.defaultColor = ColorManager.GetColor("white");
          specialTextStyling.hoverColor = Utility.ChangeColor(specialTextStyling.defaultColor, -0.25f);
          specialTextStyling.hoverCursor = new bool?(true);
          //specialTextStyling.font = TextureManager.GetFont("couriermedium");
          specialTextStyling.margin = 8;
          specialTextStyling.padding = 10;
          specialTextStyling.borderColor = ColorManager.GetColor("darkslate");
          specialTextStyling.borderWidth = 4;
          specialTextStyling.centerText = true;
          onClick = (Action<string, Rectangle>) ((content2, rect) => Utility.CreateMovableCommand(content2, new Rectangle?(rect)));
          break;
        case nameof (hidden):
          specialTextStyling.margin = 0;
          specialTextStyling.padding = 0;
          measureMultiplier = 0.0f;
          hidden = true;
          break;
        case "link":
          Color color = ColorManager.GetColor("white");
          specialTextStyling.defaultTextColor = color;
          specialTextStyling.defaultColor = ColorManager.GetColor("darkslate");
          specialTextStyling.hoverColor = ColorManager.GetColor("lightslate");
          specialTextStyling.hoverCursor = new bool?(true);
          specialTextStyling.margin = 8;
          specialTextStyling.padding = 10;
          specialTextStyling.borderColor = color;
          specialTextStyling.borderWidth = 4;
          specialTextStyling.centerText = true;
          onClick = (Action<string, Rectangle>) ((content3, rect) =>
          {
            if (!(content3 == "discord"))
              return;
            Utility.OpenURL("https://discord.gg/a62PN8BBEF");
          });
          break;
        case "orange":
          specialTextStyling.defaultTextColor = ColorManager.GetColor("orange");
          specialTextStyling.margin = 3;
          break;
        case "red":
          specialTextStyling.defaultTextColor = ColorManager.GetColor("red");
          specialTextStyling.margin = 3;
          break;
        case "smallred":
          specialTextStyling.defaultTextColor = ColorManager.GetColor("red");
          //specialTextStyling.font = TextureManager.GetFont("megaman2small");
          break;
        case "space":
          int result;
          specialTextStyling.margin = int.TryParse(content1, out result) ? result : 0;
          specialTextStyling.padding = 0;
          measureMultiplier = 0.0f;
          hidden = true;
          break;
      }
      return specialTextStyling;
    }

    public static void DrawText(
      SpriteBatch spriteBatch,
      BitmapFont font,
      string text,
      Rectangle rectangle,
      Color color,
      bool centerText = true,
      bool rightText = false,
      int textOffset = 0,
      float layerDepth = 0.5f,
      bool useRegex = false)
    {
      text = text.Trim();
      string[] strArray = text.Split('\n');
      if (strArray.Length > 1)
      {
        int height = 24;//(int)(font.MeasureStringHalf("a")).Height;
        int num = rectangle.Y + rectangle.Height / 2 - strArray.Length * height / 2;
        for (int index = 0; index < strArray.Length; ++index)
          Utility.DrawText(spriteBatch, font, strArray[index], new Rectangle(rectangle.X, num + index * height, rectangle.Width, height), color, centerText, rightText, textOffset, layerDepth);
      }
      else
      {
        string text1 = useRegex ? Utility.GetFullContent(text) : text;
        Vector2 vector2_1 = (Vector2) font.MeasureStringHalf(text1);
        int y = rectangle.Y + rectangle.Height / 2 - (int) vector2_1.Y / 2;
        Vector2 vector2_2 = !centerText ? (!rightText ? new Vector2((float) (rectangle.X + Math.Max(0, textOffset)), (float) y) : new Vector2((float) (rectangle.X + rectangle.Width) - vector2_1.X - (float) Math.Max(0, textOffset), (float) y)) : new Vector2((float) (rectangle.X + rectangle.Width / 2) - vector2_1.X / 2f, (float) y);
        Vector2 vector2_3 = vector2_2;
        if (useRegex)
        {
          for (Match match1 = Regex.Match(text, "%{\\w+}.*?(%|$)"); ((Group) match1).Success; match1 = Regex.Match(text, "%{\\w+}.*?(%|$)"))
          {
            if (((Capture) match1).Index > 0)
            {
              string text2 = text.Substring(0, ((Capture) match1).Index);
              if (text2.Length > 0)
              {
                //BitmapFontExtensions.DrawString(spriteBatch, font, text2, vector2_2, color, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, layerDepth, new Rectangle?());
                vector2_2 += new Vector2(/*font.MeasureStringHalf(text2).Width*/24, 0.0f);
              }
            }
            Match match2 = Regex.Match(text, "(?<=%{)\\w+");
            Match match3 = Regex.Match(text, "(?<=%{\\w+})[^%]+");
            Action<string, Rectangle> onClick;
            float measureMultiplier;
            bool hidden;
            Styling specialTextStyling = Utility.GetSpecialTextStyling(((Capture) match2).Value, ((Capture) match3).Value, out onClick, out measureMultiplier, out hidden);
            Vector2 vector2_4 = 
                            new Vector2(/*specialTextStyling.font.MeasureStringHalf(((Capture) match3).Value).Width*/24, 
                            /*font.MeasureStringHalf(((Capture) match3).Value).Height*/24)
                            * measureMultiplier;
            bool flag1 = vector2_2 == vector2_3;
            vector2_4.X += (float) (2 * specialTextStyling.padding);
            vector2_4.Y += (float) (2 * specialTextStyling.padding);

            Rectangle rectangle1 = new Rectangle((int) vector2_2.X 
                + (flag1 ? 0 : specialTextStyling.margin), (int) vector2_2.Y 
                - specialTextStyling.padding, (int) vector2_4.X, (int) vector2_4.Y);
            
                        if (!hidden)
            {
              specialTextStyling.color = specialTextStyling.defaultColor;
              if (Utility.MouseInside(rectangle1, true))
              {
                if (specialTextStyling.hoverColor != new Color() && MovableElement.CurrentMovedObject == null)
                  specialTextStyling.color = specialTextStyling.hoverColor;
                bool? hoverCursor = specialTextStyling.hoverCursor;
                bool flag2 = true;
                if (hoverCursor.GetValueOrDefault() == flag2 & hoverCursor.HasValue)
                  UIElement.SetCursorOverride(MouseCursor.Hand);
                if (BotsAreStupid.Input.MouseJustDowned[0] && onClick != null)
                  onClick(((Capture) match3).Value, rectangle1);
              }

              if (specialTextStyling.color != new Color())
                Utility.DrawRect(spriteBatch, rectangle1, specialTextStyling.color, layerDepth - 0.01f);

              if (specialTextStyling.borderWidth > 0 && specialTextStyling.borderColor != new Color())
                Utility.DrawOutline(spriteBatch, rectangle1, specialTextStyling.borderWidth, 
                    specialTextStyling.borderColor, layerDepth);
              Utility.DrawText(spriteBatch, /*specialTextStyling.font*/default,
                  ((Capture) match3).Value, rectangle1, specialTextStyling.defaultTextColor, 
                  specialTextStyling.centerText, textOffset: specialTextStyling.padding, layerDepth: layerDepth);
            }

            vector2_2 += new Vector2(vector2_4.X + (float) ((flag1 ? 1 : 2) * specialTextStyling.margin), 0.0f);
            text = text.Substring(((Capture) match1).Index + ((Capture) match1).Length);
          }
        }
        //BitmapFontExtensions.DrawString(spriteBatch, font, text, vector2_2, color, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, layerDepth, new Rectangle?());
      }
    }

    public static void DrawText(
      SpriteBatch spriteBatch,
      Vector2 pos,
      Styling style,
      int screenBorderMargin = -1,
      int levelBorderMargin = -1,
      int forcedWidth = -1,
      bool useRegex = false,
      bool expandUpwards = false)
    {
      style.text = style.text.Trim();
      string[] strArray = style.text.Split('\n');
      bool flag = false;
      int num1 = -1;
      int x = -1;
      for (int index = 0; index < strArray.Length; ++index)
      {
                int num2 = useRegex ? Utility.GetFullContentWidth(strArray[index], /*style.font*/default)
                            : 24;//(int) style.font.MeasureStringHalf(strArray[index]).Width;
        if (num2 > x)
        {
          x = num2;
          num1 = index;
        }
      }
      Vector2 vector2 = new Vector2((float) x, /*style.font.MeasureStringHalf("A").Height*/24);
      Rectangle rectangle1 = new Rectangle((int) pos.X + style.margin 
          - (style.centerText ? (int) ((double) vector2.X / 2.0 + (double) style.padding) : 0), (int) pos.Y, forcedWidth != -1 ? forcedWidth : (int) vector2.X + 2 * style.padding, (int) ((double) vector2.Y + (double) style.padding) * strArray.Length + style.padding);
      if (expandUpwards)
        rectangle1.Y -= (int) ((double) rectangle1.Height - (double) vector2.Y - (double) style.padding);
      if (screenBorderMargin >= 0)
      {
        (int, int) windowSize = Game1.GetWindowSize(true);
         rectangle1.X = Math.Max(screenBorderMargin, 
             Math.Min(rectangle1.X, windowSize.Item1 - rectangle1.Width - screenBorderMargin));
        rectangle1.Y = Math.Max(screenBorderMargin, Math.Min(rectangle1.Y, windowSize.Item2 - rectangle1.Height - screenBorderMargin));
      }
      if (levelBorderMargin >= 0)
        rectangle1 = Utility.ClampToRectangle(rectangle1, Utility.LevelConstraints, levelBorderMargin);
      Utility.DrawRect(spriteBatch, rectangle1, style.defaultColor);
      Utility.DrawOutline(spriteBatch, rectangle1, style.borderWidth, style.borderColor);

      for (int index = 0; index < strArray.Length; ++index)
      {
        Rectangle rectangle2 = new Rectangle(rectangle1.X, (int) ((double) (rectangle1.Y + style.padding) 
            + ((double) vector2.Y + (double) style.padding) * (double) index), rectangle1.Width, (int) vector2.Y);
        if (flag)
          Utility.DrawRect(spriteBatch, rectangle2, index == num1 ? Color.Red : Color.HotPink, 0.99f);
        Utility.DrawText(spriteBatch, /*style.font*/default, 
            strArray[index], rectangle2, style.defaultTextColor, false, style.rightText, style.padding, 1f, useRegex);
      }
    }

    public static void DrawRect(
      SpriteBatch spriteBatch,
      Rectangle rectangle,
      Color color,
      float layerDepth = 0.5f)
    {
      Texture2D rectangleTexture = TextureManager.GetRectangleTexture(1, 1, Color.White);
      spriteBatch.Draw(rectangleTexture, new Vector2((float) rectangle.X, (float) rectangle.Y), new Rectangle?(), color, 0.0f, Vector2.Zero, new Vector2((float) rectangle.Width, (float) rectangle.Height), SpriteEffects.None, layerDepth);
    }

    public static void DrawRectCorners(
      SpriteBatch spriteBatch,
      Rectangle rectangle,
      Color color,
      int lineWidth,
      int cornerSize)
    {
      Vector2 vector2_1 = new Vector2((float) cornerSize, 0.0f);
      Vector2 vector2_2 = new Vector2(0.0f, (float) -cornerSize);
      Vector2 start1 = new Vector2((float) rectangle.X, (float) rectangle.Y);
      Utility.DrawLine(spriteBatch, start1, start1 + vector2_1, lineWidth, new Color?(color), 0.0f, 0);
      Utility.DrawLine(spriteBatch, start1, start1 - vector2_2, lineWidth, new Color?(color), 0.0f, 0);
      Vector2 start2 = new Vector2((float) rectangle.X, (float) (rectangle.Y + rectangle.Height));
      Utility.DrawLine(spriteBatch, start2, start2 + vector2_1, lineWidth, new Color?(color), 0.0f, 0);
      Utility.DrawLine(spriteBatch, start2, start2 + vector2_2, lineWidth, new Color?(color), 0.0f, 0);
      Vector2 start3 = new Vector2((float) (rectangle.X + rectangle.Width), (float) (rectangle.Y + rectangle.Height));
      Utility.DrawLine(spriteBatch, start3, start3 - vector2_1, lineWidth, new Color?(color), 0.0f, 0);
      Utility.DrawLine(spriteBatch, start3, start3 + vector2_2, lineWidth, new Color?(color), 0.0f, 0);
      Vector2 start4 = new Vector2((float) (rectangle.X + rectangle.Width), (float) rectangle.Y);
      Utility.DrawLine(spriteBatch, start4, start4 - vector2_1, lineWidth, new Color?(color), 0.0f, 0);
      Utility.DrawLine(spriteBatch, start4, start4 - vector2_2, lineWidth, new Color?(color), 0.0f, 0);
    }

    public static float Interpolate(float pa, float pb, float px)
    {
      float num = (float) (1.0 - Math.Cos((double) (px * 3.14159274f))) * 0.5f;
      return (float) ((double) pa * (1.0 - (double) num) + (double) pb * (double) num);
    }

    public static float Lerp(float from, float to, float t)
    {
      float num = to - from;
      return from + num * t;
    }

    public static float LerpClamped(float from, float to, float t)
    {
        return Utility.Lerp(from, to, MathHelper.Clamp(t, 0.0f, 1f));
    }

    public static float Remap(
      float value,
      float oldMin,
      float oldMax,
      float newMin,
      float newMax)
    {
      float t = (float) (((double) value - (double) oldMin) / ((double) oldMax - (double) oldMin));
      return Utility.Lerp(newMin, newMax, t);
    }

    public static bool GetBool(float max = 0.5f, Random r = null)
    {
      return (r ?? Utility.random).NextDouble() < (double) max;
    }

    public static GameObject GetGameObjectByPoint(
      Simulation simulation,
      Vector2 point,
      Type notType = null)
    {
      GameObject returnGo = (GameObject) null;
      float maxLayerDepth = 0.0f;
      simulation.ForEachGameObject((Action<GameObject>) (g =>
      {
        if (!g.IsActive || !Utility.PointInside(point, g.CollisionRectangle) || notType != (Type) null && !(g.GetType() != notType))
          return;
        float scrambledLayerDepth = g.ScrambledLayerDepth;
        if ((double) scrambledLayerDepth > (double) maxLayerDepth)
        {
          returnGo = g;
          maxLayerDepth = scrambledLayerDepth;
        }
      }));
      return returnGo;
    }

    public static Vector2 GetMousePos(bool inLevelSpace = false, bool inTransformedLevelSpace = false)
    {
      MouseState state = Mouse.GetState();
      Matrix windowMatrix = Game1.Instance.WindowMatrix;
      Vector2 pos = Vector2.Transform(new Vector2((float) state.X, (float) state.Y), Matrix.Invert(windowMatrix));
      if (inTransformedLevelSpace)
        pos = Utility.ScreenToTransformedLevelPos(pos);
      else if (inLevelSpace)
        pos = Utility.ScreenToLevelPos(pos);
      return pos;
    }

    public static float GetZoom(out float t)
    {
      t = Utility.Remap((float) VarManager.GetInt("camzoom"), 0.0f, 1000f, 0.0f, 1f);
      return 1f + Utility.Lerp(0.0f, 2f, t);
    }

    public static Vector2 ScreenToLevelPos(Vector2 pos)
    {
      Matrix matrix = Game1.Instance.WindowMatrix * Matrix.Invert(StateManager.LevelViewMatrix);
      return Vector2.Transform(pos, matrix);
    }

    public static Vector2 LevelToScreenPos(Vector2 pos)
    {
      Matrix matrix = Matrix.Invert(Game1.Instance.WindowMatrix * Matrix.Invert(StateManager.LevelViewMatrix));
      return Vector2.Transform(pos, matrix);
    }

    public static Vector2 ScreenToTransformedLevelPos(Vector2 pos)
    {
      Matrix matrix = Game1.Instance.WindowMatrix * Matrix.Invert(StateManager.LevelViewMatrix) * Matrix.Invert(StateManager.LevelCameraMatrix);
      return Vector2.Transform(pos, matrix);
    }

    public static Vector2 TransformedLevelToScreenPos(Vector2 pos)
    {
      Matrix matrix = Matrix.Invert(Game1.Instance.WindowMatrix * Matrix.Invert(StateManager.LevelViewMatrix * Matrix.Invert(StateManager.LevelCameraMatrix)));
      return Vector2.Transform(pos, matrix);
    }

    public static Vector2 TransformedLevelPosToLevelPos(Vector2 pos)
    {
      return Vector2.Transform(pos, Matrix.Invert(StateManager.LevelCameraMatrix));
    }

    public static Vector2 LevelPosToTransformedLevelPos(Vector2 pos)
    {
      return Vector2.Transform(pos, StateManager.LevelCameraMatrix);
    }

    public static Rectangle LevelToScreenRectangle(Rectangle rect)
    {
      Vector2 pos1 = new Vector2((float) rect.X, (float) rect.Y);
      Vector2 pos2 = pos1 + new Vector2((float) rect.Width, (float) rect.Height);
      Vector2 screenPos1 = Utility.LevelToScreenPos(pos1);
      Vector2 screenPos2 = Utility.LevelToScreenPos(pos2);
      return new Rectangle((int) screenPos1.X, (int) screenPos1.Y, (int) ((double) screenPos2.X - (double) screenPos1.X), (int) ((double) screenPos2.Y - (double) screenPos1.Y));
    }

    public static bool MouseInside(Rectangle rec, bool inLevelSpace = false, bool inTransformedLevelSpace = false)
    {
      return Utility.PointInside(Utility.GetMousePos(inLevelSpace, inTransformedLevelSpace), rec);
    }

    public static Rectangle VectorsToRectangle(Vector2 v1, Vector2 v2)
    {
      int x = (int) Math.Min(v1.X, v2.X);
      int num1 = (int) Math.Max(v1.X, v2.X);
      int y = (int) Math.Min(v1.Y, v2.Y);
      int num2 = (int) Math.Max(v1.Y, v2.Y);
      return new Rectangle(x, y, num1 - x, num2 - y);
    }

    public static bool TryParseRectangle(string s, out Rectangle rect)
    {
      rect = Rectangle.Empty;
      if (string.IsNullOrEmpty(s))
        return false;
      string[] strArray = s.Trim().Split(' ');
      rect = new Rectangle(int.Parse(strArray[0]), int.Parse(strArray[1]), int.Parse(strArray[2]), int.Parse(strArray[3]));
      return true;
    }

    public static string TranslateLevelname(string name)
    {
      name = !(name.Substring(0, 3) != "d3f") ? name.Substring(3) : "Custom/" + name;
      return name;
    }

    public static string SimpleLevelName(string name)
    {
      if (string.IsNullOrEmpty(name))
        return name;
      int num = name.IndexOf('/');
      return num != -1 ? name.Substring(num + 1) : name;
    }

    public static string GetRawLevelName(string path)
    {
      int startIndex = path.Contains("/") ? path.LastIndexOf('/') + 1 : 0;
      int num = path.LastIndexOf('.');
      return path.Substring(startIndex, num - startIndex);
    }

    public static string GetLevelCreatorName(string path)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(path);
      return ((XmlNode) xmlDocument).SelectSingleNode("/level/meta/creator/name")?.InnerText ?? "null";
    }

    public static string GetLevelCreatorUUID(string path)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(path);
      return ((XmlNode) xmlDocument).SelectSingleNode("/level/meta/creator/uuid")?.InnerText ?? "null";
    }

    public static float? GetMinDistToCollision(
      Simulation simulation,
      Vector2 pos,
      Vector2 dir,
      params Type[] additionalTypes)
    {
      if (simulation == null)
        simulation = SimulationManager.MainSimulation;
      Ray ray = new Ray(new Vector3(pos, 0.0f), new Vector3(dir, 0.0f));
      List<float?> nullableList = new List<float?>();
      List<GameObject> gameObjectList = new List<GameObject>((IEnumerable<GameObject>) simulation.GetCollisionObjects());
      if (additionalTypes != null && additionalTypes.Length != 0)
        gameObjectList.AddRange((IEnumerable<GameObject>) simulation.GetAllByType(additionalTypes));
      for (int index = gameObjectList.Count - 1; index >= 0; --index)
      {
        GameObject gameObject = gameObjectList[index];
        if (gameObject.IsActive)
        {
          Rectangle collisionRectangle = gameObject.CollisionRectangle;
          float? nullable = ray.Intersects(new BoundingBox(new Vector3((float) collisionRectangle.X, (float) (collisionRectangle.Y + collisionRectangle.Height), 0.0f), new Vector3((float) (collisionRectangle.X + collisionRectangle.Width), (float) collisionRectangle.Y, 0.0f)));
          if (nullable.HasValue)
            nullableList.Add(nullable);
        }
      }
      if (nullableList.Count <= 0)
        return new float?();
      nullableList.Sort();
      return nullableList[0];
    }

    public static bool AxisOverlap(
      Vector2 axis,
      float minA,
      float maxA,
      float minB,
      float maxB,
      ref float minDistance)
    {
      float num1 = Vector2.Dot(axis, axis);
      if ((double) num1 < 9.99999993922529E-09)
        return true;
      float num2 = maxB - minA;
      float num3 = maxA - minB;
      if ((double) num2 <= 0.0 || (double) num3 <= 0.0)
        return false;
      float num4 = (double) num2 < (double) num3 ? num2 : -num3;
      Vector2 vector2 = axis * (num4 / num1);
      float num5 = Vector2.Dot(vector2, vector2);
      if ((double) num5 < (double) minDistance)
        minDistance = num5;
      return true;
    }

    public static float GetMinAxisOverlap(Rectangle rect1, Rectangle rect2)
    {
      float maxValue = float.MaxValue;
      Utility.AxisOverlap(Vector2.UnitX, (float) rect1.X, (float) (rect1.X + rect1.Width), (float) rect2.X, (float) (rect2.X + rect2.Width), ref maxValue);
      Utility.AxisOverlap(Vector2.UnitY, (float) rect1.Y, (float) (rect1.Y + rect1.Height), (float) rect2.Y, (float) (rect2.Y + rect2.Height), ref maxValue);
      return maxValue;
    }

    public static Vector2[] SphereCast(
      Simulation simulation,
      Vector2 pos,
      int range,
      int rayAmount)
    {
      List<Vector2> vector2List = new List<Vector2>();
      Vector2 vector2 = Utility.RotateVector(Vector2.UnitY, 180f / (float) rayAmount);
      for (int index = 0; index < rayAmount; ++index)
      {
        vector2 = Utility.RotateVector(vector2, 360f / (float) rayAmount);
        float? minDistToCollision = Utility.GetMinDistToCollision(simulation, pos, vector2, typeof (Spike));
        int num1;
        if (minDistToCollision.HasValue)
        {
          float? nullable = minDistToCollision;
          float num2 = (float) range;
          num1 = (double) nullable.GetValueOrDefault() > (double) num2 & nullable.HasValue ? 1 : 0;
        }
        else
          num1 = 1;
        if (num1 != 0)
          vector2List.Add(vector2);
      }
      return vector2List.ToArray();
    }

    public static bool LevelNameValidator(string name, out string errorMessage)
    {
      if (name.Length < 3)
      {
        errorMessage = "Name too short!";
        return false;
      }
      if (name.Length > 20)
      {
        errorMessage = "Name too long!";
        return false;
      }
      if (LevelManager.Exists(name))
      {
        errorMessage = "Already exists!";
        return false;
      }
      if (name.Contains(" "))
      {
        errorMessage = "No spaces allowed!";
        return false;
      }
      if (name.Contains("."))
      {
        errorMessage = "No dots allowed!";
        return false;
      }
      string errorMessage1;
      if (!Utility.CussWordValidator(name, out errorMessage1))
      {
        errorMessage = errorMessage1;
        return false;
      }
      errorMessage = "null";
      return true;
    }

    public static bool NumberValidator(string input, out string errorMessage, int max = 2147483647, int min = 0)
    {
      errorMessage = "null";
      if (VarManager.GetBool("ignorevalidator"))
        return true;
      string oldValue = VarManager.GetString("forceflag");
      bool flag = input.Contains(oldValue);
      input = input.Replace(oldValue, "").Trim();
      int result;
      if (!int.TryParse(input, out result))
      {
        errorMessage = "Invalid Number!";
        return false;
      }
      if (!flag && result < min)
      {
        errorMessage = "Value too small! (" + min.ToString() + " to " + max.ToString() + " possible)";
        return false;
      }
      if (flag || result <= max)
        return true;
      errorMessage = "Value too big! (" + min.ToString() + " to " + max.ToString() + " possible)";
      return false;
    }

    public static bool FloatNumberValidator(
      string input,
      out string errorMessage,
      float max = 3.40282347E+38f,
      float min = 0.0f)
    {
      errorMessage = "null";
      if (VarManager.GetBool("ignorevalidator"))
        return true;
      string oldValue = VarManager.GetString("forceflag");
      bool flag = input.Contains(oldValue);
      input = input.Replace(oldValue, "").Trim();
      float result;
      if (!float.TryParse(input, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        errorMessage = "Invalid Number!";
        return false;
      }
      if (!flag && (double) result < (double) min)
      {
        errorMessage = "Value too small! (" + min.ToString() + " to " + max.ToString() + " possible)";
        return false;
      }
      if (flag || (double) result <= (double) max)
        return true;
      errorMessage = "Value too big! (" + min.ToString() + " to " + max.ToString() + " possible)";
      return false;
    }

    public static Color ChangeColor(Color color, float correctionFactor, float alpha = 1f)
    {
      float r1 = (float) color.R;
      float g1 = (float) color.G;
      float b1 = (float) color.B;
      float r2;
      float g2;
      float b2;
      if ((double) correctionFactor < 0.0)
      {
        correctionFactor = 1f + correctionFactor;
        r2 = r1 * correctionFactor;
        g2 = g1 * correctionFactor;
        b2 = b1 * correctionFactor;
      }
      else
      {
        r2 = ((float) byte.MaxValue - r1) * correctionFactor + r1;
        g2 = ((float) byte.MaxValue - g1) * correctionFactor + g1;
        b2 = ((float) byte.MaxValue - b1) * correctionFactor + b1;
      }
      return new Color((int) r2, (int) g2, (int) b2, (int) ((double) byte.MaxValue * (double) alpha));
    }

    public static string GetObjectName(object obj)
    {
      string objectName = obj.GetType().ToString().Replace("BotsAreStupid.", "");
      bool flag1 = true;
      for (int index = 0; index < objectName.Length; ++index)
      {
        bool flag2 = char.IsUpper(objectName[index]);
        if (flag2 && !flag1)
          objectName = objectName.Insert(index++, " ");
        flag1 = flag2;
      }
      return objectName;
    }

    public static string BreakText(string text, int charsPerLine, out int lineCount)
    {
      lineCount = 1;
      int num1 = charsPerLine;
      string[] strArray = text.Split(' ');
      int startIndex = 0;
      for (int index = 0; index < strArray.Length; ++index)
      {
        int num2 = strArray[index].Length + 1;
        if (startIndex + num2 > num1)
        {
          text = text.Insert(startIndex, "\n");
          ++startIndex;
          ++lineCount;
          num1 = startIndex + num2 + charsPerLine;
        }
        startIndex += num2;
      }
      return text;
    }

    public static async void PlayScore(ScoreData data, string levelName, bool createNewSimulation = false)
    {
      if (string.IsNullOrEmpty(data.script) && data.databaseId != -1)
      {
        ScoreData scoreData = data;
        string str = await ScoreManager.GetScriptById(data.databaseId);
        scoreData.script = str;
        scoreData = (ScoreData) null;
        str = (string) null;
      }
      string fullScript = "// Script by: " + data.playerName + "|// Time: " + data.time.ToString() + "s|// Length: " + data.lineCount.ToString() + " Lines|||" + data.script;
      if (createNewSimulation)
      {
        SimulationManager.CreateSimulation(out bool _, instructions: data.script.Split('|'), playerName: data.playerName, scoreData: data);
        fullScript = (string) null;
      }
      else
      {
        StateManager.TransitionTo(GameState.InLevel_Watch, levelName, data.playerName, allowReEntry: true);
        TextEditor.Instance.LoadInstructions(fullScript.Split('|'));
        fullScript = (string) null;
      }
    }

    public static async Task PlayRandomScore(string levelName = null, bool createNewSimulation = false)
    {
      if (string.IsNullOrEmpty(levelName))
        levelName = LevelManager.CurrentLevelName;
      ScoreData data = await ScoreManager.GetRandom(levelName);
      bool exists = false;
      SimulationManager.ForEachSimulation((SimulationManager.ForEachHandler) (simulation =>
      {
        if (simulation.PlayerName == null || !(simulation.PlayerName == data.playerName))
          return;
        exists = true;
      }));
      if (exists)
        await Utility.PlayRandomScore(levelName, createNewSimulation);
      else
        Utility.PlayScore(data, levelName, createNewSimulation);
    }

    public static bool CussWordValidator(string name, out string errorMessage)
    {
      name = name.ToLower();
      foreach (string cussWord in Utility.cussWords)
      {
        if (name.Contains(cussWord))
        {
          errorMessage = "'" + cussWord + "' is not allowed";
          return false;
        }
      }
      errorMessage = "null";
      return true;
    }

    public static string SecondsToTimeAgo(uint seconds)
    {
      if (seconds < 60U)
        return seconds.ToString() + "s ago";
      if (seconds < 3600U)
        return ((int) (seconds / 60U)).ToString() + "m ago";
      if (seconds < 86400U)
        return ((int) (seconds / 3600U)).ToString() + "h ago";
      if (seconds < 5270400U)
        return ((int) (seconds / 86400U)).ToString() + "d ago";
      if (seconds < 31536000U)
        return ((int) ((double) seconds / 2635200.0)).ToString() + "mo ago";
      return seconds > 31536000U ? ((int) (seconds / 31536000U)).ToString() + "y ago" : "unknown";
    }

    public static void OpenURL(string url)
    {
      try
      {
        Process.Start(url);
      }
      catch
      {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
          url = url.Replace("&", "^&");
          Process.Start(new ProcessStartInfo("cmd", "/c start " + url)
          {
            CreateNoWindow = true
          });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
          Process.Start("xdg-open", url);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
          Process.Start("open", url);
        else
          throw;
      }
    }

    public static void LogError(Exception e)
    {
      string str = VarManager.GetString(VarManager.GetBool("simplelogs") ? "appdirectory" : "appdirectory") + "error.txt";
      File.AppendAllText(str, "Error occurred at " + DateTime.Now.ToString() + "\n");
      File.AppendAllText(str, e.ToString());
      File.AppendAllText(str, "\n" + new string('=', 128) + "\n");
    }

    public static Rectangle Transform(this Rectangle rect, Matrix matrix)
    {
      Vector2 vector2_1 = Vector2.Transform(new Vector2((float) rect.X, (float) rect.Y), matrix);
      Vector2 vector2_2 = Vector2.Transform(new Vector2((float) (rect.X + rect.Width), (float) (rect.Y + rect.Height)), matrix);
      rect.X = (int) vector2_1.X;
      rect.Y = (int) vector2_1.Y;
      rect.Width = (int) ((double) vector2_2.X - (double) vector2_1.X);
      rect.Height = (int) ((double) vector2_2.Y - (double) vector2_1.Y);
      return rect;
    }

    public static Rectangle ClampToRectangle(
      Rectangle rectangle,
      Rectangle constraints,
      int margin = 0)
    {
      (int, int) valueTuple1 = (constraints.X + margin, constraints.Width - margin);
      rectangle.X = rectangle.Width >= valueTuple1.Item2 - valueTuple1.Item1 
                ? MathHelper.Clamp(rectangle.Y, valueTuple1.Item2 - rectangle.Width, valueTuple1.Item1) 
                : MathHelper.Clamp(rectangle.X, valueTuple1.Item1, valueTuple1.Item2 - rectangle.Width);
      (int, int) valueTuple2 = (constraints.Y + margin, constraints.Height - margin);
      rectangle.Y = rectangle.Height >= valueTuple2.Item2 - valueTuple2.Item1
                ? MathHelper.Clamp(rectangle.Y, valueTuple2.Item2 - rectangle.Height, valueTuple2.Item1)
                : MathHelper.Clamp(rectangle.Y, valueTuple2.Item1, valueTuple2.Item2 - rectangle.Height);
      return rectangle;
    }

    public static bool FullyContains(this Rectangle rect, Rectangle check, int margin = 0)
    {
      return check.X >= rect.X + margin && check.X + check.Width <= rect.X + rect.Width - margin && check.Y >= rect.Y + margin && check.Y + check.Height <= rect.Y + rect.Height - margin;
    }

    public static void OpenContextMenu(
      bool isLevelUi,
      UIElement openingElement,
      params (string, Button.OnClick)[] options)
    {
      Utility.OpenContextMenu(isLevelUi, openingElement, new Vector2?(), false, new int?(), false, TextureManager.GetFont("megaMan2Small"), new Rectangle?(), new int?(), options);
    }

    public static void OpenContextMenu(
      bool isLevelUi,
      UIElement openingElement,
      Vector2? position,
      bool centerOnPosition,
      int? positionYMargin,
      bool autoAlignY,
      BitmapFont fontOverride,
      Rectangle? targetRect,
      int? paddingOverride,
      params (string, Button.OnClick)[] options)
    {
      BitmapFont bitmapFont = fontOverride ?? TextureManager.GetFont("megaMan2");
      int num1 = paddingOverride ?? 15;
      int height1 = /*(int) bitmapFont.MeasureStringHalf("a").Height*/24 + 2 * num1;
      int num2 = height1 / 3;
      int num3 = 0;
      int height2 = 0;
      for (int index = 0; index < options.Length; ++index)
      {
        string input = options[index].Item1;
        num3 = Math.Max(num3, Utility.GetFullContentWidth(input, bitmapFont) + 2 * num1);
        height2 += height1 + (input.StartsWith("%") ? num2 : 0) + (input.EndsWith("%") ? num2 : 0);
      }
      Styling optionStyle = Styling.AddTo(Styling.DefaultButtonStyling, new Styling?(new Styling()
      {
        //font = bitmapFont,
        textOffset = num1,
        borderWidth = 2
      })) with
      {
        centerText = false,
        useTextRegex = true
      };
      Styling focussedOptionStyle = optionStyle with
      {
        defaultColor = optionStyle.hoverColor
      };
      Vector2 vector2 = position ?? Utility.GetMousePos(isLevelUi);
      if (centerOnPosition && targetRect.HasValue)
        vector2.X += (float) (targetRect.Value.Width / 2);
      Rectangle rectangle = new Rectangle((int) vector2.X, (int) vector2.Y, num3, height2);
      bool flag = false;
      if (autoAlignY)
      {
        int num4 = positionYMargin.Value;
        (int, int) windowSize = Game1.GetWindowSize();
        if (rectangle.Y + rectangle.Height + 2 * num4 > windowSize.Item2)
        {
          rectangle.Y = rectangle.Y - rectangle.Height - num4;
          flag = true;
        }
        else
          rectangle.Y += num4;
      }
      int num5 = isLevelUi ? 1 : 0;
      UIElement openingElement1 = openingElement;
      Rectangle rect = rectangle;
      int num6 = centerOnPosition ? 1 : 0;
      Rectangle? nullable = targetRect;
      Styling? style = new Styling?();
      Rectangle? targetRect1 = nullable;
      ContextMenu contextMenu = Utility.GetContextMenu(num5 != 0, openingElement1, rect,
          num6 != 0, false, style, targetRect1);
      if (flag)
      {
        for (int index = 0; index < options.Length / 2; ++index)
        {
          (string, Button.OnClick) option = options[index];
          options[index] = options[options.Length - 1 - index];
          options[options.Length - 1 - index] = option;
        }
      }
      int num7 = 0;
      for (int index = 0; index < options.Length; ++index)
      {
        (string, Button.OnClick) option = options[index];
        int y = num7 + (option.Item1.StartsWith("%") ? num2 : 0);
        optionStyle.text = option.Item1;
        contextMenu.AddChild((UIElement) new Button(new Rectangle(0, y, num3, height1), (Button.OnClick) (() =>
        {
          contextMenu.Close();
          Button.OnClick onClick = option.Item2;
          if (onClick == null)
            return;
          onClick();
        }), optionStyle));
        num7 = y + (height1 + (option.Item1.EndsWith("%") ? num2 : 0));
      }
      int currentOption = flag ? options.Length - 1 : 0;
      setCurrentOption(currentOption);
      contextMenu.OnSelectCurrent += new System.Action(invokeCurrent);
      BotsAreStupid.Input.Register(KeyBind.Down, new InputAction.InputHandler(nextOption), ignoreTextLock: true);
      BotsAreStupid.Input.Register(KeyBind.Up, new InputAction.InputHandler(previousOption), ignoreTextLock: true);
      BotsAreStupid.Input.Register(KeyBind.Enter, new InputAction.InputHandler(contextMenu.SelectCurrent), ignoreTextLock: true);
      contextMenu.OnClose += (System.Action) (() =>
      {
        contextMenu.OnSelectCurrent -= new System.Action(invokeCurrent);
        BotsAreStupid.Input.UnRegister(KeyBind.Down, new InputAction.InputHandler(nextOption));
        BotsAreStupid.Input.UnRegister(KeyBind.Up, new InputAction.InputHandler(previousOption));
        BotsAreStupid.Input.UnRegister(KeyBind.Enter, new InputAction.InputHandler(contextMenu.SelectCurrent));
      });

      void nextOption()
      {
        BotsAreStupid.Input.CancelCurrentAction = true;
        setCurrentOption(currentOption + 1);
      }

      void previousOption()
      {
        BotsAreStupid.Input.CancelCurrentAction = true;
        setCurrentOption(currentOption - 1);
      }

      void setCurrentOption(int idx)
      {
        currentOption = (idx + options.Length) % options.Length;
        for (int index = 0; index < contextMenu.Children.Count; ++index)
          contextMenu.Children[index].AddStyle(new Styling()
          {
            defaultColor = index == currentOption ? focussedOptionStyle.defaultColor : optionStyle.defaultColor
          });
      }

      void invokeCurrent()
      {
        BotsAreStupid.Input.CancelCurrentAction = true;
        Button.OnClick onClick = options[MathHelper.Clamp(currentOption, 0, options.Length - 1)].Item2;
        if (onClick != null)
          onClick();
        if (!contextMenu.IsActive)
          return;
        contextMenu.Close();
      }
    }

    public static Rectangle ScaledCenteredRect(
      Rectangle parentRect,
      float scale = 0.5f,
      bool useWidthOnly = false,
      bool useHeightOnly = false)
    {
      int width = (int) ((useHeightOnly ? (double) parentRect.Height : (double) parentRect.Width) * (double) scale);
      int height = (int) ((useWidthOnly ? (double) parentRect.Width : (double) parentRect.Height) * (double) scale);
      return new Rectangle(parentRect.Width / 2 - width / 2, parentRect.Height / 2 - height / 2, width, height);
    }

    public static void OpenNumberContextMenu(
      bool isLevelUi,
      UIElement openingElement,
      Vector2? position,
      Decimal currentValue,
      Utility.ContextValueChangeHandler changeHandler,
      bool centerOnPosition = false,
      bool isBig = false,
      int decimalDigits = 0,
      Rectangle? targetRect = null,
      bool autoAlignY = false,
      int positionYMargin = 0,
      System.Action preChangeHandler = null,
      bool enableOtherButton = false,
      Button.OnClick otherButtonHandler = null)
    {
      BitmapFont bitmapFont = isBig ? TextureManager.GetFont("megaMan2") : TextureManager.GetFont("megaMan2Small");
      int width = isBig ? 480 : 300;
      int height1 = isBig ? 180 : 110;
      Vector2 vector2 = position ?? Utility.GetMousePos(isLevelUi);
      if (centerOnPosition && targetRect.HasValue)
        vector2.X += (float) (targetRect.Value.Width / 2);
      Rectangle rect = new Rectangle((int) vector2.X, (int) vector2.Y, width, height1);
      bool flag = false;
      if (autoAlignY)
      {
        (int, int) windowSize = Game1.GetWindowSize();
        if (rect.Y + rect.Height + 2 * positionYMargin > windowSize.Item2)
        {
          rect.Y = rect.Y - rect.Height - positionYMargin;
          flag = true;
        }
        else
          rect.Y += positionYMargin;
      }
      Styling styling1 = new Styling();
      styling1.defaultColor = ColorManager.GetColor("lightslate");
      styling1.borderColor = ColorManager.GetColor("white");
      styling1.borderWidth = 2;
      Styling styling2 = styling1;
      ContextMenu contextMenu = Utility.GetContextMenu(isLevelUi, openingElement, rect, centerOnPosition, false, new Styling?(styling2), targetRect);
      int padding = isBig ? 30 : 20;
      int height2 = isBig ? 25 : 10;
      Styling buttonStyling = Styling.DefaultButtonStyling with
      {
        //font = bitmapFont
      };
      int stepButtonWidth = (width / 2 - padding - padding / 2) / 3;
      int stepButtonHeight = height1 / 2 - padding;
      decimalDigits = Math.Min(decimalDigits, 2);
      Decimal value = currentValue;
      Decimal toIntConversion = 1M;
      for (int index = 0; index < decimalDigits; ++index)
        toIntConversion *= 10M;
      Decimal precision = 1M / toIntConversion;
      Decimal minValue = 0M;
      Decimal maxValue = 10M * precision;
      for (int index = 0; index < 2; ++index)
        maxValue *= 10M;
      Rectangle rectangle1 = new Rectangle(padding - buttonStyling.borderWidth / 2, height1 - height2 - padding, width - 2 * padding + buttonStyling.borderWidth, height2);
      Slider.SliderChangeHandler changeHandler1 = (Slider.SliderChangeHandler) ((v, f) =>
      {
        value = Math.Round((Decimal) v / toIntConversion, decimalDigits);
        changeHandler(value, f);
      });
      int minValue1 = (int) (minValue * toIntConversion);
      int maxValue1 = (int) (maxValue * toIntConversion);
      styling1 = new Styling();
      styling1.defaultColor = ColorManager.GetColor("white");
      styling1.sliderThumbScale = isBig ? 2f : 1f;
      Styling style1 = styling1;
      int num = (int) (value * toIntConversion);
      System.Action onStartDrag = preChangeHandler;
      Slider slider = new Slider(rectangle1, changeHandler1, minValue1, maxValue1, style1, num, onStartDrag);
      contextMenu.AddChild((UIElement) slider);
      if (enableOtherButton && otherButtonHandler != null)
      {
        int width1 = isBig ? 140 : 80;
        int height3 = isBig ? 50 : 30;
        ContextMenu contextMenu1 = contextMenu;
        Rectangle rectangle2 = new Rectangle(contextMenu.Width / 2 - width1 / 2, flag ? -height3 : contextMenu.Height, width1, height3);
        Button.OnClick onClick = otherButtonHandler;
        Styling style2 = buttonStyling;
        styling1 = new Styling();
        styling1.text = "other ...";
        styling1.borderColor = styling2.borderColor;
        styling1.borderWidth = styling2.borderWidth;
        Styling? newStyleNullable = new Styling?(styling1);
        Styling style3 = Styling.AddTo(style2, newStyleNullable);
        Button child = new Button(rectangle2, onClick, style3);
        contextMenu1.AddChild((UIElement) child);
      }
      createStepButtons(false);
      createStepButtons(true);

      void createStepButtons(bool add)
      {
        int x = add ? width / 2 + padding / 2 : padding;
        for (int index1 = 0; index1 < 3; ++index1)
        {
          Decimal stepSize = precision;
          for (int index2 = 0; index2 < (add ? index1 : 2 - index1); ++index2)
            stepSize *= 10M;
          buttonStyling.text = (add ? ">" : "<") + "\n" + stepSize.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          contextMenu.AddChild((UIElement) new Button(new Rectangle(x, padding, stepButtonWidth, stepButtonHeight), (Button.OnClick) (() =>
          {
            System.Action action = preChangeHandler;
            if (action != null)
              action();
            if (add)
            {
              value += stepSize;
              if (value > maxValue)
                value = maxValue;
            }
            else
            {
              value -= stepSize;
              if (value < minValue)
                value = minValue;
            }
            changeHandler(value, true);
            slider.SetValue((int) (value * toIntConversion));
          }), buttonStyling));
          x += stepButtonWidth;
        }
      }
    }

    private static ContextMenu GetContextMenu(
      bool isLevelUi,
      UIElement openingElement,
      Rectangle rect,
      bool centerOnPosX = true,
      bool centerOnPosY = true,
      Styling? style = null,
      Rectangle? targetRect = null)
    {
      if (centerOnPosX)
        rect.X -= rect.Width / 2;
      if (centerOnPosY)
        rect.Y -= rect.Height / 2;
      Rectangle constraints = isLevelUi ? Utility.LevelConstraints : Utility.WindowConstraints;
      rect = Utility.ClampToRectangle(rect, constraints, 15);
      Styling styling = new Styling()
      {
        isPriority = true,
        isLevelUi = isLevelUi
      };
      return new ContextMenu(openingElement, rect, style.HasValue ? Styling.AddTo(style.Value, new Styling?(styling)) : styling, targetRect);
    }

    public static MovableElement CreateMovableCommand(string content, Rectangle? rectangle = null)
    {
      Styling styling = new Styling()
      {
        text = content,
        //font = TextureManager.GetFont("couriermedium"),
        defaultColor = ColorManager.GetColor("white"),
        defaultTextColor = Color.Black,
        centerText = true,
        borderColor = ColorManager.GetColor("darkslate"),
        borderWidth = 2,
        isLevelUi = true,
        padding = 5
      };
      Rectangle rectangle1 = new Rectangle();
      if (rectangle.HasValue)
      {
        rectangle1 = rectangle.Value;
      }
      else
      {
        SharpDX.Size2 size2 = default;//styling.font.MeasureStringHalf(content);
        rectangle1 = new Rectangle(0, 0, (int) ((double) size2.Width
            + (double) (2 * styling.padding)), (int) ((double) size2.Height + (double) (2 * styling.padding)));
      }
      MovableElement command = new MovableElement(rectangle1, new Styling?(styling));
      command.StartDrag();
      command.CenterOnPosition(Utility.GetMousePos(true));
      command.OnDrop += (MovableElement.ActionHandler) (pos =>
      {
        Vector2 mousePos = Utility.GetMousePos();
        if (Utility.PointInside(mousePos, TextEditor.Instance.GlobalContentRect))
          TextEditor.Instance.AddLine(content, mousePos);
        command.Destroy(true);
      });
      return command;
    }

    public static string IntToRoman(int value)
    {
      if (value <= 3)
        return new string('I', value);
      if (value == 4)
        return "IV";
      if (value == 5)
        return "V";
      throw new NotImplementedException();
    }

    public static string ListShortcuts(params string[] shortcuts)
    {
      string[] array = Enumerable.ToArray<string>(Enumerable.Where<string>((IEnumerable<string>) shortcuts, (Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))));
      string str = "(Shortcut" + (array.Length > 1 ? "s" : "") + ": ";
      for (int index = 0; index < array.Length; ++index)
        str = str + array[index] + (index == array.Length - 1 ? ")" : ", ");
      return str;
    }

    public static Vector2 CalculateRenderScale(
      int width,
      int height,
      Vector2 spriteSize,
      bool fadeIn = false,
      bool fadeAway = false,
      float t = 0.0f,
      float scaleMultiplier = 1f)
    {
      Vector2 renderScale = new Vector2((float) width / spriteSize.X, (float) height / spriteSize.Y) * scaleMultiplier;
      if (!(fadeIn | fadeAway))
        return renderScale;
      if (fadeIn & fadeAway)
        return (double) t < 0.5 ? new Vector2(Utility.Interpolate(0.0f, renderScale.X, t * 2f), Utility.Interpolate(0.0f, renderScale.Y, t * 2f)) : new Vector2(Utility.Interpolate(renderScale.X, 0.0f, (float) (((double) t - 0.5) * 2.0)), Utility.Interpolate(renderScale.Y, 0.0f, (float) (((double) t - 0.5) * 2.0)));
      if (fadeIn)
        return new Vector2(Utility.Interpolate(0.0f, renderScale.X, t), Utility.Interpolate(0.0f, renderScale.Y, t));
      return fadeAway ? new Vector2(Utility.Interpolate(renderScale.X, 0.0f, t), Utility.Interpolate(renderScale.Y, 0.0f, t)) : renderScale;
    }

    public static SharpDX.Size2 MeasureStringHalf(/*this BitmapFont font,*/ string text)
    {
       return default;//font.MeasureString(text) / 2f;
    }

    public static Rectangle GetLineAlignedRectangle(
      Vector2 lineA,
      Vector2 lineB,
      Vector2 size,
      int padding = 20,
      Rectangle? clampToRect = null,
      bool forceInvert = false)
    {
      Vector2 vector2_1 = lineA;
      Vector2 vector2_2 = lineB - lineA;
      Vector2 vector = Vector2.Normalize(vector2_2);
      bool flag = false;
      if ((double) Vector2.Dot(vector, Vector2.UnitY) > 0.0)
      {
        vector2_1 = lineB;
        vector2_2 = lineA - lineB;
        vector = Vector2.Normalize(vector2_2);
        flag = !flag;
      }
      if (forceInvert)
        flag = !flag;
      Vector2 vector2_3 = Utility.RotateVector(vector, -90f);
      float num1 = Vector2.Dot(vector, Vector2.UnitX);
      float num2 = Math.Abs(Vector2.Dot(vector, Vector2.UnitX));
      Vector2 vector2_4 = vector2_1 + vector2_2 / 2f + vector * (float) ((double) size.X / 2.0 * (double) num2 * ((double) num1 < 0.0 ? 1.0 : -1.0) * (flag ? -1.0 : 1.0)) + vector2_3 * (float) padding * (flag ? 1f : -1f);
      vector2_4.Y -= size.Y / 2f;
      if (flag)
        vector2_4.X -= size.X;
      Rectangle check = new Rectangle((int) vector2_4.X, (int) vector2_4.Y, (int) size.X, (int) size.Y);
      return !forceInvert && clampToRect.HasValue && !clampToRect.Value.FullyContains(check, 10) ? Utility.GetLineAlignedRectangle(lineA, lineB, size, padding, forceInvert: true) : check;
    }

    public delegate void ContextValueChangeHandler(Decimal newValue, bool final);
  }
}

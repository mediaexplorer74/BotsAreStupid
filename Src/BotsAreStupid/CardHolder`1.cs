// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.CardHolder`1
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended.BitmapFonts;
using System;

#nullable disable
namespace BotsAreStupid
{
  internal class CardHolder<T> : ContentEditor<T> where T : MovableCard
  {
    private Styling cardStyle;
    private bool lockedCardX;
    private bool isRearrangable;
    private float cardDragDelay;
    private const float holdingScrollArea = 0.1f;
    private const float holdingScrollCooldownMax = 0.1f;
    private float holdingScrollCooldown;
    private const int deleteButtonWidth = 40;
    private int gapPosition = -1;
    private CardHolder<T>.LineDrawModifier lineDrawModifier;

    protected override bool CanScroll => MovableElement.CurrentMovedObject == null;

    public CardHolder(
      Rectangle rectangle,
      GameState gameState,
      ContentEditor<T>.CanInteractCheck canInteractCheck,
      Styling style,
      Styling cardStyle,
      int cardHeight = 40,
      bool lockedCardX = false,
      bool enableLineNumbers = false,
      bool enableDeleteButtons = false,
      bool enableScrollbar = false,
      bool isRearrangable = false,
      float cardDragDelay = 0.0f,
      int minVisibleElements = 1)
      : base(rectangle, gameState, canInteractCheck, enableLineNumbers: enableLineNumbers, enableDeleteButtons: enableDeleteButtons, enableScrollbar: enableScrollbar, style: new Styling?(style))
    {
      this.cardStyle = cardStyle;
      this.elementHeight = cardHeight;
      this.lockedCardX = lockedCardX;
      this.isRearrangable = isRearrangable;
      this.cardDragDelay = cardDragDelay;
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      if ((double) this.holdingScrollCooldown > 0.0)
        this.holdingScrollCooldown -= deltaTime;
      MovableElement currentMovedObject = MovableElement.CurrentMovedObject;
      if (currentMovedObject != null)
      {
        Vector2 pos = currentMovedObject.Center;
        if (currentMovedObject.IsLevelUI)
          pos = Utility.LevelToScreenPos(pos);
        Rectangle globalRect = this.GlobalRect;
        if ((double) this.holdingScrollCooldown <= 0.0)
        {
          int height = (int) ((double) globalRect.Height * 0.10000000149011612);
          if (this.scrollOffsetY > 0 && Utility.PointInside(pos, new Rectangle(globalRect.X, globalRect.Y, globalRect.Width, height)))
          {
            --this.scrollOffsetY;
            this.holdingScrollCooldown = 0.1f;
            if (this.gapPosition >= 0)
              --this.gapPosition;
          }
          else if (this.scrollOffsetY < this.maxScrollOffsetY && Utility.PointInside(pos, new Rectangle(globalRect.X, globalRect.Y + globalRect.Height - height, globalRect.Width, height)))
          {
            ++this.scrollOffsetY;
            this.holdingScrollCooldown = 0.1f;
            if (this.gapPosition >= 0)
              ++this.gapPosition;
          }
        }
        if (this.isRearrangable && Utility.PointInside(pos, globalRect))
        {
          T obj = currentMovedObject as T;
          Rectangle rect = currentMovedObject.GlobalRect;
          if (currentMovedObject.IsLevelUI)
            rect = Utility.LevelToScreenRectangle(rect);
          Vector2 vector2 = new Vector2((float) rect.X, (float) rect.Y);
          if ((object) obj != null && this.content.Contains(obj))
          {
            this.gapPosition = -1;
            int lineFromPos = this.GetLineFromPos(new Vector2?(vector2), true, false, -1);
            if (lineFromPos != obj.Position)
            {
              this.content.Remove(obj);
              this.content.Insert(lineFromPos, obj);
            }
          }
          else
            this.gapPosition = this.GetLineFromPos(new Vector2?(vector2), false, false, -1);
        }
        else if (this.gapPosition >= 0)
          this.gapPosition = -1;
        this.UpdateView(false);
      }
      else
      {
        if (this.gapPosition < 0)
          return;
        this.gapPosition = -1;
      }
    }

    public void SetLineDrawModifier(CardHolder<T>.LineDrawModifier lineDrawModifier)
    {
      this.lineDrawModifier = lineDrawModifier;
    }

    protected override void DrawTexture(SpriteBatch spriteBatch)
    {
      base.DrawTexture(spriteBatch);
      if (this.content.Count == 0)
      {
        Rectangle globalRect = this.GlobalRect;
        MovableElement currentMovedObject = MovableElement.CurrentMovedObject;
        Vector2 pos = currentMovedObject != null ? currentMovedObject.Center : Vector2.Zero;
        if (currentMovedObject != null && currentMovedObject.IsLevelUI)
          pos = Utility.LevelToScreenPos(pos);
        if (currentMovedObject != null && Utility.PointInside(pos, globalRect))
          return;
        Utility.DrawText(spriteBatch, /*this.cardStyle.font*/default, "Drop Commands Here", globalRect, this.cardStyle.defaultTextColor);
        globalRect.X += 30;
        globalRect.Y += 30;
        globalRect.Width -= 60;
        globalRect.Height -= 60;
        Utility.DrawRectCorners(spriteBatch, globalRect, this.cardStyle.borderColor, 4, 40);
      }
      else
      {
        for (int index = 0; index < this.maxVisibleLines; ++index)
        {
          int num = this.scrollOffsetY + index;
          if (num >= 0 && num < this.content.Count)
          {
            T obj = this.content[num];
            Rectangle globalRect = obj.GlobalRect;
            if (this.enableDeleteButtons)
              globalRect.Width -= 40;
            Rectangle positionRect = new Rectangle(globalRect.X, globalRect.Y, obj.positionWidth, globalRect.Height);
            CardHolder<T>.LineDrawModifier lineDrawModifier = this.lineDrawModifier;
            if (lineDrawModifier != null)
              lineDrawModifier(spriteBatch, num, globalRect, positionRect);
          }
        }
      }
    }

    public void DrawUnderline(
      SpriteBatch spriteBatch,
      int line,
      Color color,
      int startChar = 0,
      int endChar = 2147483647)
    {
      if (line >= this.content.Count || line < this.scrollOffsetY || line >= this.scrollOffsetY + this.maxVisibleLines)
        return;
      T obj = this.content[line];
      string content = obj.Content;
      startChar = MathHelper.Clamp(startChar, 0, content.Length);
      endChar = MathHelper.Clamp(endChar, startChar, content.Length);
      //BitmapFont font = obj.Style.font;
      Vector2 vector2_1 = (Vector2)new Vector2();//font.MeasureStringHalf(content.Substring(0, startChar));
      Vector2 vector2_2 = (Vector2)new Vector2();//font.MeasureStringHalf(content.Substring(startChar, endChar - startChar));
      Rectangle globalRect = obj.GlobalRect;
      int x = globalRect.X + obj.Style.textOffset + (int) vector2_1.X;
      int y = globalRect.Y + globalRect.Height / 2 + (int) vector2_2.Y / 2 + 5;
      Vector2 start = new Vector2((float) x, (float) y);
      Vector2 end = new Vector2((float) (x + (int) vector2_2.X), (float) y);
      Utility.DrawSquigglyLine(spriteBatch, start, end, 6, new Color?(color), 0.25f, 8);
    }

    public override object AddLine(string lineContent, Vector2? position = null, bool force = false)
    {
      Color color = ColorManager.GetColor("white");
      T card = (T) Activator.CreateInstance(typeof (T), (object) this, (object) Rectangle.Empty, (object) this.cardStyle, (object) this.enableLineNumbers, (object) this.lockedCardX, (object) this.cardDragDelay, (object) ColorManager.GetColor("lightslate"), (object) color);
      card.SetCanInteractCheck((MovableElement.CanInteractCheck) (basic => this.CanInteract(basic)));
      card.SetContent(lineContent);
      int index = position.HasValue ? this.GetLineFromPos(new Vector2?(position.Value), true, true, -1) : -1;
      if (index >= 0 && index < this.content.Count)
      {
        this.content.Insert(index, card);
      }
      else
      {
        this.content.Add(card);
        if (this.maxVisibleLines > 0 && this.scrollOffsetY < this.content.Count - this.maxVisibleLines)
          ++this.scrollOffsetY;
      }
      this.AddChild((UIElement) card);
      if (this.enableDeleteButtons)
      {
        Styling defaultButtonStyling = Styling.DefaultButtonStyling with
        {
          borderColor = this.cardStyle.borderColor,
          borderWidth = this.cardStyle.borderWidth
        };
        card.AddChild(new Button(Rectangle.Empty, (Button.OnClick) (() =>
        {
          if (!this.CanInteract())
            return;
          this.RemoveCard(card);
        }), defaultButtonStyling).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width - 40, 0, 40, parentRect.Height)), new Rectangle?(card.GetRectangle()))).AddChild(new UIElement(Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (p => Utility.ScaledCenteredRect(p, 0.4f))));
      }
      this.gapPosition = -1;
      this.UpdateView(false);
      return (object) card;
    }

    public void RemoveCard(T card)
    {
      if (!this.content.Remove(card))
        return;
      card.Destroy(true);
      this.UpdateView(false);
    }

    public override void Clear(bool addEmpty = false)
    {
      foreach (T obj in this.content)
        obj.Destroy(true);
      base.Clear(false);
    }

    public override void AddContent(string[] newContent)
    {
      if (newContent == null || newContent.Length == 0)
        return;
      foreach (string lineContent in newContent)
      {
        if (!string.IsNullOrEmpty(lineContent))
          this.AddLine(lineContent, new Vector2?(), false);
      }
    }

    public override string[] GetContent()
    {
      string[] content = new string[this.content.Count];
      for (int index = 0; index < this.content.Count; ++index)
        content[index] = this.content[index].Content;
      return content;
    }

    public override string GetLineString(int index)
    {
      return index < this.content.Count ? this.content[index].Content : (string) null;
    }

    public override Rectangle GetGlobalContentRect()
    {
      Rectangle globalRect = this.GlobalRect;
      return new Rectangle(globalRect.X + this.style.padding, globalRect.Y + this.style.padding, this.AvailableContentWidth, this.AvailableContentHeight);
    }

    public override int GetLineFromPos(
      Vector2? position = null,
      bool clampToCount = true,
      bool rounded = false,
      int minFirstEditableLineOffset = -1)
    {
      if (!rounded)
        return MathHelper.Clamp(((int) (position ?? this.LocalMousePos()).Y - this.GlobalRect.Y - this.ElementPadding + this.elementHeight / 2) / this.PaddedElementHeight + this.scrollOffsetY, 0, Math.Max(0, clampToCount ? this.content.Count - 1 : this.scrollOffsetY + this.maxVisibleLines + 1));
      Vector2? nullable = position;
      Vector2 vector2 = new Vector2(0.0f, (float) (this.PaddedElementHeight / 2));
      return Math.Min(this.GetLineFromPos(nullable.HasValue ? new Vector2?(nullable.GetValueOrDefault() + vector2) : new Vector2?(), false, false, -1), this.content.Count);
    }

    public override void ShowLine(int index, bool loadSaved)
    {
      if (loadSaved)
        index = this.savedScrollOffsetY;
      if (index <= this.scrollOffsetY + this.maxVisibleLines && index >= this.scrollOffsetY)
        return;
      this.scrollOffsetY = Math.Min(index, Math.Max(0, this.content.Count - this.maxVisibleLines));
    }

    public override void UpdateView(bool ignoreContext = false)
    {
      base.UpdateView(false);
      int padding = this.style.padding;
      int width = this.rectangle.Width - 2 * this.style.padding - (!this.enableScrollbar || !this.verticalScroll.SliderActive ? 0 : 15);
      bool flag = false;
      for (int index = 0; index < this.content.Count; ++index)
      {
        if (index == this.gapPosition)
        {
          padding += this.elementHeight + this.style.padding;
          flag = true;
        }
        T obj = this.content[index];
        obj.SetActive(index >= this.scrollOffsetY && index < this.scrollOffsetY + this.maxVisibleLines - (flag ? 1 : 0));
        if (obj.IsActive)
        {
          obj.SetDefaultPosition(new Vector2((float) this.style.padding, (float) padding), MovableElement.CurrentMovedObject == (object) obj);
          obj.SetSize(width, this.elementHeight, true);
          obj.Position = index;
          padding += this.elementHeight + this.style.padding;
        }
      }
    }

    /*public delegate void LineDrawModifier(
      SpriteBatch spriteBatch,
      int line,
      Rectangle lineRect,
      Rectangle positionRect)
      where T : MovableCard;*/

        public delegate void LineDrawModifier(SpriteBatch spriteBatch, int line, 
            Rectangle lineRect, Rectangle positionRect);
    }
}

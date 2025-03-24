// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ContentEditor`1
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal abstract class ContentEditor<T> : UIElement, IContentEditor
  {
    protected List<T> content = new List<T>();
    protected ContentEditor<T>.CanInteractCheck CanInteract;
    protected const float interactionSoundVolume = 0.25f;
    protected const int scrollbarSize = 15;
    protected Scrollbar verticalScroll;
    private int m_ScrollOffsetY;
    protected int savedScrollOffsetY;
    protected int maxVisibleLines;
    protected string allowedChars;
    protected bool enableLineNumbers;
    protected bool enableDeleteButtons;
    protected bool enableScrollbar;
    protected int elementHeight;
    protected int firstEditableLine;
    private int minVisibleElements;

    public int LineCount => this.content.Count;

    public event IContentEditor.ViewUpdateHandler OnViewUpdate;

    public event System.Action OnYScroll;

    protected virtual bool CanScroll => true;

    protected virtual int PaddedElementHeight => this.elementHeight + this.ElementPadding;

    protected virtual int ElementPadding => this.style.padding;

    protected virtual int AvailableContentWidth => this.rectangle.Width - 2 * this.style.padding;

    protected virtual int AvailableContentHeight => this.rectangle.Height - 2 * this.style.padding;

    protected int scrollOffsetY
    {
      get => this.m_ScrollOffsetY;
      set
      {
        if (this.m_ScrollOffsetY == value)
          return;
        this.m_ScrollOffsetY = value;
        System.Action onYscroll = this.OnYScroll;
        if (onYscroll != null)
          onYscroll();
      }
    }

    protected int maxScrollOffsetY => Math.Max(0, this.content.Count - this.minVisibleElements);

    public ContentEditor(
      Rectangle rectangle,
      GameState gameState,
      ContentEditor<T>.CanInteractCheck canInteractCheck,
      string allowedChars = "[def]!/().-",
      bool enableLineNumbers = false,
      bool enableDeleteButtons = false,
      bool enableScrollbar = false,
      int minVisibleElements = 1,
      Styling? style = null)
      : base(rectangle, style)
    {
      this.CanInteract = canInteractCheck;
      this.allowedChars = allowedChars;
      this.enableLineNumbers = enableLineNumbers;
      this.enableDeleteButtons = enableDeleteButtons;
      this.enableScrollbar = enableScrollbar;
      this.minVisibleElements = minVisibleElements;
      this.RegisterInput(gameState);
      this.AddResponsiveAction((UIElement.ResponsiveActionHandler) (p => this.UpdateView(false)));
      ColorManager.OnColorSchemeChange += new Action<ColorScheme>(this.OnColorSchemeChange);
      if (!enableScrollbar)
        return;
      this.AddChild((UIElement) (this.verticalScroll = new Scrollbar(Rectangle.Empty, 0, 0, (Scrollbar.ChangeHandler) (offset =>
      {
        this.scrollOffsetY = offset;
        this.UpdateView(false);
      }))));
      this.verticalScroll.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Rectangle(parentRect.Width - 15, 0, 15, parentRect.Height)));
      this.verticalScroll.SetDisplaySize(this.maxVisibleLines, true);
    }

    protected virtual void RegisterInput(GameState gameState)
    {
      Input.RegisterOnScroll(new Input.MouseScrollHandler(this.OnMouseScroll));
    }

    public virtual void UpdateView(bool ignoreContext = false)
    {
      if (this.content.Count == 0)
        return;
      this.maxVisibleLines = this.AvailableContentHeight / this.PaddedElementHeight;
      this.scrollOffsetY = MathHelper.Clamp(this.scrollOffsetY, 0, Math.Max(0, this.maxScrollOffsetY));
      this.verticalScroll?.SetDisplaySize(this.maxVisibleLines, true);
      this.verticalScroll?.UpdateView(this.content.Count + this.maxVisibleLines - this.minVisibleElements, this.scrollOffsetY);
      IContentEditor.ViewUpdateHandler onViewUpdate = this.OnViewUpdate;
      if (onViewUpdate == null)
        return;
      onViewUpdate();
    }

    public abstract object AddLine(string lineContent, Vector2? position = null, bool force = false);

    public abstract void AddContent(string[] newContent);

    public abstract string[] GetContent();

    public abstract string GetLineString(int index);

    public abstract Rectangle GetGlobalContentRect();

    public void SetFirstEditableLine(int index) => this.firstEditableLine = index;

    public virtual void Clear(bool addEmpty = true)
    {
      this.content.Clear();
      if (addEmpty)
        this.AddLine("", new Vector2?(), false);
      this.savedScrollOffsetY = 0;
      this.scrollOffsetY = 0;
      this.UpdateView(false);
    }

    public virtual void ShowLine(int index, bool loadSaved)
    {
      if (loadSaved)
        index = this.savedScrollOffsetY;
      if (index > this.scrollOffsetY + this.maxVisibleLines - 5 && index + 3 < this.content.Count || index == -1)
      {
        this.scrollOffsetY = Math.Min(Math.Max(0, index - 5), Math.Max(0, this.content.Count - this.maxVisibleLines));
      }
      else
      {
        if (loadSaved || index >= this.scrollOffsetY + 5)
          return;
        this.scrollOffsetY = Math.Max(0, index - this.maxVisibleLines + 5);
      }
    }

    public virtual int GetLineFromPos(
      Vector2? position = null,
      bool clampToCount = true,
      bool rounded = false,
      int minFirstEditableLineOffset = -1)
    {
      int min = minFirstEditableLineOffset == -1 ? 0 : this.firstEditableLine + minFirstEditableLineOffset;
      if (!rounded)
        return MathHelper.Clamp(((int) (position ?? this.LocalMousePos()).Y - this.GlobalRect.Y - this.ElementPadding) / this.PaddedElementHeight + this.scrollOffsetY, min, Math.Max(0, clampToCount ? this.content.Count - 1 : this.scrollOffsetY + this.maxVisibleLines + 1));
      Vector2? nullable = position;
      Vector2 vector2 = new Vector2(0.0f, (float) (this.PaddedElementHeight / 2));
      return MathHelper.Clamp(this.GetLineFromPos(nullable.HasValue ? new Vector2?(nullable.GetValueOrDefault() + vector2) : new Vector2?(), false, false, -1), min, this.content.Count);
    }

    public virtual void SaveScroll() => this.savedScrollOffsetY = this.scrollOffsetY;

    public virtual void LoadScroll() => this.scrollOffsetY = this.savedScrollOffsetY;

    public virtual void OnColorSchemeChange(ColorScheme colorScheme)
    {
      this.style.defaultColor = colorScheme.backgroundColor;
      this.style.color = colorScheme.backgroundColor;
      this.style.defaultTextColor = colorScheme.textColor;
    }

    private void OnMouseScroll(float scrollDiff)
    {
      if (!this.CanScroll || !this.CanInteract(true) || !this.IsCurrentState() || !this.MouseOnRectangle())
        return;
      int scrollOffsetY = this.scrollOffsetY;
      if ((double) scrollDiff < 0.0)
        ++this.scrollOffsetY;
      else if (this.scrollOffsetY > 0)
        --this.scrollOffsetY;
      this.UpdateView(false);
      if (this.scrollOffsetY == scrollOffsetY)
        return;
      this.PlayInteractionSound(0.025f);
    }

    public delegate bool CanInteractCheck(bool basic = false);
  }
}

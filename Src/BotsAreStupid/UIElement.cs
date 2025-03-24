// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.UIElement
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace BotsAreStupid
{
  internal class UIElement
  {
    public static readonly List<UIElement> ElementList = new List<UIElement>();
    public static readonly List<UIElement> LevelElementList = new List<UIElement>();
    protected bool isActive = true;
    private bool isInteractable = true;
    private bool isHovered = false;
    private UIElement.ResponsiveRectangleHandler ResponsiveRectangle;
    protected Rectangle rectangle;
    protected Styling style;
    private Vector2 renderScale = Vector2.One;
    private bool hasRectangleTexture;
    private const float tooltipDelay = 1f;
    private const int roundTextureScaler = 3;
    protected static Vector2 lastMousePos;
    protected static bool mouseHasMoved;
    protected static MouseCursor cursorOverride;
    private Func<string> dynamicTooltip;

    public List<UIElement> Children { get; private set; } = new List<UIElement>();

    public UIElement Parent { get; private set; }

    public bool IsActive
    {
      get => this.isActive && this.Parent != null ? this.Parent.IsActive : this.isActive;
      private set => this.isActive = value;
    }

    public bool IsLevelUI
    {
      get
      {
        if (this.style.isLevelUi)
          return true;
        return this.Parent != null && this.Parent.IsLevelUI;
      }
    }

    public bool IsActiveIgnoreParent => this.isActive;

    public Vector2 Center
    {
      get
      {
        return new Vector2((float) this.GlobalRect.X + (float) this.rectangle.Width / 2f, (float) this.GlobalRect.Y + (float) this.rectangle.Height / 2f);
      }
    }

    public Rectangle GlobalRect
    {
      get
      {
        Rectangle parentRectangle = this.GetParentRectangle();
        return new Rectangle(parentRectangle.X + this.rectangle.X, parentRectangle.Y + this.rectangle.Y, this.rectangle.Width, this.rectangle.Height);
      }
    }

    public int Width => this.rectangle.Width;

    public int Height => this.rectangle.Height;

    public Styling Style => this.style;

    public bool IsInteractable
    {
      get
      {
        return this.isInteractable && this.Parent != null ? this.Parent.IsInteractable : this.isInteractable;
      }
      private set => this.isInteractable = value;
    }

    public bool DrawOnTop { get; set; } = false;

    public bool IsHovered
    {
      get => !this.CheckParentForHover ? this.isHovered : this.Parent.IsHovered;
      protected set => this.isHovered = value;
    }

    public static bool IsAnyHovered { get; private set; }

    public float HoverTime { get; private set; }

    public bool AllowOccludedParentHover { get; set; } = true;

    public bool CheckParentForHover { get; set; } = false;

    private event UIElement.ResponsiveActionHandler ResponsiveAction;

    public UIElement(Rectangle rectangle, Styling? style = null)
    {
      this.rectangle = rectangle;
      this.SetStyle(style ?? new Styling());
      List<UIElement> uiElementList = this.IsLevelUI ? UIElement.LevelElementList : UIElement.ElementList;
      if (this.style.isPriority)
        uiElementList.Insert(0, this);
      else
        uiElementList.Add(this);
      this.IsActive = true;
    }

    public virtual void Initialize()
    {
      this.ForEachChild((UIElement.ForEachHandler) (child => child.UpdateSizeRecursive(this.rectangle)));
      this.ForEachChild((UIElement.ForEachHandler) (child => child.Initialize()));
    }

    public static void InitializeAll()
    {
      UIElement.ForEachElement((UIElement.ForEachHandler) (element =>
      {
        if (element.Parent != null)
          return;
        element.Initialize();
      }));
    }

    public static void UpdateAll(float deltaTime)
    {
      bool hoverAny = false;
      bool hoverAnyText = false;
      bool hoverAnyClickable = false;
      Vector2 mousePos = Utility.GetMousePos();
      UIElement.mouseHasMoved = !(mousePos == UIElement.lastMousePos);
      UIElement.ForEachElement((UIElement.ForEachHandler) (element =>
      {
        element.Update(deltaTime);
        if (!element.IsHovered)
          return;
        hoverAny = true;
        if (element.style.hoverCursor.GetValueOrDefault())
          hoverAnyClickable = true;
        else if (element is TextArea || element is Textfield)
          hoverAnyText = true;
      }));
      UIElement.IsAnyHovered = hoverAny;
      if (MovableElement.CurrentMovedObject != null)
        Mouse.SetCursor(MouseCursor.SizeAll);
      else if (hoverAnyClickable)
        Mouse.SetCursor(MouseCursor.Hand);
      else if (hoverAnyText || StateManager.IsState(GameState.InLevel_AnyWithText) && !SimulationManager.MainSimulation.HasStarted && Utility.MouseInside(TextEditor.Instance.GlobalContentRect))
        Mouse.SetCursor(MouseCursor.IBeam);
      else if (UIElement.cursorOverride != null)
        Mouse.SetCursor(UIElement.cursorOverride);
      else
        Mouse.SetCursor(MouseCursor.Arrow);
      UIElement.lastMousePos = mousePos;
      UIElement.SetCursorOverride((MouseCursor) null);
    }

    public static void DrawAllUI(SpriteBatch spriteBatch)
    {
      UIElement.ForEachElement((UIElement.ForEachHandler) (element =>
      {
        if (element.DrawOnTop || !element.IsActive || !element.IsCurrentState() || element.Parent != null)
          return;
        element.Draw(spriteBatch);
      }), true);
      UIElement.ForEachElement((UIElement.ForEachHandler) (element =>
      {
        if (!element.DrawOnTop || !element.IsActive || !element.IsCurrentState())
          return;
        element.Draw(spriteBatch);
      }), true);
      UIElement.ForEachElement((UIElement.ForEachHandler) (element =>
      {
        if (!element.IsActive || !element.IsCurrentState() || element.Parent != null)
          return;
        element.DrawTooltip(spriteBatch);
      }), true);
    }

    public static void DrawAllLevelUI(SpriteBatch spriteBatch)
    {
      UIElement.ForEachElement((UIElement.ForEachHandler) (element =>
      {
        if (element.DrawOnTop || !element.IsActive || element == PopupMenu.Instance || element == ScoreExplorer.Instance || !element.IsCurrentState() || element.Parent != null)
          return;
        element.Draw(spriteBatch);
      }), levelOnly: true);
      UIElement.ForEachElement((UIElement.ForEachHandler) (element =>
      {
        if (!element.DrawOnTop || !element.IsActive || element == PopupMenu.Instance || element == ScoreExplorer.Instance || !element.IsCurrentState() || element.Parent != null)
          return;
        element.Draw(spriteBatch);
      }), levelOnly: true);
      if (ScoreExplorer.Instance.IsActive)
        ScoreExplorer.Instance.Draw(spriteBatch);
      if (PopupMenu.Instance.IsActive)
        PopupMenu.Instance.Draw(spriteBatch);
      UIElement.ForEachElement((UIElement.ForEachHandler) (element =>
      {
        if (!element.IsActive || !element.IsCurrentState() || element.Parent != null)
          return;
        element.DrawTooltip(spriteBatch);
      }), levelOnly: true);
    }

    public static void ForEachElement(
      UIElement.ForEachHandler handler,
      bool defaultOnly = false,
      bool levelOnly = false)
    {
      if (!levelOnly)
      {
        for (int index = UIElement.ElementList.Count - 1; index >= 0; --index)
          handler(UIElement.ElementList[index]);
      }
      if (defaultOnly)
        return;
      for (int index = UIElement.LevelElementList.Count - 1; index >= 0; --index)
        handler(UIElement.LevelElementList[index]);
    }

    public static void SetCursorOverride(MouseCursor cursor) => UIElement.cursorOverride = cursor;

    public void ForEachChild(UIElement.ForEachHandler handler)
    {
      int num;
      for (int index = this.Children.Count - 1; index >= 0; index = (num = index - 1) >= this.Children.Count ? this.Children.Count - 1 : num)
        handler(this.Children[index]);
    }

    public static bool IsTypeActive(Type type)
    {
      bool isActive = false;
      UIElement.ForEachElement((UIElement.ForEachHandler) (element =>
      {
        if (!element.IsActive || !element.IsCurrentState() || !(element.GetType() == type))
          return;
        isActive = true;
      }));
      return isActive;
    }

    public static UIElement Find(string searchTerm = "", Type type = null, UIElement descendantOf = null)
    {
      UIElement[] all = UIElement.FindAll(searchTerm, type, descendantOf, 1);
      return all.Length != 0 ? all[0] : (UIElement) null;
    }

    public static UIElement[] FindAll(
      string searchTerm = "",
      Type type = null,
      UIElement descendantOf = null,
      int limit = 2147483647)
    {
      List<UIElement> results = new List<UIElement>();
      int found = 0;
      UIElement.ForEachElement((UIElement.ForEachHandler) (element =>
      {
        if (!(found != limit & (type == (Type) null || element.GetType() == type) & (string.IsNullOrEmpty(searchTerm) || !string.IsNullOrEmpty(element.style.text) && element.style.text.Contains(searchTerm)) & (descendantOf == null || descendantOf.HasChild(element))))
          return;
        results.Add(element);
        ++found;
      }));
      return results.ToArray();
    }

    public static List<UIElement> FindChildrenOfType<T>(UIElement current)
    {
      List<UIElement> childrenOfType = new List<UIElement>();
      foreach (UIElement child in current.Children)
      {
        if (child is T)
          childrenOfType.Add(child);
        else
          childrenOfType.AddRange((IEnumerable<UIElement>) UIElement.FindChildrenOfType<T>(child));
      }
      return childrenOfType;
    }

    public virtual void Update(float deltaTime)
    {
      this.CheckHover();
      if (this.IsHovered)
      {
        if (this.style.textColorEffects)
          this.style.textColor = this.style.hoverColor;
        else
          this.style.color = this.style.hoverColor;
      }
      else if (this.style.textColorEffects)
        this.style.textColor = this.style.defaultTextColor;
      else
        this.style.color = this.style.defaultColor;
      if ((double) this.HoverTime > 0.0 && UIElement.mouseHasMoved)
        this.HoverTime = 0.0f;
      if (this.IsHovered && this.IsActive)
      {
        this.HoverTime += deltaTime;
      }
      else
      {
        if ((double) this.HoverTime <= 0.0)
          return;
        this.HoverTime = 0.0f;
      }
    }

    public virtual UIElement AddChild(UIElement child)
    {
      this.Children.Add(child);
      child.Parent = this;
      child.SetDrawStateRecursively(this.style.drawInState);
      return this;
    }

    private void RemoveChild(UIElement child)
    {
      if (!this.Children.Contains(child))
        return;
      child.Parent = (UIElement) null;
      this.Children.Remove(child);
    }

    public void SetDrawStateRecursively(GameState drawInState)
    {
      this.style.drawInState = drawInState;
      this.ForEachChild((UIElement.ForEachHandler) (child => child.SetDrawStateRecursively(drawInState)));
    }

    public void PrintTree(string tab = "")
    {
      Console.WriteLine(tab + this.GetType().ToString() + " " + this.IsActive.ToString());
      this.ForEachChild((UIElement.ForEachHandler) (child => child.PrintTree(tab + "---")));
    }

    public bool HasChild(UIElement element)
    {
      bool wasFound = false;
      this.ForEachChild((UIElement.ForEachHandler) (child =>
      {
        if (child != element && !child.HasChild(element))
          return;
        wasFound = true;
      }));
      return wasFound;
    }

    protected virtual bool CheckHover()
    {
      if (this.IsActive && this.IsInteractable && this.IsCurrentState() && this.MouseOnRectangle() && MovableElement.CurrentMovedObject == null && (!PopupMenu.Instance.IsActive || PopupMenu.Instance.HasChild(this)) && (PopupMenu.Instance.IsActive && PopupMenu.Instance.HasChild(this) || !ScoreExplorer.Instance.IsActive || ScoreExplorer.Instance.HasChild(this)) && !Enumerable.Any<UIElement>((IEnumerable<UIElement>) this.Children, (Func<UIElement, bool>) (child => !child.AllowOccludedParentHover && child.IsHovered)))
      {
        this.IsHovered = true;
        return true;
      }
      this.IsHovered = false;
      return false;
    }

    public void SetStyle(Styling style)
    {
      style.color = style.defaultColor;
      if (style.hoverColor == Color.Transparent)
        style.hoverColor = style.color;
      if (style.clickColor == Color.Transparent)
        style.clickColor = style.hoverColor;
      if (style.defaultTextColor == Color.Transparent)
        style.defaultTextColor = Color.Black;
      style.textColor = style.defaultTextColor;
      if (style.drawInState == GameState.Any)
        style.drawInState = this.style.drawInState;
      //if (style.font == null)
      //  style.font = TextureManager.GetFont("courier");
      this.style = style;
      this.UpdateTexture();
    }

    public void AddStyle(Styling newStyle)
    {
      this.SetStyle(Styling.AddTo(this.style, new Styling?(newStyle)));
    }

    private void UpdateTexture()
    {
      int num;
      if (this.style.texture == null)
      {
        Styling style = this.style;
        num = this.style.spritePos != Rectangle.Empty ? 1 : 0;
      }
      else
        num = 0;
      if (num != 0)
        this.style.texture = TextureManager.GetTexture("uiSheet");
      if (this.style.texture != null)
      {
        Styling style = this.style;
        if (this.style.spritePos != Rectangle.Empty)
        {
          this.renderScale.X = (float) this.rectangle.Width / (float) this.style.spritePos.Width;
          this.renderScale.Y = (float) this.rectangle.Height / (float) this.style.spritePos.Height;
        }
        else
        {
          this.renderScale.X = (float) this.rectangle.Width / (float) this.style.texture.Width;
          this.renderScale.Y = (float) this.rectangle.Height / (float) this.style.texture.Height;
        }
      }
      else if (this.rectangle.Width <= 0 || this.rectangle.Height <= 0)
      {
        this.style.texture = (Texture2D) null;
      }
      else
      {
        if (!(this.style.defaultColor != Color.Transparent))
          return;
        if (this.style.round)
        {
          this.style.texture = TextureManager.GetCircleTexture(this.rectangle.Width * 3);
        }
        else
        {
          this.hasRectangleTexture = true;
          this.style.texture = TextureManager.GetRectangleTexture(1, 1, Color.White);
        }
      }
    }

    public void SetSize(int width, int height, bool recursive = false)
    {
      this.rectangle.Width = width;
      this.rectangle.Height = height;
      this.UpdateTexture();
      if (!recursive)
        return;
      this.ForEachChild((UIElement.ForEachHandler) (child => child.UpdateSizeRecursive(this.rectangle)));
    }

    public UIElement SetResponsiveRectangle(
      UIElement.ResponsiveRectangleHandler responsiveRectangle,
      Rectangle? parentRect = null)
    {
      this.ResponsiveRectangle = responsiveRectangle;
      if (parentRect.HasValue)
        this.UpdateRectangle(parentRect.Value);
      return this;
    }

    public UIElement AddResponsiveAction(UIElement.ResponsiveActionHandler responsiveAction)
    {
      this.ResponsiveAction += responsiveAction;
      return this;
    }

    public void UpdateRectangle(Rectangle parentRect)
    {
      UIElement.ResponsiveRectangleHandler responsiveRectangle = this.ResponsiveRectangle;
      this.rectangle = responsiveRectangle != null ? responsiveRectangle(parentRect) : this.rectangle;
      UIElement.ResponsiveActionHandler responsiveAction = this.ResponsiveAction;
      if (responsiveAction != null)
        responsiveAction(parentRect);
      this.UpdateTexture();
    }

    private void UpdateSizeRecursive(Rectangle parentRect)
    {
      this.UpdateRectangle(parentRect);
      for (int index = this.Children.Count - 1; index >= 0; --index)
        this.Children[index].UpdateSizeRecursive(this.rectangle);
    }

    public void ChangeStyle(Styling newStyle)
    {
      this.style = Styling.AddTo(this.style, new Styling?(newStyle));
    }

    public virtual void SetText(string text)
    {
      this.ChangeStyle(new Styling() { text = text });
    }

    public void SetTooltip(string tooltip)
    {
      this.ChangeStyle(new Styling() { tooltip = tooltip });
    }

    public void SetDynamicTooltip(Func<string> tooltip) => this.dynamicTooltip = tooltip;

    public bool IsCurrentState() => StateManager.IsState(this.style.drawInState);

    public Rectangle GetRectangle() => this.rectangle;

    public void SetInteractable(bool interactable) => this.IsInteractable = interactable;

    public Vector2 LocalMousePos(bool forceLevelSpace = false, bool forceTransformedLevelSpace = false)
    {
      return Utility.GetMousePos(forceLevelSpace || this.IsLevelUI, forceTransformedLevelSpace);
    }

    private Rectangle GetParentRectangle()
    {
      return this.Parent != null ? this.Parent.GlobalRect : new Rectangle(0, 0, 0, 0);
    }

    public void SetBorder(int width, Color? color = null)
    {
      this.style.borderWidth = width;
      if (!color.HasValue)
        return;
      this.style.borderColor = color.Value;
    }

    public UIElement Copy()
    {
      UIElement copy = new UIElement(this.rectangle, new Styling?(this.style));
      this.ForEachChild((UIElement.ForEachHandler) (child => copy.AddChild(child.Copy())));
      return copy;
    }

    public bool MouseOnRectangle()
    {
      Vector2 pos = this.LocalMousePos();
      return this.style.round ? (double) (pos - this.Center).Length() <= (double) this.rectangle.Width / 2.0 : Utility.PointInside(pos, this.GlobalRect);
    }

    protected void PlayInteractionSound(float volume = 0.5f) => SoundManager.Play("click", volume);

    public virtual void Draw(SpriteBatch spriteBatch)
    {
      this.DrawTexture(spriteBatch);
      this.DrawBorder(spriteBatch);
      this.DrawText(spriteBatch);
      this.DrawShadow(spriteBatch);
    }

    public void SetPosition(int? x = null, int? y = null)
    {
      ref Rectangle local1 = ref this.rectangle;
      int? nullable = x;
      int num1 = nullable ?? this.rectangle.X;
      local1.X = num1;
      ref Rectangle local2 = ref this.rectangle;
      nullable = y;
      int num2 = nullable ?? this.rectangle.Y;
      local2.Y = num2;
    }

    public void SetPosition(Vector2 position)
    {
      this.rectangle.X = (int) position.X;
      this.rectangle.Y = (int) position.Y;
    }

    protected virtual void DrawTexture(SpriteBatch spriteBatch)
    {
      Rectangle globalRect = this.GlobalRect;
      if (this.style.texture != null)
      {
        Vector2 vector2_1 = new Vector2(this.style.spritePos.Width, this.style.spritePos.Height);
        //ref Vector2 local = ref vector2_1;
        Styling style1 = this.style;
        //double width = (double) this.style.spritePos.Width;
        Styling style2 = this.style;
        //double height = (double) this.style.spritePos.Height;
        //local = new Vector2((float) width, (float) height);
        Vector2 vector2_2 = vector2_1 / 2f;
        Vector2 vector2_3 = new Vector2((float) globalRect.X, (float) globalRect.Y) + vector2_2 * this.renderScale;
        float num = (float) ((double) this.style.rotation.GetValueOrDefault() / 360.0 * 6.2831854820251465);
        Color color = VarManager.GetBool("phaseIV") ? Color.Yellow : this.style.color;
        if (!this.IsInteractable)
          color = Color.Multiply(color, 0.1f);
        Rectangle rectangle = !this.IsHovered || !this.style.hoverSpritePos.HasValue ? this.style.spritePos : this.style.hoverSpritePos.Value;
        if (rectangle != Rectangle.Empty)
          spriteBatch.Draw(this.style.texture, vector2_3, new Rectangle?(rectangle), color, num, vector2_2, this.renderScale, SpriteEffects.None, 0.0f);
        else if (this.hasRectangleTexture)
          spriteBatch.Draw(this.style.texture, vector2_3, new Rectangle?(), color, num, vector2_2, new Vector2((float) globalRect.Width, (float) globalRect.Height), SpriteEffects.None, 0.0f);
        else
          spriteBatch.Draw(this.style.texture, vector2_3, new Rectangle?(), color, num, vector2_2, this.renderScale, SpriteEffects.None, 0.0f);
      }
      this.ForEachChild((UIElement.ForEachHandler) (child =>
      {
        if (!child.IsActive || child.DrawOnTop)
          return;
        child.Draw(spriteBatch);
      }));
    }

    protected virtual void DrawBorder(SpriteBatch spriteBatch)
    {
      if (this.style.borderWidth > 0)
      {
        Rectangle globalRect = this.GlobalRect;
        Color color = this.style.borderColor;
        if (!this.IsInteractable && !this.style.interactableIgnoreBorder)
          color = Color.Multiply(color, 0.1f);
        if (this.style.round)
        {
          Texture2D circleTexture = TextureManager.GetCircleTexture(globalRect.Width * 3, (int) ((double) (globalRect.Width * 3) / 2.0 - (double) (this.style.borderWidth * 3)));
          spriteBatch.Draw(circleTexture, globalRect, new Rectangle?(), color);
        }
        else
          Utility.DrawOutline(spriteBatch, globalRect, this.style.borderWidth, color, enabledBorders: new BorderInfo?(this.style.enabledBorders));
      }
      this.ForEachChild((UIElement.ForEachHandler) (child =>
      {
        if (!child.IsActive || child.DrawOnTop)
          return;
        child.DrawBorder(spriteBatch);
      }));
    }

    private void DrawTooltip(SpriteBatch spriteBatch)
    {
      if (MovableElement.CurrentMovedObject != null)
        return;
      Func<string> dynamicTooltip = this.dynamicTooltip;
      string str = (dynamicTooltip != null ? dynamicTooltip() : (string) null) ?? this.style.tooltip;
      if (str != null && ((double) this.style.tooltipDelay != 0.0 ? (double) this.HoverTime > (double) this.style.tooltipDelay : (double) this.HoverTime > 1.0))
      {
        Vector2 pos = this.LocalMousePos();
        ColorScheme currentScheme = ColorManager.CurrentScheme;
        Utility.DrawText(spriteBatch, pos, new Styling()
        {
          defaultTextColor = currentScheme.textColor,
          defaultColor = currentScheme.backgroundColor,
          margin = 20,
          borderWidth = 2,
          padding = 10,
          //font = TextureManager.GetFont("courier"),
          text = str,
          borderColor = currentScheme.textColor
        }, 15, useRegex: true);
      }
      this.ForEachChild((UIElement.ForEachHandler) (child =>
      {
        if (!child.IsActive)
          return;
        child.DrawTooltip(spriteBatch);
      }));
    }

    protected virtual void DrawText(SpriteBatch spriteBatch)
    {
      if (this.style.text != null)
        Utility.DrawText(spriteBatch, /*this.style.font*/default, 
            this.style.text, this.GlobalRect, this.IsInteractable ? this.style.textColor : Color.Gray, this.style.centerText, this.style.rightText, this.style.textOffset, useRegex: this.style.useTextRegex);
      this.ForEachChild((UIElement.ForEachHandler) (child =>
      {
        if (!child.IsActive || child.DrawOnTop)
          return;
        child.DrawText(spriteBatch);
      }));
    }

    private void DrawShadow(SpriteBatch spriteBatch)
    {
      if (this.style.shadowSize > 0)
      {
        Rectangle globalRect = this.GlobalRect;
        Color color = new Color(Color.Black, 0.5f);
        int x1 = globalRect.X + this.style.shadowSize;
        int y1 = globalRect.Y + globalRect.Height;
        Utility.DrawLine(spriteBatch, new Vector2((float) x1, (float) y1), new Vector2((float) (x1 + globalRect.Width), (float) y1), this.style.shadowSize, new Color?(color), 0.0f, 0);
        int x2 = globalRect.X + globalRect.Width + this.style.shadowSize;
        int y2 = globalRect.Y + this.style.shadowSize;
        Utility.DrawLine(spriteBatch, new Vector2((float) x2, (float) y2), new Vector2((float) x2, (float) (y2 + globalRect.Height - this.style.shadowSize)), this.style.shadowSize, new Color?(color), 0.0f, 0);
      }
      this.ForEachChild((UIElement.ForEachHandler) (child =>
      {
        if (!child.IsActive || child.DrawOnTop)
          return;
        child.DrawShadow(spriteBatch);
      }));
    }

    public virtual void SetActive(bool active) => this.IsActive = active;

    public void ToggleActive() => this.SetActive(!this.IsActive);

    public virtual void Destroy(bool first = true)
    {
      this.SetActive(false);
      if (first && this.Parent != null)
        this.Parent.RemoveChild(this);
      this.ForEachChild((UIElement.ForEachHandler) (child => child.Destroy(false)));
      if (this.IsLevelUI)
        UIElement.LevelElementList.Remove(this);
      else
        UIElement.ElementList.Remove(this);
    }

    public void ClearChildren()
    {
      this.ForEachChild((UIElement.ForEachHandler) (child => child.Destroy()));
    }

    public delegate Rectangle ResponsiveRectangleHandler(Rectangle parentRect);

    public delegate void ResponsiveActionHandler(Rectangle parentRect);

    public delegate void ForEachHandler(UIElement element);
  }
}

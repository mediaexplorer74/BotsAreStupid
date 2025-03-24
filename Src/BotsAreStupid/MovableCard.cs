// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.MovableCard
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable disable
namespace BotsAreStupid
{
  internal class MovableCard : MovableElement
  {
    private int position;
    private bool drawPosition;
    private Color? positionRectColor;
    private Color? positionTextColor;
    private const int positionPadding = 10;
    protected IContentEditor cardHolder;

    public int Position
    {
      get => this.position;
      set
      {
        this.position = value;
        this.style.textOffset = this.positionStringSize + 30;
      }
    }

    public int positionWidth => this.positionStringSize + 20;

    private string positionString => (this.Position + 1).ToString();

    private int positionStringSize
    {
      get
      {
          return (int)24;//this.style.font.MeasureStringHalf(new string('a', this.positionString.Length)).Width;
      }
    }

    public string Content { get; private set; }

    public MovableCard(
      IContentEditor cardHolder,
      Rectangle rectangle,
      Styling? style,
      bool drawPosition,
      bool xLocked,
      float dragDelay = 0.0f,
      Color? positionRectColor = null,
      Color? positionTextColor = null)
      : base(rectangle, style, dragDelay, xLocked)
    {
      this.cardHolder = cardHolder;
      this.drawPosition = drawPosition;
      this.positionRectColor = positionRectColor;
      this.positionTextColor = positionTextColor;
    }

    protected override void DrawText(SpriteBatch spriteBatch)
    {
      base.DrawText(spriteBatch);
      if (!this.drawPosition)
        return;
      int positionWidth = this.positionWidth;
      Rectangle globalRect = this.GlobalRect;
      Rectangle rectangle = new Rectangle(globalRect.X, globalRect.Y, positionWidth, globalRect.Height);
      if (this.positionRectColor.HasValue)
        Utility.DrawRect(spriteBatch, rectangle, this.positionRectColor.Value);
      Utility.DrawText(spriteBatch, /*this.style.font*/default, this.positionString, rectangle, this.positionTextColor ?? this.style.defaultTextColor);
      Utility.DrawLine(spriteBatch, new Vector2((float) (globalRect.X + positionWidth), (float) globalRect.Y), new Vector2((float) (globalRect.X + positionWidth), (float) (globalRect.Y + globalRect.Height)), this.style.borderWidth, new Color?(this.style.borderColor), 0.0f, 0);
    }

    public virtual void SetContent(string value)
    {
      this.SetText(value);
      this.Content = value;
    }
  }
}

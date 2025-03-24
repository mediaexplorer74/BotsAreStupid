// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Flexbox
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class Flexbox : UIElement
  {
    private Point? center;
    private UIElement header;
    private UIElement footer;
    protected List<UIElement> elements = new List<UIElement>();

    private int headerHeight => this.header == null ? 0 : this.header.Height;

    private int footerHeight => this.footer == null ? 0 : this.footer.Height;

    public Flexbox(Rectangle rectangle, Styling? style, Point? center = null, UIElement header = null)
      : base(rectangle, style)
    {
      this.center = center;
      this.SetHeader(header);
      this.CalculateRectangle();
    }

    public void SetHeader(UIElement header)
    {
      if (header == null)
        return;
      this.header = header;
      this.AddChild(header);
      for (int index = 0; index < this.elements.Count; ++index)
        this.UpdatePosition(this.elements[index], index);
      this.CalculateRectangle();
    }

    public void SetFooter(UIElement footer)
    {
      if (footer == null)
        return;
      this.footer = footer;
      this.AddChild(footer);
      this.CalculateRectangle();
    }

    public void Add(UIElement newChild)
    {
      this.AddChild(newChild);
      this.elements.Add(newChild);
      int index = this.elements.Count - 1;
      this.UpdatePosition(newChild, index);
      this.CalculateRectangle();
    }

    public void Remove(UIElement child)
    {
      bool flag = false;
      for (int index = 0; index < this.elements.Count; ++index)
      {
        UIElement element = this.elements[index];
        if (!flag)
        {
          if (element == child)
          {
            this.elements.Remove(child);
            child.Destroy();
            flag = true;
            --index;
          }
        }
        else
          this.UpdatePosition(element, index);
      }
      this.CalculateRectangle();
    }

    public void Clear()
    {
      for (int index = this.elements.Count - 1; index >= 0; --index)
        this.elements[index].Destroy();
      this.elements.Clear();
      this.CalculateRectangle();
    }

    private void UpdatePosition(UIElement child, int index)
    {
      int num1 = this.headerHeight + this.style.margin;
      for (int index1 = 0; index1 < index; ++index1)
      {
        UIElement element = this.elements[index1];
        if (element.IsActiveIgnoreParent)
          num1 += element.Height + this.style.padding + element.Style.margin;
      }
      int num2 = num1 + (this.style.padding + child.Style.margin);
      child.SetPosition(new int?(this.style.padding), new int?(num2));
    }

    public void CalculateRectangle()
    {
      int num1 = 0;
      int num2 = this.headerHeight + this.style.margin;
      for (int index = 0; index < this.elements.Count; ++index)
      {
        UIElement element = this.elements[index];
        if (element.IsActiveIgnoreParent)
        {
          this.UpdatePosition(element, index);
          if (element.Width > num1)
            num1 = element.Width;
          num2 += this.style.padding + element.Height + element.Style.margin;
        }
      }
      int width = num1 + this.style.padding * 2;
      int num3 = num2 + (this.style.padding + this.style.margin);
      this.footer?.SetPosition(y: new int?(num3));
      int height = num3 + this.footerHeight;
      this.SetSize(width, height);
      if (!this.center.HasValue)
        return;
      Point point = this.center.Value;
      this.SetPosition(new int?(point.X - width / 2), new int?(point.Y - height / 2));
    }
  }
}

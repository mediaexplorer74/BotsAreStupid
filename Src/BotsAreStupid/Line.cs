// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.Line
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System;

#nullable disable
namespace BotsAreStupid
{
  internal class Line
  {
    public const int BlockSize = 3;
    private string text;

    public int Length => this.text.Length + this.BlockCharAmount;

    public int RawLength => this.text.Length;

    public bool IsEmpty => string.IsNullOrEmpty(this.text);

    public int BlockDepth { get; set; }

    public bool InBlockComment { get; set; }

    public Line(string text = "") => this.text = text;

    public void Add(string s, int pos = -1)
    {
      pos = pos == -1 ? this.Length : pos;
      pos = this.RawPos(pos);
      this.text = this.text.Substring(0, pos) + s + this.text.Substring(pos);
    }

    public bool Remove(int pos)
    {
      pos = this.RawPos(pos);
      if (this.Length <= 0)
        return false;
      this.text = this.text.Substring(0, pos) + (pos == this.RawLength ? (string) null : this.text.Substring(pos + 1));
      return true;
    }

    public void Remove(int from, int to)
    {
      from = this.RawPos(from);
      to = this.RawPos(to);
      this.text = this.text.Substring(0, from) + (to < this.RawLength - 1 ? this.text.Substring(to) : "");
    }

    public string GetAt(int pos, int length = 1)
    {
      pos = this.RawPos(pos);
      return this.RawLength > 0 && pos >= 0 && pos < this.RawLength ? this.text.Substring(pos, length) : (string) null;
    }

    public void SetAt(int pos, string s)
    {
      if (this.RawPos(pos) >= this.RawLength)
        return;
      this.Remove(pos);
      this.Add(s, pos);
    }

    public void Insert(int pos, string s) => this.text = this.text.Insert(this.RawPos(pos), s);

    public void SetText(string text) => this.text = text;

    public override string ToString() => new string(' ', this.BlockCharAmount) + this.text;

    public string ToStringRaw() => this.text;

    public string Substring(int start, int end = -1)
    {
      start = this.RawPos(start);
      return end == -1 ? this.text.Substring(start) : this.text.Substring(start, this.RawPos(end));
    }

    public string GetSpan(int start, int end)
    {
      string str = this.ToString();
      if (start >= str.Length)
        return string.Empty;
      string span = str.Substring(start);
      int length = end - start;
      if (length < span.Length)
        span = span.Substring(0, length);
      return span;
    }

    public void Clear() => this.text = "";

    public int BlockCharAmount => 3 * this.BlockDepth;

    private int RawPos(int pos) => Math.Max(pos - this.BlockCharAmount, 0);
  }
}

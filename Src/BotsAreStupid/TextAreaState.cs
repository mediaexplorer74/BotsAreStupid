// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.TextAreaState
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal struct TextAreaState(List<Line> lines, int currentLine, int currentChar)
  {
    public List<Line> lines = lines;
    public int currentLine = currentLine;
    public int currentChar = currentChar;
  }
}

// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.CharacterSet
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using System.Collections.Generic;

#nullable disable
namespace BotsAreStupid
{
  internal class CharacterSet
  {
    private const string allBasicTag = "[def]";
    private const string allLettersTag = "[letters]";
    private const string allNumbersTag = "[numbers]";
    private bool allLetters;
    private bool allNumbers;
    private List<char> allowedChars = new List<char>();

    public CharacterSet(string characters)
    {
      if (characters.Contains("[def]"))
      {
        this.allLetters = true;
        this.allNumbers = true;
        this.allowedChars.Add(' ');
        characters.Replace("[def]", (string) null);
      }
      if (characters.Contains("[letters]"))
      {
        this.allLetters = true;
        characters.Replace("[letters]", (string) null);
      }
      if (characters.Contains("[numbers]"))
      {
        this.allNumbers = true;
        characters.Replace("[numbers]", (string) null);
      }
      foreach (char character in characters)
        this.allowedChars.Add(character);
    }

    public bool Allows(char c)
    {
      if (this.allLetters && c >= 'A' && c <= 'z' || this.allNumbers && char.IsNumber(c))
        return true;
      return c != '[' && c != ']' && this.allowedChars.Contains(c);
    }
  }
}

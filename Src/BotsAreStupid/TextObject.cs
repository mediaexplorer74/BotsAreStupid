// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.TextObject
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;

#nullable disable
namespace BotsAreStupid
{
  internal class TextObject : GameObject
  {
    private Styling style;
    private Vector2 attachedOffset;
    private int rawLetters;
    private string revealedText;
    private int lastRevealedLetters = 0;
    private MatchCollection rawLetterMatches;
    private const string rawLettersPattern = "([^ {}%](?![^{]*}))";
    private const string lineBreakPattern = "(\\*)";
    private const int lineBreakMultiplier = 10;

    public int AttachedToID { get; private set; } = -1;

    public bool ShouldBeDone
    {
      get
      {
        return !this.CurrentSimulation.IsMain ? (double) this.lifeTime >= (double) this.style.textLifeTime : (double) this.lifeTime >= (double) this.style.textRevealTime * 2.0;
      }
    }

    public TextObject(Simulation simulation, float x, float y, Styling? style = null)
      : base(simulation, x, y, 10, 10)
    {
      this.style = Styling.AddTo(new Styling()
      {
        //font = TextureManager.GetFont("megaMan2Small"),
        text = "Test\nWowwwww\nSo cool",
        defaultTextColor = Color.White,
        textLifeTime = 5f,
        textRevealTime = 2.5f,
        centerText = true
      }, new Styling?(style ?? new Styling()));
      this.style.text = this.style.text.Replace('*', '\n');
      this.rawLetters = Regex.Matches(this.style.text, "([^ {}%](?![^{]*}))").Count;
      this.rawLetterMatches = Regex.Matches(this.style.text, "([^ {}%](?![^{]*}))");
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if (string.IsNullOrEmpty(this.revealedText))
        return;
      Utility.DrawText(spriteBatch, this.Center, Styling.AddTo(this.style, new Styling?(new Styling()
      {
        text = this.revealedText,
        padding = 15
      })), levelBorderMargin: 20, useRegex: true, expandUpwards: true);
    }

    public override void Update(float deltaTime)
    {
      base.Update(deltaTime);
      int num1 = (int) Utility.LerpClamped(0.0f, (float) this.rawLetters, (double) this.lifeTime >= (double) this.style.textRevealTime ? 1f : this.lifeTime / this.style.textRevealTime);
      if (num1 != this.lastRevealedLetters)
      {
        int num2 = num1 - 1;
        if (num2 < this.rawLetterMatches.Count)
          this.revealedText = this.style.text.Substring(0, ((Capture) this.rawLetterMatches[num2]).Index + 1);
        this.lastRevealedLetters = num1;
        if (this.revealedText != null)
        {
          char ch = this.revealedText[this.revealedText.Length - 1];
          if (ch != ' ' && ch != '~')
            SoundManager.Play("say", 0.1f);
        }
      }
      if ((double) this.lifeTime > (double) this.style.textLifeTime)
        this.Destroy();
      if (this.AttachedToID == -1 || this.CurrentSimulation == null)
        return;
      GameObject byId = this.CurrentSimulation.GetById(this.AttachedToID);
      if (byId != null)
        this.SetPosition(byId.Position + this.attachedOffset);
    }

    public override GameObject Copy(Simulation intoSimulation = null)
    {
      return (GameObject) new TextObject(intoSimulation ?? this.CurrentSimulation, this.X, this.Y, new Styling?(this.style));
    }

    public void AttachTo(GameObject obj)
    {
      this.AttachedToID = obj.Id;
      this.attachedOffset = this.Position - obj.Position;
    }
  }
}

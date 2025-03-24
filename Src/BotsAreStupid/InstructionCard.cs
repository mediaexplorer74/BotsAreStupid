// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.InstructionCard
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace BotsAreStupid
{
  internal class InstructionCard(
    IContentEditor cardHolder,
    Rectangle rectangle,
    Styling? style,
    bool drawPosition,
    bool xLocked,
    float dragDelay = 0.0f,
    Color? positionRectColor = null,
    Color? positionTextColor = null) : MovableCard(cardHolder, rectangle, style, drawPosition, 
        xLocked, dragDelay, positionRectColor, positionTextColor)
  {
    private List<InstructionCard.ParameterInfo> parameters = new List<InstructionCard.ParameterInfo>();
    private float clickTime = -1f;

    public void SetCommand(ScriptCommand command)
    {
      this.parameters.Add(new InstructionCard.ParameterInfo(command, command.Command));
    }

    public override void SetContent(string value)
    {
      base.SetContent(value);
      if (this.parameters.Count != 0)
        return;
      string[] strArray = this.Content.Split(' ');
      ScriptCommand[] availableCommands = ScriptParser.Instance.AvailableCommands;
      ScriptCommand parameter1 = (ScriptCommand) null;
      foreach (ScriptCommand scriptCommand in availableCommands)
      {
        if (scriptCommand.Command == strArray[0])
        {
          parameter1 = scriptCommand;
          break;
        }
      }
      if (parameter1 == null)
        return;
      this.SetParameter(0, parameter1, parameter1.Command);
      for (int idx = 1; idx < strArray.Length; ++idx)
      {
        ScriptCommand parameter2 = parameter1.FindParameter(strArray[idx]);
        if (parameter2 != null)
        {
          this.SetParameter(idx, parameter2, strArray[idx]);
          parameter1 = parameter2;
        }
      }
    }

    public void OpenContext(int idx = 0)
    {
      int index1 = Math.Min(idx, this.parameters.Count - 1);
      InstructionCard.ParameterInfo currentInfo = this.parameters[index1];
      if (!currentInfo.command.EnableContext)
        return;
      for (; currentInfo.command.Command == "not"; currentInfo = this.parameters[index1])
        --index1;
      if (!(currentInfo.command is ScriptNumber))
        idx++;
      for (int index2 = this.parameters.Count - 1; index2 > idx; --index2)
        this.parameters.RemoveAt(index2);
      this.UpdateContent();
      Rectangle parameterRect = this.GetParameterRect(idx, 10);
      Vector2 vector2 = new Vector2((float) parameterRect.X, (float) (this.GlobalRect.Y + this.rectangle.Height));
      if (currentInfo.command is ScriptNumber)
      {
        ScriptNumber scriptNumber = currentInfo.command as ScriptNumber;
        string s = currentInfo.content;
        if (!string.IsNullOrEmpty(scriptNumber.Suffix))
          s = s.Replace(scriptNumber.Suffix, "");
        Decimal result;
        Decimal currentValue = Decimal.TryParse(s, out result) ? result : 1M;
        this.SetParameter(idx, currentInfo.command, currentValue.ToString() + scriptNumber.Suffix);
        Utility.OpenNumberContextMenu(false, (UIElement) null, new Vector2?(vector2), currentValue,
            (Utility.ContextValueChangeHandler) ((v, f) => this.SetParameter(idx, currentInfo.command, v.ToString("0.##", (IFormatProvider) CultureInfo.InvariantCulture) + scriptNumber.Suffix)), true, true, scriptNumber.Decimals, new Rectangle?(parameterRect));
      }
      else
      {
        List<(string, Button.OnClick)> valueTupleList = new List<(string, Button.OnClick)>();
        ScriptCommand[] parameters = currentInfo.command.Parameters;
        if (parameters != null && parameters.Length != 0)
        {
          foreach (ScriptCommand parameter1 in currentInfo.command.Parameters)
          {
            ScriptCommand parameter = parameter1;
            if (!parameter.HideBluePrint)
              valueTupleList.Add((parameter.ContextName, (Button.OnClick) (() =>
              {
                this.SetParameter(idx, parameter, parameter.Command);
                this.OpenContext(idx);
              })));
          }
        }
        if (valueTupleList.Count > 0)
          Utility.OpenContextMenu(false, (UIElement) null, new Vector2?(vector2), true, new int?(), 
              false, (BitmapFont) null, new Rectangle?(parameterRect), new int?(), valueTupleList.ToArray());
      }
    }

    public override void Update(float delta)
    {
      base.Update(delta);
      if ((double) this.clickTime == -1.0)
        return;
      this.clickTime += delta;
    }

    protected override bool OnMouseDown()
    {
      if (!base.OnMouseDown())
        return false;
      this.clickTime = 0.0f;
      return true;
    }

    protected override void OnMouseUp()
    {
      base.OnMouseUp();
      if ((double) this.clickTime > 0.0 && (double) this.clickTime < 0.15000000596046448)
      {
        Vector2 pos = this.LocalMousePos();
        string[] strArray = this.Content.Split(' ');
        for (int index = 1; index < strArray.Length; ++index)
        {
          Rectangle parameterRect = this.GetParameterRect(index);
          if (Utility.PointInside(pos, parameterRect))
          {
            if (this.parameters[index].command is ScriptNumber)
            {
              this.OpenContext(index);
              return;
            }
            this.OpenContext(index - 1);
            return;
          }
        }
        ScriptError error;
        if (!this.parameters[0].command.CheckLine(this.Content, out error))
        {
          this.OpenContext(error.parameter);
          return;
        }
      }
      this.clickTime = -1f;
    }

    private void UpdateContent()
    {
      this.SetContent(string.Join<string>(" ", 
          Enumerable.Select<InstructionCard.ParameterInfo, string>(
              (IEnumerable<InstructionCard.ParameterInfo>) this.parameters,
              (Func<InstructionCard.ParameterInfo, string>) (p => p.content))));
      this.cardHolder.UpdateView();
    }

    private void SetParameter(int idx, ScriptCommand parameter, string content)
    {
      InstructionCard.ParameterInfo parameterInfo = new InstructionCard.ParameterInfo(parameter, content);
      if (idx < this.parameters.Count)
        this.parameters[idx] = parameterInfo;
      else
        this.parameters.Add(parameterInfo);
      this.UpdateContent();
    }

    private Rectangle GetParameterRect(int idx, int padding = 0)
    {
      //BitmapFont font = this.style.font;
      Rectangle globalRect = this.GlobalRect;
      int textOffset = this.style.textOffset;
      int height = 24;//(int) font.MeasureStringHalf("a").Height
      for (int index = 0; index < idx && index < this.parameters.Count; ++index)
      textOffset += 24;//(int) font.MeasureStringHalf(this.parameters[index].content + " ").Width;
      Rectangle parameterRect = new Rectangle(globalRect.X + textOffset - padding / 2, 
          globalRect.Y + globalRect.Height / 2 - height / 2 - padding, 70, height + 2 * padding);
           
        if (idx < this.parameters.Count)
            parameterRect.Width = 24;//(int) font.MeasureStringHalf(this.parameters[idx].content).Width;

      parameterRect.Width += padding;
      return parameterRect;
    }

    private class ParameterInfo
    {
      public ScriptCommand command;
      public string content;

      public ParameterInfo(ScriptCommand command, string content)
      {
        this.command = command;
        this.content = content;
      }
    }
  }
}

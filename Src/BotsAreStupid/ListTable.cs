// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ListTable
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace BotsAreStupid
{
  internal class ListTable : UIElement
  {
    protected List<IListElement> elements;
    private List<(int, IListElement)> visibleElements;
    protected int rowAmount = 10;
    private const int sortIndicatorWidth = 16;
    private int sortIndicatorHeight = 6;
    private const int sortIndicatorPadding = 5;
    private ColumnInfo[] columnInfos;
    private UIElement[] columnHeaders;
    private Textfield[] columnSearchFields;
    private ListElementAction[] actions;
    private Button[,] buttons;
    private UIElement buttonColumn;
    private string sortedBy;
    private bool sortReversed;
    private Comparison<IListElement> sortOrder;
    private int secondSortParameter;
    private bool secondSortReversed;
    protected int scrollOffset;
    private Scrollbar scrollbar;
    private UIElement listContainer;
    private UIElement emptyListInfo;
    private string emptyListInfoText;
    private int currentSearchedColumnIdx = -1;
    private string currentSearch;
    private bool proportionalButtonSizes;
    private bool scrollbarEnabled;
    private int buttonColumnWidth;
    private Button.OnClick emptyListInfoClickAction;

    public event ListTable.SortChangeHandler OnSortChange;

    public bool IsEmpty => this.elements == null || this.elements.Count == 0;

    public ListTable(
      Microsoft.Xna.Framework.Rectangle rectangle,
      ColumnInfo[] columnInfos,
      ListElementAction[] actions,
      string emptyListInfoText = "Nothing here yet...",
      int secondSortParameter = 0,
      bool secondSortReversed = true,
      int rowAmount = -1,
      bool proportionalButtonSizes = false,
      Styling? style = null,
      int sortIndicatorHeight = -1,
      bool scrollbarEnabled = true,
      Button.OnClick emptyListInfoClickAction = null)
      : base(rectangle, new Styling?(Styling.AddTo(new Styling()
      {
        //font = TextureManager.GetFont("megaMan2"),
        borderWidth = 4
      }, style)))
    {
      this.columnInfos = columnInfos;
      this.actions = actions;
      this.emptyListInfoText = emptyListInfoText;
      this.secondSortParameter = secondSortParameter;
      this.secondSortReversed = secondSortReversed;
      this.proportionalButtonSizes = proportionalButtonSizes;
      this.scrollbarEnabled = scrollbarEnabled;
      this.emptyListInfoClickAction = emptyListInfoClickAction;
      if (sortIndicatorHeight != -1)
        this.sortIndicatorHeight = sortIndicatorHeight;
      if (rowAmount > 0)
        this.rowAmount = rowAmount;
      Input.RegisterOnScroll(new Input.MouseScrollHandler(this.OnMouseScroll), priority: true);
      Input.RegisterOnDown(KeyBind.Enter, new InputAction.InputHandler(this.EndSearch), longPressDelay: 1f, ignoreTextLock: true);
      Input.RegisterMouse(InputEvent.OnDown, (InputAction.InputHandler) (() =>
      {
        if (!this.IsActive || this.currentSearchedColumnIdx < 0 || Utility.MouseInside(this.columnSearchFields[this.currentSearchedColumnIdx].GlobalRect, this.IsLevelUI))
          return;
        this.EndSearch();
      }));
      this.CreateChildren();
    }

    private void CreateChildren()
    {
      Microsoft.Xna.Framework.Color color = ColorManager.GetColor("white");
      ColorManager.GetColor("darkslate");
      this.listContainer = new UIElement(Microsoft.Xna.Framework.Rectangle.Empty).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(0, 0, parentRect.Width, parentRect.Height)));
      Styling styling1;
      if (this.emptyListInfoClickAction != null)
      {
        Microsoft.Xna.Framework.Rectangle empty = Microsoft.Xna.Framework.Rectangle.Empty;
        Button.OnClick listInfoClickAction = this.emptyListInfoClickAction;
        Styling clickableRedTextStyling = Styling.ClickableRedTextStyling;
        styling1 = new Styling();
        //styling1.font = this.style.font;
        styling1.centerText = true;
        styling1.text = this.emptyListInfoText;
        Styling? newStyleNullable = new Styling?(styling1);
        Styling style = Styling.AddTo(clickableRedTextStyling, newStyleNullable);
        this.emptyListInfo = (UIElement) new Button(empty, listInfoClickAction, style);
      }
      else
      {
        Microsoft.Xna.Framework.Rectangle empty = Microsoft.Xna.Framework.Rectangle.Empty;
        styling1 = new Styling();
        //styling1.font = this.style.font;
        styling1.centerText = true;
        styling1.defaultTextColor = color;
        styling1.text = this.emptyListInfoText;
        Styling? style = new Styling?(styling1);
        this.emptyListInfo = new UIElement(empty, style);
      }
      this.emptyListInfo.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) 
          (parentRect => new Microsoft.Xna.Framework.Rectangle(parentRect.Width / 4, 
          parentRect.Height / 4, parentRect.Width / 2, parentRect.Height / 2)));

      //RnD
      //this.listContainer.AddChild(this.buttonColumn = new UIElement(Microsoft.Xna.Framework.Rectangle.Empty)
      //.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler)
      //(parentRect => new Microsoft.Xna.Framework.Rectangle(buttonColumnX(parentRect),
      //rowHeight(parentRect), lastColumnWidth(parentRect), parentRect.Height))));

      int totalButtonChars1 = 0;
      if (this.proportionalButtonSizes)
      {
        foreach (ListElementAction action in this.actions)
          totalButtonChars1 += this.GetActionCharCount(action);
      }
      Styling defaultButtonStyling = Styling.DefaultButtonStyling with
      {
        //font = this.style.font,
        borderWidth = this.style.borderWidth
      };
      /*Size2*/Vector2 charSize = new Vector2();//this.style.font.MeasureStringHalf("A");
      this.buttons = new Button[this.actions.Length, this.rowAmount - 1];
      for (int index1 = 0; index1 < this.buttons.GetLength(0); ++index1)
      {
        string actionString = this.GetActionString(this.actions[index1]);
        int charCount = this.GetActionCharCount(this.actions[index1]);
        Microsoft.Xna.Framework.Rectangle? nullable = new Microsoft.Xna.Framework.Rectangle?();
        if (actionString.StartsWith("ui_"))
        {
          nullable = new Microsoft.Xna.Framework.Rectangle?(TextureManager.GetSpritePos(actionString));
          defaultButtonStyling.text = (string) null;
        }
        else
          defaultButtonStyling.text = actionString;
        for (int index2 = 0; index2 < this.buttons.GetLength(1); ++index2)
        {
          int elementIdx = index2;
          ListElementAction action = this.actions[index1];
          int column = index1;
          int row = index2;
          this.buttons[index1, index2] = new Button(Microsoft.Xna.Framework.Rectangle.Empty, (Button.OnClick) (() => this.HandleButtonClick(action, elementIdx)), defaultButtonStyling);
          //RnD
          //this.buttons[index1, index2].SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(buttonX(parentRect, column), rowHeight(parentRect) * row, buttonWidth(parentRect, column, charCount), rowHeight(parentRect))));
          if (nullable.HasValue)
          {
            Button button = this.buttons[index1, index2];
            Microsoft.Xna.Framework.Rectangle empty = Microsoft.Xna.Framework.Rectangle.Empty;
            styling1 = new Styling();
            styling1.defaultColor = color;
            styling1.spritePos = nullable.Value;
            Styling? style = new Styling?(styling1);

            //UIElement child = new UIElement(empty, style).SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler)
            //    (parentRect => new Microsoft.Xna.Framework.Rectangle(parentRect.Width / 2 
            //    - (int) charSize.Height / 2, parentRect.Height / 2 - (int) charSize.Height / 2, 
            //    (int) charSize.Height, (int) charSize.Height)));

            //button.AddChild(child);
          }
          this.buttonColumn.AddChild((UIElement) this.buttons[index1, index2]);
        }
      }
      this.columnHeaders = new UIElement[this.columnInfos.Length];
      this.columnSearchFields = new Textfield[this.columnInfos.Length];
      Styling styling2 = defaultButtonStyling with
      {
        hoverColor = new Microsoft.Xna.Framework.Color(),
        tooltip = (string) null
      };
      for (int index3 = 0; index3 < this.columnInfos.Length; ++index3)
      {
        ColumnInfo column = this.columnInfos[index3];
        defaultButtonStyling.tooltip = "Column: " + column.name + "\nLeft-click to sort" + (column.isSearchable ? "\nRight-click to search" : "");
        if (index3 > 0 && column.isInteractable)
        {
          defaultButtonStyling.text = column.name;
          this.columnHeaders[index3] = !column.isSearchable ? (UIElement) new Button(Microsoft.Xna.Framework.Rectangle.Empty, (Button.OnClick) (() => this.SortBy(column.name)), defaultButtonStyling) : (UIElement) new Button(Microsoft.Xna.Framework.Rectangle.Empty, (Button.OnClick) (() => this.SortBy(column.name)), (Button.OnClick) (() => this.BeginSearch(column.name)), defaultButtonStyling);
        }
        else
        {
          styling2.text = column.name;
          this.columnHeaders[index3] = new UIElement(Microsoft.Xna.Framework.Rectangle.Empty, new Styling?(styling2));
        }
        int columnIdx = index3;
        //this.columnHeaders[index3].SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(columnX(parentRect, columnIdx), 0, columnWidth(parentRect, columnIdx), rowHeight(parentRect))));
        this.listContainer.AddChild(this.columnHeaders[index3]);
        defaultButtonStyling.tooltip = (string) null;
        if (column.isSearchable)
        {
          UIElement listContainer = this.listContainer;
          Textfield[] columnSearchFields = this.columnSearchFields;
          int index4 = index3;
          Microsoft.Xna.Framework.Rectangle empty = Microsoft.Xna.Framework.Rectangle.Empty;
          styling1 = new Styling();
          styling1.defaultTextColor = color;
          styling1.defaultColor = ColorManager.GetColor("red");
          styling1.textOffset = 8;
          styling1.textOffsetRight = 10;
          styling1.borderWidth = 4;
          styling1.borderColor = color;
          Styling? styling3 = new Styling?(styling1);
          Microsoft.Xna.Framework.Color? cursorColor = new Microsoft.Xna.Framework.Color?(color);
          Textfield textfield1;
          Textfield textfield2 = textfield1 = new Textfield(empty, GameState.Any, styling3, 5, cursorColor, "Search:");
          columnSearchFields[index4] = textfield1;
          Textfield child = textfield2;
          listContainer.AddChild((UIElement) child);
          this.columnSearchFields[index3].SetActive(false);
          //this.columnSearchFields[index3].SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(columnX(parentRect, columnIdx), 0, columnWidth(parentRect, columnIdx), rowHeight(parentRect))));
          this.columnSearchFields[index3].OnValueChange += new Textfield.ValueChangeHandler(this.OnSearchChange);
        }
      }
      if (this.scrollbarEnabled)
      {
        UIElement listContainer = this.listContainer;
        Microsoft.Xna.Framework.Rectangle empty = Microsoft.Xna.Framework.Rectangle.Empty;
        int displaySize = this.rowAmount - 1;
        List<IListElement> elements = this.elements;
        // ISSUE: explicit non-virtual call
        int count = elements != null ? elements.Count : 0;
        Scrollbar.ChangeHandler changeHandler = (Scrollbar.ChangeHandler) (value =>
        {
          this.scrollOffset = value;
          this.UpdateView();
        });
        int num = this.rowAmount - 1;
        Styling? style = new Styling?();
        int minElementsVisible = num;
        Scrollbar child = this.scrollbar = new Scrollbar(empty, displaySize, count, changeHandler, style, minElementsVisible: minElementsVisible);
        listContainer.AddChild((UIElement) child);
        //this.scrollbar.SetResponsiveRectangle((UIElement.ResponsiveRectangleHandler) (parentRect => new Microsoft.Xna.Framework.Rectangle(-30, rowHeight(parentRect), 20, parentRect.Height - rowHeight(parentRect))));
      }
      this.AddChild(this.listContainer);
      this.AddChild(this.emptyListInfo);
      this.UpdateView();
      ListTable listTable1;

      int rowHeight(Microsoft.Xna.Framework.Rectangle parentRect)
      {
        return parentRect.Height / listTable1.rowAmount;
      }
      ListTable listTable2;

      int lastColumnWidth(Microsoft.Xna.Framework.Rectangle parentRect)
      {
        listTable2.buttonColumnWidth = (int) ((double) parentRect.Width * (double) listTable2.columnInfos[listTable2.columnInfos.Length - 1].widthPercentage);
        return listTable2.buttonColumnWidth;
      }

      int buttonColumnX(Microsoft.Xna.Framework.Rectangle parentRect)
      {
        return parentRect.Width - lastColumnWidth(parentRect);
      }
      int totalButtonChars2;

      int buttonWidth(Microsoft.Xna.Framework.Rectangle parentRect, int column, int charCount)
      {
        int num = parentRect.Width / listTable1.actions.Length;
        if (listTable1.proportionalButtonSizes)
          num = (int) ((double) listTable1.buttonColumnWidth * (double) ((float) charCount / (float) totalButtonChars2));
        return num;
      }

      int buttonX(Microsoft.Xna.Framework.Rectangle parentRect, int column)
      {
        if (column == listTable1.buttons.GetLength(0) - 1)
          return parentRect.Width - buttonWidth(parentRect, column, listTable1.GetActionCharCount(listTable1.actions[column]));
        int num = 0;
        for (int column1 = 0; column1 < listTable1.buttons.GetLength(0) && column1 < column; ++column1)
          num += buttonWidth(parentRect, column1, listTable1.GetActionCharCount(listTable1.actions[column1]));
        return num;
      }

      int columnWidth(Microsoft.Xna.Framework.Rectangle parentRect, int column)
      {
        return column != listTable2.columnInfos.Length - 1 ? (int) ((double) parentRect.Width * (double) listTable2.columnInfos[column].widthPercentage) : lastColumnWidth(parentRect);
      }

      int columnX(Microsoft.Xna.Framework.Rectangle parentRect, int column)
      {
        if (column == listTable2.columnInfos.Length - 1)
          return parentRect.Width - columnWidth(parentRect, column);
        int num = 0;
        for (int column1 = 0; column1 < listTable2.columnInfos.Length && column1 < column; ++column1)
          num += columnWidth(parentRect, column1);
        return num;
      }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if (!this.IsActive || this.IsEmpty)
        return;
      base.Draw(spriteBatch);
      Microsoft.Xna.Framework.Color color = ColorManager.GetColor("white");
      ColorManager.GetColor("lightslate");
      ColorManager.GetColor("darkslate");
      ColorManager.GetColor("red");
      Microsoft.Xna.Framework.Rectangle globalRect = this.GlobalRect;
      int height = this.rectangle.Height / this.rowAmount;
      for (int index1 = 0; index1 < this.rowAmount; ++index1)
      {
        Microsoft.Xna.Framework.Rectangle rectangle1 = new Microsoft.Xna.Framework.Rectangle(globalRect.X, globalRect.Y + index1 * height, globalRect.Width, height);
        int num1;
        int num2;
        if (index1 > 0)
        {
          int num3 = index1;
          int? count = this.visibleElements?.Count;
          num1 = this.scrollOffset;
          int? nullable = count.HasValue ? new int?(count.GetValueOrDefault() - num1) : new int?();
          int valueOrDefault = nullable.GetValueOrDefault();
          num2 = num3 <= valueOrDefault & nullable.HasValue ? 1 : 0;
        }
        else
          num2 = 0;
        bool flag = num2 != 0;
        if (flag)
          Utility.DrawOutline(spriteBatch, rectangle1, this.style.borderWidth, color);
        int x = globalRect.X;
        for (int index2 = 0; index2 < this.columnInfos.Length; ++index2)
        {
          int width = index2 == this.columnInfos.Length - 1 ? this.buttonColumnWidth : (int) ((double) this.rectangle.Width * (double) this.columnInfos[index2].widthPercentage);
          if (index1 == 0)
          {
            Microsoft.Xna.Framework.Rectangle rectangle2 = new Microsoft.Xna.Framework.Rectangle(x, globalRect.Y, width, globalRect.Height - globalRect.Height % height);
            if (index2 == this.columnInfos.Length - 1)
              rectangle2.X = globalRect.X + globalRect.Width - rectangle2.Width;
            Utility.DrawOutline(spriteBatch, rectangle2, this.style.borderWidth, color);
            Microsoft.Xna.Framework.Rectangle rectangle3 = new Microsoft.Xna.Framework.Rectangle(x, globalRect.Y, width, height);
            if (index2 != this.currentSearchedColumnIdx && this.sortedBy == this.columnInfos[index2].name.ToLower())
            {
              int num4 = rectangle3.X + rectangle3.Width / 2;
              int y = !this.sortReversed != this.columnInfos[index2].isSortDisplayReversed ? rectangle3.Y + rectangle3.Height - 5 - this.sortIndicatorHeight : rectangle3.Y + 5;
              Utility.DrawLine(spriteBatch, new Vector2((float) (num4 - 8), (float) y), new Vector2((float) (num4 + 8), (float) y), this.sortIndicatorHeight, new Microsoft.Xna.Framework.Color?(color), 0.0f, 0);
            }
          }
          else if (flag && index2 < this.columnInfos.Length - 1)
          {
            (int, IListElement) visibleElement = this.visibleElements[index1 - 1 + this.scrollOffset];
            object seconds = visibleElement.Item2.GetValue(index2 - 1);
            string timeAgo = seconds?.ToString();
            if (seconds?.GetType() == typeof (float))
              timeAgo = ((float) seconds).ToString((IFormatProvider) CultureInfo.InvariantCulture);
            if (seconds?.GetType() == typeof (uint))
              timeAgo = Utility.SecondsToTimeAgo((uint) seconds);
            if (index2 == 0)
            {
              num1 = visibleElement.Item1 + 1;
              timeAgo = num1.ToString();
            }
            string text = timeAgo ?? "null";
            Microsoft.Xna.Framework.Rectangle rectangle4 = new Microsoft.Xna.Framework.Rectangle(x, globalRect.Y + index1 * height, width, height);
            Utility.DrawRect(spriteBatch, rectangle4, new Microsoft.Xna.Framework.Color(Microsoft.Xna.Framework.Color.Black, 0.15f));
            Utility.DrawText(spriteBatch, /*this.style.font*/default, text, rectangle4, color);
          }
          x += width;
        }
      }
    }

    public void SetElements(IListElement[] elements)
    {
      this.elements = elements != null ? new List<IListElement>((IEnumerable<IListElement>) elements) : (List<IListElement>) null;
      this.UpdateView();
      Scrollbar scrollbar = this.scrollbar;
      if (scrollbar == null)
        return;
      List<IListElement> elements1 = this.elements;
      // ISSUE: explicit non-virtual call
      scrollbar.UpdateView(elements1 != null ? elements1.Count : 0, this.scrollOffset);
    }

    public void AddElement(IListElement element)
    {
      if (this.elements == null)
        this.elements = new List<IListElement>();
      this.elements.Add(element);
      this.UpdateView();
      Scrollbar scrollbar = this.scrollbar;
      if (scrollbar == null)
        return;
      List<IListElement> elements = this.elements;
      // ISSUE: explicit non-virtual call
      scrollbar.UpdateView(elements != null ? elements.Count : 0, this.scrollOffset);
    }

    public void RemoveElement(IListElement element)
    {
      if (this.elements == null || this.elements.Count == 0 || !this.elements.Contains(element))
        return;
      this.elements.Remove(element);
      this.UpdateView();
      Scrollbar scrollbar = this.scrollbar;
      if (scrollbar != null)
      {
        List<IListElement> elements = this.elements;
        // ISSUE: explicit non-virtual call
        scrollbar.UpdateView(elements != null ? elements.Count : 0, this.scrollOffset);
      }
    }

    public void RemoveElements(Func<IListElement, bool> predicate)
    {
      foreach (IListElement element in this.GetElements(predicate))
        this.RemoveElement(element);
    }

    public List<IListElement> GetElements(Func<IListElement, bool> predicate)
    {
      return Enumerable.ToList<IListElement>(Enumerable.Where<IListElement>((IEnumerable<IListElement>) this.elements, predicate));
    }

    public void UpdateView()
    {
      if (this.IsEmpty)
      {
        this.emptyListInfo.SetActive(true);
        this.listContainer.SetActive(false);
        this.scrollOffset = 0;
      }
      else
      {
        this.emptyListInfo.SetActive(false);
        this.listContainer.SetActive(true);
        this.elements?.Sort((Comparison<IListElement>) ((a, b) =>
        {
          int num = (this.sortOrder ?? this.GetSortOrder())(a, b);
          return num != 0 ? num : this.CompareValue(a, b, this.secondSortParameter, this.sortReversed ^ this.secondSortReversed);
        }));
        List<(int, IListElement)> valueTupleList = new List<(int, IListElement)>();
        if (!string.IsNullOrEmpty(this.currentSearch))
        {
          string[] strArray = this.currentSearch.Split('%');
          int id = int.Parse(strArray[0]) - 1;
          string lower1 = strArray[1].ToLower();
          int index = 0;
          while (true)
          {
            int num = index;
            int? count = this.elements?.Count;
            int valueOrDefault = count.GetValueOrDefault();
            if (num < valueOrDefault & count.HasValue)
            {
              IListElement element = this.elements[index];
              object obj = element.GetValue(id);
              string lower2 = obj.ToString().ToLower();
              if (obj?.GetType() == typeof (float))
                lower2 = ((float) obj).ToString((IFormatProvider) CultureInfo.InvariantCulture);
              if (lower2.Contains(lower1))
                valueTupleList.Add((index, element));
              ++index;
            }
            else
              break;
          }
        }
        else
        {
          int index = 0;
          while (true)
          {
            int num = index;
            int? count = this.elements?.Count;
            int valueOrDefault = count.GetValueOrDefault();
            if (num < valueOrDefault & count.HasValue)
            {
              valueTupleList.Add((index, this.elements[index]));
              ++index;
            }
            else
              break;
          }
        }
        this.visibleElements = valueTupleList;
        int val2 = this.visibleElements != null ? this.visibleElements.Count + 1 - this.rowAmount : 0;
        if (this.scrollOffset > val2)
          this.scrollOffset = Math.Max(0, val2);
        Scrollbar scrollbar = this.scrollbar;
        if (scrollbar != null)
        {
          List<(int, IListElement)> visibleElements = this.visibleElements;
          // ISSUE: explicit non-virtual call
          scrollbar.UpdateView(visibleElements != null ? visibleElements.Count : 0, this.scrollOffset);
        }
        for (int index1 = 0; index1 < this.buttons.GetLength(0); ++index1)
        {
          for (int index2 = 0; index2 < this.buttons.GetLength(1); ++index2)
          {
            if (this.visibleElements != null && index2 < this.visibleElements.Count - this.scrollOffset)
            {
              this.buttons[index1, index2].SetActive(true);
              IListElement listElement = this.visibleElements[index2 + this.scrollOffset].Item2;
              if (listElement.GetAvailableActions() != null)
              {
                bool interactable = false;
                foreach (ListElementAction availableAction in listElement.GetAvailableActions())
                {
                  if (this.actions[index1].type == availableAction.type)
                    interactable = true;
                }
                this.buttons[index1, index2].SetInteractable(interactable);
              }
              else
                this.buttons[index1, index2].SetInteractable(false);
            }
            else
              this.buttons[index1, index2].SetActive(false);
          }
        }
      }
    }

    public void ResetView()
    {
      this.scrollOffset = 0;
      if (this.visibleElements != null)
        this.scrollbar?.UpdateView(this.visibleElements.Count, this.scrollOffset);
      this.UpdateView();
    }

    public void SetButtonLabels(int columnIdx, string label)
    {
      for (int index = 0; index < this.buttons.GetLength(1); ++index)
        this.buttons[columnIdx, index].SetText(label);
    }

    private void HandleButtonClick(ListElementAction action, int elementIdx)
    {
      elementIdx += this.scrollOffset;
      if (elementIdx >= this.visibleElements.Count)
        return;
      IListElement element = this.visibleElements[elementIdx].Item2;
      ListElementAction.Action action1 = action.action;
      if (action1 != null)
        action1(element);
    }

    public void SortBy(string columnName, bool? descending = null)
    {
      columnName = columnName.ToLower();
      if (columnName == this.sortedBy)
        this.sortReversed = !this.sortReversed;
      if (descending.HasValue)
        this.sortReversed = !descending.Value;
      ListTable.SortChangeHandler onSortChange = this.OnSortChange;
      if (onSortChange != null)
        onSortChange(columnName, this.sortReversed);
      this.sortedBy = columnName;
      this.sortOrder = this.GetSortOrder();
      this.UpdateView();
    }

    //RnD
    private int CompareValue(IListElement l1, IListElement l2, int valueIdx, bool isReversed = false)
    {
      int num1 = isReversed ? 1 : -1;
      float num2 = 0;
      float num3 = 0;
      //if (l1.GetValue<float>(valueIdx, out num2) && l2.GetValue<float>(valueIdx, out num3))
      //  return num2.CompareTo(num3) * num1;
      int num4 = 0;
      int num5 = 0;
      //if (l1.GetValue<int>(valueIdx, out num4) && l2.GetValue<int>(valueIdx, out num5))
      //  return num4.CompareTo(num5) * num1;
      uint num6 = 0;
      uint num7 = 0;
      //if (l1.GetValue<uint>(valueIdx, out num6) && l2.GetValue<uint>(valueIdx, out num7))
      //  return num6.CompareTo(num7) * num1;
      string str = "";
      string strB = "";
      //return l1.GetValue<string>(valueIdx, out str) && l2.GetValue<string>(valueIdx, out strB)
      //          ? str.CompareTo(strB) * num1 : 0;
      return default;
    }

    private Comparison<IListElement> GetSortOrder()
    {
      for (int i = 0; i < this.columnInfos.Length; i++)
      {
        ColumnInfo columnInfo = this.columnInfos[i];
        if (this.sortedBy == "actions")
          return (Comparison<IListElement>) ((l1, l2) => l1.GetAvailableActions().Length.CompareTo(l2.GetAvailableActions().Length) * (this.sortReversed ? -1 : 1));
        if (this.sortedBy == columnInfo.name.ToLower())
          return (Comparison<IListElement>) ((l1, l2) => this.CompareValue(l1, l2, i - 1, this.sortReversed));
      }
      return (Comparison<IListElement>) ((l1, l2) => this.CompareValue(l1, l2, 0, this.sortReversed));
    }

    private void OnMouseScroll(float scrollDiff)
    {
      int num;
      if (this.IsActive && this.IsHovered)
      {
        List<IListElement> elements = this.elements;
        // ISSUE: explicit non-virtual call
        num = elements != null ? (elements.Count > 0 ? 1 : 0) : 0;
      }
      else
        num = 0;
      if (num == 0)
        return;
      int scrollOffset = this.scrollOffset;
      if ((double) scrollDiff < 0.0 && this.scrollOffset <= this.visibleElements.Count - this.rowAmount)
        ++this.scrollOffset;
      else if ((double) scrollDiff > 0.0 && this.scrollOffset > 0)
        --this.scrollOffset;
      if (scrollOffset != this.scrollOffset)
        this.PlayInteractionSound(0.05f);
      this.UpdateView();
      Input.CancelCurrentAction = true;
    }

    private void BeginSearch(string columnName)
    {
      if (this.currentSearchedColumnIdx >= 0)
        this.ToggleSearch(false, this.currentSearchedColumnIdx);
      columnName = columnName.ToLower();
      for (int index = 0; index < this.columnInfos.Length; ++index)
      {
        if (columnName == this.columnInfos[index].name.ToLower())
        {
          this.currentSearchedColumnIdx = index;
          this.ToggleSearch(true, this.currentSearchedColumnIdx);
          this.columnSearchFields[this.currentSearchedColumnIdx].Reset();
          this.scrollOffset = 0;
          this.currentSearch = "";
          this.UpdateView();
        }
      }
      this.UpdateColumnHeaders();
    }

    private void ToggleSearch(bool active, int columnIndex)
    {
      if (columnIndex >= this.columnInfos.Length)
        return;
      this.columnSearchFields[columnIndex].SetActive(active);
      this.columnHeaders[columnIndex].SetActive(!active);
    }

    private void EndSearch()
    {
      if (!this.IsActive || this.currentSearchedColumnIdx < 0)
        return;
      string searchString = this.columnSearchFields[this.currentSearchedColumnIdx].GetValue();
      this.ToggleSearch(false, this.currentSearchedColumnIdx);
      this.UpdateColumnHeaders(searchString);
      this.currentSearch = string.IsNullOrEmpty(searchString) ? (string) null : this.currentSearchedColumnIdx.ToString() + "%" + searchString;
      this.currentSearchedColumnIdx = -1;
      this.UpdateView();
    }

    private void OnSearchChange(string value)
    {
      if (!this.IsActive || this.currentSearchedColumnIdx < 0)
        return;
      this.currentSearch = string.IsNullOrEmpty(value) ? (string) null : this.currentSearchedColumnIdx.ToString() + "%" + value;
      this.UpdateView();
    }

    private void UpdateColumnHeaders(string searchString = "")
    {
      Microsoft.Xna.Framework.Color color1 = ColorManager.GetColor("darkslate");
      Microsoft.Xna.Framework.Color color2 = ColorManager.GetColor("lightslate");
      Microsoft.Xna.Framework.Color color3 = ColorManager.GetColor("red");
      for (int index = 0; index < this.columnInfos.Length - 1; ++index)
      {
        UIElement columnHeader = this.columnHeaders[index];
        Styling style = columnHeader.Style;
        if (index == this.currentSearchedColumnIdx && !string.IsNullOrEmpty(searchString))
        {
          style.defaultColor = color3;
          style.hoverColor = Utility.ChangeColor(color3, -0.2f);
          style.text = searchString + "?";
        }
        else
        {
          style.defaultColor = color1;
          style.hoverColor = color2;
          style.text = this.columnInfos[index].name;
        }
        columnHeader.SetStyle(style);
      }
    }

    private string GetActionString(ListElementAction action)
    {
      switch (action.type)
      {
        case ListElementActionType.Play:
        case ListElementActionType.LoadScript:
          return "ui_hollowstart";
        case ListElementActionType.Delete:
        case ListElementActionType.Remove:
          return "ui_cross";
        case ListElementActionType.More:
          return "ui_more";
        default:
          return action.type.ToString();
      }
    }

    private int GetActionCharCount(ListElementAction action)
    {
      string actionString = this.GetActionString(action);
      return actionString.StartsWith("ui_") ? 2 : actionString.Length;
    }

    public delegate void SortChangeHandler(string column, bool reversed);
  }
}

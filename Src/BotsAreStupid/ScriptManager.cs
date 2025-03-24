// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.ScriptManager
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

using Microsoft.Xna.Framework;
//using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

#nullable disable
namespace BotsAreStupid
{
  internal class ScriptManager
  {
    public static ScriptManager Instance = new ScriptManager();
    private XmlWriterSettings xmlSettings = new XmlWriterSettings()
    {
      Indent = true,
      IndentChars = "\t",
      NewLineChars = "\n"
    };
    public static Flexbox legacyScriptContainer;
    private const int legacyScriptWindowPadding = 25;
    private const int legacyScriptWindowElementWidth = 500;
    private const int legacyScriptWindowElementHeight = 50;

    private string ScriptBasePath => VarManager.GetString("scriptdirectory");

    public ScriptManager()
    {
      Directory.CreateDirectory(this.ScriptBasePath);
      Directory.CreateDirectory(this.ScriptBasePath + "Custom/");
    }

    public static Script[] GetScripts(string levelName, out Script currentScript)
    {
      currentScript = (Script) null;
      string filePath = ScriptManager.Instance.GetFilePath(levelName);
      if (!File.Exists(filePath))
      {
        ScriptManager.Instance.InitFile(filePath);
        return (Script[]) null;
      }
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(filePath);
      XmlElement documentElement = xmlDocument.DocumentElement;
      string innerText1 = ((XmlNode) documentElement).SelectSingleNode("//current").InnerText;
      XmlNodeList xmlNodeList = ((XmlNode) documentElement).SelectNodes("//list/script");
      List<Script> scriptList = new List<Script>();
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        Script script = new Script();
        string innerText2 = ((XmlNode) xmlNode["instructions"]).InnerText;
        script.name = ((XmlNode) xmlNode["name"]).InnerText;
        script.lastSaved = DateTime.Parse(((XmlNode) xmlNode["lastsaved"]).InnerText);
        script.finishTime = ((XmlNode) xmlNode["finishtime"])?.InnerText ?? "N/A";
        string[] strArray = innerText2.Split('\n');
        List<string> stringList = new List<string>();
        bool flag = true;
        foreach (string str1 in strArray)
        {
          string str2 = str1.Trim();
          if (!(string.IsNullOrEmpty(str2) & flag))
          {
            flag = false;
            stringList.Add(str2);
          }
        }
        while (stringList.Count > 0 && string.IsNullOrEmpty(stringList[stringList.Count - 1]))
          stringList.RemoveAt(stringList.Count - 1);
        script.instructions = stringList.ToArray();
        if (script.name == innerText1)
        {
          script.isCurrent = true;
          currentScript = script;
        }
        scriptList.Add(script);
      }
      return scriptList.ToArray();
    }

    public static void SaveScript(string levelName, string scriptName, string[] instructions)
    {
      string filePath = ScriptManager.Instance.GetFilePath(levelName);
      ScriptManager.Instance.InitFile(filePath);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(filePath);
      string str1 = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
      string str2 = "\n";
      if (instructions != null)
      {
        foreach (string instruction in instructions)
          str2 = str2 + new string('\t', 4) + instruction + "\n";
        str2 += new string('\t', 3);
      }
      XmlNode documentElement = (XmlNode) xmlDocument.DocumentElement;
      XmlNode xmlNode1 = documentElement.SelectSingleNode("//list/script[name/text() = \"" + scriptName + "\"]");
      if (xmlNode1 != null)
      {
        ((XmlNode) xmlNode1[nameof (instructions)]).InnerText = str2;
        ((XmlNode) xmlNode1["lastsaved"]).InnerText = str1;
      }
      else
      {
        XmlNode xmlNode2 = documentElement.SelectSingleNode("//list");
        XmlNode node1 = xmlDocument.CreateNode("element", "script", "");
        XmlNode node2 = xmlDocument.CreateNode("element", "name", "");
        node2.InnerText = scriptName;
        node1.AppendChild(node2);
        XmlNode node3 = xmlDocument.CreateNode("element", "id", "");
        node3.InnerText = Guid.NewGuid().ToString();
        node1.AppendChild(node3);
        XmlNode node4 = xmlDocument.CreateNode("element", "created", "");
        node4.InnerText = str1;
        node1.AppendChild(node4);
        XmlNode node5 = xmlDocument.CreateNode("element", "lastsaved", "");
        node5.InnerText = str1;
        node1.AppendChild(node5);
        XmlNode node6 = xmlDocument.CreateNode("element", nameof (instructions), "");
        node6.InnerText = str2;
        node1.AppendChild(node6);
        xmlNode2.AppendChild(node1);
      }
      if (!string.IsNullOrEmpty(LevelManager.CurrentLevelID))
      {
        XmlNode xmlNode3 = documentElement.SelectSingleNode("//scripts/levelid");
        if (xmlNode3 == null)
        {
          xmlNode3 = xmlDocument.CreateNode("element", "levelid", "");
          documentElement.PrependChild(xmlNode3);
        }
        xmlNode3.InnerText = LevelManager.CurrentLevelID;
      }
      using (XmlWriter xmlWriter = XmlWriter.Create(filePath, ScriptManager.Instance.xmlSettings))
        xmlDocument.Save(xmlWriter);
    }

    public static void SaveCurrentScriptName(string levelName, string scriptName)
    {
      string filePath = ScriptManager.Instance.GetFilePath(levelName);
      ScriptManager.Instance.InitFile(filePath);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(filePath);
      XmlNode xmlNode = ((XmlNode) xmlDocument.DocumentElement).SelectSingleNode("//scripts/current");
      if (xmlNode != null)
        xmlNode.InnerText = scriptName;
      using (XmlWriter xmlWriter = XmlWriter.Create(filePath, ScriptManager.Instance.xmlSettings))
        xmlDocument.Save(xmlWriter);
    }

    public static void SaveScriptTime(string levelName, string scriptName, float time)
    {
      string filePath = ScriptManager.Instance.GetFilePath(levelName);
      ScriptManager.Instance.InitFile(filePath);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(filePath);
      XmlNode xmlNode = ((XmlNode) xmlDocument.DocumentElement).SelectSingleNode("//list/script[name/text() = \"" + scriptName + "\"]");
      if (xmlNode != null)
      {
        if (xmlNode["finishtime"] != null)
        {
          ((XmlNode) xmlNode["finishtime"]).InnerText = time.ToString();
        }
        else
        {
          XmlNode node = xmlDocument.CreateNode("element", "finishtime", "");
          node.InnerText = time.ToString();
          xmlNode.InsertAfter(node, (XmlNode) xmlNode["lastsaved"]);
        }
      }
      using (XmlWriter xmlWriter = XmlWriter.Create(filePath, ScriptManager.Instance.xmlSettings))
        xmlDocument.Save(xmlWriter);
    }

    public static void DeleteScript(string levelName, string scriptName)
    {
      string filePath = ScriptManager.Instance.GetFilePath(levelName);
      ScriptManager.Instance.InitFile(filePath);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(filePath);
      XmlNode documentElement = (XmlNode) xmlDocument.DocumentElement;
      XmlNode xmlNode = documentElement.SelectSingleNode("//list");
      foreach (XmlNode selectNode in documentElement.SelectNodes("//list/script[name/text() = \"" + scriptName + "\"]"))
        xmlNode.RemoveChild(selectNode);
      using (XmlWriter xmlWriter = XmlWriter.Create(filePath, ScriptManager.Instance.xmlSettings))
        xmlDocument.Save(xmlWriter);
    }

    public static void RenameScript(string levelName, string scriptName, string newScriptName)
    {
      string filePath = ScriptManager.Instance.GetFilePath(levelName);
      ScriptManager.Instance.InitFile(filePath);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(filePath);
      XmlNode documentElement = (XmlNode) xmlDocument.DocumentElement;
      foreach (XmlNode selectNode in documentElement.SelectNodes("//list/script[name/text() = \"" + scriptName + "\"]"))
        ((XmlNode) selectNode["name"]).InnerText = newScriptName;
      XmlNode xmlNode = documentElement.SelectSingleNode("//current");
      if (xmlNode != null && xmlNode.InnerText == scriptName)
        xmlNode.InnerText = newScriptName;
      using (XmlWriter xmlWriter = XmlWriter.Create(filePath, ScriptManager.Instance.xmlSettings))
        xmlDocument.Save(xmlWriter);
    }

    public static bool ScriptNameValidator(string name, out string errorMessage)
    {
      if (name.Length < 3)
      {
        errorMessage = "Name too short! (min 3 symbols)";
        return false;
      }
      if (name.Length > 10)
      {
        errorMessage = "Name too long! (max 10 symbols)";
        return false;
      }
      string filePath = ScriptManager.Instance.GetFilePath(LevelManager.CurrentLevelName);
      ScriptManager.Instance.InitFile(filePath);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(filePath);
      XmlNode documentElement = (XmlNode) xmlDocument.DocumentElement;
      name = name.ToLower();
      if (documentElement.SelectSingleNode("//list/script[name/text() = \"" + name + "\"]") != null)
      {
        errorMessage = "Instructions with that name exist already";
        return false;
      }
      errorMessage = "null";
      return true;
    }

    public static void RenameSaveFile(string levelName, string newLevelName)
    {
      string filePath1 = ScriptManager.Instance.GetFilePath(levelName);
      if (!File.Exists(filePath1))
        return;
      string filePath2 = ScriptManager.Instance.GetFilePath(newLevelName);
      File.Move(filePath1, filePath2);
    }

    public static void RenameSaveFileById(string levelid, string levelName)
    {
      foreach (string file in Directory.GetFiles(ScriptManager.Instance.ScriptBasePath))
      {
        string levelId = ScriptManager.Instance.GetLevelId(file);
        if (levelid == levelId)
        {
          string filePath = ScriptManager.Instance.GetFilePath(levelName);
          File.Move(file, filePath);
        }
      }
    }

    public static void ReplaceInAllScripts(params (string, string)[] queries)
    {
      foreach (string file in Directory.GetFiles(ScriptManager.Instance.ScriptBasePath))
      {
        string str = File.ReadAllText(file);
        foreach ((string, string) query in queries)
          str = Regex.Replace(str, query.Item1, query.Item2);
        File.WriteAllText(file, str);
      }
    }

    public static void FindLegacyScripts(string levelName)
    {
      DirectoryInfo parent1 = Directory.GetParent(VarManager.GetString("appdirectory"));
      List<string> list = new List<string>();
      searchRec(Utility.SimpleLevelName(levelName), parent1, list);
      if (!levelName.Contains("Custom"))
      {
        DirectoryInfo parent2 = Directory.GetParent(VarManager.GetString("tempdirectory")).Parent;
        searchRec(Utility.SimpleLevelName(levelName), parent2, list);
      }
      ScriptManager.legacyScriptContainer?.Destroy();
      Vector2 virtualSize = Utility.VirtualSize;
      Color color = ColorManager.GetColor("white");
      //BitmapFont font = TextureManager.GetFont("megaMan2");
      virtualSize.X -= 340f;
      ScriptManager.legacyScriptContainer = new Flexbox(Rectangle.Empty, new Styling?(new Styling()
      {
        borderColor = color,
        defaultColor = ColorManager.GetColor("lightslate"),
        borderWidth = 4,
        padding = 25,
        isLevelUi = true
      }), new Point?(new Point(340 + (int) virtualSize.X / 2, (int) virtualSize.Y / 2)));
      Rectangle rectangle = new Rectangle(0, 0, 550, 50);
      Styling styling1 = new Styling();
      styling1.text = "Legacy Scripts for " + levelName + ":";
      styling1.borderColor = color;
      styling1.borderWidth = 2;
      styling1.defaultTextColor = color;
      //styling1.font = font;
      styling1.textOffset = 25;
      Styling? style = new Styling?(styling1);
      UIElement header = new UIElement(rectangle, style);
      Styling defaultButtonStyling1 = Styling.DefaultButtonStyling with
      {
        borderWidth = 2,
        text = "X"
      };
      header.AddChild((UIElement) new Button(new Rectangle(header.Width - 50, 0, 50, 50), (Button.OnClick) (() => ScriptManager.legacyScriptContainer.Destroy()), defaultButtonStyling1));
      ScriptManager.legacyScriptContainer.SetHeader(header);
      Styling defaultButtonStyling2 = Styling.DefaultButtonStyling with
      {
        text = "Import"
      };
      styling1 = new Styling();
      styling1.defaultTextColor = color;
      //styling1.font = font;
      styling1.textColorEffects = true;
      styling1.hoverColor = Utility.ChangeColor(color, -0.3f);
      Styling styling2 = styling1;
      int num = 0;
      foreach (string str1 in list)
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(str1);
        XmlNode xmlNode = ((XmlNode) xmlDocument.DocumentElement).SelectSingleNode("//level/instructions");
        if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText.Replace("\n", "")))
        {
          ++num;
          styling2.text = "Script " + num.ToString() + " - Hover for Info";
          string str2 = str1.Replace('\\', '/');
          if (str2.Length > 50)
          {
            int startIndex = str2.IndexOf('/', 50);
            if (startIndex != -1)
              str2 = str2.Insert(startIndex, "\n");
          }
          styling2.tooltip = "File:\n" + str2 + "\n\nInstructions:\n";
          string[] instructions = xmlNode.InnerText.Split('\n');
                 
          for (int index = 0; index < 15 && index < instructions.Length; ++index)
          {
            ref string local = ref styling2.tooltip;
            local = local + instructions[index] + "\n";
          }
          if (instructions.Length > 15)
          {
            // RnD: explicit reference operation
                        // Replace the problematic line with the following code:
            styling2.tooltip += "[...]";
           // ^ref styling2.tooltip += "[...]";
          }
          UIElement legacyScript = (UIElement) null;
          ScriptManager.legacyScriptContainer.Add(legacyScript = new UIElement(new Rectangle(0, 0, 500, 50), new Styling?(styling2)));
          legacyScript.AddChild((UIElement) new Button(new Rectangle(375, 0, 125, 50), (Button.OnClick) (() => PopupMenu.Instance.Prompt("Enter a name for the script:", new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive), (PromptMenu.InteractionHandlerString) (name =>
          {
            ScriptManager.legacyScriptContainer.Remove(legacyScript);
            name = name.ToLower();
            ScriptManager.SaveScript(levelName, name, instructions);
            if (ScriptManager.legacyScriptContainer.Children.Count == 1)
              ScriptManager.legacyScriptContainer.Destroy();
            PopupMenu.Instance.Inform("Successfully saved script:*" + name, new PromptMenu.InteractionHandler(((UIElement) PopupMenu.Instance).ToggleActive));
          }), new PromptMenu.Validator(ScriptManager.ScriptNameValidator))), defaultButtonStyling2));
        }
      }
      if (num == 0)
      {
        ScriptManager.legacyScriptContainer.Destroy();
        PopupMenu.Instance.Inform("Could not find any legacy scripts!", (PromptMenu.InteractionHandler) (() => PopupMenu.Instance.ShowLoadInstructionsMenu()));
      }
      else
        PopupMenu.Instance.SetActive(false);

      static void searchRec(string levelName, DirectoryInfo current, List<string> list)
      {
        foreach (FileInfo file in current.GetFiles())
        {
          if (((FileSystemInfo) file).Name.Contains(levelName))
            list.Add(((FileSystemInfo) file).FullName);
        }
        foreach (DirectoryInfo directory in current.GetDirectories())
          searchRec(levelName, directory, list);
      }
    }

    private string GetFilePath(string levelName) => this.ScriptBasePath + levelName + ".bas";

    private string GetLevelId(string filePath)
    {
      if (File.Exists(filePath))
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(filePath);
        XmlNode xmlNode = ((XmlNode) xmlDocument.DocumentElement).SelectSingleNode("//levelid");
        if (xmlNode != null)
          return xmlNode.InnerText;
      }
      return (string) null;
    }

    private void InitFile(string path)
    {
      if (File.Exists(path))
        return;
      XmlWriter xmlWriter = XmlWriter.Create(path, this.xmlSettings);
      xmlWriter.WriteStartDocument();
      xmlWriter.WriteStartElement("scripts");
      xmlWriter.WriteStartElement("current");
      xmlWriter.WriteEndElement();
      if (!string.IsNullOrEmpty(LevelManager.CurrentLevelID))
      {
        xmlWriter.WriteStartElement("levelid");
        xmlWriter.WriteString(LevelManager.CurrentLevelID);
        xmlWriter.WriteEndElement();
      }
      xmlWriter.WriteStartElement("list");
      xmlWriter.WriteEndElement();
      xmlWriter.WriteEndElement();
      xmlWriter.WriteEndDocument();
      xmlWriter.Close();
    }
  }
}

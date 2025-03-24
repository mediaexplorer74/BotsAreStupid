// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.EditorAction
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal struct EditorAction(
    LevelEditor.Action doHandler,
    LevelEditor.Action undoHandler,
    GameObject[] objects)
  {
    private LevelEditor.Action doHandler = doHandler;
    private LevelEditor.Action undoHandler = undoHandler;
    public GameObject[] objects = objects;

    public void Do()
    {
      LevelEditor.Action doHandler = this.doHandler;
      if (doHandler == null)
        return;
      doHandler(this.objects);
    }

    public void Undo()
    {
      LevelEditor.Action undoHandler = this.undoHandler;
      if (undoHandler == null)
        return;
      undoHandler(this.objects);
    }
  }
}

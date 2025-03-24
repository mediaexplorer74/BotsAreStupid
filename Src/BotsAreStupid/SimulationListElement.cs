// Decompiled with JetBrains decompiler
// Type: BotsAreStupid.SimulationListElement
// Assembly: BotsAreStupid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0154E4A0-7A28-4058-AC48-930AF97751F9
// Assembly location: BotsAreStupid.dll inside C:\Users\Admin\Desktop\RE\BotsAreStupid\BotsAreStupid (x64).exe)

#nullable disable
namespace BotsAreStupid
{
  internal class SimulationListElement : IListElement
  {
    public Simulation Simulation { get; private set; }

    public SimulationListElement(Simulation simulation) => this.Simulation = simulation;

    public ListElementAction[] GetAvailableActions()
    {
      return new ListElementAction[1]
      {
        new ListElementAction(ListElementActionType.Delete)
      };
    }

    public object GetValue(int id) => id == 0 ? (object) this.Simulation.PlayerName : (object) null;
  }
}

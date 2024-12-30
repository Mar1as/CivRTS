using System.Collections.Generic;
using UnityEngine;

public class PlayerRTS
{
    FactionsInCiv faction;
    public ArmyHexUnit army { get; private set; }
    GameObject spawnPoint;

    List<GameObject> allAliveUnits = new List<GameObject>();
    List<GameObject> allSelectedUnits = new List<GameObject>();


    public PlayerRTS(FactionsInCiv faction, ArmyHexUnit army)
    {
        this.faction = faction;
        this.army = army;
    }
    public PlayerRTS(PassInformation pass, GameObject spawnPoint)
    {
        this.faction = pass.player.faction;
        this.army = pass.army;
        this.spawnPoint = spawnPoint;
    }


}

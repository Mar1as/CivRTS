using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public FactionsInCiv faction; // Frakce hr·Ëe
    private List<GameObject> Armies = new List<GameObject>();

    public Player(FactionsInCiv faction)
    {
        this.faction = faction;
    }

    public void AddArmy(GameObject army)
    {
        army.GetComponent<MainHexUnit>().dataHexUnit.armyHexUnit.player = this;

        if (Armies.Contains(army)) return;

        Armies.Add(army);
    }

}

using System;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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
        if (Armies.Contains(army)) return;

        Armies.Add(army);
    }

    internal void RemoveArmy(GameObject army)
    {
        Armies.Remove(army);
    }
}

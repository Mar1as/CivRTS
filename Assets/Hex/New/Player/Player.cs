using System;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

[Serializable]
public class Player
{
    public FactionsInCiv faction; // Frakce hr·Ëe
    private List<GameObject> Armies = new List<GameObject>();
    public bool ai { get; private set; } = false;

    public Player(FactionsInCiv faction, bool ai = false)
    {
        this.faction = faction;
        this.ai = ai;
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

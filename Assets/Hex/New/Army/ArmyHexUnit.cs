using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmyHexUnit
{
    MainHexUnit mainHexUnit;

    [SerializeField]
    public Player player;
    List<GameObject> unitsInArmy = new List<GameObject>();

    public ArmyHexUnit(MainHexUnit mainHexUnit)
    {
        this.mainHexUnit = mainHexUnit;
        
        if (player == null) CivGameManagerSingleton.Instance.players[CivGameManagerSingleton.Instance.players.Length - 1].AddArmy(mainHexUnit.gameObject);
        else player.AddArmy(mainHexUnit.gameObject);
    }

    void AddUnit(GameObject unit)
    {
        unitsInArmy.Add(unit);
        Contains();
    }

    void RemoveUnit(GameObject unit)
    {
        unitsInArmy.Remove(unit);
        Contains();
    }

    void Contains()
    {
        string armyName = "";
        for (int i = 0; i < unitsInArmy.Count; i++)
        {
            string unitName = "";
            GameObject gmUnit = unitsInArmy[i];
            unitName += $"{i}: " + gmUnit.name;
            armyName += unitName + "\n";
        }
        Debug.Log(armyName);
    }
}


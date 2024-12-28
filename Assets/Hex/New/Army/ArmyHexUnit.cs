using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmyHexUnit
{
    MainHexUnit mainHexUnit;

    public List<GameObject> unitsInArmy = new List<GameObject>();

    public ArmyHexUnit()
    {
        
    }

    public ArmyHexUnit(MainHexUnit mainHexUnit)
    {
        this.mainHexUnit = mainHexUnit;
    }

    public void AddUnit(GameObject unit)
    {
        unitsInArmy.Add(unit);
        Contains();
    }

    public void RemoveUnit(GameObject unit)
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


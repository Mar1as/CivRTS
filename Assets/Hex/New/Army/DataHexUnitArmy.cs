using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataHexUnitArmy
{
    [SerializeField]
    public List<UnitData> unitsInArmy = new List<UnitData>();

    public DataHexUnitArmy()
    {
        
    }

    public DataHexUnitArmy Clone()
    {
        DataHexUnitArmy clonedArmy = new DataHexUnitArmy();
        foreach (var unit in unitsInArmy)
        {
            clonedArmy.unitsInArmy.Add(unit);
        }
        return clonedArmy;
    }

    public void AddUnit(UnitData unit)
    {
        unitsInArmy.Add(unit);
        Contains();
    }

    public void RemoveUnit(UnitData unit)
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
            UnitData gmUnit = unitsInArmy[i];
            unitName += $"{i}: " + gmUnit.name;
            armyName += unitName + "\n";
        }
        Debug.Log(armyName);
    }

    public int CostOfArmy()
    {
        float cost = 0;
        foreach (var item in unitsInArmy)
        {
            cost += item.productionCost;
        }
        return (int)Math.Ceiling(cost);
    }
}


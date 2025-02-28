using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataHexUnitArmy
{
    [SerializeField]
    public List<UnitData> unitsInArmy = new List<UnitData>();

    public Dictionary<UnitData, int> unitSupply = new Dictionary<UnitData, int>();

    public DataHexUnitArmy()
    {
        UnitSupply();
    }

    public void AddUnit(UnitData unit)
    {
        if (unitSupply.ContainsKey(unit))
        {
            unitSupply[unit]++;
        }
        else
        {
            unitSupply[unit] = 1;
        }

        unitsInArmy.Add(unit);
        Contains();
    }

    public void RemoveUnit(UnitData unit)
    {
        if (unitSupply.ContainsKey(unit) && unitSupply[unit] > 0)
        {
            unitSupply[unit]--;
            if (unitSupply[unit] == 0)
            {
                unitSupply.Remove(unit);
            }
        }

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

    Dictionary<UnitData, int> UnitSupply()
    {
        unitSupply = new Dictionary<UnitData, int>();
        for (int i = 0; i < unitsInArmy.Count; i++)
        {
            if (unitSupply.ContainsKey(unitsInArmy[i]))
            {
                unitSupply[unitsInArmy[i]]++;
            }
            else
            {
                unitSupply.Add(unitsInArmy[i], 1);
            }
        }
        return unitSupply;
    }

    public bool CanBuyUnit(UnitData unit)
    {
        return unitSupply.ContainsKey(unit) && unitSupply[unit] > 0;
    }

    public DataHexUnitArmy(DataHexUnitArmy other)
    {
        foreach (var unit in other.unitsInArmy)
        {
            unitsInArmy.Add(new UnitData()); // Kopírování jednotek
        }
    }

    public DataHexUnitArmy Clone()
    {
        return new DataHexUnitArmy(this);
    }

    public bool HasUnitsInArmy()
    {
        if (unitsInArmy.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


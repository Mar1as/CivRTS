using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DataCity
{
    MainCity mainCity;

    public StatsCity statsCity;
    public ProductionCity productionCity;

    public DataCity(MainHexCell cell ,MainCity mainHexUnit)
    {
        this.mainCity = mainHexUnit;
        productionCity = new ProductionCity(mainHexUnit);
        statsCity = new StatsCity(mainHexUnit);
        Location = cell;
        AddBordersOnCreation();
        FindNextExpansion();
    }

    public List<MainHexCell> cellsInBorder = new List<MainHexCell>();

    MainHexCell location;
    public MainHexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            location = value;
            if (value != null)
            {
                AddBorder(value);
            }
        }
    }

    public void SelfDestruct()
    {
        for (int i = 0; i < cellsInBorder.Count; i++)
        {
            cellsInBorder[i].dataHexCell.city = null;
            cellsInBorder[i].dataHexCell.wallsScript.Walled = false;
        }
    }

    public void GainResourcesPerTurn()
    {

    }

    #region Border Management

    MainHexCell nextCellToConquer;

    void AddBordersOnCreation()
    {
        if (location == null) return;

        for (HexDirection direction = HexDirection.NE; direction <= HexDirection.NW; direction++)
        {
            MainHexCell neighbor = location.brainHexCell.GetNeighbor(direction);
            if (neighbor != null && neighbor.dataHexCell.city == null)
            {
                HexEdgeType edgeType = location.brainHexCell.GetEdgeType(neighbor);
                if (edgeType != HexEdgeType.Cliff)
                {
                    AddBorder(neighbor);
                }
            }
        }
    }

    void AddBorder(MainHexCell neighbor)
    {
        cellsInBorder.Add(neighbor);
        neighbor.dataHexCell.city = mainCity;
        neighbor.dataHexCell.wallsScript.Walled = true;
    }

    #endregion

    #region Expansion Management

    void FindNextExpansion()
    {
        nextCellToConquer = null;

        foreach (var cell in cellsInBorder)
        {
            for (HexDirection direction = HexDirection.NE; direction <= HexDirection.NW; direction++)
            {
                MainHexCell neighbor = cell.brainHexCell.GetNeighbor(direction);
                if (neighbor != null && neighbor.dataHexCell.city == null)
                {
                    HexEdgeType edgeType = cell.brainHexCell.GetEdgeType(neighbor);
                    if (edgeType != HexEdgeType.Cliff)
                    {
                        nextCellToConquer = neighbor;
                        return;
                    }
                }
            }
        }
    }

    public void ExpandCity()
    {
        if (nextCellToConquer == null) return;

        AddBorder(nextCellToConquer);
        FindNextExpansion();
    }

    public void ShrinkCity()
    {
        nextCellToConquer = cellsInBorder[cellsInBorder.Count - 1];
        cellsInBorder[cellsInBorder.Count - 1].dataHexCell.wallsScript.Walled = false;
        cellsInBorder[cellsInBorder.Count - 1].dataHexCell.city = null;
    }

    #endregion
}

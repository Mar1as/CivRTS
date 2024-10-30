using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Road
{
    MainHexCell mainHexCell;

    [SerializeField] public bool[] roads;

    public bool HasRoads
    {
        get
        {
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i])
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return roads[(int)direction];
    }

    public Road(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
        roads = new bool[6];
    }

    public void RemoveRoads()
    {
        for (int i = 0; i < mainHexCell.dataHexCell.neighbours.Length; i++)
        {
            if (roads[i])
            {
                SetRoad(i, false);
                roads[i] = false;
                
            }
        }
    }

    public void AddRoad(HexDirection direction)
    {
        if (!roads[(int)direction] && !mainHexCell.dataHexCell.river.HasRiverThroughEdge(direction) && GetElevationDifference(direction) <= 1)
        {
            SetRoad((int)direction, true);
        }
    }

    public void SetRoad(int index, bool state)
    {
        roads[index] = state;

        mainHexCell.dataHexCell.neighbours[index].dataHexCell.roadScript.roads[(int)((HexDirection)index).Opposite()] = state;
        mainHexCell.dataHexCell.neighbours[index].brainHexCell.RefreshSelfOnly();
        mainHexCell.brainHexCell.RefreshSelfOnly();
    }

    public int GetElevationDifference(HexDirection direction)
    {
        int difference = mainHexCell.dataHexCell.Elevation - mainHexCell.brainHexCell.GetNeighbor(direction).dataHexCell.Elevation;
        return difference >= 0 ? difference : -difference;
    }
}

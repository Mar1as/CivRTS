using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainHexCell
{
    private MainHexCell mainHexCell; //Pointer
    private DataHexCell dataHexCell; //Pointer

    public BrainHexCell(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
        dataHexCell = mainHexCell.dataHexCell;
    }

    public MainHexCell GetNeighbor(HexDirection direction)
    {
        return dataHexCell.neighbours[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, MainHexCell cell)
    {
        dataHexCell.neighbours[(int)direction] = cell;
        cell.dataHexCell.neighbours[(int)direction.Opposite()] = mainHexCell;
    }
    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(dataHexCell.Elevation, dataHexCell.neighbours[(int)direction].dataHexCell.Elevation);
    }
    public HexEdgeType GetEdgeType(MainHexCell otherCell)
    {
        return HexMetrics.GetEdgeType(dataHexCell.Elevation, otherCell.dataHexCell.Elevation);
    }

    public void Refresh()
    {
        if (dataHexCell.chunk)
        {
            dataHexCell.chunk.Refresh();
            for (int i = 0; i < dataHexCell.neighbours.Length; i++)
            {
                MainHexCell neighbor = dataHexCell.neighbours[i];
                if (neighbor != null && neighbor.dataHexCell.chunk != dataHexCell.chunk)
                {
                    neighbor.dataHexCell.chunk.Refresh();
                }
            }
            if (dataHexCell.Unit)
            {
                dataHexCell.Unit.dataHexUnit.ValidateLocation();
            }
        }
    }
    public void RefreshSelfOnly()
    {
        dataHexCell.chunk.Refresh();
        if (dataHexCell.Unit)
        {
            dataHexCell.Unit.dataHexUnit.ValidateLocation();
        }
    }
}

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions
{

    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }

    public static HexDirection Previous2(this HexDirection direction)
    {
        direction -= 2;
        return direction >= HexDirection.NE ? direction : (direction + 6);
    }

    public static HexDirection Next2(this HexDirection direction)
    {
        direction += 2;
        return direction <= HexDirection.NW ? direction : (direction - 6);
    }


}

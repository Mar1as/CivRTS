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
}

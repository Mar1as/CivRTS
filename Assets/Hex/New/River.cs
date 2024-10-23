using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class River
{
    MainHexCell mainHexCell;

    public River(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
    }

    public bool hasIncomingRiver, hasOutgoingRiver;
    public HexDirection incomingRiver, outgoingRiver;

    public bool HasIncomingRiver
    {
        get
        {
            return hasIncomingRiver;
        }
    }

    public bool HasOutgoingRiver
    {
        get
        {
            return hasOutgoingRiver;
        }
    }

    public HexDirection IncomingRiver
    {
        get
        {
            return incomingRiver;
        }
    }

    public HexDirection OutgoingRiver
    {
        get
        {
            return outgoingRiver;
        }
    }

    public bool HasRiver
    {
        get
        {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }

    public bool HasRiverBeginOrEnd
    {
        get
        {
            return hasIncomingRiver != hasOutgoingRiver;
        }
    }

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return
            hasIncomingRiver && incomingRiver == direction ||
            hasOutgoingRiver && outgoingRiver == direction;
    }


    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver)
        {
            return;
        }
        hasOutgoingRiver = false;
        mainHexCell.brainHexCell.RefreshSelfOnly();

        MainHexCell neighbor = mainHexCell.brainHexCell.GetNeighbor(outgoingRiver);
        neighbor.dataHexCell.river.hasIncomingRiver = false;
        mainHexCell.brainHexCell.RefreshSelfOnly();
    }

    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver)
        {
            return;
        }
        hasIncomingRiver = false;
        mainHexCell.brainHexCell.RefreshSelfOnly();

        MainHexCell neighbor = mainHexCell.brainHexCell.GetNeighbor(incomingRiver);
        neighbor.dataHexCell.river.hasOutgoingRiver = false;
        mainHexCell.brainHexCell.RefreshSelfOnly();
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (hasOutgoingRiver && outgoingRiver == direction)
        {
            return;
        }
        MainHexCell neighbor = mainHexCell.brainHexCell.GetNeighbor(direction);
        if (!neighbor || mainHexCell.dataHexCell.Elevation < neighbor.dataHexCell.Elevation)
        {
            return;
        }
        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }
        hasOutgoingRiver = true;
        outgoingRiver = direction;
        mainHexCell.brainHexCell.RefreshSelfOnly();

        neighbor.dataHexCell.river.RemoveIncomingRiver();
        neighbor.dataHexCell.river.hasIncomingRiver = true;
        neighbor.dataHexCell.river.incomingRiver = direction.Opposite();
        neighbor.brainHexCell.RefreshSelfOnly();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript
{
    MainHexCell mainHexCell;

    public WaterScript(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
    }

    int waterLevel;

    public int WaterLevel
    {
        get
        {
            return waterLevel;
        }
        set
        {
            if (waterLevel == value)
            {
                return;
            }
            waterLevel = value;
            mainHexCell.dataHexCell.river.ValidateRivers();
            mainHexCell.brainHexCell.Refresh();
        }
    }

    public bool IsUnderwater
    {
        get
        {
            return waterLevel > mainHexCell.dataHexCell.Elevation;
        }
    }

    public float WaterSurfaceY
    {
        get
        {
            return
                (waterLevel + HexMetrics.waterElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    public bool IsValidRiverDestination(MainHexCell neighbor)
    {
        return neighbor && (
            mainHexCell.dataHexCell.Elevation >= neighbor.dataHexCell.Elevation || waterLevel == neighbor.dataHexCell.Elevation
        );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FeaturesHexCell
{
    MainHexCell mainHexCell;

    [SerializeField]
    int urbanLevel, farmLevel, plantLevel, specialIndex;
    public int UrbanLevel
    {
        get
        {
            return urbanLevel;
        }
        set
        {
            if (urbanLevel != value)
            {
                urbanLevel = value;
                mainHexCell.brainHexCell.RefreshSelfOnly();
            }
        }
    }

    public int FarmLevel
    {
        get
        {
            return farmLevel;
        }
        set
        {
            if (farmLevel != value)
            {
                farmLevel = value;
                mainHexCell.brainHexCell.RefreshSelfOnly();
            }
        }
    }

    public int PlantLevel
    {
        get
        {
            return plantLevel;
        }
        set
        {
            if (plantLevel != value)
            {
                plantLevel = value;
                mainHexCell.brainHexCell.RefreshSelfOnly();
            }
        }
    }

    public int SpecialIndex
    {
        get
        {
            return specialIndex;
        }
        set
        {
            if (specialIndex != value && !mainHexCell.dataHexCell.river.HasRiver)
            {
                specialIndex = value;
                mainHexCell.brainHexCell.RefreshSelfOnly();
            }
        }
    }

    public bool IsSpecial
    {
        get
        {
            return specialIndex > 0;
        }
    }


    public FeaturesHexCell(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Walls
{

    [SerializeField]
    bool walled;

    public bool Walled
    {
        get
        {
            return walled;
        }
        set
        {
            if (walled != value)
            {
                walled = value;
                mainHexCell.brainHexCell.Refresh();
            }
        }
    }

    MainHexCell mainHexCell;

    public Walls(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
    }

}

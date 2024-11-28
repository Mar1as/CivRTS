using System.IO;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class DataHexUnit
{
    MainHexUnit mainHexUnit;

    public DataHexUnit(MainHexUnit mainHexUnit)
    {
        this.mainHexUnit = mainHexUnit;
    }

    public static MainHexUnit unitPrefab;

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
            value.dataHexCell.Unit = mainHexUnit;
            mainHexUnit.transform.localPosition = value.dataHexCell.Position;
        }
    }

    float orientation;
    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            mainHexUnit.transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }


    public void ValidateLocation()
    {
        mainHexUnit.transform.localPosition = location.dataHexCell.Position;
    }

    
}

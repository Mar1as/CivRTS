using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Lol;

[System.Serializable]
public class DataHexCell
{
    /*TerrainType terrainType = TerrainType.None;

    public TerrainType TerrainType
    {
        get
        {
            return TerrainType;
        }
        set
        {
            TerrainType = value;
        }
    }*/

    [SerializeField]
    public HexCoordinates coordinates = new HexCoordinates(0,0);
    [SerializeField]
    public MainHexCell[] neighbours = new MainHexCell[6];

    public DataHexCell()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Lol;

public class DataHexCell
{
    TerrainType terrainType = TerrainType.None;

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
    }

    public HexCoordinates coordinates;

    public DataHexCell(int x, int y)
    {

    }

}

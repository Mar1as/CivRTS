using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    private MainHexCell mainHexCell; //Pointer Parent

    public HexGridChunk chunk;

    public RectTransform uiRect;
    [SerializeField]
    public HexCoordinates coordinates = new HexCoordinates(0,0);
    [SerializeField]
    public MainHexCell[] neighbours = new MainHexCell[6];
    [SerializeField]
    public Color color;

    public Color Color
    {
        get
        {
            return color;
        }
        set
        {
            if (color == value)
            {
                return;
            }
            color = value;
            mainHexCell.brainHexCell.Refresh();
        }
    }

    //[SerializeField]
    private int elevation = int.MinValue;

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if (elevation == value)
            {
                return;
            }

            elevation = value;
            Vector3 position = mainHexCell.transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;

            mainHexCell.transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;

            mainHexCell.brainHexCell.Refresh();
        }
    }

    public Vector3 Position
    {
        get
        {
            return mainHexCell.transform.localPosition;
        }
    }

    public DataHexCell(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
    }
}

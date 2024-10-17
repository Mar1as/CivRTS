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

    public RectTransform uiRect;
    [SerializeField]
    public HexCoordinates coordinates = new HexCoordinates(0,0);
    [SerializeField]
    public MainHexCell[] neighbours = new MainHexCell[6];
    [SerializeField]
    public Color color;
    //[SerializeField]
    private int elevation;

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            elevation = value;
            Vector3 position = mainHexCell.transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            mainHexCell.transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = elevation * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;
        }
    }


    public DataHexCell(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
    }
}

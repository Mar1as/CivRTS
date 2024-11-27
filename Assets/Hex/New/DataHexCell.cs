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
    public River river;
    public Road roadScript;
    public WaterScript waterScript;
    public FeaturesHexCell featuresHexCell;
    public Walls wallsScript;
    public SaveLoadHexCell saveLoadHexCell;

    public HexCellDistance hexCellDistance;

    public HexGridChunk chunk;

    public DataHexCell(MainHexCell mainHexCell)
    {
        this.mainHexCell = mainHexCell;
        river = new River(mainHexCell);
        roadScript = new Road(mainHexCell);
        waterScript = new WaterScript(mainHexCell);
        featuresHexCell = new FeaturesHexCell(mainHexCell);
        wallsScript = new Walls(mainHexCell);
        saveLoadHexCell = new SaveLoadHexCell(mainHexCell);
        hexCellDistance = new HexCellDistance(mainHexCell);
    }

    public RectTransform uiRect;
    [SerializeField]
    public HexCoordinates coordinates = new HexCoordinates(0,0);
    [SerializeField]
    public MainHexCell[] neighbours = new MainHexCell[6];
    [SerializeField]
    int terrainTypeIndex;
    public int TerrainTypeIndex
    {
        get
        {
            return terrainTypeIndex;
        }
        set
        {
            if (terrainTypeIndex != value)
            {
                terrainTypeIndex = value;
                mainHexCell.brainHexCell.Refresh();
            }
        }
    }

    public Color Color
    {
        get
        {
            return HexMetrics.colors[terrainTypeIndex];
        }
    }

    [SerializeField]
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

            RefreshPosition();

            river.ValidateRivers();

            for (int i = 0; i < roadScript.roads.Length; i++)
            {
                if (roadScript.roads[i] && roadScript.GetElevationDifference((HexDirection)i) > 1)
                {
                    roadScript.SetRoad(i, false);
                }
            }

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

    public float StreamBedY
    {
        get
        {
            return
                (elevation + HexMetrics.streamBedElevationOffset) *
                HexMetrics.elevationStep;
        }
    }

    public void RefreshPosition()
    {
        Debug.Log("Refresh");
        Vector3 position = mainHexCell.transform.localPosition;
        position.y = elevation * HexMetrics.elevationStep;
        position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;

        mainHexCell.transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = -position.y;
        uiRect.localPosition = uiPosition;
    }

    
}

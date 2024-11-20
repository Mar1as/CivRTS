using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{
    public HexGrid hexGrid;

    private int activeElevation;
    int activeWaterLevel;
    int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeTerrainTypeIndex;

    private bool applyElevation = true;
    bool applyWaterLevel = true;
    bool applyUrbanLevel, applyFarmLevel, applyPlantLevel;

    private OptionalToggle riverMode, roadMode, walledMode;
    

    bool isDrag;
    HexDirection dragDirection;
    MainHexCell previousCell;


    void Update()
    {
        if (
            Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject()
        )
        {
            HandleInput();
        }
        else
        {
            previousCell = null;
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            MainHexCell currentCell;
            try
            {
                currentCell = hexGrid.GetCell(hit.point);
            }
            catch (System.Exception)
            {
                return;
                //throw;
            }
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            EditCell(currentCell);
            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        }
    }

    public void SetTerrainTypeIndex(int index)
    {
        activeTerrainTypeIndex = index;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
    }
    public void SetRoadMode(int mode)
    {
        roadMode = (OptionalToggle)mode;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetApplyWaterLevel(bool toggle)
    {
        applyWaterLevel = toggle;
    }

    public void SetWaterLevel(float level)
    {
        activeWaterLevel = (int)level;
    }

    public void SetApplyUrbanLevel(bool toggle)
    {
        applyUrbanLevel = toggle;
    }

    public void SetUrbanLevel(float level)
    {
        activeUrbanLevel = (int)level;
    }

    public void SetApplyFarmLevel(bool toggle)
    {
        applyFarmLevel = toggle;
    }

    public void SetFarmLevel(float level)
    {
        activeFarmLevel = (int)level;
    }

    public void SetApplyPlantLevel(bool toggle)
    {
        applyPlantLevel = toggle;
    }

    public void SetPlantLevel(float level)
    {
        activePlantLevel = (int)level;
    }

    public void SetWalledMode(int mode)
    {
        walledMode = (OptionalToggle)mode;
    }

    void EditCell(MainHexCell cell)
    {
        if (activeTerrainTypeIndex >= 0)
        {
            cell.dataHexCell.TerrainTypeIndex = activeTerrainTypeIndex;
        }

        if (applyElevation)
        {
            cell.dataHexCell.Elevation = activeElevation;
        }
        if (applyWaterLevel)
        {
            cell.dataHexCell.waterScript.WaterLevel = activeWaterLevel;
        }

        if (applyUrbanLevel)
        {
            cell.dataHexCell.featuresHexCell.UrbanLevel = activeUrbanLevel;
        }
        if (applyFarmLevel)
        {
            cell.dataHexCell.featuresHexCell.FarmLevel = activeFarmLevel;
        }
        if (applyPlantLevel)
        {
            cell.dataHexCell.featuresHexCell.PlantLevel = activePlantLevel;
        }

        if (riverMode == OptionalToggle.No)
        {
            cell.dataHexCell.river.RemoveRiver();
        }
        else if (isDrag && riverMode == OptionalToggle.Yes)
        {
            previousCell.dataHexCell.river.SetOutgoingRiver(dragDirection);
        }
        if (roadMode == OptionalToggle.No)
        {
            cell.dataHexCell.roadScript.RemoveRoads();
        }
        if (walledMode != OptionalToggle.Ignore)
        {
            cell.dataHexCell.wallsScript.Walled = walledMode == OptionalToggle.Yes;
        }
        else if (isDrag && roadMode == OptionalToggle.Yes)
        {
            previousCell.dataHexCell.roadScript.AddRoad(dragDirection);
        }
    }

    void ValidateDrag(MainHexCell currentCell)
    {
        for (
            dragDirection = HexDirection.NE;
            dragDirection <= HexDirection.NW;
            dragDirection++
        )
        {
            if (previousCell.brainHexCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }

    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    #region Save Load Manager
    public void Save()
    {
        Debug.Log("Save " + Application.persistentDataPath);
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        Debug.Log("Path: " + path);
        using (
            BinaryWriter writer =
                new BinaryWriter(File.Open(path, FileMode.Create))
        )
        {
            writer.Write(0);
            hexGrid.Save(writer);
        }
        Debug.Log("Done");
    }

    public void Load()
    {
        Debug.Log("Load");
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        Debug.Log("Path: " + path);
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            int header = reader.ReadInt32();
            if (header == 0)
            {
                hexGrid.Load(reader);
            }
            else
            {
                Debug.LogWarning("Unknown map format " + header);
            }
        }
        Debug.Log("Done");

    }
    #endregion
}



using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditor : MonoBehaviour
{
    public HexGrid hexGrid;
    public MainHexUnit unitPrefab;

    private int activeElevation;
    int activeWaterLevel;
    int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;
    public int activeTerrainTypeIndex;
    int selectedPlayerIndex = 0;

    public bool applyElevation = true;
    public bool applyWaterLevel = true;
    public bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;

    public OptionalToggle riverMode, roadMode, walledMode;
    

    bool isDrag;
    HexDirection dragDirection;
    MainHexCell previousCell;

    [SerializeField] TMP_InputField saveAs;

    private void Start()
    {
        if (MenuScripts.currentFileIndex >= 0) saveAs.text = Path.GetFileNameWithoutExtension(MenuScripts.saveFiles[MenuScripts.currentFileIndex]);

    }

    void Update()
    {
       if (!EventSystem.current.IsPointerOverGameObject()) {
            if (Input.GetMouseButtonDown(0))
            {
                HandleInput(DrawMode.Put);
                return;
            }
            if (Input.GetMouseButton(0)) {
                HandleInput(DrawMode.Draw);
                return;
            }
            /*if (Input.GetKeyDown(KeyCode.U)) {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    DestroyUnit();
                }
                else CreateUnit();
                return;
            }*/
            try
            {
                /*
                if (Input.GetKeyDown(KeyCode.F)) //FARMA
                {
                    MainHexCell currentCell = GetCellUnderCursor();
                    //currentCell.dataHexCell.city.dataCity.productionCity.productionQueue.AddToQueue(new Building(2, currentCell, BuildingEnum.Farm));
                    return;
                }
                if (Input.GetKeyDown(KeyCode.G)) //URBAN
                {
                    MainHexCell currentCell = GetCellUnderCursor();
                    //currentCell.dataHexCell.city.dataCity.productionCity.productionQueue.AddToQueue(new Building(2, currentCell, BuildingEnum.Urban));
                    return;
                }
                if (Input.GetKeyDown(KeyCode.H)) //UNIT
                {
                    MainHexCell currentCell = GetCellUnderCursor();
                    //currentCell.dataHexCell.city.dataCity.productionCity.productionQueue.AddToQueue(new Unit(2, currentCell, new MainHexUnit()));
                    return;
                }*/
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        else
        {
            previousCell = null;
        }
        
    }

    void HandleInput(DrawMode dm)
    {
        MainHexCell currentCell;
        try
        {
            currentCell = GetCellUnderCursor();
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            if(UiManagerRTS.editMode == EditMode.Edit) EditCell(currentCell, dm);
            

            previousCell = currentCell;
        }
        catch (System.Exception)
        {
            return;
            throw;
        }
        
    }

    

    MainHexCell GetCellUnderCursor()
    {
        return hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    }

    public void SetSelectedPlayerIndex(Slider slider)
    {
        slider.maxValue = CivGameManagerSingleton.Instance.players.Length - 1;
        selectedPlayerIndex = (int)slider.value;
        Debug.Log(selectedPlayerIndex);
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

    public void SetApplySpecialIndex(bool toggle)
    {
        applySpecialIndex = toggle;
    }

    public void SetSpecialIndex(float index)
    {
        activeSpecialIndex = (int)index;
    }


    void EditCell(MainHexCell cell, DrawMode md)
    {
        if (activeTerrainTypeIndex > 0)
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
        if (applySpecialIndex && md == DrawMode.Put)
        {
            if (activeSpecialIndex > 0 && cell.dataHexCell.featuresHexCell.SpecialIndex < 1 && cell.dataHexCell.city == null) //Vytvoøit mìsto
            {
                Debug.Log("vyt");
                Debug.Log(cell.gameObject.transform.position);
                cell.dataHexCell.featuresHexCell.SpecialIndex = activeSpecialIndex;
                MainCity city = new MainCity(cell, CivGameManagerSingleton.Instance.players[selectedPlayerIndex]);
                //cell.dataHexCell.city = city;
            }
            else if (activeSpecialIndex < 1 && cell.dataHexCell.featuresHexCell.SpecialIndex > 0) //Vymazat mìsto
            {
                cell.dataHexCell.featuresHexCell.SpecialIndex = activeSpecialIndex;
                cell.dataHexCell.city = null;
            }

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

    


    #region Unit

    void CreateUnit()
    {
        MainHexCell cell = GetCellUnderCursor();
        if (cell && !cell.dataHexCell.Unit)
        {
            CivGameManagerSingleton.Instance.hexGrid.AddUnit(
                        CivGameManagerSingleton.Instance.players[selectedPlayerIndex].faction.armyUnitStyle[0].GetComponent<MainHexUnit>(), cell, Random.Range(0f, 360f), new DataHexUnitArmy(), CivGameManagerSingleton.Instance.players[selectedPlayerIndex]
                    );
        }
    }

    void DestroyUnit()
    {
        MainHexCell cell = GetCellUnderCursor();
        if (cell && cell.dataHexCell.Unit)
        {
            cell.dataHexCell.Unit.Die();
        }
    }

    #endregion

    #region Save Load Manager
    public void Save()
    {
        Debug.Log(Application.persistentDataPath);
        string path = Path.Combine(Application.persistentDataPath, saveAs.text + ".map");
        Debug.Log(path);
        using (
            BinaryWriter writer =
                new BinaryWriter(File.Open(path, FileMode.Create))
        )
        {
            //writer.Write(0);
            hexGrid.Save(writer);
        }
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            /*int header = reader.ReadInt32();
            if (header <= 0)
            {
                hexGrid.Load(reader);
            }*/

            hexGrid.Load(reader);
        }
    }
    #endregion

    public void Turn()
    {
        Debug.Log("Tah");
        CivGameManagerSingleton.Instance.allUnits.ForEach(unit => unit.dataHexUnit.Turn());
        CivGameManagerSingleton.Instance.allCities.ForEach(city => city.dataCity.Turn());

        //UI

        UiManagerRTS.UpdateAllCitiesBar();

        CivGameManagerSingleton.Instance.ProcessAITurns();
    }
}

public enum OptionalToggle
{
    Ignore, Yes, No
}

public enum DrawMode
{
    Draw, Put
}





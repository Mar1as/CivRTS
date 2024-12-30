using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class HexGameUI : MonoBehaviour
{
    public HexGrid hexGrid;

    MainHexCell currentCell;

    MainHexUnit selectedUnit;
    MainCity selectedCity;
    BuildingData selectedBuilding;

    [SerializeField]
    GameObject buildingShopPanel, armyShopPanel, productionPanel;
    [SerializeField]
    GameObject buildingShopButtonPrefab, armyShopButtonPrefab, productionButtonPrefab;
    [SerializeField]
    GameObject armyParent;

    public event System.Action OnArmyUpdated;

    List<GameObject> army = new List<GameObject>();

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (UiManagerRTS.editMode != EditMode.Game) return;
            if (Input.GetMouseButtonDown(0))
            {
                LeftClickHandler();
            }
            if (Input.GetMouseButtonDown(1))
            {
                RightClickHandler();
            }
            if (selectedUnit)
            {
                Pathfinding();
            }
        }
    }

    void LeftClickHandler()
    {
        MainHexCell currentCell;
        try
        {
            currentCell = GetCellUnderCursor();

            if (!selectedUnit)
            {
                Debug.Log("select unit");
                selectedUnit = SelectUnit();
            }
            else if (selectedUnit)
            {
                Debug.Log("Unselect unit");
                selectedUnit = null;
            }
            if (currentCell.dataHexCell.featuresHexCell.SpecialIndex > 0)
            {
                SelectCity(currentCell);
                Debug.Log("Capital");
            }
            else if(currentCell.dataHexCell.city == null)
            {
                UnselectCity();
                Debug.Log("Unselect");
            }
            else if(selectedBuilding != null && selectedCity != null)
            {
                Build(currentCell);
                Debug.Log("Build");
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("V píèi: " + ex);
        }

    }
    void RightClickHandler()
    {
        MainHexCell currentCell;
        try
        {
            currentCell = GetCellUnderCursor();

            if (selectedUnit)
            {
                bool isEnemyOnCell = currentCell.dataHexCell.Unit != null;
                if (isEnemyOnCell)
                {
                    MainHexCell[] neighborsEnemys = currentCell.brainHexCell.GetAllNeighbors();
                    foreach (MainHexCell neighbor in neighborsEnemys)
                    {
                        Debug.Log(neighbor.dataHexCell.Unit);
                        // Kontrola, zda na sousední buòce je jednotka nepøítele
                        if (neighbor.dataHexCell.Unit != null &&
                            neighbor.dataHexCell.Unit == selectedUnit)
                        {
                            if (selectedUnit.dataHexUnit.PlayerOwner != currentCell.dataHexCell.Unit.dataHexUnit.PlayerOwner)
                            {
                                // Útok na nepøítele
                                Debug.Log("Útok na nepøítele!");
                                selectedUnit.Attack(neighbor.dataHexCell.Unit);
                                return; // Po útoku pøerušíme další akce
                            }
                            else
                            {
                                Debug.Log("Povídání");
                                return;
                            }
                            
                        }

                    }

                }
                // Pokud není žádný nepøítel, jednotka se pohybuje
                Debug.Log("Pohyb jednotky.");
                DoMove();
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("V píèi: " + ex);
        }
    }

    MainHexCell GetCellUnderCursor()
    {
        return hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    }


    #region UnitMovement
    bool UpdateCurrentCell()
    {
        MainHexCell cell = hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell != currentCell)
        {
            currentCell = cell;
            return true;
        }
        return false;
    }

    MainHexUnit SelectUnit()
    {
        UpdateCurrentCell();
        if (currentCell)
        {
            return currentCell.dataHexCell.Unit;
        }
        return null;
    }


    void Pathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (currentCell && selectedUnit.dataHexUnit.IsValidDestination(currentCell))
            {
                bool unitOnCell = currentCell.dataHexCell.Unit != null;
                Player selectedUnitPlayer = selectedUnit.dataHexUnit.PlayerOwner;
                Player destinationUnitPlayer = unitOnCell ? currentCell.dataHexCell.Unit.dataHexUnit.PlayerOwner : null;
                UnitAtDestination unitAtD = UnitAtDestination.None;
                if (unitOnCell)
                {
                    unitAtD = selectedUnitPlayer == destinationUnitPlayer ? UnitAtDestination.Ally : UnitAtDestination.Enemy;
                }

                hexGrid.FindPath(selectedUnit.dataHexUnit.Location, currentCell, 24, unitAtD);
            }
            else
            {
                hexGrid.ClearPath();
            }
        }
    }
    void DoMove()
    {
        if (hexGrid.HasPath)
        {
            Debug.Log("Move2");
            selectedUnit.Travel(hexGrid.GetPath());
            hexGrid.ClearPath();
        }
    }
    #endregion

    #region BuildingMenu
    void SelectCity(MainHexCell currentCell)
    {
        ButtonCancel();
        Player player = currentCell.dataHexCell.city.dataCity.playerOwner;
        selectedCity = currentCell.dataHexCell.city;
        OnArmyUpdated += () => UpdateArmyPanel(player);
        selectedCity.dataCity.Production.productionQueue.OnQueueUpdated += () => UpdateProductionPanel(player);
        UpdateShopPanel(player);
        UpdateProductionPanel(player);
    }

    void UnselectCity()
    {
        ButtonCancel();
        selectedCity = null;
        selectedBuilding = null;
        UpdateShopPanel(null);
        UpdateProductionPanel(null);
    }

    void UpdateShopPanel(Player player)
    {
        foreach (Transform child in buildingShopPanel.transform)
        {
            Destroy(child.gameObject);
        }
        if (player == null) return;
        foreach (var building in player.faction.availableBuildings)
        {
            GameObject buttonObj = Instantiate(buildingShopButtonPrefab, buildingShopPanel.transform);
            Button button = buttonObj.GetComponent<Button>();
            Image image = buttonObj.GetComponentInChildren<Image>(); // Najde Image v potomcích
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>(); // Najde TextMeshProUGUI v potomcích

            if (image != null)
            {
                image.sprite = building.Icon; // Nastaví ikonu budovy
            }

            if (text != null)
            {
                text.text = building.buildingName; // Nastaví název budovy
            }

            button.onClick.AddListener(() => OnBuildingButtonClick(building));
        }
    }

    void OnBuildingButtonClick(BuildingData building)
    {
        if (building.type == BuildingType.Army) newArmy.unitsInArmy.Clear();
        selectedBuilding = building;
        Debug.Log($"Selected building: {building.buildingName}");
    }

    void Build(MainHexCell location)
    {
        Player player = location.dataHexCell.city.dataCity.playerOwner;
        if (selectedBuilding.type == BuildingType.Army)
        {
            //armyTask = new ProductionTask(selectedBuilding, location);
            armyParent.active = true;
            newArmy = new ArmyHexUnit();
            UpdateArmyPanel(player);
        }
        else
        {
            location.dataHexCell.city.dataCity.Production.ProduceBuilding(selectedBuilding, location);
        }
    }

    void UpdateProductionPanel(Player player)
    {
        //Debug.Log(selectedCity.dataCity.Production.productionQueue.queue.Count  + " kokotko ");
        foreach (Transform child in productionPanel.transform)
        {
            Destroy(child.gameObject);
        }
        if (player == null) return;
        foreach (var buildingTask in selectedCity.dataCity.Production.productionQueue.queue)
        {
            BuildingData building = buildingTask.Building;
            GameObject buttonObj = Instantiate(productionButtonPrefab, productionPanel.transform);
            Button button = buttonObj.GetComponent<Button>();
            Image image = buttonObj.GetComponentInChildren<Image>(); // Najde Image v potomcích
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>(); // Najde TextMeshProUGUI v potomcích

            if (image != null)
            {
                image.sprite = building.Icon; // Nastaví ikonu budovy
            }

            if (text != null)
            {
                text.text = building.buildingName; // Nastaví název budovy
            }

            button.onClick.AddListener(() => RemoveBuilding(buildingTask));
        }
    }

    ProductionTask armyTask;
    ArmyHexUnit newArmy = new ArmyHexUnit();

    void UpdateArmyPanel(Player player)
    {

        foreach (Transform child in armyShopPanel.transform)
        {
            Destroy(child.gameObject);
        }
        if (player == null)
        {
            return;
        }

        Dictionary<GameObject, int> unitCount = new Dictionary<GameObject, int>();

        foreach (var unit in player.faction.availableUnits)
        {
            int count = newArmy.unitsInArmy.Count(u => u == unit);

            if (unitCount.ContainsKey(unit))
            {
                unitCount[unit] += count;
            }
            else
            {
                unitCount[unit] = count;
            }
        }

        foreach (var unit in player.faction.availableUnits)
        {
            GameObject buttonObj = Instantiate(armyShopButtonPrefab, armyShopPanel.transform);
            Button[] buttons = buttonObj.GetComponentsInChildren<Button>();
            Image image = buttonObj.GetComponentInChildren<Image>(); // Najde Image v potomcích
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>(); // Najde TextMeshProUGUI v potomcích
            buttons[0].onClick.AddListener(() => AddUnit(unit));
            buttons[1].onClick.AddListener(() => RemoveUnit(unit));

            if (image != null)
            {
                //image.sprite = unit.Icon; // Nastaví ikonu budovy
            }

            if (text != null)
            {
                text.text = unit.name + " " + unitCount[unit]; 
            }
        }
    }

    void AddUnit(GameObject unit)
    {
        newArmy.AddUnit(unit);
        Debug.Log("X " + newArmy.unitsInArmy.Count);
        OnArmyUpdated.Invoke();
    }
    void RemoveUnit(GameObject unit)
    {
        newArmy.RemoveUnit(unit);
        OnArmyUpdated.Invoke();
    }

    void RemoveBuilding(ProductionTask task)
    {
        selectedCity.dataCity.Production.productionQueue.RemoveTaskFromQueue(task);
    }
    #endregion

    #region Buttons
    public void ButtonCancel()
    {
        newArmy.unitsInArmy.Clear();
        armyParent.active = false;
    }
    public void ButtonConfirm()
    {
        ArmyHexUnit arm = newArmy.Clone();
        arm.unitsInArmy = newArmy.unitsInArmy;
        Debug.Log("2X " + newArmy.unitsInArmy.Count);

        selectedBuilding.army = arm;
        Debug.Log("3X " + selectedBuilding.army.unitsInArmy.Count);

        Debug.Log("4X " + selectedBuilding.army.unitsInArmy.Count);

        armyParent.active = false;
        Debug.Log(selectedCity.dataCity.Location);
        selectedCity.dataCity.Production.productionQueue.AddToQueue(selectedBuilding, selectedCity.dataCity.Location);
    }
    #endregion
}

public enum UnitAtDestination
{
    Enemy, Ally, None
}

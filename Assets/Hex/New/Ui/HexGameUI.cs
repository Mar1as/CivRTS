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
    Player playeros;

    public HexGrid hexGrid;

    MainHexCell currentCell;

    MainHexUnit selectedUnit;
    MainCity selectedCity;
    BuildingData selectedBuilding;
    
    [SerializeField]
    GameObject cityShopPanel, armyShopPanel, productionPanel;
    [SerializeField]
    GameObject buildingShopButtonPrefab, armyShopButtonPrefab, productionButtonPrefab;
    [SerializeField]
    GameObject armyParent;

    //UI
    [SerializeField]
    GameObject cityUiHolder;
    [SerializeField]
    Image levelImg;
    [SerializeField]
    TextMeshProUGUI levelText, populationText, productionText, foodText, buildButtonText;

    public event System.Action OnArmyUpdated;

    public event System.Action OnTurn;

    List<GameObject> army = new List<GameObject>();

    float costOfArmy = 0;

    private void Start()
    {
        cityUiHolder.SetActive(false);

        OnTurn += () => UpdateStatsPanel();

        playeros = CivGameManagerSingleton.Instance.players[0];

    }

    private void OnEnable()
    {
    }

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
                if (SelectUnit())
                {
                    Debug.Log("select unit");
                    MainHexUnit unit = SelectUnit();
                    if (unit.dataHexUnit.PlayerOwner != playeros) return;
                    selectedUnit = unit;
                }
                
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
            Debug.Log("Problém: " + ex);
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
                MainHexUnit unitOnCell = currentCell.dataHexCell.Unit;
                if(unitOnCell)
                {
                    MainHexCell[] neighbors = selectedUnit.dataHexUnit.Location.brainHexCell.GetAllNeighbors();

                    foreach (MainHexCell neighbor in neighbors)
                    {
                        var neighborUnit = neighbor.dataHexCell.Unit;

                        if (neighborUnit == unitOnCell)
                        {
                            if (neighborUnit != selectedUnit &&
                                neighborUnit.dataHexUnit.PlayerOwner != selectedUnit.dataHexUnit.PlayerOwner)
                            {
                                Debug.Log("Útok na nepøítele!");
                                selectedUnit.Attack(selectedUnit, neighborUnit);
                                return;
                            }
                            else if (neighborUnit == selectedUnit)
                            {
                                Debug.Log("Povídání");
                                return;
                            }
                        }
                    }
                }

                Debug.Log("Pohyb jednotky.");
                DoMove();
            }

        }
        catch (System.Exception ex)
        {
            Debug.Log("Jáj: " + ex);
        }
    }

    MainHexCell GetCellUnderCursor()
    {
        return hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    }


    #region UnitMovement
    bool UpdateCurrentCell()
    {
        try
        {
            MainHexCell cell = hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (cell != currentCell)
            {
                currentCell = cell;
                return true;
            }
            return false;
        }
        catch (System.Exception e )
        {
            Debug.Log("Problém: " + e);
            return false;
        }
        
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
        if (!currentCell.dataHexCell.city.dataCity.playerOwner.Equals(playeros)) return;

        newArmy = new DataHexUnitArmy();
        cityUiHolder.SetActive(true);
        ButtonCancel();
        Player player = currentCell.dataHexCell.city.dataCity.playerOwner;
        selectedCity = currentCell.dataHexCell.city;
        OnArmyUpdated += () => UpdateArmyPanel(player);
        selectedCity.dataCity.Production.productionQueue.OnQueueUpdated += () => UpdateProductionPanel(player);
        UpdateShopPanel(player);
        UpdateProductionPanel(player);
        UpdateStatsPanel(
                    level: $"{selectedCity.dataCity.Stats.level}",
                    population: $"{selectedCity.dataCity.Stats.CalculatePopulation()}",
                    production: $"{selectedCity.dataCity.Stats.CalculateProduction()}",
                    food: $"{selectedCity.dataCity.Stats.CalculateFood()}",
                    levelProgress: (float)selectedCity.dataCity.Stats.levelProgress / selectedCity.dataCity.Stats.levelUpRequirement
                );
    }

    void UnselectCity()
    {
        cityUiHolder.SetActive(false);
        ButtonCancel();
        selectedCity = null;
        selectedBuilding = null;
        UpdateShopPanel(null);
        UpdateProductionPanel(null);
    }

    void UpdateShopPanel(Player player)
    {
        foreach (Transform child in cityShopPanel.transform)
        {
            Destroy(child.gameObject);
        }
        if (player == null) return;
        foreach (var building in player.faction.availableBuildings)
        {
            GameObject buttonObj = Instantiate(buildingShopButtonPrefab, cityShopPanel.transform);
            Button button = buttonObj.GetComponent<Button>();
            Image image = buttonObj.GetComponentsInChildren<Image>().LastOrDefault(); 
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>(); 

            if (image != null)
            {
                image.sprite = building.Icon; 
            }

            if (text != null)
            {
                text.text = building.buildingName; 
            }

            button.onClick.AddListener(() => OnBuildingButtonClick(building));
        }
    }

    void UpdateStatsPanel(
    string level = null,
    string population = null,
    string production = null,
    string food = null,
    float? levelProgress = null)
    {
        levelText.text = !string.IsNullOrWhiteSpace(level) ? level : "";
        populationText.text = !string.IsNullOrWhiteSpace(population) ? population : "";
        productionText.text = !string.IsNullOrWhiteSpace(production) ? production : "";
        foodText.text = !string.IsNullOrWhiteSpace(food) ? food : "";
        if (levelProgress.HasValue)
        {
            levelImg.fillAmount = levelProgress.Value;
            levelImg.color = levelProgress >= 0 ? Color.green : Color.red;
        }
    }

    void OnBuildingButtonClick(BuildingData building)
    {
        if (building.type == BuildingType.Army) newArmy = new DataHexUnitArmy();
        selectedBuilding = Instantiate(building);
        Debug.Log($"Selected building: {building.buildingName}");
    }

    void Build(MainHexCell location)
    {
        Player player = location.dataHexCell.city.dataCity.playerOwner;
        if (selectedBuilding.type == BuildingType.Army)
        {
            //armyTask = new ProductionTask(selectedBuilding, location);
            armyParent.active = true;
            newArmy = new DataHexUnitArmy();
            UpdateArmyPanel(player);
        }
        else
        {
            location.dataHexCell.city.dataCity.Production.ProduceBuilding(selectedBuilding, location);
        }
    }

    void UpdateProductionPanel(Player player)
    {
        // Vyèistit starý obsah UI panelu
        foreach (Transform child in productionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (player == null) return;

        // Získání fronty produkce mìsta
        ProductionQueue productionQueue = selectedCity.dataCity.Production.productionQueue;

        // Pokud je aktivní výroba, zobrazíme ji jako první
        if (productionQueue.currentTask != null)
        {
            BuildingData currentBuilding = productionQueue.currentTask.Building;
            GameObject currentTaskObj = Instantiate(productionButtonPrefab, productionPanel.transform);
            Button currentTaskButton = currentTaskObj.GetComponent<Button>();
            Image currentTaskImage = currentTaskObj.GetComponentsInChildren<Image>().LastOrDefault();
            TextMeshProUGUI currentTaskText = currentTaskObj.GetComponentInChildren<TextMeshProUGUI>();

            if (currentTaskImage != null)
            {
                currentTaskImage.sprite = currentBuilding.Icon;
            }

            if (currentTaskText != null)
            {
                currentTaskText.text = $"(Producing) {currentBuilding.buildingName}";
            }

            currentTaskButton.interactable = false; // Nelze ruènì odstranit aktivní úkol
        }

        // Zobrazit zbývající frontu produkce
        foreach (var buildingTask in productionQueue.queue)
        {
            BuildingData building = buildingTask.Building;
            GameObject buttonObj = Instantiate(productionButtonPrefab, productionPanel.transform);
            Button button = buttonObj.GetComponent<Button>();
            Image image = buttonObj.GetComponentsInChildren<Image>().LastOrDefault();
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (image != null)
            {
                image.sprite = building.Icon;
            }

            if (text != null)
            {
                text.text = building.buildingName;
            }

            button.onClick.AddListener(() => RemoveBuilding(buildingTask));
        }
    }

    ProductionTask armyTask;
    DataHexUnitArmy newArmy = new DataHexUnitArmy();

    void UpdateArmyPanel(Player player)
    {
        buildButtonText.text = "Build " + newArmy.CostOfArmy();
        foreach (Transform child in armyShopPanel.transform)
        {
            Destroy(child.gameObject);
        }
        if (player == null)
        {
            return;
        }

        Dictionary<UnitData, int> unitCount = new Dictionary<UnitData, int>();

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
            Image image = buttonObj.GetComponentInChildren<Image>();
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>(); 
            buttons[0].onClick.AddListener(() => AddUnit(unit));
            buttons[1].onClick.AddListener(() => RemoveUnit(unit));

            if (image != null)
            {
                Debug.Log(image.name);
                image.sprite = unit.Icon;
            }

            if (text != null)
            {
                text.text = unit.name + " " + unitCount[unit]; 
            }
        }
    }

    void AddUnit(UnitData unit)
    {
        if(selectedCity.dataCity.Stats.CalculatePopulation() > newArmy.unitsInArmy.Count)
        {
            newArmy.AddUnit(unit);
            Debug.Log("X " + newArmy.unitsInArmy.Count);
            OnArmyUpdated.Invoke();
        }
    }
    void RemoveUnit(UnitData unit)
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
        newArmy = new DataHexUnitArmy();
        armyParent.active = false;
    }
    public void ButtonConfirm()
    {
        selectedBuilding.army = newArmy;
        Debug.Log("Army2: " + selectedBuilding.army.unitsInArmy[0].name);

        armyParent.active = false;
        Debug.Log(selectedCity.dataCity.Location);
        selectedBuilding.productionCost = newArmy.CostOfArmy();
        selectedCity.dataCity.Production.productionQueue.AddToQueue(selectedBuilding, selectedCity.dataCity.Location);

        newArmy = new DataHexUnitArmy();
    }
    #endregion
}

public enum UnitAtDestination
{
    Enemy, Ally, None
}

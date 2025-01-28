using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ProductionCity
{
    private MainCity mainCity;
    public ProductionQueue productionQueue { get; private set; }

    public ProductionCity(MainCity mainHexUnit)
    {
        this.mainCity = mainHexUnit;
        productionQueue = new ProductionQueue(mainCity);
    }

    public void ProduceBuilding(BuildingData building, MainHexCell location)
    {
        Debug.Log("ProduceXD");
        productionQueue.AddToQueue(building, location);
    }

    public void ProcessTurn(int production)
    {
        Debug.Log($"Processing production for {mainCity}. Production available: {production}");
        productionQueue.ProcessProduction(production);
    }

    public string GetProductionStatus()
    {
        return productionQueue.GetQueueStatus();
    }
}

public class ProductionQueue
{
    MainCity mainCity;

    public Queue<ProductionTask> queue = new Queue<ProductionTask>();
    private ProductionTask currentTask;
    private int remainingProduction;

    public event System.Action OnQueueUpdated;

    public ProductionQueue(MainCity mainCity)
    {
        this.mainCity = mainCity;
    }

    public void AddToQueue(BuildingData building, MainHexCell location)
    {

        queue.Enqueue(new ProductionTask(building, location));
        OnQueueUpdated?.Invoke();
        if (currentTask == null)
        {
            StartNextTask();
        }
    }

    public void RemoveTaskFromQueue(ProductionTask task)
    {
        if (queue.Contains(task))
        {
            queue = new Queue<ProductionTask>(queue.Where(t => t != task));
            OnQueueUpdated?.Invoke();
        }
    }

    private void StartNextTask()
    {
        if (queue.Count > 0)
        {
            currentTask = queue.Dequeue();
            remainingProduction = currentTask.Building.productionCost;
            OnQueueUpdated?.Invoke();
        }
        else
        {
            currentTask = null;
            remainingProduction = 0;
        }
    }

    public void ProcessProduction(int production)
    {
        if (currentTask == null) return;
        Debug.Log($"{remainingProduction} -= {production}");
        remainingProduction -= production;
        
        if (remainingProduction <= 0)
        {
            CompleteCurrentTask();
            StartNextTask();
        }
    }

    public string GetQueueStatus()
    {
        if (currentTask != null)
        {
            return $"Producing {currentTask.Building.buildingName} at {currentTask.Location}. Remaining: {remainingProduction}";
        }

        return queue.Count > 0 ? "Items in queue." : "No items in production.";
    }

    

    private void CompleteCurrentTask()
    {
        Debug.Log($"Completed production of {currentTask.Building.buildingName} at {currentTask.Location}");
        switch (currentTask.Building.type)
        {
            case BuildingType.Farm:
                currentTask.Location.dataHexCell.featuresHexCell.FarmLevel++;
                break;
            case BuildingType.Urban:
                currentTask.Location.dataHexCell.featuresHexCell.UrbanLevel++;
                break;
            case BuildingType.Army:
                MainHexCell cell = currentTask.Location;
                if (cell && !cell.dataHexCell.Unit)
                {
                    Debug.Log("Velikost 1: " + currentTask.Building.army.unitsInArmy[0].name);
                    CivGameManagerSingleton.Instance.hexGrid.AddUnit(
                        mainCity.dataCity.playerOwner.faction.armyUnitStyle[0].GetComponent<MainHexUnit>(), currentTask.Location, Random.Range(0f, 360f), currentTask.Building.army, mainCity.dataCity.playerOwner
                    );
                }
                break;
            default:
                break;
        }
        currentTask = null;
    }


    public int Count()
    {
        return queue.Count;
    }
}

public class ProductionTask
{
    public BuildingData Building { get; private set; }
    public MainHexCell Location { get; private set; }

    public ProductionTask(BuildingData building, MainHexCell location)
    {
        Building = building;
        Location = location;
    }
}

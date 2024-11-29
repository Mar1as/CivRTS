using UnityEngine;
using System.Collections.Generic;

public class ProductionCity
{
    public MainCity mainCity;
    public ProductionQueue productionQueue;

    public ProductionCity(MainCity mainHexUnit)
    {
        this.mainCity = mainHexUnit;
        Awake();
    }

    private void Awake()
    {
        productionQueue = new ProductionQueue();
    }

    public void ProduceItem(IProductionItem item)
    {
        productionQueue.AddToQueue(item);
    }

    public void ProcessTurn(int production)
    {
        Debug.Log("Tah 3 " + production);
        productionQueue.ProcessProduction(production);
    }
}

public class ProductionQueue
{
    public Queue<IProductionItem> queue = new Queue<IProductionItem>();
    public IProductionItem CurrentItem { get; private set; }
    public int RemainingTurns { get; private set; }

    public void AddToQueue(IProductionItem item)
    {
        Debug.Log("ADDED to queue");
        queue.Enqueue(item);
        if (CurrentItem == null)
        {
            StartNextItem();
        }
    }

    public void ProcessProduction(int production)
    {
        Debug.Log("Tah 4 " + RemainingTurns + " " + queue.Count);
        if (CurrentItem == null) return;

        RemainingTurns -= production;

        if (RemainingTurns <= 0)
        {
            CompleteCurrentItem();
            StartNextItem();
        }
    }

    private void StartNextItem()
    {
        if (queue.Count > 0)
        {
            CurrentItem = queue.Dequeue();
            RemainingTurns = CurrentItem.productionCost;
        }
        else
        {
            CurrentItem = null;
            RemainingTurns = 0;
        }
    }

    private void CompleteCurrentItem()
    {
        CurrentItem.OnProductionComplete();
    }
}

public interface IProductionItem
{
    int productionCost { get; }
    MainHexCell location { get; }
    void OnProductionComplete();
}

public class Unit : IProductionItem
{
    public int productionCost { get; private set; }
    public MainHexCell location { get; private set; }
    public MainHexUnit unit { get; private set; }

    public Unit(int cost, MainHexCell location, MainHexUnit unit)
    {
        productionCost = cost;
        this.location = location;
        this.unit = unit;
    }

    public void OnProductionComplete()
    {
        Debug.Log($"Unit has been completed in city {location.name}!");
        CivGameManagerSingleton.Instance.hexGrid.AddUnit(
                DataHexUnit.unitPrefab, location, Random.Range(0f, 360f)
            );

    }
}

public class Building : IProductionItem
{
    public int productionCost { get; private set; }
    public MainHexCell location { get; private set; }
    public BuildingEnum be { get; private set; }

    public Building(int cost, MainHexCell location, BuildingEnum be)
    {
        productionCost = cost;
        this.location = location;
        this.be = be;
    }

    public void OnProductionComplete()
    {
        Debug.Log($"Building has been completed in city {location.name}!");
        if (be == BuildingEnum.Farm)
        {
            location.dataHexCell.featuresHexCell.FarmLevel++;
        }
        else
        {
            location.dataHexCell.featuresHexCell.UrbanLevel++;
        }
    }
}

public enum BuildingEnum
{
    Farm,
    Urban
}
using System.Collections.Generic;
using UnityEngine;

public class CityAI
{
    private MainCity city;
    private Player player;

    public CityAI(MainCity city, Player player)
    {
        this.city = city;
        this.player = player;
    }

    public void ProcessTurn()
    {
        // Decide what to build based on the city's current state
        DecideBuildingConstruction();

        // Decide if the city should expand
        //DecideExpansion();
    }

    private void DecideBuildingConstruction()
    {
        // Get the list of available buildings from the player's faction
        List<BuildingData> availableBuildings = player.faction.availableBuildings;
        if (city.dataCity.Production.productionQueue.Count() > 2)
        {
            return;
        }
        // Prioritize buildings based on the city's needs
        foreach (var building in availableBuildings)
        {
            if (ShouldBuild(building))
            {
                if (building.type == BuildingType.Army)
                {
                    
                    building.army = CreateArmy();
                }
                Debug.Log("Produce: " + building);
                city.dataCity.Production.ProduceBuilding(building, city.dataCity.GetRandomOwnedProvince());
                break; // Build one building per turn
            }
        }
    }

    private bool ShouldBuild(BuildingData building)
    {
        // Simple logic: prioritize food if the city is low on food, otherwise prioritize production or army
        int food = city.dataCity.Stats.CalculateFood();
        int population = city.dataCity.Stats.CalculatePopulation();
        int production = city.dataCity.Stats.CalculateProduction();

        // Check if the city needs more food
        if (food < population * 2 && building.type == BuildingType.Farm)
        {
            return true;
        }

        // Check if the city needs more production
        if (production < 10 && building.type == BuildingType.Urban)
        {
            return true;
        }

        // Check if the city should produce an army
        if (ShouldProduceArmy() && building.type == BuildingType.Army)
        {
            return true;
        }

        return false;
    }

    private bool ShouldProduceArmy()
    {
        // Check if the player has fewer armies than a certain threshold
        int armyCount = player.Armies.Count;
        int cityCount = CivGameManagerSingleton.Instance.allCities.FindAll(c => c.dataCity.playerOwner == player).Count;

        // Produce an army if the player has fewer armies than cities
        Debug.Log($"{armyCount} < {cityCount}");
        if (armyCount < cityCount)
        {
            return true;
        }

        // Alternatively, produce an army if the city has excess resources
        int food = city.dataCity.Stats.CalculateFood();
        int population = city.dataCity.Stats.CalculatePopulation();
        int production = city.dataCity.Stats.CalculateProduction();

        if (food > population * 2 && production > 10 &&population > 4)
        {
            return true;
        }

        return false;
    }

    private DataHexUnitArmy CreateArmy()
    {
        DataHexUnitArmy newArmy = new DataHexUnitArmy();
        List<UnitData> availableUnits = player.faction.availableUnits;

        // Se�adit jednotky podle ceny (nejlevn�j�� prvn�)
        availableUnits.Sort((a, b) => a.productionCost.CompareTo(b.productionCost));

        // Naj�t nejlevn�j�� a nejdra��� jednotku pro �k�lov�n� pravd�podobnosti
        float minCost = availableUnits[0].productionCost;
        float maxCost = availableUnits[availableUnits.Count - 1].productionCost;

        int population = city.dataCity.Stats.CalculatePopulation();

        // P�idat jednotky do arm�dy podle populace
        for (int i = 0; i < population; i++)
        {
            // Vybrat n�hodnou jednotku
            UnitData randomUnit = availableUnits[Random.Range(0, availableUnits.Count)];

            // Normalizovat cenu jednotky (0 = nejlevn�j��, 1 = nejdra���)
            float normalizedCost = (randomUnit.productionCost - minCost) / (maxCost - minCost + 1);

            // Vypo��tat pravd�podobnost p�id�n� (��m dra���, t�m men�� pravd�podobnost)
            float probability = Mathf.Clamp01(1f - normalizedCost);

            // N�hodn� p�idat jednotku na z�klad� pravd�podobnosti
            if (Random.value < probability)
            {
                newArmy.AddUnit(randomUnit);
            }
        }

        return newArmy;
    }

    private void DecideExpansion()
    {
        // Expand the city if it has enough resources
        if (city.dataCity.Stats.levelProgress >= city.dataCity.Stats.levelUpRequirement / 2)
        {
            city.dataCity.ExpandCity();
        }
    }
}
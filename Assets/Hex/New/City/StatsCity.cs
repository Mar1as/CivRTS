using UnityEngine;

public class StatsCity
{
    private MainCity mainCity;
    public int level { get; private set; } = 1;
    public int levelProgress { get; private set; } = 0;
    public int levelUpRequirement { get; private set; } = 5;

    public float productionModifier { get; private set; } = 1.0f;

    public StatsCity(MainCity mainHexUnit)
    {
        this.mainCity = mainHexUnit;
    }

    public void ProcessTurn()
    {
        int foodSurplus = CalculateFood() - CalculatePopulation();
        UpdateLevelProgress(foodSurplus);
        Debug.Log(GetStatsSummary());
    }

    public int CalculatePopulation()
    {
        int population = 0;
        foreach (var cell in mainCity.dataCity.CellsInBorder)
        {
            population += (cell.dataHexCell.featuresHexCell.UrbanLevel + cell.dataHexCell.featuresHexCell.FarmLevel) / 2;
        }
        return population;
    }

    public int CalculateFood()
    {
        int food = 10;
        foreach (var cell in mainCity.dataCity.CellsInBorder)
        {
            food += cell.dataHexCell.featuresHexCell.FarmLevel * 2;
        }
        return food + 1;
    }

    public int CalculateProduction()
    {
        int production = 10;
        foreach (var cell in mainCity.dataCity.CellsInBorder)
        {
            production += cell.dataHexCell.featuresHexCell.UrbanLevel;
        }
        return production + 1;
    }

    private void UpdateLevelProgress(int progress)
    {
        levelProgress += progress;

        if (levelProgress >= levelUpRequirement)
        {
            level++;
            levelUpRequirement *= 2;
            levelProgress = 0;
            mainCity.dataCity.ExpandCity();
        }
        else if (levelProgress < 0 && level > 1)
        {
            level--;
            levelUpRequirement /= 2;
            levelProgress = 0;
            mainCity.dataCity.ShrinkCity();
        }
    }

    public string GetStatsSummary()
    {
        return $"Level: {level}, LevelUp: {levelProgress}/{levelUpRequirement}, Population: {CalculatePopulation()}, Food: {CalculateFood()}, Production: {CalculateProduction()}";
    }
}

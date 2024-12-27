using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class StatsCity
{
    MainCity mainCity;

    public StatsCity(MainCity mainHexUnit)
    {
        this.mainCity = mainHexUnit;
    }

    float productionModifier = 1;

    int level = 1;
    int levelUp = 5;
    int levelProgress = 0;

    int LevelProgress
    {
        get
        {
            return levelProgress;
        }
        set
        {
            productionModifier = 1;
            levelProgress = value;
            if (levelProgress >= levelUp)
            {
                level++;
                levelUp *= 2;

                levelProgress = 0;

                mainCity.dataCity.ExpandCity();
                //Conquest
            }
            else if (levelProgress < 0 && level > 1)
            {
                level--;

                levelUp /= 2;
                levelProgress = 0;

                mainCity.dataCity.ShrinkCity();
                //Deconquest
            }
            else if(levelProgress < 0)
            {
                levelProgress = 0;
                productionModifier = 0;
            }
        }
    }   

    public void Turn()
    {
        Debug.Log("Tah 2");
        LevelProgress += Food - Population;
        int curProduction = Production * (int)productionModifier;
        mainCity.dataCity.productionCity.ProcessTurn(curProduction);
        Debug.Log(StatsToString());
    }

    public int Population
    {
        get
        {
            int population = 0;
            foreach (var cell in mainCity.dataCity.cellsInBorder)
            {
                population += (cell.dataHexCell.featuresHexCell.UrbanLevel + cell.dataHexCell.featuresHexCell.FarmLevel) / 2;
            }
            return population;
        }
    }

    public int Food
    {
        get
        {
            int food = 0;
            foreach (var cell in mainCity.dataCity.cellsInBorder)
            {
                food += cell.dataHexCell.featuresHexCell.FarmLevel * 2;
            }
            return food + 1;
        }
    }

    public int Production
    {
        get
        {
            int production = 0;
            foreach (var cell in mainCity.dataCity.cellsInBorder)
            {
                production += cell.dataHexCell.featuresHexCell.UrbanLevel;
            }
            return production + 1;
        }
    }

    public string StatsToString()
    {
        string x = $"Level: {level} Level Up: {levelUp} Level Progress: {LevelProgress}\nPopulation: {Population} Food: {Food} Production: {Production}";
        Debug.Log(x);
        return x;
    }
}

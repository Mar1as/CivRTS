using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Game/Building")]
public class BuildingData : ScriptableObject
{
    public string buildingName; // Název budovy
    public string description; // Popis budovy
    public int productionCost; // Náklady na výrobu
    public Sprite icon; // Ikona budovy
    public BuildingType type; // Typ budovy (napø. farmy, vojenská)

    public int foodBonus; // Bonus k jídlu
    public int productionBonus; // Bonus k produkci
    public int populationBonus; // Bonus k populaci
}

public enum BuildingType
{
    Army,
    Farm,
    Urban,
    Military,
    Research,
    Other
}

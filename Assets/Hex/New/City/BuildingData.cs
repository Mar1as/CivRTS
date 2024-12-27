using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Game/Building")]
public class BuildingData : ScriptableObject
{
    public string buildingName; // N�zev budovy
    public string description; // Popis budovy
    public int productionCost; // N�klady na v�robu
    public Sprite icon; // Ikona budovy
    public BuildingType type; // Typ budovy (nap�. farmy, vojensk�)

    public int foodBonus; // Bonus k j�dlu
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

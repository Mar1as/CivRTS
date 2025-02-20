using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Game/Building")]
public class BuildingData : ScriptableObject
{
    
    public string buildingName; // N�zev budovy
    public string description; // Popis budovy
    public int productionCost; // N�klady na v�robu
    public Sprite iconBase; // Ikona budovy
    public GameObject model; // 3D model budovy
    public BuildingType type; // Typ budovy (nap�. farmy, vojensk�)

    public int foodBonus = 0; // Bonus k j�dlu
    public int productionBonus = 0; // Bonus k produkci
    public int populationBonus = 0; // Bonus k populaci

    public DataHexUnitArmy army;

    public Sprite Icon
    {
        get { 
            if(iconBase) return iconBase;
            return Sprite.Create(AssetPreview.GetMiniThumbnail(model), new Rect(0, 0, 100, 100), new Vector2(0, 0));
        }
        set { iconBase = value; }
    }

    
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

using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Game/Unit")]
public class UnitData : ScriptableObject
{
    public GameObject unitPrefab;
    public float productionCost;
    public Sprite Icon
    {
        get
        {
            if (unitPrefab.GetComponent<TankUnitStats>() != null) return unitPrefab.GetComponent<TankUnitStats>().icon;
            else if (unitPrefab.GetComponent<UnitStats>() != null) return unitPrefab.GetComponent<UnitStats>().icon;
            else return null;
        }
    }
    public string Name
    {
        get
        {
            if (unitPrefab.GetComponent<TankUnitStats>() != null) return unitPrefab.GetComponent<TankUnitStats>().jmeno;
            else if (unitPrefab.GetComponent<UnitStats>() != null) return unitPrefab.GetComponent<UnitStats>().jmeno;
            else return null;
        }
    }
}

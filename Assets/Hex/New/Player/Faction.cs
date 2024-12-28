using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFaction", menuName = "Game/Faction")]
public class FactionsInCiv : ScriptableObject
{
    public string factionName; // Název frakce
    public List<GameObject> armyUnitStyle;
    public List<GameObject> availableUnits; // Seznam jednotek, které mùže frakce vyrábìt
    public List<BuildingData> availableBuildings; // Seznam budov, které mùže frakce vyrábìt
    public Color factionColor; // Barva frakce (pro vizuální odlišení)

    // Další vlastnosti, napøíklad bonusy nebo speciální schopnosti
    public string description;
}

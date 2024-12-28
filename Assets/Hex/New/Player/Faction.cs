using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFaction", menuName = "Game/Faction")]
public class FactionsInCiv : ScriptableObject
{
    public string factionName; // N�zev frakce
    public List<GameObject> armyUnitStyle;
    public List<GameObject> availableUnits; // Seznam jednotek, kter� m��e frakce vyr�b�t
    public List<BuildingData> availableBuildings; // Seznam budov, kter� m��e frakce vyr�b�t
    public Color factionColor; // Barva frakce (pro vizu�ln� odli�en�)

    // Dal�� vlastnosti, nap��klad bonusy nebo speci�ln� schopnosti
    public string description;
}

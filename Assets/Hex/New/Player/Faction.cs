using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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

    public TextAsset cityNamesFile; // Textov� soubor obsahuj�c� n�zvy m�st

    private List<string> usedCityNames = new List<string>(); // Seznam pou�it�ch jmen

    // Metoda pro z�sk�n� n�hodn�ho m�sta ze souboru
    public string GetRandomCityName()
    {
        if (cityNamesFile != null)
        {
            string[] cityNames = cityNamesFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> availableCityNames = new List<string>(cityNames);

            // Odstran�me ji� pou�it� jm�na
            availableCityNames.RemoveAll(name => usedCityNames.Contains(name));

            if (availableCityNames.Count > 0)
            {
                string randomCity = availableCityNames[UnityEngine.Random.Range(0, availableCityNames.Count)];
                usedCityNames.Add(randomCity); // P�id�me jm�no do seznamu pou�it�ch
                return randomCity;
            }
        }
        return "Unknown City";
    }

    // Metoda pro resetov�n� seznamu pou�it�ch jmen
    public void ResetUsedCityNames()
    {
        usedCityNames.Clear();
    }
}

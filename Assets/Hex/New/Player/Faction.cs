using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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

    public TextAsset cityNamesFile; // Textový soubor obsahující názvy mìst

    private List<string> usedCityNames = new List<string>(); // Seznam použitých jmen

    // Metoda pro získání náhodného mìsta ze souboru
    public string GetRandomCityName()
    {
        if (cityNamesFile != null)
        {
            string[] cityNames = cityNamesFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> availableCityNames = new List<string>(cityNames);

            // Odstraníme již použitá jména
            availableCityNames.RemoveAll(name => usedCityNames.Contains(name));

            if (availableCityNames.Count > 0)
            {
                string randomCity = availableCityNames[UnityEngine.Random.Range(0, availableCityNames.Count)];
                usedCityNames.Add(randomCity); // Pøidáme jméno do seznamu použitých
                return randomCity;
            }
        }
        return "Unknown City";
    }

    // Metoda pro resetování seznamu použitých jmen
    public void ResetUsedCityNames()
    {
        usedCityNames.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CivGameManagerSingleton
{
    //[SerializeField]
    //int numberOfPlayers = 1;
    private static CivGameManagerSingleton instance = null;

    public static CivGameManagerSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CivGameManagerSingleton();
                instance.allUnits = new List<MainHexUnit>();
                instance.allCities = new List<MainCity>();

                instance.pocetVytvoreni++;
            }
            //else Debug.Log("Players count: " + instance.players.Length); nefunguje
            return instance;
        }
    }

    public int pocetVytvoreni = 0;

    public FactionsInCiv[] allFactions;
    public MainHexCell[] hexagons;
    public List<MainHexUnit> allUnits;
    public List<MainCity> allCities;
    public Player[] players;
    public HexGrid hexGrid;
    public SceneSwap sceneSwap;

    public List<string> usedCityNames;

    /*public List<Player> AddPlayers()
    {
        List<Player> players = new List<Player>();
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players.Add(new Player());
        }
        return players;
    }*/

    public void ProcessAITurns()
    {
        foreach (var player in players)
        {
            if (player.ai)
            {
                // Process city AI
                foreach (var city in allCities)
                {
                    if (city.dataCity.playerOwner == player)
                    {
                        CityAI cityAI = new CityAI(city, player);
                        cityAI.ProcessTurn();
                    }
                }

                // Process army AI
                ArmyAI armyAI = new ArmyAI(player);
                armyAI.ProcessTurn();
            }
        }
    }
}

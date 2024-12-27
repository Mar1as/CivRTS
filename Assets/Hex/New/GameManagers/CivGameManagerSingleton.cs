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

            }
            //else Debug.Log("Players count: " + instance.players.Length); nefunguje
            return instance;
        }
    }

    public MainHexCell[] hexagons;
    public List<MainHexUnit> allUnits;
    public List<MainCity> allCities;
    public Player[] players;
    public HexGrid hexGrid;

    /*public List<Player> AddPlayers()
    {
        List<Player> players = new List<Player>();
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players.Add(new Player());
        }
        return players;
    }*/
}

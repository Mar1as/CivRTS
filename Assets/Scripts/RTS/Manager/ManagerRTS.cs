using System.Collections.Generic;
using UnityEngine;

public class ManagerRTS : MonoBehaviour
{
    [SerializeField]
    List<GameObject> spawnPoints;
    List<PlayerRTS> players = new List<PlayerRTS>();

    [SerializeField]
    public ArmyHexUnit temporaryArmy;

    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Setup()
    {
        //Teams.listOfPlayers[0].army = temporaryArmy;
        //Teams.listOfPlayers[1].army = temporaryArmy;

        /*
        for (int i = 0; i < SceneSwap.passInfo.Length; i++)
        {
            //GameObject sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
            if (SceneSwap.passInfo[i] != null)
            {/*
                players.Add(
                    new PlayerRTS(SceneSwap.passInfo[i], 
                    sp
                    ));

                Teams.listOfPlayers[i].army = temporaryArmy;
            }
            //spawnPoints.Remove(sp);
        }*/
    }
}

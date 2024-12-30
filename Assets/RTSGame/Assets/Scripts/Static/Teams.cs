using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teams : MonoBehaviour
{
    //static public List<GameObject> listFriendlyUnits = new List<GameObject>();
    //static public List<GameObject> listEnemyUnits = new List<GameObject>();

    [SerializeField] UiUnitsList uiUnitsListScript; //UI pro hráèe
    [SerializeField] GameObject friendlySpawnPoint, enemySpawnPoint;
    [SerializeField] List<Faction> factions;
    TeamsConstructor friendlyPlayer;
    TeamsConstructor enemyPlayer;
    [Header("AI")]
    [SerializeField] bool EnableAi;
    [SerializeField] List<Vector3> paths;
    [SerializeField] List<Wave> waves; 


    int moneyToGive = 100;

    public static List<TeamsConstructor> listOfPlayers = new List<TeamsConstructor>();

    //[SerializeField] GameObject gm;

    private void OnEnable()
    {
        if(LoadMenu.chosenFactions != null)
        {
            moneyToGive = LoadMenu.money;
            if (LoadMenu.chosenFactions.Count > 0) factions = LoadMenu.chosenFactions;
        }

        friendlyPlayer = new TeamsConstructor(
        friendlySpawnPoint, new List<GameObject>(), "FriendlyPlayer", moneyToGive, Color.green, uiUnitsListScript, factions[0]);
        enemyPlayer = new TeamsConstructor(enemySpawnPoint,new List<GameObject>(), "EnemyPlayer", moneyToGive, Color.red, factions[1], EnableAi, waves);
        listOfPlayers.Add(friendlyPlayer);
        listOfPlayers.Add(enemyPlayer);
        GetUnits();
        //Debug.Log("Count of units in listofplayers0: " + listOfPlayers[0].listUnits.Count);
    }

    private void Update()
    {
        if(waves.Count > 0 && paths.Count > 0) enemyPlayer.Updatos(paths);
        //Debug.Log(listOfPlayers[0].listSelectedUnits.Count);
        //listOfPlayers[0].listUnits.Add(gameObject);
        //listOfPlayers[0].listUnitsAdd(listOfPlayers[0].listUnits, gm);

    }

    void GetUnits()
    {
        //listFriendlyUnits = FindAllPlayerUnits("FriendlyPlayer");
        //listEnemyUnits = FindAllPlayerUnits("EnemyPlayer");

        for (int i = 0; i < listOfPlayers.Count; i++)
        {
            listOfPlayers[i].listUnits = FindAllPlayerUnits(listOfPlayers[i].tag);

        }
    }
    List<GameObject> FindAllPlayerUnits(string tag)
    {
        List<GameObject> listGameObjects = new List<GameObject>();
        GameObject[] allUnits = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject i in allUnits)
        {
            listGameObjects.Add(i);
        }
        return listGameObjects;
    }

    public static void RemoveUnitVoid(GameObject unit)
    {
        Debug.Log("LOL");
        foreach (TeamsConstructor player in listOfPlayers)
        {
            Debug.Log("Remove " + unit.name);
            player.ListSelectedUnits.Remove(unit);
            player.ListUnits.Remove(unit);
            Debug.Log(player.ListSelectedUnits.Count + " " + player.ListUnits.Count);
        }

        for (int i = 0; i < listOfPlayers.Count; i++)
        {
            if (listOfPlayers[i].tag == unit.tag)
            {
                // Shromáždìné jednotky k odstranìní
                List<GameObject> unitsToRemove = new List<GameObject> { unit };
                List<Battalions> battalionsToUpdate = new List<Battalions>();

                // Shromáždìné bataliony k aktualizaci
                foreach (Battalions bat in listOfPlayers[i].listBattalions)
                {
                    if (bat.Units.Contains(unit))
                    {
                        battalionsToUpdate.Add(bat);
                    }
                }

                // Odstranìní jednotek ze seznamu jednotek a vybraných jednotek
                RemoveUnitsFromList(listOfPlayers[i].listUnits, unitsToRemove);
                RemoveUnitsFromList(listOfPlayers[i].listSelectedUnits, unitsToRemove);

                

                // Aktualizace batalionù
                foreach (Battalions bat in battalionsToUpdate)
                {
                    bat.RemoveUnit(unit);
                }

                // AI odstranìní
                if (listOfPlayers[i].ai != null)
                {
                    listOfPlayers[i].ai.RemoveUnitFromBattalion(unit);
                }

                break;
            }
        }
    }

    private static void RemoveUnitsFromList(List<GameObject> list, List<GameObject> unitsToRemove)
    {
        foreach (GameObject unit in unitsToRemove)
        {
            list.Remove(unit);
        }
    }

    public void AddUnitVoid(GameObject unit)
    {
        for (int i = 0; i < listOfPlayers.Count; i++)
        {
            if (listOfPlayers[i].tag == unit.tag)
            {
                //listOfPlayers[i].listUnits.Add(unit);
                listOfPlayers[i].listUnitsAdd(listOfPlayers[i].listUnits, unit);

                break;
            }
        }
    }
}

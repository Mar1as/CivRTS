using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TeamsConstructor : MonoBehaviour
{
    public Vector3 spawnPointVector;
    public List<GameObject> ListUnits = new List<GameObject>();
    public List<GameObject> ListSelectedUnits = new List<GameObject>();
    public List<GameObject> factionShopUnits;
    public UIUnitListCreateButton uiUnitsListCreateButton;
    public UiManager uiManager;

    List<Battalions> ListBattalions = new List<Battalions>();
    public Faction playerFaction;

    public string tag;
    public LayerMask layerMask;
    public int money;
    public int score;
    public int controledZones;
    public Color teamColor;

    public EnemyAi ai = null;


    public List<GameObject> listUnits
    {
        get { return ListUnits; }
        set
        {
            ListUnits = value;
        }
    }

    public List<GameObject> listSelectedUnits
    {
        get { return ListSelectedUnits; }
        set
        {
            ListSelectedUnits = value;
        }
    }

    public List<Battalions> listBattalions
    {
        get { return ListBattalions; }
        set
        {
            ListBattalions = value;
            if (uiUnitsListCreateButton != null)
            {
                uiUnitsListCreateButton.UpdatePanel();
            }
        }
    }

    public void AddBattalion(Battalions battalion)
    {
        if (!ListBattalions.Contains(battalion))
        {
            ListBattalions.Add(battalion);

            listBattalions.OrderBy(o => o.Units.Count);
            if (uiUnitsListCreateButton != null)
            {
                uiUnitsListCreateButton.UpdatePanel();
            }
        }
    }

    public void AddListSelectedUnits(GameObject gm)
    {

    }

    public TeamsConstructor(GameObject spawnPointGM, List<GameObject> listUnits, string tag, int money, Color color, Faction playerFaction, bool enable, List<Wave> waves) //AI konstruktor
    {
        
        this.spawnPointVector = spawnPointGM.transform.position;
        this.listUnits = listUnits;
        this.tag = tag;
        this.money = money;
        layerMask = LayerMask.GetMask(tag);
        score = 0;
        controledZones = 0; 
        teamColor = color;
        this.playerFaction = playerFaction;
        factionShopUnits = playerFaction.factionUnits;
        if (enable && waves.Count <= 0)
        {
            ai = new EnemyAi(this);
            ManagerAi mn = spawnPointGM.AddComponent<ManagerAi>();
            mn.ai = ai;
        }

        if (waves.Count > 0)
        {
            money = -10000;
            this.waves = waves;
        }
        

    }

    public TeamsConstructor(GameObject spawnPointGM, List<GameObject> listUnits, string tag, int money, Color color, UiUnitsList uiUnitsListScript, Faction playerFaction)//Hráèský konstruktor kterému se pøidává UI
    {
        //uiUnitsList = GetUiUnitsListScript(Ui);
        this.spawnPointVector = spawnPointGM.transform.position;
        this.listUnits = listUnits;
        this.tag = tag;
        this.money = money;
        layerMask = LayerMask.GetMask(tag);
        score = 0;
        controledZones = 0;
        teamColor = color;
        this.playerFaction = playerFaction;
        factionShopUnits = playerFaction.factionUnits;
        uiUnitsListCreateButton = new UIUnitListCreateButton(listUnits, listSelectedUnits, listBattalions, Input.GetKey(KeyCode.LeftShift));
        if(uiManager == null) uiManager = new UiManager(this);
        uiManager.uiShop.CreateShop(factionShopUnits);
    }

    UiUnitsList GetUiUnitsListScript(GameObject Ui)
    {
        foreach (GameObject child in Ui.transform)
        {
            if (child.GetComponent<UiUnitsList>())
            {
                return child.GetComponent<UiUnitsList>();
            }
        }
        return null;
    }

    public void listUnitsAdd(List<GameObject> list, GameObject gameObject)
    {
        list.Add(gameObject);
    }

    public void listUnitsRemove(List<GameObject> list, GameObject gameObject)
    {
        list.Remove(gameObject);
    }

    List<Wave> waves;
    float delayCur = 0;
    float delayMax = 30f;
    int currentWave = 0;
    List<DestinationsXd> dest = new List<DestinationsXd>();

    public void Updatos(List<Vector3> paths)
    {
        if(waves.Count > 0)
        {
            Vector3 finalDest = AllFlags.Instance.flags[0].transform.position;

            if (delayCur < 0)
            {

                delayCur = delayMax;
                SpawnWave(paths);
            }
            else delayCur -= Time.deltaTime;

            for (int i = 0; i < dest.Count; i++)
            {
                DestinationsXd item = dest[i];
                if (item.unit.IsDestroyed())
                {
                    dest.Remove(item);
                    i--;
                }
                else
                {
                    if (Vector3.Distance(item.unit.transform.position, item.destination) < 1)
                    {
                        dest.Remove(item);
                        item.unit.gameObject.GetComponent<NavMeshAgent>().destination = finalDest;
                    }
                }
            }
        }
    }

    void SpawnWave(List<Vector3> paths)
    {
        List<GameObject> newUnits = new List<GameObject>();
        Debug.Log("Spawn " + currentWave);
        Vector3 finalDest = AllFlags.Instance.flags[0].transform.position;

        if(currentWave == waves.Count) currentWave = waves.Count - 1;
        foreach (WaweUnit item in waves[currentWave].units)
        {
            for (int i = 0; i < item.count; i++)
            {
                item.unit.layer = 7;
                item.unit.tag = tag;
                GameObject unit = Instantiate(item.unit,spawnPointVector,Quaternion.identity);
                newUnits.Add(unit);
            }
        }
        foreach (GameObject item in newUnits)
        {
            Vector3 randomPath = paths[Random.Range(0, paths.Count)];
            item.GetComponent<NavMeshAgent>().destination = randomPath;
            dest.Add(new DestinationsXd { unit = item, destination = randomPath });
        }

        currentWave++;
    }

    #region Army
    public ArmyHexUnit Army;

    public ArmyHexUnit army
    {
        get { return Army; }
        set
        {
            Army = value;
            if (uiUnitsListCreateButton != null)
            {
                if (uiManager == null)
                {
                    uiManager = new UiManager(this);
                }
                uiManager.armyUiShop = new ArmyUiShop(this);
                Debug.Log("JUP");
                uiManager.armyUiShop.UpdateShop();
            }
        }
    }

    #endregion
}

class DestinationsXd
{
    public GameObject unit;
    public Vector3 destination;
}

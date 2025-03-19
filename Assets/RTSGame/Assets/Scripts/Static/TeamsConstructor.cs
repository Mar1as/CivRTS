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

    public PassInformation infoFromCiv;

    public string tag;
    public LayerMask layerMask;
    public int money;
    public int score;
    public int controledZones;
    public Color teamColor;

    public EnemyAi ai = null;

    bool isAi = false;

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

    public TeamsConstructor(PassInformation infoFromCiv, GameObject spawnPointGM, List<GameObject> listUnits, string tag, int money, Color color, Faction playerFaction, bool enable, List<Wave> waves) //AI konstruktor
    {
        this.infoFromCiv = infoFromCiv;
        isAi = true;
        this.spawnPointVector = spawnPointGM.transform.position;
        this.listUnits = listUnits;
        this.tag = tag;
        this.money = money;
        layerMask = LayerMask.GetMask(tag);
        score = 0;
        controledZones = 0;
        teamColor = infoFromCiv.player.faction.factionColor;
        playerFaction.factionUnits = UnitsFromCiv(infoFromCiv).ToList();
        this.playerFaction = playerFaction;
        factionShopUnits = playerFaction.factionUnits;
        /*
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
        }*/

        Debug.Log($"AI: {infoFromCiv.player.faction} {infoFromCiv.player.ai} {infoFromCiv.army.unitsInArmy[0].name}");

        rndMax = factionShopUnits.Count / 4;
        if(rndMax < 1) rndMax = 1;
    }

    public TeamsConstructor(PassInformation infoFromCiv, GameObject spawnPointGM, List<GameObject> listUnits, string tag, int money, Color color, UiUnitsList uiUnitsListScript, Faction playerFaction)//Hráèský konstruktor kterému se pøidává UI
    {
        this.infoFromCiv = infoFromCiv;
        //uiUnitsList = GetUiUnitsListScript(Ui);
        this.spawnPointVector = spawnPointGM.transform.position;
        this.listUnits = listUnits;
        this.tag = tag;
        this.money = money;
        layerMask = LayerMask.GetMask(tag);
        score = 0;
        controledZones = 0;
        teamColor = infoFromCiv.player.faction.factionColor;
        playerFaction.factionUnits = UnitsFromCiv(infoFromCiv).ToList();
        this.playerFaction = playerFaction;
        factionShopUnits = playerFaction.factionUnits;
        Debug.Log("PO4ET " + factionShopUnits.Count);
        uiUnitsListCreateButton = new UIUnitListCreateButton(listUnits, listSelectedUnits, listBattalions, Input.GetKey(KeyCode.LeftShift));
        if (uiManager == null) uiManager = new UiManager(this, infoFromCiv.army);
        uiManager.uiShop.CreateShop();

        Debug.Log($"Player: {infoFromCiv.player.faction} {infoFromCiv.player.ai} {infoFromCiv.army.unitsInArmy[0].name}");

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
    float delayMax = 20f;
    int currentWave = 0;
    List<DestinationsXd> dest = new List<DestinationsXd>();

    int rndMax = 0;

    public void Updatos(List<PathWay> paths)
    {
        Debug.Log("Army update");
        if (paths.Count == 0) return;

        if (true)
        {
            Vector3 finalDest = AllFlags.Instance.flags[0].transform.position;

            if (delayCur < 0)
            {
                delayCur = delayMax;
                SpawnWave(paths);
            }
            else
            {
                delayCur -= Time.deltaTime;
            }

            // Aktualizace pohybu jednotek po cestì
            for (int i = 0; i < dest.Count; i++)
            {
                DestinationsXd item = dest[i];
                if (item.unit == null || item.unit.IsDestroyed())
                {
                    dest.RemoveAt(i);
                    i--;
                }
                else
                {
                    // Kontrola vzdálenosti k aktuálnímu cíli
                    if (Vector3.Distance(item.unit.transform.position, item.currentDestination) < 1f)
                    {
                        // Pøesun na další bod v cestì
                        item.currentPathIndex++;

                        if (item.currentPathIndex < item.pathWay.path.Count)
                        {
                            item.currentDestination = item.pathWay.path[item.currentPathIndex];
                            item.unit.GetComponent<NavMeshAgent>().destination = item.currentDestination;
                        }
                        else
                        {
                            // Konec cesty - jednotka dosáhla finálního cíle
                            dest.RemoveAt(i);
                            i--;
                            item.unit.GetComponent<NavMeshAgent>().destination = finalDest;
                        }
                    }
                }
            }
        }
    }

    void SpawnWave(List<PathWay> paths)
    {
        Debug.Log("PO4ET " + factionShopUnits.Count);

        List<GameObject> newUnits = new List<GameObject>();
        Debug.Log("Spawn " + currentWave);

        // Nákup jednotek pro tuto vlnu
        List<GameObject> purchasedUnits = PurchaseWaveUnits();

        foreach (GameObject unitPrefab in purchasedUnits)
        {
            GameObject unit = Instantiate(unitPrefab, spawnPointVector, Quaternion.identity);
            unit.layer = 7;
            unit.tag = tag;
            newUnits.Add(unit);

            // Nastavení prvního cíle v cestì
            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
            if (paths.Count > 0)
            {
                // Náhodný výbìr cesty pro jednotku
                PathWay selectedPath = paths[Random.Range(0, paths.Count)];
                if (selectedPath.path.Count > 0)
                {
                    agent.destination = selectedPath.path[0];
                    dest.Add(new DestinationsXd
                    {
                        unit = unit,
                        currentPathIndex = 0,
                        currentDestination = selectedPath.path[0],
                        pathWay = selectedPath
                    });
                }
            }
        }

        currentWave++;
    }


    List<GameObject> PurchaseWaveUnits()
    {
        List<GameObject> purchasedUnits = new List<GameObject>();
        int maxUnitsToBuy = Random.Range(1, rndMax); // Náhodný poèet jednotek ve vlnì
        if(rndMax > factionShopUnits.Count) maxUnitsToBuy = factionShopUnits.Count;

        int totalCost = 0;

        for (int i = 0; i < maxUnitsToBuy; i++)
        {
            if (factionShopUnits.Count == 0)
            {
                Debug.LogWarning("AI nemá dostupné jednotky k nákupu.");
                break;
            }

            // Náhodný výbìr jednotky
            GameObject unitToBuy = factionShopUnits[Random.Range(0, factionShopUnits.Count)];
            UnitStats unitStats = unitToBuy.GetComponent<UnitStats>();

            if (unitStats == null || money < totalCost + unitStats.cost)
            {
                continue;
            }

            // Pøidání jednotky a odeètení nákladù
            totalCost += unitStats.cost;
            purchasedUnits.Add(unitToBuy);
        }

        money -= totalCost;
        Debug.Log($"AI koupilo {purchasedUnits.Count} jednotek za {totalCost}");
        return purchasedUnits;
    }

    #region Army
    public DataHexUnitArmy Army;

    public DataHexUnitArmy army
    {
        get { return Army; }
        set
        {
            Army = value;
            if (uiUnitsListCreateButton != null)
            {
                if (uiManager == null)
                {
                    uiManager = new UiManager(this, infoFromCiv.army);
                }
                uiManager.armyUiShop = new ArmyUiShop(this);
                Debug.Log("JUP");
                uiManager.armyUiShop.UpdateShop();
            }
        }
    }

    GameObject[] UnitsFromCiv(PassInformation index)
    {
        GameObject[] units = new GameObject[index.army.unitsInArmy.Count()];
        for (int i = 0; i < index.army.unitsInArmy.Count(); i++)
        {
            units[i] = index.army.unitsInArmy[i].unitPrefab;
        }
        return units;
    }

    #endregion
}

class DestinationsXd
{
    public GameObject unit;
    public int currentPathIndex; // Sledujeme aktuální index v cestì
    public Vector3 currentDestination;
    public PathWay pathWay; // Reference na celou cestu
}

[System.Serializable]
public class PathWay
{
    public List<Vector3> path; // Seznam bodù na cestì
}

[System.Serializable]
public class WaweUnit
{
    public GameObject unit; // Prefab jednotky
    public int count; // Poèet jednotek
}

[System.Serializable]
public class Wave
{
    public List<WaweUnit> units; // Jednotky ve vlnì
    public float cooldown; // Èekací doba pøed další vlnou
}

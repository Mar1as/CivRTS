using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class EnemyAi
{
    TeamsConstructor teamsConstructor;
    List<SimpleZone> allFlags;
    Vector3 beginPostition;
    Orders orders = new Orders();

    Osobnost osobnost;
    Osobnost osobnostCur;

    int pocetZon = 2;
    List<Battalions> battalions = new List<Battalions>();

    List<GameObject> enemyUnits = new List<GameObject>();

    public EnemyAi(TeamsConstructor teamsConstructor)
    {
        this.teamsConstructor = teamsConstructor;
        allFlags = AllFlags.Instance.flags;
        beginPostition = teamsConstructor.spawnPointVector;

        var v = Enum.GetValues(typeof(Osobnost));
        osobnost = (Osobnost)v.GetValue(UnityEngine.Random.Range(0, v.Length));
        osobnostCur = osobnost;
        
    }

    float timeMax = 10;
    float timeCurrent = 0;
    public void Update()
    {
        timeCurrent += Time.deltaTime;
        if (timeCurrent > timeMax)
        {
            timeCurrent = 0;

            AttackOrDefend();

            Buy();
        }
    }

    void Buy()
    {
        int money = teamsConstructor.money;
        List<GameObject> koupitelne = Shop.Instance.listOfUnits;
        //Shop.Instance.Shoping(teamsConstructor, money);
        int pocet = 1 + teamsConstructor.listUnits.Count / 10;
        int rnd;
        if (EnemyHasTank() || enemyUnits.Count + (enemyUnits.Count / 10) < teamsConstructor.ListUnits.Count)
        {
            osobnostCur = Osobnost.Tank;
        }
        if (osobnostCur == Osobnost.Tank)
        {
            NajitKupovanyTypJednotky(new TankCanon(), 1);
        }
        else if (osobnostCur == Osobnost.Agresivni)
        {
            NajitKupovanyTypJednotky(new SMG(), pocet);
        }
        else if (osobnostCur == Osobnost.Defenzivni)
        {
            NajitKupovanyTypJednotky(new Rifle(), pocet);
        }
        else if(osobnostCur == Osobnost.Vyvazeny)
        {
            rnd = UnityEngine.Random.Range(0, 2);
            if (rnd == 0) NajitKupovanyTypJednotky(new Rifle(), pocet);
            if (rnd == 1) NajitKupovanyTypJednotky(new SMG(), pocet);
            if (rnd == 2) NajitKupovanyTypJednotky(new MachineGun(), pocet);
        }
        osobnostCur = osobnost;
    }

    void AttackOrDefend()
    {
        int aiScore = teamsConstructor.score;
        int playerScore = Teams.listOfPlayers[0].score;

        List<SimpleZone> zones;

        if (true) //Attack
        {
            zones = NearestUncapturedZones(pocetZon);
        }
        else //Defend
        {
            zones = FarestCapturedZones(pocetZon);
        }
        if (zones.Count > 0)
        {
            OrganizeByDistance(zones);
            MoveUnits(zones);
        }
    }

    void OrganizeByDistance(List<SimpleZone> zones)
    {
        for (int i = 0; i < zones.Count; i++)
        {
            if (battalions.Count < zones.Count) battalions.Add(new Battalions(battalions));
        }

        for (int i = 0; i < zones.Count - 1; i++)
        {
            for (int j = 0; j < teamsConstructor.listUnits.Count; j++)
            {
                if (Vector3.Distance(teamsConstructor.listUnits[j].transform.position, zones[i].transform.position) < Vector3.Distance(teamsConstructor.listUnits[j].transform.position, zones[i + 1].transform.position))
                {
                    if (!battalions[i].Units.Contains(teamsConstructor.listUnits[j]))
                    {
                        battalions[i].Units.Add(teamsConstructor.listUnits[j]); //TODO: Odebrat z listu, po znièení jednotky
                    }
                }
                else
                {
                    if (!battalions[i + 1].Units.Contains(teamsConstructor.listUnits[j]))
                    {
                        battalions[i + 1].Units.Add(teamsConstructor.listUnits[j]); //TODO: Odebrat z listu, po znièení jednotky
                    }
                }
            }
        }
    }   

    void MoveUnits(List<SimpleZone> zones)
    {
        orders.MoveSelector(zones[0].transform.position, zones[0].transform.position, battalions[0].Units, 1);
        orders.MoveSelector(zones[1].transform.position, zones[1].transform.position, battalions[1].Units, 1);
        /*
        for (int i = 0; i < battalions.Count; i++)
        {
            orders.MoveSelector(zones[i].transform.position, battalions[i].Units, 1, zones[i].transform.position);
        }*/
    }


    List<SimpleZone> NearestUncapturedZones(int pocet) //Útok 
    {
        int[] idecka = new int[100];
        List<SimpleZone> nearestZones = new List<SimpleZone>();
        for (int j = 0; j < pocet; j++)
        {
            SimpleZone currentZone = null;
            for (int i = 0; i < allFlags.Count; i++)
            {
                if (!TeamsConstructor.ReferenceEquals(allFlags[i].zoneStats.currentControl, teamsConstructor))
                {
                    if (currentZone == null)
                    {
                        currentZone = allFlags[i];
                        idecka[j] = i;
                    }
                    else if (Vector3.Distance(allFlags[i].transform.position, beginPostition) < Vector3.Distance(currentZone.transform.position, beginPostition))
                    {
                        if (!KontrolaZony(idecka, i))
                        {
                            currentZone = allFlags[i];
                            idecka[j] = i;
                        }
                    }
                }
            }
            nearestZones.Add(currentZone);
        }

        return nearestZones;
    }//Možná chyba ve vytváøení listu a pøidávání do listu?
    List<SimpleZone> FarestCapturedZones(int pocet) //Bránìní
    {
        int[] idecka = new int[100];
        List<SimpleZone> farestZones = new List<SimpleZone>();
        for (int j = 0; j < pocet; j++)
        {
            SimpleZone currentZone = null;
            for (int i = 0; i < allFlags.Count; i++)
            {
                if (TeamsConstructor.ReferenceEquals(allFlags[i].zoneStats.currentControl, teamsConstructor))
                {
                    if (currentZone == null)
                    {
                        currentZone = allFlags[i];
                        idecka[j] = i;
                    }
                    else if (Vector3.Distance(allFlags[i].transform.position, beginPostition) < Vector3.Distance(currentZone.transform.position, beginPostition))
                    {
                        if (!KontrolaZony(idecka, i))
                        {
                            currentZone = allFlags[i];
                            idecka[j] = i;
                        }
                    }
                }
            }
            farestZones[j] = currentZone;
        }

        return farestZones;
    }
    bool KontrolaZony(int[] idecka, int zonaId )
    {
        for (int i = 0; i < idecka.Length; i++)
        {
            if (idecka[i] == zonaId)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveUnitFromBattalion(GameObject unit)
    {
        bool breakOuterLoop = false;
        for (int i = 0; i < battalions.Count; i++)
        {
            for (int j = 0; j < battalions[i].Units.Count; j++)
            {
                if (battalions[i].Units[j] == unit)
                {
                    battalions[i].Units.RemoveAt(j);
                    breakOuterLoop = true;
                    break;
                }
            }
            if (breakOuterLoop)
                break;
        }
    }

    bool EnemyHasTank()
    {
        enemyUnits = Teams.listOfPlayers[0].listUnits;
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (enemyUnits[i].GetComponent<TankUnitBehaviour>())
            {
                return true;
            }
        }
        return false;
    }

    void NajitKupovanyTypJednotky(Weapon w, int pocet)
    {
        List<GameObject> koupitelne = teamsConstructor.factionShopUnits;
        int idFound = -1;
        for (int i = 0; i < koupitelne.Count; i++)
        {
            Type unitType = koupitelne[i].gameObject.GetComponent<UnitStats>().weapon.GetType();
            if (unitType == w.GetType())
            {
                if (koupitelne[i].gameObject.GetComponent<UnitStats>().costCur * pocet < teamsConstructor.money)
                {
                    idFound = i;
                    break;
                }
            }
        }
        if (idFound == -1)
        {
            for (int i = 0; i < koupitelne.Count; i++)
            {
                if (koupitelne[i].gameObject.GetComponent<UnitStats>().costCur * pocet < teamsConstructor.money)
                {
                    idFound = i;
                    break;
                }
            }
        }
        if (idFound != -1)
        {
            for (int i = 0; i < pocet; i++)
            {
                Shop.Instance.ShopingByListUnits(teamsConstructor.tag, idFound, koupitelne);
            }
        }
    }




    enum Osobnost
    {
        Agresivni,
        Defenzivni,
        Vyvazeny,
        Tank
    }
}

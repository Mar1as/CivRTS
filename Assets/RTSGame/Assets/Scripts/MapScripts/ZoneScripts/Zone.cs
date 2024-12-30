using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Zone : MonoBehaviour
{/*
    TeamsConstructor ControledByPlayer = null;

    ZoneConstructor[] pole;
    float status = 0;

    void Start()
    {
        pole = new ZoneConstructor[Teams.listOfPlayers.Count];
        for (int i = 0; i < Teams.listOfPlayers.Count; i++)
        {
            pole[i] = new ZoneConstructor(Teams.listOfPlayers[i]);
        }
    }

    void Update()
    {
        PreCheckUnitsInZone();
    }

    void PreCheckUnitsInZone()
    {
        for (int i = 0; i < pole.Length; i++)
        {
            CheckUnitsInZone(pole[i].teamConstructor, i);
        }
        HowManyTeamsInZone();

    }

    void CheckUnitsInZone(TeamsConstructor team,int index)
    {
        int maxColliders = 1;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 10, hitColliders, team.layerMask);

        if (numColliders > 0)
        {
            pole[index].isInZone = true;
        }
        else
        {
            pole[index].isInZone = false;
        }
    }

    void HowManyTeamsInZone()
    {
        int NumberOfBools = 0;
        for (int i = 0; i < pole.Length; i++)
        {
            if (pole[i].isInZone == true)
            {
                NumberOfBools++;
            }
        }
        if (NumberOfBools == 0)
        {
            NoneTeamInZone();
        }
        else if (NumberOfBools == 1)
        {
            OneTeamInZone();
        }

        ChangeControlV2();

    }
    void ChangeControl()
    {
        if (status < 100)
        {
            ControledByPlayer = null;
            if (status > 0)
            {
                status -= Time.deltaTime;
            }
        }

        if (status > -1)
        {
            foreach (ZoneConstructor item in pole)
            {
                if (item.status > 99)
                {
                    Debug.Log("xd");
                    ControledByPlayer = item.teamConstructor;
                }
            }
        }
    }
    void NoneTeamInZone()
    {
        foreach (ZoneConstructor item in pole)
        {
            if (item.isInZone == false && item.teamConstructor != ControledByPlayer)
            {
                item.status -= Time.deltaTime;
            }
        }
    }
    void OneTeamInZone()
    {
        foreach (ZoneConstructor item in pole)
        {
            if (item.isInZone == true)
            {
                if (item.status < 100)
                {
                    Debug.Log(item.status);
                    item.status += Time.deltaTime * 10;
                }
                if (ControledByPlayer != item.teamConstructor && ControledByPlayer != null)
                {
                    ControledByPlayer = null;
                }
            }
        }
    }

    void ChangeControlV2()
    {
        float biggestStatus = 0;

        foreach (ZoneConstructor item in pole)
        {
            if (item.status > biggestStatus)
            {
                biggestStatus = item.status;
                if (item.status > 99)
                {
                    ControledByPlayer = item.teamConstructor;
                }
            }
        }
        if (biggestStatus < 99)
        {
            ControledByPlayer = null;
        }
    }
    */
}

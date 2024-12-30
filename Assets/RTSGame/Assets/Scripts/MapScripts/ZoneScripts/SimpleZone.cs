using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SimpleZone : MonoBehaviour
{
    List<ZoneConstructor> listZoneConstructors = new List<ZoneConstructor>(); //List informací o této zónì´pošlu všem hráèùm
    int numberInZone; // poèet týmù v zónì
    //public TeamsConstructor currentControl = null;
    public ZoneStats zoneStats = new ZoneStats();

    [SerializeField] public float radius = 10; //polomìr zóny

    void Start()
    {
        ChangeGraphics(Color.gray);
        for (int i = 0; i < Teams.listOfPlayers.Count; i++)
        {
            listZoneConstructors.Add(new ZoneConstructor(Teams.listOfPlayers[i],this)); //každému hráèi/týmu pøiøadím zónu
        }

        AllFlags.Instance.flags.Add(this);
    }
    

    void Update()
    {
        numberInZone = 0;
        for (int i = 0; i < listZoneConstructors.Count; i++)
        {
            CheckUnitsInZone(listZoneConstructors[i].teamConstructor,i); //spoèítám kolik je v zónì TÝMÙ
        }
        
        ChangeControl();
    }

    void CheckUnitsInZone(TeamsConstructor team, int index)
    {
        int maxColliders = 1;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, team.layerMask);

        if (numColliders > 0)
        {
            listZoneConstructors[index].isInZone = true;
            numberInZone++;
        }
        else
        {
            listZoneConstructors[index].isInZone = false;
        }
    }

    void ChangeControl()
    {
        if (numberInZone == 1) //pokud je v zónì jenom jeden tým
        {
            for (int i = 0; i < listZoneConstructors.Count; i++)
            {
                if (listZoneConstructors[i].isInZone == true)
                {
                    ClearIsInZone();
                    listZoneConstructors[i].IsZoneControledByTeam = true;
                    ChangeGraphics(listZoneConstructors[i].teamConstructor.teamColor);
                    break;
                }
            }
        }
        else if (numberInZone >= 2)
        {
            ClearIsInZone();
            ChangeGraphics(Color.gray);
        }
    }

    void ClearIsInZone()
    {
        foreach (ZoneConstructor item in listZoneConstructors)
        {
            item.IsZoneControledByTeam = false;
        }
    }


    void ChangeGraphics(Color color)
    {
        var main = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().main;
        var shape = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().shape;

        main.startColor = color;
        shape.radius = radius;
    }
}


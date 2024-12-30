using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class ZoneConstructor : MonoBehaviour
{
    //Informace pro keždeho hraèe pro jednu zónu 
    public TeamsConstructor teamConstructor;
    public bool isInZone;
    bool isZoneControledByTeam;

    SimpleZone simpleZone;

    public ZoneConstructor(TeamsConstructor teamConstructor, SimpleZone simpleZone)
    {
        this.teamConstructor = teamConstructor;
        isInZone = false;
        isZoneControledByTeam = false;
        this.simpleZone = simpleZone;

    }

    public bool IsZoneControledByTeam
    {
        get { return isZoneControledByTeam; }
        set
        {
            if (isZoneControledByTeam != value)
            {
                isZoneControledByTeam = value;
                if (value == true)
                {
                    teamConstructor.controledZones++;
                    simpleZone.zoneStats.currentControl = teamConstructor;
                }
                else
                {
                    teamConstructor.controledZones--;

                }
            }
        }
    }
}

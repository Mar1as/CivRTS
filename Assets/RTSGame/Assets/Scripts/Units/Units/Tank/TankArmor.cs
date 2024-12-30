using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankArmor
{
    TankUnitStats tankUnitStats;

    int armorFront;
    int armorSide;
    int armorBack;

    Vector3 point;

    public TankArmor(TankUnitStats tankUnitStats)
    {
        this.tankUnitStats = tankUnitStats;

        armorFront = tankUnitStats.armorFront;
        armorSide = tankUnitStats.armorSide;
        armorBack = tankUnitStats.armorBack;
    }

    public bool CheckArmor(Vector3 point, int piercing)
    {
        Vector3 dir = point - tankUnitStats.transform.position;
        float angle = Vector3.Angle(dir, tankUnitStats.transform.forward);
        if (angle < 30)
        {
            if (piercing >= armorFront)
            {
                return true;
            }
        }
        else if (angle < 150)
        {
            if (piercing >= armorSide)
            {
                return true;
            }
        }
        else
        {
            if (piercing >= armorBack)
            {
                return true;
            }
        }
        return false;
    }
}

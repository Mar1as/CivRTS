using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Weapon
{
    protected virtual float ReturnIfIsMoving()
    {
        Debug.Log(unitStats.agent.velocity.magnitude);
        if (unitStats.agent.velocity.magnitude > 1)
        {

            return 4;
        }
        return 1;
    }
}

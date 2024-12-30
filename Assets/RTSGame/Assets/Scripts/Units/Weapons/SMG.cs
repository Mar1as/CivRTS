using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG : Weapon
{
    protected virtual float ReturnIfIsMoving()
    {
        if (unitStats.agent.velocity.magnitude > 1)
        {

            return 1.1f;
        }
        return 1;
    }
}

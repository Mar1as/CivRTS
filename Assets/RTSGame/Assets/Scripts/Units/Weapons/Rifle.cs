using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    protected override float ReturnIfIsMoving()
    {
        if (unitStats.agent.velocity.magnitude > 1)
        {

            return 2;
        }
        return 0.9f;
    }
}

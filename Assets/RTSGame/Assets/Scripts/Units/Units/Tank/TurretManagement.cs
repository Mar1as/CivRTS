using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretManagement
{
    UnitBehaviour tankUnitBehaviour;
    float downRotation;
    float upRotation;

    GameObject targetos;
    UnitStats unitStats;

    public TurretManagement(UnitBehaviour tankUnitBehaviour, float downRotation, float upRotation)
    {
        this.tankUnitBehaviour = tankUnitBehaviour;
        this.downRotation = Mathf.Abs(downRotation);
        this.upRotation = Mathf.Abs(upRotation);
    }

    public void Update()
    {
        RotateTurretV2();
    }


    void RotateTurretV2()
    {
        if (targetos != tankUnitBehaviour.target)
        {
            if (tankUnitBehaviour.target != null)
            {

                targetos = tankUnitBehaviour.target;
                unitStats = targetos.GetComponent<UnitStats>();
            }
        }

        if (targetos != null)
        {
            GameObject turret = tankUnitBehaviour.unitStats.spine;
            GameObject mantlet = tankUnitBehaviour.unitStats.mantlet;
            GameObject podvozek = tankUnitBehaviour.transform.GetChild(0).gameObject;

            Vector3 enemy = unitStats.spine.transform.position;

            // Calculate direction from turret to enemy
            Vector3 directionToEnemy = enemy - turret.transform.position;
            directionToEnemy = Vector3.RotateTowards(turret.transform.forward, directionToEnemy, 0.1f * Time.deltaTime, 1f);

            // Rotate turret to face the enemy
            Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy, podvozek.transform.up);
            Quaternion localLookRotation = Quaternion.Inverse(podvozek.transform.rotation) * lookRotation;
            turret.transform.localRotation = Quaternion.Euler(0, localLookRotation.eulerAngles.y, 0);

            directionToEnemy = enemy - mantlet.transform.position;
            directionToEnemy = Vector3.RotateTowards(mantlet.transform.forward, directionToEnemy, 0.1f * Time.deltaTime, 1f);

            lookRotation = Quaternion.LookRotation(directionToEnemy, turret.transform.up);
            localLookRotation = Quaternion.Inverse(turret.transform.rotation) * lookRotation;

            float verticalRot = Mathf.Abs(localLookRotation.eulerAngles.x);

            mantlet.transform.localRotation = Quaternion.Euler(localLookRotation.eulerAngles.x, 0, 0);

            if ((360 - verticalRot) < upRotation && verticalRot < downRotation)
            {
                mantlet.transform.localRotation = Quaternion.Euler(localLookRotation.eulerAngles.x, 0, 0);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Destroyer : TankUnitBehaviour
{
    [SerializeField] float turretSpeed, maxAngle;

    [SerializeField] GameObject turret, mantlet;

    Vector3 startMantletDir;
    override protected void Start()
    {
        base.Start();
        turret = unitStats.spine;
        mantlet = unitStats.mantlet;

        startMantletDir = mantlet.transform.forward.normalized;
    }

    override protected void Update()
    {
        IsMoving();
        ShootV2();
        RotateTank();
        tankMovement.Update();
        RotateTurret();
    }

    void RotateTurret()
    {
        if (target == null) return;
        startMantletDir = gameObject.transform.forward.normalized;
        Vector3 mantletForward = (mantlet.transform.forward).normalized;
        Vector3 mantletTowardsEnemy = (target.transform.position - mantlet.transform.position).normalized;

        float mantletAngle = Vector3.Angle(startMantletDir, mantletForward);
        float xdAngle = Vector3.Angle(startMantletDir, mantletTowardsEnemy);

        float mantletToEnemyAngleSigned = Vector3.SignedAngle(mantletForward, mantletTowardsEnemy, Vector3.up);
        int minusPlus = 1;
        if (mantletToEnemyAngleSigned < 0) minusPlus = -1;

        float singleStep = turretSpeed * Time.deltaTime;

        if (mantletAngle < maxAngle && !ReferenceEquals(target,null) && xdAngle < maxAngle)
        {
            Vector3 newDirection = Vector3.RotateTowards(mantletForward, mantletTowardsEnemy, singleStep, 0.0f);
            mantlet.transform.rotation = Quaternion.LookRotation(newDirection);
            /*
            Debug.Log("S");
            if (Mathf.Abs(mantletForward.x - mantletTowardsEnemy.x) > 0.01)
            {
                Debug.Log("Nahoru,dolu");
                mantlet.transform.Rotate(Vector3.right * -minusPlus, turretSpeed * Time.deltaTime);
            }
            else if (Mathf.Abs(mantletForward.y - mantletTowardsEnemy.y) > 0.01)
            {
                Debug.Log("doprava,doleva");
                mantlet.transform.Rotate(Vector3.up * minusPlus, turretSpeed * Time.deltaTime);
            }*/
        }
        else
        {
            Vector3 newDirection = Vector3.RotateTowards(mantletForward, startMantletDir, singleStep, 0.0f);
            mantlet.transform.rotation = Quaternion.LookRotation(newDirection);
        }
        Debug.DrawRay(mantlet.transform.position, mantletTowardsEnemy * 1000, Color.magenta, 100);

    }
}

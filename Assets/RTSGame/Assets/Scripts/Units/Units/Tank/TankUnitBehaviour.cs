using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Tilemaps;
using static UnityEngine.UI.Image;

public class TankUnitBehaviour : UnitBehaviour
{

    public Vector3 previousDestination = new Vector3(0, 0, 0);
    TurretManagement turretManagement;
    protected TankMovement tankMovement;

    [SerializeField] float downRotation, upRotation;

    protected override void Start()
    {
        unitStats = gameObject.GetComponent<UnitStats>();
        target = unitStats.target;
        unitGm = transform.GetChild(0).gameObject;
        timeCurrent = timeMax;

        turretManagement = new TurretManagement(this, downRotation, upRotation);
        tankMovement = new TankMovement(this);


    }

    protected override void Update()
    {
        IsMoving();
        ShootV2();
        RotateTank();
        turretManagement.Update();
        tankMovement.Update();
    }

    protected override void ShootV2()
    {
        if (EnemyStillInRange())
        {
            //unitStats.unitGraphics.Turn(target, unitStats.spine);
            //Debug.Log($"Turn {target}, {unitStats.spine}");
            

            if (unitStats.listWeapon.Count > 0)
            {
                foreach (Weapon item in unitStats.listWeapon)
                {
                    item.Shoot(target);
                }
                //unitStats.weapon.Shoot(target);
                //unitStats.unitGraphics.animator.SetBool("isShooting", true);
            }

        }
        else
        {
            //unitStats.unitGraphics.animator.SetBool("isShooting", false);
            //unitGm.transform.rotation = new Quaternion(0, unitGm.transform.rotation.y, unitGm.transform.rotation.z, unitGm.transform.rotation.w);
            if (timeCurrent < timeMax)
            {
                timeCurrent += Time.deltaTime;
                return;
            }
            else
            {
                if (attackingBool)
                {
                    //Debug.Log("Attacking");
                    GoToEnemy(target);
                }
                else if (huntBool)
                {
                    //Debug.Log("Hunting");
                    if (target != true)
                    {
                        target = FindEnemy(Mathf.Infinity);
                    }
                    GoToEnemy(target);
                }
                else
                {
                    //Debug.Log("Finding");
                    target = FindEnemy(unitStats.range);
                }

                timeCurrent = 0;
            }
        }
    }

    protected void RotateTank()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1, unitStats.terrain))
        {
            Quaternion newRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            transform.GetChild(0).transform.rotation = Quaternion.RotateTowards(transform.GetChild(0).transform.rotation, newRot, 10 * Time.deltaTime);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TankMG : Weapon
{
    [SerializeField]
    float angleUnderToShoot;

    public override void Shoot(GameObject target)
    {
        if (CheckAngle(target) == false) return;

        if (Firerate())
        {

            Vector3 dir = GetDirection(target.transform);
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue, 1);

            double distance = double.MaxValue;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitStats.enemyLayerMask + unitStats.terrain))
            {

                distance = Vector3.Distance(transform.position, hit.transform.position);


                UnitStats enemyUnitStats = hit.collider.transform.root.GetComponent<UnitStats>();
                if (enemyUnitStats)
                {
                    int piercingChance = Random.Range(0, piercing);
                    enemyUnitStats.DamageThisUnit(damage, piercingChance, hit.point);
                }
            }

            GraphicBullet(target, dir, distance);

        }
    }

    protected bool CheckAngle(GameObject target)
    {
        Vector3 gunForward = transform.forward;
        Vector3 enemyDirection = target.GetComponent<UnitStats>().spine.transform.position - transform.position;
        float angle = Vector3.Angle(gunForward, enemyDirection);

        if (angle < angleUnderToShoot)
        {
            return true;
        }
        return false;
    }
}

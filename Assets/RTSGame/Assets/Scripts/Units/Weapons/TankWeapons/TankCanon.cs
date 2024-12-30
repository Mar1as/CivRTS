using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using System.Drawing;

public class TankCanon : Weapon
{
    [SerializeField] float angleUnderToShoot, caliberInMilimeter, recoilTimeMax;
    [SerializeField] public GameObject boomDirt, boomBody, boomTank;
    [SerializeField] float force;

    protected override void Start()
    {
        base.Start();

        
    }
    public override void Shoot(GameObject target)
    {
        Debug.DrawRay(transform.position, transform.forward * 1000, UnityEngine.Color.green, 1);
        //GunRecoil();
        if (CheckAngle(target) == false) return;

        if (Firerate())
        {
            unitAnimator.SetBool("isShooting", true);

            if (unitStats.targetUnitStats.alive != true) unitStats.target = null;
            if (muzzleFlashPS.GetComponent<ParticleSystem>()) muzzleFlashPS.GetComponent<ParticleSystem>().Play();


            Vector3 dir = GetDirection(target.transform);
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 1000, UnityEngine.Color.blue, 1);

            double distance = double.MaxValue;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitStats.enemyLayerMask + unitStats.terrain))
            {
                int piercingChance = Random.Range(0, piercing);

                distance = Vector3.Distance(transform.position, hit.transform.position);

                UnitStats enemyUnitStats = hit.collider.transform.root.GetComponent<UnitStats>();
                if (enemyUnitStats)
                {
                    
                    //enemyUnitStats.DamageThisUnit(damage, piercingChance, hit.point);
                    CreateBoom(hit.point, enemyUnitStats, boomBody);
                }
                else
                {
                    CreateBoom(hit.point, enemyUnitStats, boomDirt);
                }
            }

            GraphicBullet(target, dir, distance);
        }
        else
        {
            unitAnimator.SetBool("isShooting", false);
        }
    }

    protected bool CheckAngle(GameObject target)
    {
        Vector3 gunForward = transform.forward;
        Vector3 enemyDirection = target.GetComponent<UnitStats>().spine.transform.position - transform.position;
        float angle = Vector3.Angle(gunForward, enemyDirection);

        //Debug.Log($"Gun: {gun}; Angle: {angle}");
        if (angle < angleUnderToShoot)
        {
            return true;
        }
        return false;
    }

    void CreateBoom(Vector3 point, UnitStats hitEnemy, GameObject explosion)
    {

        float radius = (caliberInMilimeter / 88) * 12f;
        Collider[] colliders = Physics.OverlapSphere(point, radius, unitStats.enemyLayerMask);
        foreach (Collider collider in colliders)
        {
            UnitStats areaHitEnemy = collider.transform.root.GetComponent<UnitStats>();
            
            if (hitEnemy != null && hitEnemy == areaHitEnemy) //Stejný cíl
            {
                areaHitEnemy.DamageThisUnit(damage, piercing, point);

                AddForce(hitEnemy, areaHitEnemy, point, radius);
            }
            else //Náhodný cíl
            {
                float modificatorOfDamage = 1f - (Vector3.Distance(point, collider.gameObject.transform.position) / radius);
                int areaDamage = Mathf.FloorToInt(damage * modificatorOfDamage);
                areaHitEnemy.DamageThisUnit(areaDamage, piercing / 10, point);

                AddForce(hitEnemy, areaHitEnemy, point, radius);
            }
        }

        if(!ReferenceEquals(hitEnemy, null))
        {
            if (hitEnemy.Hp <= 0 && !hitEnemy.GetComponent<TankUnitStats>())
            {
                ExplosionGraphic(boomBody, 10, point);
                ExplosionGraphic(boomDirt, 10, point);
            }
            else if(hitEnemy.GetComponent<TankUnitStats>())
            {
                ExplosionGraphic(boomTank, 10, point);
            }
            else
            {
                ExplosionGraphic(boomDirt, 10, point);
            }
        }
        else
        {
            ExplosionGraphic(boomDirt, 10, point);
        }
    }

    void AddForce(UnitStats hitEnemy, UnitStats areaHitEnemy, Vector3 explosionLocation, float radius)
    {
        if (hitEnemy == areaHitEnemy)
        {
            if (!hitEnemy.alive)
            {
                Destroy(hitEnemy.gameObject);
            }
        }
        else if(!areaHitEnemy.alive)
        {
            if(!areaHitEnemy.IsDestroyed() && explosionLocation != null)
            {
                float distanceModif = radius / Vector3.Distance(explosionLocation, areaHitEnemy.transform.position);
                Vector3 smerovyVektor = (areaHitEnemy.transform.position - explosionLocation).normalized;
                smerovyVektor.y = 0;
                force = caliberInMilimeter * distanceModif;

                areaHitEnemy.unitGraphics.ragdoll.AddForce(force, smerovyVektor);
            }
            
        }
    }

    void ExplosionGraphic(GameObject prefab, float time, Vector3 point)
    {
        GameObject explosionGM = Instantiate(prefab, point, Quaternion.Euler(-90, 0, 0));
        //if(!ReferenceEquals(hitEnemy, null)) explosionGM.transform.parent = hitEnemy.transform;
        Destroy(explosionGM, time);
    }

}

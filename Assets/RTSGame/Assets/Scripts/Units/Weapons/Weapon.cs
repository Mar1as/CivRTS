using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using UnityEditor;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    public int damage, accuracyWeapon, piercing;

    [SerializeField]
    public int ammoMax;
    public int ammoCurrent;

    [SerializeField]
    public float reloadTimeMax;
    float reloadTimeCurrent;

    [SerializeField]
    public float fireRateMaxPerMinute;
    float fireRateMaxPerSecound;
    float fireRateCurrent;

    [SerializeField] 
    protected GameObject bullet, muzzleFlashPS;

    public UnitStats mainUnitStats;

    public UnitAnimator unitAnimator;

    protected WeaponGraphics weaponGraphics;


    public UnitStats unitStats
    {
        get
        {
            return mainUnitStats;
        }
        set
        {
            mainUnitStats = value;
            unitAnimator = mainUnitStats.unitGraphics.animator;
            float speed = (1.66f / reloadTimeMax);
            unitAnimator.SetFloat("ReloadSpeed", speed);
        }
    }

    protected virtual void Start()
    {
        ammoCurrent = ammoMax;
        fireRateMaxPerSecound = 1 / (fireRateMaxPerMinute / 60);
        fireRateCurrent = fireRateMaxPerSecound;
        weaponGraphics = new WeaponGraphics(this);
        
    }

    private void Update()
    {
        Reload();
    }

    public virtual void Shoot(GameObject target)
    {
        if (Firerate())
        {
            Debug.Log(unitStats.targetUnitStats.alive);
            if (unitStats.targetUnitStats.alive != true) unitStats.target = null;
            if (muzzleFlashPS.GetComponent<ParticleSystem>()) muzzleFlashPS.GetComponent<ParticleSystem>().Play();

            UnitStats targetUnitStats = target.GetComponent<UnitStats>();

            Vector3 dir = GetDirection(targetUnitStats.spine.transform);
            Ray ray = new Ray(muzzleFlashPS.transform.position, dir);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue, 1);

            double distance = 1000;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity,unitStats.enemyLayerMask + unitStats.terrain))
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

    public void Reload()
    {
        if (ammoCurrent <= 0)
        {
            if (ReferenceEquals(unitAnimator, true)) unitAnimator.SetBool("isReloading", true);
            unitAnimator.SetBool("isReloading", true);
            reloadTimeCurrent -= Time.deltaTime;
        }
        if (reloadTimeCurrent <= 0)
        {
            if(ReferenceEquals(unitAnimator, true)) unitAnimator.SetBool("isReloading", false);
            unitAnimator.SetBool("isReloading", false);
            reloadTimeCurrent = reloadTimeMax;
            ammoCurrent = ammoMax;
        }
    }
    protected bool Firerate()
    {
        if (fireRateCurrent <= 0 && ammoCurrent > 0) //jeden výstøel za fireRateMax (0.5 fireratemax -> 2 výstøely za sekundu)
        {
            fireRateCurrent = fireRateMaxPerSecound;
            ammoCurrent--;
            return true;
        }
        else if (ammoCurrent > 0)
        {
            fireRateCurrent -= Time.deltaTime;
        }
        return false;
    }
    protected virtual Vector3 GetDirection(Transform enemyTransform)
    {
        Vector3 direction = enemyTransform.position - transform.position;
        float distance = direction.magnitude;        
        distance /= unitStats.range;
        distance = distance * (((100 - unitStats.skill + 400 - accuracyWeapon * 4) / 100) + 1);
        distance *= 3;
        distance *= ReturnIfIsMoving();
        direction.x += Random.Range(-distance, distance);
        direction.y += Random.Range(-distance, distance);
        direction.z += Random.Range(-distance, distance);
        direction.Normalize();
        return direction;
    }
    protected void GraphicBullet(GameObject gm, Vector3 dir, double distance)
    {
        if (this.bullet != null)
        {
            float speed = 50;
            float time = (float)(distance / speed);
            GameObject bullet = Instantiate(this.bullet, muzzleFlashPS.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(dir * 50 * speed); //7500
            
            Destroy(bullet, time);
        }
    }

    List<int> x = new List<int>();

    void Procenta(bool t)
    {
        if (t == true)
        {
            x.Add(1);
        }
        else
        {
            x.Add(0);
        }

        double y = 0;
        foreach (int item in x)
        {
            y += item;
        }
        y /= x.Count;
        y *= 100;
        //
        //(y + "%");

    }

    protected virtual float ReturnIfIsMoving()
    {
        if (unitStats.agent.velocity.magnitude > 1)
        {
            
            return 2;
        }
        return 1;
    }

    protected virtual void SetUpMuzzle()
    {
        //muzzleFlash = weaponGraphics.SetUpMuzzleFlash(prefabMuzzleFlash, 0.5f, 0.15f);
        //if (locationForFlash != null) muzzleFlash.transform.position = locationForFlash.transform.position;
    }
}

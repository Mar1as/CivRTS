using System;
using UnityEngine;

[Serializable]
public class DataWeaponRTS
{
    public MainWeaponRTS mainWeaponRTS { get; private set; }

    [SerializeField]
    int damage, range, maxAmmoCount;
    [SerializeField]
    float reloadTime, fireRate;
    [SerializeField]
    GameObject bullet, barrel;

    int curAmmoCount;
    float nextFireTime;

    public DataWeaponRTS(MainWeaponRTS mainWeaponRTS)
    {
        this.mainWeaponRTS = mainWeaponRTS;
        //Reload();
    }

    public void Update()
    {
        
    }
    /*
    void Fire(GameObject enemy)
    {
        if (curAmmoCount > 0 && Time.time >= nextFireTime)
        {
            ShootBullet();
            nextFireTime = Time.time + fireRate;
            curAmmoCount--;
        }
    }

    void ShootBullet()
    {
        if (bullet != null && barrel != null)
        {
            GameObject firedBullet = GameObject.Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
            Rigidbody bulletRb = firedBullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.velocity = barrel.transform.forward * range;
            }
        }
    }

    public void Reload()
    {
        curAmmoCount = maxAmmoCount;
    }*/
}
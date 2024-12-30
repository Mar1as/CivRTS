using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGraphics : MonoBehaviour
{
    Weapon weapon;

    public WeaponGraphics(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public void PlayMuzzleFlash(GameObject muzzleFlash)
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.GetComponent<ParticleSystem>().Play();
        }
    }
    public GameObject SetUpMuzzleFlash(GameObject muzzleFlash, float scale, float startLifeTime)
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.transform.localScale = new Vector3(scale, scale, scale);
            muzzleFlash.GetComponent<ParticleSystem>().startLifetime = startLifeTime;
            muzzleFlash = Instantiate(muzzleFlash, weapon.transform.position, Quaternion.identity, weapon.transform);
            return muzzleFlash;
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TankUnitStats : UnitStats
{
    TankArmor tankArmor;

    [SerializeField]
    public int maxOsadka;
    public int osadka;
    [SerializeField] public int armorFront, armorSide, armorBack;
    [SerializeField] GameObject deathExplosion;

    protected override void Start()
    {
        teamColor = GetTeamColor(); //Player ini
        enemyLayerMask = ReturnLayerMask();
        ChangeColliderAndSpine();
        osadka = maxOsadka;
        alive = true;
        terrain = LayerMask.GetMask("Terrain");
        //fogOfWar = new FogOfWar(gameObject);
        unitGraphics = new UnitGraphics(teamColor, gameObject, this);
        enemyLayerMask = ReturnLayerMask();
        hp = maxHp;
        agent = gameObject.GetComponent<NavMeshAgent>();
        GetWeapon(); //Najde zbraò a páteø
        tankArmor = new TankArmor(this);
    }

    override protected void Update()
    {
        unitGraphics.statsBar.Update();
    }

    protected override void GetWeapon()
    {
        if (true)
        {
            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.GetComponent<Weapon>())
                {

                    weapon = child.gameObject.GetComponent<Weapon>();
                    weapon.unitStats = this;
                    weapon.unitAnimator = unitGraphics.animator;

                    listWeapon.Add(weapon);
                }

                if (child.gameObject.GetComponent<BoxCollider>())
                {
                    child.gameObject.layer = gameObject.layer;
                }

                if (child.gameObject.name.ToLower() == "turret")
                {
                    spine = child.gameObject;
                    spine.layer = gameObject.layer;

                }

                if (child.gameObject.name.ToLower() == "mantlet")
                {
                    mantlet = child.gameObject;

                }
                if (child.gameObject.GetComponent<BoxCollider>())
                {
                    child.gameObject.layer = gameObject.layer;

                }
                if(child.gameObject.GetComponent<Collider>())
                {
                    child.gameObject.layer = gameObject.layer;
                }
            }
        }
    }

    public override int Hp
    {
        get { return hp; }
        set
        {
            osadka -= Random.Range(0, Random.Range(0, osadka));
            hp -= value;
            unitGraphics.statsBar.UpdateHealthBar();
            unitGraphics.statsBar.UpateText($"{osadka}/{maxOsadka}");
            Death();

        }
    }

    public override void DamageThisUnit(int damage, int piercing, Vector3 point)
    {
        if (tankArmor.CheckArmor(point,piercing))
        {
            Hp = damage;
        }
    }

    protected override void Death()
    {
        if (hp <= 0 || osadka <= 0)
        {
            alive = false;

            GameObject explosionGM = Instantiate(deathExplosion, transform.position, Quaternion.Euler(-90, 0, 0));
            Destroy(explosionGM, 10);
            Destroy(gameObject);
            //StartCoroutine(unitGraphics.animator.DieAnimation("isDead", true, gameObject));
        }
    }


}

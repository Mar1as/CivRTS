using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitStats : MonoBehaviour
{
    TeamsConstructor player;
    Battalions Battalion;
    public GameObject mantlet;

    [SerializeField] public LayerMask enemyLayerMask, terrain;
    [SerializeField] public int hp, maxHp, range, skill, cost, armor;
    public int costCur;

    [SerializeField] public Weapon weapon;
    [SerializeField] public List<Weapon> listWeapon = new List<Weapon>();

    public NavMeshAgent agent;
    public GameObject Target;

    [SerializeField] public string jmeno;
    [SerializeField] public string kategorie;
    [SerializeField] public Sprite icon;

    public UnitGraphics unitGraphics { get; protected set; }
    [SerializeField] public GameObject spine;
    [SerializeField] public GameObject collider;

    public bool order { get; set; }
    public bool alive { get; protected set; }

    public Color teamColor;

    [Header("Health regen")]
    [SerializeField] float procento = 10;
    [SerializeField] float timeMax = 5;
    float timeCur;

    [Header("Health Bar Field")]
    [SerializeField] public GameObject prefab;
    [SerializeField] public Image image;

    [Header("Death")]
    [SerializeField] public float deathTimer;
    [SerializeField] GameObject ragdoll;

    [SerializeField] bool die = false;
    
    public GameObject target
    {
        get
        {
            return Target;
        }
        set
        {
            Target = value;
            if (Target == null) targetUnitStats = null;
            else targetUnitStats = target.GetComponent<UnitStats>();
        }
    }

    public UnitStats targetUnitStats { get; private set; }

    protected virtual void Start()
    {
        teamColor = GetTeamColor(); //Player ini
        timeCur = timeMax;
        hp = maxHp;
        alive = true;
        terrain = LayerMask.GetMask("Terrain");
        unitGraphics = new UnitGraphics(GetTeamColor(), gameObject, this);
        enemyLayerMask = ReturnLayerMask();
        agent = gameObject.GetComponent<NavMeshAgent>();
        GetWeapon(); // Find weapon and spine
        ChangeColliderAndSpine();

    }
    virtual protected void Update()
    {
        unitGraphics.statsBar.Update();
        HealthRegen();
        // if(!ReferenceEquals(Target, null)) Debug.Log(Target.name);
        if(die) Death();
    }

    public Battalions battalion
    {
        get { return Battalion; }
        set
        {

            if (Battalion != value)
            {
                Battalion?.RemoveUnit(gameObject);
                Battalion = value;
            }
        }
    }

    public virtual int Hp
    {
        get { return hp; }
        set
        {
            if (!ReferenceEquals(battalion, null))
            {
                player.uiManager.uiShowInfoAboutBattalion.UpdateInPanels(battalion);
            }
            hp = Mathf.Max(0, value); // Ensure hp does not go below 0
            unitGraphics.statsBar.UpdateHealthBar();
            if (hp == 0) Death();
        }
    }

    public virtual void DamageThisUnit(int damage, int piercing, Vector3 point)
    {
        if (piercing >= armor)
        {
            Hp -= damage;
        }
    }

    protected virtual void GetWeapon()
    {
        if (listWeapon.Count < 1)
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
                if (child.gameObject.name == "spine.003")
                {
                    spine = child.gameObject;
                    spine.layer = gameObject.layer;
                }
                if (child.gameObject.name == "collider")
                {
                    collider = child.gameObject;
                    collider.GetComponent<SkinnedMeshRenderer>().material.color = GetTeamColor();
                    collider.layer = gameObject.layer;
                }
            }
        }
    }

    protected void ChangeColliderAndSpine()
    {
        if(spine)
        {
            spine.layer = gameObject.layer;
        }
        if (collider)
        {
            //collider.GetComponent<SkinnedMeshRenderer>().material.color = GetTeamColor();
            collider.layer = gameObject.layer;
        }
    }

    protected LayerMask ReturnLayerMask()
    {
        LayerMask mask = new LayerMask();
        foreach (var player in Teams.listOfPlayers)
        {
            if (player.tag != tag)
            {
                mask |= player.layerMask;
            }
        }
        return mask;
    }

    protected Color GetTeamColor()
    {
        foreach (var player in Teams.listOfPlayers)
        {
            if (player.tag == tag)
            {
                this.player = player;
                return player.teamColor;
            }
        }
        return Color.white;
    }

    protected virtual void Death()
    {
        if (hp <= 0 && alive)
        {
            alive = false;
            gameObject.layer = 0;
            gameObject.tag = "Untagged";

            collider.layer = 0;
            Collider box = collider.GetComponent<Collider>();
            box.enabled = false;
            box.isTrigger = true;
            Destroy(box);

            weapon.gameObject.transform.SetParent(null);
            weapon.gameObject.AddComponent<Rigidbody>();
            Destroy(weapon.gameObject, 10);
            unitGraphics.ragdoll.EnableRagdoll(10);
            ragdoll.layer = 0;

            foreach (Transform child in ragdoll.transform.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = 0;
            }


            ragdoll.gameObject.transform.SetParent(null);
            Destroy(ragdoll, 10);
            battalion = null;
            Destroy(gameObject);
            //StartCoroutine(unitGraphics.animator.DieAnimation("isDead", true, gameObject));
        }
    }

    
    void HealthRegen()
    {
        if (Hp < maxHp && alive)
        {
            timeCur -= Time.deltaTime;
            if (timeCur < 0)
            {
                int hpPlus = (int)Math.Ceiling((float)maxHp * ((float)procento / 100));
                if (Hp + hpPlus < maxHp)
                {
                    Hp += hpPlus;

                }
                else
                {
                    Hp = maxHp;
                }
                timeCur = timeMax;
            }
        }
        else timeCur = timeMax;

    }
}

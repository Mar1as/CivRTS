using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class UnitBehaviour : MonoBehaviour
{

    //public GameObject target;

    protected GameObject huntTarget;

    public UnitStats unitStats;

    protected Collider[] colliders = new Collider[20];

    protected int numColliders = 0;

    protected GameObject unitGm;

    //STATES
    [SerializeField] public bool huntBool;
    [SerializeField] public bool attackingBool;

    //Èas mezi find enemy
    protected float timeCurrent = 0;
    protected float timeMax = 1;

    public GameObject target
    {
        get
        {
            return unitStats.target;
        }
        set
        {
            unitStats.target = value;
        }
    }

    protected virtual void Start()
    {
        unitStats = gameObject.GetComponent<UnitStats>();
        target = unitStats.target;
        unitGm = transform.GetChild(0).gameObject;
        timeCurrent = timeMax;

    }

    protected virtual void Update()
    {
        IsMoving();
        ShootV2();
    }

    protected bool EnemyStillInRange()
    {
        if (target != null && CheckIfCanSeeEnemy())
        {
            if ((transform.position - target.transform.position).magnitude < unitStats.range)
            {
                return true;
            }
        }
        
        return false;
    }

    private void OnDestroy()
    {
        Teams.RemoveUnitVoid(gameObject);
        
    }

    protected virtual bool CheckIfCanSeeEnemy()
    {

        if (target == false)
        {
            return false;
        }
        Ray ray = new Ray(unitStats.spine.transform.position, target.GetComponent<UnitStats>().spine.transform.position - unitStats.spine.transform.position);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, 1);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitStats.enemyLayerMask + unitStats.terrain))
        {
            //Debug.DrawRay(ray.origin, hit.transform.position - ray.origin, Color.red, 1);

            if (unitStats.terrain == (unitStats.terrain | (1 << hit.collider.gameObject.layer))) //zjistí zda je pøekážka mezi jednotkou a cílem
            {
                
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    bool isWalking = false;

    protected void IsMoving()
    {
        if (unitStats.alive)
        {
            unitStats.unitGraphics.animator.SetFloat("Speed", unitStats.agent.velocity.magnitude);
            if (unitStats.agent.velocity.magnitude > 0.5)
            {
                isWalking = true;
                unitStats.unitGraphics.animator.SetBool("isWalking", isWalking);
            }
            else
            {
                isWalking = false;
                unitStats.unitGraphics.animator.SetBool("isWalking", isWalking);
            }
        }
        else
        {
            unitStats.agent.isStopped = true;
        }
    }

    protected GameObject FindEnemy(float radius)
    {
        /*
        numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders, unitStats.enemyLayerMask);
        HashSet<GameObject> uniqueParents = new HashSet<GameObject>();

        for (int i = 0; i < numColliders; i++)
        {
            Collider collider = colliders[i];
            if (collider != null)
            {
                GameObject parent = collider.transform.root.gameObject;
                uniqueParents.Add(parent);
            }
        }

        List<GameObject> uniqueParentList = new List<GameObject>(uniqueParents);

        if (uniqueParentList.Count == 0)
        {
            return null;
        }

        while (uniqueParentList.Count > 0)
        {
            int randomIndex = Random.Range(0, uniqueParentList.Count);
            GameObject randomParent = uniqueParentList[randomIndex];

            if (randomParent.GetComponent<UnitStats>().alive)
            {
                return randomParent;
            }
            else
            {
                uniqueParentList.RemoveAt(randomIndex);
            }
        }

        return null;*/

        numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders, unitStats.enemyLayerMask);
        
        for (int i = 0; i < numColliders; i++)
        {
            int nahodnyNepritel = Random.Range(0, numColliders);
            //int nahodnyNepritel = i;
            huntTarget = colliders[nahodnyNepritel].transform.root.gameObject;
            if (huntTarget.GetComponent<UnitStats>().alive)
            {
                return huntTarget;
            }
        }
        return null;
        
    }

    protected virtual void ShootV2()
    {
        if (unitStats.alive == false) return;
        if (EnemyStillInRange())
        {
            unitStats.unitGraphics.Turn(target, unitStats.spine);

            if (unitStats.listWeapon.Count > 0)
            {
                foreach (Weapon item in unitStats.listWeapon)
                {
                    item.Shoot(target);
                }
                //unitStats.weapon.Shoot(target);
                unitStats.unitGraphics.animator.SetBool("isShooting", true);
            }

        }
        else
        {
            unitStats.unitGraphics.animator.SetBool("isShooting", false);
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

    public void ChangeTarget(GameObject gm)
    {
        target = gm;
        attackingBool = true;
    }

    Vector3 a;
    Vector3 b;
    Vector3 c;
    int d;
    float bx;
    float bz;
    int ex;
    int ez;
    Vector3 e;
    Vector3 f;

    float k = 0;

    protected void GoToEnemy(GameObject enemy)
    {
        if (unitStats.order == true)
        {
            if (unitStats.agent.hasPath == false)
            {
                unitStats.order = false;

            }
        }
        else
        {
            if (enemy == true && unitStats.order == false)
            {
                Debug.Log(enemy);

                a = transform.position;
                b = enemy.transform.position;
                c = b - a;

                if (Vector3.Distance(a,b) > unitStats.range)
                {

                    k = 0;
                    unitStats.agent.SetDestination(SetPositionFromEnemy(enemy, unitStats.range));
                }
                else if (unitStats.agent.hasPath == false && CheckIfCanSeeEnemy() == false)
                {

                    k++;
                    unitStats.agent.SetDestination(SetPositionFromEnemy(enemy, Vector3.Distance(transform.position,enemy.transform.position)));
                }
            }
            else
            {
                attackingBool = false;
            }
        }

        if (true)
        {
            
        }
    }

    Vector3 SetPositionFromEnemy(GameObject enemy, float range)
    {
        if (unitStats.alive)
        {

            float angle = Mathf.Atan2(c.z, c.x);

            float radius = range * (0.8f - (k / 10));

            float ex = radius * Mathf.Cos(angle);
            float ez = radius * Mathf.Sin(angle);

            e = new Vector3(ex, 0, ez);

            f = b - e;

            return f;
        }
        return transform.position;
    }
}

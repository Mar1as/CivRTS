using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitGraphics : MonoBehaviour
{
    public GameObject circle { get;private set;}
    public UnitAnimator animator { get; private set; }

    UnitStats unitStats;
    public StatsBar statsBar;

    public EnableDisableRagdoll ragdoll;

    public UnitGraphics(Color teamColor, GameObject gm, UnitStats us)
    {
        animator = new UnitAnimator(gm);
        LoadCircle(teamColor,gm);
        unitStats = us;

        statsBar = new StatsBar(us);

        ragdoll = new EnableDisableRagdoll(gm,false);
    }

    private void Update()
    {
        
    }

    void LoadCircle(Color teamColor, GameObject gm)
    {
        GameObject circle = Resources.Load<GameObject>("Circle");
        
        circle.transform.localScale = new Vector3(1f, 1f, 1f) * gm.GetComponent<NavMeshAgent>().radius / 2;
        circle.GetComponent<SpriteRenderer>().color = teamColor;
        this.circle = Instantiate(circle, gm.transform.position, circle.transform.rotation, gm.transform);
        ActiveCircle(false);
    }
    public void ActiveCircle(bool boolos)
    {
        if (boolos)
        {
            if(circle)
            circle.SetActive(true);
        }
        else
        {
            if(circle)
            circle.SetActive(false);
        }
    }
    public void Turn(GameObject target, GameObject spine)
    {
        //Debug.Log($"Target: {target.transform}; Spine: {spine.transform}");

        //spine.transform.root.transform.GetChild(0).transform.LookAt(target.transform);
        spine.transform.root.transform.LookAt(target.transform);
    }

    
}

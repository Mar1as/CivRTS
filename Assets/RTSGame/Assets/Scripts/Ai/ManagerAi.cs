using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerAi : MonoBehaviour
{
    public EnemyAi ai;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ai != null) ai.Update();
    }
}

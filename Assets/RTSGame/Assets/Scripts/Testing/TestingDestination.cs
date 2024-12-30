using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestingDestination : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<NavMeshAgent>().SetDestination(transform.position + transform.forward * 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

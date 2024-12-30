using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scroll_Track : MonoBehaviour 
{

    [SerializeField]
    private float scrollSpeed = 0.05f;

    private float offset = 0.0f;
    private Renderer r;
    NavMeshAgent agent;

    void Start()
    {
        r = GetComponent<Renderer>();
        agent = transform.root.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.velocity.magnitude < 1) return;
        offset = (agent.velocity.normalized.magnitude * offset + Time.deltaTime * scrollSpeed) % 1f;
        r.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
    }
}

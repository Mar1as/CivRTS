using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankScrollWheels : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed = 0.05f;

    private float offset = 0.0f;
    private Renderer r;

    void Start()
    {
        r = GetComponent<Renderer>();
    }

    void Update()
    {
        scrollSpeed = transform.root.GetComponent<NavMeshAgent>().velocity.magnitude / 10;

        offset = (offset + Time.deltaTime * scrollSpeed) % 1f;
        r.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
    }
}

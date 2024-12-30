using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Destructable : MonoBehaviour
{
    [SerializeField] int mass;
    [SerializeField] bool isWall;
    private bool destroyed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!destroyed)
        {
            Destruction(collision);
        }
    }

    void Destruction(Collision collision)
    {
        Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        NavMeshAgent navMeshAgent = collision.gameObject.GetComponent<NavMeshAgent>();
        if (enemyRigidbody != null && !destroyed && navMeshAgent != null)
        {
            if (enemyRigidbody.mass > mass)
            {
                destroyed = true;
                GameObject destroyedVersion = Instantiate(gameObject, transform.position, transform.rotation);
                Rigidbody rb = destroyedVersion.AddComponent<Rigidbody>();
                rb.mass = mass;
                rb.AddForce(SmerovyVektor(collision.transform.position, transform.position) * 1000000 * collision.gameObject.GetComponent<NavMeshAgent>().velocity.magnitude);
                Destroy(destroyedVersion.GetComponent<Destructable>());
                Destroy(gameObject);
                SlowDown(enemyRigidbody);
            }
        }
    }

    Vector3 SmerovyVektor(Vector3 origin, Vector3 target)
    {
        Vector3 smerovy = target - origin;
        return smerovy.normalized;
    }

    void SlowDown(Rigidbody collisionRb)
    {
        NavMeshAgent navMeshAgentCollision = collisionRb.GetComponent<NavMeshAgent>();
        if (navMeshAgentCollision != null)
        {
            navMeshAgentCollision.velocity /= 10;
        }
    }

    void Wall()
    {
        if (isWall)
        {
            //Fracture f = new Fracture();
        }
    }
}

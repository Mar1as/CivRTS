using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TankRollWheels : MonoBehaviour
{
    // Speed value
    public float rotationSpeed;


    void Update()
    {
        rotationSpeed = transform.root.GetComponent<NavMeshAgent>().velocity.magnitude;

        // Apply rotation to each child object
        for (int i = 0; i < transform.childCount; i++)
        {
            // Calculate rotation based on wheel diameter and speed value
            float wheelSize = transform.GetChild(i).GetComponent<MeshFilter>().mesh.bounds.size.y;
            float rotationAngle = rotationSpeed / wheelSize;

            //Apply rotation
            transform.GetChild(i).transform.Rotate(rotationAngle, 0, 0, Space.Self);
        }

    }
}

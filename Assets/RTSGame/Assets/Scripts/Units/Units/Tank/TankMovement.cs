using System;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.AI;

public class TankMovement
{
    UnitBehaviour unitBehaviour;

    NavMeshAgent agent;

    bool rotating = false;
    float curAngle;
    //float angling = 0;
    Vector3 previousDestination;
    Vector3 vectorOnEnd = Vector3.zero;

    float angularSpeed;
    float maxSpeed;

    public TankMovement(UnitBehaviour unitBehaviour)
    {
        //this.unitBehaviour = unitBehaviour;
        agent = unitBehaviour.GetComponent<NavMeshAgent>();

        previousDestination = agent.transform.position;

        angularSpeed = agent.angularSpeed;
        maxSpeed = agent.speed;
    }

    public void Update()
    {
        CheckDestination();
        Pokus2();
        RotateOnEnd();

       // Rotate();
    }

    void Pokus2()
    {
        float maxAngle = 30;

        Vector3 agentVector = agent.transform.forward;
        Vector3 destinationVector = agent.steeringTarget - agent.transform.position;

        SlowDown(agentVector, destinationVector);

        curAngle = Vector3.Angle(agentVector, destinationVector);

        if (curAngle < maxAngle && rotating == false) // Malá zatáèka
        {

        }
        else // Velká zatáèka
        {
            rotating = true;
            maxAngle = 10;
            curAngle = Vector3.Angle(new Vector3(agentVector.x,0, agentVector.z), new Vector3(destinationVector.x,0, destinationVector.z));
            if (curAngle > maxAngle)
            {
                //RotateChasis(agentVector, destinationVector);
                /*
                float modificator = 360 / (360 - curAngle);
                Vector3 newDirection = Vector3.RotateTowards(agentVector, destinationVector, angling * Time.deltaTime * modificator, 0f);
                agent.transform.rotation = Quaternion.LookRotation(newDirection);
                agent.velocity = Vector3.zero * 0;
                Debug.Log(agent.velocity);*/
            }
            else
            {
                rotating = false;

                agent.angularSpeed = angularSpeed;
            }

        }
    }

    void RotateChasis(Vector3 agentVector, Vector3 destinationVector)
    {
        agent.angularSpeed = 0;

        float angleToDestination = Vector3.SignedAngle(agentVector, destinationVector, Vector3.up);
        Debug.Log(angleToDestination);
        int minusPlus;  
        if(angleToDestination < 0)
        {
            minusPlus = -1;
        }
        else
        {
            minusPlus = 1;
        }

        agent.transform.Rotate(0, minusPlus * Time.deltaTime * 10, 0);
        agent.velocity = Vector3.zero;
    }

    void RotateOnEnd()
    {
        if (agent.isStopped)
        {
            if (vectorOnEnd == Vector3.zero)
            {
                RotateChasis(agent.transform.forward, agent.transform.forward);
            }
            else
            {
                if (Vector3.Angle(new Vector3(agent.transform.forward.x, 0, agent.transform.forward.z),new Vector3(vectorOnEnd.x,0, vectorOnEnd.z)) < 10)
                {
                    vectorOnEnd = Vector3.zero;
                }
                
                RotateChasis(agent.transform.forward, vectorOnEnd);
            }
        }
    }

    public void ChangeRotateTo(Vector3 vector)
    {
        vectorOnEnd = vector - agent.transform.position;
    }

    void CheckDestination()
    {
        if (previousDestination != agent.destination)
        {
            previousDestination = agent.destination;
            rotating = false;

            agent.speed = maxSpeed;
            agent.angularSpeed = angularSpeed;
        }
    }

    void Rotate()
    {
        agent.transform.Rotate(0, 10 * Time.deltaTime, 0);
    }

    void SlowDown(Vector3 agentVector, Vector3 destinationVector)
    {
        float angle = Vector3.Angle(agentVector, destinationVector);
        float curAngle = 10;
        float minSpeed = 0.1f;
        Vector3 forward = agent.transform.forward.normalized;

        if (angle > curAngle * 4.5)
        {
            agent.velocity = Vector3.zero;
        }
        else if (angle > curAngle)
        {
            agent.velocity = (curAngle / angle) * forward * agent.speed;
        }
        else
        {
            agent.speed = maxSpeed;
        }

    }
}



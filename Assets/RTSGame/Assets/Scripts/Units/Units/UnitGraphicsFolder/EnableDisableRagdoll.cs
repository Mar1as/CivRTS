using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Recorder.OutputPath;

public class EnableDisableRagdoll
{

    [SerializeField] private Animator animator;
    [SerializeField] private Transform ragdollRoot;
    [SerializeField] private bool startRagdoll = false;

    private Rigidbody[] rigidbodies;
    private CharacterJoint[] joints;
    private Collider[] colliders;

    public EnableDisableRagdoll(GameObject mainUnit, bool startRagdoll)
    {
        ragdollRoot = mainUnit.transform;
        animator = mainUnit.GetComponentInChildren<Animator>();
        this.startRagdoll = startRagdoll;

        Initialize();
    }

    private void Initialize()
    {
        rigidbodies = ragdollRoot.GetComponentsInChildren<Rigidbody>();
        joints = ragdollRoot.GetComponentsInChildren<CharacterJoint>();
        colliders = ragdollRoot.GetComponentsInChildren<Collider>();

        if (animator != null)
        {
            if (startRagdoll)
            {
                EnableRagdoll();
            }
            else
            {
                EnableAnimator();
            }
        }
    }

    private void EnableRagdoll()
    {
        if (animator != true) return;

        Debug.Log("Ragdoll");
        animator.enabled = false;

        foreach (var joint in joints) joint.enableCollision = true;
        foreach (var collider in colliders) collider.enabled = true;
        foreach (var rb in rigidbodies)
        {
            rb.linearVelocity = Vector3.zero;
            rb.detectCollisions = true;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void EnableAnimator()
    {
        if (animator != true) return;
        animator.enabled = true;

        foreach (var joint in joints) joint.enableCollision = false;
        //foreach (var collider in colliders) collider.enabled = false;
        foreach (var rb in rigidbodies)
        {
            rb.detectCollisions = false;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void EnableRagdoll(float timer)
    {
        EnableRagdoll();
        GameObject.Destroy(ragdollRoot.gameObject, timer);
    }

    public void AddForce(float force, Vector3 direction)
    {
        foreach (var rb in rigidbodies)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}

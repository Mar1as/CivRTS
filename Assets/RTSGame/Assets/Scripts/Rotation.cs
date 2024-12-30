using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public AnimationCurve animCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            Quaternion newRot = Quaternion.Lerp(transform.GetChild(0).transform.rotation, Quaternion.FromToRotation(Vector3.up, hit.normal), animCurve.Evaluate(Time.deltaTime));
            transform.GetChild(0).transform.rotation = newRot;
        }
    }
}

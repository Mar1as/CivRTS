using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xdddd : MonoBehaviour
{
    [SerializeField] private GameObject target;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {

            Vector3 a = transform.position;
            Vector3 b = target.transform.position;
            Vector3 c = b - a;
            int d = (int)(c.x - c.z);
            Debug.Log("d: " + d);
            //float bx = b.x / d;
            float bx = SafeDivision(c.x, d);
            Debug.Log("bx: " + bx);
            //float bz = (b.z / d);
            float bz = SafeDivision(c.z, d);
            Debug.Log("bz: " + bz);
            int ex = (int)(bx * 30);
            Debug.Log("ex: " + ex);
            int ez = (int)(bz * 30);
            Debug.Log("ez: " + ez);
            Vector3 e = new Vector3(ex, 0, ez);
            Debug.Log("e: " + e);
            //e = e * (d / (-d));
            Vector3 f = b - e;
            Debug.Log("vektor: " + f);
            Debug.Log("Enemy: " + b);
        }
    }
    static float SafeDivision(float numerator, float denominator)
    {
        if (denominator != 0)
        {
            return Mathf.Abs(numerator) / Mathf.Abs(denominator);
        }
        else
        {
            // Return 0 when attempting to divide by zero
            return 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamera : MonoBehaviour
{
    

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectHex();
        }
    }

    void SelectHex()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            hit.collider.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            Debug.Log(hit.collider.gameObject.name);
        }
    }
}

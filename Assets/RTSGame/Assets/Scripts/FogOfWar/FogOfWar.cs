using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    MeshRenderer[] childMeshRendererArray;
    public FogOfWar(GameObject parrent)
    {
        childMeshRendererArray = new MeshRenderer[parrent.transform.childCount];
        for (int i = 0; i < childMeshRendererArray.Length; ++i)
        {
            childMeshRendererArray[i] = parrent.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>(); //Najít všechny child mesh renderery
        }
    }

    public void VisibleThisUnit(bool IsVisible)
    {
        Debug.Log("KKT");
        for (int i = 0; i < childMeshRendererArray.Length; ++i)
        {
            if (IsVisible)
            {
                childMeshRendererArray[i].enabled = true;
            }
            else
            {
                childMeshRendererArray[i].enabled = false;
            }
        }
    }
}

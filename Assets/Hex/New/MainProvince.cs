using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHexCell : MonoBehaviour
{
    public DataHexCell dataProvince;

    public void Inicilizace(int x, int y)
    {
        dataProvince = new DataHexCell(x,y);

    }
}

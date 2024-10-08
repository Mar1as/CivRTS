using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MainHexCell : MonoBehaviour
{
    [SerializeField]
    public DataHexCell dataHexCell;
    public BrainHexCell brainHexCell;
    private void Start()
    {
        
    }
    public void Inicilizace()
    {
        dataHexCell = new DataHexCell();
        brainHexCell = new BrainHexCell(this);
    }
}

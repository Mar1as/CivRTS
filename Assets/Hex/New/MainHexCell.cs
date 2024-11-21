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
        dataHexCell = new DataHexCell(this);
        brainHexCell = new BrainHexCell(this);
    }

    public void RefreshPosition()
    {
        Debug.Log("Refresh");
        Vector3 position = transform.localPosition;
        position.y = dataHexCell.Elevation * HexMetrics.elevationStep;
        position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;

        transform.localPosition = position;

        Vector3 uiPosition = dataHexCell.uiRect.localPosition;
        uiPosition.z = -position.y;
        dataHexCell.uiRect.localPosition = uiPosition;
    }
}

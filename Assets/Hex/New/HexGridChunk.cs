using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    MainHexCell[] cells;

    HexMesh hexMesh;
    Canvas gridCanvas;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new MainHexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    void Start()
    {
        //hexMesh.Triangulate(cells);
    }

    public void AddCell(int index, MainHexCell cell)
    {
        cells[index] = cell;
        cell.dataHexCell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.dataHexCell.uiRect.SetParent(gridCanvas.transform, false);
    }

    public void Refresh()
    {
        //hexMesh.Triangulate(cells);
        enabled = true;
    }

    private void LateUpdate()
    {
        hexMesh.Triangulate(cells);
        enabled = false;
    }
}

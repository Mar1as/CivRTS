using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using Color = UnityEngine.Color;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Lol : MonoBehaviour
{
    [SerializeField] private int width = 6, height = 6;
    public Color defaultColor = Color.white;
    public Color touchedColor = Color.magenta;
    public MainHexCell cellPrefab;

    public TextMeshProUGUI cellLabelPrefab;

    Canvas gridCanvas;

    HexMesh hexMesh;

    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        CivGameManagerSingleton.Instance.hexagons = new MainHexCell[width * height];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void Start()
    {
        hexMesh.Triangulate(CivGameManagerSingleton.Instance.hexagons);
    }

    private void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        MainHexCell cell = CivGameManagerSingleton.Instance.hexagons[i] = Instantiate(cellPrefab);
        cell.Inicilizace();
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.dataHexCell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.dataHexCell.color = defaultColor;

        if (x > 0)
        {
            Debug.Log(CivGameManagerSingleton.Instance.hexagons.Length);
            cell.brainHexCell.SetNeighbor(HexDirection.W, CivGameManagerSingleton.Instance.hexagons[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.brainHexCell.SetNeighbor(HexDirection.SE, CivGameManagerSingleton.Instance.hexagons[i - width]);
                if (x > 0)
                {
                    cell.brainHexCell.SetNeighbor(HexDirection.SW, CivGameManagerSingleton.Instance.hexagons[i - width - 1]);
                }
            }
            else
            {
                cell.brainHexCell.SetNeighbor(HexDirection.SW, CivGameManagerSingleton.Instance.hexagons[i - width]);
                if (x < width - 1)
                {
                    cell.brainHexCell.SetNeighbor(HexDirection.SE, CivGameManagerSingleton.Instance.hexagons[i - width + 1]);
                }
            }
        }

        TextMeshProUGUI label = Instantiate(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.dataHexCell.coordinates.ToStringOnSeparateLines();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(inputRay.origin, inputRay.direction * 1000000, Color.red, 10f); // Debugging (optional
        if (Physics.Raycast(inputRay, out hit))
        {
            TouchCell(hit.point);
        }
    }

    void TouchCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log($"Touched at {coordinates.X}, {coordinates.Y}, {coordinates.Z}");
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        MainHexCell cell = CivGameManagerSingleton.Instance.hexagons[index];
        cell.dataHexCell.color = touchedColor;
        hexMesh.Triangulate(CivGameManagerSingleton.Instance.hexagons);
    }
}
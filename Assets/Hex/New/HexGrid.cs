using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class HexGrid : MonoBehaviour
{
    public int chunkCountX = 4, chunkCountZ = 3;
    private int cellCountX = 6, cellCountZ = 6;

    public Color defaultColor = Color.white;

    public MainHexCell cellPrefab;
    public TextMeshProUGUI cellLabelPrefab;
    public HexGridChunk chunkPrefab;
    public Texture2D noiseSource;

    HexGridChunk[] chunks;

    private void Awake()
    {
        HexMetrics.noiseSource = noiseSource;

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
    }

    void Start()
    {

    }

    void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells()
    {
        CivGameManagerSingleton.Instance.hexagons = new MainHexCell[cellCountX * cellCountZ];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        MainHexCell cell = CivGameManagerSingleton.Instance.hexagons[i] = Instantiate(cellPrefab);
        cell.Inicilizace();
        //cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.dataHexCell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.dataHexCell.color = defaultColor;

        if (x > 0)
        {
            cell.brainHexCell.SetNeighbor(HexDirection.W, CivGameManagerSingleton.Instance.hexagons[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.brainHexCell.SetNeighbor(HexDirection.SE, CivGameManagerSingleton.Instance.hexagons[i - cellCountX]);
                if (x > 0)
                {
                    cell.brainHexCell.SetNeighbor(HexDirection.SW, CivGameManagerSingleton.Instance.hexagons[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.brainHexCell.SetNeighbor(HexDirection.SW, CivGameManagerSingleton.Instance.hexagons[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.brainHexCell.SetNeighbor(HexDirection.SE, CivGameManagerSingleton.Instance.hexagons[i - cellCountX + 1]);
                }
            }
        }

        TextMeshProUGUI label = Instantiate(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.dataHexCell.coordinates.ToStringOnSeparateLines();

        cell.dataHexCell.uiRect = label.rectTransform;
        cell.dataHexCell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, MainHexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    void RandomColor()
    {
        Color[] colors = { Color.blue, Color.white, Color.green, Color.yellow };
        foreach (MainHexCell cell in CivGameManagerSingleton.Instance.hexagons)
        {
            
            Color color = colors[Random.Range(0,colors.Length)];
            cell.dataHexCell.color = color;
            cell.dataHexCell.Elevation = Random.Range(0, 4);
        }
        //hexMesh.Triangulate(CivGameManagerSingleton.Instance.hexagons);
    }

    void RandomColor(int colorsCount)
    {
        Color[] c = new Color[colorsCount];
        for (int i = 0; i < colorsCount; i++)
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            c[i] = new Color(r, g, b);
        }
        foreach (MainHexCell cell in CivGameManagerSingleton.Instance.hexagons)
        {
            cell.dataHexCell.color = c[Random.Range(0, c.Length)];
        }
        //hexMesh.Triangulate(CivGameManagerSingleton.Instance.hexagons);
    }

    public MainHexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return CivGameManagerSingleton.Instance.hexagons[index];
    }

}
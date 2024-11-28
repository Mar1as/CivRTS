using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using TMPro;
using Unity.Loading;
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

    public MainHexCell cellPrefab;
    public TextMeshProUGUI cellLabelPrefab;
    public HexGridChunk chunkPrefab;
    public Texture2D noiseSource;

    public MainHexUnit unitPrefab;

    public int seed;

    HexGridChunk[] chunks;

    public Color[] colors;

    private void Awake()
    {
        HexMetrics.noiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);
        DataHexUnit.unitPrefab = unitPrefab;
        HexMetrics.colors = colors;

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();

        //RandomColor();
    }

    void Start()
    {

    }

    void OnEnable()
    {
        if (!HexMetrics.noiseSource)
        {
            HexMetrics.noiseSource = noiseSource;
            HexMetrics.InitializeHashGrid(seed);
            DataHexUnit.unitPrefab = unitPrefab;
            HexMetrics.colors = colors;
        }
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
            //cell.dataHexCell.color = color;
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
            //cell.dataHexCell.color = c[Random.Range(0, c.Length)];
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

    #region Save Load Manager

    public void Save(BinaryWriter writer)
    {
        for (int i = 0; i < CivGameManagerSingleton.Instance.hexagons.Length; i++)
        {
            CivGameManagerSingleton.Instance.hexagons[i].dataHexCell.saveLoadHexCell.Save(writer);
        }

        writer.Write(CivGameManagerSingleton.Instance.allUnits.Count);
        for (int i = 0; i < CivGameManagerSingleton.Instance.allUnits.Count; i++)
        {
            CivGameManagerSingleton.Instance.allUnits[i].dataHexUnit.Save(writer);
        }
    }

    public void Load(BinaryReader reader)
    {
        int header = reader.ReadInt32();
        ClearUnits();

        for (int i = 0; i < CivGameManagerSingleton.Instance.hexagons.Length; i++)
        {
            CivGameManagerSingleton.Instance.hexagons[i].dataHexCell.saveLoadHexCell.Load(reader);
        }
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].Refresh();
        }
        if (header >= 2)
        {
            int unitCount = reader.ReadInt32();
            for (int i = 0; i < unitCount; i++)
            {
                MainHexUnit.Load(reader, this);
            }
        }
    }

    #endregion

    #region Distance

    HexCellPriorityQueue searchFrontier;
    int searchFrontierPhase;

    public void FindPath(MainHexCell fromCell, MainHexCell toCell, int speed)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Search(fromCell, toCell, speed);
        sw.Stop();
        Debug.Log(sw.ElapsedMilliseconds);
    }

    void Search(MainHexCell fromCell, MainHexCell toCell, int speed)
    {
        searchFrontierPhase += 2;

        if (searchFrontier == null)
        {
            searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }

        for (int i = 0; i < CivGameManagerSingleton.Instance.hexagons.Length; i++)
        {
            //CivGameManagerSingleton.Instance.hexagons[i].dataHexCell.hexCellDistance.Distance = int.MaxValue;
            CivGameManagerSingleton.Instance.hexagons[i].dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text = "";
        }

        fromCell.dataHexCell.hexCellDistance.SearchPhase = searchFrontierPhase;
        fromCell.dataHexCell.hexCellDistance.Distance = 0;
        searchFrontier.Enqueue(fromCell);
        while (searchFrontier.Count > 0)
        {
            MainHexCell current = searchFrontier.Dequeue();
            current.dataHexCell.hexCellDistance.SearchPhase += 1;

            if (current == toCell)
            {
                current = current.dataHexCell.hexCellDistance.PathFrom;
                while (current != fromCell)
                {
                    current.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text += " PATH";
                    current = current.dataHexCell.hexCellDistance.PathFrom;
                }
                break;
            }

            int currentTurn = current.dataHexCell.hexCellDistance.Distance / speed;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                MainHexCell neighbor = current.brainHexCell.GetNeighbor(d);
                if (neighbor == null || neighbor.dataHexCell.hexCellDistance.SearchPhase > searchFrontierPhase) continue;
                if (neighbor.dataHexCell.waterScript.IsUnderwater) continue;
                HexEdgeType edgeType = current.brainHexCell.GetEdgeType(neighbor);
                if (edgeType == HexEdgeType.Cliff) continue;

                int moveCost = 0;
                if (current.dataHexCell.roadScript.HasRoadThroughEdge(d))
                {
                    moveCost += 1;
                }
                else if (current.dataHexCell.wallsScript.Walled != neighbor.dataHexCell.wallsScript.Walled)
                {
                    continue;
                }
                else
                {
                    moveCost += edgeType == HexEdgeType.Flat ? 5 : 10;
                    moveCost += neighbor.dataHexCell.featuresHexCell.UrbanLevel + neighbor.dataHexCell.featuresHexCell.FarmLevel + neighbor.dataHexCell.featuresHexCell.PlantLevel;
                }

                int distance = current.dataHexCell.hexCellDistance.Distance + moveCost;
                int turn = distance / speed;
                if (turn > currentTurn)
                {
                    distance = turn * speed + moveCost;
                }

                if (neighbor.dataHexCell.hexCellDistance.SearchPhase < searchFrontierPhase)
                {
                    neighbor.dataHexCell.hexCellDistance.SearchPhase = searchFrontierPhase;
                    neighbor.dataHexCell.hexCellDistance.Distance = distance;
                    neighbor.dataHexCell.hexCellDistance.PathFrom = current;
                    neighbor.dataHexCell.hexCellDistance.SearchHeuristic = neighbor.dataHexCell.coordinates.DistanceTo(toCell.dataHexCell.coordinates);
                    searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.dataHexCell.hexCellDistance.Distance)
                {
                    int oldPriority = neighbor.dataHexCell.hexCellDistance.SearchPriority;
                    neighbor.dataHexCell.hexCellDistance.Distance = distance;
                    neighbor.dataHexCell.hexCellDistance.PathFrom = current;
                    searchFrontier.Change(neighbor, oldPriority);
                }


            }
        }
        fromCell.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text += " START";
        toCell.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text += " END";
    }

    #endregion

    #region Units

    void ClearUnits()
    {
        for (int i = 0; i < CivGameManagerSingleton.Instance.allUnits.Count; i++)
        {
            CivGameManagerSingleton.Instance.allUnits[i].Die();
        }
        CivGameManagerSingleton.Instance.allUnits.Clear();
    }

    public void AddUnit(MainHexUnit unit, MainHexCell location, float orientation)
    {
        CivGameManagerSingleton.Instance.allUnits.Add(unit);
        unit.transform.SetParent(transform, false);
        unit.dataHexUnit.Location = location;
        unit.dataHexUnit.Orientation = orientation;
    }
    public void RemoveUnit(MainHexUnit unit)
    {
        CivGameManagerSingleton.Instance.allUnits.Remove(unit);
        unit.Die();
    }
    #endregion
}
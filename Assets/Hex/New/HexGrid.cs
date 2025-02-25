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

    MainHexCell currentPathFrom, currentPathTo;
    bool currentPathExists;

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

        CivGameManagerSingleton.Instance.hexGrid = this;
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
        //label.text = cell.dataHexCell.coordinates.ToStringOnSeparateLines();

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
        Debug.Log($"Ukládání mapy. Poèet hexagonù: {CivGameManagerSingleton.Instance.hexagons.Length}");
        for (int i = 0; i < CivGameManagerSingleton.Instance.hexagons.Length; i++)
        {
            CivGameManagerSingleton.Instance.hexagons[i].dataHexCell.saveLoadHexCell.Save(writer);
        }
        Debug.Log("Ukládání dokonèeno.");
    }

    public void Load(BinaryReader reader)
    {
        try
        {
            Debug.Log("Naèítání mapy.");
            int header = 0;
            Debug.Log($"Naètený header: {header}");
            ClearPath();
            ClearUnits();

            for (int i = 0; i < CivGameManagerSingleton.Instance.hexagons.Length; i++)
            {
                Debug.Log(reader.BaseStream.Position);
                Debug.Log(reader.BaseStream.Length);

                if (reader.BaseStream.Position >= reader.BaseStream.Length)
                {
                    Debug.LogError("Nedostatek dat pro naètení hexagonu.");
                    break;
                }
                Debug.Log($"Naèítání hexagonu {i}");
                CivGameManagerSingleton.Instance.hexagons[i].dataHexCell.saveLoadHexCell.Load(reader);
            }

            for (int i = 0; i < chunks.Length; i++)
            {
                chunks[i].Refresh();
            }

            if (header >= 2)
            {
                //int unitCount = reader.ReadInt32();
                /*
                for (int i = 0; i < unitCount; i++)
                {
                    MainHexUnit.Load(reader, this);
                }*/
            }
            Debug.Log("Naèítání dokonèeno.");
        }
        catch (EndOfStreamException ex)
        {
            Debug.LogError($"Chyba pøi ètení dat: {ex.Message}");
        }
    }

    #endregion

    #region Distance

    HexCellPriorityQueue searchFrontier;
    int searchFrontierPhase;

    public void FindPath(MainHexCell fromCell, MainHexCell toCell, int speed, UnitAtDestination unitAtD)
    {
        ClearPath();
        currentPathFrom = fromCell;
        currentPathTo = toCell;
        currentPathExists = Search(fromCell, toCell, speed, unitAtD);
        ShowPath(speed);
    }

    bool Search(MainHexCell fromCell, MainHexCell toCell, int speed, UnitAtDestination unitAtDest)
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

        fromCell.dataHexCell.hexCellDistance.SearchPhase = searchFrontierPhase;
        fromCell.dataHexCell.hexCellDistance.Distance = 0;
        searchFrontier.Enqueue(fromCell);

        MainHexCell previousCell = null;

        while (searchFrontier.Count > 0)
        {
            MainHexCell current = searchFrontier.Dequeue();
            current.dataHexCell.hexCellDistance.SearchPhase += 1;
            
            if (current == toCell) //Konec
            {
                return true;
            }

            previousCell = current;

            int currentTurn = (current.dataHexCell.hexCellDistance.Distance - 1) / speed;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                MainHexCell neighbor = current.brainHexCell.GetNeighbor(d);
                if (neighbor == null || neighbor.dataHexCell.hexCellDistance.SearchPhase > searchFrontierPhase) continue;
                if ((neighbor.dataHexCell.waterScript.IsUnderwater || neighbor.dataHexCell.Unit)) continue;
                HexEdgeType edgeType = current.brainHexCell.GetEdgeType(neighbor);
                if (edgeType == HexEdgeType.Cliff) continue;

                int moveCost = 0;
                if (current.dataHexCell.roadScript.HasRoadThroughEdge(d))
                {
                    moveCost += 1;
                }/*
                else if (current.dataHexCell.wallsScript.Walled != neighbor.dataHexCell.wallsScript.Walled) //Nemùže procházet zdí
                {
                    //continue;
                }*/
                else
                {
                    moveCost += edgeType == HexEdgeType.Flat ? 5 : 10;
                    moveCost += neighbor.dataHexCell.featuresHexCell.UrbanLevel + neighbor.dataHexCell.featuresHexCell.FarmLevel + neighbor.dataHexCell.featuresHexCell.PlantLevel;
                }

                int distance = current.dataHexCell.hexCellDistance.Distance + moveCost;
                int turn = (current.dataHexCell.hexCellDistance.Distance - 1) / speed;
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
        //fromCell.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text += " START";
        toCell.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text += $"CURRENT";

        return false;
    }

    void ShowPath(int speed)
    {
        int[] turns;
        if (currentPathExists)
        {
            MainHexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                int turn = (current.dataHexCell.hexCellDistance.Distance - 1) / speed;
                //current.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text += turn.ToString() + " Path";
                //current.SetLabel(turn.ToString());
                //current.EnableHighlight(Color.white);
                current = current.dataHexCell.hexCellDistance.PathFrom;
                Debug.Log(turn);
                
            }
        }
        int lastTurn = (currentPathTo.dataHexCell.hexCellDistance.Distance - 1) / speed;
        Debug.Log("Turn " + lastTurn);
        //currentPathFrom.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text += " Start";
        currentPathTo.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text += lastTurn;
        //currentPathFrom.EnableHighlight(Color.blue);
        //currentPathTo.EnableHighlight(Color.red);
    }

    public void ClearPath()
    {
        if (currentPathExists)
        {
            MainHexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text = "";//current.dataHexCell.coordinates.ToStringOnSeparateLines();
                //current.SetLabel(null);
                //current.DisableHighlight();
                current = current.dataHexCell.hexCellDistance.PathFrom;
            }
            current.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text = "";//current.dataHexCell.coordinates.ToStringOnSeparateLines();
            //current.DisableHighlight();
            currentPathExists = false;
        }
        else if (currentPathFrom)
        {
            currentPathFrom.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text = "";//currentPathFrom.dataHexCell.coordinates.ToStringOnSeparateLines();
            //currentPathFrom.DisableHighlight();
            currentPathTo.dataHexCell.uiRect.GetComponent<TextMeshProUGUI>().text = "";//currentPathTo.dataHexCell.coordinates.ToStringOnSeparateLines();
            //currentPathTo.DisableHighlight();
        }
        currentPathFrom = currentPathTo = null;
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
        MainHexUnit newUnit = MainHexUnit.Instantiate(unit);
        CivGameManagerSingleton.Instance.allUnits.Add(newUnit);
        newUnit.transform.SetParent(transform, false);
        newUnit.dataHexUnit.Location = location;
        newUnit.dataHexUnit.Orientation = orientation;
    }
    public void AddUnit(MainHexUnit unit, MainHexCell location, float orientation, DataHexUnitArmy army, Player player)
    {
        Debug.Log("Velikost: " + army.unitsInArmy.Count);
        MainHexUnit newUnit = MainHexUnit.Instantiate(unit);
        newUnit.Inicilizace(player,army);
       
        CivGameManagerSingleton.Instance.allUnits.Add(newUnit);
        newUnit.transform.SetParent(transform, false);
        Debug.Log(newUnit.dataHexUnit);
        Debug.Log(location);
        newUnit.dataHexUnit.Location = location;
        newUnit.dataHexUnit.Orientation = orientation;
        Debug.Log("Velikost: " + newUnit.dataHexUnit.armyHexUnit.unitsInArmy.Count);

        player.Armies.Add(unit.gameObject);
    }

    public void RemoveUnit(MainHexUnit unit)
    {
        CivGameManagerSingleton.Instance.allUnits.Remove(unit);
        unit.Die();
    }

    public bool HasPath
    {
        get
        {
            return currentPathExists;
        }
    }

    public List<MainHexCell> GetPath()
    {
        if (!currentPathExists)
        {
            return null;
        }
        List<MainHexCell> path = ListPool<MainHexCell>.Get();
        for (MainHexCell c = currentPathTo; c != currentPathFrom; c = c.dataHexCell.hexCellDistance.PathFrom)
        {
            path.Add(c);
        }
        path.Add(currentPathFrom);
        path.Reverse();
        return path;
    }
    #endregion

    public MainHexCell GetCell(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return GetCell(hit.point);
        }
        return null;
    }
}
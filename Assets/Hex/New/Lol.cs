using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Vector3 = UnityEngine.Vector3;

public class Lol : MonoBehaviour
{
    [SerializeField] Canvas gridCanvas;
    [SerializeField] TextMeshProUGUI cellLabelPrefab;

    [SerializeField] private Vector2Int gridSize;

    [SerializeField] Material material;
    Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white };
    List<Color> col = new List<Color>();

    void OnEnable()
    {
        CreateGrid();

        /*GameObject[] gms = CivGameManagerSingleton.Instance.hexagons[3, 3].GetComponent<MainHexCell>().dataProvince.NeighboringProvinces();
        foreach (var item in gms)
        {
            if (item != null)
            {
                item.GetComponent<MeshRenderer>().material.color = Color.black;
            }
        }*/
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            //CreateGrid();
        }
    }

    void CreateGrid()
    {

        for (int z = 0, i = 0; z < gridSize.x; z++)
        {
            for (int x = 0; x < gridSize.y; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }
    void CreateCell(int x, int z, int i)
    {
        col.Clear();
        GameObject gm = new GameObject($"Hex: {x}, {z}");
        MeshRenderer meshRenderer = gm.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = gm.AddComponent<MeshFilter>();
        MeshCollider meshCollider = gm.AddComponent<MeshCollider>();
        Mesh mesh = CreateMesh();
        Debug.Log(mesh.colors.Length + " " + col.Count);
        mesh.colors = col.ToArray();
        meshCollider.sharedMesh = mesh;
        meshFilter.mesh = mesh;
        //meshRenderer.material = material;

        

        TextMeshProUGUI label = Instantiate<TextMeshProUGUI>(cellLabelPrefab);

        gm.transform.parent = this.transform;
        
        gm.transform.localPosition = new UnityEngine.Vector3((x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f), 0, z * (HexMetrics.outerRadius * 1.5f));
        label.rectTransform.anchoredPosition = new UnityEngine.Vector2((x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f), z * (HexMetrics.outerRadius * 1.5f));

        //meshRenderer.material.color = colors[Random.Range(0, colors.Length)];

        MainHexCell mainHexCell = gm.AddComponent<MainHexCell>();  // Pøidej komponentu MainHexCell pøímo
                                                                   //mainHexCell.dataHexCell = new DataHexCell();  // Inicializuj DataProvince, pokud není již inicializováno
        mainHexCell.Inicilizace();
        mainHexCell.dataHexCell.coordinates = new HexCoordinates(x - z / 2, z);  // Nastav HexCoordinates
        if (x > 0)
        {
            mainHexCell.brainHexCell.SetNeighbor(HexDirection.W, CivGameManagerSingleton.Instance.hexagons[i - 1]);
        }
        if (z > 0)
        {
            if ((z % 2) == 0)
            {
                mainHexCell.brainHexCell.SetNeighbor(HexDirection.SE, CivGameManagerSingleton.Instance.hexagons[i - gridSize.y]);
                if (x > 0)
                {
                    mainHexCell.brainHexCell.SetNeighbor(HexDirection.SW, CivGameManagerSingleton.Instance.hexagons[i - gridSize.y - 1]);
                }
            }
            else
            {
                mainHexCell.brainHexCell.SetNeighbor(HexDirection.SW, CivGameManagerSingleton.Instance.hexagons[i - gridSize.y]);
                if (x < gridSize.y - 1)
                {
                    mainHexCell.brainHexCell.SetNeighbor(HexDirection.SE, CivGameManagerSingleton.Instance.hexagons[i - gridSize.y + 1]);
                }
            }
        }
        CivGameManagerSingleton.Instance.hexagons.Add(mainHexCell);

        //CivGameManagerSingleton.Instance.hexagons[x, y] = gm;

        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.text = $"X: {mainHexCell.dataHexCell.coordinates.X}\nY: {mainHexCell.dataHexCell.coordinates.Y}\nZ: {mainHexCell.dataHexCell.coordinates.Z}";

    }

    Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[7];
        int[] triangles = new int[18];

        vertices[0] = new Vector3(0, 0, 0);

        for (int i = 0; i < 6; i++) //Verticies
        {            
            float angleDeg = 60 * i - 30;
            float angleRad = Mathf.Deg2Rad * angleDeg;
            vertices[i + 1] = new Vector3(-1 * Mathf.Cos(angleRad), 0, 1 * Mathf.Sin(angleRad)) * HexMetrics.outerRadius;
        }
            
        for (int i = 0; i < 6; i++) //Triangles
        {

            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i == 5 ? 1 : i + 2;
            col.Add(Color.blue);
            col.Add(Color.blue);
            col.Add(Color.blue);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
        
    }

}

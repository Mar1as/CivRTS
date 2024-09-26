using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Lol : MonoBehaviour
{
    [SerializeField] Canvas gridCanvas;
    [SerializeField] TextMeshProUGUI cellLabelPrefab;


    [SerializeField] Vector2Int gridSize;
    [SerializeField] Material material;
    Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white };

    void OnEnable()
    {
        CivGameManagerSingleton.Instance.hexagons = new GameObject[gridSize.x,gridSize.y];
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
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GameObject gm = new GameObject($"Hex: {x}, {y}");
                MeshRenderer meshRenderer = gm.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = gm.AddComponent<MeshFilter>();
                Mesh mesh = CreateMesh();
                meshFilter.mesh = mesh;
                meshRenderer.material = material;

                float width = Mathf.Sqrt(3);
                float height = 3f / 2f;
                TextMeshProUGUI label = Instantiate<TextMeshProUGUI>(cellLabelPrefab);

                gm.transform.parent = this.transform;
                if (y % 2 == 0)
                {
                    gm.transform.localPosition = new Vector3(width * x, 0, height * y);
                    label.rectTransform.anchoredPosition = new Vector2(width * x, height * y);
                }
                else
                {
                    gm.transform.localPosition = new Vector3(width * x + width / 2, 0, height * y);
                    label.rectTransform.anchoredPosition = new Vector2(width * x + width / 2, height * y);
                }

                

                meshRenderer.material.color = colors[Random.Range(0, colors.Length)];
                //MainHexCell mainHexCell = new MainHexCell();
                
                
                //CivGameManagerSingleton.Instance.hexagons[x, y] = gm;

                label.rectTransform.SetParent(gridCanvas.transform, false);
                label.text = x.ToString() + "\n" + y.ToString();
            }
        }
    }
    Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[7];
        int[] triangles = new int[18];

        vertices[0] = new Vector3(0, 0, 0);

        for (int i = 0; i < 6; i++)
        {            
            float angleDeg = 60 * i - 30;
            float angleRad = Mathf.Deg2Rad * angleDeg;
            vertices[i + 1] = new Vector3(-1 * Mathf.Cos(angleRad), 0, 1 * Mathf.Sin(angleRad));
        }
            
        for (int i = 0; i < 6; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i == 5 ? 1 : i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
        
    }

    public enum TerrainType
    {
        None,
        Plains, // Planina
        Forest, // Les
        Hill,   // Kopec
        Mountain, // Hora
        Water   // Voda
    }
}

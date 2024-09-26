using UnityEngine;

public class HexagonGrid : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float hexRadius = 1f;
    public Material hexMaterial;

    void Start()
    {
        GenerateHexGrid();
    }

    void GenerateHexGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = CalculateHexPosition(x, y);
                Mesh hexMesh = CreateHexMesh(hexRadius);

                GameObject hex = new GameObject("Hex " + x + " " + y);
                hex.transform.position = position;
                hex.transform.parent = this.transform;

                MeshFilter mf = hex.AddComponent<MeshFilter>();
                mf.mesh = hexMesh;

                MeshRenderer mr = hex.AddComponent<MeshRenderer>();
                mr.material = hexMaterial;
            }
        }
    }

    // Vypoèítá pozici hexagonu ve møížce pro pointy-top orientaci
    Vector3 CalculateHexPosition(int x, int y)
    {
        float width = Mathf.Sqrt(3) * hexRadius;  // Šíøka hexagonu (pro pointy-top)
        float height = 2 * hexRadius;  // Výška hexagonu

        // X pozice je násobek šíøky
        float xPos = x * width * 0.75f;  // 0.75f = rozestup mezi sousedními hexagony
        if (y % 2 != 0)  // Posunutí lichých øádkù
        {
            xPos += width * 0.75f;
        }

        // Z pozice je násobek výšky s odsazením
        float zPos = y * (height * 0.87f);  // 0.87f = optimalizovaný rozestup

        return new Vector3(xPos, 0, zPos);
    }

    // Procedurální generování hexagonu s pointy-top orientací
    Mesh CreateHexMesh(float radius)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[7];  // 6 vrcholù a 1 støedový bod
        int[] triangles = new int[18];  // 6 trojúhelníkù

        vertices[0] = Vector3.zero;  // Støed hexagonu

        for (int i = 0; i < 6; i++)
        {
            float angleDeg = 60 * i - 30;  // Pointy-top hexagon má úhel otoèený o -30 stupòù
            float angleRad = Mathf.Deg2Rad * angleDeg;
            vertices[i + 1] = new Vector3(radius * Mathf.Cos(angleRad), 0, radius * Mathf.Sin(angleRad));
        }

        // Vytváøí trojúhelníky hexagonu
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
}

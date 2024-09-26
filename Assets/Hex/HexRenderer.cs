using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pomoc: https://www.youtube.com/watch?v=EPaSmQ2vtek
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    List<Face> faces;

    public Material material;

    public float innerSize, outerSize, height;

    public bool isFlatToped;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        mesh = new Mesh();
        mesh.name = "Hex";

        meshFilter.mesh = mesh;
        meshRenderer.material = material;
    }

    private void OnEnable()
    {
        DrawMesh();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            DrawMesh();
        }
    }

    public void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
    }

    void DrawFaces()
    {
        faces = new List<Face>();

        for (int i = 0; i < 6; i++)
        {
            faces.Add(CreateFace(innerSize, outerSize, height / 2f, height / 2f, i));
        }
        for (int i = 0; i < 6; i++)
        {
            faces.Add(CreateFace(innerSize, outerSize, -height / 2f, -height / 2f, i, true));
        }
        for (int i = 0; i < 6; i++)
        {
            faces.Add(CreateFace(outerSize, outerSize, height / 2f, -height / 2f, i, true));
        }
        for (int i = 0; i < 6; i++)
        {
            faces.Add(CreateFace(innerSize, innerSize, height / 2f, -height / 2f, i, false));
        }
    }

    void CombineFaces()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < faces.Count; i++)
        {
            vertices.AddRange(faces[i].vertices);
            uvs.AddRange(faces[i].uvs);

            int offset = 4 * i;
            foreach (int triangle in faces[i].triangles)
            {
                triangles.Add(triangle + offset);
            }
        }

         mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        
    }

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
    }

    Vector3 GetPoint(float size, float height, int index)
    {
        float angleDeg = isFlatToped ? 60 * index : 60 * index - 30;
        float angleRad = Mathf.PI / 180f * angleDeg;
        return new Vector3(size * Mathf.Cos(angleRad), height, size * Mathf.Sin(angleRad));
    }

    Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false)
    {
        Vector3 pointA = GetPoint(innerRad, heightB, point);
        Vector3 pointB = GetPoint(innerRad, heightB, (point < 5) ? point + 1 : 0);
        Vector3 pointC = GetPoint(outerRad, heightA, (point < 5) ? point + 1 : 0);
        Vector3 pointD = GetPoint(outerRad, heightA, point);

        List<Vector3> vertices = new List<Vector3>() { pointA, pointB, pointC, pointD };
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
        if (reverse)
        {
            vertices.Reverse();
        }

        return new Face(vertices,triangles,uvs);
    }

    public struct Face
    {
        public List<Vector3> vertices { get; private set; }
        public List<int> triangles { get; private set; }
        public List<Vector2> uvs { get; private set; }

        public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.uvs = uvs;
        }
    }
}

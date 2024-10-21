using System;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{

    Mesh hexMesh;
    MeshCollider meshCollider;

    static List<Vector3> vertices = new List<Vector3>();
    static List<Color> colors = new List<Color>();
    static List<int> triangles = new List<int>();

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
        
    }

    public void Triangulate(MainHexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        colors.Clear();
        triangles.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();
        meshCollider.sharedMesh = hexMesh;
    }

    void Triangulate(MainHexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }
    }

    void Triangulate(HexDirection direction, MainHexCell cell)
    {
        Vector3 center = cell.dataHexCell.Position;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

        AddTriangle(center, v1, v2);
        AddTriangleColor(cell.dataHexCell.color);

        TriangulateConnection(direction, cell, v1, v2);
    }

    private void TriangulateConnection(HexDirection direction, MainHexCell cell, Vector3 v1, Vector3 v2)
    {
        MainHexCell neighbor = cell.brainHexCell.GetNeighbor(direction);
        if (neighbor == null) return; 

        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = neighbor.dataHexCell.Position.y;

        if (cell.brainHexCell.GetEdgeType(direction) == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbor);
        }
        else
        {
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(cell.dataHexCell.color, neighbor.dataHexCell.color);
        }


        MainHexCell nextNeighbor = cell.brainHexCell.GetNeighbor(direction.Next());
        
        if(direction <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.dataHexCell.Position.y;

            if (cell.dataHexCell.Elevation <= neighbor.dataHexCell.Elevation)
            {
                if (cell.dataHexCell.Elevation <= nextNeighbor.dataHexCell.Elevation)
                {
                    TriangulateCorner(v2, cell, v4, neighbor, v5, nextNeighbor);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
                }
            }
            else if (neighbor.dataHexCell.Elevation <= nextNeighbor.dataHexCell.Elevation)
            {
                TriangulateCorner(v4, neighbor, v5, nextNeighbor, v2, cell);
            }
            else
            {
                TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
            }

            //AddTriangle(v2, v4, v5);
            //AddTriangleColor(cell.dataHexCell.color, neighbor.dataHexCell.color, nextNeighbor.dataHexCell.color);
        }
    }

    void TriangulateEdgeTerraces(Vector3 beginLeft, Vector3 beginRight, MainHexCell beginCell, Vector3 endLeft, Vector3 endRight, MainHexCell endCell)
    {
        Vector3 v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.dataHexCell.color, endCell.dataHexCell.color, 1);

        AddQuad(beginLeft, beginRight, v3, v4);
        AddQuadColor(beginCell.dataHexCell.color, c2);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c2;
            v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
            v4 = HexMetrics.TerraceLerp(beginRight, endRight, i);
            c2 = HexMetrics.TerraceLerp(beginCell.dataHexCell.color, endCell.dataHexCell.color, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2);
        }

        AddQuad(v3, v4, endLeft, endRight);
        AddQuadColor(c2, endCell.dataHexCell.color);
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    void AddTriangleColor(Color color1, Color color2, Color color3)
    {
        colors.Add(color1);
        colors.Add(color2);
        colors.Add(color3);
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        vertices.Add(Perturb(v4));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    void AddQuadColor(Color c1, Color c2)
    {
        colors.Add(c1);
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c2);
    }

    void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }

    void TriangulateCorner(
        Vector3 bottom, MainHexCell bottomCell,
        Vector3 left, MainHexCell leftCell,
        Vector3 right, MainHexCell rightCell
    )
    {
        HexEdgeType leftEdgeType = bottomCell.brainHexCell.GetEdgeType(leftCell);
        HexEdgeType rightEdgeType = bottomCell.brainHexCell.GetEdgeType(rightCell);

        if (leftEdgeType == HexEdgeType.Slope)
        {
            if (rightEdgeType == HexEdgeType.Slope)
            {
                TriangulateCornerTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
            else if (rightEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
            else
            {
                TriangulateCornerTerracesCliff(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        }
        else if (rightEdgeType == HexEdgeType.Slope)
        {
            if (leftEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            }
            else
            {
                TriangulateCornerCliffTerraces(
                    bottom, bottomCell, left, leftCell, right, rightCell
                );
            }
        }
        else if (leftCell.brainHexCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            if (leftCell.dataHexCell.Elevation < rightCell.dataHexCell.Elevation)
            {
                TriangulateCornerCliffTerraces(
                    right, rightCell, bottom, bottomCell, left, leftCell
                );
            }
            else
            {
                TriangulateCornerTerracesCliff(
                    left, leftCell, right, rightCell, bottom, bottomCell
                );
            }
        }
        else
        {
            AddTriangle(bottom, left, right);
            AddTriangleColor(bottomCell.dataHexCell.color, leftCell.dataHexCell.color, rightCell.dataHexCell.color);
        }
    }

    void TriangulateCornerTerraces(
        Vector3 begin, MainHexCell beginCell,
        Vector3 left, MainHexCell leftCell,
        Vector3 right, MainHexCell rightCell)
    {
        Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
        Color c3 = HexMetrics.TerraceLerp(beginCell.dataHexCell.color, leftCell.dataHexCell.color, 1);
        Color c4 = HexMetrics.TerraceLerp(beginCell.dataHexCell.color, rightCell.dataHexCell.color, 1);

        AddTriangle(begin, v3, v4);
        AddTriangleColor(beginCell.dataHexCell.color, c3, c4);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c3;
            Color c2 = c4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            c3 = HexMetrics.TerraceLerp(beginCell.dataHexCell.color, leftCell.dataHexCell.color, i);
            c4 = HexMetrics.TerraceLerp(beginCell.dataHexCell.color, rightCell.dataHexCell.color, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2, c3, c4);
        }

        AddQuad(v3, v4, left, right);
        AddQuadColor(c3, c4, leftCell.dataHexCell.color, rightCell.dataHexCell.color);
    }

    void TriangulateCornerTerracesCliff(
        Vector3 begin, MainHexCell beginCell,
        Vector3 left, MainHexCell leftCell,
        Vector3 right, MainHexCell rightCell)
    {
        float b = 1f / (rightCell.dataHexCell.Elevation - beginCell.dataHexCell.Elevation);
        if (b < 0)
        {
            b = -b;
        }
        Vector3 boundary = Vector3.Lerp(begin, right, b);
        Color boundaryColor = Color.Lerp(beginCell.dataHexCell.color, rightCell.dataHexCell.color, b);

        TriangulateBoundaryTriangle(
            begin, beginCell, left, leftCell, boundary, boundaryColor
        );
        if (leftCell.brainHexCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        }
        else
        {
            AddTriangle(left, right, boundary);
            AddTriangleColor(leftCell.dataHexCell.color, rightCell.dataHexCell.color, boundaryColor);
        }
    }

    void TriangulateCornerCliffTerraces(
        Vector3 begin, MainHexCell beginCell,
        Vector3 left, MainHexCell leftCell,
        Vector3 right, MainHexCell rightCell
    )
    {
        float b = 1f / (leftCell.dataHexCell.Elevation - beginCell.dataHexCell.Elevation);
        if (b < 0)
        {
            b = -b;
        }
        Vector3 boundary = Vector3.Lerp(begin, left, b);
        Color boundaryColor = Color.Lerp(beginCell.dataHexCell.color, leftCell.dataHexCell.color, b);

        TriangulateBoundaryTriangle(
            right, rightCell, begin, beginCell, boundary, boundaryColor
        );

        if (leftCell.brainHexCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(
                left, leftCell, right, rightCell, boundary, boundaryColor
            );
        }
        else
        {
            AddTriangle(left, right, boundary);
            AddTriangleColor(leftCell.dataHexCell.color, rightCell.dataHexCell.color, boundaryColor);
        }
    }

    private void TriangulateBoundaryTriangle(Vector3 begin, MainHexCell beginCell, Vector3 left, MainHexCell leftCell, Vector3 boundary, Color boundaryColor)
    {

        Vector3 v2 = HexMetrics.TerraceLerp(begin, left, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.dataHexCell.color, leftCell.dataHexCell.color, 1);

        AddTriangle(begin, v2, boundary);
        AddTriangleColor(beginCell.dataHexCell.color, c2, boundaryColor);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v2;
            Color c1 = c2;
            v2 = HexMetrics.TerraceLerp(begin, left, i);
            c2 = HexMetrics.TerraceLerp(beginCell.dataHexCell.color, leftCell.dataHexCell.color, i);
            AddTriangle(v1, v2, boundary);
            AddTriangleColor(c1, c2, boundaryColor);
        }

        AddTriangle(v2, left, boundary);
        AddTriangleColor(c2, leftCell.dataHexCell.color, boundaryColor);
    }

    Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = HexMetrics.SampleNoise(position);
        position.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrength;
        //position.y += (sample.y * 2f - 1f) * HexMetrics.cellPerturbStrength;
        position.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrength;
        return position;
    }
}

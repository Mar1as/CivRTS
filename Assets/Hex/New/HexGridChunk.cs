using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    MainHexCell[] cells;

    public HexMesh terrain, rivers;
    Canvas gridCanvas;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        terrain = GetComponentInChildren<HexMesh>();

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
        Triangulate(cells);
        enabled = false;
    }

    public void Triangulate(MainHexCell[] cells)
    {
        terrain.Clear();
        rivers.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }

        terrain.Apply();
        rivers.Apply();
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
        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction)
        );

        if (cell.dataHexCell.river.HasRiver)
        {
            if (cell.dataHexCell.river.HasRiverThroughEdge(direction))
            {
                e.v3.y = cell.dataHexCell.StreamBedY;
                if (cell.dataHexCell.river.HasRiverBeginOrEnd)
                {
                    TriangulateWithRiverBeginOrEnd(direction, cell, center, e);
                }
                else
                {
                    TriangulateWithRiver(direction, cell, center, e);
                }
            }
            else
            {
                TriangulateAdjacentToRiver(direction, cell, center, e);
            }
        }

        else TriangulateEdgeFan(center, e, cell.dataHexCell.Color);

        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, e);
        }
    }

    void TriangulateConnection(
        HexDirection direction, MainHexCell cell, EdgeVertices e1
    )
    {
        MainHexCell neighbor = cell.brainHexCell.GetNeighbor(direction);
        if (neighbor == null)
        {
            return;
        }

        Vector3 bridge = HexMetrics.GetBridge(direction);
        bridge.y = neighbor.dataHexCell.Position.y - cell.dataHexCell.Position.y;
        EdgeVertices e2 = new EdgeVertices(
            e1.v1 + bridge,
            e1.v5 + bridge
        );

        if (cell.dataHexCell.river.HasRiverThroughEdge(direction))
        {
            e2.v3.y = neighbor.dataHexCell.StreamBedY;
            TriangulateRiverQuad(
                e1.v2, e1.v4, e2.v2, e2.v4,
                cell.dataHexCell.RiverSurfaceY, neighbor.dataHexCell.RiverSurfaceY, 0.8f,
                cell.dataHexCell.river.HasIncomingRiver && cell.dataHexCell.river.IncomingRiver == direction
            );
        }

        if (cell.brainHexCell.GetEdgeType(direction) == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(e1, cell, e2, neighbor);
        }
        else
        {
            TriangulateEdgeStrip(e1, cell.dataHexCell.Color, e2, neighbor.dataHexCell.Color);
        }

        MainHexCell nextNeighbor = cell.brainHexCell.GetNeighbor(direction.Next());
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 v5 = e1.v5 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.dataHexCell.Position.y;

            if (cell.dataHexCell.Elevation <= neighbor.dataHexCell.Elevation)
            {
                if (cell.dataHexCell.Elevation <= nextNeighbor.dataHexCell.Elevation)
                {
                    TriangulateCorner(
                        e1.v5, cell, e2.v5, neighbor, v5, nextNeighbor
                    );
                }
                else
                {
                    TriangulateCorner(
                        v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor
                    );
                }
            }
            else if (neighbor.dataHexCell.Elevation <= nextNeighbor.dataHexCell.Elevation)
            {
                TriangulateCorner(
                    e2.v5, neighbor, v5, nextNeighbor, e1.v5, cell
                );
            }
            else
            {
                TriangulateCorner(
                    v5, nextNeighbor, e1.v5, cell, e2.v5, neighbor
                );
            }
        }
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
            terrain.AddTriangle(bottom, left, right);
            terrain.AddTriangleColor(bottomCell.dataHexCell.Color, leftCell.dataHexCell.Color, rightCell.dataHexCell.Color);
        }
    }

    void TriangulateEdgeTerraces(
        EdgeVertices begin, MainHexCell beginCell,
        EdgeVertices end, MainHexCell endCell
    )
    {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.dataHexCell.Color, endCell.dataHexCell.Color, 1);

        TriangulateEdgeStrip(begin, beginCell.dataHexCell.Color, e2, c2);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            EdgeVertices e1 = e2;
            Color c1 = c2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            c2 = HexMetrics.TerraceLerp(beginCell.dataHexCell.Color, endCell.dataHexCell.Color, i);
            TriangulateEdgeStrip(e1, c1, e2, c2);
        }

        TriangulateEdgeStrip(e2, c2, end, endCell.dataHexCell.Color);
    }

    void TriangulateCornerTerraces(
        Vector3 begin, MainHexCell beginCell,
        Vector3 left, MainHexCell leftCell,
        Vector3 right, MainHexCell rightCell
    )
    {
        Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
        Color c3 = HexMetrics.TerraceLerp(beginCell.dataHexCell.Color, leftCell.dataHexCell.Color, 1);
        Color c4 = HexMetrics.TerraceLerp(beginCell.dataHexCell.Color, rightCell.dataHexCell.Color, 1);

        terrain.AddTriangle(begin, v3, v4);
        terrain.AddTriangleColor(beginCell.dataHexCell.Color, c3, c4);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c3;
            Color c2 = c4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            c3 = HexMetrics.TerraceLerp(beginCell.dataHexCell.Color, leftCell.dataHexCell.Color, i);
            c4 = HexMetrics.TerraceLerp(beginCell.dataHexCell.Color, rightCell.dataHexCell.Color, i);
            terrain.AddQuad(v1, v2, v3, v4);
            terrain.AddQuadColor(c1, c2, c3, c4);
        }

        terrain.AddQuad(v3, v4, left, right);
        terrain.AddQuadColor(c3, c4, leftCell.dataHexCell.Color, rightCell.dataHexCell.Color);
    }

    void TriangulateCornerTerracesCliff(
        Vector3 begin, MainHexCell beginCell,
        Vector3 left, MainHexCell leftCell,
        Vector3 right, MainHexCell rightCell
    )
    {
        float b = 1f / (rightCell.dataHexCell.Elevation - beginCell.dataHexCell.Elevation);
        if (b < 0)
        {
            b = -b;
        }
        Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b);
        Color boundaryColor = Color.Lerp(beginCell.dataHexCell.Color, rightCell.dataHexCell.Color, b);

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
            terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
            terrain.AddTriangleColor(leftCell.dataHexCell.Color, rightCell.dataHexCell.Color, boundaryColor);
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
        Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b);
        Color boundaryColor = Color.Lerp(beginCell.dataHexCell.Color, leftCell.dataHexCell.Color, b);

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
            terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
            terrain.AddTriangleColor(leftCell.dataHexCell.Color, rightCell.dataHexCell.Color, boundaryColor);
        }
    }

    void TriangulateBoundaryTriangle(
        Vector3 begin, MainHexCell beginCell,
        Vector3 left, MainHexCell leftCell,
        Vector3 boundary, Color boundaryColor
    )
    {
        Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
        Color c2 = HexMetrics.TerraceLerp(beginCell.dataHexCell.Color, leftCell.dataHexCell.Color, 1);

        terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
        terrain.AddTriangleColor(beginCell.dataHexCell.Color, c2, boundaryColor);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v2;
            Color c1 = c2;
            v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
            c2 = HexMetrics.TerraceLerp(beginCell.dataHexCell.Color, leftCell.dataHexCell.Color, i);
            terrain.AddTriangleUnperturbed(v1, v2, boundary);
            terrain.AddTriangleColor(c1, c2, boundaryColor);
        }

        terrain.AddTriangleUnperturbed(v2, HexMetrics.Perturb(left), boundary);
        terrain.AddTriangleColor(c2, leftCell.dataHexCell.Color, boundaryColor);
    }

    void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, Color color)
    {
        terrain.AddTriangle(center, edge.v1, edge.v2);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v2, edge.v3);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v3, edge.v4);
        terrain.AddTriangleColor(color);
        terrain.AddTriangle(center, edge.v4, edge.v5);
        terrain.AddTriangleColor(color);
    }

    void TriangulateEdgeStrip(
        EdgeVertices e1, Color c1,
        EdgeVertices e2, Color c2
    )
    {
        terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
        terrain.AddQuadColor(c1, c2);
    }
    void TriangulateWithRiver(
        HexDirection direction, MainHexCell cell, Vector3 center, EdgeVertices e
    )
    {
        Vector3 centerL, centerR;
        if (cell.dataHexCell.river.HasRiverThroughEdge(direction.Opposite()))
        {
            centerL = center +
                HexMetrics.GetFirstSolidCorner(direction.Previous()) * 0.25f;
            centerR = center +
                HexMetrics.GetSecondSolidCorner(direction.Next()) * 0.25f;
        }
        else if (cell.dataHexCell.river.HasRiverThroughEdge(direction.Next()))
        {
            centerL = center;
            centerR = Vector3.Lerp(center, e.v5, 2f / 3f);
        }
        else if (cell.dataHexCell.river.HasRiverThroughEdge(direction.Previous()))
        {
            centerL = Vector3.Lerp(center, e.v1, 2f / 3f);
            centerR = center;
        }
        else if (cell.dataHexCell.river.HasRiverThroughEdge(direction.Next2()))
        {
            centerL = center;
            centerR = center +
                HexMetrics.GetSolidEdgeMiddle(direction.Next()) * (0.5f * HexMetrics.innerToOuter);
        }
        else
        {
            centerL = center +
                HexMetrics.GetSolidEdgeMiddle(direction.Previous()) * (0.5f * HexMetrics.innerToOuter);
            centerR = center;
        }
        center = Vector3.Lerp(centerL, centerR, 0.5f);

        EdgeVertices m = new EdgeVertices(
            Vector3.Lerp(centerL, e.v1, 0.5f),
            Vector3.Lerp(centerR, e.v5, 0.5f),
            1f / 6f
        );

        m.v3.y = center.y = e.v3.y;

        TriangulateEdgeStrip(m, cell.dataHexCell.Color, e, cell.dataHexCell.Color);

        terrain.AddTriangle(centerL, m.v1, m.v2);
        terrain.AddTriangleColor(cell.dataHexCell.Color);
        terrain.AddQuad(centerL, center, m.v2, m.v3);
        terrain.AddQuadColor(cell.dataHexCell.Color, cell.dataHexCell.Color);
        terrain.AddQuad(center, centerR, m.v3, m.v4);
        terrain.AddQuadColor(cell.dataHexCell.Color, cell.dataHexCell.Color);
        terrain.AddTriangle(centerR, m.v4, m.v5);
        terrain.AddTriangleColor(cell.dataHexCell.Color);

        bool reversed = cell.dataHexCell.river.IncomingRiver == direction;
        TriangulateRiverQuad(centerL, centerR, m.v2, m.v4, cell.dataHexCell.RiverSurfaceY, 0.4f, reversed);
        TriangulateRiverQuad(m.v2, m.v4, e.v2, e.v4, cell.dataHexCell.RiverSurfaceY, 0.6f, reversed);
    }

    void TriangulateWithRiverBeginOrEnd(
        HexDirection direction, MainHexCell cell, Vector3 center, EdgeVertices e)
    {
        EdgeVertices m = new EdgeVertices(
            Vector3.Lerp(center, e.v1, 0.5f),
            Vector3.Lerp(center, e.v5, 0.5f)
        );

        m.v3.y = e.v3.y;

        TriangulateEdgeStrip(m, cell.dataHexCell.Color, e, cell.dataHexCell.Color);
        TriangulateEdgeFan(center, m, cell.dataHexCell.Color);

        bool reversed = cell.dataHexCell.river.HasIncomingRiver;
        TriangulateRiverQuad(
            m.v2, m.v4, e.v2, e.v4, cell.dataHexCell.RiverSurfaceY, 0.6f, reversed
        );

        center.y = m.v2.y = m.v4.y = cell.dataHexCell.RiverSurfaceY;
        rivers.AddTriangle(center, m.v2, m.v4);
        if (reversed)
        {
            rivers.AddTriangleUV(
                new Vector2(0.5f, 0.4f),
                new Vector2(1f, 0.2f), new Vector2(0f, 0.2f)
            );
        }
        else
        {
            rivers.AddTriangleUV(
                new Vector2(0.5f, 0.4f),
                new Vector2(0f, 0.6f), new Vector2(1f, 0.6f)
            );
        }
    }

    void TriangulateAdjacentToRiver(
        HexDirection direction, MainHexCell cell, Vector3 center, EdgeVertices e
    )
    {
        if (cell.dataHexCell.river.HasRiverThroughEdge(direction.Next()))
        {
            if (cell.dataHexCell.river.HasRiverThroughEdge(direction.Previous()))
            {
                center += HexMetrics.GetSolidEdgeMiddle(direction) *
                    (HexMetrics.innerToOuter * 0.5f);
            }
            else if (
                cell.dataHexCell.river.HasRiverThroughEdge(direction.Previous2())
            )
            {
                center += HexMetrics.GetFirstSolidCorner(direction) * 0.25f;
            }
        }
        else if (
            cell.dataHexCell.river.HasRiverThroughEdge(direction.Previous()) &&
            cell.dataHexCell.river.HasRiverThroughEdge(direction.Next2())
        )
        {
            center += HexMetrics.GetSecondSolidCorner(direction) * 0.25f;
        }

        EdgeVertices m = new EdgeVertices(
            Vector3.Lerp(center, e.v1, 0.5f),
            Vector3.Lerp(center, e.v5, 0.5f)
        );

        TriangulateEdgeStrip(m, cell.dataHexCell.Color, e, cell.dataHexCell.Color);
        TriangulateEdgeFan(center, m, cell.dataHexCell.Color);
    }

    void TriangulateRiverQuad(
        Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
        float y, float v, bool reversed
    )
    {
        TriangulateRiverQuad(v1, v2, v3, v4, y, y, v, reversed);
    }

    void TriangulateRiverQuad(
        Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
        float y1, float y2, float v, bool reversed
    )
    {
        v1.y = v2.y = y1;
        v3.y = v4.y = y2;
        rivers.AddQuad(v1, v2, v3, v4);
        if (reversed)
        {
            rivers.AddQuadUV(1f, 0f, 0.8f - v, 0.6f - v);
        }
        else
        {
            rivers.AddQuadUV(0f, 1f, v, v + 0.2f);
        }
    }
}

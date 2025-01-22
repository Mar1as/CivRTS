using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    MainHexCell[] cells;

    public HexMesh terrain, rivers, roads, water;
    public HexFeatureManager features;

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
        roads.Clear();
        water.Clear();
        features.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }

        terrain.Apply();
        rivers.Apply();
        roads.Apply();
        water.Apply();
        features.Apply();
    }

    void Triangulate(MainHexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            Triangulate(d, cell);
        }
        if (!cell.dataHexCell.waterScript.IsUnderwater && !cell.dataHexCell.river.HasRiver && !cell.dataHexCell.roadScript.HasRoads)
        {
            features.AddFeature(cell, cell.dataHexCell.Position);
        }
        if (!cell.dataHexCell.waterScript.IsUnderwater && cell.dataHexCell.featuresHexCell.IsSpecial)
        {
            features.AddSpecialFeature(cell, cell.dataHexCell.Position);
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

        else
        {
            TriangulateWithoutRiver(direction, cell, center, e);

            if (!cell.dataHexCell.waterScript.IsUnderwater && !cell.dataHexCell.roadScript.HasRoadThroughEdge(direction))
            {
                features.AddFeature(cell, (center + e.v1 + e.v5) * (1f / 3f));
            }
        }

        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, e);
        }

        if (cell.dataHexCell.waterScript.IsUnderwater)
        {
            TriangulateWater(direction, cell, center);
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

        bool hasRiver = cell.dataHexCell.river.HasRiverThroughEdge(direction);
        bool hasRoad = cell.dataHexCell.roadScript.HasRoadThroughEdge(direction);

        if (hasRiver)
        {
            e2.v3.y = neighbor.dataHexCell.StreamBedY;

            if (!cell.dataHexCell.waterScript.IsUnderwater)
            {
                if (!neighbor.dataHexCell.waterScript.IsUnderwater)
                {
                    TriangulateRiverQuad(
                    e1.v2, e1.v4, e2.v2, e2.v4,
                    cell.dataHexCell.river.RiverSurfaceY, neighbor.dataHexCell.river.RiverSurfaceY, 0.8f,
                    cell.dataHexCell.river.HasIncomingRiver && cell.dataHexCell.river.IncomingRiver == direction
                    );
                }
                else if (cell.dataHexCell.Elevation > neighbor.dataHexCell.waterScript.WaterLevel)
                {
                    TriangulateWaterfallInWater(
                        e1.v2, e1.v4, e2.v2, e2.v4,
                        cell.dataHexCell.river.RiverSurfaceY, neighbor.dataHexCell.river.RiverSurfaceY,
                        neighbor.dataHexCell.waterScript.WaterSurfaceY
                    );
                }
            }
            else if (
                !neighbor.dataHexCell.waterScript.IsUnderwater &&
                neighbor.dataHexCell.Elevation > cell.dataHexCell.waterScript.WaterLevel
            )
            {
                TriangulateWaterfallInWater(
                    e2.v4, e2.v2, e1.v4, e1.v2,
                    neighbor.dataHexCell.river.RiverSurfaceY, cell.dataHexCell.river.RiverSurfaceY,
                    cell.dataHexCell.waterScript.WaterSurfaceY
                );
            }
        }

        if (cell.brainHexCell.GetEdgeType(direction) == HexEdgeType.Slope)
        {
            TriangulateEdgeTerraces(e1, cell, e2, neighbor, hasRoad);
        }
        else
        {
            TriangulateEdgeStrip(e1, cell.dataHexCell.Color, e2, neighbor.dataHexCell.Color, hasRoad);
        }

        features.AddWall(e1, cell, e2, neighbor, hasRiver, hasRoad);

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

        features.AddWall(bottom, bottomCell, left, leftCell, right, rightCell);
    }

    void TriangulateEdgeTerraces(
        EdgeVertices begin, MainHexCell beginCell,
        EdgeVertices end, MainHexCell endCell,
        bool hasRoad)
    {
        EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.dataHexCell.Color, endCell.dataHexCell.Color, 1);

        TriangulateEdgeStrip(begin, beginCell.dataHexCell.Color, e2, c2, hasRoad);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            EdgeVertices e1 = e2;
            Color c1 = c2;
            e2 = EdgeVertices.TerraceLerp(begin, end, i);
            c2 = HexMetrics.TerraceLerp(beginCell.dataHexCell.Color, endCell.dataHexCell.Color, i);
            TriangulateEdgeStrip(e1, c1, e2, c2, hasRoad);
        }

        TriangulateEdgeStrip(e2, c2, end, endCell.dataHexCell.Color, hasRoad);
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
        EdgeVertices e2, Color c2,
        bool hasRoad = false)
    {
        terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        terrain.AddQuadColor(c1, c2);
        terrain.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
        terrain.AddQuadColor(c1, c2);

        if (hasRoad)
        {
            TriangulateRoadSegment(e1.v2, e1.v3, e1.v4, e2.v2, e2.v3, e2.v4);
        }
    }

    void TriangulateWithRiver(
        HexDirection direction, MainHexCell cell, Vector3 center, EdgeVertices e)
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

        if (!cell.dataHexCell.waterScript.IsUnderwater)
        {
            bool reversed = cell.dataHexCell.river.IncomingRiver == direction;
            TriangulateRiverQuad(centerL, centerR, m.v2, m.v4, cell.dataHexCell.river.RiverSurfaceY, 0.4f, reversed);
            TriangulateRiverQuad(m.v2, m.v4, e.v2, e.v4, cell.dataHexCell.river.RiverSurfaceY, 0.6f, reversed);
        }

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
        if (!cell.dataHexCell.waterScript.IsUnderwater)
        {
            bool reversed = cell.dataHexCell.river.HasIncomingRiver;
            TriangulateRiverQuad(
                m.v2, m.v4, e.v2, e.v4, cell.dataHexCell.river.RiverSurfaceY, 0.6f, reversed
            );

            center.y = m.v2.y = m.v4.y = cell.dataHexCell.river.RiverSurfaceY;
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

    }

    void TriangulateAdjacentToRiver(
        HexDirection direction, MainHexCell cell, Vector3 center, EdgeVertices e
    )
    {
        if (cell.dataHexCell.roadScript.HasRoads)
        {
            TriangulateRoadAdjacentToRiver(direction, cell, center, e);
        }

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

        if (!cell.dataHexCell.waterScript.IsUnderwater && !cell.dataHexCell.roadScript.HasRoadThroughEdge(direction))
        {
            features.AddFeature(cell, (center + e.v1 + e.v5) * (1f / 3f));
        }
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

    void TriangulateRoadSegment(
        Vector3 v1, Vector3 v2, Vector3 v3,
        Vector3 v4, Vector3 v5, Vector3 v6)
    {
        roads.AddQuad(v1, v2, v4, v5);
        roads.AddQuad(v2, v3, v5, v6);
        roads.AddQuadUV(0f, 1f, 0f, 0f);
        roads.AddQuadUV(1f, 0f, 0f, 0f);
    }

    void TriangulateRoad(
    Vector3 center, Vector3 mL, Vector3 mR, EdgeVertices e, bool hasRoadThroughCellEdge)
    {
        if (hasRoadThroughCellEdge)
        {
            Vector3 mC = Vector3.Lerp(mL, mR, 0.5f);
            TriangulateRoadSegment(mL, mC, mR, e.v2, e.v3, e.v4);
            roads.AddTriangle(center, mL, mC);
            roads.AddTriangle(center, mC, mR);
            roads.AddTriangleUV(
                new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(1f, 0f)
            );
            roads.AddTriangleUV(
                new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f)
            );
        }
        else
        {
            TriangulateRoadEdge(center, mL, mR);
        }

    }

    void TriangulateWithoutRiver(
        HexDirection direction, MainHexCell cell, Vector3 center, EdgeVertices e)
    {
        TriangulateEdgeFan(center, e, cell.dataHexCell.Color);

        if (cell.dataHexCell.roadScript.HasRoads)
        {
            Vector2 interpolators = GetRoadInterpolators(direction, cell);
            TriangulateRoad(
                center,
                Vector3.Lerp(center, e.v1, interpolators.x),
                Vector3.Lerp(center, e.v5, interpolators.y),
                e, cell.dataHexCell.roadScript.HasRoadThroughEdge(direction)
            );
        }
    }

    void TriangulateRoadEdge(Vector3 center, Vector3 mL, Vector3 mR)
    {
        roads.AddTriangle(center, mL, mR);
        roads.AddTriangleUV(
            new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)
        );
    }

    Vector2 GetRoadInterpolators(HexDirection direction, MainHexCell cell)
    {
        Vector2 interpolators;
        if (cell.dataHexCell.roadScript.HasRoadThroughEdge(direction))
        {
            interpolators.x = interpolators.y = 0.5f;
        }
        else
        {
            interpolators.x =
                cell.dataHexCell.roadScript.HasRoadThroughEdge(direction.Previous()) ? 0.5f : 0.25f;
            interpolators.y =
                cell.dataHexCell.roadScript.HasRoadThroughEdge(direction.Next()) ? 0.5f : 0.25f;
        }
        return interpolators;
    }

    void TriangulateRoadAdjacentToRiver(
        HexDirection direction, MainHexCell cell, Vector3 center, EdgeVertices e)
    {
        bool hasRoadThroughEdge = cell.dataHexCell.roadScript.HasRoadThroughEdge(direction);
        bool previousHasRiver = cell.dataHexCell.river.HasRiverThroughEdge(direction.Previous());
        bool nextHasRiver = cell.dataHexCell.river.HasRiverThroughEdge(direction.Next());
        Vector2 interpolators = GetRoadInterpolators(direction, cell);
        Vector3 roadCenter = center;

        if (cell.dataHexCell.river.HasRiverBeginOrEnd)
        {
            roadCenter += HexMetrics.GetSolidEdgeMiddle(
                cell.dataHexCell.river.RiverBeginOrEndDirection.Opposite()
            ) * (1f / 3f);
        }
        else if (cell.dataHexCell.river.IncomingRiver == cell.dataHexCell.river.OutgoingRiver.Opposite())
        {
            Vector3 corner;
            if (previousHasRiver)
            {
                if (!hasRoadThroughEdge &&
                    !cell.dataHexCell.roadScript.HasRoadThroughEdge(direction.Next()))
                {
                    return;
                }
                corner = HexMetrics.GetSecondSolidCorner(direction);
            }
            else
            {
                if (
                    !hasRoadThroughEdge &&
                    !cell.dataHexCell.roadScript.HasRoadThroughEdge(direction.Previous())
                )
                {
                    return;
                }
                corner = HexMetrics.GetFirstSolidCorner(direction);
            }
            roadCenter += corner * 0.5f;
            if (cell.dataHexCell.river.IncomingRiver == direction.Next() && (
                cell.dataHexCell.roadScript.HasRoadThroughEdge(direction.Next2()) ||
                cell.dataHexCell.roadScript.HasRoadThroughEdge(direction.Opposite())
            ))
            {
                features.AddBridge(roadCenter, center - corner * 0.5f);
            }
            center += corner * 0.25f;
        }
        else if (cell.dataHexCell.river.IncomingRiver == cell.dataHexCell.river.OutgoingRiver.Previous())
        {
            roadCenter -= HexMetrics.GetSecondCorner(cell.dataHexCell.river.IncomingRiver) * 0.2f;
        }
        else if (cell.dataHexCell.river.IncomingRiver == cell.dataHexCell.river.OutgoingRiver.Next())
        {
            roadCenter -= HexMetrics.GetFirstCorner(cell.dataHexCell.river.IncomingRiver) * 0.2f;
        }
        else if (previousHasRiver && nextHasRiver)
        {
            if (!hasRoadThroughEdge)
            {
                return;
            }
            Vector3 offset = HexMetrics.GetSolidEdgeMiddle(direction) *
                HexMetrics.innerToOuter;
            roadCenter += offset * 0.7f;
            center += offset * 0.5f;
        }
        else
        {
            HexDirection middle;
            if (previousHasRiver)
            {
                middle = direction.Next();
            }
            else if (nextHasRiver)
            {
                middle = direction.Previous();
            }
            else
            {
                middle = direction;
            }
            if (
                !cell.dataHexCell.roadScript.HasRoadThroughEdge(middle) &&
                !cell.dataHexCell.roadScript.HasRoadThroughEdge(middle.Previous()) &&
                !cell.dataHexCell.roadScript.HasRoadThroughEdge(middle.Next())
            )
            {
                return;
            }
            Vector3 offset = HexMetrics.GetSolidEdgeMiddle(middle);
            roadCenter += offset * 0.25f;
            if (direction == middle &&
                cell.dataHexCell.roadScript.HasRoadThroughEdge(direction.Opposite()))
            {
                features.AddBridge(
                    roadCenter,
                    center - offset * (HexMetrics.innerToOuter * 0.7f)
                );
            }
        }

        Vector3 mL = Vector3.Lerp(roadCenter, e.v1, interpolators.x);
        Vector3 mR = Vector3.Lerp(roadCenter, e.v5, interpolators.y);
        TriangulateRoad(roadCenter, mL, mR, e, hasRoadThroughEdge);

        if (previousHasRiver)
        {
            TriangulateRoadEdge(roadCenter, center, mL);
        }
        if (nextHasRiver)
        {
            TriangulateRoadEdge(roadCenter, mR, center);
        }
    }
    void TriangulateWater(
        HexDirection direction, MainHexCell cell, Vector3 center
    )
    {
        center.y = cell.dataHexCell.waterScript.WaterSurfaceY;

        MainHexCell neighbor = cell.brainHexCell.GetNeighbor(direction);
        if (neighbor != null && !neighbor.dataHexCell.waterScript.IsUnderwater)
        {
            TriangulateWaterShore(direction, cell, neighbor, center);
        }
        else
        {
            TriangulateOpenWater(direction, cell, neighbor, center);
        }
    }

    private void TriangulateWaterShore(HexDirection direction, MainHexCell cell, MainHexCell neighbor, Vector3 center)
    {
        //center.y += HexMetrics.waterYOffset;

        EdgeVertices e1 = new EdgeVertices(
            center + HexMetrics.GetFirstWaterCorner(direction),
            center + HexMetrics.GetSecondWaterCorner(direction)
        );
        water.AddTriangle(center, e1.v1, e1.v2);
        water.AddTriangle(center, e1.v2, e1.v3);
        water.AddTriangle(center, e1.v3, e1.v4);
        water.AddTriangle(center, e1.v4, e1.v5);//1

        Vector3 center2 = neighbor.dataHexCell.Position;
        center2.y = center.y;

        EdgeVertices e2 = new EdgeVertices(
            center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
            center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite())
        );

        water.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        water.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        water.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        water.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);//2

        MainHexCell nextNeighbor = cell.brainHexCell.GetNeighbor(direction.Next());
        if (nextNeighbor != null)
        {
            Vector3 v3 = nextNeighbor.dataHexCell.Position + (nextNeighbor.dataHexCell.waterScript.IsUnderwater ?
                HexMetrics.GetFirstWaterCorner(direction.Previous()) :
                HexMetrics.GetFirstSolidCorner(direction.Previous()));
            v3.y = center.y;
            water.AddTriangle(e1.v5, e2.v5, v3);
        }
        /*
        MainHexCell nextNeighbor = cell.brainHexCell.GetNeighbor(direction.Next());
        if (nextNeighbor != null)
        {
            Vector3 v3 = nextNeighbor.dataHexCell.Position + (nextNeighbor.dataHexCell.waterScript.IsUnderwater ?
                HexMetrics.GetFirstWaterCorner(direction.Previous()) :
                HexMetrics.GetFirstSolidCorner(direction.Previous()));
            v3.y = center.y;
            water.AddTriangle(
                e1.v5, e2.v5, v3 + HexMetrics.GetFirstSolidCorner(direction.Previous())
            );
        }*/
    }

    void TriangulateOpenWater(
        HexDirection direction, MainHexCell cell, MainHexCell neighbor, Vector3 center)
    {
        //center.y += HexMetrics.waterYOffset;

        Vector3 c1 = center + HexMetrics.GetFirstWaterCorner(direction);
        Vector3 c2 = center + HexMetrics.GetSecondWaterCorner(direction);

        water.AddTriangle(center, c1, c2);

        if (direction <= HexDirection.SE && neighbor != null)
        {
            Vector3 bridge = HexMetrics.GetWaterBridge(direction);
            Vector3 e1 = c1 + bridge;
            Vector3 e2 = c2 + bridge;

            water.AddQuad(c1, c2, e1, e2);

            if (direction <= HexDirection.E)
            {
                MainHexCell nextNeighbor = cell.brainHexCell.GetNeighbor(direction.Next());
                if (nextNeighbor == null || !nextNeighbor.dataHexCell.waterScript.IsUnderwater)
                {
                    return;
                }
                water.AddTriangle(
                    c2, e2, c2 + HexMetrics.GetWaterBridge(direction.Next())
                );
            }
        }
    }

    void TriangulateWaterfallInWater(
    Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
    float y1, float y2, float waterY)
    {
        //v3.y += HexMetrics.waterYOffset;
        //v4.y += HexMetrics.waterYOffset;

        v1.y = v2.y = y1;
        v3.y = v4.y = y2;
        v1 = HexMetrics.Perturb(v1);
        v2 = HexMetrics.Perturb(v2);
        v3 = HexMetrics.Perturb(v3);
        v4 = HexMetrics.Perturb(v4);
        float t = (waterY - y2) / (y1 - y2);
        v3 = Vector3.Lerp(v3, v1, t);
        v4 = Vector3.Lerp(v4, v2, t);
        rivers.AddQuadUnperturbed(v1, v2, v3, v4);
        rivers.AddQuadUV(0f, 1f, 0.8f, 1f);
    }
}

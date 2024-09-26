using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridLayout : MonoBehaviour
{
    [Header("Grid settings")]
    public Vector2Int gridSize;

    [Header("Hex settings")]
    public float innerSize, outerSize, height;
    public bool isFlatToped;
    public Material material;

    public HexTileGenerationSettings settings;

    private void OnEnable()
    {
        LayoutGrid();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            LayoutGrid();
        }
    }

    void LayoutGrid()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                GameObject tile = new GameObject($"Hex {x},{y}", typeof(HexRenderer));
                tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, y));

                HexRenderer hexRenderer = tile.GetComponent<HexRenderer>();
                hexRenderer.innerSize = innerSize;
                hexRenderer.outerSize = outerSize;
                hexRenderer.height = height;
                hexRenderer.isFlatToped = isFlatToped;
                hexRenderer.SetMaterial(material);
                hexRenderer.DrawMesh();

                tile.transform.SetParent(transform,true);

            }
        }
    }

    Vector3 GetPositionForHexFromCoordinate(Vector2Int coordinate)
    {
        int column = coordinate.x;
        int row = coordinate.y;
        float width, height, horizontalDistance, verticalDistance, offset, size = outerSize;
        float xPosition;
        float yPosition;
        bool shouldOffset;

        if (!isFlatToped)
        {
            shouldOffset = (row % 2) == 0;
            width = size * Mathf.Sqrt(3);
            height = size * 2f;
            horizontalDistance = width;
            verticalDistance = height * (3f/4f);
            offset = shouldOffset ? width / 2 : 0;
            xPosition = column * horizontalDistance + offset;
            yPosition = row * verticalDistance;

        }
        else
        {
            shouldOffset = (column % 2) == 0;
            width = size * 2f;
            height = size * Mathf.Sqrt(3);
            horizontalDistance = width * (3f / 4f);
            verticalDistance = height;
            offset = shouldOffset ? height / 2 : 0;
            xPosition = column * horizontalDistance;
            yPosition = row * verticalDistance - offset;
        }
        return new Vector3(xPosition, 0, -yPosition);
    }
}

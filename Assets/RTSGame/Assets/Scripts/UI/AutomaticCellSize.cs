using UnityEngine;
using UnityEngine.UI;

public class CustomGridLayout : LayoutGroup
{
    public int rows;
    public int columns = 2;
    public Vector2 cellSize;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        rows = Mathf.CeilToInt((float)transform.childCount / columns);

        float cellWidth = parentWidth / (float)columns;
        float cellHeight = parentHeight / (float)rows;

        cellSize.x = cellWidth;
        cellSize.y = cellHeight;

        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];

            var xPos = cellSize.x * columnCount;
            var yPos = cellSize.y * rowCount;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }

    public override void CalculateLayoutInputVertical()
    {
        float totalHeight = rows * cellSize.y;
        SetLayoutInputForAxis(totalHeight, totalHeight, -1, 1);
    }

    public override void SetLayoutHorizontal()
    {
        for (int i = 0; i < rectChildren.Count; i++)
        {
            var item = rectChildren[i];
            SetChildAlongAxis(item, 0, item.anchoredPosition.x, item.sizeDelta.x);
        }
    }

    public override void SetLayoutVertical()
    {
        for (int i = 0; i < rectChildren.Count; i++)
        {
            var item = rectChildren[i];
            SetChildAlongAxis(item, 1, item.anchoredPosition.y, item.sizeDelta.y);
        }
    }
}

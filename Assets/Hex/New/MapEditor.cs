using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour
{

    public Color[] colors;

    public HexGrid hexGrid;

    private Color activeColor;
    private bool paint = true;
    private int activeElevation;
    private bool applyElevation = true;
    private OptionalToggle riverMode;
    

    bool isDrag;
    HexDirection dragDirection;
    MainHexCell previousCell;

    void Awake()
    {
        SelectColor(0);
    }

    void Update()
    {
        if (
            Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject()
        )
        {
            HandleInput();
        }
        else
        {
            previousCell = null;
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            MainHexCell currentCell = hexGrid.GetCell(hit.point);
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            EditCell(currentCell);
            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        }
    }

    public void SelectColor(int index)
    {
        if(index != 0) paint = true;
        else paint = false;
        activeColor = colors[index];
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    void EditCell(MainHexCell cell)
    {
        if (paint)
        {
            cell.dataHexCell.Color = activeColor;
        }
        if (applyElevation)
        {
            cell.dataHexCell.Elevation = activeElevation;
        }
        if (riverMode == OptionalToggle.No)
        {
            cell.dataHexCell.river.RemoveRiver();
        }
        else if (isDrag && riverMode == OptionalToggle.Yes)
        {
            previousCell.dataHexCell.river.SetOutgoingRiver(dragDirection);
        }
    }

    void ValidateDrag(MainHexCell currentCell)
    {
        for (
            dragDirection = HexDirection.NE;
            dragDirection <= HexDirection.NW;
            dragDirection++
        )
        {
            if (previousCell.brainHexCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }

    enum OptionalToggle
    {
        Ignore, Yes, No
    }
}



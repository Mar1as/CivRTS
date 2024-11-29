using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{
    public HexGrid grid;

    MainHexCell currentCell;

    MainHexUnit selectedUnit;

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoSelection();
            }
            else if (selectedUnit)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                }
                else
                {
                    DoPathfinding();
                }
            }
        }
    }

    public void SetEditMode(bool toggle)
    {
        enabled = !toggle;
        //grid.ShowUI(!toggle);
    }

    bool UpdateCurrentCell()
    {
        MainHexCell cell =
            grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell != currentCell)
        {
            currentCell = cell;
            return true;
        }
        return false;
    }

    void DoSelection()
    {
        UpdateCurrentCell();
        if (currentCell)
        {
            selectedUnit = currentCell.dataHexCell.Unit;
        }
    }

    

    void DoPathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (currentCell && selectedUnit.dataHexUnit.IsValidDestination(currentCell))
            {
                grid.FindPath(selectedUnit.dataHexUnit.Location, currentCell, 24);
            }
            else
            {
                grid.ClearPath();
            }
        }
    }
    void DoMove()
    {
        if (grid.HasPath)
        {
            selectedUnit.Travel(grid.GetPath());
            grid.ClearPath();
        }
    }
}

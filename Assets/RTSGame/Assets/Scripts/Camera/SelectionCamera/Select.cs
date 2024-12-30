using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Select : MonoBehaviour
{
    List<GameObject> listSelectedUnits;
    UiUnitsList ui;

    public Select(List<GameObject> listSelectedUnits)
    {
        this.listSelectedUnits = listSelectedUnits;
    }
    public Select(List<GameObject> listSelectedUnits, UiUnitsList ui)
    {
        this.listSelectedUnits = listSelectedUnits;
        this.ui = ui;
    }

    public void ClickOnUnit(GameObject selectedUnit)
    {

        if (IsUnitSelected(selectedUnit))
        {
            DeselectUnit(selectedUnit);
        }
        else
        {
            SelectUnit(selectedUnit);
        }
    }


    private bool IsUnitSelected(GameObject selectedUnit)
    {
        bool response = false;
        foreach (GameObject i in listSelectedUnits)
        {
            if (i == selectedUnit)
            {
                response = true;
            }
        }
        return response;
    }

    public void DeselectUnit(GameObject selectedUnit)
    {
        if (!selectedUnit.IsDestroyed())
        {
            selectedUnit.GetComponent<UnitStats>().unitGraphics.ActiveCircle(false);
            listSelectedUnits.Remove(selectedUnit);

            if (ui != null)
            {
                //ui.AnimateButton();
            }
        }
        
    }

    public void SelectUnit(GameObject selectedUnit)
    {
        selectedUnit.GetComponent<UnitStats>().unitGraphics.ActiveCircle(true);
        listSelectedUnits.Add(selectedUnit);

        if (ui != null)
        {
            //ui.AnimateButton();
        }
    }
}

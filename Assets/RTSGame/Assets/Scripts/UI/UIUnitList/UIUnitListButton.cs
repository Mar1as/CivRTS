using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitListButton : MonoBehaviour
{
    public GameObject unit;
    List<GameObject> listSelectedUnits;

    [SerializeField] Color colorSelected = new Color(0.5f, 0.5f, 0.5f, 1);
    [SerializeField] Color colorUnselected = new Color(0.5f, 0.5f, 0.5f, 1);

    Image image;

    bool isSelected = false;
    bool IsSelected
    {
        get
        {
            return isSelected;
        }
        set
        {
            isSelected = value;
            if (isSelected)
            {
                SetColor(colorSelected);
                listSelectedUnits.Add(unit);
            }
            else
            {
                SetColor(colorUnselected);
                listSelectedUnits.Remove(unit);
            }
        }
    }

    private void Start()
    {
        image = GetComponent<Image>();
    }

    void SetColor(Color color)
    {
        image.color = color;
    }

    public void OnClick()
    {
        IsSelected = !IsSelected;
    }

    public void OnStart(GameObject unit, List<GameObject> listSelectedUnits)
    {
        this.unit = unit;
        this.listSelectedUnits = listSelectedUnits;
    }
}

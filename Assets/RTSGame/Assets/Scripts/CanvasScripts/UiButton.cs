using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiButton : MonoBehaviour
{
    Color colorAtBeggining;
    [SerializeField] Color colorToChange;

    void Start()
    {
        colorAtBeggining = GetComponent<Image>().color;
    }

    public void ChangeColor()
    {

        if (colorToChange != GetComponent<Image>().color)
        {
            GetComponent<Image>().color = colorToChange;
        }
        else
        {
            GetComponent<Image>().color = colorAtBeggining;
        }
    }

    public void ChangeColor(bool selected)
    {

        if (selected)
        {
            GetComponent<Image>().color = colorToChange;
        }
        else
        {
            GetComponent<Image>().color = colorAtBeggining;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsInfo
{
    public List<GameObject> listButtons = new List<GameObject>();
    public GameObject thisButton;
    public ButtonsInfo(List<GameObject> list, GameObject thisButton)
    {
        listButtons = list;
        this.thisButton = thisButton;
    }
}

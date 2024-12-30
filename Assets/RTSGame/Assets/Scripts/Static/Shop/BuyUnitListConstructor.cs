using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyUnitListConstructor
{
    [SerializeField]
    public string jmeno;
    [SerializeField]
    public List<GameObject> listOfUnits = new List<GameObject>();

    public BuyUnitListConstructor(string name, GameObject gm)
    {
        this.jmeno = name;
        listOfUnits.Add(gm);
    }
}

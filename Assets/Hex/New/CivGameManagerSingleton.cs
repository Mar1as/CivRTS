using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CivGameManagerSingleton
{
    private static CivGameManagerSingleton instance = null;

    public static CivGameManagerSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CivGameManagerSingleton();
                instance.allUnits = new List<MainHexUnit>();
                instance.allCities = new List<MainCity>();
            }
            return instance;
        }
    }

    public MainHexCell[] hexagons;
    public List<MainHexUnit> allUnits;
    public List<MainCity> allCities;
    public HexGrid hexGrid;
}

using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class MainCity
{
    [SerializeField]
    public DataCity dataCity;

    private void Awake()
    {

    }
    private void OnDestroy()
    {
        //dataCity.SelfDestruct();
        Debug.Log("City destroyed");
    }
    public MainCity(MainHexCell cell)
    {
        CivGameManagerSingleton.Instance.allCities.Add(this);
        dataCity = new DataCity(cell, this);
    }
}

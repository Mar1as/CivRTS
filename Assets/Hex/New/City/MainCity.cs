using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MainCity
{
    [SerializeField]
    public DataCity dataCity;

    private void Awake()
    {

    }
    public void Destroy()
    {
        dataCity.SelfDestruct();
        Debug.Log("City destroyed");
        
        dataCity.Location.barText.ChangeText();
        CivGameManagerSingleton.Instance.allCities.Remove(this);
    }
    public MainCity(MainHexCell cell, Player player)
    {
        CivGameManagerSingleton.Instance.allCities.Add(this);
        dataCity = new DataCity(cell, this, player);

        UiManagerRTS.UpdateAllCitiesBar();
    }
}

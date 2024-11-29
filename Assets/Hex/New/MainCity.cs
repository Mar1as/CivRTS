using UnityEngine;

public class MainCity : MonoBehaviour
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
    public void Inicilizace(MainHexCell cell)
    {
        CivGameManagerSingleton.Instance.allCities.Add(this);
        dataCity = new DataCity(cell, this);
    }



}

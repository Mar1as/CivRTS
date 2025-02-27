using System.Collections.Generic;
using UnityEngine;

public class OrdersGraphics : MonoBehaviour
{
    Orders order;
    GameObject prefab;
    List<GameObject> listPrefabs;
    List<GameObject> listSelectedUnits;

    public OrdersGraphics(Orders order, GameObject prefab, List<GameObject> selectedUnits)
    {
        this.order = order;
        this.prefab = prefab;
        listPrefabs = new List<GameObject>();
        listSelectedUnits = selectedUnits;
    }

    public void UpdateListPrefabs(Vector3 beginVector, Vector3 endVector)
    {
        DestroyListPrefabs();

        if(Vector3.Distance(beginVector, endVector) > order.cam.distance)
        foreach (var formationPos in order.listSquareFormation)
        {
            Vector3 vec = formationPos;
            vec.y += 1;
            Quaternion rotation = Quaternion.Euler(90, 0, 0);
            GameObject circle = Instantiate(prefab, vec, rotation);
            circle.GetComponent<SpriteRenderer>().color = listSelectedUnits[0].GetComponent<UnitStats>().teamColor;
            listPrefabs.Add(circle);
        }
    }

    public void DestroyListPrefabs()
    {
        foreach (var item in listPrefabs)
        {
            Destroy(item);
        }
        listPrefabs.Clear();
    }
}

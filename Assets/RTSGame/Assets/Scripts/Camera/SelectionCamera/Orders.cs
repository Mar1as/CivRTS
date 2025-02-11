using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Orders
{
    public List<Vector3> listSquareFormation = new List<Vector3>();
    float distance = 3;
    public OrdersGraphics og;
    public CameraSelectUnits cam;

    public Orders(GameObject prefab, List<GameObject> selectedUnits, CameraSelectUnits cam)
    {
        this.cam = cam;
        og = new OrdersGraphics(this, prefab, selectedUnits);

    }
    public Orders()
    {

    }

    public void MoveSelector(Vector3 beginHit, Vector3 finalVector, List<GameObject> listSelectedUnits, float distance)
    {
        Debug.Log("MOVE2");
        float distanceToDecide = Vector3.Distance(beginHit, finalVector);
        if (distanceToDecide < distance) //Ctverec
        {
            GenerateCtverecFormation(listSelectedUnits.Count, finalVector);
        }
        else //Obdelnik
        {
            GenerateObdelnikFormation(listSelectedUnits.Count, beginHit, finalVector);
        }
        //float formationLength = Mathf.CeilToInt(Vector3.Distance(beginVector, groundHit) / 3);
        MoveSelectedUnits(listSelectedUnits);
    }

    public void MoveSelectedUnits(List<GameObject> listSelectedUnits)
    {
        for (int i = 0; i < listSelectedUnits.Count; i++)
        {
            var stats = listSelectedUnits[i].GetComponent<UnitStats>();
            stats.order = true;
            var behaviour = listSelectedUnits[i].GetComponent<UnitBehaviour>();
            behaviour.attackingBool = false;
            var navAgent = stats.agent;
            if (navAgent != null)
            {
                navAgent.SetDestination(ReturnFormation(i));
                Debug.Log("AGENT");
            }
        }
        Debug.Log("Move3");
        og.DestroyListPrefabs();
    }

    Vector3 ReturnFormation(int i)
    {
        int vybranaPozice = i;
        Vector3 vec = listSquareFormation[vybranaPozice];
        Debug.Log("Lokace: " + vec);
        return vec;
    }

    public void AttackSelectedUnit(List<GameObject> listSelectedUnits, RaycastHit enemy)
    {
        foreach (var unit in listSelectedUnits)
        {
            var stats = unit.GetComponent<UnitStats>();
            stats.order = true;
            var behaviour = unit.GetComponent<UnitBehaviour>();
            behaviour.attackingBool = true;
            if (behaviour != null)
                behaviour.ChangeTarget(enemy.collider.transform.root.gameObject);
        }
    }

    void GenerateCtverecFormation(int count, Vector3 groundHit) //Ètverec
    {
        listSquareFormation.Clear();

        int pocet = Mathf.CeilToInt(Mathf.Sqrt(count));

        for (int j = 0; j < pocet; j++)
        {
            for (int k = 0; k < pocet; k++)
            {
                listSquareFormation.Add(new Vector3(groundHit.x + (k - pocet / 2) * distance, 0, groundHit.z + (j - pocet / 2) * distance));
            }
        }
    }

    public void GenerateObdelnikFormation(float count, Vector3 beginVector, Vector3 endVector) //Obdelník
    {
        listSquareFormation.Clear();

        int pocet = Mathf.CeilToInt(count); //Poèet jednotek
        float formationLength = Mathf.CeilToInt(Vector3.Distance(beginVector, endVector) / 3);
        int pocetRadku = Mathf.CeilToInt((float)pocet / formationLength);
        int pocetVRadku = Mathf.CeilToInt((float)pocet / (float)pocetRadku);
        Vector3 smerovyVektor = endVector - beginVector;
        Vector3 normalovyVektor = -Vector3.Cross(smerovyVektor, Vector3.up);

        for (int j = 0; j < pocetRadku; j++)
        {
            for (int k = 0; k < pocetVRadku; k++)
            {
                float offsetX = k * (smerovyVektor.x / pocetVRadku);
                float offsetZ = k * (smerovyVektor.z / pocetVRadku);
                Vector3 back = j * (normalovyVektor.normalized * 3);
                Vector3 unitPosition = new Vector3(beginVector.x + offsetX + back.x, 0, beginVector.z + offsetZ + back.z);
                listSquareFormation.Add(unitPosition);
            }
        }
    }
}

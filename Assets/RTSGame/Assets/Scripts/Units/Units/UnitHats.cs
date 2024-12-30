using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHats : MonoBehaviour
{
    [SerializeField] List<GameObject> hatList;
    [SerializeField] List<GameObject> hairList;
    [SerializeField] List<GameObject> glassesList;
    [SerializeField] List<GameObject> beardList;

    [SerializeField] Transform head;


    // Start is called before the first frame update
    void Start()
    {
        GameObject hat = hatList[Random.Range(0, hatList.Count)];
        GameObject hair = hairList[Random.Range(0, hairList.Count)];
        GameObject glasses = glassesList[Random.Range(0, glassesList.Count)];
        GameObject beard = beardList[Random.Range(0, beardList.Count)];

        if (hat)
        {
            GameObject h = Instantiate(hat, head);
            h.transform.localScale *= 100;
        }
        if (hair) 
        {
            GameObject h = Instantiate(hair, head);
            h.transform.localScale *= 100;
        }
        if (glasses) 
        {
            GameObject h = Instantiate(glasses, head);
            h.transform.localScale *= 100;
        }
        if (beard) 
        {
            GameObject h = Instantiate(beard, head);
            h.transform.localScale *= 100;
        }

    }
}

using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> listOfUnits = new List<GameObject>(); //List v�ech jednotek, kter� se daj� koupit

    public List<BuyUnitListConstructor> listBuyUnit = new List<BuyUnitListConstructor>(); //List kategori� jednotek

    Teams teams;

    private static Shop instance = new Shop();
    public static Shop Instance => instance;

    private void Awake()
    {
        SetupBuyUnit();
    }

    private void Start()
    {
        teams = gameObject.GetComponent<Teams>();
    }

    public void Shoping(string tag, int indexOfUnit, int indexOfType) //kdo zavolal funkci, co chce koupit, kde se to nach�z�
    {
        for (int i = 0; i < Teams.listOfPlayers.Count; i++) //projdu v�echny t�my
        {
            if (Teams.listOfPlayers[i].tag == tag) //zjist�m kdo zavolal funkci
            {
                TeamsConstructor player = Teams.listOfPlayers[i]; //zjist�m kdo zavolal funkci
                GameObject prefabUnit = listBuyUnit[indexOfType].listOfUnits[indexOfUnit]; //zjist�m co chce koupit
                UnitStats prefabUnitStats = prefabUnit.GetComponent<UnitStats>();   //zjist�m co chce koupit


                if (player.money >= prefabUnitStats.costCur)//cena
                {

                    GameObject unit; //vytvo��m jednotku
                    unit = Instantiate(prefabUnit, player.spawnPointVector, prefabUnit.transform.rotation); //vytvo��m jednotku
                    UnitStats unitStats = unit.GetComponent<UnitStats>(); //zjist�m co chce koupit
                                        
                    ChangeUnitStats(unit, player); //zm�n�m tag a layer jednotky

                    Teams.listOfPlayers[i].listUnitsAdd(Teams.listOfPlayers[i].ListUnits,unit); //p�id�m jednotku do listu jednotek
                    //teams.AddUnitVoid(unit); //p�id�m jednotku do listu jednotek

                    player.money -= unitStats.costCur; //ode�tu pen�ze

                    prefabUnitStats.costCur += Mathf.CeilToInt((float)prefabUnitStats.cost / 10); //zv���m cenu jednotky

                }
                break;
            }
        }
    }
    public void ShopingByListUnits(string tag, int indexOfUnit, List<GameObject> koupitelne) //kdo zavolal funkci, co chce koupit, kde se to nach�z�
    {

        for (int i = 0; i < Teams.listOfPlayers.Count; i++) //projdu v�echny t�my
        {
            if (Teams.listOfPlayers[i].tag == tag) //zjist�m kdo zavolal funkci
            {
                TeamsConstructor player = Teams.listOfPlayers[i]; //zjist�m kdo zavolal funkci
                GameObject prefabUnit = koupitelne[indexOfUnit]; //zjist�m co chce koupit
                UnitStats prefabUnitStats = prefabUnit.GetComponent<UnitStats>();   //zjist�m co chce koupit


                if (player.money >= prefabUnitStats.costCur)//cena
                {

                    GameObject unit; //vytvo��m jednotku
                    unit = Instantiate(prefabUnit, player.spawnPointVector, prefabUnit.transform.rotation); //vytvo��m jednotku
                    UnitStats unitStats = unit.GetComponent<UnitStats>(); //zjist�m co chce koupit

                    ChangeUnitStats(unit, player); //zm�n�m tag a layer jednotky

                    Teams.listOfPlayers[i].listUnitsAdd(Teams.listOfPlayers[i].ListUnits, unit); //p�id�m jednotku do listu jednotek
                    //teams.AddUnitVoid(unit); //p�id�m jednotku do listu jednotek

                    player.money -= unitStats.costCur; //ode�tu pen�ze

                    prefabUnitStats.costCur += Mathf.CeilToInt((float)prefabUnitStats.cost / 10); //zv���m cenu jednotky

                }
                break;
            }
        }
    }

    public void ChangeUnitStats(GameObject unit, TeamsConstructor player) //zm�n� tag a layer jednotky
    {
        unit.tag = player.tag; //zm�n� tag jednotky
        unit.layer = LayerMask.NameToLayer(player.tag); //zm�n� layer jednotky
    }

    void SetupBuyUnit() //Nastav� list jednotek, kter� se daj� koupit
    {
        bool repeats = false; //Zda se kategorie opakuje
        int index = 0; //Index kategorie

        for (int i = 0; i < listOfUnits.Count; i++) //Projde v�echny jednotky
        {

            for (int j = 0; j < listBuyUnit.Count; j++) //Projde v�echny kategorie
            {
                if (listOfUnits[i].GetComponent<UnitStats>().kategorie == listBuyUnit[j].jmeno) //Zjist� zda se kategorie opakuje
                {
                    repeats = true; //Kategorie se opakuje
                    index = j; //Ulo�� index kategorie
                }
            }

            listOfUnits[i].GetComponent<UnitStats>().costCur = listOfUnits[i].GetComponent<UnitStats>().cost; //Nastav� aktu�ln� cenu jednotky

            if (repeats == true) //U� tam kategorie byla
            {
                listBuyUnit[index].listOfUnits.Add(listOfUnits[i]); //P�id� jednotku do kategorie
            }
            else //Poprv� kategorie nalezena (typ jednotky [p�chota])
            {
                listBuyUnit.Add(new BuyUnitListConstructor(listOfUnits[i].GetComponent<UnitStats>().kategorie, listOfUnits[i])); //Vytvo�� kategorii a p�id� jednotku do kategorie
            }

            repeats = false; //Kategorie se neopakuje
        }
    }
}

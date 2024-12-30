using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> listOfUnits = new List<GameObject>(); //List všech jednotek, které se dají koupit

    public List<BuyUnitListConstructor> listBuyUnit = new List<BuyUnitListConstructor>(); //List kategorií jednotek

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

    public void Shoping(string tag, int indexOfUnit, int indexOfType) //kdo zavolal funkci, co chce koupit, kde se to nachází
    {
        for (int i = 0; i < Teams.listOfPlayers.Count; i++) //projdu všechny týmy
        {
            if (Teams.listOfPlayers[i].tag == tag) //zjistím kdo zavolal funkci
            {
                TeamsConstructor player = Teams.listOfPlayers[i]; //zjistím kdo zavolal funkci
                GameObject prefabUnit = listBuyUnit[indexOfType].listOfUnits[indexOfUnit]; //zjistím co chce koupit
                UnitStats prefabUnitStats = prefabUnit.GetComponent<UnitStats>();   //zjistím co chce koupit


                if (player.money >= prefabUnitStats.costCur)//cena
                {

                    GameObject unit; //vytvoøím jednotku
                    unit = Instantiate(prefabUnit, player.spawnPointVector, prefabUnit.transform.rotation); //vytvoøím jednotku
                    UnitStats unitStats = unit.GetComponent<UnitStats>(); //zjistím co chce koupit
                                        
                    ChangeUnitStats(unit, player); //zmìním tag a layer jednotky

                    Teams.listOfPlayers[i].listUnitsAdd(Teams.listOfPlayers[i].ListUnits,unit); //pøidám jednotku do listu jednotek
                    //teams.AddUnitVoid(unit); //pøidám jednotku do listu jednotek

                    player.money -= unitStats.costCur; //odeètu peníze

                    prefabUnitStats.costCur += Mathf.CeilToInt((float)prefabUnitStats.cost / 10); //zvýším cenu jednotky

                }
                break;
            }
        }
    }
    public void ShopingByListUnits(string tag, int indexOfUnit, List<GameObject> koupitelne) //kdo zavolal funkci, co chce koupit, kde se to nachází
    {

        for (int i = 0; i < Teams.listOfPlayers.Count; i++) //projdu všechny týmy
        {
            if (Teams.listOfPlayers[i].tag == tag) //zjistím kdo zavolal funkci
            {
                TeamsConstructor player = Teams.listOfPlayers[i]; //zjistím kdo zavolal funkci
                GameObject prefabUnit = koupitelne[indexOfUnit]; //zjistím co chce koupit
                UnitStats prefabUnitStats = prefabUnit.GetComponent<UnitStats>();   //zjistím co chce koupit


                if (player.money >= prefabUnitStats.costCur)//cena
                {

                    GameObject unit; //vytvoøím jednotku
                    unit = Instantiate(prefabUnit, player.spawnPointVector, prefabUnit.transform.rotation); //vytvoøím jednotku
                    UnitStats unitStats = unit.GetComponent<UnitStats>(); //zjistím co chce koupit

                    ChangeUnitStats(unit, player); //zmìním tag a layer jednotky

                    Teams.listOfPlayers[i].listUnitsAdd(Teams.listOfPlayers[i].ListUnits, unit); //pøidám jednotku do listu jednotek
                    //teams.AddUnitVoid(unit); //pøidám jednotku do listu jednotek

                    player.money -= unitStats.costCur; //odeètu peníze

                    prefabUnitStats.costCur += Mathf.CeilToInt((float)prefabUnitStats.cost / 10); //zvýším cenu jednotky

                }
                break;
            }
        }
    }

    public void ChangeUnitStats(GameObject unit, TeamsConstructor player) //zmìní tag a layer jednotky
    {
        unit.tag = player.tag; //zmìní tag jednotky
        unit.layer = LayerMask.NameToLayer(player.tag); //zmìní layer jednotky
    }

    void SetupBuyUnit() //Nastaví list jednotek, které se dají koupit
    {
        bool repeats = false; //Zda se kategorie opakuje
        int index = 0; //Index kategorie

        for (int i = 0; i < listOfUnits.Count; i++) //Projde všechny jednotky
        {

            for (int j = 0; j < listBuyUnit.Count; j++) //Projde všechny kategorie
            {
                if (listOfUnits[i].GetComponent<UnitStats>().kategorie == listBuyUnit[j].jmeno) //Zjistí zda se kategorie opakuje
                {
                    repeats = true; //Kategorie se opakuje
                    index = j; //Uloží index kategorie
                }
            }

            listOfUnits[i].GetComponent<UnitStats>().costCur = listOfUnits[i].GetComponent<UnitStats>().cost; //Nastaví aktuální cenu jednotky

            if (repeats == true) //Už tam kategorie byla
            {
                listBuyUnit[index].listOfUnits.Add(listOfUnits[i]); //Pøidá jednotku do kategorie
            }
            else //Poprvé kategorie nalezena (typ jednotky [pìchota])
            {
                listBuyUnit.Add(new BuyUnitListConstructor(listOfUnits[i].GetComponent<UnitStats>().kategorie, listOfUnits[i])); //Vytvoøí kategorii a pøidá jednotku do kategorie
            }

            repeats = false; //Kategorie se neopakuje
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ButtonController : MonoBehaviour
{
    Shop shop;
    string tag;
    List<ButtonsInfo> listButtons = new List<ButtonsInfo>();

    [SerializeField]
    GameObject buttonPrefab;


    private void Start()
    {
        shop = gameObject.GetComponent<Shop>(); 
        tag = "FriendlyPlayer";

        SetUpMenuV2(); 
    }

    public void ButtonBuyUnit(int i,int j)
    {
        shop.Shoping(tag, i, j); //Nakoupí jednotku
    }
    
    public void SetUpMenuV2()
    {
        RectTransform buttonTransform = buttonPrefab.GetComponent<RectTransform>(); //Získá rozmìry buttonu
        float width = buttonTransform.rect.width; //Získá rozmìry buttonu
        float height = buttonTransform.rect.height; //Získá rozmìry buttonu


        for (int i = 0; i < shop.listBuyUnit.Count; i++)
        {
            int currentIndexI = i; //Uloží index i do promìnné, která se nezmìní
            List<GameObject> slavesButtons = new List<GameObject>(); //Vytvoøí list pro slave buttony

            GameObject masterButton = Instantiate(buttonPrefab, buttonPrefab.transform.transform.position, Quaternion.identity, buttonPrefab.transform.parent); //Vytvoøí master button
            masterButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => EnableMenus(currentIndexI)); //Pøidá listener na master button
            masterButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, i * height); //Nastaví pozici master buttonu

            masterButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{shop.listBuyUnit[i].listOfUnits[0].GetComponent<UnitStats>().kategorie}"; //Nastaví text master buttonu

            for (int j = 0; j < shop.listBuyUnit[i].listOfUnits.Count; j++)
            {
                int currentIndexJ = j; //Uloží index j do promìnné, která se nezmìní

                GameObject slaveButton = Instantiate(buttonPrefab, buttonPrefab.transform.transform.position, Quaternion.identity, masterButton.transform); //Vytvoøí slave button
                slaveButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ButtonBuyUnit(currentIndexJ, currentIndexI)); //Pøidá listener na slave button
                slaveButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width, j * height); //Nastaví pozici slave buttonu
                slaveButton.SetActive(false); //Deaktivuje slave button

                slaveButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{shop.listBuyUnit[i].listOfUnits[j].GetComponent<UnitStats>().jmeno}"; //Nastaví text slave buttonu

                slavesButtons.Add(slaveButton); //Pøidá slave button do listu slave buttonù

            }

            listButtons.Add(new ButtonsInfo(slavesButtons,masterButton)); //Pøidá master button a list slave buttonù do listu ButtonsInfo
        }
    }


    void DisableAllMenus() //Deaktivuje všechny menu
    {
        foreach (var item in listButtons) //Projde všechny ButtonsInfo
        {
            foreach (var button in item.listButtons) //Projde všechny slave buttony
            {
                button.SetActive(false); //Deaktivuje slave button
            }
        }
    }
    void EnableMenus(int k) //Aktivuje menu podle indexu k
    {
        DisableAllMenus(); //Deaktivuje všechny menu


        for (int i = 0; i < listButtons[k].listButtons.Count; i++) //Projde všechny slave buttony podle indexu k
        {
            //
            //($"I: {i}, listButtons[k].listButtons.Count: {listButtons[k].listButtons.Count}"); //Vypíše do konzole index i a poèet slave buttonù
            listButtons[k].listButtons[i].SetActive(true); //Aktivuje slave button
        }
    }
}

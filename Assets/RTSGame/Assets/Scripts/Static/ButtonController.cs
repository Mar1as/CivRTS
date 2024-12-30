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
        shop.Shoping(tag, i, j); //Nakoup� jednotku
    }
    
    public void SetUpMenuV2()
    {
        RectTransform buttonTransform = buttonPrefab.GetComponent<RectTransform>(); //Z�sk� rozm�ry buttonu
        float width = buttonTransform.rect.width; //Z�sk� rozm�ry buttonu
        float height = buttonTransform.rect.height; //Z�sk� rozm�ry buttonu


        for (int i = 0; i < shop.listBuyUnit.Count; i++)
        {
            int currentIndexI = i; //Ulo�� index i do prom�nn�, kter� se nezm�n�
            List<GameObject> slavesButtons = new List<GameObject>(); //Vytvo�� list pro slave buttony

            GameObject masterButton = Instantiate(buttonPrefab, buttonPrefab.transform.transform.position, Quaternion.identity, buttonPrefab.transform.parent); //Vytvo�� master button
            masterButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => EnableMenus(currentIndexI)); //P�id� listener na master button
            masterButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, i * height); //Nastav� pozici master buttonu

            masterButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{shop.listBuyUnit[i].listOfUnits[0].GetComponent<UnitStats>().kategorie}"; //Nastav� text master buttonu

            for (int j = 0; j < shop.listBuyUnit[i].listOfUnits.Count; j++)
            {
                int currentIndexJ = j; //Ulo�� index j do prom�nn�, kter� se nezm�n�

                GameObject slaveButton = Instantiate(buttonPrefab, buttonPrefab.transform.transform.position, Quaternion.identity, masterButton.transform); //Vytvo�� slave button
                slaveButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ButtonBuyUnit(currentIndexJ, currentIndexI)); //P�id� listener na slave button
                slaveButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-width, j * height); //Nastav� pozici slave buttonu
                slaveButton.SetActive(false); //Deaktivuje slave button

                slaveButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{shop.listBuyUnit[i].listOfUnits[j].GetComponent<UnitStats>().jmeno}"; //Nastav� text slave buttonu

                slavesButtons.Add(slaveButton); //P�id� slave button do listu slave button�

            }

            listButtons.Add(new ButtonsInfo(slavesButtons,masterButton)); //P�id� master button a list slave button� do listu ButtonsInfo
        }
    }


    void DisableAllMenus() //Deaktivuje v�echny menu
    {
        foreach (var item in listButtons) //Projde v�echny ButtonsInfo
        {
            foreach (var button in item.listButtons) //Projde v�echny slave buttony
            {
                button.SetActive(false); //Deaktivuje slave button
            }
        }
    }
    void EnableMenus(int k) //Aktivuje menu podle indexu k
    {
        DisableAllMenus(); //Deaktivuje v�echny menu


        for (int i = 0; i < listButtons[k].listButtons.Count; i++) //Projde v�echny slave buttony podle indexu k
        {
            //
            //($"I: {i}, listButtons[k].listButtons.Count: {listButtons[k].listButtons.Count}"); //Vyp�e do konzole index i a po�et slave button�
            listButtons[k].listButtons[i].SetActive(true); //Aktivuje slave button
        }
    }
}

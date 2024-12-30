using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiUnitsList : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject unitIconPrefab;
    [SerializeField] TextMeshProUGUI unitIconText; //K hovnu
    [SerializeField] Image unitIconImage; //K hovnu

    List<GameObject> unitIconList = new List<GameObject>();

    TeamsConstructor player;

    Select selectScript;

    [SerializeField] int numberInRow = 4, heightMultiplier = 2;

    public void Startos(TeamsConstructor pl)
    {
        player = pl;
        selectScript = new Select(player.ListSelectedUnits,this);
        Debug.Log("Startos");
    }

    public void UpdateUiList(List<GameObject> unitList)
    {
        ClearUnitIconList();

        UnitIcon();
        AddIcons(unitList);

        
    }

    void UnitIcon()
    {

        float unitIconWidth = panel.GetComponent<RectTransform>().rect.width / numberInRow;

        unitIconPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(unitIconWidth, unitIconWidth / heightMultiplier);

        foreach (RectTransform child in unitIconPrefab.transform)
        {
            child.sizeDelta = unitIconPrefab.GetComponent<RectTransform>().sizeDelta;
        }
    }

    void AddIcons(List<GameObject> unitList)
    {
        /*
        foreach (GameObject unit in unitList)
        {
            UnitStats unitStats = unit.GetComponent<UnitStats>();
            unitIconText.text = unitStats.jmeno;
            if (unitStats.icon.sprite != null)
            {
                unitIconImage.sprite = unitStats.icon.sprite;
            }
            unitIconImage.sprite = unitStats.icon;

            GameObject newUnitIcon = Instantiate(unitIconPrefab, panel.transform);
            newUnitIcon.GetComponent<RectTransform>().anchoredPosition = GetIconPosition(unitList.IndexOf(unit));
            //newUnitIcon.AddComponent<UiUnitIconInfo>() = unitList.IndexOf(unit);
            //newUnitIcon.GetComponent<UiUnitIconInfo>().id = unitList.IndexOf(unit);
            newUnitIcon.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectUnit(0)); //Pøidá listener na slave button

            unitIconList.Add(newUnitIcon);
        }*/
        unitIconPrefab.active = true;
        for (int i = 0; i < unitList.Count; i++)
        {
            int index = i;
            UnitStats unitStats = unitList[i].GetComponent<UnitStats>();
            unitIconText.text = unitStats.jmeno;
            /*if (unitStats.icon.sprite != null)
            {
                unitIconImage.sprite = unitStats.icon.sprite;
            }*/
            unitIconImage.sprite = unitStats.icon;

            GameObject newUnitIcon = Instantiate(unitIconPrefab, panel.transform);
            newUnitIcon.GetComponent<RectTransform>().anchoredPosition = GetIconPosition(unitList.IndexOf(unitList[i]));
            //newUnitIcon.AddComponent<UiUnitIconInfo>() = unitList.IndexOf(unit);
            //newUnitIcon.GetComponent<UiUnitIconInfo>().id = unitList.IndexOf(unit);
            Button btn = newUnitIcon.GetComponent<UnityEngine.UI.Button>();
            btn.onClick.AddListener(() => SelectUnit(index));

            unitIconList.Add(newUnitIcon);

        }
        

        unitIconPrefab.active = false;
    }

    Vector2 GetIconPosition(int index)
    {
        float unitIconWidth = unitIconPrefab.GetComponent<RectTransform>().rect.width;
        float unitIconHeight = unitIconPrefab.GetComponent<RectTransform>().rect.height;

        float x = (index % numberInRow) * unitIconWidth;
        float y = (index / numberInRow) * unitIconHeight;

        return new Vector2(x, y);
    }

    void ClearUnitIconList()
    {
        foreach (GameObject unitIcon in unitIconList)
        {
            Destroy(unitIcon);
        }
        unitIconList.Clear();
    }

    public void SelectUnit(int index)
    {
        //unitIconList[index].GetComponent<UiButton>().ChangeColor();
        AnimateButton();
        GameObject unit = player.listUnits[index].gameObject;
        selectScript.ClickOnUnit(unit);
    }

    public void AnimateButton()
    {

        
        List<GameObject> listSelectedUnits = player.listSelectedUnits;
        List<GameObject> listUnits = player.listUnits;

        for (int i = 0; i < listUnits.Count; i++)
        {
            if (listSelectedUnits.Contains(listUnits[i]))
            {
                unitIconList[i].GetComponent<UiButton>().ChangeColor(true);
                //unitIconList[i].GetComponent<Animator>().SetBool("SelectedBool", true);
            }
            else
            {
                unitIconList[i].GetComponent<UiButton>().ChangeColor(false);
                //unitIconList[i].GetComponent<Animator>().SetBool("SelectedBool", false);
            }
        }
        Debug.Log("updatos");

    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UIUnitListCreateButton
{
    List<GameObject> listUnits;
    List<GameObject> listSelectedUnits;
    List<Battalions> listBattalions;

    List<GameObject> listButtons = new List<GameObject>();

    GameObject panelOfUnits;
    GameObject unitButtonPrefab;


    bool input;
    public UIUnitListCreateButton(List<GameObject> ListUnits, List<GameObject> ListSelectedUnits, List<Battalions> ListBattalions, bool input)
    {
        panelOfUnits = UIUnitListStatic.Instance.panelOfUnits;
        unitButtonPrefab = UIUnitListStatic.Instance.unitButtonPrefab;

        this.listUnits = ListUnits;
        this.listSelectedUnits = ListSelectedUnits;
        this.listBattalions = ListBattalions;

        UpdatePanel();
        this.input = input;
    }

    public void UpdatePanel()
    {
        AdjustButtons();

    }

    void AdjustButtons()
    {
        foreach (var button in listButtons)
        {
            GameObject.Destroy(button);
        }
        listButtons.Clear();

        var newList = listBattalions.OrderByDescending(x => x.Units.Count).ToList();
        foreach (var button in newList) button.Check();
        foreach (Battalions battalion in newList)
        {
            GameObject buttonObject = GameObject.Instantiate(unitButtonPrefab, panelOfUnits.transform);
            buttonObject.GetComponent<UnityEngine.UI.Image>().sprite = SelectImage(battalion);
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = battalion.Units.Count.ToString();
            buttonObject.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => SelectBattalionUnits(battalion));
            listButtons.Add(buttonObject);
        }
    }

    void SelectBattalionUnits(Battalions battalion)
    {
        if (input)
        {
            Debug.Log("TR");
        }

        foreach (var item in listSelectedUnits)
        {
            item.GetComponent<UnitStats>().unitGraphics.ActiveCircle(false);
        }
        listSelectedUnits.Clear();
        foreach (var item in battalion.Units)
        {
            listSelectedUnits.Add(item);
            item.GetComponent<UnitStats>().unitGraphics.ActiveCircle(true);
        }
    }

    Sprite SelectImage(Battalions bat)
    {
        return bat.mainUnit.GetComponent<UnitStats>().icon;
    }

    public void UpdateButton(Battalions prapor)
    {
        var newList = listBattalions.OrderByDescending(x => x.Units.Count).ToList();
        for (int i = 0; i < newList.Count; i++)
        {
            if (prapor == newList[i])
            {
                GameObject buttonObject = listButtons[i];
                buttonObject.GetComponent<UnityEngine.UI.Image>().sprite = SelectImage(prapor);
                buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = prapor.Units.Count.ToString();
                buttonObject.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => SelectBattalionUnits(prapor));
                break;
            }
        }
    }
}

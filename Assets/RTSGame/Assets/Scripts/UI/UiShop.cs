using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiShop : MonoBehaviour
{
    GameObject uiShop;
    GameObject uiShopButtonPrefab;

    List<GameObject> prefabList;

    TeamsConstructor player;
    DataHexUnitArmy army;

    public UiShop(TeamsConstructor player, DataHexUnitArmy army)
    {
        prefabList = new List<GameObject>();

        uiShop = UIUnitListStatic.Instance.uiShop;
        uiShopButtonPrefab = UIUnitListStatic.Instance.uiShopButtonPrefab;

        this.player = player;
        this.army = army;
    }

    public void CreateShop()
    {
        CreatePrefabs();
    }

    void CreatePrefabs()
    {
        foreach (var unitData in army.unitSupply.Keys)
        {
            GameObject prefab = GameObject.Instantiate(uiShopButtonPrefab, uiShop.transform);

            UnitStats unitStats = unitData.unitPrefab.GetComponent<UnitStats>();
            TextMeshProUGUI[] textArray = prefab.GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = prefab.GetComponentsInChildren<Image>();
            Image sprite = null;

            foreach (var img in images)
            {
                if (img.gameObject != prefab)
                {
                    sprite = img;
                    break;
                }
            }

            if (textArray.Length >= 2)
            {
                textArray[0].text = unitStats.jmeno;
                textArray[1].text = $"Cena: {unitStats.costCur} | Zásoba: {army.unitSupply[unitData]}";
            }

            if (sprite != null)
            {
                sprite.sprite = unitStats.icon;
            }

            prefab.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ClickToBuy(unitData));

            prefabList.Add(prefab);
        }
    }

    void ClickToBuy(UnitData unitData)
    {
        if (army.CanBuyUnit(unitData))
        {
            army.RemoveUnit(unitData); // Odebrání jednotky ze zásoby
            Shop.Instance.ShopingByListUnits(player.tag, unitGm: unitData.unitPrefab);

            UpdatePrefab(unitData);
        }
        else
        {
            Debug.Log("Jednotka není dostupná.");
        }
    }

    void UpdatePrefab(UnitData unitData)
    {
        int index = GetPrefabIndex(unitData);
        if (index == -1) return;

        GameObject prefab = prefabList[index];
        TextMeshProUGUI[] textArray = prefab.GetComponentsInChildren<TextMeshProUGUI>();

        int remainingSupply = army.unitSupply.ContainsKey(unitData) ? army.unitSupply[unitData] : 0;
        textArray[1].text = $"Cena: {unitData.productionCost} | Zásoba: {remainingSupply}";

        prefab.GetComponent<UnityEngine.UI.Button>().interactable = remainingSupply > 0;
    }

    int GetPrefabIndex(UnitData unitData)
    {
        for (int i = 0; i < prefabList.Count; i++)
        {
            UnitStats unitStats = prefabList[i].GetComponentInChildren<UnitStats>();
            if (unitStats != null && unitStats.jmeno == unitData.name)
            {
                return i;
            }
        }
        return -1;
    }
}

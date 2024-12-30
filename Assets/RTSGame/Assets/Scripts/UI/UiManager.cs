using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class UiManager
{
    public UiShowInfoAboutBattalion uiShowInfoAboutBattalion { get; private set;}
    public UiShop uiShop { get; private set; }

    public ArmyUiShop armyUiShop;

    TeamsConstructor player;

    public UiManager(TeamsConstructor player)
    {
        uiShowInfoAboutBattalion = new UiShowInfoAboutBattalion();
        if (player.army != null)
        {
            armyUiShop = new ArmyUiShop(player);
        }
        uiShop = new UiShop(player);
    }

    public void CreateShop(List<GameObject> listFactions)
    {
        if (player.army != null)
        {
            armyUiShop.UpdateShop();
        }
        else uiShop.CreateShop(listFactions);
    }

}


public class ArmyUiShop : MonoBehaviour
{
    GameObject uiShop;
    GameObject uiShopButtonPrefab;

    TeamsConstructor player;

    ArmyHexUnit army;

    public ArmyUiShop(TeamsConstructor player)
    {
        uiShop = UIUnitListStatic.Instance.uiShop;
        uiShopButtonPrefab = UIUnitListStatic.Instance.uiShopButtonPrefabForArmy;

        this.player = player;
        army = player.army;
    }

    public void UpdateShop()
    {
        foreach (Transform child in uiShop.transform)
        {
            Destroy(child.gameObject);
        }
        if (player == null)
        {
            Debug.Log("NIC");
            //return;
        }

        Dictionary<GameObject, int> unitCount = new Dictionary<GameObject, int>();
        Debug.Log(army.unitsInArmy.Count);
        foreach (var unit in army.unitsInArmy)
        {
            int count = army.unitsInArmy.Count(u => u == unit);

            if (unitCount.ContainsKey(unit))
            {
                unitCount[unit] += count;
            }
            else
            {
                unitCount[unit] = count;
            }
        }

        for (int i = 0; i < unitCount.Count; i++)
        {
            GameObject unit = unitCount.Keys.ElementAt(i);

            GameObject buttonObj = Instantiate(uiShopButtonPrefab, uiShop.transform);
            Button[] buttons = buttonObj.GetComponentsInChildren<Button>();
            Image[] images = buttonObj.GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();
            buttons.Last().onClick.AddListener(() => ButtonClick(unit));
            texts.Last().text = unit.name + " + " + unitCount.Values.ElementAt(i).ToString();
        }
    }

    void ButtonClick(GameObject unit)
    {
        GameObject newUnit;
        newUnit = Instantiate(unit, player.spawnPointVector, Quaternion.identity);
        UnitStats prefabUnitStats = newUnit.gameObject.GetComponent<UnitStats>();

        Shop.Instance.ChangeUnitStats(newUnit, player);

        player.listUnitsAdd(player.ListUnits, newUnit);

        UpdateShop();
    }

}
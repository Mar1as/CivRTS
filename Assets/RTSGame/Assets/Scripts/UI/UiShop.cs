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
    List<GameObject> factionList;

    TeamsConstructor player;

    public UiShop(TeamsConstructor player)
    {
        prefabList = new List<GameObject>();

        uiShop = UIUnitListStatic.Instance.uiShop;
        uiShopButtonPrefab = UIUnitListStatic.Instance.uiShopButtonPrefab;

        factionList = new List<GameObject>();

        this.player = player;
    }

    public void CreateShop(List<GameObject> listFactions)
    {
        factionList = listFactions;

        for (int i = 0; i < listFactions.Count; i++)
        {
            GameObject item = listFactions[i];
            item.GetComponent<UnitStats>().costCur = item.GetComponent<UnitStats>().cost;
        }
        CreatePrefabs();
    }

    void CreatePrefabs()
    {
        for (int i = 0; i < 12; i++)
        {
            GameObject prefab = GameObject.Instantiate(uiShopButtonPrefab, uiShop.transform);

            if (i < factionList.Count)
            {
                UnitStats unit = factionList[i].GetComponent<UnitStats>();

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
                    textArray[0].text = unit.jmeno;
                    textArray[1].text = unit.costCur.ToString();
                }

                if (sprite != null)
                {
                    sprite.sprite = unit.icon;
                }

                int currentIndex = i;
                prefab.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ClickToBuy(currentIndex));
            }

            prefabList.Add(prefab);
        }
    }

    void ClickToBuy(int index)
    {
        Debug.Log($"{player.tag} {index} {factionList.Count}");
        Shop.Instance.ShopingByListUnits(player.tag, index, factionList);

        UpdatePrefab(index);
    }

    void UpdatePrefab(int index)
    {
        UnitStats unit = factionList[index].GetComponent<UnitStats>();
        GameObject prefab = prefabList[index];

        TextMeshProUGUI[] textArray = prefab.GetComponentsInChildren<TextMeshProUGUI>();

        textArray[1].text = unit.costCur.ToString();
    }
}

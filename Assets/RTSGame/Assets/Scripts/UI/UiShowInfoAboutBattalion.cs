using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiShowInfoAboutBattalion
{
    GameObject uiUnitStatsPanel; //Nalevo
    GameObject uiUnitStatsPanelPrefab;

    GameObject uiUnitsInfoPanel; //Napravo
    GameObject uiUnitsInfoPanelPrefab;

    Battalions currentBattalionDisplaying;

    GameObject uiUnitStatsPanelActive;
    List<GameObject> listUiUnitsInfoPanelPrefabActive = new List<GameObject>();

    public UiShowInfoAboutBattalion()
    {
        uiUnitStatsPanel = UIUnitListStatic.Instance.uiUnitStatsPanel;
        uiUnitStatsPanelPrefab = UIUnitListStatic.Instance.uiUnitStatsPanelPrefab;

        uiUnitsInfoPanel = UIUnitListStatic.Instance.uiUnitsInfoPanel;
        uiUnitsInfoPanelPrefab = UIUnitListStatic.Instance.uiUnitsInfoPanelPrefab;
    }

    public void CreateInPanels(Battalions bat)
    {
        currentBattalionDisplaying = bat;
        AdjustButtons(bat);
    }
    public void UpdateInPanels(Battalions bat)
    {
        AdjustButtons(bat);
    }

    void AdjustButtons(Battalions bat)
    {
        if (bat == currentBattalionDisplaying && currentBattalionDisplaying != null)
        {
            CreateUnitStatsPanel(bat);
            CreateUnitsInfoPanel(bat);
        }
    }

    void CreateUnitStatsPanel(Battalions bat)
    {
        
        TextMeshProUGUI text;
        GameObject.Destroy(uiUnitStatsPanelActive);

        uiUnitStatsPanelActive = GameObject.Instantiate(uiUnitStatsPanelPrefab, uiUnitStatsPanel.transform);
        uiUnitStatsPanelActive.GetComponentInChildren<UnityEngine.UI.Image>().sprite = bat.mainUnit.GetComponent<UnitStats>().icon;
        text = uiUnitStatsPanelActive.GetComponentInChildren<TextMeshProUGUI>();

        UnitStats unit = bat.mainUnit.GetComponent<UnitStats>();
        if (unit is TankUnitStats) //Tank
        {
            TankUnitStats tank = unit as TankUnitStats;
            text.text = $"Stats\nHP: {tank.Hp}\nOsádka: {tank.osadka}/{tank.maxOsadka}\nRange: {tank.range}\nSkill: {tank.skill}\n" +
                $"Armor\nF: {tank.armorFront}\nS: {tank.armorSide}\nB: {tank.armorBack}\n" +
                $"Weapon\nDamage: {tank.weapon.damage}\nPiercing: {tank.weapon.piercing}\nAccuracy: {tank.weapon.accuracyWeapon}\n" +
                $"Ammo: {tank.weapon.ammoCurrent}/{tank.weapon.ammoMax}\nFirerate: {tank.weapon.fireRateMaxPerMinute}r/m\nReload time: {tank.weapon.reloadTimeMax}\n" +
                $"Number of MG {tank.listWeapon.Count}";
        }
        else //Pìchota
        {
            text.text = $"Stats\nHP: {unit.Hp}\nRange: {unit.range}\nSkill: {unit.skill}\n" +
                $"Weapon\nDamage: {unit.weapon.damage}\nPiercing: {unit.weapon.piercing}\nAccuracy: {unit.weapon.accuracyWeapon}\n" +
                $"Ammo: {unit.weapon.ammoCurrent}/{unit.weapon.ammoMax}\nFirerate: {unit.weapon.fireRateMaxPerMinute}r/m\nReload time: {unit.weapon.reloadTimeMax}";
        }
    }

    void CreateUnitsInfoPanel(Battalions bat)
    {
        foreach (var button in listUiUnitsInfoPanelPrefabActive)
        {
            GameObject.Destroy(button);
        }
        listUiUnitsInfoPanelPrefabActive.Clear();
        foreach (GameObject unit in currentBattalionDisplaying.Units)
        {
            UnitStats unitS = unit.GetComponent<UnitStats>();
            GameObject buttonObject = GameObject.Instantiate(uiUnitsInfoPanelPrefab, uiUnitsInfoPanel.transform);
            buttonObject.GetComponent<UnityEngine.UI.Image>().sprite = unitS.icon;
            float healthRatio = (float)unitS.Hp / (float)unitS.maxHp;
            buttonObject.GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(Color.red, Color.white, healthRatio);
            listUiUnitsInfoPanelPrefabActive.Add(buttonObject);
        }
    }
}

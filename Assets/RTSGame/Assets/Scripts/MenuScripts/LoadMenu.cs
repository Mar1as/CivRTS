using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenu
{
    MenuManager menuManager;
    List<TMPro.TMP_Dropdown> dropdownTmps;

    public static List<Faction> chosenFactions;
    TMPro.TMP_InputField scoreInput;
    TMPro.TMP_InputField moneyInput;

    public static int money, score;

    public LoadMenu(MenuManager menuManager, List<TMPro.TMP_Dropdown> list, TMPro.TMP_InputField scoreInput, TMPro.TMP_InputField moneyInput)
    {
        this.menuManager = menuManager;
        dropdownTmps = list;

        chosenFactions = new List<Faction>();
        this.scoreInput = scoreInput;
        this.moneyInput = moneyInput;
    }

    public void Load()
    {
        foreach (TMPro.TMP_Dropdown dropdown in dropdownTmps)
        {
            if (dropdown == dropdownTmps[dropdownTmps.Count - 1]) break; //Mapa dropdown

            int chosenFaction = dropdown.value;
            chosenFactions.Add(menuManager.listFactions[chosenFaction]);
        }
        string mapName = dropdownTmps[dropdownTmps.Count - 1].options[dropdownTmps[dropdownTmps.Count - 1].value].text;

        money = int.Parse(moneyInput.text);
        score = int.Parse(scoreInput.text);

        Score.endScore = score;
        //Teams.moneyToGive = money;
        //Teams.factions = chosenFactions;
        SceneManager.LoadScene(mapName);
    }
}

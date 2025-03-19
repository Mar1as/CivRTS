using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setup
{
    List<Faction> factions;
    MenuManager menuManager;
    List<TMPro.TMP_Dropdown> dropdown;
    List<Object> sceneAsset;

    public Setup(MenuManager mm, List<Faction> factions, List<TMPro.TMP_Dropdown> dropdown, List<Object> sceneAsset)
    {
        menuManager = mm;
        this.factions = factions;
        this.dropdown = dropdown;
        this.sceneAsset = sceneAsset;

        //SetupDropdowns();
    }

    public void Update()
    {

    }

    private void SetupDropdowns()
    {
        foreach (TMP_Dropdown drops in dropdown)
        {
            if (drops == dropdown[dropdown.Count - 1]) break;

            drops.ClearOptions();

            List<TMP_Dropdown.OptionData> optionss = new List<TMP_Dropdown.OptionData>();

            foreach (Faction faction in factions)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(faction.name, faction.flag, Color.black);
                optionss.Add(option);
            }

            drops.AddOptions(optionss);

            drops.value = 0;
        }
        
        ////////////////
        
        TMP_Dropdown drop = dropdown[dropdown.Count - 1];

        drop.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (Object map in sceneAsset)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(map.name);
            options.Add(option);
        }

        drop.AddOptions(options);

        drop.value = 0;


    }
}

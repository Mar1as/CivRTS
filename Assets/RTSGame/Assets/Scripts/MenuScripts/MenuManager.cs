using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] public List<Faction> listFactions;
    [SerializeField] List<TMPro.TMP_Dropdown> dropdownTmps;
    [SerializeField] List<Object> sceneAsset;
    [Header("Load Menu")]
    [SerializeField] TMPro.TMP_InputField scoreInput;
    [SerializeField] TMPro.TMP_InputField moneyInput;

    [Header("Prefabs for buttons")]
    [SerializeField] GameObject panelBack;

    Setup setup;
    LoadMenu loadMenu;

    private void Start()
    {
        setup = new Setup(this, listFactions, dropdownTmps, sceneAsset);
        loadMenu = new LoadMenu(this, dropdownTmps, scoreInput, moneyInput);
    }

    public void LoadButton()
    {
        loadMenu.Load();
    }
    public void EnableDisablePanel()
    {
        panelBack.SetActive(!panelBack.activeSelf);
    }
    /*
    private void Awake()
    {
        setup = new Setup(this, listFactions, dropdown);
    }*/


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine;
using TMPro;

public class UIUnitListStatic : MonoBehaviour
{
    [Header("UI List of units")]
    [SerializeField] public GameObject panelOfUnits;
    [SerializeField] public GameObject unitButtonPrefab;
    [Header("UI Units stats")]
    [SerializeField] public GameObject uiUnitStatsPanel;
    [SerializeField] public GameObject uiUnitStatsPanelPrefab;
    [Header("UI Units Info")]
    [SerializeField] public GameObject uiUnitsInfoPanel;
    [SerializeField] public GameObject uiUnitsInfoPanelPrefab;
    [Header("UI Shop")]
    [SerializeField] public GameObject uiShop;
    [SerializeField] public GameObject uiShopButtonPrefab;
    [SerializeField] public GameObject uiShopButtonPrefabForArmy;



    public static UIUnitListStatic Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
    }
}


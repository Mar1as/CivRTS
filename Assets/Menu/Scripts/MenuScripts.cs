using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MenuScripts : MonoBehaviour
{
    [SerializeField] private GameObject panelForPanels;
    [SerializeField] private GameObject startGameUI, createMapUI, optionsUI;
    [SerializeField] private LayerMask[] bgLayers;


    private Dictionary<GameObject, bool> panels = new Dictionary<GameObject, bool>();
    private GameObject currentPanel;

    [Header("Start Game UI")]
    [SerializeField] private GameObject panelForSaves;
    [SerializeField] private GameObject prefab;
    [SerializeField] private TMP_Dropdown dropDown;
    [SerializeField] private Button startGameButton;


    public static int currentFileIndex;
    public static int currentFactionIndex;


    int CurrentFileIndex
    {
        get => currentFileIndex;
        set
        {
            if (value < 0) startGameButton.interactable = false;
            else startGameButton.interactable = true;
            currentFileIndex = value;
        }
    }

    public static string[] saveFiles;

    void Start()
    {
        CurrentFileIndex = int.MinValue;

        UpdateSaves();
        UpdateFactions();

        panels.Add(startGameUI, false);
        panels.Add(createMapUI, false);
        panels.Add(optionsUI, false);

        foreach (var panel in panels.Keys)
        {
            panel.SetActive(true);
        }
    }

    void Update()
    {
        ClickedElsewhere();
    }

    GameObject GMOverPointer()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                Debug.Log(results[0].gameObject);
                return results[0].gameObject;
            }
        }
        return null;
    }

    bool ClickedBackground(GameObject gm)
    {
        foreach (LayerMask layer in bgLayers)
        {
            Debug.Log($"{gm.layer} == {layer.value}");

            // Zkontrolujte, zda je vrstva obsažena v LayerMask
            if ((layer.value & (1 << gm.layer)) != 0)
            {
                Debug.Log("Clicked background");
                return true;
            }
        }
        return false;
    }

    void ClickedElsewhere()
    {
        if (Input.GetMouseButtonDown(0))
        {

            if (ClickedBackground(GMOverPointer()) && currentPanel != null)
            {
                Debug.Log(GMOverPointer());
                MovePanelBack(currentPanel);
                currentPanel = null;
                CurrentFileIndex = int.MinValue;
            }
        }
    }

    

    void MovePanelForward(GameObject panel)
    {
        if (panel == null) return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform != null && panel != currentPanel)
        {
            Animator animator = panel.GetComponent<Animator>();
            animator.SetTrigger("Open");

            currentPanel = panel;
        }
    }

    void MovePanelBack(GameObject panel)
    {
        if (panel == null) return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Animator animator = panel.GetComponent<Animator>();
            animator.SetTrigger("Close");

            currentPanel = null;
        }
    }

    public void StartGameUI()
    {
        if (currentPanel != startGameUI)
        {
            if (currentPanel != null) MovePanelBack(currentPanel);
            MovePanelForward(startGameUI);
        }
    }

    public void CreateMapUI()
    {
        if (currentPanel != createMapUI)
        {
            if (currentPanel != null) MovePanelBack(currentPanel);
            MovePanelForward(createMapUI);
        }
    }

    public void OptionsUI()
    {
        if (currentPanel != optionsUI)
        {
            if (currentPanel != null) MovePanelBack(currentPanel);
            MovePanelForward(optionsUI);
        }
    }

    public void ExitUI()
    {
        Application.Quit();
    }

    #region Start Game UI

    void UpdateSaves()
    {
        string saveFolderPath = Application.persistentDataPath;
        saveFiles = Directory.GetFiles(saveFolderPath, "*.map");
        List<string> saves = new List<string>();

        for (int i = 0; i < saveFiles.Length; i++)
        {
            string filePath = saveFiles[i];
            saves.Add(Path.GetFileName(filePath));

            GameObject but = Instantiate(prefab, panelForSaves.transform);
            but.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(filePath);

            int currentIndex = i;
            but.GetComponent<Button>().onClick.AddListener(() => CurrentFileIndex = currentIndex);
        }

        foreach (string save in saves)
        {
            Debug.Log("Save file: " + save);
        }    
    }

    private void UpdateFactions()
    {
        //dropDown.ClearOptions();

        for (int i = 0; i < CivGameManagerSingleton.Instance.allFactions.Count(); i++)
        {
            FactionsInCiv civ = CivGameManagerSingleton.Instance.allFactions[i];

            Debug.Log(civ.factionName + " " + CivGameManagerSingleton.Instance.allFactions.Count());

            dropDown.options.Add(new TMP_Dropdown.OptionData(civ.name));

            // Nastavení výchozí hodnoty
            //dropDown.value = 0; // První možnost je vybrána

            // Pøidání listeneru pro zmìnu hodnoty
            //dropDown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
    }

    #endregion
}
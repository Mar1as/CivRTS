using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScripts : MonoBehaviour
{
    [SerializeField] private GameObject panelForPanels;
    [SerializeField] private GameObject startGameUI, createMapUI, optionsUI;
    [SerializeField] private LayerMask[] bgLayers;


    private Dictionary<GameObject, bool> panels = new Dictionary<GameObject, bool>();
    private GameObject currentPanel;


    [Header("Start Game UI")]
    [SerializeField] private GameObject[] panelForSaves;
    [SerializeField] private GameObject prefab;
    [SerializeField] private TMP_Dropdown dropDown;
    [SerializeField] private Image factionImage;
    [SerializeField] private Button startGameButton;
    
    [Header("Create Map UI")]
    [SerializeField] private Button createMap, editMap;
    [SerializeField] private TMP_InputField widthInput, heightInput;
    

    [Header("Options UI")]
    [SerializeField] private TMP_Dropdown optionsResolution;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Esc")]
    [SerializeField] private Canvas escPanel;
    [SerializeField] private bool menu = true;


    [Header("Static")]
    public static GameStates gameState = GameStates.MainMenu;
    public static bool loadMap = false;

    public static Vector2 chunks;

    public static int currentFileIndex;
    public static int currentFactionIndex;

    public static string[] saveFiles;

    int CurrentFileIndex
    {
        get => currentFileIndex;
        set
        {
            if (value < 0)
            {
                startGameButton.interactable = false;
                editMap.interactable = false;
            }
            else
            {
                startGameButton.interactable = true;
                editMap.interactable = true;
            }
            currentFileIndex = value;
        }
    }


    void Start()
    {
        loadMap = false;

        StartOptionsUI();

        CurrentFileIndex = int.MinValue;
        currentFactionIndex = 0;

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
        UpdateCreateMapUI();

        if (menu == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escPanel.enabled = !escPanel.enabled;
            }
        }
        
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

    void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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

            foreach (var item in panelForSaves)
            {
                GameObject but = Instantiate(prefab, item.transform);
                but.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(filePath);

                int currentIndex = i;
                but.GetComponent<Button>().onClick.AddListener(() => CurrentFileIndex = currentIndex);
            }
            
        }

        foreach (string save in saves)
        {
            Debug.Log("Save file: " + save);
        }    
    }

    public void StartGame()
    {
        gameState = GameStates.Game;
        loadMap = true;

        LoadLevel("SampleScene");
        //CivGameManagerSingleton.Instance.LoadGame(saveFiles[currentFileIndex]);
    }

    private void UpdateFactions()
    {
        //dropDown.ClearOptions();

        for (int i = 0; i < CivGameManagerSingleton.Instance.allFactions.Count(); i++)
        {
            FactionsInCiv civ = CivGameManagerSingleton.Instance.allFactions[i];

            Debug.Log(civ.factionName + " " + CivGameManagerSingleton.Instance.allFactions.Count());

            dropDown.options.Add(new TMP_Dropdown.OptionData(civ.factionName));

            // Nastavení výchozí hodnoty
            //dropDown.value = 0; // První možnost je vybrána

            // Pøidání listeneru pro zmìnu hodnoty
            //dropDown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        dropDown.onValueChanged.AddListener(ChangeDropDownIMG);

        dropDown.RefreshShownValue();
    }

    void ChangeDropDownIMG(int index)
    {
        currentFactionIndex = dropDown.value;

        factionImage.sprite = CivGameManagerSingleton.Instance.allFactions[currentFactionIndex].factionIcon;
        factionImage.color = CivGameManagerSingleton.Instance.allFactions[currentFactionIndex].factionColor;
    }

    #endregion

    #region Create Map UI

    void UpdateCreateMapUI()
    {
        int width = int.Parse(widthInput.text);
        int height = int.Parse(heightInput.text);
        if (width > 0 && height > 0)
        {
            createMap.interactable = true;
        }
        else
        {
            createMap.interactable = false;
        }
    }

    public void CreateMap()
    {
        int width = int.Parse(widthInput.text);
        int height = int.Parse(heightInput.text);

        if (createMap.interactable)
        {
            gameState = GameStates.Editor;
            chunks = new Vector2(width, height);

            LoadLevel("SampleScene");
            //Create
        }


        //CivGameManagerSingleton.Instance.mapWidth = width;
        //CivGameManagerSingleton.Instance.mapHeight = height;
        //CivGameManagerSingleton.Instance.GenerateMap();
    }

    public void EditMap()
    {
        gameState = GameStates.Editor;
        loadMap = true;

        Debug.Log("KAOTKOIDA " + gameState);

        LoadLevel("SampleScene");
    }

    #endregion

    #region Options UI

    public void BackToMenu()
    {
        LoadLevel("Menu");
    }

    Resolution[] resolutions;

    void StartOptionsUI()
    {
        resolutions = Screen.resolutions;

        optionsResolution.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        foreach (var item in resolutions)
        {
            string option = item.width + " X " + item.height;
            options.Add(option);

            if (item.width == Screen.currentResolution.width && item.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = resolutions.Length;
            }
        }

        optionsResolution.AddOptions(options);
        optionsResolution.value = currentResolutionIndex;
        optionsResolution.RefreshShownValue(); 
    }


    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetGraphics(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    #endregion
}

public enum GameStates
{
    MainMenu,
    Game,
    Editor
}
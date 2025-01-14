using UnityEngine;

public class UiManagerRTS : MonoBehaviour
{
    [SerializeField]
    private GameObject GameUi, EditUi;
    [SerializeField]
    private MapEditor mapEditor;

    [SerializeField]
    bool Game = false;

    private int[] editSetup = new int[1];
    private bool[] editorToggles = new bool[6];
    private OptionalToggle[] toggleModes = new OptionalToggle[3];
    static public EditMode editMode = EditMode.Edit;

    private void Start()
    {
        if (Game) EnableGameUi();
        else EnableEditUi();

        UpdateAllCitiesBar();
    }
    public void EnableGameUi()
    {
        SaveEditorState();
        DisableAllUiFeatures();


        if (GameUi != null) GameUi.SetActive(true);
        editMode = EditMode.Game;
        Debug.Log(editMode);
    }

    public void EnableEditUi()
    {
        DisableAllUiFeatures();
        RestoreEditorState();

        if (EditUi != null) EditUi.SetActive(true);
        editMode = EditMode.Edit;
        Debug.Log(editMode);
    }

    void DisableAllUiFeatures()
    {
        Debug.Log("Click");
        if (GameUi != null) GameUi.SetActive(false);

        if (EditUi != null) EditUi.SetActive(false);

        mapEditor.activeTerrainTypeIndex = 0;
        mapEditor.applyElevation = false;
        mapEditor.applyWaterLevel = false;
        mapEditor.applyUrbanLevel = false;
        mapEditor.applyFarmLevel = false;
        mapEditor.applyPlantLevel = false;
        mapEditor.applySpecialIndex = false;
        mapEditor.riverMode = OptionalToggle.Ignore;
        mapEditor.roadMode = OptionalToggle.Ignore;
        mapEditor.walledMode = OptionalToggle.Ignore;

    }

    void SaveEditorState()
    {
        editSetup[0] = mapEditor.activeTerrainTypeIndex;
        editorToggles[0] = mapEditor.applyElevation;
        editorToggles[1] = mapEditor.applyWaterLevel;
        editorToggles[2] = mapEditor.applyUrbanLevel;
        editorToggles[3] = mapEditor.applyFarmLevel;
        editorToggles[4] = mapEditor.applyPlantLevel;
        editorToggles[5] = mapEditor.applySpecialIndex;
        toggleModes[0] = mapEditor.riverMode;
        toggleModes[1] = mapEditor.roadMode;
        toggleModes[2] = mapEditor.walledMode;
    }

    void RestoreEditorState()
    {
        mapEditor.activeTerrainTypeIndex = editSetup[0];
        mapEditor.applyElevation = editorToggles[0];
        mapEditor.applyWaterLevel = editorToggles[1];
        mapEditor.applyUrbanLevel = editorToggles[2];
        mapEditor.applyFarmLevel = editorToggles[3];
        mapEditor.applyPlantLevel = editorToggles[4];
        mapEditor.applySpecialIndex = editorToggles[5];
        mapEditor.riverMode = toggleModes[0];
        mapEditor.roadMode = toggleModes[1];
        mapEditor.walledMode = toggleModes[2];
    }

    public static void UpdateAllCitiesBar()
    {
        MainHexCell[] hexes = CivGameManagerSingleton.Instance.hexagons;
        for (int i = 0; i < hexes.Length; i++)
        {
            hexes[i].UpdateBarText();
        }
    }
}

public enum EditMode
{
    Edit, Game
}
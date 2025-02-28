using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

[Serializable]
public class SceneSwap : MonoBehaviour
{
    public static PassInformation[] passInfo = new PassInformation[2];

    [SerializeField] string[] scenes; // Pole názvù scén
    static private Scene campaignScene; // Uložená scéna kampanì
    private GameObject[] campaignGameObjects; // Uložené GameObjects z kampanì

    private void Start()
    {
        CivGameManagerSingleton.Instance.sceneSwap = this;
        DontDestroyOnLoad(this.gameObject); // Zachování tohoto objektu pøi pøechodu scén
    }

    private void Update()
    {
        // Debug: Naètení scény bitvy po stisknutí klávesy V
        if (Input.GetKeyDown(KeyCode.V))
        {
            LoadScene(0, false);
        }
    }

    public void LoadScene(int sceneId, bool loadBattle)
    {
        if (loadBattle)
        {
            // Uložení scény kampanì a jejích GameObjects
            if (campaignScene == default)
            {
                campaignScene = SceneManager.GetActiveScene();
                campaignGameObjects = GetAllGameObjectsInScene(campaignScene);
            }

            // Deaktivace všech GameObjects v kampani
            SetActiveForAllGameObjects(campaignGameObjects, false);

            // Spustí korutinu pro naètení scény bitvy
            StartCoroutine(LoadBattleScene(sceneId));
        }
        else
        {
            // Návrat do scény kampanì
            if (campaignScene != default)
            {
                StartCoroutine(LoadCampaignScene(sceneId));
            }
            else
            {
                Debug.LogError("Nebyla uložena žádná scéna kampanì!");
            }
        }
    }

    private IEnumerator LoadBattleScene(int sceneId)
    {
        // Asynchronní naètení scény bitvy
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenes[sceneId], LoadSceneMode.Additive);

        // Èekání na dokonèení naètení
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Nastavení scény bitvy jako aktivní
        Scene battleScene = SceneManager.GetSceneByName(scenes[sceneId]);
        if (battleScene.isLoaded)
        {
            SceneManager.SetActiveScene(battleScene);
        }
        else
        {
            Debug.LogError("Chyba: Scéna bitvy se nenaèetla správnì!");
        }
    }

    private IEnumerator LoadCampaignScene(int sceneId)
    {
        // Aktivace všech GameObjects v kampani
        SetActiveForAllGameObjects(campaignGameObjects, true);

        // Nastavení scény kampanì jako aktivní
        SceneManager.SetActiveScene(campaignScene);

        // Uvolnìní scény bitvy
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scenes[sceneId]);

        // Èekání na dokonèení uvolnìní
        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        // Resetování uložené scény kampanì (volitelné)
        campaignScene = default;
    }

    // Funkce pro získání všech GameObjectù ve scénì
    private GameObject[] GetAllGameObjectsInScene(Scene scene)
    {
        GameObject[] gameObjects = new GameObject[scene.rootCount];
        for (int i = 0; i < scene.rootCount; i++)
        {
            gameObjects[i] = scene.GetRootGameObjects()[i];
        }
        return gameObjects;
    }

    // Funkce pro nastavení aktivity všech GameObjectù
    private void SetActiveForAllGameObjects(GameObject[] gameObjects, bool isActive)
    {
        foreach (var obj in gameObjects)
        {
            obj.SetActive(isActive);
        }
    }
}

[Serializable]
public class PassInformation
{
    public Player player { get; private set; }
    public DataHexUnitArmy army { get; private set; }
    PlayerStateInBattle state;
    bool ai;

    public PassInformation(Player player, DataHexUnitArmy army, PlayerStateInBattle state)
    {
        this.player = player;
        this.army = army;
        this.state = state;
        this.ai = player.ai;
    }
}

public enum PlayerStateInBattle
{
    Attacker,
    Defender
}
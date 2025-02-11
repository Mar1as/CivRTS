using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

[Serializable]
public class SceneSwap : MonoBehaviour
{
    public static PassInformation[] passInfo = new PassInformation[2];

    [SerializeField] string[] scenes;
    static private Scene campaignScene;
    private GameObject[] campaignGameObjects;

    private void Start()
    {
        CivGameManagerSingleton.Instance.sceneSwap = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            LoadScene(0, false);
        }
    }

    public void LoadScene(int sceneId, bool loadBattle)
    {
        //sceneId = 1; //Debug
        if (loadBattle)
        {
            if (campaignScene == default)
            {
                campaignScene = SceneManager.GetActiveScene();
                campaignGameObjects = GetAllGameObjectsInScene(campaignScene);
            }

            SetActiveForAllGameObjects(campaignGameObjects, false);

            // Spustí korutinu pro naètení bitvy
            StartCoroutine(LoadBattleScene(sceneId));
        }
        else
        {
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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenes[sceneId], LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null; // Poèkáme na naètení scény
        }

        Scene battleScene = SceneManager.GetSceneByName(scenes[sceneId]);
        if (battleScene.isLoaded)
        {
            SceneManager.SetActiveScene(battleScene);
            SceneManager.UnloadSceneAsync(campaignScene.name);
        }
        else
        {
            Debug.LogError("Chyba: Scéna bitvy se nenaèetla správnì!");
        }
    }

    private IEnumerator LoadCampaignScene(int sceneId)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(campaignScene.name, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SetActiveForAllGameObjects(campaignGameObjects, true);
        SceneManager.SetActiveScene(campaignScene);
        SceneManager.UnloadSceneAsync(scenes[sceneId]);
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

using System;
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
        if (loadBattle)
        {
            // Uloží aktuální scénu kampanì, pokud není již uložena
            if (campaignScene == default)
            {
                campaignScene = SceneManager.GetActiveScene();
                // Uloží všechny GameObjecty v kampani
                campaignGameObjects = GetAllGameObjectsInScene(campaignScene);
            }

            // Vypne všechny GameObjecty v kampani
            SetActiveForAllGameObjects(campaignGameObjects, false);

            // Naète scénu bitvy v additivním režimu
            SceneManager.LoadScene(scenes[sceneId], LoadSceneMode.Additive);

            // Nastaví novou scénu jako aktivní
            Scene battleScene = SceneManager.GetSceneByName(scenes[sceneId]);
            SceneManager.SetActiveScene(battleScene);

            // Odstraní scénu kampanì
            SceneManager.UnloadSceneAsync(campaignScene.name);
        }
        else
        {
            // Zpìt do kampanì: naète uloženou scénu kampanì
            if (campaignScene != default)
            {
                // Kontrola, zda scéna kampanì již není naètena
                if (!SceneManager.GetSceneByName(campaignScene.name).isLoaded)
                {
                    SceneManager.LoadScene(campaignScene.name, LoadSceneMode.Additive);
                }

                // Zapne všechny GameObjecty v kampani
                SetActiveForAllGameObjects(campaignGameObjects, true);

                // Nastaví kampanì jako aktivní
                SceneManager.SetActiveScene(campaignScene);

                // Odstraní scénu bitvy
                SceneManager.UnloadSceneAsync(scenes[sceneId]);

                // Vyèistí uloženou scénu kampanì
                campaignScene = default;
            }
            else
            {
                Debug.LogError("Nebyla uložena žádná scéna kampanì!");
            }
        }
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

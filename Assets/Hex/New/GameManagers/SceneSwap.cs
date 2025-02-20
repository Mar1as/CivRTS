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
            // Ulo�� aktu�ln� sc�nu kampan�, pokud nen� ji� ulo�ena
            if (campaignScene == default)
            {
                campaignScene = SceneManager.GetActiveScene();
                // Ulo�� v�echny GameObjecty v kampani
                campaignGameObjects = GetAllGameObjectsInScene(campaignScene);
            }

            // Vypne v�echny GameObjecty v kampani
            SetActiveForAllGameObjects(campaignGameObjects, false);

            // Na�te sc�nu bitvy v additivn�m re�imu
            SceneManager.LoadScene(scenes[sceneId], LoadSceneMode.Additive);

            // Nastav� novou sc�nu jako aktivn�
            Scene battleScene = SceneManager.GetSceneByName(scenes[sceneId]);
            SceneManager.SetActiveScene(battleScene);

            // Odstran� sc�nu kampan�
            SceneManager.UnloadSceneAsync(campaignScene.name);
        }
        else
        {
            // Zp�t do kampan�: na�te ulo�enou sc�nu kampan�
            if (campaignScene != default)
            {
                // Kontrola, zda sc�na kampan� ji� nen� na�tena
                if (!SceneManager.GetSceneByName(campaignScene.name).isLoaded)
                {
                    SceneManager.LoadScene(campaignScene.name, LoadSceneMode.Additive);
                }

                // Zapne v�echny GameObjecty v kampani
                SetActiveForAllGameObjects(campaignGameObjects, true);

                // Nastav� kampan� jako aktivn�
                SceneManager.SetActiveScene(campaignScene);

                // Odstran� sc�nu bitvy
                SceneManager.UnloadSceneAsync(scenes[sceneId]);

                // Vy�ist� ulo�enou sc�nu kampan�
                campaignScene = default;
            }
            else
            {
                Debug.LogError("Nebyla ulo�ena ��dn� sc�na kampan�!");
            }
        }
    }

    // Funkce pro z�sk�n� v�ech GameObject� ve sc�n�
    private GameObject[] GetAllGameObjectsInScene(Scene scene)
    {
        GameObject[] gameObjects = new GameObject[scene.rootCount];
        for (int i = 0; i < scene.rootCount; i++)
        {
            gameObjects[i] = scene.GetRootGameObjects()[i];
        }
        return gameObjects;
    }

    // Funkce pro nastaven� aktivity v�ech GameObject�
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

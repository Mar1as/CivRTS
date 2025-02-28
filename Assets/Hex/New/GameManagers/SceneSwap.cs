using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

[Serializable]
public class SceneSwap : MonoBehaviour
{
    public static PassInformation[] passInfo = new PassInformation[2];

    [SerializeField] string[] scenes; // Pole n�zv� sc�n
    static private Scene campaignScene; // Ulo�en� sc�na kampan�
    private GameObject[] campaignGameObjects; // Ulo�en� GameObjects z kampan�

    private void Start()
    {
        CivGameManagerSingleton.Instance.sceneSwap = this;
        DontDestroyOnLoad(this.gameObject); // Zachov�n� tohoto objektu p�i p�echodu sc�n
    }

    private void Update()
    {
        // Debug: Na�ten� sc�ny bitvy po stisknut� kl�vesy V
        if (Input.GetKeyDown(KeyCode.V))
        {
            LoadScene(0, false);
        }
    }

    public void LoadScene(int sceneId, bool loadBattle)
    {
        if (loadBattle)
        {
            // Ulo�en� sc�ny kampan� a jej�ch GameObjects
            if (campaignScene == default)
            {
                campaignScene = SceneManager.GetActiveScene();
                campaignGameObjects = GetAllGameObjectsInScene(campaignScene);
            }

            // Deaktivace v�ech GameObjects v kampani
            SetActiveForAllGameObjects(campaignGameObjects, false);

            // Spust� korutinu pro na�ten� sc�ny bitvy
            StartCoroutine(LoadBattleScene(sceneId));
        }
        else
        {
            // N�vrat do sc�ny kampan�
            if (campaignScene != default)
            {
                StartCoroutine(LoadCampaignScene(sceneId));
            }
            else
            {
                Debug.LogError("Nebyla ulo�ena ��dn� sc�na kampan�!");
            }
        }
    }

    private IEnumerator LoadBattleScene(int sceneId)
    {
        // Asynchronn� na�ten� sc�ny bitvy
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenes[sceneId], LoadSceneMode.Additive);

        // �ek�n� na dokon�en� na�ten�
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Nastaven� sc�ny bitvy jako aktivn�
        Scene battleScene = SceneManager.GetSceneByName(scenes[sceneId]);
        if (battleScene.isLoaded)
        {
            SceneManager.SetActiveScene(battleScene);
        }
        else
        {
            Debug.LogError("Chyba: Sc�na bitvy se nena�etla spr�vn�!");
        }
    }

    private IEnumerator LoadCampaignScene(int sceneId)
    {
        // Aktivace v�ech GameObjects v kampani
        SetActiveForAllGameObjects(campaignGameObjects, true);

        // Nastaven� sc�ny kampan� jako aktivn�
        SceneManager.SetActiveScene(campaignScene);

        // Uvoln�n� sc�ny bitvy
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scenes[sceneId]);

        // �ek�n� na dokon�en� uvoln�n�
        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        // Resetov�n� ulo�en� sc�ny kampan� (voliteln�)
        campaignScene = default;
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
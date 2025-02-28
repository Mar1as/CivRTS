using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LoadSecondScene();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ReturnToFirstScene();
        }
    }

    // N�zev prvn� sc�ny (nap�. "MainScene")
    public string firstSceneName = "TestScene";

    // N�zev druh� sc�ny (nap�. "CombatScene")
    public string secondSceneName = "SecoundScene";

    // Na�te druhou sc�nu a deaktivuje prvn�
    public void LoadSecondScene()
    {
        // Na�te druhou sc�nu
        SceneManager.LoadScene(secondSceneName, LoadSceneMode.Additive);

        // Z�sk�n� prvn� sc�ny
        Scene firstScene = SceneManager.GetSceneByName(firstSceneName);

        // Deaktivace v�ech GameObjects v prvn� sc�n�
        GameObject[] rootObjects = firstScene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(false);
        }
    }

    // Vr�t� se k prvn� sc�n� a aktivuje ji
    public void ReturnToFirstScene()
    {
        // Z�sk�n� prvn� sc�ny
        Scene firstScene = SceneManager.GetSceneByName(firstSceneName);

        // Aktivace v�ech GameObjects v prvn� sc�n�
        GameObject[] rootObjects = firstScene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(true);
        }

        // Uvoln�n� druh� sc�ny
        SceneManager.UnloadSceneAsync(secondSceneName);
    }
}

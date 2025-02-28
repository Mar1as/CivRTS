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

    // Název první scény (napø. "MainScene")
    public string firstSceneName = "TestScene";

    // Název druhé scény (napø. "CombatScene")
    public string secondSceneName = "SecoundScene";

    // Naète druhou scénu a deaktivuje první
    public void LoadSecondScene()
    {
        // Naète druhou scénu
        SceneManager.LoadScene(secondSceneName, LoadSceneMode.Additive);

        // Získání první scény
        Scene firstScene = SceneManager.GetSceneByName(firstSceneName);

        // Deaktivace všech GameObjects v první scénì
        GameObject[] rootObjects = firstScene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(false);
        }
    }

    // Vrátí se k první scénì a aktivuje ji
    public void ReturnToFirstScene()
    {
        // Získání první scény
        Scene firstScene = SceneManager.GetSceneByName(firstSceneName);

        // Aktivace všech GameObjects v první scénì
        GameObject[] rootObjects = firstScene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(true);
        }

        // Uvolnìní druhé scény
        SceneManager.UnloadSceneAsync(secondSceneName);
    }
}

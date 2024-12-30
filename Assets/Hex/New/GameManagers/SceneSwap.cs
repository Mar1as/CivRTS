using System.Collections.Generic;
using UnityEngine;

public class SceneSwap : MonoBehaviour
{
    public static ArmyHexUnit[] armyArray = new ArmyHexUnit[2];

    private void Start()
    {
        CivGameManagerSingleton.Instance.sceneSwap = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadScene(int sceneId)
    {
        Debug.Log(CivGameManagerSingleton.Instance.players.Length);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
        Debug.Log(CivGameManagerSingleton.Instance.players.Length);
    }
}

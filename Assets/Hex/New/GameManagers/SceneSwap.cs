using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SceneSwap : MonoBehaviour
{
    public static PassInformation[] passInfo = new PassInformation[2];

    

    private void Start()
    {
        CivGameManagerSingleton.Instance.sceneSwap = this;
        DontDestroyOnLoad(this.gameObject);

    }

    public void LoadScene(int sceneId)
    {
        Debug.Log(passInfo[0].army.unitsInArmy.Count);
        //Debug.Log(CivGameManagerSingleton.Instance.players.Length);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
        //Debug.Log(CivGameManagerSingleton.Instance.players.Length);
        Debug.Log(passInfo[0].army.unitsInArmy.Count);

    }
}


[Serializable]
public class PassInformation
{
    public Player player { get; private set; }
    public DataHexUnitArmy army { get; private set; }

    public PassInformation(Player player, DataHexUnitArmy army)
    {
        this.player = player;
        this.army = army;
    }
}

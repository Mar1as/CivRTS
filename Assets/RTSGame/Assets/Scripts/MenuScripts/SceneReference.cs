using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SceneReference
{
    [SerializeField]
    private Object sceneAsset = null;

    [SerializeField]
    private string sceneName = "";

    public string SceneName => sceneName;

    public static implicit operator string(SceneReference sceneReference)
    {
        return sceneReference.SceneName;
    }
}

using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Scene/Scene Database")]
public class SceneDatabase : ScriptableObject
{
    public List<SceneEntry> scenes = new List<SceneEntry>();
}


[Serializable]
public class SceneEntry
{
    public string guid;
    public string sceneName;
    public bool sceneSwitch;
    public int mode;
    public string customPath;
}
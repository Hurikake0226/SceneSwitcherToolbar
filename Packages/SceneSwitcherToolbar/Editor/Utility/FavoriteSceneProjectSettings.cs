#if UNITY_EDITOR
using UnityEngine;
using SceneSwitcherToolbar.FavoriteSettingWindow;


public class FavoriteSceneSettings : ScriptableObject
{
    public FavoriteSceneSettingsSave[] favoriteScenes;
}
#endif
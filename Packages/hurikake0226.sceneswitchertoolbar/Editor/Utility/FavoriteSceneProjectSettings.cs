#if UNITY_EDITOR
using UnityEngine;
namespace SceneSwitcherToolbar
{
    public class FavoriteSceneSettings : ScriptableObject
    {
        public FavoriteSceneSettingsSave[] favoriteScenes;
    }
}
#endif
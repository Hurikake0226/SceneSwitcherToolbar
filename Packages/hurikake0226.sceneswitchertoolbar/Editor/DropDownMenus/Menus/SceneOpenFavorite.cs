#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using SceneSwitcherToolbar.FavoriteSettingWindow;

namespace SceneSwitcherToolbar.DropDownMenu.Menus
{
    public static class SceneOpenFavorite
    {
        public static void Build(GenericMenu menu)
        {
            var settings = FavoriteSceneSettingsProvider.LoadCurrent();
            if (settings == null || settings.favoriteScenes == null)
                return;

            foreach (var favorite in settings.favoriteScenes)
            {
                if (favorite?.activeScene == null)
                    continue;

                menu.AddItem(
                    new GUIContent($"Favorite/{settings.name}/{favorite.favoriteName}"),
                    false,
                    () => OpenScene(favorite)
                );
            }
        }


        private static void OpenScene(FavoriteSceneSettingsSave favorite)
        {
            if (favorite.activeScene == null)
                return;

            // 保存確認
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            // Active Scene
            EditorSceneManager.OpenScene(
                AssetDatabase.GetAssetPath(favorite.activeScene),
                OpenSceneMode.Single
            );

            // Additive Scenes
            if (favorite.additiveScene == null)
                return;

            foreach (var scene in favorite.additiveScene)
            {
                if (scene == null)
                    continue;

                EditorSceneManager.OpenScene(
                    AssetDatabase.GetAssetPath(scene),
                    OpenSceneMode.Additive
                );
            }
        }
    }
}
#endif
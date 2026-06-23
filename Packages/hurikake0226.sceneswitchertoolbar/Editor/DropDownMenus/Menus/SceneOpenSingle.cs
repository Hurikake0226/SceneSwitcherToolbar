#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SceneSwitcherToolbar.DropDownMenu.Menus
{
    public static class SceneOpenSingle
    {
        public static void Build(GenericMenu menu, SceneAssetData scene, string activeSceneName)
        {
            menu.AddItem(
                new GUIContent($"Single/{scene.name}"),
                scene.name == activeSceneName,
                () => Open(scene.path)
            );
        }

        public static void Build(GenericMenu menu, ConfiguredSceneData configured, string activeSceneName)
        {
            if (configured == null || configured.entry == null) return;

            var mode = (SceneType)configured.entry.mode;
            if (mode != SceneType.Default && mode != SceneType.Single) return;

            var scene = configured.assetData;
            menu.AddItem(
                new GUIContent($"Single/{scene.name}"),
                scene.name == activeSceneName,
                () => Open(scene.path)
            );
        }

        private static void Open(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            }
        }
    }
}
#endif

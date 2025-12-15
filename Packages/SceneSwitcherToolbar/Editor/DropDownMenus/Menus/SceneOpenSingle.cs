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
                () => Open(scene.name, scene.path)
            );
        }

        private static void Open(string name, string path)
        {
            // 保存の確認
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            }
        }
    }
}
#endif
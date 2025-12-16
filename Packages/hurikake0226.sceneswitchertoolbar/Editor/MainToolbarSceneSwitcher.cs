#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Toolbars;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SceneSwitcherToolbar.DropDownMenu;

namespace SceneSwitcherToolbar
{
    public class MainToolbarSceneSwitcher
    {
        const string kElementPath = "Examples/SceneSwitcher";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            // シーンが開かれたときにツールバーを更新
            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                MainToolbar.Refresh(kElementPath);
            };

            // アクティブシーンが変更されたときにツールバーを更新
            EditorSceneManager.activeSceneChangedInEditMode += (oldScene, newScene) =>
            {
                MainToolbar.Refresh(kElementPath);
            };

            // シーンが保存されたときにツールバーを更新
            EditorSceneManager.sceneSaved += (scene) =>
            {
                MainToolbar.Refresh(kElementPath);
            };
        }

        [MainToolbarElement(kElementPath, defaultDockPosition = MainToolbarDockPosition.Right)]
        public static MainToolbarElement Create()
        {
            var icon = EditorGUIUtility.IconContent("SceneAsset Icon").image as Texture2D;

            var content = new MainToolbarContent()
            {
                image = icon,
                text = "Untitled",
                tooltip = "Scene Switcher"
            };

            // テキスト部分を現在のシーン名に変更
            var activeScene = SceneManager.GetActiveScene();
            content.text = string.IsNullOrEmpty(activeScene.name) ? "Untitled" : activeScene.name;

            return new MainToolbarDropdown(content, rect => SceneSwitcherMenu.Show(rect));
        }
    }
}
#endif
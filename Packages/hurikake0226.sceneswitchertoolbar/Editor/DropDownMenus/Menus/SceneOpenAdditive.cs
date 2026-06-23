#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SceneSwitcherToolbar.DropDownMenu.Menus
{
    public static class SceneOpenAdditive
    {
        public static void Build(GenericMenu menu, SceneAssetData scene)
        {
            AddMenuItem(menu, scene);
        }

        public static void Build(GenericMenu menu, ConfiguredSceneData configured)
        {
            if (configured == null || configured.entry == null) return;

            var mode = (SceneType)configured.entry.mode;
            if (mode != SceneType.Default && mode != SceneType.Additive) return;

            AddMenuItem(menu, configured.assetData);
        }

        private static void AddMenuItem(GenericMenu menu, SceneAssetData scene)
        {
            var content = new GUIContent($"Additive/{scene.name}");
            if (SceneSwitcherUtility.IsSceneLoaded(scene.path))
            {
                menu.AddDisabledItem(content);
                return;
            }

            menu.AddItem(content, false, () => Open(scene.path));
        }

        private static void Open(string path)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        }
    }
}
#endif

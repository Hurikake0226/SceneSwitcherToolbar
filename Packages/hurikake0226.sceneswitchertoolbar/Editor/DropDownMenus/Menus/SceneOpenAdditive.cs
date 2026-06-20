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
            if (SceneSwitcherUtility.IsSceneLoaded(scene.path))
            {
                menu.AddDisabledItem(
                    new GUIContent($"Additive/{scene.name}")
                );
                return;
            }

            menu.AddItem(
                new GUIContent($"Additive/{scene.name}"),
                false,
                () => Open(scene.path)
            );
        }

        public static void Build(GenericMenu menu, ConfiguredSceneData configured)
        {
            // モードがシングルorデフォルトの場合
            if (configured == null || configured.entry.mode == 0 || configured.entry.mode == 2)
            {
                var scene = configured.assetData;

                if (SceneSwitcherUtility.IsSceneLoaded(scene.path))
                {
                    // 表示はするが選択できないようにする
                    menu.AddDisabledItem(
                        new GUIContent($"Additive/{scene.name}")
                    );
                    return;
                }


                menu.AddItem(
                    new GUIContent($"Additive/{scene.name}"),
                    false,
                    () => Open(scene.path)
                );
            }
        }

        private static void Open(string path)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        }
    }
}
#endif

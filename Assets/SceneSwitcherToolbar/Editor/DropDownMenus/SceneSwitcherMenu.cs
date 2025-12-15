#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneSwitcherToolbar.DropDownMenu.Menus;

namespace SceneSwitcherToolbar.DropDownMenu
{
    public static class SceneSwitcherMenu
    {
        public static void Show(Rect rect)
        {
            var menu = new GenericMenu();
            var scenes = SceneSwitcherUtility.GetAllProjectScenes();

            if (scenes.Length == 0) return;

            string activeSceneName = SceneManager.GetActiveScene().name;

            foreach (var scene in scenes)
            {
                SceneOpenSingle.Build(menu, scene, activeSceneName);
                SceneOpenAdditive.Build(menu, scene);
                SceneOpenFavorite.Build(menu);
            }

            // お気に入り設定
            menu.AddSeparator("");
            WindowOpenFavoriteSetting.Build(menu);

            // 新しいシーンを作成
            menu.AddSeparator("");
            SceneCreateAction.Build(menu);

            menu.DropDown(rect);
        }
    }
}
#endif
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
            var scemesnew = SceneSwitcherUtility.GetConfiguredScenes();

            if (scemesnew.Count == 0) return;

            string activeSceneName = SceneManager.GetActiveScene().name;

            foreach (var scene in scemesnew)
            {
                SceneOpenSingle.Build(menu, scene, activeSceneName);
            }

            // Additive は後でまとめて追加
            foreach (var scene in scemesnew)
            {
                SceneOpenAdditive.Build(menu, scene);
            }

            // Favorite も最後にまとめる（必要なら）
            foreach (var scene in scemesnew)
            {
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
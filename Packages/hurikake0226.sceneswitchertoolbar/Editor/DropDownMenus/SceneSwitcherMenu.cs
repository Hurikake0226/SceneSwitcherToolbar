#if UNITY_EDITOR
using SceneSwitcherToolbar.DropDownMenu.Menus;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSwitcherToolbar.DropDownMenu
{
    public static class SceneSwitcherMenu
    {
        public static void Show(Rect rect)
        {
            var menu = new GenericMenu();
            var configuredScenes = SceneSwitcherUtility.GetConfiguredScenes();

            if (configuredScenes.Count == 0) return;

            string activeSceneName = SceneManager.GetActiveScene().name;

            foreach (var scene in configuredScenes)
            {
                SceneOpenSingle.Build(menu, scene, activeSceneName);
            }

            foreach (var scene in configuredScenes)
            {
                SceneOpenAdditive.Build(menu, scene);
            }

            SceneOpenFavorite.Build(menu);

            menu.AddSeparator("");
            WindowOpenFavoriteSetting.Build(menu);

            menu.AddSeparator("");
            SceneCreateAction.Build(menu);

            menu.DropDown(rect);
        }
    }
}
#endif

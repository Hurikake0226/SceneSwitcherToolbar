#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SceneSwitcherToolbar.FavoriteSettingWindow;

namespace SceneSwitcherToolbar.DropDownMenu.Menus
{

    public class WindowOpenFavoriteSetting
    {
        private const string MenuPath = "Tools/SceneSwitcherToolbar/Favorite Setting";

        public static void Build(GenericMenu menu)
        {
            menu.AddItem(
                new GUIContent($"Favorite Setting"),
                false,
                () => Open()
            );
        }

        private static void Open()
        {
            EditorApplication.ExecuteMenuItem(MenuPath);
        }
    }
}
#endif
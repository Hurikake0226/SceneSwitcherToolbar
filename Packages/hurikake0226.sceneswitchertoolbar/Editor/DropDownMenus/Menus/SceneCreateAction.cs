#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SceneSwitcherToolbar.DropDownMenu.Menus
{
    public static class SceneCreateAction
    {
        public static void Build(GenericMenu menu)
        {
            menu.AddItem(
                new GUIContent("New Scene"),
                false,
                Create
            );
        }

        private static void Create()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorApplication.ExecuteMenuItem("File/New Scene");
            }
        }
    }
}
#endif
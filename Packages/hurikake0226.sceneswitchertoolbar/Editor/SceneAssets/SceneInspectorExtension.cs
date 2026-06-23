#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public enum SceneType
{
    Default = 0,
    Single = 1,
    Additive = 2,
    None = 3,
}

[InitializeOnLoad]
public static class SceneInspectorExtension
{
    static SceneInspectorExtension()
    {
        Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
    }

    static void OnPostHeaderGUI(Editor editor)
    {
        if (!(editor.target is SceneAsset scene)) return;

        var entry = SceneDatabaseUtility.GetOrCreateEntry(scene);

        EditorGUI.BeginChangeCheck();

        GUILayout.Space(4);
        EditorGUILayout.BeginHorizontal();

        entry.sceneSwitch = EditorGUILayout.ToggleLeft(
            "Scene Switch",
            entry.sceneSwitch,
            GUILayout.Width(95)
        );

        if (!entry.sceneSwitch)
        {
            entry.mode = (int)SceneType.Default;
        }

        EditorGUI.BeginDisabledGroup(!entry.sceneSwitch);

        var currentMode = (SceneType)EditorGUILayout.EnumPopup(
            (SceneType)entry.mode,
            GUILayout.ExpandWidth(true)
        );

        entry.mode = (int)currentMode;

        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(SceneDatabaseUtility.GetDatabase());
        }
    }
}
#endif

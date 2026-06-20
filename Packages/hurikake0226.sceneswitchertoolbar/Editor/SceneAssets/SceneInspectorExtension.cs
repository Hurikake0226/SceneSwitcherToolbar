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
    // EnumPopupを使用するため、文字列の options 配列は不要になります

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

        // 🔵 前回のmodeを保持 (intをSceneTypeにキャストして扱う)
        SceneType prevMode = (SceneType)entry.mode;

        // 🔵 左：トグル
        entry.sceneSwitch = EditorGUILayout.ToggleLeft(
            "Scene Switch",
            entry.sceneSwitch,
            GUILayout.Width(95)
        );

        // 🔵 OFF時はDefault固定
        if (!entry.sceneSwitch)
        {
            entry.mode = (int)SceneType.Default;
        }

        // 🔵 Dropdown
        EditorGUI.BeginDisabledGroup(!entry.sceneSwitch);

        // Popupの代わりに EnumPopup を使用
        SceneType currentMode = (SceneType)EditorGUILayout.EnumPopup(
            (SceneType)entry.mode,
            GUILayout.ExpandWidth(true)
        );

        // 選択されたEnumをintに戻して保存
        entry.mode = (int)currentMode;

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();

        // 🔥 None (旧Custom) に切り替わった瞬間の処理
        if (currentMode != SceneType.None)
        {
            switch (prevMode)
            {
                case SceneType.Single:
                    entry.customPath = "Single/";
                    break;
                case SceneType.Additive:
                    entry.customPath = "Additive/";
                    break;
                    //case SceneType.Custom:
                    //    entry.customPath = "Custom/";
                    //    break;
            }
        }

        // 🔵 None (旧Custom) 時だけPath表示
        //if (entry.sceneSwitch && currentMode == SceneType.Custom)
        //{
        //    EditorGUI.indentLevel++;

        //    entry.customPath = EditorGUILayout.TextField("Path", entry.customPath);

        //    EditorGUI.indentLevel--;
        //}

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(SceneDatabaseUtility.GetDatabase());
        }
    }
}
using SceneSwitcherToolbar;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SceneDatabaseWindow : EditorWindow
{
    Vector2 scroll;
    bool showOnlyEnabled;

    [MenuItem("Tools/SceneSwitcherToolbar/Scene Database")]
    public static void Open()
    {
        GetWindow<SceneDatabaseWindow>("Scene DB");
    }

    void OnGUI()
    {
        // 取得した「設定済みシーンのリスト（一時データ）」
        var configuredScenes = SceneSwitcherUtility.GetConfiguredScenes();

        // 変更を保存するための「大元のデータベース実体」
        var dbAsset = SceneDatabaseUtility.GetDatabase();

        DrawToolbar();

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var sceneData in configuredScenes)
        {
            // SceneEntryがまだ作られていないシーンはスキップ（Refreshボタンで生成されます）
            if (sceneData.entry == null) continue;

            // Scene取得（sceneData.assetData にパス情報が入っているのでそれを利用）
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneData.assetData.path);

            // 存在しない場合スキップ
            if (scene == null) continue;

            // フィルタ
            if (showOnlyEnabled && !sceneData.entry.sceneSwitch) continue;

            DrawRow(sceneData.entry, scene);
        }

        EditorGUILayout.EndScrollView();

        // 変更があった場合は大元のデータベースを保存
        if (GUI.changed)
        {
            EditorUtility.SetDirty(dbAsset);
        }
    }

    void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        showOnlyEnabled = GUILayout.Toggle(showOnlyEnabled, "Enabled Only", EditorStyles.toolbarButton);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
        {
            RefreshDatabase();
        }

        EditorGUILayout.EndHorizontal();
    }

    void DrawRow(SceneEntry entry, SceneAsset scene)
    {
        EditorGUILayout.BeginHorizontal();

        // Scene名（クリックで選択）
        if (GUILayout.Button(scene.name, GUILayout.Width(200)))
        {
            Selection.activeObject = scene;
        }

        // トグル
        entry.sceneSwitch = EditorGUILayout.Toggle(entry.sceneSwitch, GUILayout.Width(20));

        // 🔵 OFF時はDefault固定
        if (!entry.sceneSwitch)
        {
            entry.mode = (int)SceneType.Default;
        }

        // Mode (Inspectorと同じように EnumPopup を使用)
        EditorGUI.BeginDisabledGroup(!entry.sceneSwitch);

        SceneType currentMode = (SceneType)EditorGUILayout.EnumPopup(
            (SceneType)entry.mode,
            GUILayout.Width(100) // 横幅はお好みで調整してください
        );
        entry.mode = (int)currentMode;

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();
    }

    void RefreshDatabase()
    {
        var db = SceneDatabaseUtility.GetDatabase();

        // プロジェクト内の全Sceneを検索
        var allSceneGuids = AssetDatabase.FindAssets("t:Scene");

        foreach (var guid in allSceneGuids)
        {
            if (!db.scenes.Any(s => s.guid == guid))
            {
                db.scenes.Add(new SceneEntry { guid = guid });
            }
        }

        EditorUtility.SetDirty(db);
    }
}
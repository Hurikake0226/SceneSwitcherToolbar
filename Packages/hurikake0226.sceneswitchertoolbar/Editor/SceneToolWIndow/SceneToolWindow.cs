#if UNITY_EDITOR
using SceneSwitcherToolbar;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SceneToolWindow : EditorWindow
{
    private const float LEFT_WIDTH = 180f;

    private int _menuIndex;

    private readonly string[] _menus =
    {
        "Scene Database",
        "Favorite Presets"
    };

    // ===== Scene DB =====
    private Vector2 _sceneScroll;
    private bool _showOnlyEnabled;

    // ===== Favorite =====
    private Vector2 _favoriteScroll;
    private FavoriteSceneSettings _current;
    private SerializedObject _serializedObject;
    private SerializedProperty _favoriteScenesProp;

    [MenuItem("Tools/SceneSwitcherToolbar/Scene Tool")]
    public static void Open()
    {
        GetWindow<SceneToolWindow>("Scene Tool");
    }

    private void OnEnable()
    {
        LoadCurrentPreset();
    }

    void LoadCurrentPreset()
    {
        _current = FavoriteSceneSettingsProvider.LoadCurrent();
        Bind();
    }

    void Bind()
    {
        if (_current == null) return;

        _serializedObject = new SerializedObject(_current);
        _favoriteScenesProp = _serializedObject.FindProperty("favoriteScenes");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        DrawMenu();
        DrawDetail();

        EditorGUILayout.EndHorizontal();
    }

    // =========================
    // LEFT MENU
    // =========================
    void DrawMenu()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(LEFT_WIDTH));

        GUILayout.Space(5);

        for (int i = 0; i < _menus.Length; i++)
        {
            bool selected = _menuIndex == i;

            if (GUILayout.Toggle(selected, _menus[i], "Button"))
            {
                _menuIndex = i;
            }
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndVertical();
    }

    // =========================
    // RIGHT DETAIL
    // =========================
    void DrawDetail()
    {
        EditorGUILayout.BeginVertical();

        switch (_menuIndex)
        {
            case 0:
                DrawSceneDatabase();
                break;

            case 1:
                DrawFavoritePresets();
                break;
        }

        EditorGUILayout.EndVertical();
    }

    // =========================
    // Scene Database
    // =========================
    void DrawSceneDatabase()
    {
        var configuredScenes = SceneSwitcherUtility.GetConfiguredScenes();
        var dbAsset = SceneDatabaseUtility.GetDatabase();

        DrawSceneDatabaseSelector(dbAsset);
        DrawSceneToolbar();

        _sceneScroll = EditorGUILayout.BeginScrollView(_sceneScroll);

        foreach (var sceneData in configuredScenes)
        {
            if (sceneData.entry == null) continue;

            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneData.assetData.path);
            if (scene == null) continue;

            if (_showOnlyEnabled && !sceneData.entry.sceneSwitch) continue;

            DrawSceneRow(sceneData.entry, scene);
        }

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(dbAsset);
        }
    }

    void DrawSceneToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        _showOnlyEnabled = GUILayout.Toggle(_showOnlyEnabled, "Enabled Only", EditorStyles.toolbarButton);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
        {
            RefreshDatabase();
        }

        EditorGUILayout.EndHorizontal();
    }

    void DrawSceneDatabaseSelector(SceneDatabase currentDatabase)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        var selectedDatabase = (SceneDatabase)EditorGUILayout.ObjectField(
            "Database",
            currentDatabase,
            typeof(SceneDatabase),
            false
        );

        if (EditorGUI.EndChangeCheck())
        {
            SceneDatabaseUtility.SetDatabase(selectedDatabase);
            GUI.changed = true;
        }

        if (GUILayout.Button("Select", GUILayout.Width(60)))
        {
            Selection.activeObject = currentDatabase;
        }

        if (GUILayout.Button("New", GUILayout.Width(60)))
        {
            CreateNewDatabase();
        }

        EditorGUILayout.EndHorizontal();

        string path = currentDatabase == null
            ? "None"
            : AssetDatabase.GetAssetPath(currentDatabase);

        EditorGUILayout.LabelField("Path", path, EditorStyles.miniLabel);
        GUILayout.Space(4);
    }

    void DrawSceneRow(SceneEntry entry, SceneAsset scene)
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(scene.name, GUILayout.Width(200)))
        {
            Selection.activeObject = scene;
        }

        entry.sceneSwitch = EditorGUILayout.Toggle(entry.sceneSwitch, GUILayout.Width(20));

        if (!entry.sceneSwitch)
        {
            entry.mode = (int)SceneType.Default;
        }

        EditorGUI.BeginDisabledGroup(!entry.sceneSwitch);

        SceneType currentMode = (SceneType)EditorGUILayout.EnumPopup(
            (SceneType)entry.mode,
            GUILayout.Width(100)
        );

        entry.mode = (int)currentMode;

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();
    }

    void RefreshDatabase()
    {
        var db = SceneDatabaseUtility.GetDatabase();
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

    void CreateNewDatabase()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create Scene Database",
            "SceneDatabase",
            "asset",
            "Choose location for Scene Database"
        );

        if (string.IsNullOrEmpty(path)) return;

        var database = SceneDatabaseUtility.CreateDatabaseAt(path);
        Selection.activeObject = database;
        RefreshDatabase();
    }

    // =========================
    // Favorite Presets
    // =========================
    void DrawFavoritePresets()
    {
        var all = FavoriteSceneSettingsProvider.FindAll();

        EditorGUILayout.BeginHorizontal();

        int index = System.Array.IndexOf(all, _current);

        int newIndex = EditorGUILayout.Popup(
            "Preset",
            Mathf.Max(index, 0),
            System.Array.ConvertAll(all, a => a.name)
        );

        if (newIndex != index && newIndex >= 0 && newIndex < all.Length)
        {
            _current = all[newIndex];
            FavoriteSceneSettingsProvider.SetCurrent(_current);
            Bind();
        }

        if (GUILayout.Button("Select", GUILayout.Width(60)))
        {
            if (_current != null)
                Selection.activeObject = _current;
        }

        if (GUILayout.Button("New", GUILayout.Width(60)))
        {
            CreateNewPreset();
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (_current == null)
        {
            EditorGUILayout.HelpBox("No preset selected.", MessageType.Info);
            return;
        }

        _favoriteScroll = EditorGUILayout.BeginScrollView(_favoriteScroll);

        _serializedObject.Update();
        EditorGUILayout.PropertyField(_favoriteScenesProp, true);
        _serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_current);
        }
    }

    void CreateNewPreset()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create FavoriteSceneSettings",
            "FavoriteSceneSettings",
            "asset",
            ""
        );

        if (string.IsNullOrEmpty(path)) return;

        var asset = CreateInstance<FavoriteSceneSettings>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        _current = asset;
        FavoriteSceneSettingsProvider.SetCurrent(_current);
        Bind();
    }
}
#endif

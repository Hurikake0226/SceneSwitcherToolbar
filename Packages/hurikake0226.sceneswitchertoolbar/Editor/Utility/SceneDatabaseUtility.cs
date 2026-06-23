#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SceneDatabaseUtility
{
    private const string PrefKey = "SceneSwitcher_DB_PATH";
    private const string DefaultPath = "Assets/SceneSwitcher/SceneDatabase.asset";

    public static SceneDatabase GetDatabase()
    {
        string path = ResolveDatabasePath(promptIfMissing: true);
        return LoadOrCreateDatabase(path);
    }

    public static bool TryGetDatabase(out SceneDatabase database)
    {
        string path = ResolveDatabasePath(promptIfMissing: false);
        database = string.IsNullOrEmpty(path)
            ? null
            : AssetDatabase.LoadAssetAtPath<SceneDatabase>(path);

        return database != null;
    }

    public static string GetCurrentDatabasePath()
    {
        var database = GetDatabase();
        return AssetDatabase.GetAssetPath(database);
    }

    public static void SetDatabase(SceneDatabase database)
    {
        if (database == null) return;

        SetDatabasePath(AssetDatabase.GetAssetPath(database));
    }

    public static SceneDatabase CreateDatabaseAt(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        path = NormalizeAssetPath(path);
        var existing = AssetDatabase.LoadAssetAtPath<SceneDatabase>(path);
        if (existing != null)
        {
            SetDatabasePath(path);
            return existing;
        }

        var database = LoadOrCreateDatabase(path);
        SetDatabasePath(path);
        return database;
    }

    public static void SetDatabasePath(string newPath)
    {
        if (string.IsNullOrEmpty(newPath)) return;

        newPath = NormalizeAssetPath(newPath);
        if (!newPath.StartsWith("Assets/") || Path.GetExtension(newPath) != ".asset")
        {
            Debug.LogWarning($"SceneDatabase path must be an .asset under Assets: {newPath}");
            return;
        }

        EditorPrefs.SetString(PrefKey, newPath);
    }

    public static SceneEntry GetOrCreateEntry(SceneAsset scene)
    {
        var database = GetDatabase();

        string path = AssetDatabase.GetAssetPath(scene);
        string guid = AssetDatabase.AssetPathToGUID(path);

        var entry = database.scenes.FirstOrDefault(s => s.guid == guid);
        if (entry == null)
        {
            entry = new SceneEntry { guid = guid };
            database.scenes.Add(entry);
        }

        entry.sceneName = scene.name;
        EditorUtility.SetDirty(database);

        return entry;
    }

    private static string ResolveDatabasePath(bool promptIfMissing)
    {
        string savedPath = NormalizeAssetPath(EditorPrefs.GetString(PrefKey, ""));
        if (AssetDatabase.LoadAssetAtPath<SceneDatabase>(savedPath) != null)
        {
            return savedPath;
        }

        string foundPath = FindFirstDatabasePath();
        if (!string.IsNullOrEmpty(foundPath))
        {
            EditorPrefs.SetString(PrefKey, foundPath);
            return foundPath;
        }

        if (!promptIfMissing) return null;

        string selectedPath = EditorUtility.SaveFilePanelInProject(
            "Create Scene Database",
            "SceneDatabase",
            "asset",
            "Choose location for Scene Database"
        );

        string path = string.IsNullOrEmpty(selectedPath) ? DefaultPath : selectedPath;
        path = NormalizeAssetPath(path);
        EditorPrefs.SetString(PrefKey, path);
        return path;
    }

    private static SceneDatabase LoadOrCreateDatabase(string path)
    {
        var database = AssetDatabase.LoadAssetAtPath<SceneDatabase>(path);
        if (database != null) return database;

        CreateFolderRecursive(Path.GetDirectoryName(path));

        database = ScriptableObject.CreateInstance<SceneDatabase>();
        AssetDatabase.CreateAsset(database, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return database;
    }

    private static string FindFirstDatabasePath()
    {
        return AssetDatabase.FindAssets("t:SceneDatabase")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => AssetDatabase.LoadAssetAtPath<SceneDatabase>(path) != null)
            .OrderBy(path => path)
            .FirstOrDefault();
    }

    private static string NormalizeAssetPath(string path)
    {
        return string.IsNullOrEmpty(path)
            ? path
            : path.Replace('\\', '/');
    }

    private static void CreateFolderRecursive(string folderPath)
    {
        folderPath = NormalizeAssetPath(folderPath);

        if (string.IsNullOrEmpty(folderPath) || folderPath == "Assets") return;
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        string parent = NormalizeAssetPath(Path.GetDirectoryName(folderPath));
        if (!AssetDatabase.IsValidFolder(parent))
        {
            CreateFolderRecursive(parent);
        }

        AssetDatabase.CreateFolder(parent, Path.GetFileName(folderPath));
    }
}
#endif

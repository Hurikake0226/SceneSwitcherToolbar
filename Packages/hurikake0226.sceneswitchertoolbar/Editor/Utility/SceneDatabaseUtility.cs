using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public static class SceneDatabaseUtility
{
    const string PREF_KEY = "SceneSwitcher_DB_PATH";
    const string DEFAULT_PATH = "Assets/SceneSwitcher/SceneDatabase.asset";

    public static SceneDatabase GetDatabase()
    {
        string path = GetDatabasePath();

        var db = AssetDatabase.LoadAssetAtPath<SceneDatabase>(path);

        if (db == null)
        {
            // 🔥 フォルダ生成
            string folder = Path.GetDirectoryName(path);
            CreateFolderRecursive(folder);

            db = ScriptableObject.CreateInstance<SceneDatabase>();
            AssetDatabase.CreateAsset(db, path);
            AssetDatabase.SaveAssets();
        }

        return db;
    }

    static string GetDatabasePath()
    {
        string path = EditorPrefs.GetString(PREF_KEY, "");

        if (string.IsNullOrEmpty(path))
        {
            // 🔥 初回選択
            path = EditorUtility.SaveFilePanelInProject(
                "Create Scene Database",
                "SceneDatabase",
                "asset",
                "Choose location for Scene Database"
            );

            if (string.IsNullOrEmpty(path))
            {
                // キャンセル時 fallback
                path = DEFAULT_PATH;
            }

            EditorPrefs.SetString(PREF_KEY, path);
        }

        return path;
    }

    public static void SetDatabasePath(string newPath)
    {
        if (!string.IsNullOrEmpty(newPath))
        {
            EditorPrefs.SetString(PREF_KEY, newPath);
        }
    }

    static void CreateFolderRecursive(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        string parent = Path.GetDirectoryName(folderPath);
        string newFolder = Path.GetFileName(folderPath);

        if (!AssetDatabase.IsValidFolder(parent))
        {
            CreateFolderRecursive(parent);
        }

        AssetDatabase.CreateFolder(parent, newFolder);
    }

    public static SceneEntry GetOrCreateEntry(SceneAsset scene)
    {
        var db = GetDatabase();

        string path = AssetDatabase.GetAssetPath(scene);
        string guid = AssetDatabase.AssetPathToGUID(path);

        var entry = db.scenes.FirstOrDefault(s => s.guid == guid);

        if (entry == null)
        {
            entry = new SceneEntry { guid = guid };
            db.scenes.Add(entry);
        }

        entry.sceneName = scene.name;

        EditorUtility.SetDirty(db);

        return entry;
    }
}
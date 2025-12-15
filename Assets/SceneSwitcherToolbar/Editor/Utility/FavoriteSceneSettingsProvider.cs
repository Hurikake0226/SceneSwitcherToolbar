#if UNITY_EDITOR
using UnityEditor;
using System.Linq;

public static class FavoriteSceneSettingsProvider
{
    private const string LastUsedKey = "SceneSwitcher.LastFavoriteSettings";

    public static FavoriteSceneSettings LoadCurrent()
    {
        string path = EditorUserSettings.GetConfigValue(LastUsedKey);
        if (!string.IsNullOrEmpty(path))
        {
            var asset = AssetDatabase.LoadAssetAtPath<FavoriteSceneSettings>(path);
            if (asset != null)
                return asset;
        }

        // フォールバック：最初に見つかったもの
        return FindAll().FirstOrDefault();
    }

    public static void SetCurrent(FavoriteSceneSettings settings)
    {
        if (settings == null)
            return;

        string path = AssetDatabase.GetAssetPath(settings);
        EditorUserSettings.SetConfigValue(LastUsedKey, path);
    }

    public static FavoriteSceneSettings[] FindAll()
    {
        return AssetDatabase
            .FindAssets("t:FavoriteSceneSettings")
            .Select(guid =>
                AssetDatabase.LoadAssetAtPath<FavoriteSceneSettings>(
                    AssetDatabase.GUIDToAssetPath(guid)
                ))
            .Where(a => a != null)
            .ToArray();
    }
}
#endif
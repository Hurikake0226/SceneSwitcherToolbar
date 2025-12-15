#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public static class SceneSwitcherUtility
{
    public const string SaveKey = "FavoriteSettingWindow.FavoriteScenes";

    public static SceneAssetData[] GetAllProjectScenes()
    {
        return AssetDatabase.FindAssets("t:Scene")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(IsValidScenePath)
            .Select(path =>
                new SceneAssetData(
                    Path.GetFileNameWithoutExtension(path),
                    path
                )
            )
            .OrderBy(scene => scene.name)
            .ToArray();
    }

    public static bool IsSceneLoaded(string path)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).path == path)
                return true;
        }
        return false;
    }

    private static bool IsValidScenePath(string path)
    {
        return File.Exists(path)
            && Path.GetExtension(path) == ".unity"
            && path.StartsWith("Assets/")
            && !path.Contains("Addressable")
            && !path.Contains("_Recovery")
            && !path.Contains("SerializationTests");
    }
}
#endif
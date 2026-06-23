#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace SceneSwitcherToolbar
{
    public class ConfiguredSceneData
    {
        public SceneAssetData assetData;
        public SceneEntry entry;

        public bool IsSwitchEnabled => entry != null && entry.sceneSwitch;
    }

    public static class SceneSwitcherUtility
    {
        public static SceneAssetData[] GetAllProjectScenes()
        {
            return AssetDatabase.FindAssets("t:Scene")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(IsValidScenePath)
                .Select(path => new SceneAssetData(Path.GetFileNameWithoutExtension(path), path))
                .OrderBy(scene => scene.name)
                .ToArray();
        }

        public static List<ConfiguredSceneData> GetConfiguredScenes()
        {
            SceneAssetData[] scenes = GetAllProjectScenes();
            SceneDatabase database = SceneDatabaseUtility.GetDatabase();

            var entriesByGuid = database.scenes
                .Where(entry => !string.IsNullOrEmpty(entry.guid))
                .GroupBy(entry => entry.guid)
                .ToDictionary(group => group.Key, group => group.First());

            var result = new List<ConfiguredSceneData>();

            foreach (var scene in scenes)
            {
                string guid = AssetDatabase.AssetPathToGUID(scene.path);
                if (!entriesByGuid.TryGetValue(guid, out var entry))
                {
                    entry = new SceneEntry { guid = guid };
                    database.scenes.Add(entry);
                    entriesByGuid.Add(guid, entry);
                }

                entry.sceneName = scene.name;

                result.Add(new ConfiguredSceneData
                {
                    assetData = scene,
                    entry = entry
                });
            }

            EditorUtility.SetDirty(database);
            return result;
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
}
#endif

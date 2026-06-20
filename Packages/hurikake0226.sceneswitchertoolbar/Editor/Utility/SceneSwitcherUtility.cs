#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Collections.Generic; // Listを使うために追加

namespace SceneSwitcherToolbar
{
    /// <summary>
    /// 💡 シーン情報(SceneAssetData)と設定項目(SceneEntry)をまとめるデータクラス
    /// </summary>
    public class ConfiguredSceneData
    {
        public SceneAssetData assetData;
        public SceneEntry entry; // インスペクタで一度も触っていない場合は null になります

        // ツールバーに表示すべきかどうか（ONになっているか）を簡単に判定できるプロパティ
        public bool IsSwitchEnabled => entry != null && entry.sceneSwitch;
    }

    public static class SceneSwitcherUtility
    {
        /// <summary>
        /// 全シーンを取得する（元のメソッド）
        /// </summary>
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

        /// <summary>
        /// 🔥 全シーンを取得し、SceneDatabaseの設定項目を紐付けて返す
        /// </summary>
        public static List<ConfiguredSceneData> GetConfiguredScenes()
        {
            // 1. プロジェクト内の全シーンを取得
            SceneAssetData[] scenes = GetAllProjectScenes();

            // 2. データベースを取得
            SceneDatabase db = SceneDatabaseUtility.GetDatabase();

            List<ConfiguredSceneData> resultList = new List<ConfiguredSceneData>();

            foreach (var scene in scenes)
            {
                // ※ SceneAssetDataに "path" というプロパティ/フィールドがある前提です
                // シーンのパスからGUIDを取得する
                string guid = AssetDatabase.AssetPathToGUID(scene.path);

                // データベースからGUIDが一致するエントリーを検索する
                SceneEntry matchedEntry = db.scenes.FirstOrDefault(s => s.guid == guid);

                // ペアにしてリストに追加
                resultList.Add(new ConfiguredSceneData
                {
                    assetData = scene,
                    entry = matchedEntry
                });
            }

            return resultList;
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
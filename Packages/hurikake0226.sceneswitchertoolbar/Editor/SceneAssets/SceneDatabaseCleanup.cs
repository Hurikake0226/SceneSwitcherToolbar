using UnityEditor;
using System.Linq;

public class SceneDatabaseCleanup : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] imported,
        string[] deleted,
        string[] moved,
        string[] movedFrom)
    {
        var db = SceneDatabaseUtility.GetDatabase();

        // 存在するSceneのGUID一覧
        var validGuids = AssetDatabase.FindAssets("t:Scene")
            .Select(g => g)
            .ToHashSet();

        // 不要エントリ削除
        int before = db.scenes.Count;

        db.scenes.RemoveAll(e => !validGuids.Contains(e.guid));

        if (db.scenes.Count != before)
        {
            EditorUtility.SetDirty(db);
        }
    }
}
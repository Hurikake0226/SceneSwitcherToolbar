#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

public class SceneDatabaseCleanup : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] imported,
        string[] deleted,
        string[] moved,
        string[] movedFrom)
    {
        if (!SceneDatabaseUtility.TryGetDatabase(out var database)) return;

        var validGuids = AssetDatabase.FindAssets("t:Scene")
            .Select(guid => guid)
            .ToHashSet();

        int before = database.scenes.Count;
        database.scenes.RemoveAll(entry => !validGuids.Contains(entry.guid));

        if (database.scenes.Count != before)
        {
            EditorUtility.SetDirty(database);
        }
    }
}
#endif

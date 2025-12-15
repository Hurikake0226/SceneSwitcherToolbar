#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SceneSwitcherToolbar.FavoriteSettingWindow
{
    public class FavoriteSettingWindow : EditorWindow
    {
        [SerializeField] FavoriteSceneSettings _current;

        private SerializedObject _serializedObject;
        private SerializedProperty _favoriteScenesProp;

        [MenuItem("Window/Favorite Setting")]
        public static void Open()
        {
            GetWindow<FavoriteSettingWindow>("Favorite Setting");
        }

        private void OnEnable()
        {
            _current = FavoriteSceneSettingsProvider.LoadCurrent();
            Bind();
        }

        private void Bind()
        {
            if (_current == null)
                return;

            _serializedObject = new SerializedObject(_current);
            _favoriteScenesProp =
                _serializedObject.FindProperty("favoriteScenes");
        }

        private void OnGUI()
        {
            DrawPresetSelector();

            if (_current == null)
            {
                EditorGUILayout.HelpBox("No FavoriteSceneSettings asset found.", MessageType.Info);
                return;
            }

            _serializedObject.Update();
            EditorGUILayout.PropertyField(_favoriteScenesProp, true);
            _serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_current);
            }
        }

        private void DrawPresetSelector()
        {
            var all = FavoriteSceneSettingsProvider.FindAll();
            int index = System.Array.IndexOf(all, _current);

            EditorGUILayout.BeginHorizontal();

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
                Selection.activeObject = _current;
            }

            if (GUILayout.Button("New", GUILayout.Width(60)))
            {
                CreateNewPreset();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void CreateNewPreset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create FavoriteSceneSettings",
                "FavoriteSceneSettings",
                "asset",
                ""
            );

            if (string.IsNullOrEmpty(path))
                return;

            var asset = CreateInstance<FavoriteSceneSettings>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            _current = asset;
            FavoriteSceneSettingsProvider.SetCurrent(_current);
            Bind();
        }
    }
}

[System.Serializable]
public class FavoriteSceneSettingsSave
{
    public string favoriteName;
    public SceneAsset activeScene;
    public SceneAsset[] additiveScene;
}
#endif
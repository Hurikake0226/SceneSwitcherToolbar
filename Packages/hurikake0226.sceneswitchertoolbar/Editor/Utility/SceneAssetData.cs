#if UNITY_EDITOR

namespace SceneSwitcherToolbar
{
    public readonly struct SceneAssetData
    {
        public string name { get; }
        public string path { get; }

        public SceneAssetData(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
    }
}
#endif
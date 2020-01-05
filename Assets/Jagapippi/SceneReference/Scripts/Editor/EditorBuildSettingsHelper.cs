#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

namespace Jagapippi.SceneReference
{
    public static class EditorBuildSettingsHelper
    {
        public static void OpenWindow() => EditorApplication.ExecuteMenuItem("File/Build Settings...");
        public static bool Contains(string path) => EditorBuildSettings.scenes.Any(scene => scene.path == path);

        public static void AddScene(string path, bool enabled = true)
        {
            InsertScene(EditorBuildSettings.scenes.Length, path, enabled);
        }

        public static void InsertScene(int index, string path, bool enabled = true)
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            scenes.Insert(index, new EditorBuildSettingsScene(path, enabled));
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        public static void RemoveScene(string path)
        {
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Where(scene => scene.path != path).ToArray();
        }
    }
}
#endif
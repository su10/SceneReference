#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jagapippi.SceneReference
{
    public static class ISceneReferenceExtensions
    {
#if UNITY_EDITOR
        public static SceneAsset FindSceneAsset(this ISceneReference self)
        {
            return AssetDatabase.LoadAssetAtPath<SceneAsset>(self.path);
        }
#endif
        public static void LoadScene(this ISceneReference self, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(self.path, mode);
        }

        public static Scene LoadScene(this ISceneReference self, LoadSceneParameters parameters)
        {
            return SceneManager.LoadScene(self.path, parameters);
        }

        public static AsyncOperation LoadSceneAsync(this ISceneReference self, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return SceneManager.LoadSceneAsync(self.path, mode);
        }

        public static AsyncOperation LoadSceneAsync(this ISceneReference self, LoadSceneParameters parameters)
        {
            return SceneManager.LoadSceneAsync(self.path, parameters);
        }

        public static AsyncOperation UnloadSceneAsync(this ISceneReference self, UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            return SceneManager.UnloadSceneAsync(self.path, options);
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jagapippi.SceneReference
{
    public class SceneReferenceAsset : ScriptableObject, ISceneReference
    {
#if UNITY_EDITOR

        #region static

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            foreach (var instance in FindAll())
            {
                instance.Subscribe();
            }
        }

        public static IEnumerable<SceneReferenceAsset> FindAll()
        {
            return AssetDatabase.FindAssets($"t: {nameof(SceneReferenceAsset)}")
                .Select(GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SceneReferenceAsset>);
        }

        public static SceneReferenceAsset FindByScenePath(string path)
        {
            return FindAll().FirstOrDefault(asset => asset.path == path);
        }

        public static SceneReferenceAsset CreateByScenePath(string scenePath)
        {
            var sceneGUID = AssetPathToGUID(scenePath);

            if (string.IsNullOrEmpty(sceneGUID)) return null;

            var instance = CreateInstance<SceneReferenceAsset>();
            instance.guid = sceneGUID;
            instance.Subscribe();

            var assetPath = Regex.Replace(scenePath, @".unity$", ".asset");
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AssetDatabase.CreateAsset(instance, uniquePath);

            return instance;
        }

        public static SceneReferenceAsset FindOrCreate(string scenePath)
        {
            var asset = FindByScenePath(scenePath);
            if (asset == null) asset = CreateByScenePath(scenePath);
            return asset;
        }

        private static string GUIDToAssetPath(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            return File.Exists(path) ? path : "";
        }

        private static string AssetPathToGUID(string path)
        {
            return File.Exists(path) ? AssetDatabase.AssetPathToGUID(path) : "";
        }

        private static SceneReference ScenePathToSceneReference(string path)
        {
            var index = 0;

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == path) return new SceneReference(path, scene.enabled, (scene.enabled ? index : -1));
                if (scene.enabled) index++;
            }

            return null;
        }

        #endregion

        [SerializeField, HideInInspector] private string _guid = "";
        public string guid
        {
            get { return _guid; }
            private set { _guid = value; }
        }
#endif
        [SerializeField, ReadOnly] private SceneReference _reference = null;

        public SceneReference reference => _reference;
        public string sceneName => this.reference.name;

        #region ISceneReference

        string ISceneReference.name => this.reference.name;
        public string path => this.reference.path;
        public int buildIndex => this.reference.buildIndex;
        public bool enabled => this.reference.enabled;

        #endregion

#if UNITY_EDITOR
        void OnDestroy() => this.Unsubscribe();

        private void Subscribe()
        {
            this.Unsubscribe();
            this.Update();

            EditorBuildSettings.sceneListChanged += this.Update;
            SceneAssetEvent.onWillMove += this.OnWillMoveScene;
            SceneAssetEvent.onWillDelete += this.OnWillDeleteScene;
        }

        private void Unsubscribe()
        {
            EditorBuildSettings.sceneListChanged -= this.Update;
            SceneAssetEvent.onWillMove -= this.OnWillMoveScene;
            SceneAssetEvent.onWillDelete -= this.OnWillDeleteScene;
        }

        public void Update()
        {
            if (this == null) return;

            var scenePath = GUIDToAssetPath(this.guid);

            if (string.IsNullOrEmpty(scenePath))
            {
                this.Delete();
            }
            else
            {
                _reference = ScenePathToSceneReference(scenePath) ?? new SceneReference(scenePath);
                EditorUtility.SetDirty(this);
            }
        }

        private void OnWillMoveScene(string sourcePath, string destinationPath)
        {
            if (sourcePath == this.path) EditorApplication.delayCall += this.Update;
        }

        private void OnWillDeleteScene(string assetPath)
        {
            if (assetPath == this.path) this.Delete();
        }

        private static bool allowToDelete = false;

        private void Delete()
        {
            var path = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(path) || File.Exists(path) == false) return;

            allowToDelete = true;
            AssetDatabase.DeleteAsset(path);
            allowToDelete = false;
        }

        private class AssetModificationProcessor : UnityEditor.AssetModificationProcessor
        {
            static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
            {
                if (assetPath.EndsWith(".asset") == false) return AssetDeleteResult.DidNotDelete;

                var asset = AssetDatabase.LoadAssetAtPath<SceneReferenceAsset>(assetPath);

                if (asset == null || allowToDelete)
                {
                    return AssetDeleteResult.DidNotDelete;
                }

                return AssetDeleteResult.FailedDelete;
            }
        }

#endif
    }
}
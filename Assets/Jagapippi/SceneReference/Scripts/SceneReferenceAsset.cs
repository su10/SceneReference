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
            EditorApplication.playModeStateChanged += (mode) =>
            {
                switch (mode)
                {
                    case PlayModeStateChange.EnteredEditMode:
                    case PlayModeStateChange.EnteredPlayMode:
                        SubscribeAll();
                        break;
                }
            };

            if (EditorApplication.isPlayingOrWillChangePlaymode == false)
            {
                SubscribeAll();
            }
        }

        public static void SubscribeAll()
        {
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

        public static SceneReferenceAsset FindOrCreate(SceneAsset sceneAsset)
        {
            var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            if (string.IsNullOrEmpty(scenePath)) return null;
            return FindOrCreate(scenePath);
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
                var newReference = ScenePathToSceneReference(scenePath) ?? new SceneReference(scenePath);

                if (_reference == null || _reference.Equals(newReference) == false)
                {
                    _reference = newReference;
                    EditorUtility.SetDirty(this);
                }
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

        private void Delete()
        {
            if (this) AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
        }

        private class AssetPostprocessor : UnityEditor.AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                List<SceneReferenceAsset> allAssets = null;

                foreach (var path in importedAssets)
                {
                    if (path.EndsWith(".asset") == false) continue;

                    var importedAsset = AssetDatabase.LoadAssetAtPath<SceneReferenceAsset>(path);
                    if (importedAsset == null) continue;

                    if (allAssets == null) allAssets = FindAll().ToList();
                    if (allAssets.All(asset => asset == importedAsset || asset.path != importedAsset.path)) continue;

                    Debug.LogWarning($"{nameof(SceneReferenceAsset)} already exists for '{importedAsset.path}'.");
                    importedAsset.Delete();
                }
            }
        }
#endif
    }
}
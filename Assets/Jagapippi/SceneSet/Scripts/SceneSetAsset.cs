using System.Collections;
using System.Collections.Generic;
using Jagapippi.SceneReference;
using UnityEngine;

namespace Jagapippi.SceneSet
{
    public class SceneSetAsset : ScriptableObject, ISceneSet
    {
        [SerializeField] private List<SceneReferenceAsset> _sceneReferenceAssets = new List<SceneReferenceAsset>();

        #region ISceneSet

        public IEnumerator<ISceneReference> GetEnumerator() => _sceneReferenceAssets.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _sceneReferenceAssets.GetEnumerator();
        public int Count => _sceneReferenceAssets.Count;
        public ISceneReference this[int index] => _sceneReferenceAssets[index];

        #endregion

        public void Add(SceneReferenceAsset sceneReferenceAsset)
        {
            if (_sceneReferenceAssets.Contains(sceneReferenceAsset) == false)
            {
                _sceneReferenceAssets.Add(sceneReferenceAsset);
            }
        }

        public void AddAll(IEnumerable<SceneReferenceAsset> sceneReferenceAssets)
        {
            foreach (var asset in sceneReferenceAssets)
            {
                if (asset) this.Add(asset);
            }
        }

        public void Remove(SceneReferenceAsset sceneReferenceAsset)
        {
            _sceneReferenceAssets.RemoveAll(asset => asset == sceneReferenceAsset);
        }

        public void Remove(string scenePath)
        {
            _sceneReferenceAssets.RemoveAll(asset => asset.path == scenePath);
        }

        public void Clear()
        {
            _sceneReferenceAssets.Clear();
        }

#if UNITY_EDITOR
        public void Add(string scenePath)
        {
            this.Add(SceneReferenceAsset.FindOrCreate(scenePath));
        }

        public void AddAll(IEnumerable<string> scenePaths)
        {
            foreach (var path in scenePaths)
            {
                this.Add(path);
            }
        }
#endif
    }
}
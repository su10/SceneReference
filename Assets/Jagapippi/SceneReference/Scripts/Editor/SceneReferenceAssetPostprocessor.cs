using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Jagapippi.SceneReference
{
    public class SceneReferenceAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            List<SceneReferenceAsset> allAssets = null;

            foreach (var path in importedAssets)
            {
                if (path.EndsWith(".asset") == false) continue;

                var importedAsset = AssetDatabase.LoadAssetAtPath<SceneReferenceAsset>(path);
                if (importedAsset == null) continue;

                if (allAssets == null) allAssets = SceneReferenceAsset.FindAll().ToList();
                if (allAssets.All(asset => asset == importedAsset || asset.path != importedAsset.path)) continue;

                Debug.LogWarning($"{nameof(SceneReferenceAsset)} already exists for '{importedAsset.path}'.");
                importedAsset.Delete();
            }
        }
    }
}
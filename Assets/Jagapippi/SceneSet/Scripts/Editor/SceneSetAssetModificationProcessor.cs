using Jagapippi.SceneReference;
using UnityEditor;

namespace Jagapippi.SceneSet
{
    public class SceneSetAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (var path in paths)
            {
                if (path.EndsWith(".asset") == false) continue;
                var sceneSetAsset = AssetDatabase.LoadAssetAtPath<SceneSetAsset>(path);
                if (sceneSetAsset == null) continue;

                sceneSetAsset.Remove((SceneReferenceAsset) null);
            }

            return paths;
        }
    }
}
using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

namespace Jagapippi.SceneReference
{
    public static class SceneReferenceAssetMenuItem
    {
        private const string MenuItemPath = "Assets/Create/SceneReference";

        [MenuItem(MenuItemPath, true)]
        static bool MenuItemValidateFunction()
        {
            return Selection.objects.Select(AssetDatabase.GetAssetPath).Any(path => path.EndsWith(".unity"));
        }

        [MenuItem(MenuItemPath, false, 201)]
        static void MenuItem()
        {
            var scenePaths = Selection.objects.Select(AssetDatabase.GetAssetPath).Where(path => path.EndsWith(".unity"));

            foreach (var scenePath in scenePaths)
            {
                var asset = SceneReferenceAsset.FindByScenePath(scenePath);

                if (asset)
                {
                    Debug.LogWarning($"{nameof(SceneReferenceAsset)} already exists for '{asset.path}'.");
                }
                else
                {
                    asset = SceneReferenceAsset.CreateByScenePath(scenePath);
                }

                Selection.activeObject = asset;
            }
        }
    }
}
#endif
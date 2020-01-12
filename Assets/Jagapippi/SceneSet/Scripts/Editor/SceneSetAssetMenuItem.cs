using System.Collections.Generic;
using System.IO;
using Jagapippi.SceneReference;
using UnityEditor;
using UnityEngine;

namespace Jagapippi.SceneSet
{
    public static class SceneSetAssetMenuItem
    {
        private const string MenuItemPath = "Assets/Create/SceneSet";
        private static readonly string DefaultAssetName = "SceneSet.asset";
        private static readonly string DefaultAssetPath = $"Assets/{DefaultAssetName}";

        [MenuItem(MenuItemPath, true)]
        static bool MenuItemValidateFunction() => true;

        [MenuItem(MenuItemPath, false, 201)]
        static void MenuItem()
        {
            var instance = ScriptableObject.CreateInstance<SceneSetAsset>();
            {
                var scenePaths = new List<string>();

                if (Selection.activeObject)
                {
                    foreach (var obj in Selection.objects)
                    {
                        switch (obj)
                        {
                            case SceneAsset sceneAsset:
                                scenePaths.Add(AssetDatabase.GetAssetPath(sceneAsset));
                                break;
                            case ISceneReference sceneReference:
                                scenePaths.Add(sceneReference.path);
                                break;
                        }
                    }
                }

                instance.AddAll(scenePaths);
            }

            var assetPath = DefaultAssetPath;

            if (Selection.activeObject)
            {
                var selectedAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

                if (File.GetAttributes(selectedAssetPath).HasFlag(FileAttributes.Directory))
                {
                    assetPath = $"{selectedAssetPath}/{DefaultAssetName}";
                }
                else
                {
                    assetPath = $"{Path.GetDirectoryName(selectedAssetPath)}/{DefaultAssetName}";
                }
            }

            ProjectWindowUtil.CreateAsset(instance, assetPath);
        }
    }
}
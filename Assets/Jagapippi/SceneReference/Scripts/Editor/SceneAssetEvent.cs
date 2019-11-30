#if UNITY_EDITOR
using System;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Jagapippi.SceneReference
{
    public class SceneAssetEvent : UnityEditor.AssetModificationProcessor
    {
        public static event Action<string> onWillCreate;
        public static event Action<string, string> onWillMove;
        public static event Action<string> onWillDelete;

        static void OnWillCreateAsset(string path)
        {
            if (path.EndsWith(".unity.meta")) onWillCreate?.Invoke(Regex.Replace(path, @".meta$", ""));
        }

        static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            if (sourcePath.EndsWith(".unity")) onWillMove?.Invoke(sourcePath, destinationPath);
            return AssetMoveResult.DidNotMove;
        }

        static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
        {
            if (assetPath.EndsWith(".unity")) onWillDelete?.Invoke(assetPath);
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
#endif
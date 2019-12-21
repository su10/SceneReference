#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Jagapippi.SceneReference
{
    [CustomPropertyDrawer(typeof(SceneReferenceAsset))]
    public class SceneReferenceAssetDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sceneReferenceAsset = (SceneReferenceAsset) property.objectReferenceValue;
            Object obj = (sceneReferenceAsset) ? sceneReferenceAsset.FindSceneAsset() : null;

            var sceneAsset = (SceneAsset) EditorGUI.ObjectField(position, label, obj, typeof(SceneAsset), false);
            var scenePath = AssetDatabase.GetAssetPath(sceneAsset);

            if (sceneAsset == null || string.IsNullOrEmpty(scenePath))
            {
                property.objectReferenceValue = null;
            }
            else if (sceneReferenceAsset == null || sceneReferenceAsset.path != scenePath)
            {
                property.objectReferenceValue = FindOrCreateSceneReferenceAsset(scenePath);
            }
        }

        private static SceneReferenceAsset FindOrCreateSceneReferenceAsset(string scenePath)
        {
            var asset = SceneReferenceAsset.FindByScenePath(scenePath);
            if (asset == null) asset = SceneReferenceAsset.CreateByScenePath(scenePath);
            return asset;
        }
    }
}
#endif
#if UNITY_EDITOR && !ODIN_INSPECTOR
using UnityEditor;
using UnityEngine;

namespace Jagapippi.SceneReference
{
    [CustomPropertyDrawer(typeof(SceneReferenceAsset), false)]
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
                property.objectReferenceValue = SceneReferenceAsset.FindOrCreate(scenePath);
            }
        }
    }
}
#endif
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Jagapippi.SceneReference
{
    public class SceneReferenceAssetDrawerWithOdin : OdinValueDrawer<SceneReferenceAsset>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect();
            if (label != null) rect = EditorGUI.PrefixLabel(rect, label);

            var sceneReferenceAsset = this.ValueEntry.SmartValue;
            Object obj = (sceneReferenceAsset) ? sceneReferenceAsset.FindSceneAsset() : null;

            var sceneAsset = (SceneAsset) SirenixEditorFields.UnityObjectField(rect, obj, typeof(SceneAsset), false);
            var scenePath = AssetDatabase.GetAssetPath(sceneAsset);

            if (sceneAsset == null || string.IsNullOrEmpty(scenePath))
            {
                this.ValueEntry.SmartValue = null;
            }
            else if (sceneReferenceAsset == null || sceneReferenceAsset.path != scenePath)
            {
                this.ValueEntry.SmartValue = SceneReferenceAsset.FindOrCreate(scenePath);
            }
        }
    }
}
#endif
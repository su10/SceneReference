using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

namespace Jagapippi.SceneReference
{
    [CustomEditor(typeof(SceneSetAsset))]
    public class SceneSetAssetEditor : Editor
    {
        private static readonly string PropertyName = "_sceneReferenceAssets";
        private SerializedProperty _property;
        private ReorderableList _reorderableList;

        void OnEnable()
        {
            _property = this.serializedObject.FindProperty(PropertyName);
            _reorderableList = this.CreateReorderableList(_property);
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            _reorderableList.DoLayoutList();
            this.serializedObject.ApplyModifiedProperties();
        }

        void OnDisable()
        {
            _property = null;
            _reorderableList = null;
        }

        private ReorderableList CreateReorderableList(SerializedProperty property)
        {
            var reorderableList = new ReorderableList(
                this.serializedObject,
                property,
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true
            );

            reorderableList.drawHeaderCallback += (rect) => EditorGUI.LabelField(rect, "Scenes");
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 1.5f;

                var elementProperty = property.GetArrayElementAtIndex(index);
                var sceneReferenceAsset = (SceneReferenceAsset) elementProperty.objectReferenceValue;
                var currentSceneAsset = sceneReferenceAsset ? sceneReferenceAsset.FindSceneAsset() : null;
                var selectedSceneAsset = (SceneAsset) EditorGUI.ObjectField(
                    rect,
                    new GUIContent(),
                    currentSceneAsset,
                    typeof(SceneAsset),
                    allowSceneObjects: false
                );

                if (selectedSceneAsset == null)
                {
                    elementProperty.objectReferenceValue = null;
                }
                else if (currentSceneAsset != selectedSceneAsset)
                {
                    elementProperty.objectReferenceValue = SceneReferenceAsset.FindOrCreate(selectedSceneAsset);
                }
            };

            return reorderableList;
        }
    }
}
#endif
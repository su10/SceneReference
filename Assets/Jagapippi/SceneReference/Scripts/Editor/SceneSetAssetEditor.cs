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
        private int _lastElementControlID = 0;

        void OnEnable()
        {
            _property = this.serializedObject.FindProperty(PropertyName);
            _reorderableList = this.CreateReorderableList(_property);
            _lastElementControlID = EditorGUIUtility.GetControlID(FocusType.Passive);
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            this.HandleObjectPicker(_lastElementControlID);
            _reorderableList.DoLayoutList();
            this.serializedObject.ApplyModifiedProperties();
        }

        void OnDisable()
        {
            RemoveNull(_reorderableList);

            if (this.serializedObject.targetObject)
            {
                this.serializedObject.ApplyModifiedProperties();
            }

            _property = null;
            _reorderableList = null;
            _lastElementControlID = 0;
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
            reorderableList.onAddCallback += (list) =>
            {
                list.serializedProperty.arraySize += 1;
                list.serializedProperty.GetArrayElementAtIndex(list.count - 1).objectReferenceValue = null;
                EditorGUIUtility.ShowObjectPicker<SceneAsset>(null, false, "", _lastElementControlID);
            };
            reorderableList.onRemoveCallback += (list) =>
            {
                var count = list.serializedProperty.arraySize;
                list.serializedProperty.DeleteArrayElementAtIndex(list.index);

                if (count == list.serializedProperty.arraySize)
                {
                    list.serializedProperty.DeleteArrayElementAtIndex(list.index);
                }
            };

            return reorderableList;
        }

        private void HandleObjectPicker(int controlID)
        {
            switch (Event.current.commandName)
            {
                case "ObjectSelectorUpdated":
                    if (EditorGUIUtility.GetObjectPickerControlID() != controlID) return;

                    var selectedScene = (SceneAsset) EditorGUIUtility.GetObjectPickerObject();
                    var elementProperty = _property.GetArrayElementAtIndex(_property.arraySize - 1);
                    if (selectedScene)
                    {
                        elementProperty.objectReferenceValue = SceneReferenceAsset.FindOrCreate(selectedScene);
                    }
                    else
                    {
                        elementProperty.objectReferenceValue = null;
                    }

                    break;
                case "ObjectSelectorClosed":
                    RemoveNull(_reorderableList);
                    break;
            }
        }

        private static void RemoveNull(ReorderableList list)
        {
            for (var i = 0; i < list.count; i++)
            {
                var property = list.serializedProperty;
                var elementProperty = property.GetArrayElementAtIndex(i);
                if (elementProperty.objectReferenceValue) continue;

                property.DeleteArrayElementAtIndex(i);
                i--;
            }
        }
    }
}
#endif
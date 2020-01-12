using Jagapippi.SceneReference;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Jagapippi.SceneSet
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

            EditorBuildSettings.sceneListChanged += this.Repaint;
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
            EditorBuildSettings.sceneListChanged -= this.Repaint;
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
                var elementProperty = property.GetArrayElementAtIndex(index);
                var sceneReferenceAsset = (SceneReferenceAsset) elementProperty.objectReferenceValue;
                var currentSceneAsset = sceneReferenceAsset ? sceneReferenceAsset.FindSceneAsset() : null;
                var objectFieldRect = new Rect(
                    rect.x,
                    rect.y + 1.5f,
                    rect.width - (EditorGUIUtility.singleLineHeight + 2.0f),
                    EditorGUIUtility.singleLineHeight
                );

                var selectedSceneAsset = (SceneAsset) EditorGUI.ObjectField(
                    objectFieldRect,
                    new GUIContent(),
                    currentSceneAsset,
                    typeof(SceneAsset),
                    allowSceneObjects: false
                );

                var sceneStatusRect = new Rect(
                    objectFieldRect.x + objectFieldRect.width + 3.0f,
                    rect.y + 0.75f,
                    EditorGUIUtility.singleLineHeight,
                    EditorGUIUtility.singleLineHeight
                );

                if (selectedSceneAsset == null)
                {
                    elementProperty.objectReferenceValue = null;
                    SceneStatusButton.Show(sceneStatusRect, false);
                }
                else if (currentSceneAsset != selectedSceneAsset)
                {
                    var selectedAsset = SceneReferenceAsset.FindOrCreate(selectedSceneAsset);
                    elementProperty.objectReferenceValue = selectedAsset;
                    SceneStatusButton.Show(sceneStatusRect, selectedAsset.enabled);
                }
                else
                {
                    SceneStatusButton.Show(sceneStatusRect, sceneReferenceAsset.enabled);
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

        private static class SceneStatusButton
        {
            private static readonly Texture EnabledIconTexture;
            private static readonly Texture DisabledIconTexture;
            private static readonly string EnabledTooltipText = "This Scene will be included in your build.";
            private static readonly string DisabledTooltipText = "This Scene will NOT be included in your build.";
            private static readonly GUIStyle Style = new GUIStyle {alignment = TextAnchor.MiddleCenter};

            static SceneStatusButton()
            {
                DisabledIconTexture = EditorGUIUtility.IconContent("winbtn_mac_close").image;
                EnabledIconTexture = EditorGUIUtility.IconContent("winbtn_mac_max").image;
            }

            public static bool Show(Rect rect, bool enabled)
            {
                var buttonTexture = enabled ? EnabledIconTexture : DisabledIconTexture;
                var tooltipText = enabled ? EnabledTooltipText : DisabledTooltipText;
                return GUI.Button(rect, new GUIContent(buttonTexture, tooltipText), Style);
            }
        }
    }
}
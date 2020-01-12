#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jagapippi.SceneReference
{
    [CustomEditor(typeof(SceneReferenceAsset), false)]
    public class SceneReferenceAssetEditor : Editor
    {
        private static readonly string WarningPrefsKey = $"{nameof(SceneReferenceAssetEditor)}/{nameof(WarningPrefsKey)}";

        private static readonly string HelpText =
            "This asset stores information about correspond to the scene and will be deleted simultaneously with the scene." +
            " In many cases, this asset is automatically created in necessary.\n" +
            "\n" +
            "You can delete this asset manually but keep in mind that it may cause 'Missing' or 'NullReferenceException' in SceneSetAsset etc.";

        private const float LabelWidth = 80;

        public override void OnInspectorGUI()
        {
            if (EditorPrefs.GetBool(WarningPrefsKey, false) == false)
            {
                EditorGUILayout.HelpBox(HelpText, MessageType.Info);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Got it!"))
                    {
                        EditorPrefs.SetBool(WarningPrefsKey, true);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            GUI.enabled = false;
            {
                base.OnInspectorGUI();
            }
            GUI.enabled = true;

            var sceneReference = target as ISceneReference;
            var sceneAsset = sceneReference.FindSceneAsset();

            using (new GUILayout.HorizontalScope())
            using (new EditorGUI.DisabledScope(sceneAsset == null))
            {
                GUILayout.Label("Scene", GUILayout.Width(LabelWidth));
                DrawSelectSceneButton(sceneAsset);
                DrawOpenSceneButton(sceneReference);
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Build Settings", GUILayout.Width(LabelWidth));
                DrawAddBuildSettingsButton(sceneAsset, sceneReference);
                DrawRemoveBuildSettingsButton(sceneReference);
                DrawOpenBuildSettingsButton();
            }
        }

        private static void DrawSelectSceneButton(Object sceneAsset)
        {
            if (GUILayout.Button("Select") == false) return;

            EditorGUIUtility.PingObject(sceneAsset);
        }

        private static void DrawOpenSceneButton(ISceneReference sceneReference)
        {
            if (GUILayout.Button("Open") == false) return;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(sceneReference.path);
            }
        }

        private static void DrawAddBuildSettingsButton(Object sceneAsset, ISceneReference sceneReference)
        {
            using (new EditorGUI.DisabledScope(sceneAsset == null || EditorBuildSettingsHelper.Contains(sceneReference.path)))
            {
                if (GUILayout.Button("Add"))
                {
                    EditorBuildSettingsHelper.AddScene(sceneReference.path);
                }
            }
        }

        private static void DrawRemoveBuildSettingsButton(ISceneReference sceneReference)
        {
            using (new EditorGUI.DisabledScope(EditorBuildSettingsHelper.Contains(sceneReference.path) == false))
            {
                if (GUILayout.Button("Remove"))
                {
                    EditorBuildSettingsHelper.RemoveScene(sceneReference.path);
                }
            }
        }

        private static void DrawOpenBuildSettingsButton()
        {
            if (GUILayout.Button("Open"))
            {
                EditorBuildSettingsHelper.OpenWindow();
            }
        }
    }
}
#endif
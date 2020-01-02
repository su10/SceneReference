#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Jagapippi.SceneReference
{
    [CustomEditor(typeof(SceneReferenceAsset))]
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

            base.OnInspectorGUI();

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
                DrawAddBuildSettingsButton(sceneReference, sceneAsset);
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

        private static void DrawAddBuildSettingsButton(ISceneReference sceneReference, Object sceneAsset)
        {
            var scenes = EditorBuildSettings.scenes.ToList();

            using (new EditorGUI.DisabledScope(sceneAsset == null || scenes.Any(scene => scene.path == sceneReference.path)))
            {
                if (GUILayout.Button("Add"))
                {
                    scenes.Add(new EditorBuildSettingsScene(sceneReference.path, true));
                    EditorBuildSettings.scenes = scenes.ToArray();
                }
            }
        }

        private static void DrawRemoveBuildSettingsButton(ISceneReference sceneReference)
        {
            var scenes = EditorBuildSettings.scenes.ToList();

            using (new EditorGUI.DisabledScope(scenes.All(scene => scene.path != sceneReference.path)))
            {
                if (GUILayout.Button("Remove"))
                {
                    EditorBuildSettings.scenes = scenes.Where(scene => scene.path != sceneReference.path).ToArray();
                }
            }
        }

        private static void DrawOpenBuildSettingsButton()
        {
            if (GUILayout.Button("Open"))
            {
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
            }
        }
    }
}
#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;

namespace Jagapippi.SceneReference
{
    public static class SceneReferenceAssetOnOpen
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            // TODO: 再生中の場合の対応
            if (EditorApplication.isPlayingOrWillChangePlaymode) return true;

            var sceneReferenceAsset = EditorUtility.InstanceIDToObject(instanceID) as SceneReferenceAsset;
            if (sceneReferenceAsset == null) return false;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(sceneReferenceAsset.path);
            }

            return true;
        }
    }
}
#endif
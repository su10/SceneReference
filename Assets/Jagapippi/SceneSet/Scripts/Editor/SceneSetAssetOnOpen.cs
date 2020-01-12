#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;

namespace Jagapippi.SceneSet
{
    public static class SceneSetAssetOnOpen
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            // TODO: 再生中の場合の対応
            if (EditorApplication.isPlayingOrWillChangePlaymode) return true;

            var sceneSetAsset = EditorUtility.InstanceIDToObject(instanceID) as SceneSetAsset;
            if (sceneSetAsset == null) return false;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false) return true;

            for (var i = 0; i < sceneSetAsset.Count; i++)
            {
                var sceneReference = sceneSetAsset[i];
                if (sceneReference == null) continue;

                EditorSceneManager.OpenScene(
                    sceneReference.path,
                    (i == 0)
                        ? OpenSceneMode.Single
                        : OpenSceneMode.AdditiveWithoutLoading
                );
            }

            return true;
        }
    }
}
#endif
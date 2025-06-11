using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace BeatebyteToolsEditor.Runtime
{
    using Editor = UnityEditor.Editor;
    public class AssetHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceID, int line)
        {
            FootstepSurface obj = EditorUtility.InstanceIDToObject(instanceID) as FootstepSurface;
            if (obj != null)
            {
                FootstepObjectEditorWindow.Open(obj);
                return true;
            }
            return false;
        }

    }

    [CustomEditor(typeof(FootstepSurface))]
    public class FootstepSurfaceEditor : Editor
    {
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";        

        GUISkin bteSkin;

        public override void OnInspectorGUI()
        {
            if (!bteSkin)
            {
                bteSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(AssetDatabase.GUIDToAssetPath(GUISkinGUID));
            }
            GUI.skin = bteSkin;

            if (GUILayout.Button("Open Editor", bteSkin.GetStyle("bteButton")))
            {
                FootstepObjectEditorWindow.Open((FootstepSurface)target);
            }
        }
    }

}
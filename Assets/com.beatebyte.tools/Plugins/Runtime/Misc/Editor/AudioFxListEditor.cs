using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;



namespace BeatebyteToolsEditor.Runtime
{
    using Editor = UnityEditor.Editor;
    public class AssetAudioFXHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceID, int line)
        {
            AudioFXList obj = EditorUtility.InstanceIDToObject(instanceID) as AudioFXList;
            if (obj != null)
            {
                AudioFxListWindowEditor.Open(obj);
                return true;
            }
            return false;
        }

    }


    [CustomEditor(typeof(AudioFXList))]
    public class AudioFxListEditor : Editor
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
                AudioFxListWindowEditor.Open((AudioFXList)target);
            }
        }
    }

}
using UnityEditor;
using UnityEngine;
using static BeatebyteToolsEditor.Shared.Util;


namespace BeatebyteToolsEditor.Runtime
{
    public class AudioFxListWindowEditor : ExtendedEditorWindow
    {
        private static readonly string IconGUID = "1e22269ce2f8f55408a38b7fcb64832b";
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";

        GUISkin bteSkin;
        public static void Open(AudioFXList dataObject)
        {
            AudioFxListWindowEditor window = GetWindow<AudioFxListWindowEditor>("Audio FX List Editor");
            window.serializedObject = new SerializedObject(dataObject);
        }

        private void OnGUI()
        {
            var logo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(IconGUID));
            Texture2D _logo = ScaleTexture(logo, 50, 50);
            if (!bteSkin)
            {
                bteSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(AssetDatabase.GUIDToAssetPath(GUISkinGUID));
            }
            GUI.skin = bteSkin;

            GUIStyle titleStyle = new GUIStyle(bteSkin.GetStyle("bteTitle"));

            GUILayout.BeginHorizontal();

            GUIContent content = new();

            content.image = _logo;
            content.text = "  AUDIO FX LIST EDITOR";
            content.tooltip = "Audio FX List Editor";
            GUILayout.Label(content, titleStyle);
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            currentProperty = serializedObject.FindProperty("audioElements");
            if (currentProperty == null)
            {
                Debug.LogError("currentProperty is null");
                return;
            }
            DrawSelectedPropertyPanel();

            EditorGUILayout.EndVertical();
            Apply();

        }
        void DrawSelectedPropertyPanel()
        {
            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("audioElements", false);
            GUI.skin = bteSkin;
            EditorGUILayout.EndVertical();
        }
    }
}
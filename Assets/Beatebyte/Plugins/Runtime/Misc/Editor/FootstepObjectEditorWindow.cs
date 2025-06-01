using UnityEditor;
using UnityEngine;
using static BeatebyteToolsEditor.Shared.Util;

namespace BeatebyteToolsEditor.Runtime
{

    public class FootstepObjectEditorWindow : ExtendedEditorWindow
    {
        private static readonly string IconGUID = "7465d92e862b65746b9253a30cf41d06";
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";

        private GUISkin bteSkin;
        public static void Open(FootstepSurface dataObject)
        {
            FootstepObjectEditorWindow window = GetWindow<FootstepObjectEditorWindow>("Footstep Surface Editor");
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
            content.text = "       FOOTSTEP SURFACE EDITOR";
            content.tooltip = "Footstep Surface Editor";
            GUILayout.Label(content, titleStyle);

            GUILayout.EndHorizontal();

            currentProperty = serializedObject.FindProperty("gameData");

            EditorGUILayout.BeginVertical(GUI.skin.box);

            DrawSelectedPropertyPanel();

            EditorGUILayout.EndVertical();

            Apply();

        }
        void DrawSelectedPropertyPanel()
        {
            EditorGUILayout.BeginVertical("box");

            DrawField("surfaceName", true);
            DrawField("surfaceMaterial", true);
            DrawField("surfaceTexture", true);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("walkSounds", true);
            GUI.skin = bteSkin;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("runSounds", true);
            GUI.skin = bteSkin;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("jumpSounds", true);
            GUI.skin = bteSkin;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("landSounds", true);
            GUI.skin = bteSkin;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            GUI.skin = null;
            DrawField("slideSounds", true);
            GUI.skin = bteSkin;
            EditorGUILayout.EndVertical();
        }
    }
}
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
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
        private ReorderableList reorderableList;

        GUISkin bteSkin;
       /* private void OnEnable()
        {
            SerializedProperty listProp = serializedObject.FindProperty("audioElements");

            reorderableList = new ReorderableList(serializedObject, listProp, true, true, true, true);

            reorderableList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Audio Elements");
            };

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = listProp.GetArrayElementAtIndex(index);
                rect.y += 2;

                float third = rect.width / 3f;
                float height = EditorGUIUtility.singleLineHeight;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, third, height), element.FindPropertyRelative("name"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + third, rect.y, third, height), element.FindPropertyRelative("clip"), GUIContent.none);
                EditorGUI.PropertyField(new Rect(rect.x + third * 2, rect.y, third, height), element.FindPropertyRelative("category"), GUIContent.none);
            };

            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight + 6;
        }
*/
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
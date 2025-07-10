using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Runtime
{
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(AudioElement))]
    public class AudioElementDrawer : PropertyDrawer
    {
        GUISkin bteSkin;
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!bteSkin)
            {
                bteSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(AssetDatabase.GUIDToAssetPath(GUISkinGUID));
            }
            GUI.skin = bteSkin;
            EditorGUI.BeginProperty(position, label, property);

            // Foldout principale
            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded,
                label,
                true
            );

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                float y = position.y + EditorGUIUtility.singleLineHeight + 2f;
                float lineHeight = EditorGUIUtility.singleLineHeight;
                float fieldWidth = position.width;

                // Campo: categoria
                var categoryProp = property.FindPropertyRelative("category");
                EditorGUI.PropertyField(new Rect(position.x, y, fieldWidth, lineHeight), categoryProp);
                y += lineHeight + 2f;

                // Campo: name
                var nameProp = property.FindPropertyRelative("name");
                EditorGUI.PropertyField(new Rect(position.x, y, fieldWidth, lineHeight), nameProp);
                 y += lineHeight + 2f;
                float spacing = 2f;

                // Campo: clip + bottone "▶"
                var clipProp = property.FindPropertyRelative("clip");
                Rect clipRect = new Rect(position.x, y, fieldWidth - 45f, lineHeight);
                Rect playButtonRect = new Rect(position.x + spacing + fieldWidth - 42f, y, 20f, lineHeight + 2);
                Rect stopButtonRect = new Rect(position.x + spacing + fieldWidth - 20f, y, 20f, lineHeight + 2);

                /*//float y2 = position.y;
                float buttonWidth = 22f;
                float spacing = 2f;

                Rect clipRect = new Rect(position.x, y, position.width - (buttonWidth * 2 + spacing * 2), EditorGUIUtility.singleLineHeight);
                Rect playButtonRect = new Rect(clipRect.xMax + spacing, y, buttonWidth, EditorGUIUtility.singleLineHeight);
                Rect stopButtonRect = new Rect(playButtonRect.xMax + spacing, y, buttonWidth, EditorGUIUtility.singleLineHeight);*/

                EditorGUI.PropertyField(clipRect, clipProp, GUIContent.none);
                GUIContent playIcon = EditorGUIUtility.IconContent("d_PlayButton");
                GUIContent stopIcon = EditorGUIUtility.IconContent("d_PreMatQuad");

                if (GUI.Button(playButtonRect, playIcon, "bteMiniButton"))
                {
                    AudioClip clip = clipProp.objectReferenceValue as AudioClip;
                    if (clip != null)
                    {
                        AudioPreviewUtility.PlayPreviewClip(clip);
                    }
                }
                if (GUI.Button(stopButtonRect, stopIcon, "bteMiniButton"))
                {
                    AudioPreviewUtility.Stop();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            /*            float lineHeight = EditorGUIUtility.singleLineHeight;
                        float spacing = 2f;
                        return (lineHeight + spacing) * 3;
            */
            if(!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            // 3 campi * (altezza + padding)
            return (EditorGUIUtility.singleLineHeight + 2f) * 4;

        }

        private void PlayClip(AudioClip clip)
        {
            Debug.Log("PlayClip " + clip == null);
            if (clip == null) return;

            // Usa l'editor interno di Unity
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

            MethodInfo method = audioUtilClass.GetMethod(
                "PlayClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] { typeof(AudioClip) },
                null
            );
            Debug.Log("PlayClip " + method == null);

            if (method == null)
            {
                // Fallback per Unity versioni più nuove
                method = audioUtilClass.GetMethod(
                    "PlayPreviewClip",
                    BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                    null
                );

                if (method != null)
                    method.Invoke(null, new object[] { clip, 0, false });
            }
            else
            {
                method.Invoke(null, new object[] { clip });
            }
        }
        private void PlayPreviewClip(AudioClip clip)
        {
            if (clip == null) return;

            System.Type audioUtilType = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");
            if (audioUtilType == null)
            {
                Debug.LogWarning("AudioUtil non trovato.");
                return;
            }

            MethodInfo playMethod = audioUtilType.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );

            if (playMethod != null)
            {
                playMethod.Invoke(null, new object[] { clip, 0, false });
            }
            else
            {
                Debug.LogWarning("Metodo PlayPreviewClip non trovato.");
            }
        }
    }
}
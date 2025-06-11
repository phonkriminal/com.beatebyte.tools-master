using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Utils
{
    using Editor = UnityEditor.Editor;

    /// <summary>
    /// The e comment editor.
    /// </summary>
    [CustomEditor(typeof(eComment))]
    public class eCommentEditor : Editor
    {
        /// <summary>
        /// In edit.
        /// </summary>
        SerializedProperty inEdit;
        /// <summary>
        /// The comment.
        /// </summary>
        SerializedProperty comment;
        /// <summary>
        /// The header.
        /// </summary>
        SerializedProperty header;
        /// <summary>
        /// The text content.
        /// </summary>
        GUIContent textContent, headerContent, editButtonContent;
        /// <summary>
        /// The window.
        /// </summary>
        GUIStyle window, iconStyle, textStyle;
        /// <summary>
        /// The skin.
        /// </summary>
        GUISkin skin;

        /// <summary>
        /// On enable.
        /// </summary>
        private void OnEnable()
        {
            inEdit = serializedObject.FindProperty("inEdit");
            comment = serializedObject.FindProperty("comment");
            header = serializedObject.FindProperty("header");
            skin = Resources.Load("eSkin") as GUISkin;
            textContent = new GUIContent();
            editButtonContent = new GUIContent("", Resources.Load("eCommentEditIcon") as Texture2D, "Enable or Disable Edit Mode");
            headerContent = new GUIContent(Resources.Load("icon_v2") as Texture2D);
        }

        /// <summary>
        /// On disable.
        /// </summary>
        private void OnDisable()
        {
            inEdit.boolValue = false; if (serializedObject != null && serializedObject.targetObject != null) serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Use default margins.
        /// </summary>
        /// <returns>A bool</returns>
        public override bool UseDefaultMargins()
        {
            return false;
        }

        /// <summary>
        /// On inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            /* textStyle = new GUIStyle(EditorStyles.label);
             window = skin.GetStyle("eCommentWindow");
             iconStyle = skin.GetStyle("eCommentHeader");

             GUILayout.BeginVertical(window);            
             {
                 GUILayout.Space(-(window.padding.top - 5));

                 GUILayout.BeginHorizontal();
                 {
                     Color color = GUI.color;
                     headerContent.text = header.stringValue;
                     GUILayout.Box(headerContent, iconStyle, GUILayout.Height(30));
                     GUILayout.Space(-10);
                     if (GUILayout.Button(editButtonContent, GUIStyle.none, GUILayout.Width(30), GUILayout.Height(30)))                    
                     {
                         GenericMenu menu = new GenericMenu();
                         menu.AddSeparator("");
                         menu.AddItem(new GUIContent(!inEdit.boolValue ? "Edit Comment" : "Exit Edit"), false, () => { inEdit.boolValue = !inEdit.boolValue; serializedObject.ApplyModifiedProperties(); });
                         menu.AddSeparator("");
                         menu.ShowAsContext();
                     }
                 }
                 GUILayout.EndHorizontal();

                 GUILayout.Space((window.padding.top));

                 if (inEdit.boolValue)
                 {
                     EditorGUILayout.HelpBox("You can use RichText to customize your comment and header", MessageType.Info);
                     GUILayout.Label("Header", EditorStyles.centeredGreyMiniLabel);
                     header.stringValue = GUILayout.TextField(header.stringValue, 50);
                     GUILayout.Label("Comment", EditorStyles.boldLabel);
                     EditorGUILayout.PropertyField(comment, GUIContent.none);
                 }
                 else
                 {
                     if (textContent != null && textStyle != null)
                     {
                         textStyle.richText = true;
                         textStyle.normal.background = null;
                         textStyle.wordWrap = true;
                         textStyle.font = window.font;
                         textStyle.fontStyle = window.fontStyle;
                         textStyle.fontSize = window.fontSize;
                         textStyle.alignment = window.alignment;
                         GUILayout.Box(comment.stringValue, textStyle);
                     }
                 }
             }
             GUILayout.EndVertical();*/
            textStyle = new GUIStyle(EditorStyles.label);
            window = skin.GetStyle("eCommentWindow");
            iconStyle = skin.GetStyle("eCommentHeader");

            GUILayout.BeginVertical(window);
            {
                GUILayout.Space(-(window.padding.top - 5));

                Color color = GUI.color;

                GUILayout.BeginHorizontal(skin.box);
                headerContent.text = header.stringValue;
                GUILayout.Label(headerContent, iconStyle, GUILayout.Width(350), GUILayout.Height(30), GUILayout.ExpandWidth(true));
                //GUILayout.Box(headerContent, GUIStyle.none, GUILayout.Height(30));


                //GUILayout.Box(headerContent, iconStyle, GUILayout.Height(30));

                // GUILayout.Space(-10);

                if (GUILayout.Button(editButtonContent, GUIStyle.none, GUILayout.Width(30), GUILayout.Height(30)))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent(!inEdit.boolValue ? "Edit Comment" : "Exit Edit"), false, () => { inEdit.boolValue = !inEdit.boolValue; serializedObject.ApplyModifiedProperties(); });
                    menu.AddSeparator("");
                    menu.ShowAsContext();
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space((window.padding.top));

            if (inEdit.boolValue)
            {
                EditorGUILayout.HelpBox("You can use RichText to customize your comment and header", MessageType.Info);
                GUILayout.Label("Header", EditorStyles.centeredGreyMiniLabel);
                header.stringValue = GUILayout.TextField(header.stringValue, 80);
                GUILayout.Label("Comment", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(comment, GUIContent.none);
            }
            else
            {
                if (textContent != null && textStyle != null)
                {
                    textStyle.richText = true;
                    textStyle.normal.background = null;
                    textStyle.wordWrap = true;
                    textStyle.font = window.font;
                    textStyle.fontStyle = window.fontStyle;
                    textStyle.fontSize = window.fontSize;
                    textStyle.alignment = window.alignment;
                    GUILayout.Box(comment.stringValue, textStyle);
                }
            }
            EditorGUILayout.EndVertical();


            serializedObject.ApplyModifiedProperties();
        }
    }
}

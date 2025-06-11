using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e help box decorator.
    /// </summary>
    [CustomPropertyDrawer(typeof(eHelpBoxAttribute))]
    public class eHelpBoxDecorator : DecoratorDrawer
    {
        /// <summary>
        /// The size.
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// The style.
        /// </summary>
        GUIStyle style = new GUIStyle(EditorStyles.helpBox);
        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        public override void OnGUI(Rect position)
        {
            style ??= new GUIStyle(EditorStyles.helpBox);

            var helpbox = attribute as eHelpBoxAttribute;

            GUIContent content = new GUIContent(helpbox.text);

            switch (helpbox.messageType)
            {
                case eHelpBoxAttribute.MessageType.Info:
                    content = EditorGUIUtility.IconContent("console.infoicon", helpbox.text);
                    break;
                case eHelpBoxAttribute.MessageType.Warning:
                    content = EditorGUIUtility.IconContent("console.warnicon", helpbox.text);
                    break;
            }
            content.text = helpbox.text;
            style.richText = true;
            GUI.Box(position, content, style);
        }

        /// <summary>
        /// Get the height.
        /// </summary>
        /// <returns>A float</returns>
        public override float GetHeight()
        {
            var helpBoxAttribute = attribute as eHelpBoxAttribute;
            if (helpBoxAttribute == null) return base.GetHeight();
            style ??= new GUIStyle(EditorStyles.helpBox);
            style.richText = true;
            if (style == null) return base.GetHeight();
            return Mathf.Max(EditorGUIUtility.singleLineHeight, style.CalcHeight(new GUIContent(helpBoxAttribute.text), EditorGUIUtility.currentViewWidth) + 10);
        }
    }
}
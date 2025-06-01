using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// eSeparatorDrawer is a custom property drawer for the eSeparator attribute.
    /// It draws a separator line with a label in the Unity Inspector.
    /// </summary>

    [CustomPropertyDrawer(typeof(eSeparator))]
    public class eSeparatorDrawer : DecoratorDrawer
    {
        /// <summary>
        /// The separator.
        /// </summary>
        eSeparator _separator;
        /// <summary>
        /// The style.
        /// </summary>
        GUIStyle _style;
        /// <summary>
        /// The content.
        /// </summary>
        GUIContent _content;
        /// <summary>
        /// Gets the separator.
        /// </summary>
        public eSeparator separator
        {
            get
            {
                _separator ??= (eSeparator)attribute;

                return _separator;
            }
        }

        /// <summary>
        /// Gets the style.
        /// </summary>
        public GUIStyle style
        {
            get
            {
                if (_style == null)
                {
                    if (string.IsNullOrEmpty(separator.style))
                    {
                        _style = new GUIStyle(EditorStyles.helpBox);
                        _style.fontStyle = FontStyle.Bold;
                        _style.alignment = TextAnchor.UpperCenter;
                        _style.fontSize = 12;
                        _style.normal.textColor = new Color(1f, 0.5490196f, 0f, 1f);
                    }
                    else
                    {
                        _style = new GUIStyle(separator.style);
                    }
                }
                _style.richText = true;
                return _style;
            }
        }


        /// <summary>
        /// Gets the content.
        /// </summary>
        public GUIContent content
        {
            get
            {
                _content ??= new GUIContent(separator.label, separator.tooltip);
                return _content;
            }
        }

        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        public override void OnGUI(Rect position)
        {
            position.height -= EditorGUIUtility.singleLineHeight;
            position.y += EditorGUIUtility.singleLineHeight * 0.5f;
            base.OnGUI(position);
            style.fontSize = separator.fontSize;
            GUI.Box(position, new GUIContent(separator.label, separator.tooltip), style);
        }

        /// <summary>
        /// Get the height.
        /// </summary>
        /// <returns>A float</returns>
        public override float GetHeight()
        {
            style.fontSize = separator.fontSize;
            return style.CalcHeight(content, EditorGUIUtility.currentViewWidth) + EditorGUIUtility.singleLineHeight;
        }
    }
}
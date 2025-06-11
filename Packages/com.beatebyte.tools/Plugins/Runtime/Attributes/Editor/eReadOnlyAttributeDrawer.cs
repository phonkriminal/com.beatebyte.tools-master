using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// Custom property drawer for the eReadOnlyAttribute.
    /// </summary>
    /// <remarks>
    /// This class is used to create a custom property drawer for the eReadOnlyAttribute.
    /// It allows displaying read-only properties in the inspector with a specific style.
    /// </remarks>

    [CustomPropertyDrawer(typeof(eReadOnlyAttribute))]
    public class eReadOnlyAttributeDrawer : PropertyDrawer
    {

        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            var att = attribute as eReadOnlyAttribute;
            if (att.justInPlayMode && !Application.isPlaying) return;
            string value = "Null";

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    value = property.intValue.ToString();
                    break;
                case SerializedPropertyType.Boolean:
                    value = property.boolValue.ToString();
                    break;
                case SerializedPropertyType.Float:
                    value = property.floatValue.ToString("0.0");
                    break;
                case SerializedPropertyType.String:
                    value = property.stringValue;
                    break;

                case SerializedPropertyType.Quaternion:
                    value = property.quaternionValue.eulerAngles.ToString();
                    break;
                case SerializedPropertyType.Vector2:
                    value = property.vector2Value.ToString();
                    break;
                case SerializedPropertyType.Vector3:
                    value = property.vector3Value.ToString();
                    break;
                case SerializedPropertyType.Enum:
                    value = property.enumDisplayNames[property.enumValueIndex];
                    break;
                case SerializedPropertyType.ObjectReference:
                    value = "Null";
                    break;
                default:
                    value = "(not supported)";
                    break;
            }

            var fontStyle = GUI.skin.label.fontStyle;
            GUI.skin.label.fontStyle = FontStyle.BoldAndItalic;
            GUIStyle style = new GUIStyle(EditorStyles.wordWrappedLabel);
            style.fontStyle = FontStyle.BoldAndItalic;

            style.normal.textColor = Color.black;
            style.alignment = TextAnchor.MiddleLeft;
            var rect = position;
            rect.width = position.width * 0.6f;
            EditorGUI.LabelField(rect, "", label.text, style);
            style.normal.textColor = property.propertyType == SerializedPropertyType.Boolean ? property.boolValue ? Color.green : Color.red :
                                    (property.propertyType == SerializedPropertyType.ObjectReference ? property.objectReferenceValue ? Color.green : Color.red : Color.black);
            style.alignment = TextAnchor.MiddleLeft;
            position.x += rect.width + 0.05f;
            position.width *= 0.35f;
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue) EditorGUI.ObjectField(position, property.objectReferenceValue, typeof(Object), true);
            else EditorGUI.LabelField(position, "", value, style);
            GUI.skin.label.fontStyle = fontStyle;
        }

        /// <summary>
        /// Get property height.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        /// <returns>A float</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var att = attribute as eReadOnlyAttribute;
            if (att.justInPlayMode && !Application.isPlaying) return 0;
            return base.GetPropertyHeight(property, label);
        }
    }
}
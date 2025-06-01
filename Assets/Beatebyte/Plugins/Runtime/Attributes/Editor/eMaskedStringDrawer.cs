using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{

    /// <summary>
    /// The e masked string drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(eMaskedStringAttribute))]
    public class eMaskedStringDrawer : PropertyDrawer
    {
        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            eMaskedStringAttribute eMaskedString = (eMaskedStringAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.PasswordField(position, label, property.stringValue, eMaskedString.style);
            }
        }
    }

}
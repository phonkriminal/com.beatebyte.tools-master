using System;
using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// Custom property drawer for eToggleOptionAttribute.
    /// </summary>
    /// <remarks>
    /// This class is used to create a custom property drawer for the eToggleOptionAttribute.
    /// It allows the user to select between two options using a dropdown menu.
    /// </remarks>
    /// <example>
    /// [eToggleOption("Label", "False Value", "True Value")]
    /// public bool myBool;
    /// </example>

    [CustomPropertyDrawer(typeof(eToggleOptionAttribute), true)]
    public class eToggleOptionAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Boolean)
            {

                var toogle = attribute as eToggleOptionAttribute;
                if (toogle.label != "") label.text = toogle.label;
                var options = new GUIContent[] { new GUIContent(toogle.falseValue), new GUIContent(toogle.trueValue) };
                property.boolValue = Convert.ToBoolean(EditorGUI.Popup(position, label, Convert.ToInt32(property.boolValue), options));
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    [CustomPropertyDrawer(typeof(eColorAttribute))]

    public class eColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            eColorAttribute colAttr = (eColorAttribute)attribute;

            GUI.skin = null;

            if (property.propertyType == SerializedPropertyType.Color)
            {
                // Visualizza il color picker con supporto HDR
                Color newColor = EditorGUI.ColorField(position,
                    new GUIContent(string.IsNullOrEmpty(colAttr.label) ? label.text : colAttr.label),
                    property.colorValue, true, true, false);

                property.colorValue = newColor;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [HDRColor] with Color fields.");
            }
        }
    }

}
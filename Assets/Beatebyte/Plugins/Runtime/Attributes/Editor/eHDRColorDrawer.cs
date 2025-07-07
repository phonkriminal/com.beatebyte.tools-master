using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    [CustomPropertyDrawer(typeof(eHDRColorAttribute))]
    public class eHDRColorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {          
           
            eHDRColorAttribute hdrAttr = (eHDRColorAttribute)attribute;

            GUI.skin = null;

            if (property.propertyType == SerializedPropertyType.Color)
            {
                // Visualizza il color picker con supporto HDR
                Color32 newColor = EditorGUI.ColorField(position,
                    new GUIContent(string.IsNullOrEmpty(hdrAttr.label) ? label.text : hdrAttr.label),
                    property.colorValue, true, true, true);

                property.colorValue = newColor;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [HDRColor] with Color fields.");
            }
        }
    }

}
using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Runtime
{
    [CustomPropertyDrawer(typeof(AudioElement))]
    public class AudioElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty categoryProp = property.FindPropertyRelative("category");
            SerializedProperty nameProp = property.FindPropertyRelative("name");
            SerializedProperty clipProp = property.FindPropertyRelative("clip");
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, categoryProp);
            position.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(position, nameProp);
            position.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(position, clipProp);
            position.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2) * 3;
        }
    }

}
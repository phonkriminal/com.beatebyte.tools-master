using System;
using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e dropdown list drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(eDropdownListAttribute), true)]
    public class eDropdownListDrawer : PropertyDrawer
    {
        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            var options = GetOptions(property);
            DrawDropdown(position, property, label, options);

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Get the options.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>An array of string</returns>
        private string[] GetOptions(SerializedProperty property)
        {
            var attr = attribute as eDropdownListAttribute;

            var methodName = attr.MethodName;

            var objectType = fieldInfo.DeclaringType;

            var methodOwnerType = attr.Location == eDropdownListAttribute.MethodLocation.PropertyClass ? objectType : attr.MethodOwnerType;

            var methodInfo = methodOwnerType.GetMethod
                (methodName,
                System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Static
                | System.Reflection.BindingFlags.Instance);

            if (methodInfo == null)
            {
                Debug.LogError($"Method {methodName} In {methodOwnerType.FullName} Could Not Be Found!");
                return new string[] { "<error: method not found>" };
            }
            var methodInfoReturnValueIsStringArray = methodInfo.ReturnType == typeof(string[]);
            if (!methodInfoReturnValueIsStringArray)
            {
                Debug.LogError($"Method {methodName} In {methodOwnerType.FullName} Does Not Have A Return Type Of {typeof(string[]).FullName}");
                return new string[] { "<error: invalid return value>" };
            }

            var invokeReference = attr.Location == eDropdownListAttribute.MethodLocation.StaticClass ? null : property.serializedObject.targetObject;

            var returnValue = methodInfo.Invoke(invokeReference, null) as string[];

            return returnValue;
        }
        /// <summary>
        /// Draws the dropdown.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        /// <param name="options">The options.</param>
        private void DrawDropdown(Rect position, SerializedProperty property, GUIContent label, string[] options)
        {
            if (options == null || options.Length == 0)
            {
                options = new string[] { "<empty>" };
            }

            var selectedIndex = Array.IndexOf(options, property.stringValue);
            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                selectedIndex = EditorGUI.Popup(EditorGUI.PrefixLabel(position, label), selectedIndex, options);
                if (check.changed)
                {
                    property.stringValue = options[selectedIndex];
                }
            }
        }
    }

}
using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{

    /// <summary>
    /// The e slider attribute drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(eSliderAttribute))]
    public class eSliderAttributeDrawer : PropertyDrawer
    {

        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            eSliderAttribute sliderAttribute = (eSliderAttribute)attribute;           

            if (property.propertyType == SerializedPropertyType.Float)
            {
                label.text = string.IsNullOrEmpty(sliderAttribute.label) ? label.text : sliderAttribute.label;
                EditorGUI.Slider(position, property, sliderAttribute.min, sliderAttribute.max, label);
                
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                label.text = string.IsNullOrEmpty(sliderAttribute.label) ? label.text : sliderAttribute.label;
                EditorGUI.IntSlider(position, property, (int)sliderAttribute.min, (int)sliderAttribute.max, label);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use eSlider with float or int.");
            }            

            if (GUI.changed)
            {
                // Fix for CS0149 and CS1002
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();

                var methodName = sliderAttribute.MethodName;

                var objectType = fieldInfo.DeclaringType;

                var methodOwnerType = sliderAttribute.Location == eSliderAttribute.MethodLocation.PropertyClass ? objectType : sliderAttribute.MethodOwnerType;
                var methodInfo = methodOwnerType.GetMethod
                   (methodName,
                   System.Reflection.BindingFlags.NonPublic
                   | System.Reflection.BindingFlags.Public
                   | System.Reflection.BindingFlags.Static
                   | System.Reflection.BindingFlags.Instance);

                if (methodInfo == null)
                {
                    Debug.LogError($"Method {methodName} In {methodOwnerType.FullName} Could Not Be Found!");
                }

                var invokeReference = sliderAttribute.Location == eSliderAttribute.MethodLocation.StaticClass ? null : property.serializedObject.targetObject;
                object name = property.name;
                object value = property.floatValue;
                var args = new[] { name, value };
                if ( invokeReference != null ) { methodInfo.Invoke( invokeReference, args); }
            }
        }
    }

}
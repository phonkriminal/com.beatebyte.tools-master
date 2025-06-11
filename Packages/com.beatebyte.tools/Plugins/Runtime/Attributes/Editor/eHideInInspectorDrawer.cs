using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The v hide in inspector drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(eHideInInspectorAttribute), true)]
  
    public class vHideInInspectorDrawer : PropertyDrawer
    {
        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            eHideInInspectorAttribute _attribute = attribute as eHideInInspectorAttribute;

            if (_attribute != null && property.serializedObject.targetObject)
            {
                var propertyName = property.propertyPath.Replace(property.name, "");
                var booleamProperties = _attribute.refbooleanProperty.Split(';');
                for (int i = 0; i < booleamProperties.Length; i++)
                {
                    var booleanProperty = property.serializedObject.FindProperty(propertyName + booleamProperties[i]);
                    if (booleanProperty != null)
                    {
                        _attribute.hideProperty = (bool)_attribute.invertValue ? booleanProperty.boolValue : !booleanProperty.boolValue;
                        if (_attribute.hideProperty)
                        {
                            break;
                        }
                    }
                    else
                    {

                        EditorGUI.PropertyField(position, property, label, true);
                    }
                }
                if (!_attribute.hideProperty)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
                EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Get property height.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        /// <returns>A float</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            eHideInInspectorAttribute _attribute = attribute as eHideInInspectorAttribute;
            if (_attribute != null)
            {
                var propertyName = property.propertyPath.Replace(property.name, "");
                var booleamProperties = _attribute.refbooleanProperty.Split(';');
                var valid = true;
                for (int i = 0; i < booleamProperties.Length; i++)
                {
                    var booleamProperty = property.serializedObject.FindProperty(propertyName + booleamProperties[i]);
                    if (booleamProperty != null)
                    {
                        valid = _attribute.invertValue ? !booleamProperty.boolValue : booleamProperty.boolValue;
                        if (!valid) break;
                    }
                }
                if (valid) return base.GetPropertyHeight(property, label);
                else return 0;
            }
            return base.GetPropertyHeight(property, label);
        }

    }
}
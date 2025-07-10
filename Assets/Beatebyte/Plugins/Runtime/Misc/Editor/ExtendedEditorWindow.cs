using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BeatebyteToolsEditor.Runtime
{
    public class ExtendedEditorWindow : EditorWindow
    {
        protected SerializedObject serializedObject;
        protected SerializedProperty currentProperty;

        private string selectedPropertyPath;
        protected SerializedProperty selectedProperty;
        protected Dictionary<string, ReorderableList> reorderableLists = new();

        protected void DrawProperty(SerializedProperty prop, bool drawChildren)
        {
            string lastPropPath = string.Empty;

            foreach (SerializedProperty p in prop)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperty(p, drawChildren);
                        EditorGUI.indentLevel--;

                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                    lastPropPath = p.propertyPath;

                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }

        protected void DrawSidebar(SerializedProperty prop)
        {
            foreach (SerializedProperty p in prop)
            {
                if (GUILayout.Button(p.displayName))
                {
                    selectedPropertyPath = p.propertyPath;
                }
            }

            if (!string.IsNullOrEmpty(selectedPropertyPath))
            {
                selectedProperty = serializedObject.FindProperty(selectedPropertyPath);

            }
        }

        protected void DrawField(string propName, bool relative)
        {
            /* if (relative && currentProperty != null)
             {
                 EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(propName), true);
             }
             else if (serializedObject != null)
             {
                 EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), true);
             }*/
            SerializedProperty prop = null;

            if (relative && currentProperty != null)
            {
                prop = currentProperty.FindPropertyRelative(propName);
            }
            else if (serializedObject != null)
            {
                prop = serializedObject.FindProperty(propName);
            }

            if (prop == null)
            {
                EditorGUILayout.HelpBox($"⚠️ Proprietà '{propName}' non trovata!", MessageType.Warning);
                return;
            }

            if (prop.isArray && prop.propertyType != SerializedPropertyType.String)
            {
                DrawReorderableList(propName, prop);
            }
            else
            {
                EditorGUILayout.PropertyField(prop, includeChildren: true);
            }
        }
        private void DrawReorderableList(string key, SerializedProperty prop)
        {
            if (!reorderableLists.ContainsKey(key))
            {
                var list = new ReorderableList(prop.serializedObject, prop, true, true, true, true);

                list.drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, ObjectNames.NicifyVariableName(prop.name));
                };

                list.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = prop.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(rect, element, GUIContent.none, includeChildren: true);
                };

                list.elementHeightCallback = index =>
                {
                    var element = prop.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element, true) + 4;
                };

                reorderableLists[key] = list;
            }

            reorderableLists[key].DoLayoutList();
        }
       
        protected void Apply()
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

}
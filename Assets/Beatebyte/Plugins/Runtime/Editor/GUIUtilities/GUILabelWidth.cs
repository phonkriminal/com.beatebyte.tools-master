using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Editor.GUIUtilities
{
    public class GUILabelWidth : GUI.Scope
    {
        private readonly float _previousLabelWidth;

        public GUILabelWidth(float labelWidth)
        {
            _previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        protected override void CloseScope()
        {
            EditorGUIUtility.labelWidth = _previousLabelWidth;
        }
    }
}
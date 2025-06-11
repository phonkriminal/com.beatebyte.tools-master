using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Editor
{
    using Editor = UnityEditor.Editor;
    [InitializeOnLoad]
    static class eInspectorSearchTool
    {
        static eInspectorSearchTool()
        {
            Editor.finishedDefaultHeaderGUI -= DrawInpectorSearchTool;
            Editor.finishedDefaultHeaderGUI += DrawInpectorSearchTool;
        }
        public static string search;
        private static bool fold = false;

        public static GameObject lastSelection;
        static void DrawInpectorSearchTool(Editor editor)
        {
            if (editor.target.GetType() != typeof(GameObject))
            {
                return;
            }
            if (Selection.activeGameObject)
            {
                if (lastSelection != Selection.activeGameObject)
                {
                    lastSelection = Selection.activeGameObject;
                    search = "";
                }
                var components = Selection.activeGameObject.GetComponents<Component>().ToList();
                var totalRect = EditorGUILayout.GetControlRect();
                try
                {
                    EditorGUI.LabelField(totalRect, $"eInspector Search Tool | Hided Components : {components.FindAll(c => c.hideFlags == HideFlags.HideInInspector).Count.ToString("00")} | {components.Count.ToString("00")}", EditorStyles.toolbar);
                    totalRect = EditorGUILayout.GetControlRect();
                    search = EditorGUI.TextField(totalRect, search, EditorStyles.toolbarSearchField);

                    totalRect = EditorGUILayout.GetControlRect();
                    if (GUI.Button(totalRect, fold ? "Unfold All Scripts" : "Fold All Scripts"))
                    {
                        for (int i = 0; i < components.Count; i++)
                        {
                            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(components[i], fold);
                        }
                        ActiveEditorTracker.sharedTracker.ForceRebuild();
                        fold = !fold;
                    }
                    if (string.IsNullOrEmpty(search))
                    {
                        for (int i = 0; i < components.Count; i++)
                        {
                            var targetState = HideFlags.None;
                            if (targetState != components[i].hideFlags)
                                components[i].hideFlags = targetState;

                        }
                    }
                    else
                    {
                        for (int i = 0; i < components.Count; i++)
                        {
                            if (components[i].GetType().Name.ToUpper().Contains(search.ToUpper()))
                            {
                                var targetState = HideFlags.None;
                                if (targetState != components[i].hideFlags)
                                    components[i].hideFlags = targetState;
                            }
                            else
                            {
                                var targetState = HideFlags.HideInInspector;
                                if (targetState != components[i].hideFlags)
                                    components[i].hideFlags = targetState;
                            }
                        }
                    }
                }
                catch
                {
                    ///DO Nothing
                }
            }
        }
    }

}
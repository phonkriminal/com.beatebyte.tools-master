using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static BeatebyteToolsEditor.Shared.Util;

namespace BeatebyteToolsEditor
{
    /// <summary>
    /// The EDS remove all components.
    /// </summary>
    public class RemoveAllComponents : EditorWindow
    {

        private static readonly string IconGUID = "643fff9a5f3ccf94aa0a9320c6ca2dba";
        
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";

        //This is an editor script that is used to remove all components of a gameobject.
        //To use: Add this script to a gameobject
        //To clear multiple objects from components, mark multiple objects
        //and add the script to them

        /// <summary>
        /// The logo.
        /// </summary>
        Texture2D logo;
        /// <summary>
        /// The logo.
        /// </summary>
        Texture2D _logo;

        /// <summary>
        /// The es skin.
        /// </summary>
        GUISkin bteSkin;

        /// <summary>
        /// The min rect.
        /// </summary>
        Vector2 minRect = new(300, 50);

        /// <summary>
        /// Check again.
        /// </summary>
        private bool checkAgain = false;

        /// <summary>
        /// The game object.
        /// </summary>
        public GameObject gameObject = null;

        /// <summary>
        /// The components.
        /// </summary>
        public List<Component> components = new();

        /// <summary>
        /// The index.
        /// </summary>
        int index = 0;

        /// <summary>
        /// Show the window.
        /// </summary>
        [MenuItem("Window/Beatebyte Creations/Tools/Remove All Components", false, 2)]
        [MenuItem("GameObject/Remove All Components",isValidateFunction: false, 13)]
        public static void ShowWindow()
        {
            GetWindow<RemoveAllComponents>();
        }

        [MenuItem("Window/Beatebyte Creations/Tools/Remove All Components", true)]
        [MenuItem("GameObject/Remove All Components", isValidateFunction: true)]
        static bool ValidateSelection()
        {
            return Selection.gameObjects.Length > 0;
        }
        /// <summary>
        /// On enable.
        /// </summary>
        private void OnEnable()
        {
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(IconGUID));
            _logo = ScaleTexture(logo, 50, 50);
            //CheckConditions();
        }


        /// <summary>
        /// Check the conditions.
        /// </summary>
        void CheckConditions()
        {
            if (Selection.activeObject)
            {
                gameObject = Selection.activeGameObject;
            }
            if (gameObject)
            {
                checkAgain = false;
            }
            if (!gameObject)
            {
                checkAgain = true;
            }
        }



        /// <summary>
        /// Get current components.
        /// </summary>
        void GetCurrentComponents()
        {
            gameObject.GetComponents<Component>(components);
        }



        /// <summary>
        /// On GUI.
        /// </summary>
        private void OnGUI()
        {
            if (!bteSkin)
            {
                bteSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(AssetDatabase.GUIDToAssetPath(GUISkinGUID));
            }

            GUI.skin = bteSkin;
            //Texture2D preview;

            this.minSize = minRect;
            this.titleContent = new GUIContent("Components Cleaner", null, "Remove All Components from GameObject");

            GUIStyle titleStyle = new GUIStyle(bteSkin.GetStyle("bteTitle"));
            GUIStyle textureBoxStyle = new GUIStyle(bteSkin.GetStyle("bteTextureBox"));
            GUIStyle labelStyle = new GUIStyle(bteSkin.GetStyle("bteLabelTex"));


            GUILayoutOption[] layoutOptions = new GUILayoutOption[]
            {
                GUILayout.Width(minRect.x),
                GUILayout.Height(minRect.y)
            };

            GUILayout.BeginVertical("       COMPONENTS CLEANER", titleStyle);
            {
                GUILayout.Label(logo, GUILayout.MaxHeight(48));
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical("", textureBoxStyle);
            {   

                if (checkAgain) CheckConditions();

                GUILayoutOption[] layoutOptionsBox = new GUILayoutOption[]
                {
                GUILayout.Width(minRect.x -20),
                GUILayout.Height(minRect.y - 20),
                GUILayout.ExpandWidth(true)
                };

                GUILayout.BeginVertical(textureBoxStyle);
                {

                    if (!gameObject)
                    {
                        EditorGUILayout.HelpBox("Select a GameObject!", MessageType.Info);
                        checkAgain = true;
                    }
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("GameObject ", labelStyle, GUILayout.ExpandWidth(true));
                        gameObject = EditorGUILayout.ObjectField("", gameObject, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                    }
                    GUILayout.EndHorizontal();

                    if (gameObject != null && gameObject.scene.name == null)
                    {
                        EditorGUILayout.HelpBox("GameObject cannot be a Prefab!", MessageType.Info);
                    }
                }
                GUILayout.EndVertical();


                GUILayout.BeginVertical("List of Components", textureBoxStyle);
                {
                    GUILayout.Space(25);

                    if (gameObject && gameObject.scene.name != null)
                    {
                        GetCurrentComponents();
                        if (components.Count > 0)
                        {
                            List<string> componentsList = new();
                            foreach (Component item in components)
                            {
                                string componentName = item.GetType().ToString();
                                //Debug.Log(componentName);
                                if (componentName != "UnityEngine.Transform" && !componentsList.Contains(componentName))
                                {
                                    componentsList.Add(componentName);
                                }
                            }

                            string[] options = componentsList.ToArray();
                            GUILayout.Space(5);
                            index = EditorGUILayout.Popup(index, options);

                            if (GUILayout.Button("Clean", layoutOptionsBox))
                            {
                                Clean(index, options[index]);
                            }
                            //Debug.Log(componentsList.Count());
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        private void Clean(int index, string componentName)
        {

            if (EditorUtility.DisplayDialog("Remove Components ?",
                    $"Are you sure you want to remove all components of type {componentName}?", "Yes", "Cancel"))
            {
                foreach (var item in gameObject.GetComponents<Component>())
                {
                    if (item.GetType().ToString() == componentName)
                    {
                        DestroyImmediate(item);
                    }
                }
                Debug.Log("Clean");
            }
        }

    }
}
using System;
using System.Linq;
using System.Reflection;
using Codice.Client.IssueTracker;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;
using static BeatebyteToolsEditor.Shared.Util;

namespace BeatebyteToolsEditor
{

    /// <summary>
    /// The EDS skinned mesh baker editor.
    /// </summary>
    public class SkinnedMeshBakerEditor : EditorWindow
    {
        /// <summary>
        /// The GUI SKIN PATH.
        /// </summary>
        private static readonly string IconGUID = "643fff9a5f3ccf94aa0a9320c6ca2dba";

        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";
        /// <summary>
        /// The rect.
        /// </summary>
        Vector2 rect = new Vector2(400, 300);
        /// <summary>
        /// The E skin.
        /// </summary>
        GUISkin bteSkin;
        /// <summary>
        /// The M logo.
        /// </summary>
        Texture2D m_Logo;

        /// <summary>
        /// The skinned mesh.
        /// </summary>
        [SerializeField]
        private SkinnedMeshRenderer skinnedMesh;
        /// <summary>
        /// The default GO.
        /// </summary>
        [SerializeField]
        private GameObject defaultGO;
        /// <summary>
        /// The default mesh.
        /// </summary>
        private Mesh defaultMesh;

        /// <summary>
        /// The mesh name.
        /// </summary>
        [SerializeField]
        private string meshName;
        /// <summary>
        /// The baked mesh.
        /// </summary>
        private Mesh bakedMesh;
        /// <summary>
        /// The mesh filter.
        /// </summary>
        private MeshFilter meshFilter;
        /// <summary>
        /// Can bake.
        /// </summary>
        private bool canBake = false;
        /// <summary>
        /// The toggle.
        /// </summary>
        private bool toggle = false;
        /// <summary>
        /// Is legal.
        /// </summary>
        private bool isLegal = false;

        private bool addToScene = false;
        private bool optimizeMesh = false;
        /// <summary>
        /// Show the window.
        /// </summary>
        [MenuItem("Window/Beatebyte Creations/Tools/Mesh Baker", false, 2)]
        

        public static void ShowWindow()
        {
            GetWindow<SkinnedMeshBakerEditor>();
        }      

        /// <summary>
        /// On GUI.
        /// </summary>
        private void OnGUI()
        {

            m_Logo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(IconGUID));
            Texture2D _logo = ScaleTexture(m_Logo, 48, 48);
            if (!bteSkin)
            {
                bteSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(AssetDatabase.GUIDToAssetPath(GUISkinGUID));
            }
            GUI.skin = bteSkin;

            GUIStyle titleStyle = new GUIStyle(bteSkin.GetStyle("bteTitle"));
            GUIStyle textureBoxStyle = new GUIStyle(bteSkin.GetStyle("bteTextureBox"));
            GUIStyle labelStyle = new GUIStyle(bteSkin.GetStyle("bteLabelTex"));

            this.minSize = rect;
            this.titleContent = new GUIContent("Character", null, "Character Creator Mesh Baker");
            GUILayout.BeginVertical("       MESH BACKER", titleStyle);
            {
                GUILayout.Label(m_Logo, GUILayout.MaxHeight(48));
            }
            GUILayout.EndVertical();
           

            #region HEADER
           
            GUILayout.BeginVertical(textureBoxStyle);

            toggle = EditorGUILayout.Toggle("Is Skinned Mesh ", toggle, "toggle");

            if (Selection.activeGameObject)
            {
               defaultGO = Selection.activeGameObject;

                if (defaultGO.transform.TryGetComponent<SkinnedMeshRenderer>(out skinnedMesh))
                { 
                    toggle = true;
                }
                else
                {
                    toggle = false;
                }
            }

            if (toggle)
            {
                if (defaultGO != null) 
                { 
                    skinnedMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Skinned Mesh", skinnedMesh, typeof(SkinnedMeshRenderer), true);
                }
                else 
                { 
                    skinnedMesh = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Skinned Mesh", skinnedMesh, typeof(SkinnedMeshRenderer), true);

                    if (!skinnedMesh)
                    {
                        EditorGUILayout.Separator();

                        EditorGUILayout.HelpBox("You must assign a skinned mesh renderer component", MessageType.Warning);
                    }
                }

                canBake = skinnedMesh;
            }
            else
            {
                defaultGO = (GameObject)EditorGUILayout.ObjectField("Game Object", defaultGO, typeof(GameObject), true);

                if (!defaultGO)
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.HelpBox("You must assign a GameObject", MessageType.Warning);

                }
                else
                {
                    isLegal = defaultGO.TryGetComponent<MeshFilter>(out meshFilter);

                    if (!isLegal)
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("You must assign a GameObject with the MeshFilter component.", MessageType.Error);
                    }
                    else
                    {
                        defaultMesh = meshFilter.sharedMesh;
                        if (!defaultMesh)
                        {
                            EditorGUILayout.Separator();
                            EditorGUILayout.HelpBox("You must assign a mesh renderer component", MessageType.Warning);
                        }
                    }
                }
                canBake = defaultGO & isLegal;
            }

            EditorGUILayout.Separator();
            meshName = EditorGUILayout.TextField("Mesh Name", meshName);

            if (meshName is null || meshName.Trim().Length == 0)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("You must assign a name to the new mesh", MessageType.Info);
                canBake &= false;
            }


            #endregion

            EditorGUILayout.Space(20);
            EditorGUILayout.BeginHorizontal(textureBoxStyle);
            {
                EditorGUILayout.LabelField("Add to Scene", GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
                addToScene = EditorGUILayout.Toggle(addToScene, "toggle", GUILayout.Width(20));
                EditorGUILayout.LabelField("Optimize Mesh", GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
                optimizeMesh = EditorGUILayout.Toggle(optimizeMesh, "toggle", GUILayout.Width(20));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            if (GUILayout.Button("Bake Mesh"))
            {
                if (toggle && canBake)
                {
                    BakeSkinnedMesh();
                }
                else if (!toggle && canBake)
                {
                    BakeMesh();
                }
            }

            EditorGUILayout.Separator();
            if (GUILayout.Button("Reset"))
            {
                toggle = false;
                defaultGO = null;
                skinnedMesh = null;
                meshName = "";
            }

            EditorGUILayout.EndVertical();

        }

        /// <summary>
        /// Bake the mesh.
        /// </summary>
        private void BakeMesh()
        {
            bakedMesh = defaultMesh;

            Material material = defaultGO.GetComponent<Renderer>().sharedMaterial;
            //SaveMesh(bakedMesh, meshName, false, true);
            if (optimizeMesh)
            {
                MeshUtility.Optimize(bakedMesh);
            }
            GameObject tempGameObject = new();
            tempGameObject.name = meshName;
            tempGameObject.AddComponent<MeshFilter>();
            tempGameObject.AddComponent<MeshRenderer>();
            tempGameObject.GetComponent<MeshFilter>().sharedMesh = bakedMesh;
            tempGameObject.GetComponent<MeshRenderer>().material = material;

            ExportMesh(tempGameObject, meshName, addToScene, optimizeMesh);
            Debug.Log($"Mesh {meshName} saved correctly.");
            DestroyImmediate(tempGameObject);
        }
        /// <summary>
        /// Bake skinned mesh.
        /// </summary>
        private void BakeSkinnedMesh()
        {
            bakedMesh = new Mesh();
            var rootObject = Instantiate(skinnedMesh, Vector3.zero, Quaternion.identity);
            rootObject.name = "tempObj";
            var tempSkinned = rootObject.GetComponent<SkinnedMeshRenderer>();
            
            tempSkinned.transform.rotation = Quaternion.identity;
            tempSkinned.BakeMesh(bakedMesh, true);
            //skinnedMesh.BakeMesh(bakedMesh);


            //Material material = skinnedMesh.sharedMaterial;
            Material material = tempSkinned.sharedMaterial;

            GameObject tempGameObject = new();
            tempGameObject.name = meshName;
            
            tempGameObject.AddComponent<MeshFilter>();
            tempGameObject.AddComponent<MeshRenderer>();
            tempGameObject.GetComponent<MeshFilter>().sharedMesh = bakedMesh;
            tempGameObject.GetComponent<MeshRenderer>().material = material;

            ExportMesh(tempGameObject, meshName, addToScene, optimizeMesh);
            Debug.Log($"Mesh {meshName} saved correctly.");
            DestroyImmediate(tempSkinned);
            DestroyImmediate(rootObject);
            DestroyImmediate(tempGameObject);
            DestroyImmediate(GameObject.Find("tempObj"));
            
            
        }

        /// <summary>
        /// Exports the mesh.
        /// </summary>
        /// <param name="go">The go.</param>
        /// <param name="name">The name.</param>
        /// <param name="makeNewInstance">If true, make new instance.</param>
        /// <param name="optimizeMesh">If true, optimize mesh.</param>
        private void ExportMesh(GameObject go, string name, bool makeNewInstance, bool optimizeMesh)
        {

            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "fbx");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetPhysicalPath(path);
            Debug.Log(Application.dataPath);
                        
            ExportBinaryFBX(path, go);
            AssetDatabase.Refresh();

            if (makeNewInstance)
            {
                GameObject gameobject = Instantiate(go, Vector3.zero, Quaternion.identity);       
                gameobject.name = name;
            }

        }

        /// <summary>
        /// Save the mesh.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        /// <param name="name">The name.</param>
        /// <param name="makeNewInstance">If true, make new instance.</param>
        /// <param name="optimizeMesh">If true, optimize mesh.</param>
        private void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
        {
            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);

            Mesh meshToSave = makeNewInstance ? UnityEngine.Object.Instantiate(mesh) as Mesh : mesh;

            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();

        }
        /// <summary>
        /// Exports binary FBX.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="singleObject">The single object.</param>
        private static void ExportBinaryFBX(string filePath, UnityEngine.Object singleObject)
        {
            // Find relevant internal types in Unity.Formats.Fbx.Editor assembly
            Type[] types = AppDomain.CurrentDomain.GetAssemblies().First(x => x.FullName == "Unity.Formats.Fbx.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").GetTypes();
            Type optionsInterfaceType = types.First(x => x.Name == "IExportOptions");
            Type optionsType = types.First(x => x.Name == "ExportOptionsSettingsSerializeBase");
            Type optionsPositionType = types.First(x => x.Name == "ExportModelSettingsSerialize");
            // Instantiate a settings object instance
            MethodInfo optionsProperty = typeof(ModelExporter).GetProperty("DefaultOptions", BindingFlags.Static | BindingFlags.NonPublic).GetGetMethod(true);

            object optionsInstance = optionsProperty.Invoke(null, null);
            FieldInfo exportPositionField = optionsPositionType.GetField("objectPosition", BindingFlags.Instance | BindingFlags.NonPublic);
            exportPositionField.SetValue(optionsInstance, 1);

            // Change the export setting from ASCII to binary
            FieldInfo exportFormatField = optionsType.GetField("exportFormat", BindingFlags.Instance | BindingFlags.NonPublic);
            exportFormatField.SetValue(optionsInstance, 1);

            // Invoke the ExportObject method with the settings param
            MethodInfo exportObjectMethod = typeof(ModelExporter).GetMethod("ExportObject", BindingFlags.Static | BindingFlags.NonPublic, Type.DefaultBinder, new Type[] { typeof(string), typeof(UnityEngine.Object), optionsInterfaceType }, null);
            exportObjectMethod.Invoke(null, new object[] { filePath, singleObject, optionsInstance });
        }
    }

}
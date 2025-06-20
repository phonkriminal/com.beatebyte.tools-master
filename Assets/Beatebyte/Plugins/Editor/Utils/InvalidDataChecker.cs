using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace BeatebyteToolsEditor
{
    [InitializeOnLoad]
    public class InvalidDataChecker
    {
        private static string[] m_invalidData = {   "674ea7bed6b1cd94b8057074298096db", //"/Samples",
													"2738539936eacef409be91f148b2a4a0", //"/Resources",
													"c880e50f07f2be9499d414ac6f9f3a7a", //"/Templates",
													"563f992b9989cf547ac59bf748442c17"};//"/Textures"};
                                                                                        //private static string m_ASEFolderPath;
        private static string m_invalidDataCollected = string.Empty;
        static InvalidDataChecker()
        {
            bool foundInvalidData = false;
            //m_ASEFolderPath = AssetDatabase.GUIDToAssetPath( IOUtils.ASEFolderGUID );
            int count = 0;
            for (int i = 0; i < m_invalidData.Length; i++)
            {
                //m_invalidData[ i ] = m_ASEFolderPath + m_invalidData[ i ];
                m_invalidData[i] = AssetDatabase.GUIDToAssetPath(m_invalidData[i]);
                if (AssetDatabase.IsValidFolder(m_invalidData[i]))
                {
                    foundInvalidData = true;
                    m_invalidDataCollected += m_invalidData[i] + "\n";
                    count += 1;
                }
            }
            if (count < 5)
            {
                for (; count < 5; count++)
                {
                    m_invalidDataCollected += "\n";
                }
            }

            if (foundInvalidData)
            {
                InvalidDataPopUp window = (InvalidDataPopUp)EditorWindow.GetWindow(typeof(InvalidDataPopUp), true, "Found Invalid Data");
                window.minSize = new Vector2(502, 265);
                window.maxSize = new Vector2(502, 265);
                window.Show();
            }

            EditorApplication.update += Update;
            EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
        }

        static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
        {
            if (property.serializedObject.targetObject.GetType() == typeof(MeshFilter) | property.serializedObject.targetObject.GetType() == typeof(SkinnedMeshRenderer))
            {
                Debug.Log(property.serializedObject.targetObject.GetType());
                var propertyCopy = property.Copy();

                menu.AddItem(new GUIContent("Open Mesh Baker"), false, () =>
                {
                    SkinnedMeshBakerEditor.ShowWindow();
                });

            }

        }
        static void Update()
        {
            EditorApplication.update -= Update;

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Preferences.ShowOption show = Preferences.ShowOption.Never;
                if (!EditorPrefs.HasKey(Preferences.PrefStartUp))
                {
                    show = Preferences.ShowOption.Always;
                    EditorPrefs.SetInt(Preferences.PrefStartUp, 0);
                }
                else
                {
                    if (Time.realtimeSinceStartup < 10)
                    {
                        show = (Preferences.ShowOption)EditorPrefs.GetInt(Preferences.PrefStartUp, 0);
                        // check version here
                        if (show == Preferences.ShowOption.OnNewVersion)
                        {
                            BTEStartScreen.StartBackgroundTask(StartRequest(BTEStartScreen.ChangelogURL, () =>
                            {
                                var changeLog = ChangeLogInfo.CreateFromJSON(www.downloadHandler.text);
                                if (changeLog != null)
                                {
                                    if (changeLog.Version > VersionInfo.FullNumber)
                                        BTEStartScreen.Init();
                                }
                            }));
                        }
                    }
                }

                if (show == Preferences.ShowOption.Always)
                    BTEStartScreen.Init();
            }

        }
        static UnityWebRequest www;

        static IEnumerator StartRequest(string url, Action success = null)
        {
            using (www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                while (www.isDone == false)
                    yield return null;

                if (success != null)
                    success();
            }
        }
        public static void CleanInvalidData()
        {
            for (int i = 0; i < m_invalidData.Length; i++)
            {
                if (FileUtil.DeleteFileOrDirectory(m_invalidData[i]))
                {
                    Debug.Log("Removed invalid " + m_invalidData[i]);
                    if (FileUtil.DeleteFileOrDirectory(m_invalidData[i] + ".meta"))
                    {
                        Debug.Log("Removed invalid " + m_invalidData[i] + ".meta");
                    }
                }
            }
            AssetDatabase.Refresh();
        }
        public static string InvalidDataCollected { get { return m_invalidDataCollected; } }
    }
    
    public class InvalidDataPopUp : EditorWindow
    {
        private readonly GUIContent m_buttonContent = new GUIContent("Remove Invalid Data");
        private Vector2 m_scrollPosition = Vector2.zero;
        public void OnGUI()
        {
            GUILayout.BeginVertical();
            {
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.wordWrap = true;
                GUILayout.Label("\nBeatebyte Editor Tools " + VersionInfo.StaticToString(), labelStyle, GUILayout.ExpandWidth(true));
                GUILayout.Space(5);
                GUILayout.Label("Invalid/Legacy Data was found on your previous BTE folder which needs to be removed in order for it to work correctly.", labelStyle, GUILayout.ExpandWidth(true));
                GUILayout.Space(5);
                GUILayout.Label("Below are the detected files/folders which require to be removed.", labelStyle, GUILayout.ExpandWidth(true));
                GUILayout.Space(5);

                m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.Height(85));

                GUILayout.TextArea(InvalidDataChecker.InvalidDataCollected);
                GUILayout.EndScrollView();


                GUILayout.Label("VERY IMPORTANT: If you have assets of yours inside these folders you need to move them to another location before hitting the button below or they will be PERMANENTLY DELETED", labelStyle, GUILayout.ExpandWidth(true));
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(151);
                    if (GUILayout.Button(m_buttonContent, GUILayout.Width(200)))
                    {
                        InvalidDataChecker.CleanInvalidData();
                        Close();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

        }
    }
}
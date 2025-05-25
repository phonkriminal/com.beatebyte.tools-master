//  Beatebyte Editor Tools and Utilities - Gamekit for Developers Tools
//  Copyright (c) Beatebyte Creations, <info@beatebyte.com>

using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEditor;
using UnityEngine;


namespace BeatebyteToolsEditor
{
    public class BTEStartScreen : EditorWindow
    {
        [MenuItem("Window/Beatebyte Creations/Start Screen", false, 1999)]
        public static void Init() 
        { 
            BTEStartScreen window = (BTEStartScreen)GetWindow(typeof(BTEStartScreen), true, "Beatebyte Editor Tools Start Screen");
            window.minSize = new Vector2(650, 500);
            window.maxSize = new Vector2(650, 500);
            window.Show();
        }
        
        private static readonly string IconGUID = "643fff9a5f3ccf94aa0a9320c6ca2dba";
        private static readonly string ChangeLogGUID = "e52c83e772ce9fb4e879ba0dccd73e44";
        

        //private static readonly string ResourcesGUID = "c0a0a980c9ba86345bc15411db88d34f";

        public static readonly string ChangelogURL = "https://github.com/phonkriminal/com.edeastudio.tools/main/BTEchangelog.json";
        private static readonly string ManualURL = "https://github.com/phonkriminal/com.edeastudio.tools/wiki/Quick-Start";


        private static readonly string SiteURL = "https://github.com/phonkriminal/com.edeastudio.tools";

        private static readonly GUIContent UpdateTitle = new GUIContent("Latest Update", "Check the lastest additions, improvements and bug fixes done to BTE");
        private static readonly GUIContent BTETitle = new GUIContent("Beatebyte Editor Tools", "Are you using the latest version? Now you know");
        private static readonly GUIContent ResourcesTitle = new GUIContent("Learning Resources", "Check the online wiki for various topics about how to use BTE with examples and explanations");


        private const string OnlineVersionWarning = "Please enable \"Allow downloads over HTTP*\" in Player Settings to access latest version information via Start Screen.";

        Vector2 m_scrollPosition = Vector2.zero;
        Preferences.ShowOption m_startup = Preferences.ShowOption.Never;
        
        [NonSerialized]
        Texture packageIcon = null;
        [NonSerialized]
        Texture textIcon = null;
        [NonSerialized]
        Texture webIcon = null;

        GUIContent URPbutton = null;
        GUIContent BuiltInbutton = null;

        GUIContent Manualbutton = null;
        GUIContent Basicbutton = null;
        GUIContent APIbutton = null;


        GUIContent BTEIcon = null;
        RenderTexture rt;

        [NonSerialized]
        GUIStyle m_buttonStyle = null;
        [NonSerialized]
        GUIStyle m_buttonLeftStyle = null;
        [NonSerialized]
        GUIStyle m_buttonRightStyle = null;
        [NonSerialized]
        GUIStyle m_minibuttonStyle = null;
        [NonSerialized]
        GUIStyle m_labelStyle = null;
        [NonSerialized]
        GUIStyle m_linkStyle = null;
        
        private ChangeLogInfo m_changeLog;
        private bool m_infoDownloaded = false;
        private string m_newVersion = string.Empty;

        private void OnEnable()
        {
            rt = new RenderTexture(16, 16, 0);
            rt.Create();

            m_startup = (Preferences.ShowOption)EditorPrefs.GetInt(Preferences.PrefStartUp, 0);
            if (textIcon == null)
            {
                Texture icon = EditorGUIUtility.IconContent("TextAsset Icon").image;
                var cache = RenderTexture.active;
                RenderTexture.active = rt;
                Graphics.Blit(icon, rt);
                RenderTexture.active = cache;
                textIcon = rt;

                Manualbutton = new GUIContent(" Manual", textIcon);
                Basicbutton = new GUIContent(" Basic use tutorials", textIcon);
                APIbutton = new GUIContent(" Node API", textIcon);
            }
            if (packageIcon == null)
            {
                packageIcon = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image;
                URPbutton = new GUIContent(" URP Samples", packageIcon);
                BuiltInbutton = new GUIContent(" Built-In Samples", packageIcon);
            }

            if (webIcon == null)
            {
                webIcon = EditorGUIUtility.IconContent("BuildSettings.Web.Small").image;
            }

            if (m_changeLog == null)
            {

                var changelog = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(ChangeLogGUID));
                string lastUpdate = string.Empty;
                if (changelog != null)
                {
                    int oldestReleaseIndex = changelog.text.LastIndexOf(string.Format("v{0}.{1}.{2}", VersionInfo.Major, VersionInfo.Minor, VersionInfo.Release));
                    lastUpdate = changelog.text.Substring(0, changelog.text.IndexOf("\nv", oldestReleaseIndex + 25));// + "\n...";
                    lastUpdate = lastUpdate.Replace("* ", "\u2022 ");
                }
                m_changeLog = new ChangeLogInfo(VersionInfo.FullNumber, lastUpdate);
            }

            if (BTEIcon == null)
            {
                BTEIcon = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(IconGUID)));
            }
        }
        private void OnDisable()
        {
            if (rt != null)
            {
                rt.Release();
                DestroyImmediate(rt);
            }
        }

        private void OnGUI()
        {
            if (!m_infoDownloaded)
            {
                m_infoDownloaded = true;
                StartBackgroundTask(StartRequest (ChangelogURL, () => 
                {

                    var temp = ChangeLogInfo.CreateFromJSON(www.downloadHandler.text);
                    Debug.Log(temp.Version + " " + temp.LastUpdate);

                    if (temp != null && temp.Version >= m_changeLog.Version)
                    {
                        m_changeLog = temp;
                    }

                    int version = m_changeLog.Version;
                    int major = version / 10000;
                    int minor = version / 1000 - major * 10;
                    int release = version / 100 - (version / 1000) * 10;
                    int revision = version - (version / 100) * 100;

                    m_newVersion = major + "." + minor + "." + release + (revision > 0 ? "." + revision : "");

                    Debug.Log(m_newVersion);

                    Repaint();

                }));
            }

            if (m_buttonStyle == null)
            {
                m_buttonStyle = new GUIStyle(GUI.skin.button);
                m_buttonStyle.alignment = TextAnchor.MiddleLeft;
            }

            if (m_buttonLeftStyle == null)
            {
                m_buttonLeftStyle = new GUIStyle("ButtonLeft");
                m_buttonLeftStyle.alignment = TextAnchor.MiddleLeft;
                m_buttonLeftStyle.margin = m_buttonStyle.margin;
                m_buttonLeftStyle.margin.right = 0;
            }

            if (m_buttonRightStyle == null)
            {
                m_buttonRightStyle = new GUIStyle("ButtonRight");
                m_buttonRightStyle.alignment = TextAnchor.MiddleLeft;
                m_buttonRightStyle.margin = m_buttonStyle.margin;
                m_buttonRightStyle.margin.left = 0;
            }

            if (m_minibuttonStyle == null)
            {
                m_minibuttonStyle = new GUIStyle("MiniButton");
                m_minibuttonStyle.alignment = TextAnchor.MiddleLeft;
                m_minibuttonStyle.margin = m_buttonStyle.margin;
                m_minibuttonStyle.margin.left = 20;
                m_minibuttonStyle.normal.textColor = m_buttonStyle.normal.textColor;
                m_minibuttonStyle.hover.textColor = m_buttonStyle.hover.textColor;
            }

            if (m_labelStyle == null)
            {
                m_labelStyle = new GUIStyle("BoldLabel");
                m_labelStyle.margin = new RectOffset(4, 4, 4, 4);
                m_labelStyle.padding = new RectOffset(2, 2, 2, 2);
                m_labelStyle.fontSize = 13;
            }

            if (m_linkStyle == null)
            {
                var inv = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath("1004d06b4b28f5943abdf2313a22790a")); // find a better solution for transparent buttons
                m_linkStyle = new GUIStyle();
                m_linkStyle.normal.textColor = new Color(0.2980392f, 0.4901961f, 1f);
                m_linkStyle.hover.textColor = Color.white;
                m_linkStyle.active.textColor = Color.grey;
                m_linkStyle.margin.top = 3;
                m_linkStyle.margin.bottom = 2;
                m_linkStyle.hover.background = inv;
                m_linkStyle.active.background = inv;
            }
            EditorGUILayout.BeginHorizontal(GUIStyle.none, GUILayout.ExpandWidth(true));
            {
                //  Left Column
                EditorGUILayout.BeginVertical(GUILayout.Width(175));
                {

                    GUILayout.Label(ResourcesTitle, m_labelStyle);
                    if (GUILayout.Button(Manualbutton, m_buttonStyle))
                        Application.OpenURL(ManualURL);


                    GUILayout.Label("TEST", m_labelStyle);
                    if (GUILayout.Button("TEST", m_buttonStyle))
                    {
                        StartBackgroundTask(StartRequest(ChangelogURL, () =>
                        {

                            var temp = ChangeLogInfo.CreateFromJSON(www.downloadHandler.text);
                            Debug.Log(temp.Version + " " + temp.LastUpdate);

                            if (temp != null && temp.Version >= m_changeLog.Version)
                            {
                                Debug.Log("NOT NULL");

                                m_changeLog = temp;
                            }

                            int version = m_changeLog.Version;
                            int major = version / 10000;
                            int minor = version / 1000 - major * 10;
                            int release = version / 100 - (version / 1000) * 10;
                            int revision = version - (version / 100) * 100;

                            m_newVersion = major + "." + minor + "." + release + (revision > 0 ? "." + revision : "");

                            Debug.Log(m_newVersion);

                            Repaint();

                        }));
                    }

                }
                EditorGUILayout.EndVertical();
                // Right Column
                EditorGUILayout.BeginVertical(GUILayout.Width(650 - 175 - 9), GUILayout.ExpandHeight(true));
                {
                        GUILayout.Label(UpdateTitle, m_labelStyle);
                        m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, "ProgressBarBack", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                        GUILayout.Label(m_changeLog.LastUpdate, "WordWrappedMiniLabel", GUILayout.ExpandHeight(true));
                        GUILayout.EndScrollView();

                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    {
                        EditorGUILayout.BeginVertical();
                        GUILayout.Label(BTETitle, m_labelStyle);

                        GUILayout.Label("Installed Version: " + VersionInfo.StaticToString());

                        Debug.Log("CHECK " + (m_changeLog.Version > VersionInfo.FullNumber) + m_changeLog.Version + " " + VersionInfo.FullNumber);
                        if (m_changeLog.Version > VersionInfo.FullNumber)
                        {
                            var cache = GUI.color;
                            GUI.color = Color.red;
                            GUILayout.Label("New version available: " + m_newVersion, "BoldLabel");
                            GUI.color = cache;
                        }
                        else
                        {
                            var cache = GUI.color;
                            GUI.color = Color.green;
                            GUILayout.Label("You are using the latest version", "BoldLabel");
                            GUI.color = cache;
                        }

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Download links:");
                        if (GUILayout.Button("Beatebyte", m_linkStyle))
                            Application.OpenURL(SiteURL);
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(7);
                        EditorGUILayout.EndVertical();

                        GUILayout.FlexibleSpace();
                        EditorGUILayout.BeginVertical();
                        GUILayout.Space(7);
                        GUILayout.Label(BTEIcon);
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal("ProjectBrowserBottomBarBg", GUILayout.ExpandWidth(true), GUILayout.Height(22));
            {
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                var cache = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 100;
                m_startup = (Preferences.ShowOption)EditorGUILayout.EnumPopup("Show At Startup", m_startup, GUILayout.Width(220));
                EditorGUIUtility.labelWidth = cache;
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(Preferences.PrefStartUp, (int)m_startup);
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }

        UnityWebRequest www;

        IEnumerator StartRequest(string url, Action success = null)
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

        public static void StartBackgroundTask(IEnumerator update, Action end = null)
        {
            EditorApplication.CallbackFunction closureCallback = null;

            closureCallback = () =>
            {
                try
                {
                    if (update.MoveNext() == false)
                    {
                        if (end != null)
                            end();
                        EditorApplication.update -= closureCallback;
                    }
                }
                catch (Exception ex)
                {
                    if (end != null)
                        end();
                    Debug.LogException(ex);
                    EditorApplication.update -= closureCallback;
                }
            };

            EditorApplication.update += closureCallback;
        }
    }


    [Serializable]
    internal class ChangeLogInfo
    {
        public int Version;
        public string LastUpdate;

        public static ChangeLogInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ChangeLogInfo>(jsonString);
        }

        public ChangeLogInfo(int version, string lastUpdate)
        {
            Version = version;
            LastUpdate = lastUpdate;
        }
    }
}
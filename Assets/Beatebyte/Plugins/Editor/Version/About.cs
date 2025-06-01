//  Beatebyte Editor Tools and Utilities - Gamekit for Developers Tools
//  Copyright (c) Beatebyte Creations, <info@beatebyte.com>using System.Collections;

using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor
{

    public class About : EditorWindow
    {
        private const string AboutImageGUID = "f54da05d2f06abd43baa280725ebb28c";
        private Vector2 m_scrollPosition = Vector2.zero;
        private Texture2D m_aboutImage;


        [MenuItem("Window/Beatebyte Creations/About...", false, 2001)]
        static void Init()
        {
            About window = (About)GetWindow(typeof(About), true, "About Beatebyte Editor Tools");
            window.minSize = new Vector2(502, 290);
            window.maxSize = new Vector2(502, 290);
            window.Show();
        }
        [MenuItem("Window/Beatebyte Creations/Manual", false, 2000)]
        static void OpenManual()
        {
            Application.OpenURL("http://beatebyte.com/wiki/Quick-Start.php");
        }
        private void OnEnable()
        {
            m_aboutImage = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(AboutImageGUID));
        }

        public void OnGUI()
        {
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);

            GUILayout.BeginVertical();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(m_aboutImage, GUIStyle.none);

            if (Event.current.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                Application.OpenURL("https://github.com/phonkriminal/com.edeastudio.tools");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.wordWrap = true;

            GUILayout.Label("\nBeatebyte Editor Tools " + VersionInfo.StaticToString(), labelStyle, GUILayout.ExpandWidth(true));

            GUILayout.Label("\nCopyright (c) Beatebyte Creations, All rights reserved.\n", labelStyle, GUILayout.ExpandWidth(true));

            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }
    }

}
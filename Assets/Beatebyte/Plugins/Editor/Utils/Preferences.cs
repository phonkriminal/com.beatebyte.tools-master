//  Beatebyte Editor Tools and Utilities - Gamekit for Developers Tools
//  Copyright (c) Beatebyte Creations, <info@beatebyte.com>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BeatebyteToolsEditor
{
    public class Preferences
    {
        public enum ShowOption
        {
            Always = 0,
            OnNewVersion = 1,
            Never = 2
        }


        private static readonly GUIContent StartUp = new GUIContent("Show start screen on Unity launch", "You can set if you want to see the start screen everytime Unity launchs, only just when there's a new version available or never.");
        public static readonly string PrefStartUp = "BTELastSession";

        public static ShowOption GlobalStartUp { get; private set; } = 0;

        private static readonly GUIContent DefineSymbol = new GUIContent("Add Beatebyte Editor Tools define symbol", "Turning it OFF will disable the automatic insertion of the define symbol and remove it from the list while turning it ON will do the opposite.\nThis is used for compatibility with other plugins, if you are not sure if you need this leave it ON.");
        public static readonly string PrefDefineSymbol = "BTEDefineSymbol";
        public static bool GlobalDefineSymbol { get; private set; } = true;

        [SettingsProvider]
        public static SettingsProvider EditorToolsSettings()
        {
            var provider = new SettingsProvider("Preferences/Beatebyte Editor Tools", SettingsScope.User)
            {
                guiHandler = (string searchContext) =>
                {
                    PreferencesGUI();
                },

                keywords = new HashSet<string>(new[] { "start", "screen", "import", "templates", "define", "symbol", "tools" }),
            };

            return provider;
        }

        private static void ResetSettings()
        {
            EditorPrefs.DeleteKey(PrefStartUp);
            EditorPrefs.DeleteKey(PrefDefineSymbol);
        }

        private static void LoadSettings()
        {
            GlobalStartUp = (ShowOption)EditorPrefs.GetInt(PrefStartUp, 0);
            GlobalDefineSymbol = EditorPrefs.GetBool(PrefDefineSymbol, true);
        }

        private static void SaveSettings()
        {
            bool prevDefineSymbol = EditorPrefs.GetBool(PrefDefineSymbol, true);

            if (GlobalDefineSymbol != prevDefineSymbol)
            {
                if (GlobalDefineSymbol)
                {
//                    IOUtils.SetAmplifyDefineSymbolOnBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                }
                else
                {
  //                  IOUtils.RemoveAmplifyDefineSymbolOnBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                }
            }


            EditorPrefs.SetInt(PrefStartUp, (int)GlobalStartUp);
            EditorPrefs.SetBool(PrefDefineSymbol, GlobalDefineSymbol);
        }

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            LoadSettings();
        }

        public static void PreferencesGUI()
        {
            var cache = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 250;

            EditorGUI.BeginChangeCheck();
            {
                GlobalStartUp = (ShowOption)EditorGUILayout.EnumPopup(StartUp, GlobalStartUp);
                GlobalDefineSymbol = EditorGUILayout.Toggle(DefineSymbol, GlobalDefineSymbol);
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveSettings();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if ( GUILayout.Button("Reset and Forget All"))
            {
                ResetSettings();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = cache;
        }
    }
}
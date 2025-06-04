using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace BeatebyteToolsEditor
{
    public class ScriptingDefineSymbolIntegration
    {
        public static readonly string[] Symbols = new string[] {
                "BEATEBYTE",
                "BEATEBYTETOOLSEDITOR",
                "C_RENAME_IS_PRESENT"
            };

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
#if BEATEBYTE && BEATEBYTETOOLSEDITOR && C_RENAME_IS_PRESENT

            //Debug.Log("ASSEMBLY SCRIPT DEFINITION Is Present.");


#elif !BEATEBYTE || !BEATEBYTETOOLSEDITOR || !C_RENAME_IS_PRESENT
            
            try
            {
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, Symbols);
                Debug.Log("ASSEMBLY SCRIPT DEFINITION SUCCESFULLY ADDED.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Script Define Symbols failed: {e.Message}");
            }
#endif
        }
    }

}
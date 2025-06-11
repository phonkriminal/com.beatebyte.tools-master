using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BeatebyteToolsEditor.Editor
{

    public class AudioSFXGameobject : MonoBehaviour
    {
        [MenuItem("GameObject/Beatebyte/AudioSFX Player", false, 10)]
        static void CreateAudioSFXPlayerGameObject(MenuCommand menuCommand)
        {
            GameObject go = new("Audio SFX Player");
            go.AddComponent<Components.AudioSFXPlayer>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }

    }

}
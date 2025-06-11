using System.Collections.Generic;
using UnityEngine;

namespace BeatebyteToolsEditor.Runtime
{

    [CreateAssetMenu(fileName = "NewFXAudioList", menuName = "Beatebyte/FX Audio List", order = 1)]
    public class AudioFXList : ScriptableObject
    {
        public List<AudioElement> audioElements = new List<AudioElement>();

    }

    [System.Serializable]
    public class AudioElement
    {

        public AudioCategory category;

        public string name;

        public AudioClip clip;

    }


    [System.Serializable]
    public enum AudioCategory
    {
        UI, FX, Music, Other
    }


}
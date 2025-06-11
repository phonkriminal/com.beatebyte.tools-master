using BeatebyteToolsEditor.Attributes;
using BeatebyteToolsEditor.Utils;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace BeatebyteToolsEditor.Components
{
    [AddComponentMenu("Beatebyte/Sound Looping")]
    [eClassHeader("     Sound Looping", iconName = "icon_v2")]
    [ExecuteAlways]

    public class bteSoundLooping : eMonoBehaviour
    {
        //  private static readonly string AudioSourcePrefabGUID = "bc6a7e4a1b4b3944e9b390fad690206a";
        /// <summary>
        /// The audio source.
        /// </summary>
        [eEditorToolbar("Audio Component")]
        [Space(10)]
        [eButton("Start Loop Function", "StartEditorLoop", typeof(bteSoundLooping), enabledJustInPlayMode: false)]
        [Space(10)]
        [SerializeField]
        private AudioSource audioSource;
        /// <summary>
        /// The audio source prefab.
        /// </summary>
        [SerializeField]
        private GameObject audioSourcePrefab;
        /// <summary>
        /// The audio source object.
        /// </summary>
        private GameObject audioSourceObject;

        private bool isEditorLooping = false;

        /// <summary>
        /// Gets or sets a value indicating whether the audio or media playback starts automatically  when the object is
        /// initialized.
        /// </summary>
        
        [field: SerializeField, Tooltip("Starts Playing on Start Function only Player mode.")]        
        public bool PlayingOnAwake { get; set; } = false;
        /// <summary>
        /// The audio clips.
        /// </summary>
        [eEditorToolbar("Audio Clips")]
        [Space(10)]
        [SerializeField]
        private List<AudioClip> audioClips;
        /// <summary>
        /// Start time.
        /// </summary>
        [SerializeField]
        private List<float> startTime;

        /// <summary>
        /// The volume.
        /// </summary>
        [eEditorToolbar("Audio Source")]
        [Space(10)]

        [Range(0.1f, 1f)]
        [SerializeField]
        private float _volume = 0.5f;

        /// <summary>
        /// The pitch.
        /// </summary>
        [SerializeField]
        [Range(-2f, 2f)]
        private float _pitch = 1f;

        /// <summary>
        /// The spatial blend.
        /// </summary>
        [SerializeField]
        [Range(0, 1)]
        private float _spatialBlend = 0.5f;

        /// <summary>
        /// Time to trigger.
        /// </summary>
        [SerializeField]
        [Range(0f, 10f)]
        private float _timeToTrigger;

        /// <summary>
        /// The audio lenght.
        /// </summary>
        private float audioLenght;

        /// <summary>
        /// Play loop.
        /// </summary>
        private Coroutine PlayLoop;

        private Coroutine EditorPlayLoop = null;

        /// <summary>
        /// Repeat.
        /// </summary>
        private bool repeat = false;

        private void Start()
        {
            if (Application.isPlaying && PlayingOnAwake && audioSource) Invoke("StartLoop", _timeToTrigger);

            if (audioClips ==  null && audioClips.Count == 0) { return; }
            for (int i = 0; i < audioClips.Count; i++)
            {
                audioLenght += audioClips[i].length;
            }
            repeat = false;
        }

        private void OnEnable()
        {

            if (!audioSource && !audioSourcePrefab)
            {
                gameObject.AddComponent<AudioSource>();
                audioSource = GetComponent<AudioSource>();
            }
            else if (!audioSource && audioSourcePrefab)
            {
                audioSourceObject = Instantiate(audioSourcePrefab);
                audioSourceObject.name = "AudioSourceObject";
                audioSource = audioSourceObject.GetComponent<AudioSource>();
            }
#if UNITY_EDITOR
            EditorApplication.update += Update;
#endif
        }

        private void OnDisable()
        {

            if (Application.isPlaying)
            {
                if (audioSourceObject != null)
                {
                    Destroy(GameObject.Find(audioSourceObject.name));
                    audioSource = null;
                }
                else
                {
                    if (audioSource != null)
                    {
                        Destroy(audioSource);
                        audioSource = null;
                    }
                }
            }

#if UNITY_EDITOR
            EditorApplication.update -= Update;
            if (audioSourceObject != null)
            {
                DestroyImmediate(GameObject.Find(audioSourceObject.name));
                audioSource = null;
            }
            else
            {
                if (audioSource != null)
                {
                    DestroyImmediate(audioSource);
                    audioSource = null;
                }
            }
#endif
        }



        private void InitAudio()
        {
            if (audioSource == null) return;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.pitch = _pitch;
            audioSource.spatialBlend = _spatialBlend;
            repeat = false;
        }
        public void StartEditorLoop()
        {
            isEditorLooping = !isEditorLooping;
            repeat = true;
        }
        private void StartLoop()
        {
            PlayLoop = StartCoroutine(PlayLoopChain());
        }
        private IEnumerator PlayLoopChain()
        {
            repeat = false;            

            for (int i = 0; i < audioClips.Count; i++)
            {
                InitAudio();
                float timeToDestroy = audioClips[i].length;
                yield return new WaitForSeconds(startTime[i]);
                audioSource.PlayOneShot(audioClips[i], _volume);
            }            

            repeat = true;
        }

        private void Update()
        {
            if (Application.isPlaying && repeat)
            {
                if (PlayLoop != null)
                {
                    StopCoroutine(PlayLoop);
                    PlayLoop = null;
                }

                PlayLoop = StartCoroutine(PlayLoopChain());
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (isEditorLooping && repeat)
                {
                    if (EditorPlayLoop != null)
                    {
                        StopCoroutine(EditorPlayLoop);
                        EditorPlayLoop = null;
                    }
                    EditorPlayLoop = StartCoroutine(PlayLoopChain());
                }
                else if (!isEditorLooping)
                {
                    if (EditorPlayLoop != null)
                    {
                        StopCoroutine(PlayLoopChain());
                        EditorPlayLoop = null;
                    }
                }
                if (EditorApplication.isPlaying)
                {
                    Debug.Log("App is playing");
                }
                else
                {
                    EditorApplication.QueuePlayerLoopUpdate();
                    SceneView.RepaintAll();
                }
            }
#endif

        }

    }
}

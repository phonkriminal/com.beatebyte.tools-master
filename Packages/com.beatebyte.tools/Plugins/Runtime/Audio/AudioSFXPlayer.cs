using BeatebyteToolsEditor.Attributes;
using BeatebyteToolsEditor.Utils;
using BeatebyteToolsEditor.Runtime;
using System.Linq;
using UnityEngine;

namespace BeatebyteToolsEditor.Components
{
    /// <summary>
    /// The audio SFX player.
    /// </summary>
    [eClassHeader("Audio SFX Player", iconName = "icon_v2")]
    public class AudioSFXPlayer : eMonoBehaviour
    {
        /// <summary>
        /// The audio list.
        /// </summary>
        [Space(10)]
        [SerializeField]
        private AudioFXList audioList;
        /// <summary>
        /// The instance.
        /// </summary>
        private static AudioSFXPlayer _instance;
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static AudioSFXPlayer Instance { get => _instance; }
        /// <summary>
        /// Gets or sets the array of <see cref="AudioSource"/> objects.
        /// </summary>
        [field: SerializeField, Tooltip("AudioSources Array List")]
        public AudioSource[] audioSources { get; set; } = new AudioSource[0];

        private readonly string[] componentName = { "Common", "UI", "FX", "Music", "Other" };

        /// <summary>
        /// The audio source.
        /// </summary>
        private AudioSource audioSource;
        private AudioSource audioSourceUI;
        private AudioSource audioSourceFX;
        private AudioSource audioSourceMusic;
        private AudioSource audioSourceOther;

        [field: SerializeField, Tooltip("Mantain the instance after change scene as well")]
        public new bool  DontDestroyOnLoad { get; set; } = true;

        private void OnValidate()
        {
            InitArray();
        }
        private void OnEnable()
        {
            InitArray();
        }
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                _instance = null;
                return;
            }

            _instance = this;
           
            if (Application.isPlaying && DontDestroyOnLoad) DontDestroyOnLoad(gameObject);            
        }

        /// <summary>
        /// Stops the currently playing music if any is active.
        /// </summary>
        /// <remarks>This method checks if a music audio source is available and currently playing. If so,
        /// it stops the playback. If no music is playing, the method has no effect.</remarks>
        public void StopMusic()
        {
            if (audioSourceMusic && audioSourceMusic.isPlaying)
            {
                audioSourceMusic.Stop();
            }
        }
        /// <summary>
        /// Starts playback of the music clip assigned to the audio source.
        /// </summary>
        /// <remarks>This method plays the music clip if the <see cref="audioSourceMusic"/> is assigned
        /// and has a valid clip. If no clip is assigned, the method does nothing.</remarks>
        public void StartMusic()
        {
            if (audioSourceMusic && audioSourceMusic.clip)
            {
                audioSourceMusic.Play();
            }
        }
        /// <summary>
        /// Plays the specified audio clip once using an auxiliary audio source.
        /// </summary>
        /// <remarks>If the auxiliary audio source is unavailable, the method does nothing. Ensure the
        /// <paramref name="clip"/> parameter is not null to avoid silent failure.</remarks>
        /// <param name="clip">The audio clip to play. Must not be null.</param>
        public void PlayOneShot(AudioClip clip)
        {
            if (audioSourceOther == null) { return; }

            if (clip != null) audioSourceOther.PlayOneShot(clip);
        }
        /// <summary>
        /// Plays the specified audio clip on an alternate audio source.
        /// </summary>
        /// <remarks>If the alternate audio source is not initialized, the method does nothing. The method
        /// stops any currently playing audio on the alternate audio source before starting the new clip.</remarks>
        /// <param name="clip">The audio clip to play. Cannot be null.</param>
        /// <param name="loop">A value indicating whether the audio clip should loop. <see langword="true"/> to loop the clip; otherwise,
        /// <see langword="false"/>.</param>
        public void PlayOther(AudioClip clip, bool loop)
        {
            if (audioSourceOther == null) { return; }
            audioSourceOther.Stop();
            audioSourceOther.loop = loop;
            audioSourceOther.clip = clip;
            audioSourceOther.Play();
        }
        /// <summary>
        /// Plays a UI audio clip from the specified index in the audio list.
        /// </summary>
        /// <remarks>This method plays an audio clip categorized as "UI" from the audio list. If the audio
        /// list is null, empty, or the audio source is null, the method does nothing. Ensure the <paramref
        /// name="index"/> corresponds to a valid UI audio element in the list to avoid unexpected behavior.</remarks>
        /// <param name="index">The zero-based index of the UI audio clip to play. Must correspond to a valid UI audio element in the list.</param>
        /// <param name="loop">A value indicating whether the audio clip should loop. <see langword="true"/> to loop the clip; otherwise,
        /// <see langword="false"/>.</param>
        public void PlayUI(int index, bool loop)
        {
            if (audioList == null || audioList.audioElements.Count == 0 || audioSourceUI == null) { return; }
            AudioClip clip = audioList.audioElements.Where(x => x.category.ToString() == "UI").ElementAt(index).clip;

            if (clip != null)
            {
                audioSourceUI.loop = loop;
                audioSourceUI.clip = clip;
                audioSourceUI.Play();
            }
        }

        /// <summary>
        /// Plays a UI audio clip based on the specified name and loop setting.
        /// </summary>
        /// <remarks>This method plays an audio clip categorized as "UI" from the available audio elements. If no matching
        /// clip is found, or if the audio list or audio source is not properly initialized, the method does nothing.</remarks>
        /// <param name="defaultName">The name of the audio clip to play. Must match the name of a UI category audio element.</param>
        /// <param name="loop">A value indicating whether the audio clip should loop. <see langword="true"/> to loop the clip; otherwise, <see
        /// langword="false"/>.</param>
        public void PlayUI(string defaultName, bool loop)
        {
            if (audioList == null || audioList.audioElements.Count == 0 || audioSourceUI == null) { return; }

            AudioClip clip = audioList.audioElements.Where(x => x.category.ToString() == "UI").First(x => x.name == defaultName).clip;

            if (clip != null)
            {
                audioSourceUI.loop = loop;
                audioSourceUI.clip = clip;
                audioSourceUI.Play();
            }
        }

        /// <summary>
        /// Plays a sound effect from the audio list at the specified index.
        /// </summary>
        /// <remarks>This method plays a sound effect categorized as "FX" from the audio list. If the
        /// specified index is invalid, or if the audio list or audio source is not properly initialized, the method
        /// will return without playing any sound.</remarks>
        /// <param name="index">The zero-based index of the sound effect to play. Must correspond to an existing sound effect in the audio
        /// list.</param>
        /// <param name="loop">A value indicating whether the sound effect should loop. <see langword="true"/> to loop the sound effect;
        /// otherwise, <see langword="false"/>.</param>
        public void PlayFX(int index, bool loop)
        {
            if (audioList == null || audioList.audioElements.Count == 0 || audioSourceFX == null) { return; }
            AudioClip clip = audioList.audioElements.Where(x => x.category.ToString() == "FX").ElementAt(index).clip;

            if (clip != null)
            {
                audioSourceFX.loop = loop;
                audioSourceFX.clip = clip;
                audioSourceFX.Play();
            }
        }

        /// <summary>
        /// Plays a sound effect from the audio list based on the specified name and looping preference.
        /// </summary>
        /// <remarks>If the audio list is null, contains no audio elements, or the audio source for sound
        /// effects is null, the method will exit without playing any sound.</remarks>
        /// <param name="defaultName">The name of the sound effect to play. Must match the name of an audio element in the "FX" category.</param>
        /// <param name="loop">A value indicating whether the sound effect should loop. <see langword="true"/> to loop the sound effect;
        /// otherwise, <see langword="false"/>.</param>
        public void PlayFX(string defaultName, bool loop)
        {
            if (audioList == null || audioList.audioElements.Count == 0 || audioSourceFX == null) { return; }

            AudioClip clip = audioList.audioElements.Where(x => x.category.ToString() == "FX").First(x => x.name == defaultName).clip;

            if (clip != null)
            {
                audioSourceFX.loop = loop;
                audioSourceFX.clip = clip;
                audioSourceFX.Play();
            }
        }

        /// <summary>
        /// Plays a music track from the audio list at the specified index.
        /// </summary>
        /// <remarks>This method plays a music track categorized as "Music" from the audio list. If the
        /// specified index is invalid,  or if the audio list is empty, or if the audio source is not initialized, the
        /// method does nothing.</remarks>
        /// <param name="index">The zero-based index of the music track to play. Must correspond to a valid music track in the audio list.</param>
        /// <param name="loop">A value indicating whether the music track should loop continuously.  <see langword="true"/> to loop the
        /// track; otherwise, <see langword="false"/>.</param>
        public void PlayMusic(int index, bool loop)
        {
            if (audioList == null || audioList.audioElements.Count == 0 || audioSourceMusic == null) {return; }

            AudioClip clip = audioList.audioElements.Where(x => x.category.ToString() == "Music").ElementAt(index).clip;

            if (clip != null)
            {
                audioSourceMusic.Stop();
                audioSourceMusic.clip = clip;
                audioSourceMusic.loop = loop;
                audioSourceMusic.Play();
            }
        }

        /// <summary>
        /// Plays a music track specified by its name.
        /// </summary>
        /// <remarks>If the specified music track is not found or the required audio components are not
        /// properly initialized, the method will exit without playing any music.</remarks>
        /// <param name="defaultName">The name of the music track to play. Must match the name of an existing audio element in the music category.</param>
        /// <param name="loop">A value indicating whether the music track should loop continuously. <see langword="true"/> to loop the
        /// track; otherwise, <see langword="false"/>.</param>
        public void PlayMusic(string defaultName, bool loop)
        {
            if (audioList == null || audioList.audioElements.Count == 0 || audioSourceUI == null) { return; }

            AudioClip clip = audioList.audioElements.Where(x => x.category.ToString() == "Music").First(x => x.name == defaultName).clip;

            if (clip != null)
            { 
                audioSourceMusic.Stop();
                audioSourceMusic.clip = clip;
                audioSourceMusic.loop = loop;
                audioSourceMusic.Play();
            }
        }
        /// <summary>
        /// Plays an audio clip from the "Other" category based on the specified name.
        /// </summary>
        /// <remarks>If the audio list is null, contains no audio elements, or the audio source is
        /// unavailable, the method does nothing.</remarks>
        /// <param name="defaultName">The name of the audio clip to play. Must match the name of an audio clip in the "Other" category.</param>
        /// <param name="loop">A value indicating whether the audio clip should loop.  <see langword="true"/> to loop the clip; otherwise,
        /// <see langword="false"/>.</param>
        public void PlayOther(string defaultName, bool loop)
        {
            if (audioList == null || audioList.audioElements.Count == 0 || audioSourceOther == null) { return; }

            AudioClip clip = audioList.audioElements.Where(x => x.category.ToString() == "Other").First(x => x.name == defaultName).clip;

            if (clip != null)
            {
                audioSourceOther.loop = loop;
                audioSourceOther.clip = clip;
                audioSourceOther.Play();
            }
        }

        /// <summary>
        /// Plays an audio clip from the "Other" category at the specified index.
        /// </summary>
        /// <remarks>If the audio list is null, contains no elements, or the audio source is null, the
        /// method does nothing.</remarks>
        /// <param name="index">The zero-based index of the audio clip to play within the "Other" category.</param>
        /// <param name="loop">A value indicating whether the audio clip should loop.  <see langword="true"/> to loop the clip; otherwise,
        /// <see langword="false"/>.</param>
        public void PlayOther(int index, bool loop)
        {
            if (audioList == null || audioList.audioElements.Count == 0 || audioSourceOther == null) { return; }
            AudioClip clip = audioList.audioElements.Where(x => x.category.ToString() == "Other").ElementAt(index).clip;

            if (clip != null)
            {
                audioSourceOther.loop = loop;
                audioSourceOther.clip = clip;
                audioSourceOther.Play();
            }
        }

        /// <summary>
        /// Initializes the <see cref="audioSources"/> array and assigns individual <see cref="AudioSource"/> components
        /// to specific roles within the application.
        /// </summary>
        /// <remarks>If the <see cref="audioSources"/> array is empty, this method creates and adds new
        /// <see cref="AudioSource"/>  components to the current <see cref="GameObject"/>. The array is populated with
        /// five <see cref="AudioSource"/>  instances, each assigned to a specific role: general audio, UI audio, sound
        /// effects, music, and other audio.</remarks>
        private void InitArray()
        {
            if (audioSources.Length == 0)
            {
                int componentCount = transform.GetComponents<AudioSource>().Length;
                if (componentCount == 0)
                {
                    Debug.Log(0);
                    audioSources = new AudioSource[5];
                    for (int i = 0; i < 5; i++)
                    {
                        AudioSource source = new();
                        source = gameObject.AddComponent<AudioSource>();
                        audioSources[i] = source;
                    }
                }                
            }
            audioSource = audioSources[0];
            audioSourceUI = audioSources[1];
            audioSourceFX = audioSources[2];
            audioSourceMusic = audioSources[3];
            audioSourceOther = audioSources[4];
        }

        private void Start()
        {
            InitArray();
        }
    }

}
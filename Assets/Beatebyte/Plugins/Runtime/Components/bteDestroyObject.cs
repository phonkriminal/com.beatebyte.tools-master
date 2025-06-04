using BeatebyteToolsEditor.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BeatebyteToolsEditor.Utils;
using static BeatebyteToolsEditor.Shared.Util;
using UnityEngine.Rendering;

namespace BeatebyteToolsEditor.Components
{
    [AddComponentMenu("Beatebyte/Destroy Object")]
    [eClassHeader("     Destroy Object", iconName = "icon_v2")]
    public class bteDestroyObject : eMonoBehaviour
    {
        [eEditorToolbar("Timing")]
        [Space(10)]
        /// <summary>
        /// Set to True if want start Destroy on start function or set to False if you want start Destroy calling Destroy from another script
        /// </summary>
        [SerializeField]
        [Tooltip("Set true if want start Destroy on start function.\bSet False if you want start Destroy calling Destroy from another script.")]
        private bool destroyOnStart = false;
        [eSeparator(fontSize = 12, label = "Time settings")]
        /// <summary>
        /// Wait time to start destroy in seconds
        /// </summary>
        [SerializeField]
        [Tooltip("Time to start destroy in seconds")]
        private float destroyTime = 0f;
        
        /// <summary>
        /// Get or set wait time to start destroy in seconds
        /// </summary>
        public float DestroyTime { get => destroyTime; set => destroyTime = value; }
        
        /// <summary>
        /// Time to wait to start fading material in seconds
        /// </summary>
        [SerializeField]
        [Tooltip("Time to wait to start fading material in seconds")]
        private float waitTime = 1f;
        
        /// <summary>
        /// Get or set time to wait to start fading material in seconds
        /// </summary>
        public float WaitTime { get => waitTime; set => waitTime = value; }

        /// <summary>
        /// Delay fade time.
        /// </summary>
        [SerializeField]
        [Tooltip("Time to delay to start fading material in seconds")]
        private float delayFadeTime = 1f;

        /// <summary>
        /// Get or set time to delay to start fading material in seconds.
        /// </summary>
        public float DelayFadeTime { get => delayFadeTime; set => delayFadeTime = value; }
        
        /// <summary>
        /// How long is fading in seconds.
        /// </summary>
        [SerializeField]
        [Tooltip("Speed of fade FX (0 is infinity, higher value means much faster)")]
        private float fadeSpeed = 1f;

        /// <summary>
        /// Get or set the speed of fade fx (0 is infinity, higher value means much faster)
        /// </summary>
        public float FadeSpeed { get => fadeSpeed; set => fadeSpeed = value; }

        [eSeparator(fontSize = 12, label = "Flicker FX Settings")]

        [SerializeField]
        [Tooltip("Set dissolve with flicker FX")]
        private bool useFlickerFX = false;
        /// <summary>
        /// Gets or sets a value indicating whether the flicker effect is enabled.
        /// </summary>
        public bool UseFlickerFX { get => useFlickerFX; set => useFlickerFX = value; }

        [SerializeField]
        [Tooltip("Speed of flicker FX")]
        private float flickSpeed = 1f;
        /// <summary>
        /// Gets or sets the speed at which the flicker effect occurs.
        /// </summary>
        public float FlickerSpeed { get => flickSpeed; set => flickSpeed = value; }

        [SerializeField, Tooltip("Duration of flicker FX in seconds")]
        private float flickerDuration = 1f;

        /// <summary>
        /// Gets or sets the duration of the flicker effect, in seconds.
        /// </summary>
        public float FlickerDuration { get => flickerDuration; set => flickerDuration = value; }

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Minimum alpha value of flicker FX")]
        private float minFlickAlphaValue = 0f;
        /// <summary>
        /// Gets or sets the minimum alpha value for flicker effects (value must be between 0 and 1.
        /// </summary>
        public float MinFlickAlphaValue { get => minFlickAlphaValue; set => minFlickAlphaValue = Mathf.Clamp01(value); }

        [SerializeField]
        [Tooltip("Smooth flicker FX")]
        private bool smoothFlickerFX = false;
        /// <summary>
        /// Gets or sets a value indicating whether the smooth flicker effect is enabled.
        /// </summary>
        public bool SmoothFlickerFX { get => smoothFlickerFX; set => smoothFlickerFX = value; }
        
        [eSeparator(fontSize = 12, label = "Particles FX")]
        /// <summary>
        /// The Prefab FX that you want to be instanced on Destroy Event.
        /// </summary>
        
        [SerializeField]
        [Tooltip("The Prefab FX that you want to be instanced on Destroy Event")]
        private GameObject fxPrefab;
        
        /// <summary>
        /// Gets or sets the GameObject used as the visual effect prefab.
        /// </summary>
        public GameObject FxPrefab { get => fxPrefab; set => fxPrefab = value; }
        
        [SerializeField]
        [Tooltip("Time to start instancing the particles FX prefab in seconds")]
        private float fxStartTime = 0f;
        
        /// <summary>
        /// Get or set time to start instancing the particles FX prefab in seconds.
        /// </summary>
        public float FxStartTime { get => fxStartTime; set => fxStartTime = value; }
        [SerializeField, Tooltip("Time to destroy the Particles FX")]
        private float prefabDestroyTime = 1f;
        /// <summary>
        /// Gets or sets the time, in seconds, after which the prefab will be destroyed.
        /// </summary>
        public float PrefabDestroyTime { get => prefabDestroyTime; set => prefabDestroyTime = value; }
        [eEditorToolbar("Sounds FX")]
        [Space(10)]

        [SerializeField, Tooltip("Audiosource component for fading sound")]
        private AudioSource audioSource;
        /// <summary>
        /// Gets or sets the audio source associated with this object.
        /// </summary>
        public AudioSource AudioSource { get => audioSource; set => audioSource = value; }

        [SerializeField, Tooltip("Sounds while is in fading")]
        private AudioClip fadeSound = null;
        /// <summary>
        /// Gets or sets the audio clip used for the fade sound effect.
        /// </summary>
        [SerializeField, Tooltip("Play sound looping")]
        private bool loopFadeSound = false;
        /// <summary>
        /// Gets or sets a value indicating whether the sound should loop with a fade effect.
        /// </summary>
        public bool LoopFadeSound { get => loopFadeSound; set => loopFadeSound = value; }
        public AudioClip FadeSound { get => fadeSound; set => fadeSound = value; }

        [field: SerializeField, Tooltip("Set to True if you want fade out sound on destroy")] public bool FadeSoundOnDestroy { get; set; } = false;
        [field: SerializeField, Tooltip("Set fadeout time"), Range(0f, 5f)] public float FadeOutTime { get; set; } = 0f;

        [SerializeField, Tooltip("Sounds when particle FX is spawned")]
        private AudioClip spawnedParticlesSound = null;
        /// <summary>
        /// Gets or sets the audio clip that plays when a particle is spawned.
        /// </summary>
        public AudioClip SpawnedParticleSound { get => spawnedParticlesSound; set => spawnedParticlesSound = value; }

        [SerializeField, Tooltip("Volume of spawned Sound")]
        [Range(0f, 1f)]
        private float spawnedSoundVolume = 1f;
        /// <summary>
        /// Gets or sets the volume level for sounds spawned by the system (value must be between 0 and 1)
        /// </summary>
        public float SpawnedSoundVolume { get => spawnedSoundVolume; set => spawnedSoundVolume = Mathf.Clamp01(value); }

        [eEditorToolbar("Materials")]
        [Space(10)]
        /// <summary>
        /// The array of materials you want to affect the fade FX.
        /// </summary>
        [SerializeField]
        [Tooltip("The array of static meshes materials that you want to be affected by the fade FX")]
        private Material[] materials;
        /// <summary>
        /// The mesh renderers.
        /// </summary>
        [SerializeField]
        [Tooltip("The array of Mesh Renderers Components that you want be affected by the fade FX")]
        private SkinnedMeshRenderer[] meshRenderers;
        
        /// <summary>
        /// The fx object.
        /// </summary>
        private GameObject fxObject;
        /// <summary>
        /// The alpha.
        /// </summary>
        private readonly float _alpha;
        /// <summary>
        /// Can fade.
        /// </summary>
        private bool canFade = false;

        private bool canFlick = false;
        /// <summary>
        /// Can destroy.
        /// </summary>
        private bool canDestroy = false;
        private bool particleInstantiated = false;

        private Coroutine coroutine = null;
        private Coroutine audioCoroutine = null;

        private bool isSRP = false;

        private float _flickTime = 0f;
        private float _fxTime = 0f;
        private bool _flickIsMin = false;

        [Serializable]
        public class OnStartFadehHandler : UnityEvent { }
        [Serializable]
        public class OnFadeHandler : UnityEvent { }
        [Serializable]
        public class OnDestroyHandler : UnityEvent { }

        /// <summary>
        /// Callback of OnFade Event.
        /// </summary>
        [eEditorToolbar("Events")]
        [Tooltip("Callback of OnFade Event")]
        public OnStartFadehHandler onFade = new();
        /// <summary>
        /// Callback of OnStartFade Event.
        /// </summary>
        [Tooltip("Callback of OnStartFade Event")]
        public OnFadeHandler onStartFade = new();
        /// <summary>
        /// Callback of OnDestroy Event.
        /// </summary>
        [Tooltip("Callback of OnDestroy Event")]
        public OnDestroyHandler onDestroy = new();

        /// <summary>
        /// On fade action.
        /// </summary>
        private UnityAction OnFadeAction;
        /// <summary>
        /// On start fade action.
        /// </summary>
        private UnityAction OnStartFadeAction;
        /// <summary>
        /// On destroy action.
        /// </summary>
        private UnityAction OnDestroyAction;



        /// <summary>
        /// On enable.
        /// </summary>
        private void OnEnable()
        {
            OnFadeAction += OnFade;
            OnStartFadeAction += OnStartFade;
            OnDestroyAction += OnDestroy;

            isSRP = GraphicsSettings.defaultRenderPipeline == null;

        }

        /// <summary>
        /// On disable.
        /// </summary>
        private void OnDisable()
        {
            OnFadeAction -= OnFade;
            OnStartFadeAction -= OnStartFade;
            OnDestroyAction -= OnDestroy;
        }
        void Start()
        {
            //Debug.Log(isSRP);
            if (destroyOnStart)
            {
                Destroy();
            }
        }
        /// <summary>
        /// Call this function to Destroy
        /// </summary>
        public void Destroy()
        {
            if (materials.Length == 0 && meshRenderers.Length == 0) return;
            else if (materials.Length == 0 && meshRenderers.Length > 0)
            {
                materials = new Material[meshRenderers.Length];


                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    materials[i] = meshRenderers[i].material;
                }
            }

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            if (useFlickerFX) 
            { 
                coroutine = StartCoroutine(IEFlickerFX(waitTime, delayFadeTime));
            }
            else
            {
                coroutine = StartCoroutine(IEFadeColor(waitTime, delayFadeTime));
            }
        }

        private void StartFX()
        {
            if (fxPrefab && fxObject == null)
            {
                fxObject = Instantiate(fxPrefab, transform.position, Quaternion.identity);
                if (spawnedParticlesSound != null)
                { 
                    AudioSource.PlayClipAtPoint(spawnedParticlesSound, fxObject.transform.transform.position);
                }
            }            

            if (fxObject != null) 
            { 
                Destroy(fxObject, prefabDestroyTime);
            }

            Destroy(gameObject, destroyTime);

        }

        private void OnFade()
        {
            onFade?.Invoke();
        }

        private void OnStartFade()
        {
            if (fadeSound != null)
            {
                audioSource = audioSource != null ? audioSource : gameObject.AddComponent<AudioSource>();
                audioSource.loop = loopFadeSound;
                audioSource.clip = fadeSound;
                audioSource.Play();
            }
            onStartFade?.Invoke();
        }

        private void OnDestroy()
        {
            if (materials.Length != 0)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    UnityEngine.Color color = materials[i].color;
                    color.a = 1f;
                    materials[i].color = color;
                    if (isSRP)
                    {
                        SetMaterialTransparentEx(materials[i], false);
                    }
                    else
                    {
                        SetMaterialTransparent(materials[i], false);
                    }                        
                }
            }
            onDestroy?.Invoke();
        }

        private void Update()
        {
            if (materials.Length != 0 && canFade)
            {
                Fade();
            }

            if (materials.Length != 0 && canFlick)
            {
                Flick();
            }

            if (!particleInstantiated && canDestroy)
            {
                particleInstantiated = true;
                Invoke(nameof(StartFX), fxStartTime);

            }

        }
        private void HideMeshes()
        {
            if (meshRenderers.Length > 0)
            {
                foreach (var item in meshRenderers)
                {
                    item.enabled = false;
                }
            }
            else if (transform.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.enabled = false;
            }

            if (audioSource != null && audioSource.isPlaying)
            {
                if (FadeSoundOnDestroy)
                {
                    if (audioCoroutine != null)
                    {
                        StopCoroutine(audioCoroutine);
                        audioCoroutine = null;
                    }
                    audioCoroutine = StartCoroutine(IEFadeSound(FadeOutTime));                                        
                }
                else
                {
                    audioSource.Stop();
                }
            }
        }

        private void Flick()
        {
            _flickTime += Time.deltaTime;
            _fxTime += Time.deltaTime;
            
            if (_fxTime > flickerDuration)
            {
                HideMeshes();
                canDestroy = true;
                canFlick = false;
            }
            if (smoothFlickerFX) 
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    Color color = materials[i].color;
                    color.a = Mathf.Clamp((Mathf.Sin(Time.time * flickSpeed * 10) / 2) + .5f, minFlickAlphaValue, 1f);
                    materials[i].color = color;
                } 
            }
            else
            {
                if (_flickTime * 10 > flickSpeed)
                {
                    for (int i = 0; i < materials.Length; i++)
                    {
                        Color color = materials[i].color;
                        color.a = _flickIsMin ? minFlickAlphaValue : 1f;                     
                        materials[i].color = color;
                    }
                    _flickTime = 0;
                    _flickIsMin = !_flickIsMin;
                }
            }
        }

        private void Fade()
        {
            OnFadeAction?.Invoke();

            for (int i = 0; i < materials.Length; i++)
            {
                UnityEngine.Color color = materials[i].color;
                color.a = Mathf.Lerp(color.a, _alpha, fadeSpeed * Time.deltaTime);
                materials[i].color = color;

                double transparency = Math.Round(color.a, 2);

                if (Mathf.Approximately((float)transparency, 0.01f))
                {
                    HideMeshes();
                    canDestroy = true;
                }
            }
        }

        IEnumerator IEFadeSound(float fadeOutTime)
        {
            bool isFading = true;

            while (isFading) 
            { 
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * fadeOutTime * 10);
                if (audioSource.volume <= 0f)
                {
                    isFading = false;
                }

                yield return null;
            }

            audioSource.Stop();
                        
        }

        IEnumerator IEFlickerFX(float wait , float delay) 
        { 
            yield return new WaitForSeconds(wait);
            
            OnStartFadeAction?.Invoke();

            yield return new WaitForSeconds(delay);

            for (int i = 0; i < materials.Length; i++)
            {
                if (isSRP)
                {
                    SetMaterialTransparentEx(materials[i], true);
                }
                else
                {
                    SetMaterialTransparent(materials[i], true);
                }
            }

            canFlick = true;
        }

        IEnumerator IEFadeColor(float wait, float delay)
        {

            yield return new WaitForSeconds(wait);

            OnStartFadeAction?.Invoke();

            yield return new WaitForSeconds(delay);

            for (int i = 0; i < materials.Length; i++)
            {
                if (isSRP)
                {
                    SetMaterialTransparentEx(materials[i], true);
                }
                else
                {
                    SetMaterialTransparent(materials[i], true);
                }
            }

            canFade = true;
           
        }
    }

}
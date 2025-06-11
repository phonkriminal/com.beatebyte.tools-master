using BeatebyteToolsEditor.Attributes;
using BeatebyteToolsEditor.Runtime;
using BeatebyteToolsEditor.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static BeatebyteToolsEditor.Shared.Utils;


namespace BeatebyteToolsEditor.Components
{
    /// <summary>
    /// The footsteps.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Beatebyte/Footsteps")]
    [eClassHeader("     Footsteps Manager", iconName = "icon_v2")]

    public class Footsteps : eMonoBehaviour
    {
        private static readonly string GizmosIconGUID = "7465d92e862b65746b9253a30cf41d06";

        /// <summary>
        /// The footstep source.
        /// </summary>
        [eEditorToolbar("Audio Component")]
        [Space(5)]
        [SerializeField] AudioSource footstepSource;
        /// <summary>
        /// The audio source prefab.
        /// </summary>
        [SerializeField]
        private GameObject audioSourcePrefab;

        /// <summary>
        /// The animator.
        /// </summary>
        [SerializeField]
        private Animator _animator;
        /// <summary>
        /// Last footstep.
        /// </summary>
        private float _lastFootstep;

        /// <summary>
        /// The audio source object.
        /// </summary>
        private GameObject audioSourceObject;

        /// <summary>
        /// The left foot collider.
        /// </summary>
        private SphereCollider _leftFootCollider;
        /// <summary>
        /// The right foot collider.
        /// </summary>
        private SphereCollider _rightFootCollider;

        /// <summary>
        /// The footsteps.
        /// </summary>
        [eEditorToolbar("Audio Surface")]
        [Space(10)]
        [SerializeField] List<FootstepSurface> footsteps = new();

        /// <summary>
        /// The left foot transform.
        /// </summary>
        [eEditorToolbar("Feet Transform")]
        [Space(10)]
        [eSeparator("Set the footsteps surface recognition mode", style = "box")]
        [SerializeField] FootstepMode footstepMode;
        [Space(10)]
        [SerializeField] private Transform leftFootTransform;
        /// <summary>
        /// The right foot transform.
        /// </summary>
        [SerializeField] private Transform rightFootTransform;
        /// <summary>
        /// The footstep radius.
        /// </summary>
        [Range(0.1f, 1f)]
        [SerializeField] private float footstepRadius = 0.1f;
        /// <summary>
        /// The ground layer.
        /// </summary>
        [eLayerMask]
        public LayerMask groundLayer = 1 << 0;
        
        [Tooltip("The name of character speed parameter in the animator.")]

        [field: SerializeField]
        public bool UseCollider { get; set; } = false;

        [eEditorToolbar("Speed Parameters")]
        [Space(10)]
        [Tooltip("The name of character speed parameter in the animator.")]
        [SerializeField] private string speedParameterName = string.Empty;
        [SerializeField] private float walkSpeed = 0f;
        [SerializeField] private float runSpeed = 0f;
        [SerializeField] private float sprintSpeed = 0f;
        [Space(5)]
        [Header("Optional")]
        [SerializeField] private bool useJumpSound = false;
        [SerializeField] private bool useLandSound = false;
        [SerializeField] private bool useSlideSound = false;

        /// <summary>
        /// The current surface name.
        /// </summary>
        private string _currentSurfaceName = string.Empty;


        /// <summary>
        /// Walk sounds.
        /// </summary>
        private AudioClip[] walkSounds = new AudioClip[0];
        /// <summary>
        /// Run sounds.
        /// </summary>
        private AudioClip[] runSounds  = new AudioClip[0];
        /// <summary>
        /// Land sounds.
        /// </summary>
        private AudioClip[] landSounds = new AudioClip[0];
        /// <summary>
        /// Jump sounds.
        /// </summary>
        private AudioClip[] jumpSounds = new AudioClip[0];
        /// <summary>
        /// The slide sounds.
        /// </summary>
        private AudioClip[] slideSounds = new AudioClip[0];

        /// <summary>
        /// The footstep type.
        /// </summary>
        private FootstepType _footstepType = FootstepType.Walk;
        /// <summary>
        /// The Start method.
        /// </summary>
        private void Start()
        {
            if (footstepSource == null && audioSourcePrefab == null)
            {
                gameObject.AddComponent<AudioSource>();
                footstepSource = GetComponent<AudioSource>();
            }

            if (footstepSource == null && audioSourcePrefab != null)
            {
                audioSourceObject = Instantiate(audioSourcePrefab, transform);
                footstepSource = audioSourceObject.GetComponent<AudioSource>();
            }
        }

        /// <summary>
        /// On validate.
        /// </summary>
        private void OnValidate()
        {
            if (!_animator) { _animator = GetComponent<Animator>(); }

            if (!leftFootTransform)
            {
                // Corrected the code to use HumanBodyBones.LeftFoot directly with the Animator component
                leftFootTransform = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);

                if (leftFootTransform == null)
                {
                    Debug.LogWarning("Left foot transform not found. Please assign it in the inspector.");
                    return;
                }

            }

            if (leftFootTransform.GetComponent<SphereCollider>() == null)
            {
                _leftFootCollider = leftFootTransform.gameObject.AddComponent<SphereCollider>();
                _leftFootCollider.isTrigger = true;
                _leftFootCollider.radius = footstepRadius; // Adjust the radius as needed
            }
            else
            {
                _leftFootCollider = leftFootTransform.GetComponent<SphereCollider>();

            }

            if (!rightFootTransform)
            {
                rightFootTransform = _animator.GetBoneTransform(HumanBodyBones.RightFoot);

                if (rightFootTransform == null)
                {
                    Debug.LogWarning("Right foot transform not found. Please assign it in the inspector.");
                }
            }

            if (rightFootTransform.GetComponent<SphereCollider>() == null)
            {
                _rightFootCollider = rightFootTransform.gameObject.AddComponent<SphereCollider>();
                _rightFootCollider.isTrigger = true;
                _rightFootCollider.radius = footstepRadius; // Adjust the radius as needed
            }
            else
            {
                _rightFootCollider = rightFootTransform.GetComponent<SphereCollider>();
            }
        }
        /// <summary>
        /// The update method.
        /// </summary>
        private void Update()
        {
            UpdateColliders();
            UpdateFootsteps();
        }

        /// <summary>
        /// Get surface name.
        /// </summary>
        /// <returns>A string</returns>
        private string GetSurfaceName()
        {
            if (_leftFootCollider)
            {
                if (Physics.Raycast(leftFootTransform.position, Vector3.down, out RaycastHit hit, 10f, groundLayer))
                {
                    string rtnString = string.Empty;

                    switch (footstepMode)
                    {
                        case FootstepMode.PhysicsMaterial:
                            rtnString = hit.collider.gameObject.GetComponent<MeshCollider>().material.name;
                            break;
                        case FootstepMode.Material:
                            rtnString = hit.collider.gameObject.GetComponent<MeshRenderer>().material.name;
                            break;
                        case FootstepMode.Texture:
                            Texture mainTexture = hit.collider.gameObject.GetComponent<MeshRenderer>().material.GetTexture("_MainTex");
                            rtnString = mainTexture.name;
                            break;
                        case FootstepMode.Terrain:

                            Terrain terrain = Terrain.activeTerrain;

                            if (terrain != null)
                            {
                                TerrainData terrainData = terrain.terrainData;
                                TerrainLayer[] terrainLayers = terrainData.terrainLayers;

                                if (terrainLayers != null && terrainLayers.Length > 0)
                                {
                                    Debug.Log("Terrain Texture Names: " + terrainLayers[GetMainTexture(transform.position)].diffuseTexture.name);
                                    rtnString = terrainLayers[GetMainTexture(transform.position)].diffuseTexture.name;
                                }
                                else
                                {
                                    Debug.Log("No terrain textures found.");
                                }
                            }
                            else
                            {
                                Debug.LogError("No terrain assigned.");
                            }
                            break;
                        default:
                            break;
                    }

                    rtnString = rtnString.Replace(" (Instance)", string.Empty);
                    Debug.Log("Footstep surface: " + rtnString);
                    return rtnString;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Get clips for surface.
        /// </summary>
        /// <param name="surfaceName">The surface name.</param>
        /// <returns>A bool</returns>
        private bool GetClipsForSurface(string surfaceName)
        {
            if (footsteps.Count == 0 || string.IsNullOrEmpty(surfaceName))
            {
                Debug.LogWarning("No footstep surface assigned.");
                return false;
            }

            if (_currentSurfaceName != surfaceName)
            {
                walkSounds?.Initialize();
                runSounds?.Initialize();
                landSounds?.Initialize();
                jumpSounds?.Initialize();
                slideSounds?.Initialize();
                switch (footstepMode)
                {
                    case FootstepMode.PhysicsMaterial:
                        walkSounds = footsteps.Find(x => x.gameData.SurfaceName == surfaceName)?.gameData.WalkSounds;
                        runSounds = footsteps.Find(x => x.gameData.SurfaceName == surfaceName)?.gameData.RunSounds;
                        landSounds = footsteps.Find(x => x.gameData.SurfaceName == surfaceName)?.gameData.LandSounds;
                        jumpSounds = footsteps.Find(x => x.gameData.SurfaceName == surfaceName)?.gameData.JumpSounds;
                        slideSounds = footsteps.Find(x => x.gameData.SurfaceName == surfaceName)?.gameData.SlideSounds;
                        break;
                    case FootstepMode.Material:
                        walkSounds = footsteps.Find(x => x.gameData.SurfaceMaterial.name == surfaceName)?.gameData.WalkSounds;
                        runSounds = footsteps.Find(x => x.gameData.SurfaceMaterial.name == surfaceName)?.gameData.RunSounds;
                        landSounds = footsteps.Find(x => x.gameData.SurfaceMaterial.name == surfaceName)?.gameData.LandSounds;
                        jumpSounds = footsteps.Find(x => x.gameData.SurfaceMaterial.name == surfaceName)?.gameData.JumpSounds;
                        slideSounds = footsteps.Find(x => x.gameData.SurfaceMaterial.name == surfaceName)?.gameData.SlideSounds;
                        break;
                    case FootstepMode.Texture:
                        walkSounds = footsteps.Find(x => x.gameData.SurfaceTexture.name == surfaceName)?.gameData.WalkSounds;
                        runSounds = footsteps.Find(x => x.gameData.SurfaceTexture.name == surfaceName)?.gameData.RunSounds;
                        landSounds = footsteps.Find(x => x.gameData.SurfaceTexture.name == surfaceName)?.gameData.LandSounds;
                        jumpSounds = footsteps.Find(x => x.gameData.SurfaceTexture.name == surfaceName)?.gameData.JumpSounds;
                        slideSounds = footsteps.Find(x => x.gameData.SurfaceTexture.name == surfaceName)?.gameData.SlideSounds;
                        break;
                    case FootstepMode.Terrain:
                        walkSounds = footsteps.Find(x => x.gameData.SurfaceTexture.name == surfaceName)?.gameData.WalkSounds;
                        runSounds = footsteps.Find(x => x.gameData.SurfaceTexture.name == surfaceName)?.gameData.RunSounds;
                        landSounds = footsteps.Find(x => x.gameData.SurfaceTexture.name == surfaceName)?.gameData.LandSounds;
                        jumpSounds = footsteps.Find(x => x.gameData.SurfaceTexture.name == surfaceName)?.gameData.JumpSounds;
                        slideSounds = footsteps.Find(x => x.gameData.SurfaceTexture.name == surfaceName)?.gameData.SlideSounds;
                        break;
                }

                Debug.Log(walkSounds.Length);

                _currentSurfaceName = surfaceName;
            }

            return true;
        }

        /// <summary>
        /// Update the colliders.
        /// </summary>
        private void UpdateColliders()
        {
            if (!_leftFootCollider || !_rightFootCollider)
            {
                return;
            }

            if (_leftFootCollider.radius != footstepRadius)
            {
                _leftFootCollider.radius = footstepRadius;
            }
            if (_rightFootCollider.radius != footstepRadius)
            {
                _rightFootCollider.radius = footstepRadius;
            }
        }
        /// <summary>
        /// Update the footsteps.
        /// </summary>
        private void UpdateFootsteps()
        {

            if (!Application.isPlaying)
            {
                return;
            }
            var _footstep = _animator.GetFloat("Footstep");

            if (Mathf.Abs(_footstep) < 0.00001f)
            {
                _footstep = 0;
            }

            if (_lastFootstep > 0 && _footstep < 0 || _lastFootstep < 0 && _footstep > 0)
            {
                PlayFootstep();
            }

            _lastFootstep = _footstep;

        }

        /// <summary>
        /// Play the footstep.
        /// </summary>
        public void PlayFootstep()
        {
            if (!GetClipsForSurface(GetSurfaceName())) return;

            AudioClip clip = null;

            switch (_footstepType)
            {
                case FootstepType.Walk:
                    if (walkSounds.Length > 0)
                    {
                        clip = walkSounds[Random.Range(0, walkSounds.Length)];
                    }
                    break;
                case FootstepType.Run:
                    if (runSounds.Length > 0)
                    {
                        clip = runSounds[Random.Range(0, runSounds.Length)];
                    }
                    else
                    {
                        if (walkSounds.Length > 0)
                        {
                            clip = walkSounds[Random.Range(0, walkSounds.Length)];
                        }
                    }
                        break;
                case FootstepType.Jump:
                    if (jumpSounds.Length > 0)
                    {
                        clip = jumpSounds[Random.Range(0, jumpSounds.Length)];
                    }
                    break;
                case FootstepType.Land:
                    if (landSounds.Length > 0)
                    {
                        clip = landSounds[Random.Range(0, landSounds.Length)];
                    }
                    break;
                case FootstepType.Slide:
                    if (slideSounds.Length > 0)
                    {
                        clip = slideSounds[Random.Range(0, slideSounds.Length)];
                    }
                    break;
                default:
                    if (walkSounds.Length > 0)
                    {
                        clip = walkSounds[Random.Range(0, walkSounds.Length)];
                    }
                    break;
            }

            if (clip && footstepSource)
            {
                footstepSource.PlayOneShot(clip);
                Debug.Log($"Play Footstep Clip ({clip.name})." );

            }
        }
        /// <summary>
        /// On draw gizmos.
        /// </summary>
        private void OnDrawGizmosSelected()
        {                    
            if (_leftFootCollider)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(leftFootTransform.position, _leftFootCollider.radius);
            }

            if (_rightFootCollider)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(rightFootTransform.position, _rightFootCollider.radius);
            }

        }

        private void OnDrawGizmos()
        {
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(GizmosIconGUID));
            Gizmos.DrawIcon(transform.position, AssetDatabase.GetAssetPath(icon), true);
        }

        /// <summary>
        /// The footstep types.
        /// </summary>
        public enum FootstepType
        {
            Walk = 0,
            Run = 1,
            Jump = 2,
            Land = 3, 
            Slide = 4
        }

        public enum FootstepMode
        {
            PhysicsMaterial = 0,
            Material = 1,
            Texture = 2,
            Terrain = 3
        }
    }

}
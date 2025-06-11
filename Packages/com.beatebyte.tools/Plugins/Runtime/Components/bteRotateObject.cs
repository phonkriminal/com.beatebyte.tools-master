using BeatebyteToolsEditor.Attributes;
using BeatebyteToolsEditor.Utils;
using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Components
{
    /// <summary>
    /// The EDS rotate object.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Beatebyte/Rotate Object")]
    [eClassHeader("     Rotate Object", iconName = "icon_v2")]


    public class bteRotateObject : eMonoBehaviour
    {
        /// <summary>
        /// The X rotate speed.
        /// </summary>
        [eEditorToolbar("Rotation Parameters")]
        [Space(10)]

        [SerializeField] private float xRotateSpeed;
        /// <summary>
        /// The Y rotate speed.
        /// </summary>
        [SerializeField] private float yRotateSpeed;
        /// <summary>
        /// The Z rotate speed.
        /// </summary>
        [SerializeField] private float zRotateSpeed;
        /// <summary>
        /// The activate in editor.
        /// </summary>
        [eToggleOption]
        [SerializeField] private bool _activateInEditor = true;
        /// <summary>
        /// The space.
        /// </summary>
        public Space space;
        /// <summary>
        /// Is clamp.
        /// </summary>
        [eToggleOption("Clamp Rotation to Axis")]
        public bool isClamp;
        /// <summary>
        /// The maxis name.
        /// </summary>
        [eDropdownList(nameof(GetAxisName))]
        public string m_axisName;

        /// <summary>
        /// The clamp angle.
        /// </summary>
        [Range(1f, 180f)]
        public float clampAngle = 90f;
        /// <summary>
        /// Is smooth.
        /// </summary>
        [eToggleOption("Use Smoothing")]
        public bool isSmooth;

        /// <summary>
        /// The smooth ratio.
        /// </summary>
        public float smoothRatio = 5.0f;

        /// <summary>
        /// Start time.
        /// </summary>
        private float startTime = 0;

        /// <summary>
        /// The M text.
        /// </summary>
        [eEditorToolbar("Debug")]
        [Space(10)]
        [eButton("Reset to Original Rotation", "ResetStartRotation", typeof(bteRotateObject), false)]
        [eLabel(style: "bteTextarea")]
        [Multiline]
        [SerializeField] private string m_text;
        //[SerializeField] private bool m_debug;
        /// <summary>
        /// The initial rotation.
        /// </summary>
        private Quaternion _initialRotation = Quaternion.identity;

        private void Start()
        {
            _initialRotation = transform.localRotation;
            startTime = Time.time;
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update += Update;
#endif

            _initialRotation = transform.localRotation;
            startTime = Time.time;
        }

        private void OnValidate()
        {

        }
        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= Update;
#endif

            transform.localRotation = _initialRotation;
        }

        public void ResetStartRotation()
        {
            transform.localRotation = _initialRotation;
            startTime = Time.time;
            SetDebugText();
        }

        private string[] GetAxisName() => new string[] { "X Axis", "Y Axis", "Z Axis", "All Axis" };

        private Axis GetAxis(string axisName)
        {
            switch (axisName)
            {
                case "X Axis":
                    return Axis.X;
                case "Y Axis":
                    return Axis.Y;
                case "Z Axis":
                    return Axis.Z;
                case "All Axis":
                    return Axis.All;
                default:
                    return Axis.All;
            }
        }
        void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && !_activateInEditor)
            {
                return;
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
#endif
            if (isClamp)
            {
                Axis axis = GetAxis(m_axisName);
                float t = (Time.time - startTime) / smoothRatio;
                switch (axis)
                {
                    case Axis.X:
                        transform.localRotation = Quaternion.Euler(Mathf.PingPong(Time.time * xRotateSpeed, clampAngle * 2) - clampAngle, 0.0f, 0.0f);
                        break;

                    case Axis.Y:
                        transform.localRotation = Quaternion.Euler(0.0f, Mathf.PingPong(Time.time * yRotateSpeed, clampAngle * 2) - clampAngle, 0.0f);
                        break;

                    case Axis.Z:
                        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.PingPong(Time.time * zRotateSpeed, clampAngle * 2) - clampAngle);
                        break;

                    case Axis.All:
                        transform.localRotation = Quaternion.Euler(Mathf.PingPong(Time.time * xRotateSpeed, clampAngle * 2) - clampAngle,
                                                                    Mathf.PingPong(Time.time * yRotateSpeed, clampAngle * 2) - clampAngle,
                                                                    Mathf.PingPong(Time.time * zRotateSpeed, clampAngle * 2) - clampAngle);
                        break;

                }
            }
            else
            {
                transform.Rotate(xRotateSpeed * Time.deltaTime, yRotateSpeed * Time.deltaTime, zRotateSpeed * Time.deltaTime, space);
            }
            SetDebugText();
            /*            m_text = $"<color=white>Start Rotation Values : </color>{GetFormatRotation(_initialRotation)}\n" + 
                                    $"<color=white>Current Rotation Value : </color>{GetFormatRotation(transform.localRotation)}";
            */
        }

        private float Hermite(float t, bool doSmooth)
        {
            if (doSmooth)
            {
                return ((-t * t * t * 2f) + (t * t * 3f)) * 0.001f;
            }

            return t;
        }
        private void SetDebugText()
        {
            m_text = $"<color=white>Start Rotation Values : </color>{GetFormatRotation(_initialRotation)}\n" +
                        $"<color=white>Current Rotation Value : </color>{GetFormatRotation(transform.localRotation)}";
        }

        private string GetFormatRotation(Quaternion rotation)
        {
            return rotation.eulerAngles.ToString();
        }
    }

    /// <summary>
    /// The axes.
    /// </summary>
    [System.Serializable]
    public enum Axis
    {
        X, Y, Z, All
    }

}

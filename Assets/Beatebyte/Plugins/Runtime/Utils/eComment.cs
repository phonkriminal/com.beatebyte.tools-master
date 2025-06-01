using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeatebyteToolsEditor.Utils
{
    public class eComment : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] protected string header = "COMMENT";
        [Multiline]
        [SerializeField] protected string comment;

        [SerializeField] protected bool inEdit;

#endif
    }
}
using UnityEngine;

namespace BeatebyteToolsEditor.Utils
{
    public class eSetParent : MonoBehaviour
    {
        public void RemoveParent()
        {
            transform.parent = null;
        }

        public void RemoveParent(Transform target)
        {
            target.parent = null;
        }

        public void SetParent(Transform parent)
        {
            transform.parent = parent;
        }

    }
}
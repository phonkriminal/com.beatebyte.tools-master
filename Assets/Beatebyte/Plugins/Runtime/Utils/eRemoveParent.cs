using UnityEngine;

namespace BeatebyteToolsEditor.Utils
{
    public class eRemoveParent : MonoBehaviour
    {

        public bool removeOnStart = true;

        private void Start()
        {
            if (removeOnStart)
            {
                RemoveParent();
            }
        }

        public void RemoveParentOfOtherTransform(Transform target)
        {
            target.SetParent(null);
        }
        public void RemoveParent()
        {
            transform.SetParent(null);
        }
    }
}
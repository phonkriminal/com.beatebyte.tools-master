using UnityEngine;
using UnityEngine.UI;

namespace BeatebyteToolsEditor.Utils
{
    public class eSliderSizeControl : MonoBehaviour
    {
        public Slider slider;
        public RectTransform rectTransform;
        public float multipScale = 0.1f;
        float oldMaxValue;
        void OnDrawGizmosSelected()
        {
            UpdateScale();
        }
        public void UpdateScale()
        {
            if (rectTransform && slider)
            {
                if (slider.maxValue != oldMaxValue)
                {
                    var sizeDelta = rectTransform.sizeDelta;
                    sizeDelta.x = slider.maxValue * multipScale;
                    rectTransform.sizeDelta = sizeDelta;
                    oldMaxValue = slider.maxValue;
                }
            }
        }
    }
}

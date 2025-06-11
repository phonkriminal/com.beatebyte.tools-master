using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e min max attribute.
    /// </summary>
    public class eMinMaxAttribute : PropertyAttribute
    {
        /// <summary>
        /// The min limit.
        /// </summary>
        public float minLimit = 0;
        /// <summary>
        /// The max limit.
        /// </summary>
        public float maxLimit = 1f;
        /// <summary>
        /// Initializes a new instance of the <see cref="eMinMaxAttribute"/> class.
        /// </summary>
        public eMinMaxAttribute()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="eMinMaxAttribute"/> class.
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        public eMinMaxAttribute(float min, float max)
        {
            minLimit = min;
            maxLimit = max;
        }
    }

}
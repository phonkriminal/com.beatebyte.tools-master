using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e read only attribute.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class eReadOnlyAttribute : PropertyAttribute
    {
        /// <summary>
        /// The just in play mode.
        /// </summary>
        public readonly bool justInPlayMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="eReadOnlyAttribute"/> class.
        /// </summary>
        /// <param name="justInPlayMode">If true, just in play mode.</param>
        public eReadOnlyAttribute(bool justInPlayMode = true)
        {
            this.justInPlayMode = justInPlayMode;
        }
    }
}

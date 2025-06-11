using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e bar display attribute.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class eBarDisplayAttribute : PropertyAttribute
    {
        /// <summary>
        /// The max value property.
        /// </summary>
        public readonly string maxValueProperty;
        /// <summary>
        /// Show junt in play mode.
        /// </summary>
        public readonly bool showJuntInPlayMode;
        /// <summary>
        /// Initializes a new instance of the <see cref="eBarDisplayAttribute"/> class.
        /// </summary>
        /// <param name="maxValueProperty">The max value property.</param>
        /// <param name="showJuntInPlayMode">If true, show junt in play mode.</param>
        public eBarDisplayAttribute(string maxValueProperty, bool showJuntInPlayMode = false)
        {
            this.maxValueProperty = maxValueProperty;
            this.showJuntInPlayMode = showJuntInPlayMode;
        }
    }

}
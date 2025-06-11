using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e rich text attribute.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]

    public class eRichTextAttribute : PropertyAttribute
    {
        /// <summary>
        /// Label.
        /// </summary>
        public string label;
        /// <summary>
        /// The tooltip.
        /// </summary>
        public string tooltip;
        /// <summary>
        /// The style.
        /// </summary>
        public string style;

        /// <summary>
        /// Initializes a new instance of the <see cref="eRichTextAttribute"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <param name="style">The style.</param>
        public eRichTextAttribute(string label, string tooltip, string style)
        {
            this.label = label;
            this.tooltip = tooltip;
            this.style = style;
        }
    }

}
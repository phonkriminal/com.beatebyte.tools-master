using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e masked string attribute.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]

    public class eMaskedStringAttribute : PropertyAttribute
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
        /// The mask char.
        /// </summary>
        public char maskChar;

        /// <summary>
        /// Initializes a new instance of the <see cref="eMaskedStringAttribute"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <param name="style">The style.</param>
        public eMaskedStringAttribute(string label, string tooltip, string style)
        {
            this.label = label;
            this.tooltip = tooltip;
            this.style = style;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="eMaskedStringAttribute"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <param name="maskChar">The mask char.</param>
        /// <param name="style">The style.</param>
        public eMaskedStringAttribute(string label, string tooltip, char maskChar, string style)
        {
            this.label = label;
            this.tooltip = tooltip;
            this.style = style;
            this.maskChar = maskChar;
        }

    }

}
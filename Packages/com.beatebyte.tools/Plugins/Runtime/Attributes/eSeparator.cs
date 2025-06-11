using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e separator.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class eSeparator : PropertyAttribute
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
        /// The font size.
        /// </summary>
        public int fontSize = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="eSeparator"/> class.
        /// </summary>
        public eSeparator()
        {
            fontSize = 15;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="eSeparator"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="tooltip">The tooltip.</param>
        public eSeparator(string label, string tooltip = "")
        {
            this.label = label;
            this.tooltip = tooltip;
            this.fontSize = 15;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="eSeparator"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="tooltip">The tooltip.</param>
        public eSeparator(string label, int fontSize, string tooltip = "")
        {
            this.label = label;
            this.tooltip = tooltip;
            this.fontSize = fontSize;
        }
    }

}
using System;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e label attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]

    public class eLabelAttribute : PropertyAttribute
    {
        /// <summary>
        /// Label.
        /// </summary>
        public string label;
        /// <summary>
        /// The icon.
        /// </summary>
        public string icon;
        /// <summary>
        /// The tooltip.
        /// </summary>
        public string tooltip;
        /// <summary>
        /// The style.
        /// </summary>
        public string style;
        /// <summary>
        /// The alignment.
        /// </summary>
        public string alignment;
        /// <summary>
        /// Initializes a new instance of the <see cref="eLabelAttribute"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <param name="style">The style.</param>
        /// <param name="alignment">The alignment.</param>
        public eLabelAttribute(string label = "", string icon = "", string tooltip = "", string style = "", string alignment = "center")
        {
            this.label = label;
            this.icon = icon;
            this.tooltip = tooltip;
            this.style = style;
            this.alignment = alignment;
        }
    }

}
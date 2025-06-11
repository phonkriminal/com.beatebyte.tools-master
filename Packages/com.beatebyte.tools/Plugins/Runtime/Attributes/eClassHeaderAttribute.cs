using System;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e class header attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class eClassHeaderAttribute : Attribute
    {

        /// <summary>
        /// The header.
        /// </summary>
        public string header;
        /// <summary>
        /// Open close.
        /// </summary>
        public bool openClose;
        /// <summary>
        /// The icon name.
        /// </summary>
        public string iconName;
        /// <summary>
        /// Use help box.
        /// </summary>
        public bool useHelpBox;
        /// <summary>
        /// Help box text.
        /// </summary>
        public string helpBoxText;

        /// <summary>
        /// Initializes a new instance of the <see cref="eClassHeaderAttribute"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="openClose">If true, open close.</param>
        /// <param name="iconName">The icon name.</param>
        /// <param name="useHelpBox">If true, use help box.</param>
        /// <param name="helpBoxText">The help box text.</param>
        public eClassHeaderAttribute(string header, bool openClose = true, string iconName = "icon_v2", bool useHelpBox = false, string helpBoxText = "")
        {
            this.header = header.ToUpper();
            this.openClose = openClose;
            this.iconName = iconName;
            this.useHelpBox = useHelpBox;
            this.helpBoxText = helpBoxText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="eClassHeaderAttribute"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="helpBoxText">The help box text.</param>
        public eClassHeaderAttribute(string header, string helpBoxText)
        {
            this.header = header.ToUpper();
            this.openClose = true;
            this.iconName = "icon_v2";
            this.useHelpBox = true;
            this.helpBoxText = helpBoxText;
        }
    }
}
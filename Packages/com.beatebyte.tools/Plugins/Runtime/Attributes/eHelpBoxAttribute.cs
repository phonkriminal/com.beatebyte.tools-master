using System;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e help box attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class eHelpBoxAttribute : PropertyAttribute
    {
        /// <summary>
        /// The text.
        /// </summary>
        public string text;
        /// <summary>
        /// Initializes a new instance of the <see cref="eHelpBoxAttribute"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="messageType">The message type.</param>
        public eHelpBoxAttribute(string text, MessageType messageType = MessageType.None) { this.text = text; this.messageType = messageType; }
        /// <summary>
        /// The line space.
        /// </summary>
        public int lineSpace;

        /// <summary>
        /// The message types.
        /// </summary>
        public enum MessageType
        {
            None,
            Info,
            Warning
        }

        /// <summary>
        /// The message type.
        /// </summary>
        public MessageType messageType;
    }
}
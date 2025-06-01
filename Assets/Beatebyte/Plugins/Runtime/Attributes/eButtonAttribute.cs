using System;
using UnityEngine;
namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e button attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class eButtonAttribute : PropertyAttribute
    {
        /// <summary>
        /// Label.
        /// </summary>
        public readonly string label;
        /// <summary>
        /// The function.
        /// </summary>
        public readonly string function;
        /// <summary>
        /// The id.
        /// </summary>
        public readonly int id;
        /// <summary>
        /// Type.
        /// </summary>
        public readonly Type type;
        /// <summary>
        /// The enabled just in play mode.
        /// </summary>
        public readonly bool enabledJustInPlayMode;

        /// <summary>
        /// Create a button in Inspector
        /// </summary>
        /// <param name="label">button label</param>
        /// <param name="function">function to call on press</param>
        /// <param name="type">parent class type button</param>
        /// <param name="enabledJustInPlayMode">button is enabled just in play mode</param>
        public eButtonAttribute(string label, string function, Type type, bool enabledJustInPlayMode = true)
        {
            this.label = label;
            this.function = function;
            this.type = type;
            this.enabledJustInPlayMode = enabledJustInPlayMode;
        }
    }
}


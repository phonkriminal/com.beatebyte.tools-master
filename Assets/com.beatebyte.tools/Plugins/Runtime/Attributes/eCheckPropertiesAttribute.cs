using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// Check Property is used to validate field using other fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class eCheckPropertyAttribute : PropertyAttribute
    {
        [Serializable]
        public struct CheckValue
        {
            /// <summary>
            /// The property.
            /// </summary>
            public string property;
            /// <summary>
            /// The value.
            /// </summary>
            public object value;

            /// <summary>
            /// Gets a value indicating whether valid.
            /// </summary>
            public bool isValid => value != null;
            /// <summary>
            /// Initializes a new instance of the <see cref="CheckValue"/> class.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <param name="value">The value.</param>
            public CheckValue(string property, object value)
            {
                this.property = property;
                this.value = value;
            }
        }


        /// <summary>
        /// Check values.
        /// </summary>
        public List<CheckValue> checkValues = new List<CheckValue>();

        /// <summary>
        /// The hide in inspector.
        /// </summary>
        public bool hideInInspector;
        /// <summary>
        /// The invert result.
        /// </summary>
        public bool invertResult;
        /// <summary>
        /// Check Property is used to validate field using other filds.
        /// </summary>
        /// <param name="propertyNames"> Properties names  separated by "," (comma)  Exemple "PropertyA,PropertyB". Only Enum and Boolean is accepted.</param>
        /// <param name="values">The values to compare, you need to set all values to compare with all properties</param>
        public eCheckPropertyAttribute(string propertyNames, params object[] values)
        {

            checkValues.Clear();
            var _props = propertyNames.Split(',');

            for (int i = 0; i < _props.Length; i++)
            {
                try
                {
                    checkValues.Add(new CheckValue(_props[i], values[i]));
                }
                catch
                {
                    break;
                }
            }

        }
    }
}
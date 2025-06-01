using System;
using UnityEngine;
namespace BeatebyteToolsEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class eHideInInspectorAttribute : PropertyAttribute
    {
        public bool hideProperty { get; set; }
        public string refbooleanProperty;
        public bool invertValue;
        public eHideInInspectorAttribute(string refbooleanProperty, bool invertValue = false)
        {
            this.refbooleanProperty = refbooleanProperty;
            this.invertValue = invertValue;
        }

    }
}

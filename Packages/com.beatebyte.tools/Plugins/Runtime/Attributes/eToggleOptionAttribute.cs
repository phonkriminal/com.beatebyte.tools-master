using System;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class eToggleOptionAttribute : PropertyAttribute
    {
        public string label, falseValue, trueValue;
        public eToggleOptionAttribute(string label = "", string falseValue = "No", string trueValue = "Yes")
        {
            this.label = label;
            this.falseValue = falseValue;
            this.trueValue = trueValue;
        }
    }

}
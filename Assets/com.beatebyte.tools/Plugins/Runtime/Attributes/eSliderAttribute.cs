using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class eSliderAttribute : PropertyAttribute
    {
        public enum MethodLocation { PropertyClass, StaticClass }
        public MethodLocation Location { get; private set; }
        public string MethodName { get; private set; }
        public Type MethodOwnerType { get; private set; }

        public float min;
        public float max;
        public string label;
        
        public eSliderAttribute(float min, float max, string methodName = null)
        {
            this.Location = MethodLocation.PropertyClass;
            this.min = min;
            this.max = max;
            this.MethodName = methodName;
        }
        public eSliderAttribute(float min, float max, string label = "", string methodName = null)
        {
            this.min = min;
            this.max = max;
            this.label = label;
            this.Location = MethodLocation.PropertyClass;
            this.MethodName = methodName;
            this.MethodOwnerType = MethodOwnerType;
        }

    }

}
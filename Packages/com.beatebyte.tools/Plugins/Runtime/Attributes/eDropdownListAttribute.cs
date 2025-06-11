using System;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]

    public class eDropdownListAttribute : PropertyAttribute
    {
        public enum MethodLocation { PropertyClass, StaticClass }
        public MethodLocation Location { get; private set; }
        public string MethodName { get; private set; }
        public Type MethodOwnerType { get; private set; }
        public string[] options { get; private set; }

        public eDropdownListAttribute(string methodName)
        {
            Location = MethodLocation.PropertyClass;
            MethodName = methodName;
        }
        public eDropdownListAttribute(Type methodOwner, string methodName)
        {
            Location = MethodLocation.StaticClass;
            MethodOwnerType = methodOwner;
            MethodName = methodName;
        }

    }

}
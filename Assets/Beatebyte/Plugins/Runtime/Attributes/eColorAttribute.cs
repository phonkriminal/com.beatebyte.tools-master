using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    public class eColorAttribute : PropertyAttribute
    {
        public string label;
        public eColorAttribute(string label = "")
        {
            this.label = label;
        }
    }

}
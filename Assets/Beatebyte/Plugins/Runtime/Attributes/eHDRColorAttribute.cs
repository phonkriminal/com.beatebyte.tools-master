using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    public class eHDRColorAttribute : PropertyAttribute
    {
        public string label;
        public eHDRColorAttribute(string label = "")
        {
            this.label = label;
        }
    }

}
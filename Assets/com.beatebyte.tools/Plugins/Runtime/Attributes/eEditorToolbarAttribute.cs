using UnityEngine;
namespace BeatebyteToolsEditor.Attributes
{
    public class eEditorToolbarAttribute : PropertyAttribute
    {
        public readonly string title;
        public readonly string icon;
        public readonly bool useIcon;
        public readonly bool overrideChildOrder;
        public readonly bool overrideIcon;
        public eEditorToolbarAttribute(string title, bool useIcon = false, string iconName = "", bool overrideIcon = false, bool overrideChildOrder = false)
        {
            this.title = title;
            this.icon = iconName;
            this.useIcon = useIcon;
            this.overrideChildOrder = overrideChildOrder;
            this.overrideIcon = overrideIcon;
        }
    }
}

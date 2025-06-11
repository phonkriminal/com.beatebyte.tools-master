using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The e label attribute drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(eLabelAttribute), true)]

    public class eLabelAttributeDrawer : PropertyDrawer
    {
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";
        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUISkin bteSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(AssetDatabase.GUIDToAssetPath(GUISkinGUID));
            GUI.skin = bteSkin;
            var style = new GUIStyle(EditorStyles.label);

            eLabelAttribute elabel = (eLabelAttribute)attribute;

            style = string.IsNullOrEmpty(elabel.style) ? bteSkin.label : bteSkin.GetStyle(elabel.style);

            if (property.propertyType == SerializedPropertyType.String)
            {
                //Debug.Log("TRUE");
                GUIContent content = new(string.IsNullOrEmpty(elabel.label) ? property.stringValue : elabel.label, string.IsNullOrEmpty(elabel.tooltip) ? "" : elabel.tooltip);
                if (content != null)
                {
                    if (!string.IsNullOrEmpty(elabel.icon))
                    {
                        var _logo = Resources.Load(elabel.icon) as Texture2D;

                        if (_logo != null) { content.image = _logo; }
                    }
                }

                if (!string.IsNullOrEmpty(elabel.alignment))
                {
                    style.alignment = GetTextAnchor(elabel.alignment);
                }

                GUILayout.Label(content, style, GUILayout.ExpandHeight(true));
            }

        }

        /// <summary>
        /// Get text anchor.
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        /// <returns>A TextAnchor</returns>
        private TextAnchor GetTextAnchor(string alignment)
        {
            switch (alignment)
            {
                case "UpperLeft":
                    return TextAnchor.UpperLeft;
                case "UpperCenter":
                    return TextAnchor.UpperCenter;
                case "UpperRight":
                    return TextAnchor.UpperRight;
                case "MiddleLeft":
                    return TextAnchor.MiddleLeft;
                case "MiddleCenter":
                    return TextAnchor.MiddleCenter;
                case "MiddleRight":
                    return TextAnchor.MiddleRight;
                case "LowerLeft":
                    return TextAnchor.LowerLeft;
                case "LowerCenter":
                    return TextAnchor.LowerCenter;
                case "LowerRight":
                    return TextAnchor.LowerRight;

                default:
                    return TextAnchor.MiddleCenter;
            }

        }

    }
}
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BeatebyteToolsEditor.Attributes
{

    // Replace 'BasePropertyDrawer' with 'PropertyDrawer' as 'BasePropertyDrawer' is not defined
    /// <summary>
    /// The layer mask attribute drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(eLayerMaskAttribute))]
    public class eLayerMaskAttributeDrawer : PropertyDrawer
    {
        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        public override void OnGUI(UnityEngine.Rect position, UnityEditor.SerializedProperty property, UnityEngine.GUIContent label)
        {
            property.intValue = eLayerMaskDrawer.LayerMaskField(position, label.text, property.intValue);
        }
    }

    /// <summary>
    /// The layer mask drawer.
    /// </summary>
    public static class eLayerMaskDrawer
    {

        /// <summary>
        /// Layers mask field.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="layermask">The layermask.</param>
        /// <returns>An int</returns>
        public static int LayerMaskField(string label, int layermask)
        {
            return FieldToLayerMask(EditorGUILayout.MaskField(label, LayerMaskToField(layermask),
                InternalEditorUtility.layers));
        }

        /// <summary>
        /// Layers mask field.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="label">The label.</param>
        /// <param name="layermask">The layermask.</param>
        /// <returns>An int</returns>
        public static int LayerMaskField(Rect position, string label, int layermask)
        {
            return FieldToLayerMask(EditorGUI.MaskField(position, label, LayerMaskToField(layermask),
                InternalEditorUtility.layers));
        }

        /// <summary>
        /// Converts field LayerMask values to in game LayerMask values
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static int FieldToLayerMask(int field)
        {
            if (field == -1) return -1;
            int mask = 0;
            var layers = InternalEditorUtility.layers;
            for (int c = 0; c < layers.Length; c++)
            {
                if ((field & (1 << c)) != 0)
                {
                    mask |= 1 << LayerMask.NameToLayer(layers[c]);
                }
                else
                {
                    mask &= ~(1 << LayerMask.NameToLayer(layers[c]));
                }
            }

            return mask;
        }

        /// <summary>
        /// Converts in game LayerMask values to field LayerMask values
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        private static int LayerMaskToField(int mask)
        {
            if (mask == -1) return -1;
            int field = 0;
            var layers = InternalEditorUtility.layers;
            for (int c = 0; c < layers.Length; c++)
            {
                if ((mask & (1 << LayerMask.NameToLayer(layers[c]))) != 0)
                {
                    field |= 1 << c;
                }
            }

            return field;
        }
    }

}
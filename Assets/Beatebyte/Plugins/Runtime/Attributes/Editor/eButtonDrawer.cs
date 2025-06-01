using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BeatebyteToolsEditor.Attributes
{
    /// <summary>
    /// The singleton editor.
    /// </summary>
    [CustomPropertyDrawer(typeof(eButtonAttribute))]
    public class SingletonEditor : DecoratorDrawer
    {
        /// <summary>
        /// On GUI.
        /// </summary>
        /// <param name="position">The position.</param>
        public override void OnGUI(Rect position)
        {
            GUISkin skin = Resources.Load("eSkin") as GUISkin;

            GUI.skin = skin;

            eButtonAttribute target = (eButtonAttribute)attribute;

            if (target != null)
            {
                GUI.enabled = target.enabledJustInPlayMode && Application.isPlaying || !target.enabledJustInPlayMode;
                Rect rect = position;
                rect.height = 20;

                if (GUI.Button(rect, new GUIContent(target.label, GUI.enabled ? "Call function " + target.function : "Enabled Just in Play Mode")))
                {
                    ExecuteFunction(target);
                }
                GUI.enabled = true;
            }

        }

        /// <summary>
        /// Get the height.
        /// </summary>
        /// <returns>A float</returns>
        public override float GetHeight()
        {
            return 30f;
        }

        /// <summary>
        /// Execute the function.
        /// </summary>
        /// <param name="target">The target.</param>
        void ExecuteFunction(eButtonAttribute target)
        {
            if (target.type == null) return;
            UnityEngine.Object theObject = Selection.activeGameObject.GetComponent(target.type) as UnityEngine.Object;

            MethodInfo tMethod = theObject.GetType().GetMethods().FirstOrDefault(method => method.Name == target.function
                     && method.GetParameters().Count() == 0);

            if (tMethod != null)
            {
                tMethod.Invoke(theObject, null);
            }
        }
    }
}


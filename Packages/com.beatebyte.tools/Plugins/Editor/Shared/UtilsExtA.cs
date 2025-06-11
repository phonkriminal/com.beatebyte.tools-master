using System;
using UnityEngine;
using UnityEditor;

namespace BeatebyteToolsEditor.Shared
{
    public static partial class Util
    {
        private const int MATERIAL_OPAQUE = 0;
        private const int MATERIAL_TRANSPARENT = 1;

        public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
            Color[] rpixels = result.GetPixels(0);
            float incX = 1.0f / (float)targetWidth;
            float incY = (1.0f / (float)targetHeight);
            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
            }
            result.SetPixels(rpixels, 0);
            result.Apply();
            return result;
        }
        public static Texture2D GetPrefabPreview(string path)
        {
            Debug.Log("Generate preview for " + path);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var editor = UnityEditor.Editor.CreateEditor(prefab);
            Texture2D tex = editor.RenderStaticPreview(path, null, 200, 200);
            EditorWindow.DestroyImmediate(editor);
            return tex;
        }
        public static Texture2D GetPrefabPreview(GameObject gameObject, int width, int height)
        {
            Debug.Log("Generate preview for " + gameObject.transform.name);

            var editor = UnityEditor.Editor.CreateEditor(gameObject);
            string path = AssetDatabase.GetAssetPath(gameObject);
            Texture2D tex = editor.RenderStaticPreview(path, null, width, height);
            EditorWindow.DestroyImmediate(editor);
            return tex;
        }
        /// <summary>
        /// Configures the transparency settings of a material.
        /// </summary>
        /// <remarks>When transparency is enabled, the material's rendering mode is adjusted to support
        /// transparency, including changes to blending, shadow casting, and render queue settings. Disabling
        /// transparency restores the material to an opaque rendering mode.</remarks>
        /// <param name="material">The material to modify. Cannot be <see langword="null"/>.</param>
        /// <param name="enabled"><see langword="true"/> to enable transparency for the material; otherwise, <see langword="false"/> to
        /// disable it.</param>
        public static void SetMaterialTransparent(Material material, bool enabled)
        {
            material.SetFloat("_Surface", enabled ? MATERIAL_TRANSPARENT : MATERIAL_OPAQUE);
            material.SetShaderPassEnabled("SHADOWCASTER", !enabled);
            material.renderQueue = enabled ? 3000 : 2000;
            material.SetFloat("_DstBlend", enabled ? 10 : 0);
            material.SetFloat("_SrcBlend", enabled ? 5 : 1);
            material.SetFloat("_ZWrite", enabled ? 0 : 1);
        }
        /// <summary>
        /// Configures the transparency settings of a material.
        /// </summary>
        /// <remarks>When transparency is enabled, the material's blending mode, ZWrite settings, and
        /// render queue  are adjusted to support alpha blending. Additionally, the material's color alpha is set to 0.1
        /// to ensure partial transparency. When transparency is disabled, the material is restored to  opaque rendering
        /// settings.</remarks>
        /// <param name="material">The material to modify. Cannot be <see langword="null"/>.</param>
        /// <param name="enabled">A value indicating whether transparency should be enabled.  If <see langword="true"/>, the material is set
        /// to render with transparency;  otherwise, it is set to render as opaque.</param>
        public static void SetMaterialTransparentEx(Material material, bool enabled)
        {
            if (enabled)
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;

//                material.color = new Color(material.color.r, material.color.g, material.color.b, 0.1f);
            }
            else 
            {
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
            }
        }

        public static Texture2D AlphaBlend(this Texture2D aBottom, Texture2D aTop)
        {
            if (aBottom.width != aTop.width || aBottom.height != aTop.height)
                throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");
            var bData = aBottom.GetPixels();
            var tData = aTop.GetPixels();
            int count = bData.Length;
            var rData = new Color[count];
            for (int i = 0; i < count; i++)
            {
                Color B = bData[i];
                Color T = tData[i];
                float srcF = T.a;
                float destF = 1f - T.a;
                float alpha = srcF + destF * B.a;
                Color R = (T * srcF + B * B.a * destF) / alpha;
                R.a = alpha;
                rData[i] = R;
            }
            var res = new Texture2D(aTop.width, aTop.height);
            res.SetPixels(rData);
            res.Apply();
            return res;
        }
        public static Texture2D ConvertSpriteToTexture(this Sprite sprite)
        {
            try
            {
                if (sprite.rect.width != sprite.texture.width)
                {
                    Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                    Color[] colors = newText.GetPixels();
                    Color[] newColors = sprite.texture.GetPixels((int)System.Math.Ceiling(sprite.textureRect.x),
                                                                 (int)System.Math.Ceiling(sprite.textureRect.y),
                                                                 (int)System.Math.Ceiling(sprite.textureRect.width),
                                                                 (int)System.Math.Ceiling(sprite.textureRect.height));
                    Debug.Log(colors.Length + "_" + newColors.Length);
                    newText.SetPixels(newColors);
                    newText.Apply();
                    return newText;
                }
                else
                    return sprite.texture;
            }
            catch
            {
                return sprite.texture;
            }
        }
        public static Sprite ToSprite(this Texture2D texture2D)
        {
            try
            {
                Sprite newSprite;

                newSprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100.0f);

                return newSprite;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void SaveTexture(Texture2D texture2D)
        {

            byte[] bytes = texture2D.EncodeToPNG();

            string iconName = "R_" + UnityEngine.Random.Range(0, 100000);

            string path = EditorUtility.SaveFilePanel("Save Icon image file", "Assets/", iconName, "png");

            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetPhysicalPath(path);

            System.IO.File.WriteAllBytes(path, bytes);

            Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + path);
            UnityEditor.AssetDatabase.Refresh();

        }
        public static Texture2D DuplicateTexture(Texture2D source)
        {
            byte[] pix = source.GetRawTextureData();
            Texture2D readableText = new Texture2D(source.width, source.height, source.format, false);
            readableText.LoadRawTextureData(pix);
            readableText.Apply();
            return readableText;
        }
        public static Texture2D DuplicateTextureR(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
    }

  /*  public static class TransformEx
    { /// <summary>
      /// Check if Transfom is children
      /// </summary>
      /// <param name="me"></param>
      /// <param name="target"></param>
      /// <returns></returns>
        public static bool isChild(this Transform me, Transform target)
        {
            if (!target)
            {
                return false;
            }

            var objName = target.gameObject.name;
            var obj = me.FindChildByNameRecursive(objName);
            if (obj == null)
            {
                return false;
            }
            else
            {
                return obj.Equals(target);
            }
        }

        public static Transform FindChildByNameRecursive(this Transform me, string name)
        {
            if (me.name == name)
            {
                return me;
            }

            for (int i = 0; i < me.childCount; i++)
            {
                var child = me.GetChild(i);
                var found = child.FindChildByNameRecursive(name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
        public static Transform FirstOrDefault(this Transform transform, Func<Transform, bool> query)
        {
            if (query(transform))
            {
                return transform;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                var result = FirstOrDefault(transform.GetChild(i), query);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        public static bool TryGetChildComponent<T>(this Transform child, out T reference) where T : Component
        {
            if (child != null)
                return child.TryGetComponent(out reference);

            reference = null;
            return false;
        }
    }*/
    /*public static class ObjectStateExtensions
    {
        public static IStateListener GetStateListener(this GameObject obj)
        {
            return obj.GetComponent<ObjectStateListener>() ?? obj.AddComponent<ObjectStateListener>();
        }

        public interface IStateListener
        {
            event Action Enabled;
            event Action Disabled;
        }

        class ObjectStateListener : MonoBehaviour, IStateListener
        {
            public event Action Enabled;
            public event Action Disabled;

            void Awake()
            {
                hideFlags = HideFlags.DontSaveInBuild | HideFlags.HideInInspector;
            }

            void OnEnable()
            {
                TryInvoke(Enabled);
            }

            void OnDisable()
            {
                TryInvoke(Disabled);
            }

            void TryInvoke(Action action)
            {
                if (action != null)
                    action();
            }
        }
    }*/
}
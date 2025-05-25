using System;
using System.Collections.Generic;
using BeatebyteToolsEditor.Shared;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using static BeatebyteToolsEditor.Shared.Util;
using Object = UnityEngine.Object;

namespace BeatebyteToolsEditor
{  
    public class TextureChannelPacker : EditorWindow
    {
        private static readonly string IconGUID = "643fff9a5f3ccf94aa0a9320c6ca2dba";
        private static readonly string ButtonGUID = "e1446f75c6cf026469e33657a155f135";
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";

        public Texture2D m_metallic, m_ambientOcclusion, m_detailMask, m_smoothness, m_maskMap, m_tex;
        private Texture2D _metallic, _ao, _detail, _smooth;

        public string m_textureName = "Untitled";

        public int m_width = 2048, m_height = 2048;
        public bool m_inverseSmoothness = false;

        Vector2 rect = new(536, 694);

        GUISkin bteSkin;

        Texture2D m_Logo;
        Texture2D m_buttonIcon;

        private int _sizeSelected = 2;
        private int _typeSelected = 0;
        private bool _enableControl;

        

        private readonly string[] m_texturesSize = new string[] { "512x512", "1024x1024", "2048x2048", "4096x4096" };
        private readonly string[] m_textureOut = new string[] { "MASK MAP (M, AO, H, S)", "ALBEDO ALPHA (COLOR, OPACITY)" };
        private string GetPath
        {
            get
            {
                string _path = "";

                if (m_metallic != null)
                {
                    _path = AssetDatabase.GetAssetPath((Object)m_metallic);
                    _path = _path[.._path.IndexOf(m_metallic.name)];
                }

                if (m_ambientOcclusion != null)
                {
                    _path = AssetDatabase.GetAssetPath((Object)m_ambientOcclusion);
                    _path = _path[.._path.IndexOf(m_ambientOcclusion.name)];
                }

                if (m_detailMask != null)
                {
                    _path = AssetDatabase.GetAssetPath((Object)m_detailMask);
                    _path = _path[.._path.IndexOf(m_detailMask.name)];
                }

                if (m_smoothness != null)
                {
                    _path = AssetDatabase.GetAssetPath((Object)m_smoothness);
                    _path = _path[.._path.IndexOf(m_smoothness.name)];
                }

                return _path;
            }
        }

        private static TextureChannelPacker instance = null;

        public TextureChannelPacker()
        {
            instance = this;
        }

        [MenuItem("Window/Beatebyte Creations/Tools/Texture Channel Packer &#t")]

        public static void ShowWindow()
        {
            if (instance == null)
            {
                // "Get existing open window or if none, make a new one:" says documentation.
                // But if called after script reloads a second instance will be opened! => Custom singleton required.
                TextureChannelPacker window = EditorWindow.GetWindow<TextureChannelPacker>(true);
                window.titleContent = new GUIContent("Texture Channel Packer");
                instance = window;
                instance.Show();
            }
            else
            {
                instance.Focus();
            }
        }
        
        Rect buttonRect;

        public void OnGUI()
        {
            m_Logo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(IconGUID));
            m_buttonIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(ButtonGUID));

            Texture2D _logo = ScaleTexture(m_Logo, 48, 48);
            Texture2D _buttonIcon = ScaleTexture(m_buttonIcon, 32, 32);

            if (!bteSkin)
            {
                bteSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(AssetDatabase.GUIDToAssetPath(GUISkinGUID));
            }
            GUI.skin = bteSkin;

            instance.minSize = rect;
            instance.maxSize = rect;

            GUIStyle labelStyle = new GUIStyle(bteSkin.GetStyle("bteLabelTex"));
            GUIStyle labelRightStyle = new GUIStyle(bteSkin.GetStyle("bteLabelTexRight"));
            GUIStyle textStyle = new GUIStyle(bteSkin.GetStyle("bteTextfield"));
            GUIStyle intStyle = new GUIStyle(bteSkin.GetStyle("intfield"));
            GUIStyle titleStyle = new GUIStyle(bteSkin.GetStyle("bteTitle"));
            GUIStyle textureBoxStyle = new GUIStyle(bteSkin.GetStyle("bteTextureBox"));

            textStyle.fontSize = 12;

            GUILayout.BeginVertical("       TEXTURE CHANNEL PACKER", titleStyle);
            {
                GUILayout.Label(m_Logo, GUILayout.MaxHeight(48));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("TEXTURES", textureBoxStyle);
            {
                GUILayout.Space(20);

                GUILayoutOption[] layoutOptions = new GUILayoutOption[] { GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(40) };
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(400), GUILayout.Height(EditorGUIUtility.singleLineHeight) };

                GUILayout.BeginVertical("", textureBoxStyle);
                {
                    GUILayout.BeginHorizontal();
                    {

                        EditorGUILayout.LabelField("Name", labelStyle);


                        GUI.SetNextControlName("inText");

                        m_textureName = EditorGUILayout.TextField(m_textureName, textStyle, options);

                        GUI.SetNextControlName("inButton");

                        if (GUILayout.Button(_buttonIcon, "buttonicon", layoutOptions))
                        {
                            if (m_metallic != null)
                            {
                                m_textureName = m_metallic.name + ((_typeSelected == 0) ? "_maskMap" : "_opacity");
                                Debug.Log(m_textureName);
                                GUI.FocusControl("inText");
                                GUI.FocusControl("inButton");
                            }
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(20);

                    options = new GUILayoutOption[] { GUILayout.MaxWidth(50) };
                    GUIStyle _popupStyle = new GUIStyle();
                    _popupStyle = EditorStyles.popup;
                    _popupStyle.font = textureBoxStyle.font;
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Width", labelStyle);
                        GUI.SetNextControlName("width");
                        m_width = EditorGUILayout.IntField(m_width, intStyle, options);
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("Height", labelStyle);
                        GUI.SetNextControlName("height");
                        m_height = EditorGUILayout.IntField(m_height, intStyle, options);
                        GUILayout.Space(100);

                        EditorGUI.BeginChangeCheck();



                        _sizeSelected = EditorGUILayout.Popup(_sizeSelected, m_texturesSize, _popupStyle);

                        if (EditorGUI.EndChangeCheck())
                        {
                            //Debug.Log(m_texturesSize[_sizeSelected][..m_texturesSize[_sizeSelected].IndexOf("x")]);
                            int textureSize = int.Parse(m_texturesSize[_sizeSelected][..m_texturesSize[_sizeSelected].IndexOf("x")]);
                            m_width = textureSize;
                            m_height = textureSize;

                            GUI.FocusControl("height");
                            GUI.FocusControl("width");
                        }
                    }
                    GUILayout.EndHorizontal();

                    //if (GUILayout.Button("Textures Size", GUILayout.Width(200)))

                    EditorGUILayout.Separator();

                    GUILayout.BeginHorizontal();
                    {

                        EditorGUILayout.LabelField("Select Texture Output", labelStyle);
                        _typeSelected = EditorGUILayout.Popup(_typeSelected, m_textureOut, _popupStyle);

                        //Debug.Log(m_textureOut[_typeSelected]);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical("", "ObjectField");
                {

                    GUILayoutOption[] textureFieldLayout = new GUILayoutOption[]
                    {
                        GUILayout.Height(150),
                        GUILayout.Width(250),
                        GUILayout.ExpandWidth(false),
                    GUILayout.ExpandHeight(false),
                    };

                    Texture2D _red = new Texture2D(250, 150);
                    Texture2D _green = new Texture2D(250, 150);
                    Texture2D _blue = new Texture2D(250, 150);
                    Texture2D _alpha = new Texture2D(250, 150);

                    #region Upper

                    GUILayout.BeginHorizontal();
                    {
                        _red.SimpleTexture(BackColor.Red, 250, 150);
                        _green.SimpleTexture(BackColor.Green, 250, 150);
                        _blue.SimpleTexture(BackColor.Blue, 250, 150);
                        _alpha.SimpleTexture(BackColor.Alpha, 250, 150);
                        labelStyle.fontSize = 12;
                        GUILayout.BeginVertical(_red, "TextureGUI", textureFieldLayout);
                        {
                            //GUI.skin = null;
                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("RED", labelStyle, GUILayout.Width(30));
                                m_metallic = ShowTextureGUI("", m_metallic);

                                if (m_metallic)
                                {
                                    Rect rect = GUILayoutUtility.GetLastRect();
                                    rect.y += EditorGUIUtility.singleLineHeight + 10;
                                    rect.width = 100;
                                    rect.height = rect.width;
                                    rect.x = 250 / 2 - rect.width / 2 + 10;
                                    EditorGUI.DrawPreviewTexture(rect, m_metallic);

                                }

                                //GUI.skin = bteSkin;
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);

                        GUILayout.BeginVertical(_green, "TextureGUI", textureFieldLayout);
                        {
                            //GUI.skin = null;
                            GUILayout.BeginHorizontal();
                            {

                                _enableControl = (_typeSelected == 0);
                                GUI.enabled = _enableControl;

                                EditorGUILayout.LabelField("GREEN", labelStyle);
                                m_ambientOcclusion = ShowTextureGUI("", m_ambientOcclusion);

                                if (m_ambientOcclusion)
                                {
                                    Rect rect = GUILayoutUtility.GetLastRect();
                                    rect.y += EditorGUIUtility.singleLineHeight + 10;
                                    rect.width = 100;
                                    rect.height = rect.width;
                                    rect.x = 250 / 2 - rect.width / 2 + 270;
                                    EditorGUI.DrawPreviewTexture(rect, m_ambientOcclusion);
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                    #endregion

                    GUILayout.Space(5);

                    #region Lower
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.BeginVertical(_blue, "TextureGUI", textureFieldLayout);
                        {
                            //GUI.skin = null;
                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("BLUE", labelStyle);

                                m_detailMask = ShowTextureGUI("", m_detailMask);

                                if (m_detailMask)
                                {
                                    Rect rect = GUILayoutUtility.GetLastRect();
                                    rect.y += EditorGUIUtility.singleLineHeight + 10;
                                    rect.width = 100;
                                    rect.height = rect.width;
                                    rect.x = 250 / 2 - rect.width / 2 + 10;
                                    EditorGUI.DrawPreviewTexture(rect, m_detailMask);
                                }
                                //GUI.skin = bteSkin;
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);

                        GUILayout.BeginVertical(_alpha, "TextureGUI", textureFieldLayout);
                        {
                            //GUI.skin = null;
                            GUILayout.BeginHorizontal();
                            {
                                GUI.enabled = true;
                                EditorGUILayout.LabelField("ALPHA", labelStyle);
                                m_smoothness = ShowTextureGUI("", m_smoothness);
                                if (m_smoothness)
                                {
                                    Rect rect = GUILayoutUtility.GetLastRect();
                                    rect.y += EditorGUIUtility.singleLineHeight + 10;
                                    rect.width = 100;
                                    rect.height = rect.width;
                                    rect.x = 250 / 2 - rect.width / 2 + 275;
                                    EditorGUI.DrawPreviewTexture(rect, m_smoothness);
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                    #endregion
                }

                GUILayout.BeginHorizontal("rightBox");
                {
                    EditorGUILayout.LabelField("Invert Roughness / Smoothness", labelRightStyle);
                    m_inverseSmoothness = GUILayout.Toggle(m_inverseSmoothness, "", "toggle", GUILayout.Width(20));
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();



                EditorGUILayout.Separator();

                if (GUILayout.Button("Pack Textures"))
                {
                    PackTextures();
                }

                if (GUILayout.Button("Clear"))
                {
                    Clear();
                }
            }
            GUILayout.EndVertical();
        }


        private void Clear()
        {
            m_metallic = null;
            m_ambientOcclusion = null;
            m_detailMask = null;
            m_smoothness = null;
            m_maskMap = null;
            m_textureName = "Untitled";
            m_width = 2048;
            m_height = 2048;
            m_inverseSmoothness = false;
            _sizeSelected = 2;
            _typeSelected = 0;
            GUI.FocusControl("inText");
        }
        private void PackTextures()
        {
            if (m_metallic == null && m_ambientOcclusion == null && m_detailMask == null && m_smoothness == null) return;
            AdjustTexturesSize(m_width, m_height);

            m_maskMap = new Texture2D(m_width, m_height);
            m_maskMap.SetPixels(ColorsArray());

            byte[] m_tex = m_maskMap.EncodeToPNG();

            string path = EditorUtility.SaveFilePanel("Save Icon image file", "Assets/", m_textureName, "png");

            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetPhysicalPath(path);

            System.IO.File.WriteAllBytes(path, m_tex);
            Debug.Log(m_tex.Length / 1024 + "Kb was saved as: " + path);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            /*
            FileStream _stream = new FileStream(GetPath + m_textureName + ".png", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            BinaryWriter _writer = new(_stream);

            for (int i = 0; i < m_tex.Length; i++)
            {
                _writer.Write(m_tex[i]);
            }

            _stream.Close();
            _writer.Close();

            AssetDatabase.ImportAsset(GetPath + m_textureName + ".png", ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();*/
        }

        private void AdjustTexturesSize(int t_width, int t_height)
        {

            if (m_metallic != null)
            {
                _metallic = GPUTextureScaler.Scaled(m_metallic, t_width, t_height, FilterMode.Bilinear);
            }
            if (m_ambientOcclusion != null)
            {
                _ao = GPUTextureScaler.Scaled(m_ambientOcclusion, t_width, t_height, FilterMode.Bilinear);
            }
            if (m_detailMask != null)
            {
                _detail = GPUTextureScaler.Scaled(m_detailMask, t_width, t_height, FilterMode.Bilinear);
            }
            if (m_smoothness != null)
            {
                _smooth = GPUTextureScaler.Scaled(m_smoothness, t_width, t_height, FilterMode.Bilinear);
            }
        }

        private Color[] ColorsArray()
        {
            Color[] m_colors = new Color[m_width * m_height];

            for (int i = 0; i < m_colors.Length; i++)
            {
                m_colors[i] = new Color();

                if (_metallic != null)
                {
                    m_colors[i].r = _metallic.GetPixel(i % m_width, i / m_width).r;
                }
                else
                {
                    m_colors[i].r = 1;
                }

                if (_ao != null)
                {
                    m_colors[i].g = _ao.GetPixel(i % m_width, i / m_width).g;
                }
                else
                {
                    if (_typeSelected == 1) m_colors[i].g = _metallic.GetPixel(i % m_width, i / m_width).g;
                    else m_colors[i].g = 1;
                }

                if (_detail != null)
                {
                    m_colors[i].b = _detail.GetPixel(i % m_width, i / m_width).b;
                }
                else
                {
                    if (_typeSelected == 1) m_colors[i].b = _metallic.GetPixel(i % m_width, i / m_width).b;
                    else m_colors[i].b = 1;
                }

                if (_smooth != null)
                {
                    m_colors[i].a = m_inverseSmoothness ? 1 - _smooth.GetPixel(i % m_width, i / m_width).r : _smooth.GetPixel(i % m_width, i / m_width).r;
                }
                else
                {
                    m_colors[i].a = 1;
                }
            }

            return m_colors;

        }
        /*      public static class CustomEditorGUILayout
                {
                    public static void ObjectField<T>(string label, T obj, bool allowSceneReferences) where T : UnityEngine.Object
                    {
                        obj = (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneReferences);
                    }
                }*/
        public Texture2D ShowTextureGUI(string fieldName, Texture2D texture)
        {
            GUILayoutOption[] options = { GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(150), };
            return (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, options);
        }
    }

}
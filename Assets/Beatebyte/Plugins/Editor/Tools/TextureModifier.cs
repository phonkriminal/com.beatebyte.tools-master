using UnityEditor;
using UnityEngine;
using BeatebyteToolsEditor.Shared;
using static BeatebyteToolsEditor.Shared.Util;

namespace BeatebyteToolsEditor
{ 
    /// <summary>
    /// The texture modifier.
    /// </summary>
    public class TextureModifier : EditorWindow
    {
        private static readonly string IconGUID = "643fff9a5f3ccf94aa0a9320c6ca2dba";
        private static readonly string ButtonGUID = "e1446f75c6cf026469e33657a155f135";
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";

        /// <summary>
        /// The rect.
        /// </summary>
        Vector2 rect = new(536, 710);

        /// <summary>
        /// The E skin.
        /// </summary>
        private GUISkin bteSkin;

        /// <summary>
        /// The M logo.
        /// </summary>
        private Texture2D m_Logo;
        /// <summary>
        /// The mbutton icon.
        /// </summary>
        private Texture2D m_buttonIcon;

        /// <summary>
        /// The mtextures size.
        /// </summary>
        private readonly string[] m_texturesSize = new string[] { "512x512", "1024x1024", "2048x2048", "4096x4096" };
        /// <summary>
        /// The mtexture out.
        /// </summary>
        private readonly string[] m_textureOut = new string[] { "ADD TEXTURE", "SUBTRACT TEXTURE", "MULTIPLY TEXTURE", "BLEND TOP", "BLEND BOTTOM", "ADD COLOR", "SUBTRACT COLOR", "MULTIPLY COLOR" };

        /// <summary>
        /// The size selected.
        /// </summary>
        private int _sizeSelected = 2;
        /// <summary>
        /// Type selected.
        /// </summary>
        private int _typeSelected = 0;
        /// <summary>
        /// The current type.
        /// </summary>
        private int _currentType = 0;
        /// <summary>
        /// Show preview.
        /// </summary>
        private bool _showPreview;

        /// <summary>
        /// The mtexture name.
        /// </summary>
        public string m_textureName = "Untitled";
        /// <summary>
        /// The M width.
        /// </summary>
        public int m_width = 2048, m_height = 2048;

        /// <summary>
        /// The M texture1.
        /// </summary>
        public Texture2D m_texture1, m_texture2, m_textureMod, _texture1, _texture2;
        /// <summary>
        /// The M color.
        /// </summary>
        public Color m_color = Color.white;
        /// <summary>
        /// The current tex1.
        /// </summary>
        private string currentTex1 = string.Empty, currentTex2 = string.Empty;
        /// <summary>
        /// The current color.
        /// </summary>
        private Color currentColor = Color.white;

        /// <summary>
        /// The instance.
        /// </summary>
        private static TextureModifier instance = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModifier"/> class.
        /// </summary>
        public TextureModifier()
        {
            instance = this;
        }

        /// <summary>
        /// Show the window.
        /// </summary>
        [MenuItem("Window/Beatebyte Creations/Tools/Texture Modifier %#t")]
        public static void ShowWindow()
        {
            if (instance == null)
            {
                // "Get existing open window or if none, make a new one:" says documentation.
                // But if called after script reloads a second instance will be opened! => Custom singleton required.
                TextureModifier window = EditorWindow.GetWindow<TextureModifier>(true);
                window.titleContent = new GUIContent("Texture Modifier");
                instance = window;
                instance.Show();
                GUIStyle colorStyle = new GUIStyle();
                colorStyle = EditorStyles.colorField;
            }
            else
            {
                instance.Focus();
            }
        }

        /// <summary>
        /// On GUI.
        /// </summary>
        public void OnGUI()
        {
            m_Logo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(IconGUID));
            m_buttonIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(ButtonGUID));

            Texture2D _logo = ScaleTexture(m_Logo, 50, 50);
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
            GUIStyle textureOutStyle = new GUIStyle(bteSkin.GetStyle("bteTextureOut"));
            GUIStyle textureObjectStyle = new GUIStyle(bteSkin.GetStyle("bteTextureObj"));
            GUIStyle textureGUIStyle = new GUIStyle(bteSkin.GetStyle("TextureGUI"));


            GUIStyle _popupStyle = new GUIStyle();

            _popupStyle = EditorStyles.popup;
            _popupStyle.font = textureBoxStyle.font;


            GUILayout.BeginVertical("       TEXTURE MODIFIER TOOL", titleStyle);
            {
                GUILayout.Label(m_Logo, GUILayout.MaxHeight(48));
            }
            GUILayout.EndVertical();

            textStyle.fontSize = 12;

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
                            if (m_texture1 != null)
                            {
                                m_textureName = m_texture1.name + "_mod";//((_typeSelected == 0) ? "_maskMap" : "_opacity");
                                Debug.Log(m_textureName);
                                GUI.FocusControl("inText");
                                GUI.FocusControl("inButton");
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(20);

                    options = new GUILayoutOption[] { GUILayout.MaxWidth(50) };

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
                            // Debug.Log(m_texturesSize[_sizeSelected][..m_texturesSize[_sizeSelected].IndexOf("x")]);
                            int textureSize = int.Parse(m_texturesSize[_sizeSelected][..m_texturesSize[_sizeSelected].IndexOf("x")]);
                            m_width = textureSize;
                            m_height = textureSize;

                            GUI.FocusControl("height");
                            GUI.FocusControl("width");
                        }
                    }
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Separator();

                    GUILayout.BeginHorizontal();
                    {

                        EditorGUILayout.LabelField("Select Texture Modifier", labelStyle);

                        _typeSelected = EditorGUILayout.Popup(_typeSelected, m_textureOut, _popupStyle);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Show Preview", labelStyle);
                        _showPreview = EditorGUILayout.Toggle(_showPreview, "toggle");
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical("", "ObjectField"); // object field

                GUILayoutOption[] textureFieldLayout = new GUILayoutOption[]
                {
                    GUILayout.Height(150),
                    GUILayout.Width(250),
                    GUILayout.ExpandWidth(false),
                    GUILayout.ExpandHeight(false),
                };
                GUILayoutOption[] textureObjLayout = new GUILayoutOption[]
                {
                    GUILayout.Height(100),
                    GUILayout.Width(100),
                    GUILayout.ExpandWidth(false),
                    GUILayout.ExpandHeight(false),
                };


                GUILayout.BeginHorizontal();
                {  // UPPER

                    GUILayout.BeginVertical("", textureGUIStyle, textureFieldLayout);
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("SOURCE 1", labelStyle, GUILayout.Width(80));
                            m_texture1 = ShowTextureGUI("", m_texture1);
                        }
                        GUILayout.EndHorizontal();
                        if (m_texture1)
                        {
                            EditorGUILayout.Space(5);
                            GUILayout.BeginVertical(m_texture1, textureObjectStyle, textureObjLayout);
                            EditorGUILayout.Space(100);
                            GUILayout.EndVertical();
                        }                        
                    }
                    GUILayout.EndVertical();

                    GUILayout.Space(5);

                    GUILayout.BeginVertical("", textureGUIStyle, textureFieldLayout);
                    {
                        GUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("SOURCE 2", labelStyle, GUILayout.Width(80));
                            if (_typeSelected < 5)
                            {
                                m_texture2 = ShowTextureGUI("", m_texture2);
                            }
                            else
                            {
                                GUI.skin = null;
                                m_color = EditorGUILayout.ColorField(m_color);
                                GUI.skin = bteSkin;
                            }
                        }
                        GUILayout.EndHorizontal();
                        if (_typeSelected < 5 && m_texture2)
                        {
                            EditorGUILayout.Space(5);
                            GUILayout.BeginVertical(m_texture2, textureObjectStyle, textureObjLayout);
                            EditorGUILayout.Space(100);
                            GUILayout.EndVertical();
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();  // UPPER

                GUILayout.Space(5);

                GUILayout.BeginVertical("RESULT", textureBoxStyle, new GUILayoutOption[] { GUILayout.Width(505), GUILayout.Height(150) });
                { // LOWER
                    GUILayout.Space(5);

                    if (m_texture1 && m_texture1.isReadable == false)
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.HelpBox("Texture Source ONE is not readable!", MessageType.Error);
                    }
                    if (m_texture2 && m_texture2.isReadable == false)
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.HelpBox("Texture Source TWO is not readable!", MessageType.Error);
                    }
                    if ((m_texture1 && m_texture2) && (m_texture1.height != m_texture2.height | m_texture1.width != m_texture2.width))
                    {
                        GUILayout.Space(10);
                        EditorGUILayout.HelpBox("Input Textures have different sizes!", MessageType.Error);
                    }

                    if (CheckInputTextures() && _typeSelected < 5)
                    {

                        EditorGUILayout.Space(15);
                        if ((currentTex1 != m_texture1.name | currentTex2 != m_texture2.name | _currentType != _typeSelected) && _showPreview)
                        {
                            Debug.Log("Perform");
                            m_textureMod = PerformModifier(_typeSelected, m_texture1);
                            currentTex1 = m_texture1.name;
                            currentTex2 = m_texture2.name;
                            _currentType = _typeSelected;
                        }
                        if (!_showPreview)
                        {
                            m_textureMod = null;
                        }
                        //m_textureMod.Apply();
                        GUILayout.BeginVertical(m_textureMod, textureOutStyle, textureObjLayout);
                        {
                            EditorGUILayout.Space(100);
                        }
                        GUILayout.EndVertical();
                    }

                    if (m_texture1 != null && _typeSelected >= 5)
                    {
                        EditorGUILayout.Space(15);
                        if ((currentTex1 != m_texture1.name | currentColor != m_color) && _showPreview)
                        {
                            Debug.Log("Perform with Color");
                            m_textureMod = PerformModifier(_typeSelected, m_texture1);
                            currentTex1 = m_texture1.name;
                            currentColor = m_color;
                            _currentType = _typeSelected;
                        }
                        if (!_showPreview)
                        { 
                            m_textureMod = null;
                        }
                        GUILayout.BeginVertical(m_textureMod, textureOutStyle, textureObjLayout);
                        {
                            EditorGUILayout.Space(100);
                        }
                        GUILayout.EndVertical();
                    }

                }
                GUILayout.EndVertical();    // object field
            }
            GUILayout.EndVertical();    //  MAIN
           
            EditorGUILayout.Separator();

            if (GUILayout.Button("Pack Textures"))
            {
                PackTextures();
            }

            if (GUILayout.Button("Clear"))
            {
                Clear();
            }

            GUILayout.BeginHorizontal("box");
            {
                string t1Size = m_texture1 ? $"{m_texture1.name} : {m_texture1.width}x{m_texture1.height}" : string.Empty;
                string t2Size = m_texture2 ? $"  --  {m_texture2.name} : {m_texture2.width}x{m_texture2.height}" : string.Empty;
                EditorGUILayout.LabelField(t1Size + t2Size, EditorStyles.miniLabel, GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();

        }

        /// <summary>
        /// Check input textures.
        /// </summary>
        /// <returns>A bool</returns>
        private bool CheckInputTextures()
        {
            return (m_texture1 && m_texture2 && m_texture1.isReadable && m_texture2.isReadable && m_texture1.width == m_texture2.width && m_texture1.height == m_texture2.height);
        }

        /// <summary>
        /// Pack the textures.
        /// </summary>
        private void PackTextures()
        {
            if (CheckInputTextures() && _typeSelected < 5)
            {
                Texture2D _saveTexture = PerformModifier(_typeSelected, m_texture1);
                if (_saveTexture.width != m_width)
                {
                    _saveTexture = AdjustTextureSize(_saveTexture, m_width, m_height);
                    _saveTexture.Apply();
                }

                byte[] m_tex = _saveTexture.EncodeToPNG();

                string path = EditorUtility.SaveFilePanel("Save Icon image file", "Assets/", m_textureName, "png");

                if (string.IsNullOrEmpty(path)) return;

                path = FileUtil.GetPhysicalPath(path);

                System.IO.File.WriteAllBytes(path, m_tex);
                Debug.Log(m_tex.Length / 1024 + "Kb was saved as: " + path);
            }
            
            if (m_texture1 && _typeSelected >= 5)
            {
                Texture2D _saveTexture = PerformModifier(_typeSelected, m_texture1);
                if (_saveTexture.width != m_width)
                {
                    _saveTexture = AdjustTextureSize(_saveTexture, m_width, m_height);
                    _saveTexture.Apply();
                }

                byte[] m_tex = _saveTexture.EncodeToPNG();

                string path = EditorUtility.SaveFilePanel("Save Icon image file", "Assets/", m_textureName, "png");

                if (string.IsNullOrEmpty(path)) return;

                path = FileUtil.GetPhysicalPath(path);

                System.IO.File.WriteAllBytes(path, m_tex);
                Debug.Log(m_tex.Length / 1024 + "Kb was saved as: " + path);

            }

            UnityEditor.AssetDatabase.Refresh();
        }

        private void Clear()
        {
            m_texture1 = null;
            m_texture2 = null;
            m_textureMod = null;
            m_textureName = "Untitled";
            m_width = 2048;
            m_height = 2048;
            _sizeSelected = 2;
            _typeSelected = 0;
            GUI.FocusControl("inText");
            currentTex1 = string.Empty;
            currentTex2 = string.Empty;
            _showPreview = false;
            currentColor = Color.white;
        }
        private Texture2D AdjustTextureSize(Texture2D texture, int t_width, int t_height)
        {
            return GPUTextureScaler.Scaled(texture, t_width, t_height, FilterMode.Bilinear);
        }
        public Texture2D PerformModifier(int operation, Texture2D texture)
        {
            Texture2D temp = texture.Copy();
            Texture2D temp2 = m_texture2? m_texture2.Copy() : null;

            switch (operation)
            {
                case 0:
                    temp.Add(temp2);
                    break;
                case 1:
                    temp.Subtract(temp2);
                    break;
                case 2:
                    temp.Multiply(temp2);
                    break;
                case 3:
                    temp.BlendTop(temp2);
                    break;
                case 4:
                    temp.BlendBottom(temp2);
                    break;
                case 5:
                    temp.Add(m_color);
                    break;
                case 6:
                    temp.Subtract(m_color);
                    break;
                case 7:
                    temp.Multiply(m_color);
                    break;
                default:
                    break;
            }

            temp.Apply();

            return temp;
        }
        public Texture2D ShowTextureGUI(string fieldName, Texture2D texture)
        {
            GUILayoutOption[] options = { GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(150), };
            return (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, options);
        }
    }
}
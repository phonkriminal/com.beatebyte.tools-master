using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BeatebyteToolsEditor.Shared.Util;

namespace BeatebyteToolsEditor
{
    /// <summary>
    /// The EDS create icon from prefab editor.
    /// </summary>
    public class EDSCreateIconFromPrefabEditor : EditorWindow
    {
        private static readonly string IconGUID = "643fff9a5f3ccf94aa0a9320c6ca2dba";
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";
        private static readonly string IconCanvasGUID = "e13d99fe381414c4aa2ac68a615a2164";
        private static readonly string ItemCameraGUID = "6b96d4a9f99a40b449927b426be259ad";
        private static readonly string EmptyTextureGUID = "6920c2c072a84704d8e3025edb20cacf";
        private static readonly string BorderTextureGUID = "ceed8b93ecf77aa48aa29e1431ff422b";


        /// <summary>
        /// The logo.
        /// </summary>
        private Texture2D logo;
        /// <summary>
        /// The logo.
        /// </summary>
        private Texture2D _logo;

        /// <summary>
        /// The es skin.
        /// </summary>
        private GUISkin bteSkin;

        /// <summary>
        /// The min rect.
        /// </summary>
        private Vector2 minRect = new Vector2(500, 700);

        /// <summary>
        /// Check again.
        /// </summary>
        private bool checkAgain = false;

        /// <summary>
        /// The char obj.
        /// </summary>
        public GameObject charObj;

        /// <summary>
        /// The humanoidpreview.
        /// </summary>
        private UnityEditor.Editor humanoidpreview;

        /// <summary>
        /// The size index.
        /// </summary>
        public int sizeIndex = 0;
        /// <summary>
        /// The invert layers.
        /// </summary>
        [field: SerializeField]
        public bool InvertLayers { get; set; } = false;

        [field: SerializeField]
        public bool UseBorders { get; set; } = false;
        /// <summary>
        /// Back ground.
        /// </summary>
        public Sprite _backGround;
        
        private Sprite _originalSprite;
        /// <summary>
        /// The camera prefab.
        /// </summary>
        public GameObject _cameraPrefab;
        /// <summary>
        /// The item canvas prefab.
        /// </summary>
        public GameObject _itemCanvasPrefab;
        /// <summary>
        /// emptyTexture.
        /// </summary>
        private Texture2D emptyTexture;
        private Texture2D borderTexture;
        /// <summary>
        /// The off set.
        /// </summary>
        [SerializeField]
        [Range(0, 32)]
        private int offSet = 10;
        /// <summary>
        /// The icon name.
        /// </summary>
        [SerializeField]
        private string iconName;

        /// <summary>
        /// The item canvas.
        /// </summary>
        [HideInInspector]
        public GameObject _itemCanvas;
        /// <summary>
        /// Can save.
        /// </summary>
        private bool canSave;
        /// <summary>
        /// The item camera.
        /// </summary>
        [HideInInspector]
        public Camera _itemCamera;

        #region CAMERA Preview
        /// <summary>
        /// The camera.
        /// </summary>
        private Camera _camera;
        /// <summary>
        /// The camera render size.
        /// </summary>
        private Vector2 _cameraRenderSize = new Vector2(1080, 1920);

        //set these to skybox if you use skybox
        /// <summary>
        /// Clear flags.
        /// </summary>
        private CameraClearFlags _clearFlags = CameraClearFlags.SolidColor;
        /// <summary>
        /// Render texture.
        /// </summary>
        private RenderTexture _renderTexture;

        /// <summary>
        /// The CONTROL AREA HEIGHT.
        /// </summary>
        private const float CONTROL_AREA_HEIGHT = 80;
        #endregion

        /// <summary>
        /// The image size.
        /// </summary>
        public string[] imageSize = new string[] { "64x64", "128x128", "256x256", "512x512", "1024x1024" };

        /// <summary>
        /// Is camera.
        /// </summary>
        private bool isCamera, isCanvas = false;
        /// <summary>
        /// The icon size.
        /// </summary>
        private int iconSize = 64;

        /// <summary>
        /// On enable.
        /// </summary>
        private void OnEnable()
        {
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(IconGUID));
            _logo = ScaleTexture(logo, 48, 48);
            _itemCanvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(IconCanvasGUID));
            _cameraPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(ItemCameraGUID));
            emptyTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(EmptyTextureGUID));
            borderTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(BorderTextureGUID));
            _backGround = emptyTexture.ToSprite();
            _originalSprite = _backGround;
            CheckConditions();
        }

        [MenuItem("Window/Beatebyte Creations/Tools/Icon Generator", false, 1)]
        public static void Init()
        {
            EDSCreateIconFromPrefabEditor window = GetWindow<EDSCreateIconFromPrefabEditor>();
            window.autoRepaintOnSceneChange = true;
            window.Populate();
            window.Show();
        }
        private void Populate()
        {
            RefreshRenderSize();
            RefreshTexture();
        }
        private void Update()
        {
            if (_camera != null)
            {
                Scene previousScene = _camera.scene;
                CameraClearFlags previousClearFlags = _camera.clearFlags;
                RenderTexture previousTexture = _camera.targetTexture;

                //tells the camera to render the prefab scene if it's in one
                if (PrefabUtility.IsPartOfAnyPrefab(_camera))
                {
                    PrefabStage stage = PrefabStageUtility.GetPrefabStage(_camera.gameObject);
                    if (stage != null)
                    {
                        _camera.scene = stage.scene;
                    }
                }

                //i use clear depth for my cameras so i want to override the clearflags
                _camera.clearFlags = _clearFlags;

                _camera.targetTexture = _renderTexture;
                _camera.Render();

                //reset camera values to what they were
                _camera.targetTexture = previousTexture;
                _camera.scene = previousScene;
                _camera.clearFlags = previousClearFlags;
            }
        }
        private void RefreshRenderSize()
        {
            string[] screenRes = new string[2] { _itemCanvasPrefab.GetComponent<Canvas>().renderingDisplaySize.y.ToString(), _itemCanvasPrefab.GetComponent<Canvas>().renderingDisplaySize.x.ToString() };

            _cameraRenderSize = new Vector2(int.Parse(screenRes[0]), int.Parse(screenRes[1]));
        }
        private void RefreshTexture()
        {
            //_renderTexture = new RenderTexture((int)_cameraRenderSize.x, (int)_cameraRenderSize.y, 24);
            _renderTexture = new RenderTexture(iconSize, iconSize, 24);
        }
        private void CheckConditions()
        {
            if (Selection.activeObject)
            {
                charObj = Selection.activeGameObject;
            }
            if (charObj)
            {
                checkAgain = false;
                humanoidpreview = UnityEditor.Editor.CreateEditor(charObj);
            }
            isCamera = _cameraPrefab;
            isCanvas = _itemCanvasPrefab;
        }
        public Sprite GetIcon()
        {
            Renderer renderer = charObj.transform.GetComponentInChildren<Renderer>();
            //_itemCamera.orthographicSize = renderer.bounds.extents.y + 0.1f;

            // Get our dimensions
            int resX = _itemCamera.pixelWidth;
            int resY = _itemCamera.pixelHeight;

            // Variables for clipping image down to square

            int clipX = 0;
            int clipY = 0;

            if (resX > resY)
                clipX = resX - resY;
            else if (resY > resX)
                clipY = resY - resX;


            // Initialise all parts.
            Texture2D tex = new Texture2D(resX - clipX, resY - clipY, TextureFormat.RGBA32, false);
            RenderTexture renderTexture = new(resX, resY, 24);
            _itemCamera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            // Grab the icon and stick it in the texture.
            _itemCamera.Render();
            tex.ReadPixels(new Rect(clipX / 2, clipY / 2, resX, resY), 0, 0);
            tex.Apply();

            //Clean up.

            _itemCamera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(renderTexture);

            // Convert Texture2D into Sprite and return it.

            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

        }
        private void OnGUI()
        {            

            Event e = Event.current;
            if (e.isScrollWheel && canSave)
            {
                UpdateCameraSize(e.delta);
            }
            else if (e.isMouse && canSave)
            {

                if (e.type == EventType.MouseDrag && !e.alt && !e.control)
                {
                    UpdateCameraPosition(e.delta);
                }
                else if (e.type == EventType.MouseDrag && e.alt)
                {
                    charObj?.transform.Rotate(Vector3.up, e.delta.x, Space.Self);
                }
                else if (e.type == EventType.MouseDrag && e.control)
                {
                    charObj?.transform.Rotate(Vector3.right, e.delta.x, Space.Self);
                }

            }

            logo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(IconGUID));
            _logo = ScaleTexture(logo, 48, 48);

            if (!bteSkin)
            {
                bteSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(AssetDatabase.GUIDToAssetPath(GUISkinGUID));
            }

            GUI.skin = bteSkin;

            GUIStyle labelStyle = new GUIStyle(bteSkin.GetStyle("bteLabelTex"));
            GUIStyle labelRightStyle = new GUIStyle(bteSkin.GetStyle("bteLabelTexRight"));
            GUIStyle textStyle = new GUIStyle(bteSkin.GetStyle("bteTextfield"));
            GUIStyle intStyle = new GUIStyle(bteSkin.GetStyle("intfield"));
            GUIStyle titleStyle = new GUIStyle(bteSkin.GetStyle("bteTitle"));
            GUIStyle textureBoxStyle = new GUIStyle(bteSkin.GetStyle("bteTextureBox"));

            //Texture2D preview;

            this.minSize = minRect;
            this.maxSize = minRect;
            this.titleContent = new GUIContent("Icon Prefab Creator", null, "Icon Prefab Creator");

            GUILayout.BeginVertical("       ICON PREFAB EDITOR", titleStyle);
            {
                GUILayout.Label(logo, GUILayout.MaxHeight(48));
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical("", textureBoxStyle);
            {

                EditorGUILayout.Separator();

                if (checkAgain) CheckConditions();


                GUILayout.BeginVertical();
                {

                    if (!charObj)
                    {
                        EditorGUILayout.HelpBox("Select FBX model to generate Icon!", MessageType.Info);
                        checkAgain = true;
                        canSave = false;
                    }
                    else if (!isCamera)
                    {
                        EditorGUILayout.HelpBox("You have to assign the camera prefab. /n You find in Resources/Prefabs", MessageType.Info);
                        checkAgain = true;
                        canSave = false;
                    }
                    else if (!isCanvas)
                    {
                        EditorGUILayout.HelpBox("You have to assign the canvas prefab. /n You find in Resources/Prefabs", MessageType.Info);
                        checkAgain = true;
                        canSave = false;
                    }

                    charObj = EditorGUILayout.ObjectField("Your Model ", charObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                    _cameraPrefab = EditorGUILayout.ObjectField("Camera Prefab", _cameraPrefab, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                    _itemCanvasPrefab = EditorGUILayout.ObjectField("Canvas Prefab ", _itemCanvasPrefab, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                    GUI.skin = null;
                    EditorGUI.BeginChangeCheck();
                    _backGround = EditorGUILayout.ObjectField("Background Image", _backGround, typeof(Sprite), true, GUILayout.ExpandWidth(true)) as Sprite;
                    if (EditorGUI.EndChangeCheck() && !UseBorders)
                    {
                        _originalSprite = _backGround;
                    }
                    else if ( EditorGUI.EndChangeCheck() && UseBorders)
                    {
                        _backGround = borderTexture.ToSprite();    
                    }

                    GUI.skin = bteSkin;
                    EditorGUI.BeginChangeCheck();
                    UseBorders = EditorGUILayout.Toggle("Use borders", UseBorders, "toggle");
                    if (EditorGUI.EndChangeCheck()) 
                    {                        
                        _backGround = UseBorders ? borderTexture.ToSprite() : _originalSprite;                       
                    }

                    EditorGUILayout.Separator();
                    if (GUILayout.Button("Set Empty Background Texture"))
                    {
                        UseBorders = false;
                        _backGround = emptyTexture.ToSprite();
                        _originalSprite = _backGround;
                    }

                    offSet = EditorGUILayout.IntField("Icon Offset ", offSet, GUILayout.ExpandWidth(true));
                    InvertLayers = EditorGUILayout.Toggle("Invert Image Layers", InvertLayers, "toggle");

                    if (offSet < 0)
                    {
                        EditorGUILayout.HelpBox("The Icon OffSet value can't be negative.", MessageType.Warning);
                        offSet = 0;
                    }
                    else if (offSet >= 32)
                    {
                        EditorGUILayout.HelpBox("The Icon OffSet value reaches the maximum value.", MessageType.Warning);
                        offSet = 32;
                    }
                    EditorGUILayout.Space();

                    /////////////////////////////////////PROBLEMA ////////////////////////////////////

                   /* if (GUI.changed && charObj != null)
                    {
                        humanoidpreview = UnityEditor.Editor.CreateEditor(charObj);
                    }*/
                }
                GUILayout.EndVertical();

                if (charObj != null)
                {
                    GUILayout.BeginVertical(textureBoxStyle);
                    {
                        if (!canSave)
                        {
                            DrawHumanoidPreview();
                        }
                        else
                        {
                            DrawCameraPreview();
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.Space(10);

                    EditorGUI.BeginChangeCheck();
                    sizeIndex = EditorGUILayout.Popup(sizeIndex, imageSize);

                    switch (sizeIndex)
                    {
                        case 0:
                            iconSize = 64;
                            break;
                        case 1:
                            iconSize = 128;
                            break;
                        case 2:
                            iconSize = 256;
                            break;
                        case 3:
                            iconSize = 512;
                            break;
                        case 4:
                            iconSize = 1024;
                            break;
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        RefreshTexture();
                    }


                    GUILayout.BeginHorizontal();
                    {

                        if (isCamera && isCanvas && !canSave)
                        {
                            if (GUILayout.Button("Set Up"))
                            {
                                ShowCameraRender();
                            }
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUI.SetNextControlName("IconText");
                    iconName = EditorGUILayout.TextField("Icon Name", iconName);

                    if (iconName is null || iconName.Trim().Length == 0)
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("You must assign a name to the icon file", MessageType.Info);
                    }
                    if (canSave)
                    {
                        if (GUILayout.Button("Create Icon"))
                        {
                            CreateIcon();
                        }
                    }
                }
            }
            GUILayout.EndVertical();
        }
        public virtual void UpdateCameraSize(Vector2 cameraSize)
        {
            cameraSize = cameraSize.normalized * 0.01f;
            _itemCamera.GetComponent<Camera>().orthographicSize += cameraSize.y;
        }
        public virtual void UpdateCameraPosition(Vector2 offset)
        {
            _itemCamera.transform.position += new Vector3(offset.x, offset.y) * 0.001f;
        }
        public virtual void ShowCameraRender()
        {
            GameObject _cameraContainer = InstantiateNewObject(_cameraPrefab);
            _itemCamera = _cameraContainer.GetComponent<Camera>();
            _itemCanvas = InstantiateNewObject(_itemCanvasPrefab);
            canSave = true;
        }
        public virtual void CreateIcon()
        {
            switch (sizeIndex)
            {
                case 0:
                    iconSize = 64;
                    break;
                case 1:
                    iconSize = 128;
                    break;
                case 2:
                    iconSize = 256;
                    break;
                case 3:
                    iconSize = 512;
                    break;
                case 4:
                    iconSize = 1024;
                    break;
            }

            GameObject imageBG = new("BG");
            RectTransform rectTransform = imageBG.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(iconSize, iconSize);
            Image sourceImage = imageBG.AddComponent<Image>();
            //        imageBG.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            if (_backGround) sourceImage.sprite = _backGround;
            imageBG.transform.SetParent(_itemCanvas.transform, false);
            GameObject imageModel = new("Icon");
            RectTransform rectTransform2 = imageModel.AddComponent<RectTransform>();
            rectTransform2.sizeDelta = new Vector2(iconSize, iconSize);
            Image sourceImage2 = imageModel.AddComponent<Image>();
            sourceImage2.sprite = GetIcon();
            imageModel.transform.SetParent(imageBG.transform, false);
            if (_backGround && !InvertLayers) SaveIconAsset(sourceImage.sprite.ConvertSpriteToTexture(), sourceImage2.sprite.ConvertSpriteToTexture(), iconSize);
            else if (_backGround && InvertLayers) SaveIconAsset(sourceImage2.sprite.ConvertSpriteToTexture(), sourceImage.sprite.ConvertSpriteToTexture(), iconSize);
            else if (!_backGround) SaveIconAsset(emptyTexture, sourceImage2.sprite.ConvertSpriteToTexture(), iconSize);

            CleanScene();
        }
        public virtual void SaveIconAsset(Texture2D bottom, Texture2D top, int iconSize)
        {

            Texture2D _bottom = ScaleTexture(bottom, iconSize, iconSize);
            Texture2D _top = ScaleTexture(top, iconSize, iconSize);
            Texture2D combined = _bottom.AlphaBlend(_top);
            SaveTexture(combined);
        }
        private void OnDestroy()
        {
            CleanScene();
        }
        private void CleanScene()
        {
            if (_renderTexture != null)
            {
                _renderTexture.DiscardContents();
                _renderTexture = null;
            }
            if (_itemCamera != null)
            {
                GameObject tempCamera = GameObject.Find(_itemCamera.gameObject.name);
                DestroyImmediate(tempCamera);
                _itemCamera = null;
            }
            if (_itemCanvas != null)
            {
                GameObject tempCanvas = GameObject.Find(_itemCanvas.gameObject.name);
                DestroyImmediate(tempCanvas);
                _itemCanvas = null;
            }

            iconName = string.Empty;
            canSave = false;
            Repaint();

            GUI.UnfocusWindow();

            

        }
        private void SaveTexture(Texture2D texture)
        {
            byte[] bytes = texture.EncodeToPNG();

            //        if (iconName is null || iconName.Trim().Length == 0) iconName = "R_" + UnityEngine.Random.Range(0, 100000);

            string path = EditorUtility.SaveFilePanel("Save Icon image file", "Assets/", iconName, "png");

            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetPhysicalPath(path);

            //System.IO.File.WriteAllBytes(path + "/" + iconName + ".png", bytes);
            System.IO.File.WriteAllBytes(path, bytes);

            Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + path);
            AssetDatabase.Refresh();

        }
        private GameObject InstantiateNewObject(GameObject selected)
        {
            if (selected == null)
            {
                return selected;
            }

            if (selected.scene.IsValid())
            {
                return selected;
            }

            return PrefabUtility.InstantiatePrefab(selected) as GameObject;
        }
        public virtual void DrawHumanoidPreview()
        {
            // GUILayout.FlexibleSpace();

            if (humanoidpreview != null)
            {
                humanoidpreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 200), "window");
            }
        }
        public virtual void DrawCameraPreview()
        {
            _camera = _itemCamera;

            if (_renderTexture != null)
            {

                Rect viewPort = GUILayoutUtility.GetRect(192, 192);

                EditorGUI.DrawTextureTransparent(viewPort, _renderTexture, ScaleMode.ScaleToFit);
            }
        }
    }
}
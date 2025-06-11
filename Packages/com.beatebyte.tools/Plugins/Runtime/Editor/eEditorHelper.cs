using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using BeatebyteToolsEditor.Utils;
using BeatebyteToolsEditor.Attributes;

namespace BeatebyteToolsEditor.Editor
{
    using Editor = UnityEditor.Editor;

    /// <summary>
    /// Editor Helper
    /// </summary>

    public class eEditorHelper : Editor
    {
        /// <summary>
        /// Get PropertyName
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyLambda">You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'</param>
        /// <returns></returns>
        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            return propertyLambda.Body is not MemberExpression me
                ? throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'")
                : me.Member.Name;
        }

        /// <summary>
        /// Check if type is a <see cref="UnityEngine.Events.UnityEvent"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUnityEventyType(Type type)
        {
            if (type.Equals(typeof(UnityEngine.Events.UnityEvent))) return true;
            if (type.BaseType.Equals(typeof(UnityEngine.Events.UnityEvent))) return true;
            if (type.Name.Contains("UnityEvent") || type.BaseType.Name.Contains("UnityEvent")) return true;
            return false;
        }
    }

    /// <summary>
    /// The e editor base.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(eMonoBehaviour), true)]
    public class eEditorBase : Editor
    {
        #region Variables   
        private static readonly string IconGUID = "643fff9a5f3ccf94aa0a9320c6ca2dba";
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";

        /// <summary>
        /// Ignore events.
        /// </summary>
        public string[] ignoreEvents;
        /// <summary>
        /// The not event properties.
        /// </summary>
        public string[] notEventProperties;
        /// <summary>
        /// Gets the ignoree mono.
        /// </summary>
        public virtual string[] ignore_eMono => new string[] { "openCloseWindow", "openCloseEvents", "selectedToolbar" };
        /// <summary>
        /// The header attribute.
        /// </summary>
        public eClassHeaderAttribute headerAttribute;
        /// <summary>
        /// The skin.
        /// </summary>
        public GUISkin skin;
        /// <summary>
        /// The M logo.
        /// </summary>
        public Texture2D m_Logo;
        /// <summary>
        /// The toolbars.
        /// </summary>
        public List<eToolBar> toolbars;

        #endregion

        public class eToolBar
        {
            /// <summary>
            /// The title.
            /// </summary>
            public string title;
            /// <summary>
            /// Use icon.
            /// </summary>
            public bool useIcon;
            /// <summary>
            /// The icon name.
            /// </summary>
            public string iconName;
            /// <summary>
            /// The variables.
            /// </summary>
            public List<string> variables;
            /// <summary>
            /// Order.
            /// </summary>
            public int order;
            /// <summary>
            /// Initializes a new instance of the <see cref="eToolBar"/> class.
            /// </summary>
            public eToolBar()
            {
                title = string.Empty;
                variables = new List<string>();
            }
        }

        /// <summary>
        /// On enable.
        /// </summary>
        protected virtual void OnEnable()
        {

            var targetObject = serializedObject.targetObject;
            var hasAttributeHeader = targetObject.GetType().IsDefined(typeof(eClassHeaderAttribute), true);
            if (hasAttributeHeader)
            {
                var attributes = Attribute.GetCustomAttributes(targetObject.GetType(), typeof(eClassHeaderAttribute), true);
                if (attributes.Length > 0)
                    headerAttribute = (eClassHeaderAttribute)attributes[0];
            }

            skin = AssetDatabase.LoadAssetAtPath<GUISkin>(AssetDatabase.GUIDToAssetPath(GUISkinGUID));
            m_Logo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(IconGUID));
            var prop = serializedObject.GetIterator();

            if (((eMonoBehaviour)target) != null)
            {
                List<string> events = new();

                toolbars = new List<eToolBar>();
                eToolBar toolbar = new()
                {
                    title = "Default"
                };

                toolbars.Add(toolbar);
                var index = 0;
                bool needReorder = false;
                int oldOrder = 0;

                while (prop.NextVisible(true))
                {
                    if (!targetObject.TryGetField(prop.name, out FieldInfo fieldInfo))
                    {
                        continue;
                    }


                    if (fieldInfo != null)
                    {

                        var toolBarAttributes = fieldInfo.GetCustomAttributes(typeof(eEditorToolbarAttribute), true);

                        if (toolBarAttributes.Length > 0)
                        {
                            var attribute = toolBarAttributes[0] as eEditorToolbarAttribute;
                            var _toolbar = toolbars.Find(tool => tool != null && tool.title == attribute.title);

                            if (_toolbar == null)
                            {
                                toolbar = new eToolBar
                                {
                                    title = attribute.title,
                                    useIcon = attribute.useIcon,
                                    iconName = attribute.icon
                                };
                                toolbars.Add(toolbar);
                                toolbar.order = attribute.order;
                                if (oldOrder < attribute.order) needReorder = true;
                                index = toolbars.Count - 1;

                            }
                            else
                            {
                                index = toolbars.IndexOf(_toolbar);
                                if (index < toolbars.Count)
                                {
                                    if (attribute.overrideChildOrder)
                                    {
                                        if (oldOrder < attribute.order) needReorder = true;
                                        toolbars[index].order = attribute.order;
                                    }
                                    if (attribute.overrideIcon)
                                    {
                                        toolbars[index].useIcon = true;
                                        toolbars[index].iconName = attribute.icon;
                                    }
                                }
                            }
                        }
                        if (index < toolbars.Count)
                        {
                            toolbars[index].variables.Add(prop.name);
                        }

                        if ((eEditorHelper.IsUnityEventyType(fieldInfo.FieldType) && !events.Contains(fieldInfo.Name)))
                        {
                            events.Add(fieldInfo.Name);
                        }
                    }
                }
                if (needReorder)
                    toolbars.Sort((a, b) => a.order.CompareTo(b.order));
                var nullToolBar = toolbars.FindAll(tool => tool != null && (tool.variables == null || tool.variables.Count == 0));
                for (int i = 0; i < nullToolBar.Count; i++)
                {
                    if (toolbars.Contains(nullToolBar[i]))
                        toolbars.Remove(nullToolBar[i]);
                }

                ignoreEvents = events.eToArray();
                if (headerAttribute != null)
                    m_Logo = headerAttribute.iconName == "icon_v2" ? m_Logo : Resources.Load(headerAttribute.iconName) as Texture2D;
                //else headerAttribute = new vClassHeaderAttribute(target.GetType().Name);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether open close window.
        /// </summary>
        protected virtual bool openCloseWindow
        {
            get
            {
                return serializedObject.FindProperty("openCloseWindow").boolValue;
            }
            set
            {
                var _openClose = serializedObject.FindProperty("openCloseWindow");
                if (_openClose != null && value != _openClose.boolValue)
                {
                    _openClose.boolValue = value;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether open close events.
        /// </summary>
        protected virtual bool openCloseEvents
        {
            get
            {
                var _openCloseEvents = serializedObject.FindProperty("openCloseEvents");
                return _openCloseEvents != null && _openCloseEvents.boolValue;
            }
            set
            {
                var _openCloseEvents = serializedObject.FindProperty("openCloseEvents");
                if (_openCloseEvents != null && value != _openCloseEvents.boolValue)
                {
                    _openCloseEvents.boolValue = value;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected tool bar.
        /// </summary>
        protected virtual int selectedToolBar
        {
            get
            {
                var _selectedToolBar = serializedObject.FindProperty("selectedToolbar");
                return _selectedToolBar != null ? _selectedToolBar.intValue : 0;
            }
            set
            {
                var _selectedToolBar = serializedObject.FindProperty("selectedToolbar");

                if (_selectedToolBar != null && value != _selectedToolBar.intValue)
                {
                    _selectedToolBar.intValue = value;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// Use default margins.
        /// </summary>
        /// <returns>A bool</returns>
        public override bool UseDefaultMargins()
        {
            return false;
        }

        /// <summary>
        /// On inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {

            GUI.skin = skin;
            GUIStyle guiStyle = new(skin.GetStyle("bteCustomWindow"));
            GUIStyle guiToolbarButton = new(EditorStyles.toolbarButton);

            guiToolbarButton = skin.GetStyle("bteButton");
            serializedObject.Update();
            if (toolbars != null && toolbars.Count > 1)
            {
                GUILayout.BeginVertical(headerAttribute != null ? headerAttribute.header : target.GetType().Name, guiStyle);

                GUILayout.Label(m_Logo, skin.label, GUILayout.MaxHeight(48));

                if (headerAttribute.openClose)
                {
                    openCloseWindow = GUILayout.Toggle(openCloseWindow, openCloseWindow ? "Close Properties" : "Open Properties", guiToolbarButton);
                }

                if (!headerAttribute.openClose || openCloseWindow)
                {
                    var titles = getToobarTitles();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));

                    var customToolbar = skin.GetStyle("bteCustomToolbar");

                    if (headerAttribute.useHelpBox)
                    {
                        EditorStyles.helpBox.richText = true;
                        EditorGUILayout.HelpBox(headerAttribute.helpBoxText, MessageType.Info);
                    }

                    GUILayout.Space(10);
                    selectedToolBar = GUILayout.SelectionGrid(selectedToolBar, titles, titles.Length > 2 ? 3 : titles.Length, customToolbar, GUILayout.MinWidth(250));
                    if (!(selectedToolBar < toolbars.Count)) selectedToolBar = 0;
                    //GUILayout.Space(10);
                    //GUILayout.Box(toolbars[selectedToolBar].title, skin.box, GUILayout.ExpandWidth(true));
                    var ignore = getIgnoreProperties(toolbars[selectedToolBar]);
                    var ignoreProperties = ignore.Append(ignore_eMono);
                    DrawPropertiesExcluding(serializedObject, ignoreProperties);
                    AdditionalGUI();
                }
                GUILayout.EndVertical();
            }
            else
            {
                if (headerAttribute == null)
                {
                    if (((eMonoBehaviour)target) != null)
                        DrawPropertiesExcluding(serializedObject, ignore_eMono);
                    else
                        base.OnInspectorGUI();
                    AdditionalGUI();
                }
                else
                {
                    GUILayout.BeginVertical(headerAttribute.header, guiStyle);
                    GUILayout.Label(m_Logo, skin.label, GUILayout.Width(48), GUILayout.Height(48));
                    if (headerAttribute.openClose)
                    {
                        openCloseWindow = GUILayout.Toggle(openCloseWindow, openCloseWindow ? "Close Properties" : "Open Properties", guiToolbarButton);
                    }

                    if (!headerAttribute.openClose || openCloseWindow)
                    {
                        if (headerAttribute.useHelpBox)
                        {
                            EditorStyles.helpBox.richText = true;
                            EditorGUILayout.HelpBox(headerAttribute.helpBoxText, MessageType.Info);
                        }

                        if (ignoreEvents != null && ignoreEvents.Length > 0)
                        {
                            var ignoreProperties = ignoreEvents.Append(ignore_eMono);
                            DrawPropertiesExcluding(serializedObject, ignoreProperties);
                            openCloseEvents = GUILayout.Toggle(openCloseEvents, (openCloseEvents ? "Close " : "Open ") + "Events ", skin.button);

                            if (openCloseEvents)
                            {
                                foreach (string propName in ignoreEvents)
                                {
                                    var prop = serializedObject.FindProperty(propName);
                                    if (prop != null)
                                        EditorGUILayout.PropertyField(prop);
                                }
                            }
                        }
                        else
                        {
                            var ignoreProperties = ignoreEvents.Append(ignore_eMono);
                            DrawPropertiesExcluding(serializedObject, ignoreProperties);
                        }
                    }
                    AdditionalGUI();
                    EditorGUILayout.EndVertical();
                }
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
        }

        /// <summary>
        /// Get toobar titles.
        /// </summary>
        /// <returns>An array of GUIContents</returns>
        public virtual GUIContent[] getToobarTitles()
        {
            List<GUIContent> props = new();
            for (int i = 0; i < toolbars.Count; i++)
            {
                if (toolbars[i] != null)
                {
                    Texture icon = null;
                    var _icon = Resources.Load(toolbars[i].iconName);
                    if (_icon) icon = _icon as Texture;
                    GUIContent content = new(toolbars[i].useIcon ? "" : toolbars[i].title, icon, "");

                    props.Add(content);
                }

            }
            return props.eToArray();
        }

        /// <summary>
        /// Get ignore properties.
        /// </summary>
        /// <param name="toolbar">The toolbar.</param>
        /// <returns>An array of string</returns>
        public virtual string[] getIgnoreProperties(eToolBar toolbar)
        {
            List<string> props = new();
            for (int i = 0; i < toolbars.Count; i++)
            {
                if (toolbars[i] != null && toolbar != null && toolbar.variables != null)
                {
                    for (int a = 0; a < toolbars[i].variables.Count; a++)
                    {
                        if (!props.Contains(toolbars[i].variables[a]) && !toolbar.variables.Contains(toolbars[i].variables[a]))
                        {
                            props.Add(toolbars[i].variables[a]);
                        }
                    }
                }
            }
           
            props.Add("m_Script");
            return props.eToArray();
        }

        /// <summary>
        /// Additionals the GUI.
        /// </summary>
        protected virtual void AdditionalGUI()
        {
        }
    }
}
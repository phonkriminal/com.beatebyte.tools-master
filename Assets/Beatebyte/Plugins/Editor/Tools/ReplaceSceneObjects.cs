using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using BeatebyteToolsEditor.Shared;

namespace BeatebyteToolsEditor
{
    [CanEditMultipleObjects]
    public class ReplaceSceneObjects : EditorWindow
    {

        public SerializedObject serializedObject;

        public SerializedProperty listProperty;
        public SerializedProperty listPropertyBehaviour;

        public ReorderableList reorderableList;
        public ReorderableList reorderableListBehaviour;

        public string[] groups = { "Not assigned" };

        public GameObject replaceSceneObject = null;
        public GameObject[] gameObjectsReplaceWith = new GameObject[0];

        [SerializeField]
        private List<ObjectList> replaceObjectsList = new();

        [SerializeField]
        private List<ObjectsListsBehaviour> replaceObjectsListBehaviour = new();

        [SerializeField, Tooltip("This option delete the replaced object from scene, use carefully and save your work before.")]
        private bool destroyOriginalObjects = false;
        
        [SerializeField]
        private GameObject gameObjectContainer = null;

        private GameObject[] selectedGameObjects = new GameObject[0];

        private Texture2D logo;
        private Texture2D rightArrowIcon;
        private Texture2D downArrowIcon;
        private Texture2D currentArrowIcon;
        private Texture2D currentArrowIconReplace;

        private GUISkin bteSkin;
        private GUIStyle titleStyle;
        private GUIStyle textureBoxStyle;
        private GUIStyle labelStyle;
        private GUIStyle listHeaderStyle;
        private GUIStyle listElementStyle;
        private GUIStyle listFoldOutStyle;

        private static readonly string IconGUID = "643fff9a5f3ccf94aa0a9320c6ca2dba";
        private static readonly string GUISkinGUID = "98de12020fe6aad43a4afcf7464f805a";
        private static readonly string RightArrowUIGUID = "0174207df03810e4ab1e3390c700612a";
        private static readonly string DownArrowUIGUID = "eb5220419d6be29478552f1fd12b8520";
        private static bool doPopulate = true;

        internal bool isFoldOut = false;             //  objects to replace
        internal bool isFoldOutReplace = true;     // objects to replace with
        internal Vector2 scrollPosition = Vector2.zero;
        internal List<string> popupDisplayOptions = new List<string>();
        internal List<int> groupIndexUsed = new List<int>();    // index group for replace objects
        internal List<int> randomIndexUsed = new List<int>();   // index random for replace objects


        int containersCount = 0;

        [MenuItem("Window/Beatebyte Creations/Tools/Replace Scene's Objects", false, 3)]
        [MenuItem("GameObject/Replace Scene's Objects", isValidateFunction: false, 14)]

        public static void ShowWindow()
        {
            var window = GetWindow<ReplaceSceneObjects>();
            window.maxSize = new Vector2(600f, 1200f);
            window.minSize = new Vector2(600f, 600f);

            Vector2 screenRect = Handles.GetMainGameViewSize();

            Rect newPosition = new((screenRect.x - 600f) / 2, (screenRect.y - 600f) / 2, 600f, 600f);
            window.position = newPosition;
        }

        [MenuItem("Window/Beatebyte Creations/Tools/Replace Scene's Objects", true)]
        [MenuItem("GameObject/Replace Scene's Objects", isValidateFunction: true)]

        private static bool ValidateSelection()
        {
            return Selection.gameObjects.Length > 0;
        }

        internal void PopulateListArray()
        {
            if (!doPopulate)
            {
                return;
            }

            gameObjectsReplaceWith = new GameObject[0];
            replaceObjectsList = new();

            if (doPopulate)
            {
                replaceObjectsList.Clear();
                selectedGameObjects.Initialize();
                selectedGameObjects = Selection.gameObjects;
                foreach (GameObject gameObjectInArray in selectedGameObjects)
                {
                    var item = new ObjectList(gameObjectInArray.name, gameObjectInArray.gameObject);
                    replaceObjectsList.Add(item);
                }
            }

            reorderableList.list = replaceObjectsList;

            doPopulate = false;
        }
        void OnContextualPropertyMenu(GenericMenu menu, SerializedProperty property)
        {
            if (property != null && property.propertyPath == "replaceObjectsList")
            {
                foreach (var item in popupDisplayOptions)
                {
                    menu.AddItem(new GUIContent("Set Group To Selected/" + item), false, () =>
                    {
                        for (int i = 0; i < reorderableList.count; i++)
                        {
                            string m_name = reorderableList.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;

                            if (reorderableList.IsSelected(i))
                            {
                                replaceObjectsList[i].GroupIndex = popupDisplayOptions.IndexOf(item);
                                EditorUtility.SetDirty(this);
                            }
                        }
                    });
                }

                Repaint();
            }
        }

        private void OnEnable()
        {
            EditorApplication.contextualPropertyMenu += OnContextualPropertyMenu;

            serializedObject = new SerializedObject(this);

            listProperty = serializedObject.FindProperty("replaceObjectsList");
            listPropertyBehaviour = serializedObject.FindProperty("replaceObjectsListBehaviour");

            reorderableList = new ReorderableList(serializedObject, listProperty, false, true, true, true);
            reorderableListBehaviour = new ReorderableList(serializedObject, listPropertyBehaviour, false, true, true, true);

            popupDisplayOptions.Add("not assigned");

            reorderableList.drawHeaderCallback = OnDrawHeaderList;
            reorderableList.drawElementCallback = OnDrawElementsList;
            reorderableList.onSelectCallback += OnSelectElement;
            reorderableList.onRemoveCallback = OnRemoveElementList;
            reorderableList.onAddCallback = OnAddElementList;

            reorderableListBehaviour.drawHeaderCallback = OnDrawHeader;
            reorderableListBehaviour.drawElementCallback = OnDrawElements;
            reorderableListBehaviour.onSelectCallback += OnSelectElement;
            reorderableListBehaviour.onAddCallback = OnAddElement;
            reorderableListBehaviour.onRemoveCallback = OnRemoveElement;

            doPopulate = true;
            PopulateListArray();

            if (CheckTags()) { Debug.Log("Tag Container found."); }

        }
        private void OnDestroy()
        {
            reorderableList.drawHeaderCallback -= OnDrawHeaderList;
            reorderableList.drawElementCallback -= OnDrawElementsList;
            reorderableList.onSelectCallback -= OnSelectElement;
            reorderableList.onRemoveCallback -= OnRemoveElementList;
            reorderableList.onAddCallback -= OnAddElementList;

            reorderableListBehaviour.drawHeaderCallback -= OnDrawHeader;
            reorderableListBehaviour.drawElementCallback -= OnDrawElements;
            reorderableListBehaviour.onSelectCallback -= OnSelectElement;
            reorderableListBehaviour.onAddCallback -= OnAddElement;
            reorderableListBehaviour.onRemoveCallback -= OnRemoveElement;

            EditorApplication.contextualPropertyMenu -= OnContextualPropertyMenu;

        }

        internal void OnWeightChanged(int index, int value)
        {
            //Debug.Log($"Weight Index {index} value : " + value);
        }

        internal void OnToggle(int index, bool toggle)
        {
            //Debug.Log($"TOGGLE Index {index} value : " + toggle);


            if (toggle)
            {
                if (popupDisplayOptions.Contains<string>(replaceObjectsListBehaviour[index].Name))
                {
                    Debug.Log($"Group {replaceObjectsListBehaviour[index].Name} is already present.");
                }
                else
                {
                    popupDisplayOptions.Add(replaceObjectsListBehaviour[index].Name);
                }
                if (!groupIndexUsed.Contains(index)) { groupIndexUsed.Add(index); }
            }
            else
            {
                if (popupDisplayOptions.Contains<string>(replaceObjectsListBehaviour[index].Name))
                {
                    popupDisplayOptions.Remove(replaceObjectsListBehaviour[index].Name);
                }
                if (groupIndexUsed.Contains(index)) { groupIndexUsed.Remove(index); }
            }
            EditorUtility.SetDirty(this);
        }

        internal void OnToggleRandom(int index, bool toggle)
        {
            if (toggle)
            {
                if (!randomIndexUsed.Contains(index)) { randomIndexUsed.Add(index); }
            }
            else
            {
                if (randomIndexUsed.Contains(index)) { randomIndexUsed.Remove(index); }
            }
        }

        internal void OnDrawElementsList(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty gameObjectProperty = element.FindPropertyRelative("gameObject");
            SerializedProperty groupsProperty = element.FindPropertyRelative("groups");
            SerializedProperty groupIndexProperty = element.FindPropertyRelative("groupIndex");

            rect.y += 2;

            gameObjectProperty.objectReferenceValue = EditorGUI.ObjectField(new Rect(rect.x, rect.y, 240, EditorGUIUtility.singleLineHeight), GUIContent.none, (GameObject)gameObjectProperty.objectReferenceValue, typeof(GameObject), true);
            groupIndexProperty.intValue = EditorGUI.Popup(new Rect(rect.x + 250, rect.y, 240, EditorGUIUtility.singleLineHeight), groupIndexProperty.intValue, popupDisplayOptions.ToArray());
        }

        internal bool CheckTags()
        {
            return TagsAndLayers.AddTag("Container");
        }

        internal void OnDrawHeaderList(Rect rect)
        {
            EditorGUI.LabelField(rect, "    OBJECT TO REPLACE", listHeaderStyle);
        }

        internal void OnAddElementList(ReorderableList list)
        {
            ObjectList item = new();
            item.Groups = popupDisplayOptions.ToArray();
            item.GroupIndex = 0;
            replaceObjectsList.Add(item);
            EditorUtility.SetDirty(this);
        }

        internal void OnRemoveElementList(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }

        internal void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 240, EditorGUIUtility.singleLineHeight), "PREFABS", listHeaderStyle);
            EditorGUI.LabelField(new Rect(rect.x + 240, rect.y, 65, EditorGUIUtility.singleLineHeight), "| RANDOM ", listHeaderStyle);
            EditorGUI.LabelField(new Rect(rect.x + 305, rect.y, 55, EditorGUIUtility.singleLineHeight), "| GROUP ", listHeaderStyle);
            EditorGUI.LabelField(new Rect(rect.x + 360, rect.y, rect.width - 360, EditorGUIUtility.singleLineHeight), "| WEIGHT", listHeaderStyle);
        }

        internal void OnDrawElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reorderableListBehaviour.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty gameObjectProperty = element.FindPropertyRelative("gameObject");
            SerializedProperty randomReplaceProperty = element.FindPropertyRelative("randomReplace");
            SerializedProperty useAsGroupProperty = element.FindPropertyRelative("useAsGroup");
            SerializedProperty replaceWeightProperty = element.FindPropertyRelative("replaceWeight");

            rect.y += 2;

            gameObjectProperty.objectReferenceValue = EditorGUI.ObjectField(new Rect(rect.x, rect.y, 220, EditorGUIUtility.singleLineHeight), GUIContent.none, (GameObject)gameObjectProperty.objectReferenceValue, typeof(GameObject), false);
            EditorGUI.BeginDisabledGroup(((GameObject)gameObjectProperty.objectReferenceValue) == null || replaceObjectsListBehaviour.Count < 2);
            {             

                randomReplaceProperty.boolValue = EditorGUI.Toggle(new Rect(rect.x + 240, rect.y, 60, EditorGUIUtility.singleLineHeight), randomReplaceProperty.boolValue, "toggle");

                useAsGroupProperty.boolValue = EditorGUI.Toggle(new Rect(rect.x + 300, rect.y, 60, EditorGUIUtility.singleLineHeight), !randomReplaceProperty.boolValue, "toggle");

                EditorGUI.BeginDisabledGroup(randomReplaceProperty.boolValue == false || replaceObjectsListBehaviour.Count < 2 || useAsGroupProperty.boolValue == true);
                {
                    EditorGUI.PropertyField(new Rect(rect.x + 360, rect.y, rect.width - 360, EditorGUIUtility.singleLineHeight), replaceWeightProperty, GUIContent.none);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();

            replaceObjectsListBehaviour[index].Name = (GameObject)gameObjectProperty.objectReferenceValue != null ? ((GameObject)gameObjectProperty.objectReferenceValue).name : null;
            replaceObjectsListBehaviour[index].GameObject = (GameObject)gameObjectProperty.objectReferenceValue != null ? ((GameObject)gameObjectProperty.objectReferenceValue) : null;
            replaceObjectsListBehaviour[index].RandomReplace = randomReplaceProperty.boolValue;
            replaceObjectsListBehaviour[index].UseAsGroup = useAsGroupProperty.boolValue;
            replaceObjectsListBehaviour[index].ReplaceWeight = replaceWeightProperty.intValue;
            replaceObjectsListBehaviour[index].Index = index;
        }

        internal void OnSelectElement(ReorderableList list)
        {
            list.multiSelect = true;

            for (int i = 0; i < list.serializedProperty.arraySize; i++)
            {
                string m_name = list.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;

                if (list.IsSelected(i))
                {
                    //{ Debug.Log(m_name); }
                }
            }

            var prefab = list.serializedProperty.GetArrayElementAtIndex((int)list.index).FindPropertyRelative("gameObject").objectReferenceValue as GameObject;

            if (prefab)
            {
                EditorGUIUtility.PingObject(prefab.gameObject);
            }

        }

        internal void OnAddElement(ReorderableList list)
        {
            ObjectsListsBehaviour item = new();
            item.onToggle += OnToggle;
            item.onToggleRandom += OnToggleRandom;
            item.onWeightChanged += OnWeightChanged;
            replaceObjectsListBehaviour.Add(item);
            EditorUtility.SetDirty(this);
        }

        internal void OnRemoveElement(ReorderableList list)
        {
            popupDisplayOptions.Remove(replaceObjectsListBehaviour[list.index].Name);
            replaceObjectsListBehaviour[list.index].onToggle -= OnToggle;
            replaceObjectsListBehaviour[list.index].onToggleRandom -= OnToggleRandom;
            replaceObjectsListBehaviour[list.index].onWeightChanged -= OnWeightChanged;
            replaceObjectsListBehaviour.RemoveAt(list.index);

            EditorUtility.SetDirty(this);
        }

        private void OnGUI()
        {
            #region GUI VARIABLES

            if (!bteSkin) { bteSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(AssetDatabase.GUIDToAssetPath(GUISkinGUID)); }

            if (!logo) { logo = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(IconGUID)); }

            if (!rightArrowIcon) { rightArrowIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(RightArrowUIGUID)); }

            if (!downArrowIcon) { downArrowIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(DownArrowUIGUID)); }

            if (!currentArrowIcon) { currentArrowIcon = rightArrowIcon; }

            if (!currentArrowIconReplace) { currentArrowIconReplace = downArrowIcon; }

            GUI.skin = bteSkin;

            titleStyle = new GUIStyle(bteSkin.GetStyle("bteTitle"));
            textureBoxStyle = new GUIStyle(bteSkin.GetStyle("bteTextureBox"));
            labelStyle = new GUIStyle(bteSkin.GetStyle("bteLabelTex"));
            listFoldOutStyle = new GUIStyle(bteSkin.GetStyle("bteFoldOut"));
            listHeaderStyle = new GUIStyle(bteSkin.GetStyle("bteListHeader"));
            listElementStyle = new GUIStyle(bteSkin.GetStyle("bteListElement"));

            this.titleContent = new GUIContent("Object Replacement", null, "Replace Gameobjects selected in the current scene");

            #endregion


            GUILayout.BeginVertical("       OBJECTS REPLACEMENT", titleStyle);
            {
                GUILayout.Label(logo, GUILayout.MaxHeight(48));
            }
            GUILayout.EndVertical();

            serializedObject.Update();

            GUILayout.Space(5);

            GUILayout.BeginVertical("", listFoldOutStyle);
            {
                GUIContent foldInHeaderReplace = new() { text = "  REPLACEMENT OBJECTS", image = currentArrowIconReplace };

                isFoldOutReplace = EditorGUILayout.BeginFoldoutHeaderGroup(isFoldOutReplace, foldInHeaderReplace, listFoldOutStyle);
                {
                    currentArrowIconReplace = isFoldOutReplace ? downArrowIcon : rightArrowIcon;

                    if (isFoldOutReplace)
                    {
                        GUILayout.BeginVertical("", listElementStyle);
                        {
                            EditorGUI.BeginChangeCheck();
                            {
                                reorderableListBehaviour.DoLayoutList();
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                // Debug.Log("GUI CHANGED");
                            }
                        }
                        GUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();



                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal(textureBoxStyle);
                    {
                        EditorGUILayout.LabelField(new GUIContent(text: "DELETE REPLACED OBJECTS : "), listHeaderStyle, GUILayout.ExpandWidth(false), GUILayout.Width(200), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        destroyOriginalObjects = EditorGUILayout.Toggle(new GUIContent(text: "", tooltip: "If set to true you delete the objects from scene.\bIf the value is false the objects replaced will be parented to an on object in the scene."), destroyOriginalObjects, "toggle");

                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(textureBoxStyle);
                    {
                        EditorGUILayout.LabelField("PARENT GAMEOBJECT", listHeaderStyle, GUILayout.ExpandWidth(false), GUILayout.Width(160), GUILayout.ExpandWidth(false), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        gameObjectContainer = EditorGUILayout.ObjectField(gameObjectContainer, typeof(GameObject), true, GUILayout.Width(200), GUILayout.ExpandWidth(false), GUILayout.Height(EditorGUIUtility.singleLineHeight)) as GameObject;
                        if (GUILayout.Button("CREATE PARENT OBJECT", "bteButton", GUILayout.ExpandWidth(true)))
                        {
                            containersCount = GameObject.FindGameObjectsWithTag("Container").Length;
                            string suffix = containersCount.ToString().PadLeft(2, '0');

                            if (gameObjectContainer == null)
                            {

                                if (containersCount > 0)
                                {
                                    if (!EditorUtility.DisplayDialog($"Warning...", "There are one or more container objects in the scene.\nDo you want create a new?\nIf not you can select what you prefer from the scene view. ", "Yes, create anew one.", "No, I'll select one from the scene."))
                                    {
                                        Debug.Log("Select Object from scene view.");                                 
                                    }
                                }
                                else
                                {
                                    gameObjectContainer = new GameObject($"===CONTAINER{suffix}===");
                                    gameObjectContainer.tag = "Container";
                                }
                            }
                            else
                            {
                                if (EditorUtility.DisplayDialog("Warning...", "The Parent object is just selected. are you sure you want create another one?", "Yes", "No, Use this."))
                                {
                                    gameObjectContainer = new GameObject($"===CONTAINER{suffix}===");
                                    gameObjectContainer.tag = "Container";
                                }
                                else
                                {
                                    Debug.Log("Parent Object set to Selected.");
                                }
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical("", textureBoxStyle);
            {
                if (GUILayout.Button("Populate Array", "bteButton"))
                {
                    doPopulate = true;
                    PopulateListArray();
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("", listFoldOutStyle);
            {
                GUIContent foldInHeader = new() { text = "  SELECTED OBJECTS", image = currentArrowIcon };

                isFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(isFoldOut, foldInHeader, listFoldOutStyle);
                {
                    currentArrowIcon = isFoldOut ? downArrowIcon : rightArrowIcon;

                    GUIStyle customLineStyle = new() { alignment = TextAnchor.UpperLeft, stretchWidth = true, imagePosition = ImagePosition.TextOnly };

                    if (isFoldOut)
                    {
                        if (replaceObjectsList.Count != 0)
                        {
                            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                            {
                                GUILayout.BeginVertical("", listElementStyle);
                                {
                                    serializedObject.Update();
                                    reorderableList.DoLayoutList();
                                    serializedObject.ApplyModifiedProperties();
                                }
                                GUILayout.EndVertical();
                            }
                            EditorGUILayout.EndScrollView();
                        }
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                EditorGUILayout.Space(5);

                EditorGUILayout.BeginVertical(textureBoxStyle);
                {
                    if (GUILayout.Button("REPLACE", "bteButton"))
                    {
                        if (ValidateReplaceObject(replaceObjectsList))
                        {
                            Replace();
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

        }

        internal bool ValidateReplaceObject(List<ObjectList> objectsToValidate)
        {
            if (objectsToValidate == null || objectsToValidate.Count == 0)
            {
                EditorUtility.DisplayDialog("Application Hint...", "You must assign a Prefab to Replace Object Field.", "Understand");
                return false;
            }

            return true;
        }

        internal int CheckReplacementObject()
        {
            int result = 0;
            //Debug.Log(replaceObjectsListBehaviour.Count);

            foreach (var item in replaceObjectsListBehaviour)
            {
             //   Debug.Log(item.GameObject.name + " " + item.UseAsGroup);
                if (item.UseAsGroup) { result++; }
            }
           // Debug.Log(replaceObjectsListBehaviour.Count + " " + result + " " + (replaceObjectsListBehaviour.Count - result));
            if (result == 0) 
            { 
            //    Debug.Log("No Group Used."); 
                return 0; 
            }
            
            if (result > 0 && (replaceObjectsListBehaviour.Count - result == 0))
            {
                //Debug.Log("All Groups Used."); return 1;
                return 1;
            }

            if (result > 0 &&  (replaceObjectsListBehaviour.Count - result != 0))
            {
                //Debug.Log("Some Groups Used."); return 2;
                return 2;
            }

            return result ;
        }

        internal bool CheckList(List<ObjectList> list)
        {
            int groupResult = CheckReplacementObject();

            //Debug.Log(list.Count + " " + groupResult);
            foreach (ObjectList obj in list)
            {
              //  Debug.Log(obj.GroupIndex == 0 && groupResult == 1);
                if (groupResult == 1 && obj.GroupIndex == 0)
                {
                    Debug.LogError("At least one replaced gameobject has not group assigned while you selected use all groups.");
                    return false;
                }
            }
            return true;
        }

        private void Replace()
        {

            if (replaceObjectsListBehaviour.Count == 0) { return; }
            if (!CheckList(replaceObjectsList)) 
            { 
                return; 
            }

            int counter = 0;

            for (int i = 0; i < replaceObjectsList.Count; i++)
            {
                Transform transforms = replaceObjectsList[i].GameObject.transform;
                GameObject fo = null;
                GameObject rootObject = null;

                if (replaceObjectsList[i].GroupIndex != 0 && replaceObjectsListBehaviour[replaceObjectsList[i].GroupIndex - 1].UseAsGroup)
                {                  
                    fo = Instantiate(replaceObjectsListBehaviour[replaceObjectsList[i].GroupIndex - 1].GameObject, transforms.position, transforms.rotation) as GameObject;
                    fo.transform.name = transforms.name + "_replaced_with_group_" + replaceObjectsListBehaviour[replaceObjectsList[i].GroupIndex - 1].GameObject.name;                    
                    counter++;
                }
                else if (replaceObjectsList[i].GroupIndex == 0)
                {
                    GameObject randomResult = DetermineAndFireRandomObjects();
                    fo = Instantiate(randomResult, transforms.position, transforms.rotation) as GameObject;                
                    counter++;
                }

                if (replaceObjectsList[i].GameObject.name == replaceObjectsList[i].GameObject.transform.root.name)
                {
                    Debug.Log(replaceObjectsList[i].GameObject.name + " is the root Object");
                }
                else
                {
                    rootObject = replaceObjectsList[i].GameObject.transform.root.gameObject;
                    Debug.Log(rootObject.gameObject.transform.name+ " is the root Object");
                    fo.transform.parent = rootObject ? rootObject.transform : null;
                }
            }
            if (counter > 0) UpdateScene();
        }
        private void UpdateScene()
        {
            if (!destroyOriginalObjects && !gameObjectContainer)
            {
                if (EditorUtility.DisplayDialog("Warning...", "You must select a Game Object from the scene as Container for the replaced objects.", "I understand")) {  return; }
            }           

            Selection.activeGameObject = null;
            
            int counter = 0;

            for (int i = 0; i < replaceObjectsList.Count; i++)
            {
                GameObject temp = GameObject.Find(replaceObjectsList[i].GameObject.transform.name);
                if (temp != null)
                {
                    counter++;
                    if (destroyOriginalObjects)
                    {
                        DestroyImmediate(temp);
                    }
                    else
                    {
                        temp.transform.parent = gameObjectContainer.transform;
                    }
                }
            }

            if (gameObjectContainer) { gameObjectContainer.SetActive(false); }

            selectedGameObjects.Initialize();
            replaceObjectsList.Clear();

            Debug.Log(counter + " Characters replaced!");

        }

        internal GameObject DetermineAndFireRandomObjects()
        {
            float totalChance = 0f;

            foreach (var item in replaceObjectsListBehaviour)
            {
                if (!item.UseAsGroup)
                {
                    totalChance += item.ReplaceWeight;
                }
            }

            float rand = UnityEngine.Random.Range(0, totalChance);
            float cumulativeChance = 0f;

            foreach (var item in replaceObjectsListBehaviour)
            {
                if (!item.UseAsGroup)
                {
                    cumulativeChance += item.ReplaceWeight;


                    if (rand <= cumulativeChance)
                    {
                        return item.GameObject;
                    }
                }                
            }
            return null;
        }
    }
}


[Serializable]
public class ObjectsListsBehaviour
{
    public delegate void OnToggleDelegate(int index, bool isActive);
    public OnToggleDelegate onToggle;

    public delegate void OnToggleRandomDelegate(int index, bool isActive);
    public OnToggleRandomDelegate onToggleRandom;

    public delegate void OnWeightChangedDelegate(int index, int value);
    public OnWeightChangedDelegate onWeightChanged;

    [SerializeField]
    private string name = string.Empty;
    public string Name { get => name; set => name = value; }

    [SerializeField]
    private GameObject gameObject = null;
    public GameObject GameObject { get => gameObject; set => gameObject = value; }

    [SerializeField]
    private bool randomReplace = true;
    public bool RandomReplace
    {
        get => randomReplace; set
        {
            if (randomReplace != value) { randomReplace = value; onToggleRandom?.Invoke(index, randomReplace); UseAsGroup = !value; }
        }
    }

    [SerializeField]
    private int index = 0;
    public int Index { get => index; set => index = value; }

    [SerializeField]
    private bool useAsGroup = false;
    public bool UseAsGroup
    {
        get => useAsGroup; set
        {
            if (useAsGroup != value) { useAsGroup = value; onToggle?.Invoke(index, useAsGroup); RandomReplace = !value; }
        }
    }

    [SerializeField, Range(0, 100)]
    private int replaceWeight = 0;
    public int ReplaceWeight
    {
        get => replaceWeight; set
        {
            if (replaceWeight != value)
            {
                replaceWeight = Mathf.Clamp(value, 0, 100);
                onWeightChanged?.Invoke(index, replaceWeight);
            }
        }
    }

    public ObjectsListsBehaviour(string name = null, GameObject gameObject = null, bool random = true, bool useAsGroup = false, int replaceWeight = 100)
    {
        this.name = name;
        this.gameObject = gameObject;
        this.randomReplace = random;
        this.useAsGroup = useAsGroup;
        this.replaceWeight = replaceWeight;
    }

}

[Serializable]
public class ObjectList
{
    [SerializeField]
    private string name = null;
    public string Name { get => name; set => name = value; }    

    [SerializeField]
    private GameObject gameObject = null;
    public GameObject GameObject { get => gameObject; set => gameObject = value; }
    [SerializeField]
    private string[] groups = new string[0];
    public string[] Groups { get => groups; set => groups = value; }

    [SerializeField]
    private int groupIndex = 0;
    public int GroupIndex { get => groupIndex; set => groupIndex = value; }

    public ObjectList(string name = null, GameObject gameObject = null, string[] groups = null, int groupIndex = 0)
    {
        if (groups == null) groups = new string[0];
        this.name = name;
        this.gameObject = gameObject;        
        this.groups = groups;
        this.groupIndex = groupIndex;

        if (gameObject != null) {name = gameObject.name;}
    }
}
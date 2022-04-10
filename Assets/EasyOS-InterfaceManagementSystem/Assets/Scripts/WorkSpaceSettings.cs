using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Alert;
using Assets.Scripts.Base;
using Assets.Scripts.BottomPanel;
using Assets.Scripts.Configs;
using Assets.Scripts.Folder;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{

  public enum SizeWindow
  {
    FullScreen, 
    HalfScreen = 1,
    Custom
  }
  
  public enum ReturnedButtonAlert
  {
    Negative, 
    Positive,
    None
  }

  [Serializable]
  public class FileShortcut
  {
    public string title;
    public string name;
    public Sprite icon;
    public GameObject contentPrefab;
    public bool scrollingContent = true;
    public string uniqHash;
    [NonSerialized] public List<FileShortcut> contentList = new List<FileShortcut>();
    public GetDataFile parentShortcut;
  }

  [Serializable]
  public class ButtonAlert
  {
    public GameObject buttonPrefab;
    public ReturnedButtonAlert returnedButtonAlert;
  }

  [Serializable]
  public class AlertBuilder
  {
    public string slug;
    public Sprite icon;
    public List<ButtonAlert> buttons = new List<ButtonAlert>();
    public TextAnchor alignmentButtons;
    public Vector2 openedSizeAlert;
  }
  
  public class WorkSpaceSettings : MonoBehaviour
  {
    public static WorkSpaceSettings Instance;

    public Transform mainCanvas;
    public ConfigEditor configEditor;
    
    [Header("Windows")] 
    public Transform rootWindows;

    public GameObject windowPrefab;
    public int maxWindowsOnScreen = 10;
    public bool borderRestriction = false;
    public bool resizableWindow = true;

    public SizeWindow minSizeWindow;
    public Vector2 minSizeWindowValue;
    
    public SizeWindow openedSizeWindow;
    public Vector2 openedSizeWindowValue;

    [Header("Desktop"), Space(15)] 
    public FlexibleGridLayout desktopGrid;
    public GameObject gridPrefab;
    public bool gridVisualizeDesktop;

    [Header("Cursors"), Space(15)] 
    public Texture2D cursorDefault;
    public Texture2D cursorHorizontal;
    public Texture2D cursorVertical;
    public Texture2D cursorAngleLeft;
    public Texture2D cursorAngleRight;
    public Texture2D cursorDraggable;

    [Header("BottomTaskBar"), Space(15)] 
    public Transform bottomPanelRoot;
    public BottomPanelController bottomPanelController;

    [Header("FoldersAndFiles"), Space(15)]
    public GameObject fileShortcutPrefab;
    public List<FileShortcut> fileShortcutsList;
    public bool isMovingDrag;
    public GameObject cloneDraggableFolderPrefab;
    public bool openInOneInstance;

    [Header("AlertConfig"), Space(15)] 
    public Transform alertsRoot;
    public GameObject alertPrefab;
    public List<AlertBuilder> alertsList;
    
    private void Awake()
    {
      SetDefaultCursor();

      if (Instance == null)
      {
        Instance = this;
      }
      else if (Instance == this)
      {
        Destroy(gameObject);
      }
    }

    // private void Start()
    // {
    //   rectRootWindows = rootWindows.GetComponent<RectTransform>().rect;
    // }

    private void FixedUpdate()
    {
      if (openedSizeWindow == SizeWindow.FullScreen)
      {
        openedSizeWindowValue = new Vector2(Screen.width, Screen.height);
      }else if (openedSizeWindow == SizeWindow.HalfScreen)
      {
        openedSizeWindowValue = new Vector2(Screen.width/2, Screen.height/2);
      }
      
      if (minSizeWindow == SizeWindow.FullScreen)
      {
        minSizeWindowValue = new Vector2(Screen.width, Screen.height);
      }else if (minSizeWindow == SizeWindow.HalfScreen)
      {
        minSizeWindowValue = new Vector2(Screen.width/2, Screen.height/2);
      }
    }

    public void SetDefaultCursor()
    {
      Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.ForceSoftware);
    }
    
    public void SetCursor(Texture2D cursorTexture)
    {
      if (cursorTexture != null) Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), CursorMode.ForceSoftware);
    }

    public void Alert(string slug, string title, string content, Action returnedPositive = null, Action returnedNegative = null)
    {
      
      if (alertsRoot && alertPrefab)
      {
        AlertBuilder alertData = alertsList.Find(x => x.slug == slug);
        if (alertData != null)
        {
          GameObject alert = Instantiate(alertPrefab, alertsRoot.position, Quaternion.identity, alertsRoot);
          
          alert.GetComponent<SetDataAlert>().Init(alertData, title, content, returnedPositive, returnedNegative);
        }
        else
        {
          Debug.LogError("Dont find alert with field \"slug\" = " + slug);
        }
      }
    }
    
  }

  
  //Custom inspector starts here
#if UNITY_EDITOR
 
  [CustomEditor(typeof(WorkSpaceSettings))]
  public class EnumInspectorEditor : Editor
  {
    private WorkSpaceSettings workSpaceSettings;
    
    public int toolbarInt = 0;
    
    private GUIStyle _headerStyle;
    private GUIStyle _tabStyle;
    private GUIStyle _titleStyle;
    private GUIStyle _buttonOnStyle;
    private GUIStyle _buttonOffStyle;
    private GUIStyle _labelStyle;
    private GUIStyle _selectStyle;
    private GUIStyle _inputFieldStyle;
    private GUISkin _skin;
    private GUIContent[] _contentToolbar = new GUIContent[] { };
    private Rect _currRect;
    private static readonly Color backgroundColor = new Color(0.2235294f, 0.254902f, 0.3568628f, 0.2f);
    private static readonly GUILayoutOption _miniButtonWidth = GUILayout.Width(20f);

    private bool _setNewSizeGrid;
    private int _gridColumns = 0;
    private int _gridRows = 0;
    
    private float _bottomPanelHeight = 0;

    private void Awake()
    {
      Initialize();
    }

    private void Initialize()
    {
      workSpaceSettings = target as WorkSpaceSettings;
      if (workSpaceSettings == null) return;
      
      if (!workSpaceSettings.configEditor) return;
      
      _skin = workSpaceSettings.configEditor.guiSkin;
      _tabStyle = _skin.GetStyle("Tab");
      _headerStyle = _skin.GetStyle("Header");
      _buttonOnStyle = _skin.GetStyle("ButtonOn");
      _buttonOffStyle = _skin.GetStyle("ButtonOff");
      _labelStyle = _skin.GetStyle("Label");
      _titleStyle = _skin.GetStyle("Title");
      _selectStyle = _skin.GetStyle("SelectPanel");
      _inputFieldStyle = _skin.GetStyle("InputField");

      if (workSpaceSettings.desktopGrid)
      {
        _gridColumns = workSpaceSettings.desktopGrid.columns;
        _gridRows = workSpaceSettings.desktopGrid.rows;
      }

      if (workSpaceSettings.bottomPanelRoot)
      {
        _bottomPanelHeight = workSpaceSettings.bottomPanelRoot.GetComponent<RectTransform>().rect.height;
      }

      if (workSpaceSettings.desktopGrid)
      {
        if (workSpaceSettings.desktopGrid.transform.childCount > 0)
        {
          if (workSpaceSettings.desktopGrid.transform.GetChild(0).GetComponent<Image>().color == new Color(1, 1, 1, 0.2f))
            workSpaceSettings.gridVisualizeDesktop = true;
        }
      }
    }
    private string GetTitleIfImage(Texture tex, string title)
    {
      return tex ? null : title;
    }
    
    public override void OnInspectorGUI()
    {
      if (workSpaceSettings == null) return;
      
      if(!_skin)
        Initialize();

      serializedObject.Update();
      
      if (!workSpaceSettings.configEditor)
      {
        workSpaceSettings.configEditor = (ConfigEditor) EditorGUILayout.ObjectField("Config for editor", workSpaceSettings.configEditor, typeof(ConfigEditor), true);
        return;
      }

      _contentToolbar = new GUIContent[workSpaceSettings.configEditor.iconsToolbar.Count];
      
      foreach ((Toolbar item, int i) in workSpaceSettings.configEditor.iconsToolbar.Select((value, i) => (value, i)))
      {
        _contentToolbar[i] = new GUIContent() {text = GetTitleIfImage(item.icon, item.title), image = item.icon, tooltip = item.title};
      }

      toolbarInt = GUILayout.Toolbar(toolbarInt, _contentToolbar, _tabStyle);

      switch (toolbarInt)
      {
        case 0:
          MainPanelsConfig();
          break;
        case 1:
          WindowsPanelsConfig();
          break;
        case 2:
          FoldersPanelConfigs();
          break;
        case 3:
          BottomPanelConfig();
          break;
        case 4:
          DesktopPanelsConfig();
          break;
        case 5:
          CursorsPanelConfigs();
          break;
        case 6:
          AlertsPanelConfigs();
          break;
      }
      
      EditorGUILayout.EndVertical();
 
      _currRect = new Rect(_currRect.x - 18, _currRect.y - 5, EditorGUIUtility.currentViewWidth, _currRect.height + 14);
      EditorGUI.DrawRect(_currRect, backgroundColor);
      
      serializedObject.ApplyModifiedProperties();
    }

    private void MainPanelsConfig()
    {
      EditorGUILayout.LabelField("main configs", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);
      
      workSpaceSettings.configEditor = (ConfigEditor) EditorGUILayout.ObjectField("Config for editor", workSpaceSettings.configEditor, typeof(ConfigEditor), true);
      workSpaceSettings.mainCanvas = (Transform) EditorGUILayout.ObjectField("Main Canvas", workSpaceSettings.mainCanvas, typeof(Transform), true);
    }

    private void WindowsPanelsConfig()
    {
      EditorGUILayout.LabelField("window config", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);

        workSpaceSettings.rootWindows = (Transform) EditorGUILayout.ObjectField("Root all windows", workSpaceSettings.rootWindows, typeof(Transform), true);
        if(!workSpaceSettings.rootWindows) return;
        
        workSpaceSettings.windowPrefab = (GameObject) EditorGUILayout.ObjectField("Window prefab", workSpaceSettings.windowPrefab, typeof(GameObject), true);
        if(!workSpaceSettings.windowPrefab) return;
        
        workSpaceSettings.maxWindowsOnScreen = EditorGUILayout.IntField("Max windows on screen", workSpaceSettings.maxWindowsOnScreen, _inputFieldStyle);
        workSpaceSettings.borderRestriction = EditorGUILayout.ToggleLeft("Border restriction", workSpaceSettings.borderRestriction);
        workSpaceSettings.resizableWindow = EditorGUILayout.ToggleLeft("Resizable", workSpaceSettings.resizableWindow);

        workSpaceSettings.openedSizeWindow = (SizeWindow) EditorGUILayout.EnumPopup("Opened size window", workSpaceSettings.openedSizeWindow);
        workSpaceSettings.openedSizeWindowValue = SwitchSize(workSpaceSettings.openedSizeWindow, "opened");
        EditorGUI.indentLevel--;

        workSpaceSettings.minSizeWindow = (SizeWindow) EditorGUILayout.EnumPopup("Min size window", workSpaceSettings.minSizeWindow);
        workSpaceSettings.minSizeWindowValue = SwitchSize(workSpaceSettings.minSizeWindow, "min");
        EditorGUI.indentLevel--;
        
        Vector2 SwitchSize(SizeWindow sizeWindow, string type)
        {
          var rectRootWindowsSize = new Vector2(Screen.width, Screen.height);

          switch (sizeWindow)
          {
            case SizeWindow.Custom:
              EditorGUI.indentLevel++;
              if(type == "min")
                return EditorGUILayout.Vector2Field("Size window", workSpaceSettings.minSizeWindowValue);
              else
                return EditorGUILayout.Vector2Field("Size window", workSpaceSettings.openedSizeWindowValue);
            case SizeWindow.FullScreen:
              EditorGUI.indentLevel++;
              EditorGUILayout.LabelField("Width: " + rectRootWindowsSize.x);
              EditorGUILayout.LabelField("Height: " + rectRootWindowsSize.y);
              return new Vector2(rectRootWindowsSize.x,  rectRootWindowsSize.y);
            case SizeWindow.HalfScreen:
              EditorGUI.indentLevel++;
              EditorGUILayout.LabelField("HalfWidth: " + rectRootWindowsSize.x/2);
              EditorGUILayout.LabelField("HalfHeight: " + rectRootWindowsSize.y/2);
              return new Vector2(rectRootWindowsSize.x/2,  rectRootWindowsSize.y/2);
            default:
              return Vector2.zero;
          }
        }
    }

    private void FoldersPanelConfigs()
    {
      EditorGUILayout.LabelField("folders and files config", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);
      
      workSpaceSettings.fileShortcutPrefab = (GameObject) EditorGUILayout.ObjectField("File shortcut prefab", workSpaceSettings.fileShortcutPrefab, typeof(GameObject), true);
      if(!workSpaceSettings.fileShortcutPrefab) return;

      workSpaceSettings.isMovingDrag = EditorGUILayout.ToggleLeft("Moving drag", workSpaceSettings.isMovingDrag);
      if (workSpaceSettings.isMovingDrag)
      {
        EditorGUI.indentLevel++;
        workSpaceSettings.cloneDraggableFolderPrefab = (GameObject) EditorGUILayout.ObjectField("Clone draggable folder prefab", workSpaceSettings.cloneDraggableFolderPrefab, typeof(GameObject), true);
        EditorGUI.indentLevel--;
      }

      workSpaceSettings.openInOneInstance = EditorGUILayout.ToggleLeft("Open in one instance", workSpaceSettings.openInOneInstance);
      
      SerializedProperty listShortcuts = serializedObject.FindProperty("fileShortcutsList");
      
      EditorGUILayout.Separator();
      GUILayout.BeginVertical(_labelStyle);
      EditorGUILayout.LabelField("Shortcuts builder", _titleStyle);
      EditorGUILayout.Separator();

      EditorGUI.indentLevel += 1;

      for (var i = 0; i < listShortcuts.arraySize; i++)
      {
        string titleList = listShortcuts.GetArrayElementAtIndex(i).FindPropertyRelative("title").stringValue;
        string nameList = listShortcuts.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
        Object iconList = listShortcuts.GetArrayElementAtIndex(i).FindPropertyRelative("icon").objectReferenceValue;
        Object contentList = listShortcuts.GetArrayElementAtIndex(i).FindPropertyRelative("contentPrefab").objectReferenceValue;
        bool scrollingList = listShortcuts.GetArrayElementAtIndex(i).FindPropertyRelative("scrollingContent").boolValue;

        var titleListShortcuts = "new shortcut";
        if (titleList != "")
          titleListShortcuts = titleList;
        
        EditorGUILayout.BeginHorizontal();
        listShortcuts.GetArrayElementAtIndex(i).isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(listShortcuts.GetArrayElementAtIndex(i).isExpanded, titleListShortcuts, _selectStyle);
        ShowButtons(listShortcuts, i);
        EditorGUILayout.EndHorizontal();

        if (listShortcuts.arraySize > i && listShortcuts.GetArrayElementAtIndex(i) != null)
        {
          if (listShortcuts.GetArrayElementAtIndex(i).isExpanded)
          {
            listShortcuts.GetArrayElementAtIndex(i).FindPropertyRelative("title").stringValue = EditorGUILayout.TextField("title", titleList, _inputFieldStyle);
            listShortcuts.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue = EditorGUILayout.TextField("name", nameList, _inputFieldStyle);
            listShortcuts.GetArrayElementAtIndex(i).FindPropertyRelative("icon").objectReferenceValue = (Sprite) EditorGUILayout.ObjectField("icon", iconList, typeof(Sprite), true);
            listShortcuts.GetArrayElementAtIndex(i).FindPropertyRelative("contentPrefab").objectReferenceValue = (GameObject) EditorGUILayout.ObjectField("content prefab", contentList, typeof(GameObject), true);
            listShortcuts.GetArrayElementAtIndex(i).FindPropertyRelative("scrollingContent").boolValue = EditorGUILayout.Toggle("scrolling content", scrollingList);
          }
        }

        EditorGUI.EndFoldoutHeaderGroup();
      }

      if (GUILayout.Button(addButtonContent, _buttonOnStyle))
      {
        listShortcuts.arraySize += 1;
      }

      EditorGUI.indentLevel -= 1;
      
      EditorGUILayout.Separator();
      GUILayout.EndVertical();
      
    }
    
    private void ShowButtons (SerializedProperty list, int index) {

      if (workSpaceSettings.desktopGrid)
      {
        if (GUILayout.Button(spawnButtonContent, EditorStyles.miniButtonLeft, _miniButtonWidth))
        {
          for (var i = 0; i < workSpaceSettings.desktopGrid.transform.childCount; i++)
          {
            Transform cell = workSpaceSettings.desktopGrid.transform.GetChild(i);
            
            if (cell.childCount == 0)
            {
              string path = AssetDatabase.GetAssetPath(workSpaceSettings.fileShortcutPrefab); 
              var pref  = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
              var obj = (GameObject) PrefabUtility.InstantiatePrefab(pref);
              obj.transform.SetParent(cell);

              SerializedProperty element = list.GetArrayElementAtIndex(index);
              
              var prop = new FileShortcut()
              {
                  title = element.FindPropertyRelative("title").stringValue,
                  name = element.FindPropertyRelative("name").stringValue,
                  icon = (Sprite) element.FindPropertyRelative("icon").objectReferenceValue,
                  contentPrefab = (GameObject) element.FindPropertyRelative("contentPrefab").objectReferenceValue,
                  scrollingContent = element.FindPropertyRelative("scrollingContent").boolValue,
              };
              obj.GetComponent<GetDataFile>().Init(prop);
              obj.transform.name = prop.title;
              break;
            }
          }
        }
      }

      if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, _miniButtonWidth)) {
        int oldSize = list.arraySize;
        list.DeleteArrayElementAtIndex(index);
        if (list.arraySize == oldSize) {
          list.DeleteArrayElementAtIndex(index);
        }
      }
    }
    
    private static GUIContent
      spawnButtonContent = new GUIContent("+", "spawn on desktop"),
      deleteButtonContent = new GUIContent("\u2613", "delete"),
      addButtonContent = new GUIContent("Add item", "add element");

    private void BottomPanelConfig()
    {
      EditorGUILayout.LabelField("task bar config", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);
      
      workSpaceSettings.bottomPanelController = (BottomPanelController) EditorGUILayout.ObjectField("Bottom panel controller", workSpaceSettings.bottomPanelController, typeof(BottomPanelController), true);
      
      workSpaceSettings.bottomPanelRoot = (Transform) EditorGUILayout.ObjectField("Bottom panel root", workSpaceSettings.bottomPanelRoot, typeof(Transform), true);

      if(!workSpaceSettings.bottomPanelRoot) return;
      
      EditorGUILayout.BeginHorizontal();
      _bottomPanelHeight = EditorGUILayout.Slider("Panel height (" + workSpaceSettings.bottomPanelRoot.GetComponent<LayoutElement>().minHeight + " px)", _bottomPanelHeight, 3, Screen.height);
      
      if(GUILayout.Button("set",  EditorStyles.miniButtonLeft, GUILayout.Width(50f)))
      {
        workSpaceSettings.bottomPanelRoot.GetComponent<LayoutElement>().minHeight = _bottomPanelHeight;
        workSpaceSettings.bottomPanelRoot.GetComponent<LayoutElement>().preferredHeight = _bottomPanelHeight;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(workSpaceSettings.bottomPanelRoot.GetComponent<RectTransform>());
        
        SceneView.RepaintAll ();
        EditorApplication.QueuePlayerLoopUpdate();
        
      }
      EditorGUILayout.EndHorizontal();
    }
    
    private void DesktopPanelsConfig()
    {
      EditorGUILayout.LabelField("desktop config", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);

      workSpaceSettings.desktopGrid = (FlexibleGridLayout) EditorGUILayout.ObjectField("Desktop grid", workSpaceSettings.desktopGrid, typeof(FlexibleGridLayout), true);
      if (!workSpaceSettings.desktopGrid) return;
      
      workSpaceSettings.gridPrefab = (GameObject) EditorGUILayout.ObjectField("Cell prefab", workSpaceSettings.gridPrefab, typeof(GameObject), true);
      if (!workSpaceSettings.gridPrefab) return;
      
        EditorGUILayout.Separator();

        GUILayout.BeginHorizontal("box");
        bool buttOnVis;
        bool buttOffVis;

        if (workSpaceSettings.gridVisualizeDesktop)
        {
          buttOnVis = GUILayout.Button("Show visualize grid", _buttonOffStyle);
          buttOffVis = GUILayout.Button("Hide visualize grid", _buttonOnStyle);
        }
        else
        {
          buttOnVis = GUILayout.Button("Show visualize grid", _buttonOnStyle);
          buttOffVis = GUILayout.Button("Hide visualize grid", _buttonOffStyle);
        }
        GUILayout.EndHorizontal();
        
        if(buttOnVis && !workSpaceSettings.gridVisualizeDesktop)
        {
          workSpaceSettings.gridVisualizeDesktop = true;
          VisualizeGrid(new Color(1f, 1f, 1f, 0.2f));
        }
        if (buttOffVis && workSpaceSettings.gridVisualizeDesktop)
        {
          workSpaceSettings.gridVisualizeDesktop = false;
          VisualizeGrid(new Color(1f, 1f, 1f, 0f));
        }

        var labelS = new GUIStyle("Box") {richText = true};

        EditorGUILayout.Separator();
        GUILayout.BeginVertical(_labelStyle);
        EditorGUILayout.LabelField("Cell size:", _titleStyle);
        EditorGUILayout.Separator();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Width: <b>" + workSpaceSettings.desktopGrid.cellSize.x + "</b> px", labelS);
        EditorGUILayout.LabelField("Height: <b>" + workSpaceSettings.desktopGrid.cellSize.y + "</b> px", labelS);
        GUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        GUILayout.EndVertical();
        
        EditorGUILayout.Separator();
        GUILayout.BeginVertical(_labelStyle);
        EditorGUILayout.LabelField("Grid size", _titleStyle);
        EditorGUILayout.Separator();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Columns: <b>" + workSpaceSettings.desktopGrid.columns + "</b>", labelS);
        EditorGUILayout.LabelField("Rows: <b>" + workSpaceSettings.desktopGrid.rows + "</b>", labelS);
        GUILayout.EndHorizontal();
        
        
        EditorGUI.indentLevel++;
        _setNewSizeGrid = EditorGUILayout.BeginFoldoutHeaderGroup(_setNewSizeGrid, "Set new size grid", _selectStyle);
        EditorGUI.indentLevel--;
        
        if (_setNewSizeGrid)
        {
          _gridColumns = EditorGUILayout.IntSlider("Columns", _gridColumns, 3, 100);
          _gridRows = EditorGUILayout.IntSlider("Rows", _gridRows, 3, 100);

          if(GUILayout.Button("Apply new grid", _buttonOnStyle))
          {
            workSpaceSettings.desktopGrid.columns = _gridColumns;
            workSpaceSettings.desktopGrid.rows = _gridRows;

            int countCells = _gridColumns * _gridRows;
            int differenceCells = workSpaceSettings.desktopGrid.transform.childCount - countCells;

            if (differenceCells < 0)
            {
              string path = AssetDatabase.GetAssetPath(workSpaceSettings.gridPrefab); 
              var pref  = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
              for (var i = 0; i < Math.Abs(differenceCells); i++)
              {
                var obj = (GameObject) PrefabUtility.InstantiatePrefab(pref);
                obj.transform.SetParent(workSpaceSettings.desktopGrid.transform);
              }
            } else if (differenceCells > 0)
            {
              for (var i = 0; i < Math.Abs(differenceCells); i++)
              {
                GameObject obj = workSpaceSettings.desktopGrid.transform.GetChild(workSpaceSettings.desktopGrid.transform.childCount - 1).gameObject;
                DestroyImmediate(obj);
                
              }
            }
            SceneView.RepaintAll ();
            EditorApplication.QueuePlayerLoopUpdate();
          }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Separator();
        GUILayout.EndVertical();

    }

    private void VisualizeGrid(Color color)
    {
      string path = AssetDatabase.GetAssetPath(workSpaceSettings.gridPrefab);
      var obj  = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
      for (var i = 0; i < workSpaceSettings.desktopGrid.transform.childCount; i++)
      {
        workSpaceSettings.desktopGrid.transform.GetChild(i).GetComponent<Image>().color = color;
      }
      if (obj != null) obj.GetComponent<Image>().color = color;
      EditorUtility.SetDirty(obj);
      
      SceneView.RepaintAll ();
      EditorApplication.QueuePlayerLoopUpdate();
    }
    
    private void CursorsPanelConfigs()
    {
      EditorGUILayout.LabelField("cursors config", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);
      
      workSpaceSettings.cursorDefault = (Texture2D) EditorGUILayout.ObjectField("Cursor default", workSpaceSettings.cursorDefault, typeof(Texture2D), true);
      workSpaceSettings.cursorHorizontal = (Texture2D) EditorGUILayout.ObjectField("Cursor horizontal resize", workSpaceSettings.cursorHorizontal, typeof(Texture2D), true);
      workSpaceSettings.cursorVertical = (Texture2D) EditorGUILayout.ObjectField("Cursor vertical resize", workSpaceSettings.cursorVertical, typeof(Texture2D), true);
      workSpaceSettings.cursorAngleLeft = (Texture2D) EditorGUILayout.ObjectField("Cursor angle left resize", workSpaceSettings.cursorAngleLeft, typeof(Texture2D), true);
      workSpaceSettings.cursorAngleRight = (Texture2D) EditorGUILayout.ObjectField("Cursor angle right resize", workSpaceSettings.cursorAngleRight, typeof(Texture2D), true);
      workSpaceSettings.cursorDraggable = (Texture2D) EditorGUILayout.ObjectField("Cursor draggable", workSpaceSettings.cursorDraggable, typeof(Texture2D), true);
    }

    private void AlertsPanelConfigs()
    {
      EditorGUILayout.LabelField("alerts config", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);
      
      workSpaceSettings.alertsRoot = (Transform) EditorGUILayout.ObjectField("Alerts root", workSpaceSettings.alertsRoot, typeof(Transform), true);
      if(!workSpaceSettings.alertsRoot) return;
      
      workSpaceSettings.alertPrefab = (GameObject) EditorGUILayout.ObjectField("Alert prefab", workSpaceSettings.alertPrefab, typeof(GameObject), true);
      if(!workSpaceSettings.alertPrefab) return;

      EditorGUILayout.Separator();
      GUILayout.BeginVertical(_labelStyle);
      EditorGUILayout.LabelField("Alert builder:", _titleStyle);
      EditorGUILayout.Separator();

      EditorGUI.indentLevel += 1;
      
      SerializedProperty listAlerts = serializedObject.FindProperty("alertsList");
      
      
      EditorGUILayout.PropertyField(listAlerts);

      EditorGUI.indentLevel -= 1;
      EditorGUILayout.Separator();
      GUILayout.EndVertical();
    }

    private void DrawUILine(Color color, int thickness = 1, int padding = 10)
    {
      Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
      r.height = thickness;
      r.y+=padding/2;
      r.x-=2;
      r.width +=6;
      EditorGUI.DrawRect(r, color);
    }
    
    private void RepaintUnfocusedSceneViews () {
      if (SceneView.sceneViews.Count > 1) {
        foreach (SceneView sv in SceneView.sceneViews) {
          if (EditorWindow.focusedWindow != (EditorWindow) sv) {
            sv.Repaint ();
          }
        }
      }
    }
    
  }
 
#endif
}

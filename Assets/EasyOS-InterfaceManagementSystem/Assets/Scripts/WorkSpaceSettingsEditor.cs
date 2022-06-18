using System;
using System.Linq;
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
  //Custom inspector starts here
#if UNITY_EDITOR
  
  [CustomEditor(typeof(WorkSpaceSettings))]
  public class WorkSpaceSettingsEditor : Editor
  {
    private WorkSpaceSettings _workSpaceSettings;
    
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
    private static readonly Color BackgroundColor = new Color(0.2235294f, 0.254902f, 0.3568628f, 0.2f);
    private static readonly GUILayoutOption MiniButtonWidth = GUILayout.Width(20f);

    private bool _setNewSizeGrid;
    private int _gridColumns = 0;
    private int _gridRows = 0;
    
    private float _bottomPanelHeight = 0;
    
    private static readonly GUIContent
      SpawnButtonContent = new GUIContent("+", "spawn on desktop"),
      DeleteButtonContent = new GUIContent("\u2613", "delete"),
      AddButtonContent = new GUIContent("Add item", "add element");

    private void Awake()
    {
      Initialize();
    }

    private void Initialize()
    {
      _workSpaceSettings = target as WorkSpaceSettings;
      if (_workSpaceSettings == null) return;
      
      if (!_workSpaceSettings.configEditor) return;
      
      _skin = _workSpaceSettings.configEditor.guiSkin;
      _tabStyle = _skin.GetStyle("Tab");
      _headerStyle = _skin.GetStyle("Header");
      _buttonOnStyle = _skin.GetStyle("ButtonOn");
      _buttonOffStyle = _skin.GetStyle("ButtonOff");
      _labelStyle = _skin.GetStyle("Label");
      _titleStyle = _skin.GetStyle("Title");
      _selectStyle = _skin.GetStyle("SelectPanel");
      _inputFieldStyle = _skin.GetStyle("InputField");

      if (_workSpaceSettings.desktopGrid)
      {
        _gridColumns = _workSpaceSettings.desktopGrid.columns;
        _gridRows = _workSpaceSettings.desktopGrid.rows;
      }

      if (_workSpaceSettings.bottomPanelRoot)
      {
        _bottomPanelHeight = _workSpaceSettings.bottomPanelRoot.GetComponent<RectTransform>().rect.height;
      }

      if (_workSpaceSettings.desktopGrid)
      {
        if (_workSpaceSettings.desktopGrid.transform.childCount > 0)
        {
          if (_workSpaceSettings.desktopGrid.transform.GetChild(0).GetComponent<Image>().color == new Color(1, 1, 1, 0.2f))
            _workSpaceSettings.gridVisualizeDesktop = true;
        }
      }
    }
    private string GetTitleIfImage(Texture tex, string title)
    {
      return tex ? null : title;
    }
    
    public override void OnInspectorGUI()
    {
      if (_workSpaceSettings == null) return;
      
      if(!_skin)
        Initialize();

      serializedObject.Update();
      
      if (!_workSpaceSettings.configEditor)
      {
        _workSpaceSettings.configEditor = (ConfigEditor) EditorGUILayout.ObjectField("Config for editor", _workSpaceSettings.configEditor, typeof(ConfigEditor), true);
        return;
      }

      _contentToolbar = new GUIContent[_workSpaceSettings.configEditor.iconsToolbar.Count];
      
      foreach ((Toolbar item, int i) in _workSpaceSettings.configEditor.iconsToolbar.Select((value, i) => (value, i)))
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
      EditorGUI.DrawRect(_currRect, BackgroundColor);
      
      serializedObject.ApplyModifiedProperties();
    }

    private void MainPanelsConfig()
    {
      EditorGUILayout.LabelField("main configs", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);
      
      _workSpaceSettings.configEditor = (ConfigEditor) EditorGUILayout.ObjectField("Config for editor", _workSpaceSettings.configEditor, typeof(ConfigEditor), true);
      _workSpaceSettings.mainCanvas = (Transform) EditorGUILayout.ObjectField("Main Canvas", _workSpaceSettings.mainCanvas, typeof(Transform), true);
    }

    private void WindowsPanelsConfig()
    {
      EditorGUILayout.LabelField("window config", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);

      _workSpaceSettings.rootWindows = (Transform) EditorGUILayout.ObjectField("Root all windows", _workSpaceSettings.rootWindows, typeof(Transform), true);
      if(!_workSpaceSettings.rootWindows) return;
        
      _workSpaceSettings.windowPrefab = (GameObject) EditorGUILayout.ObjectField("Window prefab", _workSpaceSettings.windowPrefab, typeof(GameObject), true);
      if(!_workSpaceSettings.windowPrefab) return;
        
      _workSpaceSettings.maxWindowsOnScreen = EditorGUILayout.IntField("Max windows on screen", _workSpaceSettings.maxWindowsOnScreen, _inputFieldStyle);
      _workSpaceSettings.borderRestriction = EditorGUILayout.ToggleLeft("Border restriction", _workSpaceSettings.borderRestriction);
      _workSpaceSettings.resizableWindow = EditorGUILayout.ToggleLeft("Resizable", _workSpaceSettings.resizableWindow);

      _workSpaceSettings.openedSizeWindow = (SizeWindow) EditorGUILayout.EnumPopup("Opened size window", _workSpaceSettings.openedSizeWindow);
      _workSpaceSettings.openedSizeWindowValue = SwitchSize(_workSpaceSettings.openedSizeWindow, "opened");
      EditorGUI.indentLevel--;

      _workSpaceSettings.minSizeWindow = (SizeWindow) EditorGUILayout.EnumPopup("Min size window", _workSpaceSettings.minSizeWindow);
      _workSpaceSettings.minSizeWindowValue = SwitchSize(_workSpaceSettings.minSizeWindow, "min");
      EditorGUI.indentLevel--;
        
      Vector2 SwitchSize(SizeWindow sizeWindow, string type)
      {
        var rectRootWindowsSize = new Vector2(Screen.width, Screen.height);

        switch (sizeWindow)
        {
          case SizeWindow.Custom:
            EditorGUI.indentLevel++;
            if(type == "min")
              return EditorGUILayout.Vector2Field("Size window", _workSpaceSettings.minSizeWindowValue);
            else
              return EditorGUILayout.Vector2Field("Size window", _workSpaceSettings.openedSizeWindowValue);
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
      
      _workSpaceSettings.fileShortcutPrefab = (GameObject) EditorGUILayout.ObjectField("File shortcut prefab", _workSpaceSettings.fileShortcutPrefab, typeof(GameObject), true);
      if(!_workSpaceSettings.fileShortcutPrefab) return;

      _workSpaceSettings.isMovingDrag = EditorGUILayout.ToggleLeft("Moving drag", _workSpaceSettings.isMovingDrag);
      if (_workSpaceSettings.isMovingDrag)
      {
        EditorGUI.indentLevel++;
        _workSpaceSettings.cloneDraggableFolderPrefab = (GameObject) EditorGUILayout.ObjectField("Clone draggable folder prefab", _workSpaceSettings.cloneDraggableFolderPrefab, typeof(GameObject), true);
        EditorGUI.indentLevel--;
      }

      _workSpaceSettings.openInOneInstance = EditorGUILayout.ToggleLeft("Open in one instance", _workSpaceSettings.openInOneInstance);
      
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

      if (GUILayout.Button(AddButtonContent, _buttonOnStyle))
      {
        listShortcuts.arraySize += 1;
      }

      EditorGUI.indentLevel -= 1;
      
      EditorGUILayout.Separator();
      GUILayout.EndVertical();
      
    }
    
    private void ShowButtons (SerializedProperty list, int index) {

      if (_workSpaceSettings.desktopGrid)
      {
        if (GUILayout.Button(SpawnButtonContent, EditorStyles.miniButtonLeft, MiniButtonWidth))
        {
          for (var i = 0; i < _workSpaceSettings.desktopGrid.transform.childCount; i++)
          {
            Transform cell = _workSpaceSettings.desktopGrid.transform.GetChild(i);
            
            if (cell.childCount == 0)
            {
              string path = AssetDatabase.GetAssetPath(_workSpaceSettings.fileShortcutPrefab); 
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

      if (GUILayout.Button(DeleteButtonContent, EditorStyles.miniButtonRight, MiniButtonWidth)) {
        int oldSize = list.arraySize;
        list.DeleteArrayElementAtIndex(index);
        if (list.arraySize == oldSize) {
          list.DeleteArrayElementAtIndex(index);
        }
      }
    }

    private void BottomPanelConfig()
    {
      EditorGUILayout.LabelField("task bar config", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);
      
      _workSpaceSettings.bottomPanelController = (BottomPanelController) EditorGUILayout.ObjectField("Bottom panel controller", _workSpaceSettings.bottomPanelController, typeof(BottomPanelController), true);
      
      _workSpaceSettings.bottomPanelRoot = (Transform) EditorGUILayout.ObjectField("Bottom panel root", _workSpaceSettings.bottomPanelRoot, typeof(Transform), true);

      if(!_workSpaceSettings.bottomPanelRoot) return;
      
      EditorGUILayout.BeginHorizontal();
      _bottomPanelHeight = EditorGUILayout.Slider("Panel height (" + _workSpaceSettings.bottomPanelRoot.GetComponent<LayoutElement>().minHeight + " px)", _bottomPanelHeight, 3, Screen.height);
      
      if(GUILayout.Button("set",  EditorStyles.miniButtonLeft, GUILayout.Width(50f)))
      {
        _workSpaceSettings.bottomPanelRoot.GetComponent<LayoutElement>().minHeight = _bottomPanelHeight;
        _workSpaceSettings.bottomPanelRoot.GetComponent<LayoutElement>().preferredHeight = _bottomPanelHeight;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_workSpaceSettings.bottomPanelRoot.GetComponent<RectTransform>());
        
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

      _workSpaceSettings.desktopGrid = (FlexibleGridLayout) EditorGUILayout.ObjectField("Desktop grid", _workSpaceSettings.desktopGrid, typeof(FlexibleGridLayout), true);
      if (!_workSpaceSettings.desktopGrid) return;
      
      _workSpaceSettings.gridPrefab = (GameObject) EditorGUILayout.ObjectField("Cell prefab", _workSpaceSettings.gridPrefab, typeof(GameObject), true);
      if (!_workSpaceSettings.gridPrefab) return;
      
      EditorGUILayout.Separator();

      GUILayout.BeginHorizontal("box");
      bool buttOnVis;
      bool buttOffVis;

      if (_workSpaceSettings.gridVisualizeDesktop)
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
        
      if(buttOnVis && !_workSpaceSettings.gridVisualizeDesktop)
      {
        _workSpaceSettings.gridVisualizeDesktop = true;
        VisualizeGrid(new Color(1f, 1f, 1f, 0.2f));
      }
      if (buttOffVis && _workSpaceSettings.gridVisualizeDesktop)
      {
        _workSpaceSettings.gridVisualizeDesktop = false;
        VisualizeGrid(new Color(1f, 1f, 1f, 0f));
      }

      var labelS = new GUIStyle("Box") {richText = true};

      EditorGUILayout.Separator();
      GUILayout.BeginVertical(_labelStyle);
      EditorGUILayout.LabelField("Cell size:", _titleStyle);
      EditorGUILayout.Separator();
      GUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Width: <b>" + _workSpaceSettings.desktopGrid.cellSize.x + "</b> px", labelS);
      EditorGUILayout.LabelField("Height: <b>" + _workSpaceSettings.desktopGrid.cellSize.y + "</b> px", labelS);
      GUILayout.EndHorizontal();
      EditorGUILayout.Separator();
      GUILayout.EndVertical();
        
      EditorGUILayout.Separator();
      GUILayout.BeginVertical(_labelStyle);
      EditorGUILayout.LabelField("Grid size", _titleStyle);
      EditorGUILayout.Separator();
      GUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Columns: <b>" + _workSpaceSettings.desktopGrid.columns + "</b>", labelS);
      EditorGUILayout.LabelField("Rows: <b>" + _workSpaceSettings.desktopGrid.rows + "</b>", labelS);
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
          _workSpaceSettings.desktopGrid.columns = _gridColumns;
          _workSpaceSettings.desktopGrid.rows = _gridRows;

          int countCells = _gridColumns * _gridRows;
          int differenceCells = _workSpaceSettings.desktopGrid.transform.childCount - countCells;

          if (differenceCells < 0)
          {
            string path = AssetDatabase.GetAssetPath(_workSpaceSettings.gridPrefab); 
            var pref  = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            for (var i = 0; i < Math.Abs(differenceCells); i++)
            {
              var obj = (GameObject) PrefabUtility.InstantiatePrefab(pref);
              obj.transform.SetParent(_workSpaceSettings.desktopGrid.transform);
            }
          } else if (differenceCells > 0)
          {
            for (var i = 0; i < Math.Abs(differenceCells); i++)
            {
              GameObject obj = _workSpaceSettings.desktopGrid.transform.GetChild(_workSpaceSettings.desktopGrid.transform.childCount - 1).gameObject;
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
      string path = AssetDatabase.GetAssetPath(_workSpaceSettings.gridPrefab);
      var obj  = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
      for (var i = 0; i < _workSpaceSettings.desktopGrid.transform.childCount; i++)
      {
        _workSpaceSettings.desktopGrid.transform.GetChild(i).GetComponent<Image>().color = color;
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
      
      _workSpaceSettings.cursorDefault = (Texture2D) EditorGUILayout.ObjectField("Cursor default", _workSpaceSettings.cursorDefault, typeof(Texture2D), true);
      _workSpaceSettings.cursorHorizontal = (Texture2D) EditorGUILayout.ObjectField("Cursor horizontal resize", _workSpaceSettings.cursorHorizontal, typeof(Texture2D), true);
      _workSpaceSettings.cursorVertical = (Texture2D) EditorGUILayout.ObjectField("Cursor vertical resize", _workSpaceSettings.cursorVertical, typeof(Texture2D), true);
      _workSpaceSettings.cursorAngleLeft = (Texture2D) EditorGUILayout.ObjectField("Cursor angle left resize", _workSpaceSettings.cursorAngleLeft, typeof(Texture2D), true);
      _workSpaceSettings.cursorAngleRight = (Texture2D) EditorGUILayout.ObjectField("Cursor angle right resize", _workSpaceSettings.cursorAngleRight, typeof(Texture2D), true);
      _workSpaceSettings.cursorDraggable = (Texture2D) EditorGUILayout.ObjectField("Cursor draggable", _workSpaceSettings.cursorDraggable, typeof(Texture2D), true);
    }

    private void AlertsPanelConfigs()
    {
      EditorGUILayout.LabelField("alerts config", _headerStyle);
      _currRect = EditorGUILayout.BeginVertical();
      EditorGUILayout.Space(15);
      
      _workSpaceSettings.alertsRoot = (Transform) EditorGUILayout.ObjectField("Alerts root", _workSpaceSettings.alertsRoot, typeof(Transform), true);
      if(!_workSpaceSettings.alertsRoot) return;
      
      _workSpaceSettings.alertPrefab = (GameObject) EditorGUILayout.ObjectField("Alert prefab", _workSpaceSettings.alertPrefab, typeof(GameObject), true);
      if(!_workSpaceSettings.alertPrefab) return;

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
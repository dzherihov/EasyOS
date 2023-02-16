using System;
using System.Collections.Generic;
using Assets.Scripts.Alert;
using Assets.Scripts.Base;
using Assets.Scripts.BottomPanel;
using Assets.Scripts.Configs;
using Assets.Scripts.Folder;
using UnityEngine;

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

  //Maintaining folder and file content hierarchy.
  //This class can be converted to json format to store data on the server or locally.
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
  public class ContextMenuItem
  {
    public string name;
    public Sprite icon;
    public GameObject prefab;
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
    
    [Header("Context menu"), Space(15)] 
    public GameObject contextPrefab;
    public List<ContextMenuItem> contextMenuItems;


    // [Header("Context menu")] 
    // [HideInInspector] public GameObject tempContextMenu;
    
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

  



}

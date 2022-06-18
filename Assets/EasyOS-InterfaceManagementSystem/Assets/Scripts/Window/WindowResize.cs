using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Window
{
   public class WindowResize : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
   {
      private enum DirectionCursor {Horizontal = 0, Vertical, AngleLeft, AngleRight };
      private enum DirectionResize {Left, Right, Top, Bottom, LeftTop, RightTop, BottomLeft, BottomRight };

      [SerializeField] private DirectionCursor _directionCursor;
      [SerializeField] private DirectionResize _directionResize;
      [SerializeField] private RectTransform _rootWindow;

      private float _startWidthWindow;
      private float _startHeightWindow;
      private Vector3 _startPosWindow;
      private float _pointPosX, _pointPosY;
      private Vector2 _minSizeWindow;
      private int _dirX;
      private int _dirY;
      private Vector3 _minPosCamera,_maxPosCamera;
      private float _widthWindow, _heightWindow;
      private float _pointPosXRestriction, _pointPosYRestriction;
      
      private WindowActions _windowActions;
      private bool _borderRestriction;
      private RectTransform _bottomPanel;

      private static bool isResize = false;
      
      private void Awake()
      {
         _windowActions = _rootWindow.GetComponent<WindowActions>();
      }

      private void Start()
      {
         if(WorkSpaceSettings.Instance.bottomPanelController)
            _bottomPanel = WorkSpaceSettings.Instance.bottomPanelController.GetComponent<RectTransform>();
      }

      private void ChooseCursor()
      {
         switch (_directionCursor)
         {
            case DirectionCursor.Horizontal:
               WorkSpaceSettings.Instance.SetCursor(WorkSpaceSettings.Instance.cursorHorizontal);
               break;
            case DirectionCursor.Vertical:
               WorkSpaceSettings.Instance.SetCursor(WorkSpaceSettings.Instance.cursorVertical);
               break;
            case DirectionCursor.AngleLeft:
               WorkSpaceSettings.Instance.SetCursor(WorkSpaceSettings.Instance.cursorAngleLeft);
               break;
            case DirectionCursor.AngleRight:
               WorkSpaceSettings.Instance.SetCursor(WorkSpaceSettings.Instance.cursorAngleRight);
               break;
            default:
               WorkSpaceSettings.Instance.SetCursor(WorkSpaceSettings.Instance.cursorDefault);
               break;
         }
      }

      public void OnPointerEnter(PointerEventData eventData)
      {
         if(!WorkSpaceSettings.Instance.resizableWindow) return;
         if (_windowActions.isFullScreen) return;
         if(!isResize && !WindowDrug.isDrag)
            ChooseCursor();
      }

      public void OnPointerExit(PointerEventData eventData)
      {
         if(!WorkSpaceSettings.Instance.resizableWindow) return;
         if (_windowActions.isFullScreen) return;
         
         if(!isResize && !WindowDrug.isDrag)
            WorkSpaceSettings.Instance.SetDefaultCursor();

      }
      
      public void OnBeginDrag(PointerEventData eventData)
      {
         if(!WorkSpaceSettings.Instance.resizableWindow || eventData.button != PointerEventData.InputButton.Left) return;
         if (_windowActions.isFullScreen) return;

         isResize = true;
         
         ChooseCursor();
         _borderRestriction = WorkSpaceSettings.Instance.borderRestriction;
         
         _startWidthWindow = _rootWindow.rect.width;
         _startHeightWindow = _rootWindow.rect.height;
         _pointPosX = Input.mousePosition.x;
         _pointPosY = Input.mousePosition.y;

         _startPosWindow = _rootWindow.position;
         _windowActions.SortToTopFocus();
         
         if(_bottomPanel)
            _minPosCamera = Camera.main.ViewportToScreenPoint(new Vector2(0,  _bottomPanel.rect.height/Screen.height));
         else
            _minPosCamera = Camera.main.ViewportToScreenPoint(new Vector2(0, 0));
         
         _maxPosCamera = Camera.main.ViewportToScreenPoint(new Vector2(1, 1));
         _widthWindow = _rootWindow.localToWorldMatrix[0] * _rootWindow.GetComponent<RectTransform>().rect.width;
         _heightWindow = _rootWindow.localToWorldMatrix[0] * _rootWindow.GetComponent<RectTransform>().rect.height;
         _pointPosXRestriction = Input.mousePosition.x - _startPosWindow.x;
         _pointPosYRestriction = Input.mousePosition.y - _startPosWindow.y;
      }

      public void OnDrag(PointerEventData eventData)
      {
         if(!WorkSpaceSettings.Instance.resizableWindow || eventData.button != PointerEventData.InputButton.Left) return;
         if (_windowActions.isFullScreen) return;
         SetResize();
         _minSizeWindow = WorkSpaceSettings.Instance.minSizeWindowValue;
      }
      
      private void SetResize()
      {
         switch (_directionResize)
         {
            case DirectionResize.Left:
               SetLeftSize();
               break;
            case DirectionResize.Right:
               SetRightSize();
               break;
            case DirectionResize.Top:
               SetTopSize();
               break;
            case DirectionResize.Bottom:
               SetBottomSize();
               break;
            case DirectionResize.LeftTop:
               SetLeftSize();
               SetTopSize();
               break;
            case DirectionResize.RightTop:
               SetRightSize();
               SetTopSize();
               break;
            case DirectionResize.BottomLeft:
               SetBottomSize();
               SetLeftSize();
               break;
            case DirectionResize.BottomRight:
               SetBottomSize();
               SetRightSize();
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
         _rootWindow.ForceUpdateRectTransforms();
      }

      public void OnEndDrag(PointerEventData eventData)
      {
         if(!WorkSpaceSettings.Instance.resizableWindow || eventData.button != PointerEventData.InputButton.Left) return;
         if (_windowActions.isFullScreen) return;
         
         isResize = false;
         
         WorkSpaceSettings.Instance.SetDefaultCursor();
         ResetPivot();
      }

      private void ResetPivot()
      {
         _rootWindow.pivot= new Vector2(0.5f,0.5f);
         Vector3 position = _rootWindow.position;
         Rect rect = _rootWindow.rect;
         position = new Vector2(position.x + ((rect.width*0.5f) * _dirX) ,position.y + ((rect.height*0.5f) * _dirY));
         _rootWindow.position = position;
      }
      
      private void SetBottomSize()
      {
         if (_borderRestriction && Input.mousePosition.y <= (_minPosCamera.y + _heightWindow / 2) + _pointPosYRestriction) return;
         if (!(_startHeightWindow - (Input.mousePosition.y - _pointPosY) >= _minSizeWindow.y)) return;
         _rootWindow.pivot = new Vector2(_rootWindow.pivot.x, 1);
         _rootWindow.position = new Vector2(_rootWindow.position.x, _startPosWindow.y + (_startHeightWindow * 0.5f));

         _rootWindow.sizeDelta = new Vector2(_rootWindow.sizeDelta.x, _startHeightWindow - (Input.mousePosition.y - _pointPosY));
         _dirY = -1;
      }

      private void SetTopSize()
      {
         if (_borderRestriction && Input.mousePosition.y >= (_maxPosCamera.y - _heightWindow / 2) + _pointPosYRestriction) return;
         if (!(_startHeightWindow + (Input.mousePosition.y - _pointPosY) >= _minSizeWindow.y)) return;
         _rootWindow.pivot = new Vector2(_rootWindow.pivot.x, 0);
         _rootWindow.position = new Vector2(_rootWindow.position.x, _startPosWindow.y - (_startHeightWindow * 0.5f));
         _rootWindow.sizeDelta = new Vector2(_rootWindow.sizeDelta.x, _startHeightWindow + (Input.mousePosition.y - _pointPosY));
         _dirY = 1;
         // _rootWindow.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom,_startPosWindow.y - (_startHeightWindow/2), _startHeightWindow + (Input.mousePosition.y - _pointPosY));
      }

      private void SetLeftSize()
      {
         if (_borderRestriction && Input.mousePosition.x <= (_minPosCamera.x + _widthWindow / 2) + _pointPosXRestriction) return;
         if (!(_startWidthWindow - (Input.mousePosition.x - _pointPosX) >= _minSizeWindow.x)) return;
         _rootWindow.pivot= new Vector2(1, _rootWindow.pivot.y);
         _rootWindow.position = new Vector2(_startPosWindow.x + (_startWidthWindow*0.5f),_rootWindow.position.y);
         _rootWindow.sizeDelta = new Vector2(_startWidthWindow - (Input.mousePosition.x - _pointPosX),_rootWindow.sizeDelta.y);
         _dirX = -1;
      }

      private void SetRightSize()
      {
         if (_borderRestriction && Input.mousePosition.x >= (_maxPosCamera.x - _widthWindow / 2) + _pointPosXRestriction) return;
         if (!(_startWidthWindow + (Input.mousePosition.x - _pointPosX) >= _minSizeWindow.x)) return;
         _rootWindow.pivot= new Vector2(0, _rootWindow.pivot.y);
         _rootWindow.position = new Vector2(_startPosWindow.x - (_startWidthWindow*0.5f),_rootWindow.position.y);
         _rootWindow.sizeDelta = new Vector2(_startWidthWindow + (Input.mousePosition.x - _pointPosX),_rootWindow.sizeDelta.y);
         _dirX = 1;
         // _rootWindow.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,_startPosWindow.x - (_startWidthWindow/2), _startWidthWindow + (Input.mousePosition.x - _pointPosX));
      }
   }
}

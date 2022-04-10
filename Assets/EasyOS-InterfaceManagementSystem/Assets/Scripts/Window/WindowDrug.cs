using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Window
{
    public class WindowDrug : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerDownHandler, IEndDragHandler
    {
        [SerializeField] private Transform _rootWindow;

        private Vector3 _startPosWindow;
        private float _pointPosX, _pointPosY;
        private Vector3 _minPosCamera,_maxPosCamera;
        private Vector3 _curPosWindow;
        private float _widthWindow, _heightWindow;
        private WindowActions _windowActions;
        private bool _isBorderRestriction;
        private RectTransform _bottomPanel;
        
        public static bool isDrag = false;

        private void Start()
        {
            _windowActions = _rootWindow.GetComponent<WindowActions>();
            if( WorkSpaceSettings.Instance.bottomPanelController)
                _bottomPanel = WorkSpaceSettings.Instance.bottomPanelController.GetComponent<RectTransform>();
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (_windowActions.isFullScreen) return;
            if (_isBorderRestriction)
            {
                if (Input.mousePosition.x >= (_maxPosCamera.x - _widthWindow / 2) + _pointPosX && Input.mousePosition.y < (_maxPosCamera.y - _heightWindow / 2) + _pointPosY && Input.mousePosition.y > (_minPosCamera.y + _heightWindow / 2) + _pointPosY)
                    _curPosWindow = new Vector3(_maxPosCamera.x - _widthWindow / 2, Input.mousePosition.y - _pointPosY, _startPosWindow.z);
                else if (Input.mousePosition.x >= (_maxPosCamera.x - _widthWindow / 2) + _pointPosX && Input.mousePosition.y >= (_maxPosCamera.y - _heightWindow / 2) + _pointPosY)
                    _curPosWindow = new Vector3(_maxPosCamera.x - _widthWindow / 2, _maxPosCamera.y - _heightWindow / 2, _startPosWindow.z);
                else if (Input.mousePosition.x >= (_maxPosCamera.x - _widthWindow / 2) + _pointPosX && Input.mousePosition.y <= (_minPosCamera.y + _heightWindow / 2) + _pointPosY)
                    _curPosWindow = new Vector3(_maxPosCamera.x - _widthWindow / 2, _minPosCamera.y + _heightWindow / 2, _startPosWindow.z);

                if (Input.mousePosition.x <= (_minPosCamera.x + _widthWindow / 2) + _pointPosX && Input.mousePosition.y < (_maxPosCamera.y - _heightWindow / 2) + _pointPosY && Input.mousePosition.y > (_minPosCamera.y + _heightWindow / 2) + _pointPosY)
                    _curPosWindow = new Vector3(_minPosCamera.x + _widthWindow / 2, Input.mousePosition.y - _pointPosY, _startPosWindow.z);
                else if (Input.mousePosition.x <= (_minPosCamera.x + _widthWindow / 2) + _pointPosX && Input.mousePosition.y >= (_maxPosCamera.y - _heightWindow / 2) + _pointPosY)
                    _curPosWindow = new Vector3(_minPosCamera.x + _widthWindow / 2, _maxPosCamera.y - _heightWindow / 2, _startPosWindow.z);
                else if (Input.mousePosition.x <= (_minPosCamera.x + _widthWindow / 2) + _pointPosX && Input.mousePosition.y <= (_minPosCamera.y + _heightWindow / 2) + _pointPosY)
                    _curPosWindow = new Vector3(_minPosCamera.x + _widthWindow / 2, _minPosCamera.y + _heightWindow / 2, _startPosWindow.z);

                if (Input.mousePosition.y >= (_maxPosCamera.y - _heightWindow / 2) + _pointPosY && Input.mousePosition.x > (_minPosCamera.x + _widthWindow / 2) + _pointPosX && Input.mousePosition.x < (_maxPosCamera.x - _widthWindow / 2) + _pointPosX)
                    _curPosWindow = new Vector3(Input.mousePosition.x - _pointPosX, _maxPosCamera.y - _heightWindow / 2, _startPosWindow.z);

                if (Input.mousePosition.y <= (_minPosCamera.y + _heightWindow / 2) + _pointPosY && Input.mousePosition.x > (_minPosCamera.x + _widthWindow / 2) + _pointPosX && Input.mousePosition.x < (_maxPosCamera.x - _widthWindow / 2) + _pointPosX)
                    _curPosWindow = new Vector3(Input.mousePosition.x - _pointPosX, _minPosCamera.y + _heightWindow / 2, _startPosWindow.z);

                if ((Input.mousePosition.y < (_maxPosCamera.y - _heightWindow / 2) + _pointPosY) && (Input.mousePosition.y > (_minPosCamera.y + _heightWindow / 2) + _pointPosY) && (Input.mousePosition.x > (_minPosCamera.x + _widthWindow / 2) + _pointPosX) && (Input.mousePosition.x < (_maxPosCamera.x - _widthWindow / 2) + _pointPosX))
                    _curPosWindow = new Vector3(Input.mousePosition.x - _pointPosX, Input.mousePosition.y - _pointPosY, _startPosWindow.z);
            }
            else
            {
                _curPosWindow = new Vector3(Input.mousePosition.x - _pointPosX, Input.mousePosition.y - _pointPosY, _startPosWindow.z);
            }

            _rootWindow.transform.position = _curPosWindow;
            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_windowActions.isFullScreen)
            {
                _windowActions.ReduceFromFullScreen(Input.mousePosition.x, Input.mousePosition.y);
            }
            
            WorkSpaceSettings.Instance.SetCursor(WorkSpaceSettings.Instance.cursorDraggable);
            isDrag = true;
            
            _startPosWindow = _rootWindow.transform.position;
            _pointPosX = Input.mousePosition.x - _startPosWindow.x;
            _pointPosY = Input.mousePosition.y - _startPosWindow.y;

            _isBorderRestriction = WorkSpaceSettings.Instance.borderRestriction;
            
            if(_bottomPanel)
                _minPosCamera = Camera.main.ViewportToScreenPoint(new Vector2(0,  _bottomPanel.rect.height/Screen.height));
            else
                _minPosCamera = Camera.main.ViewportToScreenPoint(new Vector2(0, 0));

            _maxPosCamera = Camera.main.ViewportToScreenPoint(new Vector2(1, 1));
            _widthWindow = _rootWindow.localToWorldMatrix[0] * _rootWindow.GetComponent<RectTransform>().rect.width;
            _heightWindow = _rootWindow.localToWorldMatrix[0] * _rootWindow.GetComponent<RectTransform>().rect.height;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _windowActions.SortToTopFocus();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            WorkSpaceSettings.Instance.SetDefaultCursor();
            isDrag = false;
        }
    }
}

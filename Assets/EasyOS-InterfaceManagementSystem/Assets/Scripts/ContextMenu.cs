using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class ContextMenu : MonoBehaviour, IPointerClickHandler
    {
        public GameObject contextPrefab;
        
        private Transform _parentContextPanel;
        private GameObject _contextPanel;
        private RectTransform _bottomPanel;
        private  Vector3 _min, _max;

        private void Start()
        {
            if (WorkSpaceSettings.Instance.bottomPanelController)
                _bottomPanel = WorkSpaceSettings.Instance.bottomPanelController.GetComponent<RectTransform>();
            
            if (WorkSpaceSettings.Instance.mainCanvas)
                _parentContextPanel = WorkSpaceSettings.Instance.mainCanvas;
            
           
        }

        private Vector3 CheckScreenSpawn(Vector2 pos, RectTransform panel)
        {
            _max = Camera.main.ViewportToScreenPoint(new Vector2(1, 1));
            
            if (_bottomPanel)
                _min = Camera.main.ViewportToScreenPoint(new Vector2(0, _bottomPanel.rect.height / Screen.height));
            else
                _min = Camera.main.ViewportToScreenPoint(new Vector2(0, 0));
            
            var widthCanvas = panel.transform.localToWorldMatrix[0] * panel.rect.width;
            var heightCanvas = panel.transform.localToWorldMatrix[0] * panel.rect.height;
            
            Vector3 curPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

            if (Input.mousePosition.x >= (_max.x - widthCanvas) && Input.mousePosition.y < (_max.y - heightCanvas) && Input.mousePosition.y > (_min.y + heightCanvas))
                curPos = new Vector3(_max.x - widthCanvas, pos.y, 0);
            else if (Input.mousePosition.x >= (_max.x - widthCanvas) && Input.mousePosition.y >= (_max.y - heightCanvas))
                curPos = new Vector3(_max.x - widthCanvas, pos.y);
            else if (Input.mousePosition.x >= (_max.x - widthCanvas) && Input.mousePosition.y <= (_min.y + heightCanvas))
                curPos = new Vector3(_max.x - widthCanvas, _min.y + heightCanvas, 0);
            
            if (Input.mousePosition.x <= (_min.x + widthCanvas) && Input.mousePosition.y <= (_min.y + heightCanvas))
                curPos = new Vector3(pos.x, _min.y + heightCanvas, 0);
            
            if (Input.mousePosition.y <= (_min.y + heightCanvas) && Input.mousePosition.x > (_min.x + widthCanvas) && Input.mousePosition.x < (_max.x - widthCanvas))
                curPos = new Vector3(pos.x, _min.y + heightCanvas, 0);

            return curPos;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (contextPrefab != null)
            {
                if (eventData.button == PointerEventData.InputButton.Right)
                {
                   
                    GameObject window = Instantiate(contextPrefab, _parentContextPanel.position, Quaternion.identity, _parentContextPanel);
                    _contextPanel = window;
                    window.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
                    window.transform.localScale = new Vector3(1, 1, 1);

                    if (_contextPanel != null)
                    {
                        Canvas canvas = WorkSpaceSettings.Instance.mainCanvas.GetComponent<Canvas>();
                        if (canvas != null)
                        {
                            RectTransform trans = _contextPanel.GetComponent<RectTransform>();
                            if (trans == null) throw new UnityException("The display panel must have a RectTransform component attached in order to be displayed properly.");

                            Vector2 pos = CanvasMouseFollower.GetPointerPosOnCanvas(canvas, Input.mousePosition);
                            trans.position = CheckScreenSpawn(pos, _contextPanel.GetComponent<RectTransform>());
                        }
                        if (canvas == null) throw new UnityException("The display panel must be the child of a canvas in order to be displayed properly.");
                    }
                    
                }
            }
        }
    }
}

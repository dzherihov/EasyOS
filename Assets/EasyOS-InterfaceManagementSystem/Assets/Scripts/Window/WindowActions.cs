using System.Collections;
using Assets.Scripts.BottomPanel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Window
{
  public class WindowActions : MonoBehaviour, IPointerDownHandler
  {
    [SerializeField] private RectTransform _rootWindow;
    [SerializeField] private RectTransform _drugPanel;
    [SerializeField] private Animator _anim;

    [HideInInspector] public bool isFullScreen;
    [HideInInspector] public bool isCollapse;
    [HideInInspector] public SortLayers parenSorting;
    
    [HideInInspector] public ItemBottom bottomItem;

    private Vector2 _lastPosWindow = Vector2.zero;
    private Vector2 _lastSizeWindow = Vector2.zero;
    private Vector2 _lastSizeDrugPanel = Vector2.zero;
    
    
    private readonly int _closeIndex = Animator.StringToHash("close");
    private readonly int _expandInIndex = Animator.StringToHash("expandIn");
    private readonly int _expandOutIndex = Animator.StringToHash("expandOut");
    private readonly int _collapseIndex = Animator.StringToHash("collapse");

    private void Start()
    {
      if(transform.parent)
        parenSorting = transform.parent.GetComponent<SortLayers>();
    }
    
    private void LastSizePosWindow()
    {
      _lastSizeWindow = _rootWindow.sizeDelta;
      _lastPosWindow = _rootWindow.position;
      _lastSizeDrugPanel = _drugPanel.sizeDelta;
    }

    public void ReduceFromFullScreen(float pointPosX, float pointPosY)
    {
      float perPoint = _drugPanel.sizeDelta.x / pointPosX;
      float newPerPoint = _lastSizeDrugPanel.x / perPoint;
      _rootWindow.anchorMin = new Vector2(0.5f,0.5f);
      _rootWindow.anchorMax = new Vector2(0.5f,0.5f);
      SortToTopFocus();
      _rootWindow.sizeDelta = _lastSizeWindow;
      _rootWindow.position = new Vector2(pointPosX - newPerPoint + _lastSizeWindow.x/2, pointPosY - _lastSizeWindow.y/2 + 12.5f);
      isFullScreen = false;
    } 
    public void SetFullScreen()
    {
      if (!isFullScreen)
      {
        SortToTopFocus();
        LastSizePosWindow();
        _rootWindow.anchorMin = Vector2.zero;
        _rootWindow.anchorMax = Vector2.one;
        _rootWindow.position = Vector2.zero;
        _rootWindow.sizeDelta = Vector2.zero;
        _rootWindow.anchoredPosition = Vector2.zero;
        isFullScreen = true;
        StartCoroutine(PlayAnim(_expandInIndex));
      }
      else
      {
        SetLastSizeWindow();
        isFullScreen = false;
        _rootWindow.anchorMin = new Vector2(0.5f,0.5f);
        _rootWindow.anchorMax = new Vector2(0.5f,0.5f);
        StartCoroutine(PlayAnim(_expandOutIndex));
      }
    }

    public void SortToTopFocus()
    {
      if (parenSorting)
        parenSorting.Sorting(_rootWindow);

      if(bottomItem != null && WorkSpaceSettings.Instance.bottomPanelController)
        WorkSpaceSettings.Instance.bottomPanelController.InFocus(bottomItem);
    }
    
    public bool IsFocused()
    {
      return _rootWindow == parenSorting.GetFocusElem();
    }

    private void SetLastSizeWindow()
    {
      if (_lastSizeWindow != Vector2.zero || _lastPosWindow != Vector2.zero)
      {
        SortToTopFocus();
        _rootWindow.sizeDelta = _lastSizeWindow;
        _rootWindow.position = _lastPosWindow;
      }
    }
    
    public void CloseWindow() {
      if (_anim != null) _anim.SetBool(_closeIndex, true);
      if(bottomItem != null && WorkSpaceSettings.Instance.bottomPanelController)
        WorkSpaceSettings.Instance.bottomPanelController.RemoveItem(bottomItem);
      Destroy(_rootWindow.gameObject, 0.1f);
    }
    
    public void CollapseWindow()
    {
      isCollapse = !isCollapse;
      if (_anim != null) _anim.SetBool(_collapseIndex, isCollapse);
      if(!WorkSpaceSettings.Instance.bottomPanelController) return;
      if(bottomItem != null && isCollapse)
        WorkSpaceSettings.Instance.bottomPanelController.InFocus(new ItemBottom());
      else
        WorkSpaceSettings.Instance.bottomPanelController.InFocus(bottomItem);
    }

    private IEnumerator PlayAnim(int indexParameter)
    {
      if (_anim != null) _anim.SetBool(indexParameter, true);
      yield return new WaitForSeconds(0.1f);
      if (_anim != null) _anim.SetBool(indexParameter, false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SortToTopFocus();
    }
  }
}

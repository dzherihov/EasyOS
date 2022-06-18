using System.Collections.Generic;
using Assets.Scripts.Window;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Folder
{
    public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private LayerMask _layerMaskGrid;
        [SerializeField] private LayerMask _layerMaskFolder;
        [SerializeField] private GetDataFile _dataFile;
        
        private GameObject _cloneDrag;
        private GameObject _cloneDragTemp;
        private bool _isMoving;
        private SetDataWindow _setDataWindowTarget;

        private void Start()
        {
          _isMoving = WorkSpaceSettings.Instance.isMovingDrag;
          _cloneDrag = WorkSpaceSettings.Instance.cloneDraggableFolderPrefab;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
          if(!_isMoving || _cloneDrag == null || eventData.button != PointerEventData.InputButton.Left) return;
          _cloneDragTemp = Instantiate(_cloneDrag, Input.mousePosition, quaternion.identity, WorkSpaceSettings.Instance.mainCanvas);
          _cloneDragTemp.GetComponent<RectTransform>().sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
          _cloneDragTemp.GetComponent<SetDataDraggableFile>().Init(_dataFile.titleFile.text, _dataFile.iconFile.sprite);
          
          
          var results = new List<RaycastResult>();
          EventSystem.current.RaycastAll(eventData, results);
          RaycastResult resFolder = results.Find(r => (_layerMaskFolder & (1 << r.gameObject.layer)) != 0);
          if (resFolder.isValid)
          {
            Transform targetWindow = resFolder.gameObject.transform.parent.GetComponent<GetRootWindow>().windowRoot;
            _setDataWindowTarget = targetWindow.GetComponent<SetDataWindow>();
          }
        }

        public void OnDrag(PointerEventData eventData)
        {
          if(!_isMoving || _cloneDrag  == null || eventData.button != PointerEventData.InputButton.Left) return;
          
          _cloneDragTemp.transform.position = Input.mousePosition;
        }
        
        
        public void OnEndDrag(PointerEventData eventData)
        {
          if(!_isMoving || _cloneDrag  == null || eventData.button != PointerEventData.InputButton.Left) return;
          
          Destroy(_cloneDragTemp);
          
          eventData.position = Input.mousePosition;
          var results = new List<RaycastResult>();
          EventSystem.current.RaycastAll(eventData, results);
          RaycastResult res = results.Find(r => (_layerMaskGrid & (1 << r.gameObject.layer)) != 0);
          RaycastResult resFolder = results.Find(r => (_layerMaskFolder & (1 << r.gameObject.layer)) != 0);
          if (resFolder.isValid)
          {
              Transform targetWindow = resFolder.gameObject.transform.parent.GetComponent<GetRootWindow>().windowRoot;
              _setDataWindowTarget = targetWindow.GetComponent<SetDataWindow>();

              if (!CheckAncestralTree(_setDataWindowTarget.dataFileShortcut.fileShortcut))
              {
                transform.SetParent(resFolder.gameObject.transform);
                if (_setDataWindowTarget.dataFileShortcut)
                {
                  _setDataWindowTarget.dataFileShortcut.RemoveContentList(_dataFile.fileShortcut);
                  _setDataWindowTarget.dataFileShortcut.fileShortcut.contentList.Add(_dataFile.fileShortcut);
                  _dataFile.fileShortcut.parentShortcut = _setDataWindowTarget.dataFileShortcut;
                }
                
              }
          }
          else if (res.isValid)
          {
            if (res.gameObject.transform.childCount <= 0)
            {
              transform.SetParent(res.gameObject.transform);
               _dataFile.fileShortcut.parentShortcut = null;
               if(_setDataWindowTarget)
                  _setDataWindowTarget.dataFileShortcut.RemoveContentList(_dataFile.fileShortcut);
            }
          }
        }


        private bool CheckAncestralTree(FileShortcut targetShortcut)
        {
          FileShortcut selfShortcut = _dataFile.fileShortcut;
          GetDataFile parentTarget = targetShortcut.parentShortcut;

          if (selfShortcut.uniqHash == targetShortcut.uniqHash)
          {
            WorkSpaceSettings.Instance.Alert("warning", "Folder move error", "Can't move the current folder to itself!");
            return true;
          }

          while (parentTarget != null)
          {
            if (selfShortcut.uniqHash == parentTarget.fileShortcut.uniqHash)
            {
              WorkSpaceSettings.Instance.Alert("warning", "Folder move error", "Can't move current folder to child folders!");
              return true;
            }

            parentTarget = parentTarget.fileShortcut.parentShortcut;
          }

          return false;
        }
    }
}

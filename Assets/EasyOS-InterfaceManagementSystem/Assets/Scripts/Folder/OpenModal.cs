using Assets.Scripts.BottomPanel;
using Assets.Scripts.Window;
using UnityEngine;

namespace Assets.Scripts.Folder
{
    public class OpenModal : MonoBehaviour
    {
        [SerializeField] private GetDataFile _dataFile;

        private GameObject _modal;
        private WorkSpaceSettings _workSpaceSettings;
        private Transform _rootWindows;

        private void Start()
        {
            _workSpaceSettings = WorkSpaceSettings.Instance;
            _rootWindows = _workSpaceSettings.rootWindows;
            _modal = _workSpaceSettings.windowPrefab;
        }

        public void Open()
        {
            if (_modal == null) return;

            ItemBottom item = _workSpaceSettings.bottomPanelController.GetOpenedItem(_dataFile);
            if (item != null && _workSpaceSettings.openInOneInstance)
            {
                var windowActionsItem = item.windowPanel.GetComponent<WindowActions>();
                windowActionsItem.SortToTopFocus();
                if (windowActionsItem.isCollapse)
                    windowActionsItem.CollapseWindow();
            }
            else
            {
                Transform modal = Instantiate(_modal.transform, _rootWindows.position, Quaternion.identity,_rootWindows);
                // modal.SetParent(_rootWindows);
                modal.GetComponent<SetDataWindow>().Init(_workSpaceSettings.openedSizeWindowValue, _dataFile);
                CheckCountModals();
                _workSpaceSettings.bottomPanelController.AddItem(modal, _dataFile);
                modal.GetComponent<WindowActions>().SortToTopFocus();
                _dataFile.windowPanel = modal.transform;
                modal.GetComponent<RectTransform>().localPosition = Vector3.zero;
                // modal.localScale = Vector3.one;
            }

        }

        private void CheckCountModals()
        {
            // if (!_workSpaceSettings.bottomPanelController) return;
            // if (_workSpaceSettings.bottomPanelController.itemsList.Count >= _workSpaceSettings.maxWindowsOnScreen)
            // {
            //     _workSpaceSettings.bottomPanelController.itemsList[0].windowPanel.GetComponent<WindowActions>().CloseWindow();
            // }
            
            if (_workSpaceSettings.rootWindows.childCount > _workSpaceSettings.maxWindowsOnScreen)
            {
                _workSpaceSettings.rootWindows.GetChild(0).GetComponent<WindowActions>().CloseWindow();
            }
        }
    }
}

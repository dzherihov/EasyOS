using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.ContextMenu
{
    public class ContextMenuPanel : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        private List<ContextMenuItem> _contextMenuItems;

        private void Start()
        {
            if (WorkSpaceSettings.Instance.contextMenuItems.Count > 0)
            {
                _contextMenuItems = WorkSpaceSettings.Instance.contextMenuItems;
                SpawnContextItems();
            }

            GetComponent<Selectable>().Select();
        }

        private void SpawnContextItems()
        {
            foreach (var item in _contextMenuItems)
            {
                var itemObj = Instantiate(item.prefab, Vector3.one, Quaternion.identity, gameObject.transform);

                var setValue = itemObj.GetComponent<SetContextItem>();
                if (item.name != null && setValue)
                    setValue.Init(item.name, item.icon);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
         
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Destroy(gameObject);
        }
    }
}

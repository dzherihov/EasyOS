using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    public class ContextMenuPanel : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        private void Start()
        {
           GetComponent<Selectable>().Select();
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

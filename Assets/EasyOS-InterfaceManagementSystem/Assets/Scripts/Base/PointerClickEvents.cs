using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Base
{
    [Serializable] public class ClickEvents : UnityEvent{ }
    public class PointerClickEvents : MonoBehaviour, IPointerClickHandler
    {
       
        public ClickEvents SingleClickEvent;
        public ClickEvents DoubleClickEvent;
        public ClickEvents MultiClickEvent;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                int clickCount = eventData.clickCount;

                switch (clickCount)
                {
                    case 1:
                        OnSingleClick();
                        break;
                    case 2:
                        OnDoubleClick();
                        break;
                    default:
                    {
                        OnMultiClick();
                        break;
                    }
                }
            }
        }
        
        private void OnSingleClick()
        {
            SingleClickEvent.Invoke();
        }

        private void OnDoubleClick()
        {
            DoubleClickEvent.Invoke();
        }

        private void OnMultiClick()
        {
            MultiClickEvent.Invoke();
        }
    }
}

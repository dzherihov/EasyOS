using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Alert
{
    public class SetDataAlert : MonoBehaviour
    {
        [SerializeField] private RectTransform _alertRoot;
        [SerializeField] private AlertActions _alertActions;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _contentText;
        [SerializeField] private Image _icon;
        [SerializeField] private Transform _buttonsRoot;

        public void Init(AlertBuilder alertData, string title, string content, Action returnedPositive = null, Action returnedNegative = null)
        {
            _title.text = title;
            _contentText.text = content;
            _icon.sprite = alertData.icon;

            foreach (ButtonAlert button in alertData.buttons)
            {
                GameObject btnInst = Instantiate(button.buttonPrefab, _buttonsRoot.position, quaternion.identity, _buttonsRoot);
                Button btn = btnInst.GetComponent<Button>();
                
                switch (button.returnedButtonAlert)
                {
                    case ReturnedButtonAlert.Negative:
                        btn.onClick.AddListener(_alertActions.ReturnedButton(returnedNegative));
                        break;
                    case ReturnedButtonAlert.Positive:
                        btn.onClick.AddListener(_alertActions.ReturnedButton(returnedPositive));
                        break;
                    case ReturnedButtonAlert.None:
                        btn.onClick.AddListener(_alertActions.ReturnedButton(null));
                        break;
                }
            }
            _buttonsRoot.GetComponent<HorizontalLayoutGroup>().childAlignment = alertData.alignmentButtons;
            
            _alertRoot.sizeDelta = alertData.openedSizeAlert;
        }
 
    }
}

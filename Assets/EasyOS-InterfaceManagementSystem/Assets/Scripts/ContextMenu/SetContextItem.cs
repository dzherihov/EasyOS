using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ContextMenu
{
    public class SetContextItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private Image icon;

        public void Init(string title, Sprite icon)
        {
            this.title.text = title;
            if (icon)
                this.icon.sprite = icon;
            else
            {
                this.icon.gameObject.SetActive(false);
            }


        }
    }
}

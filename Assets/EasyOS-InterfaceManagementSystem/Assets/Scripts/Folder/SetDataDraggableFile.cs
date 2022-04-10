using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Folder
{
    public class SetDataDraggableFile : MonoBehaviour
    {
        [SerializeField] private Image _iconFile;
        [SerializeField] private TMP_Text _titleFile;
        
        public void Init(string title, Sprite iconFile)
        {
            _titleFile.text = title;
            _iconFile.sprite = iconFile;
        }
    }
}

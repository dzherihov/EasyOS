using Assets.Scripts.Window;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.BottomPanel
{
    public class BottomItemAction : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        
        [SerializeField] private WindowActions _windowActions;
        [SerializeField] private Animator _anim;
        private readonly int _focusedIndex = Animator.StringToHash("Focused");
        private readonly int _normalIndex = Animator.StringToHash("Normal");

        public void Init(ItemBottom item)
        {
            iconImage.sprite = item.spriteIcon;
            _windowActions = item.windowPanel.GetComponent<WindowActions>();
            _windowActions.bottomItem = item;
        }

        public void InFocus()
        {
            _anim.SetBool(_focusedIndex, true);
        }
        public void OutFocus()
        {
            _anim.SetBool(_focusedIndex, false);
        }

        public void Collapse()
        {
            if (!_windowActions) return;
            
            if(_windowActions.IsFocused())
                _windowActions.CollapseWindow();
            else if(!_windowActions.isCollapse)
                _windowActions.SortToTopFocus();
            else
            {
                _windowActions.CollapseWindow();
                _windowActions.SortToTopFocus();
            }
        }
    }
}

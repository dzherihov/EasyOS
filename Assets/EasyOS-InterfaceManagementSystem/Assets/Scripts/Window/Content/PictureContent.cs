using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Window.Content
{
   public class PictureContent : MonoBehaviour
   {
      [SerializeField] private Image _pictureImage;
      [SerializeField] private Sprite _picture;
   
      private void Start()
      {
         Init(_picture);
      }

      public void Init(Sprite picture)
      {
         _pictureImage.sprite = picture;
      }
   }
}

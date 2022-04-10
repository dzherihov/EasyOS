using UnityEngine;

namespace Assets.Scripts.Window
{
    public class SortLayers : MonoBehaviour
    {
      public void Sorting(Transform window)
      {
        window.transform.SetSiblingIndex(transform.childCount);
      }

      public Transform GetFocusElem()
      {
        return transform.GetChild(transform.childCount-1);
      }
      
    }
}

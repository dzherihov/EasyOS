using System.Collections.Generic;
using Assets.Scripts.Folder;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Window.Content
{
   public class FolderContent : MonoBehaviour
   {
      public LayoutElement layoutElement;

      private RectTransform _parentContent;
      public void InitContent(List<FileShortcut> contentList)
      {
         foreach (var item in contentList)
         {
            GameObject obj =  Instantiate(WorkSpaceSettings.Instance.fileShortcutPrefab, transform.position, Quaternion.identity, transform);
        
            obj.GetComponent<GetDataFile>().Init(item);
            obj.transform.name = item.title;
         }
      }

      private void Start()
      {
         _parentContent = transform.parent.parent.GetComponent<RectTransform>();
      }

      private void Update()
      {
         layoutElement.minHeight = _parentContent.rect.height-40;
      }
   }
}

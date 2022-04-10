using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Folder
{
    public class GetDataFile : MonoBehaviour
    {
        public FileShortcut fileShortcut;
        public Image iconFile;
        public TMP_Text titleFile;
        [HideInInspector] public Transform windowPanel;
        
        
        public void Init(FileShortcut fileShortcutDate)
        {
            fileShortcut = fileShortcutDate;
            if (fileShortcut.icon) 
                iconFile.sprite = fileShortcut.icon;

            if(fileShortcut.name != null)
                titleFile.text = fileShortcut.name;

            fileShortcut.uniqHash = Guid.NewGuid().ToString();
        }

        public void RemoveContentList(FileShortcut item)
        {
            FileShortcut searchItem = fileShortcut.contentList.Find(x => x.uniqHash == item.uniqHash);
            if (searchItem != null)
            {
                fileShortcut.contentList.Remove(searchItem);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Assets.Scripts.Folder;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.BottomPanel
{

    [Serializable]
    public class ItemBottom
    {
        public Transform windowPanel;
        public Transform itemElem;
        public Sprite spriteIcon;
        public GetDataFile dataFile;
    }
    public class BottomPanelController : MonoBehaviour
    {
        public List<ItemBottom> itemsList;
        [SerializeField] private Transform _itemPrefab;
        
        public void AddItem(Transform window, GetDataFile dataFolder)
        {
            Transform elem = Instantiate(_itemPrefab, transform.position, quaternion.identity, transform);
            
            var item = new ItemBottom()
            {
                windowPanel = window,
                spriteIcon = dataFolder.fileShortcut.icon,
                itemElem = elem,
                dataFile = dataFolder
            };
            itemsList.Add(item);
            
            elem.GetComponent<BottomItemAction>().Init(item);
            
        }

        public ItemBottom GetOpenedItem(GetDataFile dataFolder)
        {
            return itemsList.Find(x => x.dataFile == dataFolder);
        }

        public void RemoveItem(ItemBottom item)
        {
            if(item.itemElem != null)
                Destroy(item.itemElem.gameObject);
            if(itemsList.Contains(item))
                itemsList.Remove(item);
        }
        
        public void InFocus(ItemBottom item)
        {
            foreach (ItemBottom elem in itemsList)
            {
                if (elem == item)
                {
                    elem.itemElem.GetComponent<BottomItemAction>().InFocus();
                }
                else
                {
                    elem.itemElem.GetComponent<BottomItemAction>().OutFocus(); 
                }
                
            }
        }
    }
}

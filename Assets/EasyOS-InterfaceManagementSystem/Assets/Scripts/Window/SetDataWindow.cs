using Assets.Scripts.Folder;
using Assets.Scripts.Window.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Window
{
  public class SetDataWindow : MonoBehaviour
  {
    [SerializeField] private TMP_Text _title;
    [SerializeField] private Image _icon;
    [SerializeField] private RectTransform _windowRootRect;
    [SerializeField] private Transform _contentRootRect;
    public GetDataFile dataFileShortcut;
    
    public void Init(Vector2 sizeDelta, GetDataFile dataFile)
    {
      
      _title.text = dataFile.titleFile.text;
      _windowRootRect.sizeDelta = sizeDelta;
      _icon.sprite = dataFile.iconFile.sprite;

      if (dataFile)
        dataFileShortcut = dataFile;

      if (dataFile.fileShortcut.contentPrefab)
      {
        GameObject content = Instantiate(dataFile.fileShortcut.contentPrefab, _contentRootRect.position, Quaternion.identity, _contentRootRect);
        if (dataFile.fileShortcut.contentList.Count > 0)
        {
          content.GetComponent<FolderContent>().InitContent(dataFile.fileShortcut.contentList);
        }
      }

      if (dataFile.fileShortcut.scrollingContent)
        _contentRootRect.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
      else
      {
        _contentRootRect.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        var rect = _contentRootRect.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.position = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
      }
      
    }
    
  }
}

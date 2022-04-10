using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    
    [Serializable]
    public class Toolbar
    {
        public Texture icon;
        public string title;
    }

    [CreateAssetMenu(fileName = "ConfigEditor", menuName = "ScriptableObjects/ConfigEditor", order = 1)]
    public class ConfigEditor : ScriptableObject
    {
        public GUISkin guiSkin;

        // public Texture[] iconsToolbar;
        public List<Toolbar> iconsToolbar;
    }
}

using System;
using UnityEngine;

namespace UI.MainMenu.LevelScroll
{
    [Serializable]
    public class LevelItemModel
    {
        public Sprite levelIcon;
        public string levelName;
        public int progressPercent;
    }
}
using System;
using UnityEngine;

namespace SaveSystem.DataObjects.Progress
{
    [Serializable]
    public class ProgressLevelSaveData
    {
        [SerializeField] public string levelName;
        [SerializeField] public int progress;

        public ProgressLevelSaveData(string levelName, int progress)
        {
            this.levelName = levelName;
            this.progress = progress;
        }
    }
}
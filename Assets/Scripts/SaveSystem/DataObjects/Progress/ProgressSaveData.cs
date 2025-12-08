using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem.DataObjects.Progress
{
    [Serializable]
    public class ProgressSaveData
    {
        [SerializeField] public int version;
        [SerializeField] public List<ProgressLevelSaveData> progressLevelsSaveData = new();
    }
}
using System;
using System.Collections.Generic;

namespace SaveSystem.DataObjects.Level.New
{
    [Serializable]
    public class LevelSaveData
    {
        public List<DetailSaveData> details = new();
    }
}
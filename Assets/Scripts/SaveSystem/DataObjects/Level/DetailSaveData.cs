using System;
using System.Collections.Generic;

namespace SaveSystem.DataObjects.Level.New
{
    [Serializable]
    public class DetailSaveData
    {
        public string id;
        public int currentCount;
        public List<PointSaveData> points = new();
    }
}
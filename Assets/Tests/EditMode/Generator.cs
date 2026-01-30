using System.Collections.Generic;
using _1_LEVEL_REWORK.New.Data;
using SaveSystem.DataObjects.Level.New;
using UnityEngine;

namespace Tests.EditMode
{
    public static class Generator
    {
        public static LevelSaveData CreateLevelSaveData(List<DetailSaveData> detailSaveData = null)
        {
            var levelSaveData = new LevelSaveData();
            if (detailSaveData != null)
                levelSaveData.details = detailSaveData;
            return levelSaveData;
        }

        public static DetailSaveData CreateDetailSaveData(string id = "1234", int count = 1,
            List<PointSaveData> points = null)
        {
            var detailSaveData = new DetailSaveData
            {
                id = id,
                currentCount = count
            };
            if (points != null)
                detailSaveData.points = points;
            return detailSaveData;
        }

        public static PointSaveData CreatePointSaveData(bool isInstalled = false)
        {
            var pointSaveData = new PointSaveData
            {
                isInstalled = isInstalled
            };
            return pointSaveData;
        }

        public static LevelData CreateLevelData(DetailData ground = null, List<DetailData> details = null)
        {
            var levelData = ScriptableObject.CreateInstance<LevelData>();
            if (ground != null)
                Utils.SetPrivateField(levelData, "ground", ground);
            if (details != null)
                Utils.SetPrivateField(levelData, "details", details);
            return levelData;
        }

        public static DetailData CreateDetailData(string name = "test_detail", int count = 1,
            List<PointData> points = null)
        {
            var detailData = ScriptableObject.CreateInstance<DetailData>();
            detailData.name = name;
            detailData.Count = count;
            if (points != null)
                detailData.points = points;
            return detailData;
        }

        public static PointData CreatePointData(List<ParentConstraint> constraints = null)
        {
            var pointData = new PointData();
            if (constraints != null) pointData.constraints = constraints;

            return pointData;
        }

        public static ParentConstraint CreateParentConstraint(DetailData parent, List<int> indexes)
        {
            var parentConstraint = new ParentConstraint
            {
                ParentDetail = parent
            };
            indexes.ForEach(index => parentConstraint.AddIndex(index));
            return parentConstraint;
        }
    }
}
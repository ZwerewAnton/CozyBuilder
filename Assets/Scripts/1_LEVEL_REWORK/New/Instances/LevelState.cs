using System.Collections.Generic;
using System.Linq;
using _1_LEVEL_REWORK.New.Data;
using SaveSystem.DataObjects.Level.New;

namespace _1_LEVEL_REWORK.New.Instances
{
    public class LevelState
    {
        private readonly Dictionary<string, DetailInstance> _detailInstances = new();
        private readonly DependencyGraph _dependencyGraph = new();
        
        public IReadOnlyDictionary<string, DetailInstance> Details => _detailInstances;
        public DependencyGraph DependencyGraph => _dependencyGraph;

        public void CreateDetailsInstances(DetailData ground, List<DetailData> details, List<DetailSaveData> detailsSaveData)
        {
            var saveDataDict = detailsSaveData.ToDictionary(data => data.id, data => data);

            var groundInstance = new DetailInstance(ground, true);
            groundInstance.Points.Add(ground.points.Count < 1
                ? new PointInstance(new PointData(), true)
                : new PointInstance(ground.points[0], true));
            groundInstance.RemainingCount = 0;
            _detailInstances[ground.Id] = groundInstance;
            
            foreach (var detailData in details)
            {
                var instance = new DetailInstance(detailData);
                _detailInstances[detailData.Id] = instance;

                if (saveDataDict.TryGetValue(detailData.Id, out var saveData))
                {
                    instance.RemainingCount = saveData.currentCount;
                }

                for (var i = 0; i < detailData.points.Count; i++)
                {
                    var isInstalled = false;
                    if (saveData != null && i < saveData.points.Count)
                    {
                        isInstalled = saveData.points[i].isInstalled;
                    }
                    var pointData = detailData.points[i];
                    instance.Points.Add(new PointInstance(pointData, isInstalled));
                }
            }

            CreateDependencyGraph(details);
        }

        public List<DetailInstance> GetInstalledDetails()
        {
            var details = new List<DetailInstance>();
            foreach (var detailInstance in _detailInstances)
            {
                if (detailInstance.Value.IsAnyInstalled())
                {
                    details.Add(detailInstance.Value);
                }
            }

            return details;
        }

        public bool TryInstallDetail(string detailId, int pointIndex)
        {
            var detailInstance = _detailInstances[detailId];
            if (detailInstance == null)
            {
                return false;
            }
            
            var pointInstance = detailInstance.Points[pointIndex];
            var isReady = _dependencyGraph.IsReady(pointInstance);
            
            return isReady && detailInstance.TryInstall(pointIndex);
        }

        public bool IsPointReady(PointInstance point)
        {
            return _dependencyGraph.IsReady(point);
        }

        private void CreateDependencyGraph(List<DetailData> details)
        {
            foreach (var detailData in details)
            {
                var detailInstance = _detailInstances[detailData.Id];
                for (var i = 0; i < detailData.points.Count; i++)
                {
                    var pointData = detailData.points[i];
                    var pointInstance = detailInstance.Points[i];

                    foreach (var parent in pointData.constraints)
                    {
                        var parentDetail = _detailInstances[parent.ParentDetail.Id];
                        
                        foreach (var index in parent.ParentPointIndexes)
                        {
                            var parentPoint = parentDetail.Points[index];
                            _dependencyGraph.AddDependency(pointInstance, parentPoint);
                        }
                    }
                }
            }
        }
    }
}
using System.Collections.Generic;
using Gameplay.Instances;

namespace _1_LEVEL_REWORK.New.Instances
{
    public class DependencyGraph
    {
        private readonly Dictionary<PointInstance, List<PointInstance>> _dependencies = new();

        public void AddDependency(PointInstance target, PointInstance parent)
        {
            if (!_dependencies.ContainsKey(target))
            {
                _dependencies[target] = new List<PointInstance>();
            }

            _dependencies[target].Add(parent);
        }

        public bool IsReady(PointInstance point)
        {
            if (!_dependencies.TryGetValue(point, out var parents))
            {
                return true;
            }

            foreach (var parent in parents)
            {
                if (!parent.IsInstalled)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
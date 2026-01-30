using System.Collections.Generic;
using System.Linq;
using _1_LEVEL_REWORK.New.Data;
using Gameplay.Instances;
using UnityEngine;

namespace _1_LEVEL_REWORK.New.Instances
{
    public class DetailInstance
    {
        private readonly DetailData _data;

        public DetailInstance(DetailData data, bool isGround = false)
        {
            _data = data;
            RemainingCount = data.Count;
            Points = new List<PointInstance>();
            IsGround = isGround;
        }

        public bool IsGround { get; }

        public List<PointInstance> Points { get; }
        public int RemainingCount { get; set; }

        public Sprite GetDetailIcon()
        {
            return _data.Icon;
        }

        public DetailPrefab GetDetailPrefab()
        {
            return _data.Prefab;
        }

        public Mesh GetDetailMesh()
        {
            return _data.Mesh;
        }

        public Material GetDetailMaterial()
        {
            return _data.Material;
        }

        public bool TryInstall(int pointIndex)
        {
            if (RemainingCount <= 0 || pointIndex < 0 || pointIndex >= Points.Count) return false;

            RemainingCount--;
            Points[pointIndex].Install();
            return true;
        }

        public bool IsAnyInstalled()
        {
            return Points.Any(instance => instance.IsInstalled);
        }

        public bool IsAllInstalled()
        {
            return Points.All(instance => instance.IsInstalled);
        }

        public void Reset()
        {
            RemainingCount = _data.Count;
            foreach (var p in Points) p.Uninstall();
        }
    }
}
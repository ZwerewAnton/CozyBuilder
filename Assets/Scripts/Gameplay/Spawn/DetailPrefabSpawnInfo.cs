using _1_LEVEL_REWORK.New.Instances;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Gameplay.Spawn
{
    public struct DetailPrefabSpawnInfo
    {
        public readonly DetailPrefab Prefab;
        public Vector3 Position;
        public Quaternion Rotation;

        public DetailPrefabSpawnInfo(DetailPrefab prefab, Vector3 position, Quaternion rotation)
        {
            Prefab = prefab;
            Position = position;
            Rotation = rotation;
        }
    }
}
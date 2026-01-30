using UnityEngine;

namespace Gameplay.Movement
{
    public struct PointTransform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public int PointIndex;

        public PointTransform(Vector3 position, Quaternion rotation, int pointIndex)
        {
            Position = position;
            Rotation = rotation;
            PointIndex = pointIndex;
        }
    }
}
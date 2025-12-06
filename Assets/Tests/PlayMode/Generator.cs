using Gameplay.Movement;
using UnityEngine;

namespace Tests.PlayMode
{
    public static class Generator
    {
        public static PointTransform CreatePointTransform(Vector3 position, Quaternion rotation, int pointIndex)
        {
            var pointTransform = new PointTransform
            {
                Position = position,
                Rotation = rotation,
                PointIndex = pointIndex
            };
            return pointTransform;
        }
    }
}
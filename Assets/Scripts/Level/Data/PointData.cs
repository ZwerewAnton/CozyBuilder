using System;
using System.Collections.Generic;
using UnityEngine;

namespace _1_LEVEL_REWORK.New.Data
{
    [Serializable]
    public class PointData
    {
        public Vector3 position;
        public Vector3 rotation;

        public List<ParentConstraint> constraints = new();
    }
}
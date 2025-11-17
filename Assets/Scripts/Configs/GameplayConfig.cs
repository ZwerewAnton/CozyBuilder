using System;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class GameplayConfig
    {
        [Header("Detail View Moving")]
        public float magnetDistance = 0.5f;
        public float ghostDistance = 20f;
        public Vector2 screenOffset = new (0, 100);
    }
}
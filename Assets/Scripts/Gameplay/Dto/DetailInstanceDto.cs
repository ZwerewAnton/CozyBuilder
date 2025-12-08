using System.Collections.Generic;
using _1_LEVEL_REWORK.New.Instances;
using UnityEngine;

namespace Gameplay.Dto
{
    public class DetailInstanceDto
    {
        public int CurrentCount;
        public bool IsGround;
        public DetailPrefab Prefab;
        public Mesh Mesh;
        public Material Material;
        public Sprite Icon;
        public List<PointInstanceDto> Points = new();
    }
}
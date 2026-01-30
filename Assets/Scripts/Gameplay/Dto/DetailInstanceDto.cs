using System.Collections.Generic;
using _1_LEVEL_REWORK.New.Instances;
using UnityEngine;

namespace Gameplay.Dto
{
    public class DetailInstanceDto
    {
        public int CurrentCount;
        public Sprite Icon;
        public bool IsGround;
        public Material Material;
        public Mesh Mesh;
        public List<PointInstanceDto> Points = new();
        public DetailPrefab Prefab;
    }
}
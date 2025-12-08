using _1_LEVEL_REWORK.New.Instances;
using UnityEngine;

namespace Gameplay.Movement
{
    public class GhostDetailView : DetailView
    {
        public void SetMaterialColor(Color color)
        {
            MeshRenderer.material.color = color;
        }
    }
}
using _1_LEVEL_REWORK.New.Instances;
using UnityEngine;

namespace Gameplay.Movement
{
    public class MovingDetailView : DetailView
    {
        public void Show(Mesh mesh, Material material)
        {
            MeshRenderer.material = material;
            Show(mesh);
        }
    }
}
using UnityEngine;

namespace Cameras
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private Camera sceneCamera;

        public Vector3 ScreenToWorldPoint(Vector2 position, float depth)
        {
            var depthPosition = new Vector3(position.x, position.y, depth);
            return sceneCamera.ScreenToWorldPoint(depthPosition);
        }

        public Vector3 WorldToScreenPoint(Vector3 position)
        {
            return sceneCamera.WorldToScreenPoint(position);
        }
    }
}
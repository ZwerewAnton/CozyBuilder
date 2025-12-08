using Cameras.Movement;
using UnityEngine;
using Zenject;

namespace Cameras
{
    public class OrbitCameraController : MonoBehaviour
    {
        private OrbitCameraMovement _cameraMovement;
        private Transform _cameraTransform;

        [Inject]
        private void Construct(OrbitCameraMovement cameraMovement, CameraHandler cameraHandler)
        {
            _cameraMovement = cameraMovement;
            _cameraTransform = cameraHandler.transform;
        }

        private void LateUpdate()
        {
            UpdateCameraMovement();
        }

        private void UpdateCameraMovement()
        {
            var result = _cameraMovement.CalculateMovement();

            _cameraTransform.rotation = result.Rotation;
            _cameraTransform.position = result.Position;
        }
    }
}
using Cameras.Movement;
using UnityEngine;
using Zenject;

namespace Cameras
{
    public class OrbitCameraController : MonoBehaviour
    {
        private OrbitCameraMovement _cameraMovement;
        private Transform _cameraTransform;

        private void LateUpdate()
        {
            UpdateCameraMovement();
        }

        [Inject]
        private void Construct(OrbitCameraMovement cameraMovement, CameraHandler cameraHandler)
        {
            _cameraMovement = cameraMovement;
            _cameraTransform = cameraHandler.transform;
        }

        private void UpdateCameraMovement()
        {
            var result = _cameraMovement.CalculateMovement();

            _cameraTransform.rotation = result.Rotation;
            _cameraTransform.position = result.Position;
        }
    }
}
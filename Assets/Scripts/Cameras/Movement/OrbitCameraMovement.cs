using Cameras.Input;
using Configs;
using UnityEngine;
using Utils;
using Zenject;

namespace Cameras.Movement
{
    public class OrbitCameraMovement
    {
        private readonly CameraConfigs _configs;
        private readonly ICameraInputProvider _input;

        private Vector2 _eulerAngles;
        private Vector2 _smoothedDelta;
        private float _desiredDistance;
        private float _currentDistance;
        
        private float _currentHeightOffset;
        private const float MinHeight = -5f;
        private const float MaxHeight = 5f;
        private const float HeightSmooth = 5f;

        [Inject]
        private OrbitCameraMovement(ApplicationConfigs configs, ICameraInputProvider inputProvider)
        {
            _configs = configs.camera;
            _input = inputProvider;
            InitializeParameters();
        }

        public CameraMovementResult CalculateMovement()
        {
            _input.UpdateInput();

            var delta = _input.IsRotationAllowed ? _input.RotationDelta : Vector2.zero;

            _smoothedDelta = Vector2.Lerp(_smoothedDelta, delta, Time.deltaTime * _configs.mouseSmooth);

            _eulerAngles.y += _smoothedDelta.x * _configs.xSpeed * Time.deltaTime;
            _eulerAngles.x -= _smoothedDelta.y * _configs.ySpeed * Time.deltaTime;
            _eulerAngles.x = MathUtils.ClampAngle(_eulerAngles.x, _configs.yMinLimit, _configs.yMaxLimit);

            var heightDelta = _input.HeightDelta;
            if (Mathf.Abs(heightDelta) > 0.0001f)
            {
                _currentHeightOffset += heightDelta;
                _currentHeightOffset = Mathf.Clamp(_currentHeightOffset, MinHeight, MaxHeight);
            }
            
            var zoomDelta = _input.ZoomDelta;
            if (Mathf.Abs(zoomDelta) > 0.0001f)
            {
                var maxDistance = _configs.maxDistance;
                var minDistance = _configs.minDistance;
                _desiredDistance -= zoomDelta * (maxDistance - minDistance) * _configs.zoomSpeed;
                _desiredDistance = Mathf.Clamp(_desiredDistance, minDistance, maxDistance);
            }

            _currentDistance = Mathf.Lerp(_currentDistance, _desiredDistance, Time.deltaTime * _configs.zoomDampening);

            var rotation = Quaternion.Euler(_eulerAngles.x, _eulerAngles.y, 0);
            var negativeDistance = new Vector3(0, 0, -_currentDistance);
            
            var offset = _configs.targetOffset;
            offset.y += _currentHeightOffset;
            
            var position = rotation * negativeDistance + offset;

            return new CameraMovementResult(position, rotation);
        }

        private void InitializeParameters()
        {
            _eulerAngles = _configs.startRotation.eulerAngles;
            _desiredDistance = _currentDistance = _configs.distance;
        }
    }
}
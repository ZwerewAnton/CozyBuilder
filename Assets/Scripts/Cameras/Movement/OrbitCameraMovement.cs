using System;
using Cameras.Input;
using Configs;
using UnityEngine;
using Utils;
using Zenject;

namespace Cameras.Movement
{
    public class OrbitCameraMovement
    {
        public event Action CameraMoved;
        private readonly CameraConfigs _configs;
        private readonly ICameraInputProvider _input;

        private Vector2 _eulerAngles;
        private Vector2 _smoothedDelta;
        private float _currentDistance;
        private float _currentZoomOffset;
        private float _currentHeightOffset;
        private Vector3 _previousCalculatePosition;
        
        public float DesiredZoomOffset { get; private set; }
        public float DesiredHeightOffset { get; private set; }

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
                DesiredHeightOffset += heightDelta;
                DesiredHeightOffset = Mathf.Clamp(DesiredHeightOffset, _configs.minHeight, _configs.maxHeight);
            }

            _currentHeightOffset = Mathf.Lerp(
                _currentHeightOffset,
                DesiredHeightOffset,
                Time.deltaTime * _configs.heightSmooth
            );
            var offset = _configs.targetOffset;
            offset.y += _currentHeightOffset;
            
            var zoomDelta = _input.ZoomDelta;
            if (Mathf.Abs(zoomDelta) > 0.0001f)
            {
                var maxDistance = _configs.maxDistance;
                var minDistance = _configs.minDistance;

                DesiredZoomOffset -= zoomDelta * (maxDistance - minDistance) * _configs.zoomSpeed;
                DesiredZoomOffset = Mathf.Clamp(
                    DesiredZoomOffset,
                    minDistance - _configs.distance,
                    maxDistance - _configs.distance
                );
            }

            _currentZoomOffset = Mathf.Lerp(
                _currentZoomOffset, DesiredZoomOffset, 
                Time.deltaTime * _configs.zoomSmooth
            );
            _currentDistance = _configs.distance + _currentZoomOffset;

            var rotation = Quaternion.Euler(_eulerAngles.x, _eulerAngles.y, 0);
            var negativeDistance = new Vector3(0, 0, -_currentDistance);
            var position = rotation * negativeDistance + offset;
            
            if (Vector3.Distance(position, _previousCalculatePosition) > Mathf.Epsilon)
                CameraMoved?.Invoke();
            _previousCalculatePosition = position;
            
            return new CameraMovementResult(position, rotation);
        }

        public void ResetCamera()
        {
            DesiredZoomOffset = 0f;
            DesiredHeightOffset = 0f;
        }

        private void InitializeParameters()
        {
            _eulerAngles = _configs.startRotation.eulerAngles;
            _currentDistance = _configs.distance;
            _currentZoomOffset = DesiredZoomOffset = 0f;
            _currentHeightOffset = DesiredHeightOffset = 0f;
        }
    }
}
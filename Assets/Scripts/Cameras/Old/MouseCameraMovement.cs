using Cameras.Movement;
using Configs;
using Input;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Zenject;

namespace Cameras
{
    public class MouseCameraMovement : ICameraMovement
    {
        private const float ScrollCoefficient = 0.001f;
        
        private readonly CameraConfigs _configs;
        private InputActions.CameraActions _cameraActions;
        
        private Vector2 _eulerAngles;
        private Vector2 _smoothedDelta;
        private Vector2 _rawDelta;

        private float _desiredDistance;
        private float _currentDistance;

        private bool _rotateAllowed;

        [Inject]
        public MouseCameraMovement(ApplicationConfigs configs, InputHandler inputHandler)
        {
            _configs = configs.camera;
            _cameraActions = inputHandler.CameraActions;
            InitializeParameters();
        }

        public CameraMovementResult CalculateMovement()
        {
            if (_cameraActions.Press.WasPressedThisFrame())
            {
                _rotateAllowed = !EventSystem.current.IsPointerOverGameObject();
            }
            
            if (_cameraActions.Press.IsPressed() && _rotateAllowed)
            {
                _rawDelta = _cameraActions.Look.ReadValue<Vector2>();
            }
            else
            {
                _rawDelta = Vector2.zero;
            }
            
            _smoothedDelta = Vector2.Lerp(_smoothedDelta, _rawDelta, Time.deltaTime * _configs.mouseSmooth);

            _eulerAngles.y += _smoothedDelta.x * _configs.xSpeed * Time.deltaTime;
            _eulerAngles.x -= _smoothedDelta.y * _configs.ySpeed * Time.deltaTime;
            _eulerAngles.x = MathUtils.ClampAngle(_eulerAngles.x, _configs.yMinLimit, _configs.yMaxLimit);

            var maxDistance = _configs.maxDistance;
            var minDistance = _configs.minDistance;
            var scroll = _cameraActions.Zoom.ReadValue<float>();
            _desiredDistance -= scroll * (maxDistance - minDistance) * _configs.zoomSpeed * ScrollCoefficient;
            _desiredDistance = Mathf.Clamp(_desiredDistance, minDistance, maxDistance);

            _currentDistance = Mathf.Lerp(_currentDistance, _desiredDistance, Time.deltaTime * _configs.zoomSmooth);

            var rotation = Quaternion.Euler(_eulerAngles.x, _eulerAngles.y, 0);
            var negativeDistance = new Vector3(0.0f, 0.0f, -_currentDistance);
            var position = rotation * negativeDistance + _configs.targetOffset;
            return new CameraMovementResult(position, rotation);
        }

        private void InitializeParameters()
        {
            _eulerAngles = _configs.startRotation.eulerAngles;
            _desiredDistance = _currentDistance = _configs.distance;
        }
    }
}
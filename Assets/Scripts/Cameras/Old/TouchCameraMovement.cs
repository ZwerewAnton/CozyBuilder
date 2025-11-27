using Cameras.Movement;
using Configs;
using Input;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Zenject;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Cameras
{
    public class TouchCameraMovement : ICameraMovement
    {
        private readonly CameraConfigs _configs;
        private readonly InputActions.CameraActions _cameraActions;

        private Vector2 _eulerAngles;
        private Vector2 _smoothedDelta;
        private Vector2 _rawDelta;

        private float _desiredDistance;
        private float _currentDistance;

        private bool _rotateAllowed;
        private float _previousPinchDistance;

        [Inject]
        public TouchCameraMovement(ApplicationConfigs configs, InputHandler inputHandler)
        {
            _configs = configs.camera;
            _cameraActions = inputHandler.CameraActions;
            InitializeParameters();
        }

        public CameraMovementResult CalculateMovement()
        {
            var touches = EnhancedTouch.Touch.activeTouches;
            var touchCount = touches.Count;

            if (touchCount == 1)
            {
                var touch = touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    _rotateAllowed = !EventSystem.current.IsPointerOverGameObject(touch.touchId);
                }

                if (_rotateAllowed && touch.phase == TouchPhase.Moved)
                {
                    _rawDelta = touch.delta;
                }
                else if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled)
                {
                    _rawDelta = Vector2.zero;
                }
            }
            else
            {
                _rawDelta = Vector2.zero;
                _rotateAllowed = false;
            }

            var smoothFactor = 1f - Mathf.Exp(-_configs.mouseSmooth * Time.deltaTime);
            _smoothedDelta = Vector2.Lerp(_smoothedDelta, _rawDelta, smoothFactor);

            _eulerAngles.y += _smoothedDelta.x * _configs.xSpeed * Time.deltaTime;
            _eulerAngles.x -= _smoothedDelta.y * _configs.ySpeed * Time.deltaTime;
            _eulerAngles.x = MathUtils.ClampAngle(_eulerAngles.x, _configs.yMinLimit, _configs.yMaxLimit);

            if (touchCount == 2)
            {
                var t0 = touches[0];
                var t1 = touches[1];

                var currentDistance = Vector2.Distance(t0.screenPosition, t1.screenPosition);

                if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
                {
                    _previousPinchDistance = currentDistance;
                }
                else
                {
                    var delta = currentDistance - _previousPinchDistance;
                    _previousPinchDistance = currentDistance;

                    var maxDistance = _configs.maxDistance;
                    var minDistance = _configs.minDistance;

                    _desiredDistance -= delta / Screen.height * (maxDistance - minDistance) * _configs.zoomSpeed;
                    _desiredDistance = Mathf.Clamp(_desiredDistance, minDistance, maxDistance);
                }
            }

            _currentDistance = Mathf.Lerp(_currentDistance, _desiredDistance, Time.deltaTime * _configs.zoomSmooth);

            var rotation = Quaternion.Euler(_eulerAngles.x, _eulerAngles.y, 0);
            var negativeDistance = new Vector3(0, 0, -_currentDistance);
            var position = rotation * negativeDistance + _configs.targetOffset;

            return new CameraMovementResult(position, rotation);
        }

        private void InitializeParameters()
        {
            _eulerAngles = _configs.startRotation.eulerAngles;
            _desiredDistance = _currentDistance = _configs.distance;
            EnhancedTouch.EnhancedTouchSupport.Enable();
        }
    }
}

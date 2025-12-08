using Input.TouchRegistry;
using UnityEngine;
using Zenject;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Cameras.Input
{
    public class TouchCameraInputProvider : ICameraInputProvider
    {
        private readonly TouchRegistry _touchRegistry;
        private readonly float _zoomCoefficient = 1f / Screen.height;
        private float _previousPinchDistance;

        private const float PinchThreshold = 5f;
        private const float HeightThreshold = 0.5f;

        public bool IsRotationAllowed { get; private set; }
        public Vector2 RotationDelta { get; private set; }
        public float ZoomDelta { get; private set; }
        
        public bool IsHeightChangeAllowed { get; private set; }
        public float HeightDelta { get; private set; }

        [Inject]
        private TouchCameraInputProvider(TouchRegistry touchRegistry)
        {
            _touchRegistry = touchRegistry;
        }

        public void UpdateInput()
        {
            var touches = _touchRegistry.GetAvailableTouches();
            var count = touches.Count;

            RotationDelta = Vector2.zero;
            ZoomDelta = 0f;
            HeightDelta = 0f;
            IsHeightChangeAllowed = false;

            if (count == 1)
                HandleSingleTouch(touches[0]);
            else
                IsRotationAllowed = false;

            if (count == 2)
                HandleMultiTouch(touches[0], touches[1]);
        }

        private void HandleSingleTouch(Touch touch)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    IsRotationAllowed = true;
                    break;
                case TouchPhase.Moved:
                    RotationDelta = touch.delta;
                    IsRotationAllowed = true;
                    break;
                case TouchPhase.Ended or TouchPhase.Canceled:
                    IsRotationAllowed = false;
                    break;
            }
        }

        private void HandleMultiTouch(Touch touch0, Touch touch1)
        {
            if (!IsPinchAllowed(touch0, touch1))
            {
                _previousPinchDistance = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);
                return;
            }

            var currentDist = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);
            var distDelta = currentDist - _previousPinchDistance;

            var isZoomGesture = Mathf.Abs(distDelta) > PinchThreshold;

            if (isZoomGesture)
                HandlePinchZoom(distDelta);
            else
                HandleHeightChange(touch0, touch1);

            _previousPinchDistance = currentDist;
        }

        private void HandlePinchZoom(float distDelta)
        {
            ZoomDelta = distDelta * _zoomCoefficient;
            IsHeightChangeAllowed = false;
        }

        private void HandleHeightChange(Touch touch0, Touch touch1)
        {
            var deltaY0 = touch0.delta.y;
            var deltaY1 = touch1.delta.y;

            var sameDirection = Mathf.Approximately(Mathf.Sign(deltaY0), Mathf.Sign(deltaY1));

            if (sameDirection && Mathf.Abs((deltaY0 + deltaY1) * 0.5f) > HeightThreshold)
            {
                HeightDelta = (deltaY0 + deltaY1) * 0.5f * 0.01f;
                IsHeightChangeAllowed = true;
            }
            else
            {
                HeightDelta = 0f;
                IsHeightChangeAllowed = false;
            }
        }

        private static bool IsPinchAllowed(Touch touch0, Touch touch1)
        {
            if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended)
                return false;
            if (touch0.phase == TouchPhase.Canceled || touch1.phase == TouchPhase.Canceled)
                return false;
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
                return false;

            return true;
        }
    }
}

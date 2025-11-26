using Input.TouchRegistry;
using UnityEngine;
using UnityEngine.EventSystems;
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

        public bool IsRotationAllowed { get; private set; }
        public Vector2 RotationDelta { get; private set; }
        public float ZoomDelta { get; private set; }
        public bool IsHeightChangeAllowed { get; }
        public float HeightDelta { get; }

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

            if (count == 1)
            {
                var t = touches[0];
                switch (t.phase)
                {
                    case TouchPhase.Began:
                        IsRotationAllowed = true;
                        break;
                    case TouchPhase.Moved:
                        RotationDelta = t.delta;
                        IsRotationAllowed = true;
                        break;
                    case TouchPhase.Ended or TouchPhase.Canceled:
                        IsRotationAllowed = false;
                        break;
                }
            }
            else
            {
                IsRotationAllowed = false;
            }

            if (count == 2)
            {
                var t0 = touches[0];
                var t1 = touches[1];

                if (IsPinchAllowed(t0, t1))
                {
                    var currentDist = Vector2.Distance(t0.screenPosition, t1.screenPosition);
                    ZoomDelta = (currentDist - _previousPinchDistance) * _zoomCoefficient;
                    _previousPinchDistance = currentDist;
                }
                else
                {
                    _previousPinchDistance = Vector2.Distance(t0.screenPosition, t1.screenPosition);
                }
            }
        }

        private static bool IsPinchAllowed(Touch t0, Touch t1)
        {
            if (t0.phase == TouchPhase.Ended || t1.phase == TouchPhase.Ended)
                return false;

            if (t0.phase == TouchPhase.Canceled || t1.phase == TouchPhase.Canceled)
                return false;

            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
                return false;

            return true;
        }
    }
}

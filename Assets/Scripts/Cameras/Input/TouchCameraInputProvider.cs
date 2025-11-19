using Input.TouchRegistry;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
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
                if (t.phase == TouchPhase.Began)
                {
                    IsRotationAllowed = !EventSystem.current.IsPointerOverGameObject(t.touchId);
                }

                if (IsRotationAllowed && t.phase == TouchPhase.Moved)
                {
                    RotationDelta = t.delta;
                }
                else if (t.phase is TouchPhase.Ended or TouchPhase.Canceled)
                {
                    IsRotationAllowed = false;
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

                var currentDist = Vector2.Distance(t0.screenPosition, t1.screenPosition);
                if (t0.phase != TouchPhase.Began && t1.phase != TouchPhase.Began)
                {
                    ZoomDelta = (currentDist - _previousPinchDistance) * _zoomCoefficient;
                }

                _previousPinchDistance = currentDist;
            }
        }
    }
}

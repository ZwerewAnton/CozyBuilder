using Input;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Cameras.Input
{
    public class MouseCameraInputProvider : ICameraInputProvider
    {
        private const float ScrollCoefficient = 0.001f;
        private const float HeightCoefficient = 0.02f;
        private readonly InputActions.CameraActions _actions;

        public bool IsRotationAllowed { get; private set; }
        public Vector2 RotationDelta { get; private set; }
        public float ZoomDelta { get; private set; }
        public bool IsHeightChangeAllowed { get; private set; }
        public float HeightDelta { get; private set; }

        [Inject]
        public MouseCameraInputProvider(InputHandler inputHandler)
        {
            _actions = inputHandler.CameraActions;
        }

        public void UpdateInput()
        {
            if (_actions.Press.WasPressedThisFrame())
                IsRotationAllowed = !EventSystem.current.IsPointerOverGameObject();

            RotationDelta = (_actions.Press.IsPressed() && IsRotationAllowed)
                ? _actions.Look.ReadValue<Vector2>()
                : Vector2.zero;

            if (_actions.Height.WasPressedThisFrame())
                IsHeightChangeAllowed = !EventSystem.current.IsPointerOverGameObject();

            HeightDelta = (_actions.Height.IsPressed() && IsHeightChangeAllowed)
                ? _actions.Look.ReadValue<Vector2>().y * HeightCoefficient
                : 0f;

            ZoomDelta = _actions.Zoom.ReadValue<float>() * ScrollCoefficient;
        }
    }
}
using System;
using Cameras;
using Configs;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Gameplay.Movement.Input
{
    public class MouseDetailViewMoverInputProvider : IDetailViewMoverInputProvider, IDisposable
    {
        public event Action InputCanceled;
        
        private float _depth;
        private readonly InputHandler _inputHandler;
        private readonly CameraHandler _cameraHandler;
        private readonly Vector2 _screenOffset;

        [Inject]
        private MouseDetailViewMoverInputProvider(InputHandler inputHandler, CameraHandler cameraHandler, ApplicationConfigs configs)
        {
            _inputHandler = inputHandler;
            _inputHandler.DetailActions.Tap.canceled += OnTapCanceled;
            _cameraHandler = cameraHandler;
            _screenOffset = configs.gameplay.screenOffset;
        }

        public bool IsInputActive()
        {
            return _inputHandler.DetailActions.Tap.IsPressed();
        }

        public void UpdateDepth(Vector3 worldDepthPosition)
        {
            _depth = _cameraHandler.WorldToScreenPoint(worldDepthPosition).z;
        }

        public Vector3 GetDesiredPosition()
        {
            return GetCursorWorldPoint();
        }

        public void Dispose()
        {
            _inputHandler.DetailActions.Tap.canceled -= OnTapCanceled;
        }

        public void BindPointer(int pointerId) {}

        private void OnTapCanceled(InputAction.CallbackContext callbackContext)
        {
            InputCanceled?.Invoke();
        }
        
        private Vector3 GetCursorWorldPoint()
        {
            var input = _inputHandler.DetailActions.Cursor.ReadValue<Vector2>();
            return _cameraHandler.ScreenToWorldPoint(input + _screenOffset, _depth);
        }
    }
}
using System;
using Cameras;
using Configs;
using Input;
using Input.TouchRegistry;
using UnityEngine;
using Zenject;

namespace Gameplay.Movement
{
    public class TouchDetailViewMoverInputProvider : IDetailViewMoverInputProvider, IDisposable
    {
        public event Action InputCanceled;
        
        private float _depth;
        private readonly CameraHandler _cameraHandler;
        private readonly TouchRegistry _touchRegistry;
        private readonly Vector2 _screenOffset;
        private int _currentMoverPointerId;

        [Inject]
        private TouchDetailViewMoverInputProvider(
            CameraHandler cameraHandler, 
            TouchRegistry touchRegistry, 
            ApplicationConfigs configs)
        {
            _cameraHandler = cameraHandler;
            _touchRegistry = touchRegistry;
            _touchRegistry.TouchCanceled += OnTouchCanceled;
            _screenOffset = configs.gameplay.screenOffset;
        }

        public bool IsInputActive()
        {
            return _touchRegistry.IsTouchPressed(_currentMoverPointerId);
        }

        public void UpdateDepth(Vector3 worldDepthPosition)
        {
            _depth = _cameraHandler.WorldToScreenPoint(worldDepthPosition).z;
        }

        public Vector3 GetDesiredPosition()
        {
            return GetCursorWorldPoint();
        }

        public void BindPointer(int pointerId)
        {
            _currentMoverPointerId = pointerId;
        }

        public void Dispose()
        {
            _touchRegistry.TouchCanceled -= OnTouchCanceled;
        }

        private void OnTouchCanceled(int touchId)
        {
            if (touchId == _currentMoverPointerId)
                InputCanceled?.Invoke();
        }
        
        private Vector3 GetCursorWorldPoint()
        {
            var input = _touchRegistry.GetTouchPosition(_currentMoverPointerId);
            return _cameraHandler.ScreenToWorldPoint(input + _screenOffset, _depth);
        }
    }
}
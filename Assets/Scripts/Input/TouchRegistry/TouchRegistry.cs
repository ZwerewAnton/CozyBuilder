using System;
using System.Collections.Generic;
using UI.Raycast;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Zenject;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Input.TouchRegistry
{
    public class TouchRegistry : IDisposable, ITouchPointerLock
    {
        private readonly HashSet<int> _lockedTouches = new();

        private readonly UIRaycasterHelper _raycasterHelper;
        private readonly Dictionary<int, bool> _touchBeganOverUI = new();

        [Inject]
        public TouchRegistry(UIRaycasterHelper raycasterHelper)
        {
            _raycasterHelper = raycasterHelper;
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += OnFingerDown;
            Touch.onFingerUp += OnFingerUp;
        }

        public void Dispose()
        {
            Touch.onFingerDown -= OnFingerDown;
            Touch.onFingerUp -= OnFingerUp;
        }

        public bool IsTouchLocked(int touchId)
        {
            return _lockedTouches.Contains(touchId);
        }

        public void LockTouch(int touchId)
        {
            _lockedTouches.Add(touchId);
        }

        public void UnlockTouch(int touchId)
        {
            _lockedTouches.Remove(touchId);
        }

        public event Action<int> TouchCanceled;

        public bool IsTouchPressed(int touchId)
        {
            var touches = Touch.activeTouches;

            var index = touches.IndexOf(touch => touch.touchId == touchId);
            return index >= 0 && touches[index].inProgress;
        }

        public Vector2 GetTouchPosition(int touchId)
        {
            var touches = Touch.activeTouches;

            var index = touches.IndexOf(touch => touch.touchId == touchId);
            return index >= 0 ? touches[index].screenPosition : Vector2.zero;
        }

        public List<Touch> GetAvailableTouches()
        {
            var result = new List<Touch>(Touch.activeTouches.Count);

            foreach (var touch in Touch.activeTouches)
            {
                var isLocked = _lockedTouches.Contains(touch.touchId);
                _touchBeganOverUI.TryGetValue(touch.touchId, out var isBeganOverUI);
                if (!isLocked && !isBeganOverUI)
                    result.Add(touch);
            }

            return result;
        }

        private void OnFingerDown(Finger finger)
        {
            AddTouchEntry(finger);
        }

        private void OnFingerUp(Finger finger)
        {
            RemoveTouchEntry(finger);
            TouchCanceled?.Invoke(finger.currentTouch.touchId);
        }

        private void AddTouchEntry(Finger finger)
        {
            var touchId = finger.currentTouch.touchId;
            var touchPosition = finger.currentTouch.screenPosition;
            var state = _raycasterHelper.IsOverUI(touchPosition);
            _touchBeganOverUI[touchId] = state;
        }

        private void RemoveTouchEntry(Finger finger)
        {
            var touchId = finger.currentTouch.touchId;
            if (_touchBeganOverUI.ContainsKey(touchId)) _touchBeganOverUI.Remove(touchId);
        }
    }
}
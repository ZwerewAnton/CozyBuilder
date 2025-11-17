using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Input.TouchRegistry
{
    public class TouchRegistry : IDisposable, ITouchPointerLock
    {
        public event Action<int> TouchCanceled;
        private readonly Dictionary<int, bool> _touchBeganOverUI = new();
        private readonly HashSet<int> _lockedTouches = new();

        private TouchRegistry()
        {
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += AddTouchEntry;
            Touch.onFingerUp += RemoveTouchEntry;
        }

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
        
        public List<Touch> GetUnlockedTouches()
        {
            var result = new List<Touch>(Touch.activeTouches.Count);

            foreach (var touch in Touch.activeTouches)
            {
                if (!_lockedTouches.Contains(touch.touchId))
                    result.Add(touch);
            }

            return result;
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

        public void Dispose()
        {
            Touch.onFingerDown -= OnFingerDown;
            Touch.onFingerUp -= OnFingerUp;
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
            var state = EventSystem.current.IsPointerOverGameObject(touchId);
            _touchBeganOverUI[touchId] = state;
        }

        private void RemoveTouchEntry(Finger finger)
        {
            var touchId = finger.currentTouch.touchId;
            if(_touchBeganOverUI.ContainsKey(touchId))
            {
                _touchBeganOverUI.Remove(touchId);
            }
        }
    }
}
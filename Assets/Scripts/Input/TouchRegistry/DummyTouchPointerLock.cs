namespace Input.TouchRegistry
{
    public class DummyTouchPointerLock : ITouchPointerLock
    {
        public bool IsTouchLocked(int touchId) => false;
        public void LockTouch(int touchId) { }
        public void UnlockTouch(int touchId) { }
    }
}
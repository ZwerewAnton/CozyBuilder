namespace Input.TouchRegistry
{
    public class DummyTouchPointerLock : ITouchPointerLock
    {
        public bool IsTouchLocked(int touchId)
        {
            return false;
        }

        public void LockTouch(int touchId)
        {
        }

        public void UnlockTouch(int touchId)
        {
        }
    }
}
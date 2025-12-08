namespace Input.TouchRegistry
{
    public interface ITouchPointerLock
    {
        bool IsTouchLocked(int touchId);
        void LockTouch(int touchId);
        void UnlockTouch(int touchId);
    }
}
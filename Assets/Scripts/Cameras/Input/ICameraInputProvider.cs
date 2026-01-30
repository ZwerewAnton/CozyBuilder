using UnityEngine;

namespace Cameras.Input
{
    public interface ICameraInputProvider
    {
        bool IsRotationAllowed { get; }
        Vector2 RotationDelta { get; }
        float ZoomDelta { get; }
        bool IsHeightChangeAllowed { get; }
        float HeightDelta { get; }
        void UpdateInput();
    }
}
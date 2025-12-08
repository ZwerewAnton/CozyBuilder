using System;
using UnityEngine;

namespace Gameplay.Movement.Input
{
    public interface IDetailViewMoverInputProvider
    {
        event Action InputCanceled;
        Vector3 GetDesiredPosition();
        bool IsInputActive();
        void UpdateDepth(Vector3 worldDepthPosition);
        void BindPointer(int pointerId);
    }
}
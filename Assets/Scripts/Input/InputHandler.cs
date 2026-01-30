using System;

namespace Input
{
    public class InputHandler : IDisposable
    {
        public InputHandler()
        {
            InputActions = new InputActions();
            CameraActions = InputActions.Camera;
            DetailActions = InputActions.Detail;
            EnableActions();
        }

        public InputActions.CameraActions CameraActions { get; }
        public InputActions.DetailActions DetailActions { get; private set; }
        private InputActions InputActions { get; }

        public void Dispose()
        {
            DisableActions();
            InputActions?.Dispose();
        }

        private void EnableCamera()
        {
            CameraActions.Enable();
        }

        private void DisableCamera()
        {
            CameraActions.Disable();
        }

        private void EnableActions()
        {
            InputActions.Enable();
        }

        private void DisableActions()
        {
            InputActions.Disable();
        }
    }
}
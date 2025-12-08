using System;

namespace Input
{
    public class InputHandler : IDisposable
    {
        public InputActions.CameraActions CameraActions { get; private set; }
        public InputActions.DetailActions DetailActions { get; private set; }
        private InputActions InputActions { get; set; }

        public InputHandler()
        {
            InputActions = new InputActions();
            CameraActions = InputActions.Camera;
            DetailActions = InputActions.Detail;
            EnableActions();
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

        public void Dispose()
        {
            DisableActions();
            InputActions?.Dispose();
        }
    }
}
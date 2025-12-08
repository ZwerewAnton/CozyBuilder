namespace Common
{
    public class GameState
    {
        public bool IsFirstMenuLaunch { get; private set; } = true;
        public string SelectedLevelName { get; set; }
        public bool IsLevelCompletedOnStart { get; set; }

        public void MarkMenuAsLaunched()
        {
            IsFirstMenuLaunch = false;
        }
    }
}
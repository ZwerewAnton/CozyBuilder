using System.Threading;
using System.Threading.Tasks;
using SaveSystem.DataObjects.Level.New;
using SaveSystem.DataObjects.Progress;

namespace SaveSystem
{
    public interface ISaveLoadService
    {
        ProgressSaveData ProgressData { get; }
        Task LoadProgressDataAsync(CancellationToken cancellationToken = default);
        Task SaveProgressDataAsync(string levelName, int progress);
        Task<LevelSaveData> LoadLevelDataAsync(string levelName, CancellationToken cancellationToken = default);
        Task SaveLevelDataAsync(string levelName, LevelSaveData data);
        void DeleteSaveDirectory();
    }
}
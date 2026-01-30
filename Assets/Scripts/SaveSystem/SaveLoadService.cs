using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SaveSystem.DataObjects.Level.New;
using SaveSystem.DataObjects.Progress;
using UnityEngine;
using Utils.Paths;

namespace SaveSystem
{
    public class SaveLoadService : ISaveLoadService
    {
        public ProgressSaveData ProgressData { get; private set; } = new();

        public async Task LoadProgressDataAsync(CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(Paths.PathToSaveDirectory) ||
                !File.Exists(Paths.GetPathToProgressData()))
                return;

            ProgressData = await LoadDataAsync<ProgressSaveData>(Paths.GetPathToProgressData(), cancellationToken);
        }

        public async Task SaveProgressDataAsync(string levelName, int progress)
        {
            UpdateProgressData(levelName, progress);
            await SaveDataAsync(ProgressData, Paths.PathToSaveDirectory, Paths.GetPathToProgressData());
        }

        public async Task<LevelSaveData> LoadLevelDataAsync(string levelName,
            CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(Paths.GetPathToLevelDataDirectory(levelName)) ||
                !File.Exists(Paths.GetPathToLevelData(levelName)))
            {
                Debug.Log($"Level {levelName} save data not found. Returning default.");
                return new LevelSaveData();
            }

            return await LoadDataAsync<LevelSaveData>(Paths.GetPathToLevelData(levelName), cancellationToken);
        }

        public async Task SaveLevelDataAsync(string levelName, LevelSaveData data)
        {
            await SaveDataAsync(data, Paths.GetPathToLevelDataDirectory(levelName),
                Paths.GetPathToLevelData(levelName));
        }

        public void DeleteSaveDirectory()
        {
            if (!Directory.Exists(Paths.PathToSaveDirectory))
                return;

            Directory.Delete(Paths.PathToSaveDirectory, true);
            ProgressData = new ProgressSaveData();
        }

        private void UpdateProgressData(string levelName, int progress)
        {
            var dataList = ProgressData.progressLevelsSaveData;
            var index = dataList.FindIndex(data => data.levelName == levelName);
            if (index == -1)
                dataList.Add(new ProgressLevelSaveData(levelName, progress));
            else
                dataList[index].progress = progress;
        }

        private static async Task<T> LoadDataAsync<T>(string path, CancellationToken cancellationToken = default)
            where T : new()
        {
            try
            {
                if (!File.Exists(path))
                    return new T();

                var dataStr = await File.ReadAllTextAsync(path, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                var result = JsonUtility.FromJson<T>(dataStr);
                return result ?? new T();
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"LoadDataAsync cancelled: {path}");
                return new T();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Load failed: {ex.Message}");
                return new T();
            }
        }

        private static async Task SaveDataAsync<T>(T dataObject, string directoryPath, string filePath)
        {
            try
            {
                var jsonStr = JsonUtility.ToJson(dataObject);
                Directory.CreateDirectory(directoryPath);
                await File.WriteAllTextAsync(filePath, jsonStr);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Save failed: {ex.Message}");
                throw;
            }
        }
    }
}
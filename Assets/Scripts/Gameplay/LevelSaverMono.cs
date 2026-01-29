using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gameplay.Dto;
using SaveSystem;
using SaveSystem.DataObjects.Level.New;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class LevelSaverMono : MonoBehaviour
    {
        private ISaveLoadService _saveLoadService;
        private LevelService _levelService;

        [Inject]
        private void Construct(ISaveLoadService saveLoadService, LevelService levelService)
        {
            _saveLoadService = saveLoadService;
            _levelService = levelService;
        }

        private void OnEnable()
        {
            _levelService.LevelCompleted += SaveProgress;
        }

        private void OnDisable()
        {
            _levelService.LevelCompleted -= SaveProgress;
        }
        
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveProgress();
            }
        }
        
        public void SaveProgress()
        {
            var levelName = _levelService.GetLevelName();
            var progress = _levelService.GetLevelProgress();
            var details = _levelService.GetDetailsInfo();
            var data = CreateLevelSaveData(details);
            
            SaveLevelDataAsync(levelName, progress, data).Forget();
        }

        private async UniTask SaveLevelDataAsync(string levelName, int progress, LevelSaveData data)
        {
            try
            {
                await _saveLoadService.SaveLevelDataAsync(levelName, data);
                await _saveLoadService.SaveProgressDataAsync(levelName, progress);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save level progress: {ex}");
            }
        }

        private static LevelSaveData CreateLevelSaveData(Dictionary<string, DetailInstanceDto> details)
        {
            var levelSaveData = new LevelSaveData();

            foreach (var (id, detail) in details)
            {
                var detailData = new DetailSaveData
                {
                    id = id,
                    currentCount = detail.CurrentCount
                };

                foreach (var point in detail.Points)
                {
                    var pointData = new PointSaveData
                    {
                        isInstalled = point.IsInstalled
                    };
                    detailData.points.Add(pointData);
                }
                
                levelSaveData.details.Add(detailData);
            }

            return levelSaveData;
        }
    }
}
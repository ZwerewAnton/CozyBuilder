using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _1_LEVEL_REWORK.New.Data;
using _1_LEVEL_REWORK.New.Instances;
using Common;
using Gameplay.Dto;
using Level;
using SaveSystem;
using SaveSystem.DataObjects.Level.New;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class LevelService : IDisposable
    {
        public event Action LevelInitialized;
        public event Action LevelUpdated;
        public event Action LevelCompleted;
        
        private readonly LevelState _levelState;
        private readonly SaveLoadService _saveLoadService;
        private readonly GameState _gameState;
        private readonly LevelsRepository _repository;
        private LevelData _levelData;
        private CancellationTokenSource _cts;
        private Dictionary<string, DetailInstanceDto> _dtos = new();
        
        [Inject]
        private LevelService(
            GameState gameState,
            LevelsRepository repository,
            LevelState levelState,
            SaveLoadService saveLoadService)
        {
            _levelState = levelState;
            _saveLoadService = saveLoadService;
            _gameState = gameState;
            _repository = repository;
        }

        public async Task InitializeLevel()
        {
            _cts = new CancellationTokenSource();
        
            var result = _repository.TryGetLevel(_gameState.SelectedLevelName, out _levelData);
            
            if (!result)
                return;
        
            var saveData = await _saveLoadService.LoadLevelDataAsync(_levelData.LevelName, _cts.Token);
        
            InitializeLevelState(saveData);
            FillDetailsDtoList();
            
            LevelInitialized?.Invoke();
            CheckLevelComplete();
        }

        public bool TryInstallDetail(string detailId, int pointIndex)
        {
            var isSuccess = _levelState.TryInstallDetail(detailId, pointIndex);
            if (isSuccess)
            {
                UpdateDtoList();
                LevelUpdated?.Invoke();
                CheckLevelComplete();
            }
            
            return isSuccess;
        }

        public Dictionary<string, DetailInstanceDto> GetDetailsInfo()
        {
            return _dtos;
        }

        public string GetLevelName()
        {
            return _levelData.LevelName;
        }

        public int GetLevelProgress()
        {
            float allDetails = 0;
            float installedDetails = 0;
            var details = _levelState.Details;
            foreach (var (_, detail) in details)
            {
                foreach (var point in detail.Points)
                {
                    if (point.IsInstalled)
                    {
                        installedDetails++;
                    }
                    allDetails++;
                }
            }
            return Mathf.RoundToInt(installedDetails / allDetails * 100);
        }

        private void InitializeLevelState(LevelSaveData saveData)
        {
            _levelState.CreateDetailsInstances(_levelData.Ground, _levelData.Details, saveData.details);
        }

        private void FillDetailsDtoList()
        {
            var details = _levelState.Details;
            foreach (var (id, detailInstance) in details)
            {
                var detailDto = new DetailInstanceDto
                {
                    Icon = detailInstance.GetDetailIcon(),
                    Prefab = detailInstance.GetDetailPrefab(),
                    CurrentCount = detailInstance.RemainingCount,
                    IsGround = detailInstance.IsGround,
                    Mesh = detailInstance.GetDetailMesh(),
                    Material = detailInstance.GetDetailMaterial(),
                    Points = new List<PointInstanceDto>()
                };
                foreach (var pointInstance in detailInstance.Points)
                {
                    detailDto.Points.Add(new PointInstanceDto
                    {
                        IsInstalled = pointInstance.IsInstalled,
                        Position = pointInstance.Position,
                        Rotation = pointInstance.Rotation,
                        IsAvailable = _levelState.IsPointReady(pointInstance)
                    });
                }
                _dtos.Add(id, detailDto);
            }
        }

        private void UpdateDtoList()
        {
            var details = _levelState.Details;
            foreach (var (id, detailInstance) in details)
            {
                var detailDto = _dtos[id];
                detailDto.CurrentCount = detailInstance.RemainingCount;
                for (var i = 0; i < detailInstance.Points.Count; i++)
                {
                    var pointInstance = detailInstance.Points[i];
                    var pointDto = detailDto.Points[i];
                    pointDto.IsInstalled = pointInstance.IsInstalled;
                    pointDto.IsAvailable = _levelState.IsPointReady(pointInstance);
                }
            }
        }

        private void CheckLevelComplete()
        {
            var details = _levelState.Details;
            var isCompleted = true;
            foreach (var (_, detailInstance) in details)
            {
                if (detailInstance.IsGround || detailInstance.RemainingCount == 0)
                    continue;
                
                isCompleted = false;
                break;
            }

            if (isCompleted)
                LevelCompleted?.Invoke();
        }
        
        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
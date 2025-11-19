using System;
using System.Collections.Generic;
using System.Linq;
using _1_LEVEL_REWORK.New.Instances;
using Gameplay.Dto;
using Gameplay.Movement;
using Gameplay.Movement.Input;
using Gameplay.Spawn;
using Input.TouchRegistry;
using UI.Game.DetailsScroll;
using UI.Mediators;
using Zenject;

namespace Gameplay
{
    public class LevelInteractableCoordinator : IDisposable
    {
        private readonly LevelService _levelService;
        private readonly DetailPrefabSpawner _spawner;
        private readonly LevelMediator _levelMediator;
        private readonly DetailViewMover _detailViewMover;
        private readonly IDetailViewMoverInputProvider _moverInputProvider;
        private readonly ITouchPointerLock _pointerLock;
        private DragOutInfo _movingDetailInfo;
        
        [Inject]
        private LevelInteractableCoordinator(
            LevelService levelService,
            DetailPrefabSpawner spawner,
            LevelMediator levelMediator,
            IDetailViewMoverInputProvider moverInputProvider,
            ITouchPointerLock pointerLock,
            DetailViewMover detailViewMover)
        {
            _levelService = levelService;
            _levelService.LevelInitialized += OnLevelServiceInitialize;
            _spawner = spawner;
            _levelMediator = levelMediator;
            _detailViewMover = detailViewMover;
            _moverInputProvider = moverInputProvider;
            _pointerLock = pointerLock;
            _detailViewMover.PlacementEnded += OnDetailPlacementEnded;
        }

        private void OnLevelServiceInitialize()
        {
            var details = _levelService.GetDetailsInfo();
            SetDetailsScrollItems(details);
            SpawnStartDetailPrefabs(details);
        }
        
        private void SetDetailsScrollItems(Dictionary<string,DetailInstanceDto> details)
        {
            var detailModels = CreateDetailModelList(details);
            _levelMediator.InitializeDetailsScroll(detailModels);
            _levelMediator.DetailItemDragOutStarted += OnDetailDragOutStarted;
        }
        
        private void OnDetailPlacementEnded(PlacementResult placementResult)
        {
            if (_movingDetailInfo == null)
                return;

            var detailId = _movingDetailInfo.DetailId;
            
            _levelMediator.MarkItemDragOutState(detailId, false);
            
            if (!placementResult.Success) 
                return;
            
            var installResult = _levelService.TryInstallDetail(detailId, placementResult.PointIndex);
            if (!installResult) 
                return;
            
            var details = _levelService.GetDetailsInfo();
            _levelMediator.UpdateScrollController(CreateDetailModelList(details));
            SpawnDetailPrefab(details[detailId], placementResult.PointIndex);
            _pointerLock.UnlockTouch(_movingDetailInfo.PointerId);
        }

        private List<DetailItemModel> CreateDetailModelList(Dictionary<string,DetailInstanceDto> details)
        {
            var detailModels = new List<DetailItemModel>();
            foreach (var (id, detail) in details)
            {
                if (detail.CurrentCount == 0 || detail.IsGround)
                    continue;
                
                detailModels.Add(new DetailItemModel
                {
                    ID = id,
                    Icon = detail.Icon,
                    Count = detail.CurrentCount,
                    IsInactive = !detail.Points.Any(point => point.IsAvailable)
                });
            }

            return detailModels;
        }
        
        private void OnDetailDragOutStarted(DragOutInfo dragInfo)
        {
            _movingDetailInfo = dragInfo;
            _levelMediator.MarkItemDragOutState(_movingDetailInfo.DetailId, true);
            _moverInputProvider.BindPointer(_movingDetailInfo.PointerId);
            _pointerLock.LockTouch(_movingDetailInfo.PointerId);
            var detail = _levelService.GetDetailsInfo()[_movingDetailInfo.DetailId];
            StartDetailViewMove(detail);
        }
        
        private void StartDetailViewMove(DetailInstanceDto detail)
        {
            var pointList = detail.Points.Select(pointInstance => new PointTransform(pointInstance.Position, pointInstance.Rotation)).ToList();
            _detailViewMover.StartMove(detail.Mesh, detail.Material, pointList);
        }
        
        private void SpawnStartDetailPrefabs(Dictionary<string,DetailInstanceDto> details)
        {
            var spawnInfoList = new List<DetailPrefabSpawnInfo>();
            foreach (var (_, detail) in details)
            {
                foreach (var point in detail.Points)
                {
                    if (!point.IsInstalled)
                    {
                        continue;
                    }
                    
                    var spawnInfo = new DetailPrefabSpawnInfo(detail.Prefab, point.Position, point.Rotation);
                    spawnInfoList.Add(spawnInfo);
                }
            }
            _spawner.SpawnPrefabs(spawnInfoList);
        }

        private void SpawnDetailPrefab(DetailInstanceDto detail, int pointIndex)
        {
            var point = detail.Points[pointIndex];
            _spawner.SpawnPrefab(new DetailPrefabSpawnInfo(detail.Prefab, point.Position, point.Rotation));
        }

        public void Dispose()
        {
            _levelService.LevelInitialized -= OnLevelServiceInitialize;
            _levelMediator.DetailItemDragOutStarted -= OnDetailDragOutStarted;
            _detailViewMover.PlacementEnded -= OnDetailPlacementEnded;
        }
    }
}
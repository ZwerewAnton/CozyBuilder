using System;
using System.Collections.Generic;
using _1_LEVEL_REWORK.New.Instances;
using Configs;
using UnityEngine;
using Zenject;

namespace Gameplay.Movement
{
    public class DetailViewMover : MonoBehaviour
    {
        [SerializeField] private MovingDetailView movingDetailViewPrefab;
        [SerializeField] private GhostDetailView ghostDetailViewPrefab;

        private IDetailViewMoverInputProvider _moverInputProvider;
        private float _magnetDistance;
        private float _ghostDistance;
        
        private MovingDetailView _movingDetail;
        private GhostDetailView _ghostDetail;

        private List<PointTransform> _connectionPoints;
        private bool _isMoving;
        private bool _isConnected;
        private int _bestPointIndex;

        private static readonly Color NearColor = new(1, 1, 1, 0.5f);
        private static readonly Color FarColor = new(1, 1, 1, 0.0f);

        public event Action<PlacementResult> PlacementEnded;

        [Inject]
        private void Construct(ApplicationConfigs config, IDetailViewMoverInputProvider moverInputProvider)
        {
            _magnetDistance = config.gameplay.magnetDistance;
            _ghostDistance = config.gameplay.ghostDistance;
            _moverInputProvider = moverInputProvider;
        }
        
        private void Awake()
        {
            InstantiateDetailViews();
            _moverInputProvider.InputCanceled += OnInputCanceled;
        }

        private void Update()
        {
            if (!_isMoving)
                return;
            
            UpdateMove(_moverInputProvider.GetDesiredPosition());
        }

        private void OnDestroy()
        {
            _moverInputProvider.InputCanceled -= OnInputCanceled;
        }

        public void StartMove(Mesh mesh, Material material, List<PointTransform> connectionPoints)
        {
            _moverInputProvider.UpdateDepth(Vector3.zero);
            _isMoving = true;
            _isConnected = false;
            
            if (connectionPoints.Count == 0 || !_moverInputProvider.IsInputActive())
            {
                StopMove();
            }
            
            _connectionPoints = connectionPoints;
            _movingDetail.Show(mesh, material);
            _ghostDetail.Show(mesh);
            _ghostDetail.SetMaterialColor(FarColor);
        }

        public void StopMove()
        {
            if (!_isMoving)
                return;

            _isMoving = false;

            var result = new PlacementResult
            {
                Success = _isConnected,
                PointIndex = _bestPointIndex
            };

            PlacementEnded?.Invoke(result);

            _movingDetail.Hide();
            _ghostDetail.Hide();
        }

        private void InstantiateDetailViews()
        {
            _movingDetail = Instantiate(movingDetailViewPrefab, Vector3.zero, Quaternion.identity, transform);
            _movingDetail.Hide();
            _ghostDetail = Instantiate(ghostDetailViewPrefab, Vector3.zero, Quaternion.identity, transform);
            _ghostDetail.Hide();
        }
        
        private void UpdateMove(Vector3 desiredPosition)
        {
            var minDistance = float.MaxValue;
            var closestPoint = new PointTransform();
            var closestPointIndex = 0;

            for (var i = 0; i < _connectionPoints.Count; i++)
            {
                var point = _connectionPoints[i];
                var distance = Vector3.Distance(desiredPosition, point.Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = point;
                    closestPointIndex = i;
                }
            }

            var distanceToBest = Vector3.Distance(desiredPosition, closestPoint.Position);
            _moverInputProvider.UpdateDepth(closestPoint.Position);

            if (distanceToBest <= _magnetDistance)
            {
                _isConnected = true;
                _bestPointIndex = closestPointIndex;
                _movingDetail.SetPositionAndRotation(closestPoint.Position, closestPoint.Rotation);
                _ghostDetail.SetMaterialColor(FarColor);
            }
            else
            {
                _isConnected = false;
                _movingDetail.SetPositionAndRotation(desiredPosition, closestPoint.Rotation);
                _ghostDetail.SetPositionAndRotation(closestPoint.Position, closestPoint.Rotation);
                var t = Mathf.InverseLerp(0, _ghostDistance, distanceToBest);
                var color = Color.Lerp(NearColor, FarColor, t);
                _ghostDetail.SetMaterialColor(color);
            }
        }

        private void OnInputCanceled()
        {
            StopMove();
        }
    }
}
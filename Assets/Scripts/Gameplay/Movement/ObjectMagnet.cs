using System.Collections.Generic;
using Infrastructure.SceneManagement;
using Level;
using SaveSystem;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using VFX;
using UnityEngine.InputSystem;

namespace Gameplay.Movement
{
    public class ObjectMagnet : MonoBehaviour
    {
        [FormerlySerializedAs("cam")] public Camera mainCamera;
        [FormerlySerializedAs("magnDist")] public float magnetDistance = 0.5f;
        [SerializeField] private Material grayMaterial;
        [SerializeField] private Vector3 objectOffset;
        [SerializeField] private UnityEvent restartEvent;
        [FormerlySerializedAs("_onInstantiateGround")] [SerializeField] private UnityEvent<Transform> onInstantiateGround;
        [SerializeField] private ParticlePlayer particlePlayer;
        [SerializeField] private LevelMenu levelMenu;
    
        private GameObject _target;
        private float _mZCoord;
        private bool _isConnect;
        private bool _isInstantiate;
        private List<GameObject> _instantiateDetailObjects;
        private List<Detail> _allDetails;
        private List<Point> _connectionPoints;
        private List<GameObject> _connectionObjects;
        private List<PointParentConnector> _allPoints;
        private Detail _instDetail;
        private Material _mainMaterial;
        private MeshRenderer _meshRenderer;
        private readonly Color _grayNearColor = new Color32(255, 255, 255, 150);
        private readonly Color _grayFarColor = new Color32(255, 255, 255, 0);
        private float _lerp;
        private GameObject _currentConnObject;
        private Detail _ground;
    
        private void Awake() 
        {
            _allDetails = LevelContainer.currentLevelContainer.GetLoadLevel();
            _ground = LevelContainer.currentLevelContainer.GetCurrentLevelGround();
        
            _instantiateDetailObjects = new List<GameObject>();
            _connectionPoints = new List<Point>();
            _connectionObjects = new List<GameObject>();
            StartDetailInstance();
            if (IsEndOfDetails())
            {
                levelMenu.ShowEndScreen();
            }

            objectOffset.y = Screen.height * 0.1f;
        }

        public void InstantiateObject(Detail instDetail)
        {
            _instDetail = instDetail;
            var instObject = instDetail.prefab.transform.GetChild(0).gameObject;

            _connectionPoints.Clear();
            _connectionObjects.Clear();

            instObject.transform.position = Vector3.zero;
            _mZCoord = mainCamera.WorldToScreenPoint(instObject.transform.position).z;

            _target = Instantiate(instObject, UnityEngine.Input.touchSupported ? GetTouchAsWorldPoint() : GetMouseAsWorldPoint(), Quaternion.identity);

            _connectionPoints = _instDetail.GetAvailablePoints();

            foreach (var connectionPoint in _connectionPoints)
            {
                var connectionObject = Instantiate(instObject, connectionPoint.position, Quaternion.Euler(connectionPoint.rotation));
                _connectionObjects.Add(connectionObject);
                _meshRenderer = connectionObject.GetComponent<MeshRenderer>();
                _meshRenderer.material = grayMaterial;
                connectionObject.SetActive(false);
            }
            _isInstantiate = true;
        }

        public bool InstallOrDropObject()
        {
            _isInstantiate = false;

            foreach (var connectionObject in _connectionObjects)
            {
                Destroy(connectionObject);
            }

            if (_isConnect)
            {
                var targetTransform = _target.GetComponent<Transform>();
                foreach (var pointParConn in _instDetail.points)
                {
                    var angle = Quaternion.Angle(targetTransform.rotation, Quaternion.Euler(pointParConn.point.rotation));
                    if (targetTransform.position == pointParConn.point.position && angle == 0)
                    {
                        pointParConn.Install();
                    }
                }
                _meshRenderer.material = _mainMaterial;
                _instantiateDetailObjects.Add(_target);
                _target = null;
                return true;
            }
            Destroy(_target);
            return false;
        }

        public bool IsLastDetail()
        {
            var isLastDetail = _instDetail.IsLastDetail();
            if (!isLastDetail) 
                return false;
        
            _allDetails.Remove(_instDetail);
            if (!IsEndOfDetails()) 
                return true;
        
            particlePlayer.Play();
            //levelMenu.PlayEndClip();
            levelMenu.ShowEndScreen();
            return true;
        }

        public bool IsEndOfDetails()
        {
            return _allDetails.Count == 0;
        }

        private Vector3 GetMouseAsWorldPoint()
        {
            var mousePoint = UnityEngine.Input.mousePosition + objectOffset;
            mousePoint.z = _mZCoord;
            return mainCamera.ScreenToWorldPoint(mousePoint);
        }
    
        private Vector3 GetTouchAsWorldPoint()
        {
            Vector3 touchData = UnityEngine.Input.GetTouch(0).position;
            var touchPoint = touchData + objectOffset;
            touchPoint.z = _mZCoord;
            return mainCamera.ScreenToWorldPoint(touchPoint);
        }
    
        private List<Detail> GetOpenDetails()
        {
            var list = new List<Detail>();
            foreach (var allDetail in _allDetails)
            {
                foreach (var allDetailPpc in allDetail.points)
                {
                    if (allDetailPpc.IsInstalled) 
                        continue;
                
                    var isOpen = true;
                    foreach (var allDetailParent in allDetailPpc.parentList)
                    {
                        foreach (var allDetailParentPpc in allDetailParent.GetAllPpc())
                        {
                            if (!allDetailParentPpc.IsInstalled){
                                isOpen = false;
                            }
                        }
                    }
                    if (isOpen)
                    {
                        list.Add(allDetail);
                    }
                }
            }
            return list;
        }

        private void StartDetailInstance()
        {
            var clearDetailList = new List<Detail>();
            InstantiateGround(_ground);
            foreach (var detail in _allDetails)
            {
                var isInstalled = true;
                foreach (var pointParentConnector in detail.points)
                {
                    if (pointParentConnector.IsInstalled)
                    {
                        var instObject = detail.prefab.transform.GetChild(0).gameObject;
                        instObject = Instantiate(
                            instObject, pointParentConnector.point.position, 
                            Quaternion.Euler(pointParentConnector.point.rotation));
                        _instantiateDetailObjects.Add(instObject);
                    }
                    else
                    {
                        isInstalled = false;
                    }
                }
                if (!isInstalled)
                {
                    clearDetailList.Add(detail);
                }
            }
            _allDetails = clearDetailList;
        }

        public List<Detail> GetALlDetails()
        {
            return _allDetails;
        }
    
        public List<Detail> GetAvailableDetails()
        {
            return GetOpenDetails();
        }
    
        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.Escape))
            // {
            //     SceneSwitcher.sceneLoader.LoadNextScene();
            // }

            if (!_isInstantiate) 
                return;
        
            TransformPosition(_target.transform.position,
                UnityEngine.Input.touchSupported ? GetTouchAsWorldPoint() : GetMouseAsWorldPoint(),
                magnetDistance);
        }

        private void InstantiateGround(Detail ground)
        {
            ground.points[0].Install();
            var instObject = Instantiate(
                ground.prefab.transform.GetChild(0).gameObject,
                ground.points[0].point.position,
                Quaternion.Euler(ground.points[0].point.rotation));
            onInstantiateGround.Invoke(instObject.transform);
        }

        private void TransformPosition(Vector3 targetPosition, Vector3 mousePosition, float distance)
        {
            Vector3 bestPoint = Vector3.zero, bestRotation = Vector3.zero;
            Vector3 yPosition;
            var tempDist = float.MaxValue;

            if (_connectionPoints.Count == 0)
            {
                _isConnect = false;
                yPosition =  mousePosition;
                if (yPosition.y < 0)
                {
                    yPosition.y = 0f;
                }
                _target.transform.position = yPosition;
            }
            else
            {
                for (var i = 0; i < _connectionPoints.Count; i++)
                {
                    var dist = Vector3.Distance (targetPosition, _connectionPoints[i].position);
                    _connectionObjects[i].SetActive(false);

                    if (!(dist < tempDist)) 
                        continue;
                
                    tempDist = dist;
                    bestPoint = _connectionPoints[i].position;
                    bestRotation = _connectionPoints[i].rotation;
                    _currentConnObject =_connectionObjects[i];
                }
                _currentConnObject.SetActive(true);

                var distanceApart = GetSqrDistance(bestPoint, mousePosition);
                _lerp = MapValue(distanceApart, 0f, 50f, 0f, 1f);
                grayMaterial.color = Color.Lerp(_grayNearColor, _grayFarColor, _lerp);
            
                Debug.DrawLine(targetPosition, bestPoint);
    
                _mZCoord = mainCamera.WorldToScreenPoint(bestPoint).z;
                _target.transform.rotation = Quaternion.Euler(bestRotation);

                if (distanceApart <= distance)
                {
                    _currentConnObject.SetActive(false);
                    _isConnect = true;
                    _target.transform.position = bestPoint;
                }
                else
                {
                    _isConnect = false;
                    yPosition =  mousePosition;
                    _target.transform.position = yPosition;
                }
            }
        }

        private static float GetSqrDistance(Vector3 v1, Vector3 v2)
        {
            return (v1 - v2).sqrMagnitude;
        }

        private static float MapValue(float mainValue, float inValueMin, float inValueMax, float outValueMin, float outValueMax)
        {
            return (mainValue - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
        }
    
        public void Restart()
        {
            LevelSaver.DeleteSaveFile();

            _allDetails = LevelContainer.currentLevelContainer.ResetAllDetails();
            _connectionPoints.Clear();
            _connectionObjects.Clear();
            foreach (var detailObject in _instantiateDetailObjects)
            {
                Destroy(detailObject);
            }
            _instantiateDetailObjects.Clear();
            StartDetailInstance();
            restartEvent.Invoke();
        }
    }
}
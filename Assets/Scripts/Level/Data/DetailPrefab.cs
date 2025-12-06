using UnityEngine;

namespace _1_LEVEL_REWORK.New.Instances
{
    public class DetailPrefab : MonoBehaviour
    {
        [SerializeField] private bool resetTransform = true;
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (resetTransform)
                _meshRenderer.transform.localPosition = Vector3.zero;
        }
    }
}
using UnityEngine;

namespace _1_LEVEL_REWORK.New.Instances
{
    public class DetailPrefab : MonoBehaviour
    {
        [SerializeField] private bool resetTransform = true;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        private void Awake()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _meshFilter = GetComponentInChildren<MeshFilter>();
            if (resetTransform)
                _meshRenderer.transform.localPosition = Vector3.zero;
        }

        public Material GetMaterial()
        {
            return _meshRenderer.material;
        }

        public Mesh GetMesh()
        {
            return _meshFilter.mesh;
        }
    }
}
using System;
using System.Collections.Generic;
using _1_LEVEL_REWORK.New.Instances;
using UnityEngine;

namespace _1_LEVEL_REWORK.New.Data
{
    [CreateAssetMenu(menuName = "Level/DetailData")]
    public class DetailData : ScriptableObject
    {
        [SerializeField] private string id = Guid.NewGuid().ToString();
        [SerializeField] private int count = 1;
        [SerializeField] private DetailPrefab prefab;
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material material;
        [SerializeField] private Sprite icon;
        public List<PointData> points = new();
        
        public string Id => id;
        
        public int Count
        {
            get => count;
            set => count = value;
        }
        
        public DetailPrefab Prefab
        {
            get => prefab;
            set => prefab = value;
        }
        
        public Mesh Mesh
        {
            get => mesh;
            set => mesh = value;
        }
        
        public Material Material
        {
            get => material;
            set => material = value;
        }
        
        public Sprite Icon
        {
            get => icon;
            set => icon = value;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (prefab == null)
                return;

            var meshFilter = prefab.GetComponentInChildren<MeshFilter>();
            var meshRenderer = prefab.GetComponentInChildren<MeshRenderer>();

            if (meshFilter != null && (mesh == null || mesh != meshFilter.sharedMesh))
                mesh = meshFilter.sharedMesh;

            if (meshRenderer != null && (material == null || material != meshRenderer.sharedMaterial))
                material = meshRenderer.sharedMaterial;
        }
#endif
    }
}
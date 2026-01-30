using UnityEngine;

namespace _1_LEVEL_REWORK.New.Instances
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class DetailView : MonoBehaviour
    {
        protected MeshFilter MeshFilter;
        protected MeshRenderer MeshRenderer;

        protected void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();
            MeshFilter = GetComponent<MeshFilter>();
        }

        public void Show(Mesh mesh)
        {
            MeshFilter.mesh = mesh;
            gameObject.SetActive(true);
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
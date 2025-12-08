using UnityEngine;
using UnityEngine.UI;

namespace UI.Raycast
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class CanvasRaycasterHandler : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster raycaster;

        public GraphicRaycaster Raycaster => raycaster;
    }
}
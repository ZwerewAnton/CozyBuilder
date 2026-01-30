using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.Movement
{
    public class DragCorrector : MonoBehaviour
    {
        private void Start()
        {
            var defaultValue = EventSystem.current.pixelDragThreshold;
            EventSystem.current.pixelDragThreshold = Mathf.Max(defaultValue, (int)(defaultValue * Screen.dpi / 160f));
        }
    }
}
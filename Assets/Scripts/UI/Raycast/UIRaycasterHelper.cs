using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace UI.Raycast
{
    public class UIRaycasterHelper
    {
        private readonly GraphicRaycaster _raycaster;
        private static readonly PointerEventData EventData = new(EventSystem.current);
        private static readonly List<RaycastResult> Results = new();
        
        [Inject]
        private UIRaycasterHelper(CanvasRaycasterHandler raycasterHandler)
        {
            _raycaster = raycasterHandler.Raycaster;
        }

        public bool IsOverUI(Vector2 screenPos)
        {
            Results.Clear();
            EventData.position = screenPos;
            _raycaster.Raycast(EventData, Results);

            return Results.Count > 0;
        }
    }
}
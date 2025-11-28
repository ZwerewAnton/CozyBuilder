using System;
using System.Collections.Generic;
using UI.Scroll;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace UI.Game.DetailsScroll
{
    public class DetailsScrollController : ScrollControllerBase<DetailItemModel, DetailItemView>
    {
        [Header("Canvas")]
        [SerializeField] private Canvas canvas;
        [Header("Drag Settings")]
        [SerializeField] private float dragThreshold = 30f;

        public event Action<DragOutInfo> DragOutStarted;

        private float _scaledDragThreshold;
        private bool _isDragOutStarted;
        private int _activePointerId = -1;
        private Vector2 _startDragPos;
        private int _draggedItemIndex = -1;
        private bool _isDraggedItemInactive;

        public override void Initialize(List<DetailItemModel> newModels)
        {
            base.Initialize(newModels);
            _scaledDragThreshold = dragThreshold * canvas.scaleFactor;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (_isDragOutStarted || _activePointerId != -1)
                return;
            
            if (eventData.pointerId >= 0)
                if (eventData.button != PointerEventData.InputButton.Left)
                    return;
            
            base.OnBeginDrag(eventData);

            _activePointerId = eventData.pointerId;
            _startDragPos = eventData.position;

            var draggedItem = GetItemUnderPointer(eventData);

            if (draggedItem == null) 
                return;
            
            _draggedItemIndex = draggedItem.ItemIndex;
            _isDraggedItemInactive = Models[_draggedItemIndex].IsInactive;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId)
                return;
            
            if (_isDraggedItemInactive || _draggedItemIndex < 0)
                return;

            if (_isDragOutStarted)
            {
                eventData.Use();
                return;
            }

            var delta = eventData.position - _startDragPos;

            if (Mathf.Abs(delta.x) > _scaledDragThreshold && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                StartDragOut(eventData);
            else
                base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId)
                return;
            
            base.OnEndDrag(eventData);

            _activePointerId = -1;
            _draggedItemIndex = -1;
            _isDragOutStarted = false;
        }

        public void MarkItemDragOutState(string detailId, bool isDragOut)
        {
            var model = Models.Find(itemModel => itemModel.ID == detailId);
            if (model == null)
                return;
            
            model.IsDragOut = isDragOut;

            MarkToUpdate();
        }

        private void StartDragOut(PointerEventData eventData)
        {
            if (_draggedItemIndex < 0 || _draggedItemIndex >= Models.Count)
                return;

            _isDragOutStarted = true;

            var pointerId = GetPointerId(eventData);
            
            DragOutStarted?.Invoke(new DragOutInfo(Models[_draggedItemIndex].ID, pointerId));
            
            ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
            scrollRect.velocity = Vector2.zero;
        }

        private DetailItemView GetItemUnderPointer(PointerEventData eventData)
        {
            foreach (var item in ActiveItems)
            {
                var rect = item.RectTransform;
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, eventData.position, eventData.pressEventCamera))
                    return item;
            }
            return null;
        }
        
        private int GetPointerId(PointerEventData eventData)
        {
            return (eventData as ExtendedPointerEventData)?.touchId ?? eventData.pointerId;
        }
    }
}
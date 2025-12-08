using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UI.Scroll
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class ScrollItemView<TModel> : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform rectTransform;
        public int ItemIndex { get; set; }
        public event Action<int> Clicked;

        public RectTransform RectTransform => rectTransform;

        public virtual void SetData(int itemIndex, TModel model)
        {
            ItemIndex = itemIndex;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(ItemIndex);
        }
    }
}
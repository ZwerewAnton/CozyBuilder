using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Scroll
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class ScrollItemView<TModel> : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform rectTransform;
        public int ItemIndex { get; set; }

        public RectTransform RectTransform => rectTransform;

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(ItemIndex);
        }

        public event Action<int> Clicked;

        public virtual void SetData(int itemIndex, TModel model)
        {
            ItemIndex = itemIndex;
        }
    }
}
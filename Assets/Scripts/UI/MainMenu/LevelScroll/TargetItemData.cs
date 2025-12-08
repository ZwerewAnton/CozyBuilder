using UnityEngine;

namespace UI.MainMenu.LevelScroll
{
    public struct TargetItemData
    {
        public Vector2 AnchoredPosition;
        public int ItemIndex;

        public TargetItemData(Vector2 anchoredPosition, int itemIndex)
        {
            AnchoredPosition = anchoredPosition;
            ItemIndex = itemIndex;
        }
    }
}
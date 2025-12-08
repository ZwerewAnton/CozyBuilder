using TMPro;
using UI.Scroll;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.DetailsScroll
{
    public class DetailItemView: ScrollItemView<DetailItemModel>
    {
        [SerializeField] private TMP_Text countText;
        [SerializeField] private Image detailImage;
        
        private static readonly Color DefaultColor = new(1, 1, 1, 1f);
        private static readonly Color InActiveColor = new(1, 1, 1, 0.6f);

        public override void SetData(int itemIndex, DetailItemModel model)
        {
            base.SetData(itemIndex, model);
            
            detailImage.sprite = model.Icon;
            detailImage.color = model.IsInactive ? InActiveColor : DefaultColor;
            detailImage.preserveAspect = true;
            var count = model.IsDragOut ? model.Count - 1 : model.Count;
            if (count <= 1)
            {
                countText.enabled = false;
            }
            else
            {
                countText.enabled = true;
                countText.SetText(count.ToString());
            }
        }
    }
}
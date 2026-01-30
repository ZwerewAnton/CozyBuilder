using TMPro;
using UI.Scroll;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu.LevelScroll
{
    public class LevelItemView : ScrollItemView<LevelItemModel>
    {
        [SerializeField] private Image levelIcon;
        [SerializeField] private TMP_Text levelName;
        [SerializeField] private TMP_Text progressPercent;

        public override void SetData(int itemIndex, LevelItemModel model)
        {
            base.SetData(itemIndex, model);

            levelIcon.sprite = model.levelIcon;
            levelName.SetText(model.levelName);
            progressPercent.SetText(model.progressPercent.ToString());
        }
    }
}
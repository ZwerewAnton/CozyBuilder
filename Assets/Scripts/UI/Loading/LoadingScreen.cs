using UnityEngine;
using UnityEngine.UI;

namespace UI.Loading
{
    public class LoadingScreen : LoadingScreenBase
    {
        [SerializeField] private Slider progressBar;

        public override void SetProgress(float value)
        {
            progressBar.value = value;
        }
    }
}
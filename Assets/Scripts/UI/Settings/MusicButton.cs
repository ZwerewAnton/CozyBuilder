// using UI.Mediators;
// using UnityEngine;
//
// namespace UI.Settings
// {
//     public class MusicButton : MonoBehaviour
//     {
//         private SettingsMediator _mediator;
//         private ToggleButton _toggleButton;
//
//         private void Awake()
//         {
//             _toggleButton = GetComponent<ToggleButton>();
//             _toggleButton.OnClicked.AddListener(Click);
//             _toggleButton.SetState(_mediator.IsMusicOn);
//         }
//
//         private void OnDestroy()
//         {
//             _toggleButton.OnClicked.RemoveListener(Click);
//         }
//
//         private void Click()
//         {
//             _mediator.ToggleMusic();
//             _mediator.PlaySettingButtonClip();
//         }
//     }
// }
// using DG.Tweening;
// using SceneManagement;
// using UnityEngine;
// using UnityEngine.Audio;
// using UnityEngine.Serialization;
// using UnityEngine.UI;
//
// namespace UI
// {
//     public class SettingsDropdownMenu : MonoBehaviour
//     {
//         [Header ("Spacing between menu items")]
//         [FormerlySerializedAs("_spacing")] [SerializeField] 
//         private Vector2 spacing;
//         [FormerlySerializedAs("_offset")] [SerializeField] 
//         private Vector2 offset;
//         private const float AudioOffValue = -80f;
//         private const float AudioOnValue = 0f;
//
//         [Space]
//         [Header ("Button rotation setting")]
//         [SerializeField] private float rotationDuration;
//         [SerializeField] private Ease rotationEase;
//
//         [Space]
//         [Header ("Animation setting")]
//         [SerializeField] private float expandDuration;
//         [SerializeField] private float collapseDuration;
//         [FormerlySerializedAs("expandeEase")] [SerializeField] private Ease expandEase;
//         [SerializeField] private Ease collapseEase;
//
//         [Space]
//         [Header ("Fading setting")]
//         [SerializeField] private float expandFadeDuration;
//         [SerializeField] private float collapseFadeDuration;
//         [SerializeField] private AudioMixer audioMixer;
//
//         public AudioSource gearSound;
//
//         [SerializeField] private Button settingButton;
//         [SerializeField] private Button musicButton;
//         [SerializeField] private Button soundButton;
//         [SerializeField] private Button backButton;
//         [SerializeField] private Sprite musicOnSprite;
//         [SerializeField] private Sprite musicOffSprite;
//         [SerializeField] private Sprite soundOnSprite;
//         [SerializeField] private Sprite soundOffSprite;
//         
//         private UISettingsMenuItem[] _menuItems;
//         private bool _isExpanded;
//         private Vector2 _settingButtonPosition;
//         private int _itemsCount;
//         private readonly string[] _playerPrefsKey = {"music", "sound"};
//         private readonly string[] _mixerVolumeParameters = {"MusicVolume", "SoundVolume"};
//         private readonly bool[] _isAudioOn = {true, true};
//         
//         private SceneLoader _sceneLoader;
//     
//         private void Awake()
//         {
//             for (var i = 0; i < _playerPrefsKey.Length; i++)
//             {
//                 if (!PlayerPrefs.HasKey(_playerPrefsKey[i]))
//                 {
//                     PlayerPrefs.SetFloat(_playerPrefsKey[i], AudioOnValue);
//                     audioMixer.SetFloat(_mixerVolumeParameters[i], AudioOnValue);
//                     _isAudioOn[i] = true;
//                     PlayerPrefs.Save ();
//                 }
//                 else
//                 {
//                     if (Mathf.Approximately(PlayerPrefs.GetFloat (_playerPrefsKey[i]), AudioOffValue))
//                     {
//                         audioMixer.SetFloat(_mixerVolumeParameters[i], AudioOffValue);
//                         _menuItems[i].Click();
//                         _isAudioOn[i] = false;
//                     }
//                     else
//                     {
//                         audioMixer.SetFloat(_mixerVolumeParameters[i], AudioOnValue);
//                         _isAudioOn[i] = true;
//                     }
//                 }
//             }
//         }
//
//         private void Start()
//         {
//             _sceneLoader = FindObjectOfType<SceneLoader>();
//             _itemsCount = transform.childCount - 1;
//             settingButton = transform.GetChild(0).GetComponent<Button>();
//             settingButton.transform.SetAsLastSibling();
//             _settingButtonPosition = settingButton.GetComponent<RectTransform>().localPosition;
//             ResetPosition();
//         }
//     
//         private void ResetPosition()
//         {
//             for (var i = 0; i < _itemsCount; i++)
//             {
//                 _menuItems[i].trans.localPosition = _settingButtonPosition;
//             }
//         }
//
//         public void ToggleMenu()
//         {
//             gearSound.Play();
//             if(_isExpanded)
//             {
//                 for(int i = 0; i < _itemsCount; i++)
//                 {
//                     _menuItems[i].trans.DOLocalMove(_settingButtonPosition, collapseDuration).SetEase(collapseEase);   
//                     _menuItems[i].img.DOFade(0f, collapseFadeDuration);
//                 }
//             }
//             else
//             {
//                 for(int i = 0; i < _itemsCount; i++)
//                 {
//                     _menuItems[i].trans.DOLocalMove(_settingButtonPosition + spacing * (i+1), expandDuration).SetEase(expandEase);
//                     _menuItems[i].img.DOFade(1f, expandFadeDuration).From(0f);
//                 }
//             }
//             settingButton.transform
//                 .DOLocalRotate(Vector3.forward * 180f, rotationDuration)
//                 .From(Vector3.zero)
//                 .SetEase(rotationEase);
//
//             _isExpanded = !_isExpanded;
//         }
//
//         public void ToggleMusic()
//         {
//             ToggleAudio(0);
//         }
//
//         public void ToggleSound()
//         {
//             ToggleAudio(1);
//         }
//
//         private void ToggleAudio(int i)
//         {
//             if (_isAudioOn[i])
//             {
//                 PlayerPrefs.SetFloat (_playerPrefsKey[i], AudioOffValue);
//                 audioMixer.SetFloat(_mixerVolumeParameters[i], AudioOffValue);
//                 _isAudioOn[i] = false;
//             }
//             else
//             {
//                 PlayerPrefs.SetFloat (_playerPrefsKey[i], AudioOnValue);
//                 audioMixer.SetFloat(_mixerVolumeParameters[i], AudioOnValue);
//                 _isAudioOn[i] = true;
//             }
//             PlayerPrefs.Save ();
//         }
//
//         public void Home()
//         {
//             _sceneLoader.LoadNextScene();
//         }
//     }
// }

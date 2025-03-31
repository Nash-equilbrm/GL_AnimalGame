using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using Game.Audio;
using Game.Config;

namespace Game.UI
{
    public class SettingPopup : BasePopup
    {
        [SerializeField] private Slider _backgroundMusicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Button _quitBtn;
        [SerializeField] private Button _aboutMeBtn;
        [SerializeField] private Button _quitPopupBtn;
        [SerializeField] private Image _noMusicIcon;
        [SerializeField] private Image _musicIcon;
        [SerializeField] private Image _noSFXIcon;
        [SerializeField] private Image _sfxIcon;


        [Header("Show Animation Settings")]
        [SerializeField] private float _showDuration = 0.5f;
        [SerializeField] private Ease _showEase = Ease.OutBack;

        [Header("Hide Animation Settings")]
        [SerializeField] private float _hideDuration = 0.5f;
        [SerializeField] private Ease _hideEase = Ease.InBack;

        [SerializeField] private RectTransform _container;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Vector2 _startPositionOffset = new Vector3(0, -100f);
        private Vector2 _endPosition;

        [SerializeField] private EventTrigger _sfxSliderPointerUp;
        [SerializeField] private EventTrigger _musicSliderPointerUp;


        public override void Init()
        {
            base.Init();

            _backgroundMusicSlider.onValueChanged.AddListener(OnBGMValueChanged);
            _sfxSlider.onValueChanged.AddListener(OnSFXValueChanged);
            _quitBtn.onClick.AddListener(OnQuitBtnClicked);
            _aboutMeBtn.onClick.AddListener(OnAboutMeBtnClicked);
            _quitPopupBtn.onClick.AddListener(OnQuitPopupBtnClicked);
            InitSliderEvent();
            _endPosition = Vector2.zero;
            _container.anchoredPosition += _startPositionOffset;

            _canvasGroup.alpha = 0;
        }

        public override void Show(object data)
        {
            base.Show(data);
            ShowAnimation();
            Time.timeScale = 0f;

            _backgroundMusicSlider.value = AudioManager.Instance.MusicVolume;
            _sfxSlider.value = AudioManager.Instance.SfxVolume;
            SetSliderIcon();
        }

        public override void Hide()
        {
            HideAnimation(() => {
                Time.timeScale = 1f;
                base.Hide();
            });
        }


        private void InitSliderEvent()
        {
            {
                EventTrigger.Entry entry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerUp
                };

                entry.callback.AddListener((eventData) => {
                    AudioManager.Instance.SetSFXVolume(_sfxSlider.value);
                    AudioManager.Instance.PlaySFX(Constants.AUDIO_HIT_BLOCK_2);
                    SetSliderIcon();
                });
                _sfxSliderPointerUp.triggers.Add(entry);
            }


            {
                EventTrigger.Entry musicEntry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerUp
                };

                musicEntry.callback.AddListener((eventData) => {
                    AudioManager.Instance.SetMusicVolume(_backgroundMusicSlider.value);
                    SetSliderIcon();
                });
                _musicSliderPointerUp.triggers.Add(musicEntry);
            }
        }


        private void SetSliderIcon()
        {
            if (_sfxSlider.value > 0)
            {
                _sfxIcon.gameObject.SetActive(true);
                _noSFXIcon.gameObject.SetActive(false);
            }
            else
            {
                _sfxIcon.gameObject.SetActive(false);
                _noSFXIcon.gameObject.SetActive(true);
            }

            if (_backgroundMusicSlider.value > 0)
            {
                _musicIcon.gameObject.SetActive(true);
                _noMusicIcon.gameObject.SetActive(false);
            }
            else
            {
                _musicIcon.gameObject.SetActive(false);
                _noMusicIcon.gameObject.SetActive(true);
            }
        }

        
        private void ShowAnimation()
        {
            _canvasGroup.DOFade(1, _showDuration).SetUpdate(true);
            _container.DOAnchorPos(_endPosition, _showDuration)
                .SetEase(_showEase).SetUpdate(true);
        }

        
        private void HideAnimation(TweenCallback onComplete = null)
        {
            _canvasGroup.DOFade(0, _hideDuration).SetUpdate(true);
            _container.DOAnchorPos(_endPosition + _startPositionOffset, _hideDuration)
                .SetEase(_hideEase)
                .OnComplete(onComplete).SetUpdate(true);
        }

        private void OnBGMValueChanged(float value)
        {
            Debug.Log("BGM Volume: " + value);
        }

        private void OnSFXValueChanged(float value)
        {
            Debug.Log("SFX Volume: " + value);
        }

        
        private void OnQuitBtnClicked()
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            Application.Quit();
        }

        
        private void OnAboutMeBtnClicked()
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            Application.OpenURL("https://github.com/Nash-equilbrm");
        }

        
        private void OnQuitPopupBtnClicked()
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            Hide();
        }
    }
}

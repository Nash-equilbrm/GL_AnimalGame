using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Patterns;
using Game.Audio;
using Game.Config;

namespace Game.UI
{
    public class MainMenuScreen : BaseScreen
    {
        [SerializeField] private Button _playBtn;
        [SerializeField] private Button _settingBtn;

        [Header("Show Animation Settings")]
        [SerializeField] private float _showDuration = 0.5f;
        [SerializeField] private Ease _showEase = Ease.OutBack;

        [Header("Hide Animation Settings")]
        [SerializeField] private float _hideDuration = 0.5f;
        [SerializeField] private Ease _hideEase = Ease.InBack;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _endPosition;

        private bool _isFirstTime = true;

        public override void Init()
        {
            base.Init();
            _playBtn.onClick.AddListener(OnPlayBtnClicked);
            _settingBtn.onClick.AddListener(OnSettingBtnClicked);

            _startPosition = Vector3.up * Screen.height;
            _endPosition = Vector3.zero;
            _rectTransform.anchoredPosition = Vector2.zero;
        }

        public override void Show(object data)
        {
            base.Show(data);
            if (_isFirstTime)
            {
                _isFirstTime = false;
                return;
            }

            ShowAnimation();
        }

        public override void Hide()
        {
            HideAnimation(base.Hide);
        }

        public void HideDelayForCallback(TweenCallback onComplete = null)
        {
            HideAnimation(() => {
                base.Hide();
                onComplete?.Invoke();
            });
        }

        
        private void ShowAnimation()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
                _startPosition = _rectTransform.anchoredPosition;
            }
            _rectTransform.anchoredPosition = _startPosition;
            _rectTransform.DOAnchorPos(_endPosition, _showDuration)
                .SetEase(_showEase);
        }

        
        private void HideAnimation(TweenCallback onComplete = null)
        {
            _rectTransform.DOAnchorPos(_startPosition, _hideDuration)
                .SetEase(_hideEase)
                .OnComplete(() => onComplete?.Invoke());
        }

        
        private void OnPlayBtnClicked()
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            HideDelayForCallback(() =>
            {
                this.PubSubBroadcast(EventID.OnPlayBtnClicked);
            });
        }

        
        private void OnSettingBtnClicked()
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            UIManager.Instance.ShowPopup<SettingPopup>(forceShowData: true);
        }

       
    }
}

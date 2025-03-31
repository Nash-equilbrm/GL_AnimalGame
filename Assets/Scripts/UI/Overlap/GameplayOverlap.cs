using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Game.Config;
using Patterns;
using TMPro;
using Commons;
using Game.Audio;
using System;

namespace Game.UI
{
    public class GameplayOverlap : BaseOverlap
    {
        [SerializeField] private Button _backToMenuBtn;
        [SerializeField] private Button _settingBtn;
        [SerializeField] private RectTransform _container;
        [SerializeField] private RectTransform _healthBar;
        [SerializeField] private GameObject _healthIconTemplate;
        [SerializeField] private RedDotController _redDotController;

        [Header("Show Animation Settings")]
        [SerializeField] private float _showDuration = 0.5f;
        [SerializeField] private Ease _showEase = Ease.OutBack;
        [SerializeField] private float _healthFadeDelay = 0.1f;

        [Header("Hide Animation Settings")]
        [SerializeField] private float _hideDuration = 0.5f;
        [SerializeField] private Ease _hideEase = Ease.InBack;

        [Header("Take Damage Animation")]
        [SerializeField] private float _damageFadeDuration = 0.3f;

        private List<Image> _healthIcons = new List<Image>();
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _endPosition;

        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private TMP_Text _tutorialText;


        private bool _showTutorial = false;


        public override void Init()
        {
            base.Init();

            GameManager.Instance.GameStopwatch.OnTimeUpdated += (t) =>
            {
                _timerText.text = Common.FormatTime(t);
            };

            _backToMenuBtn.onClick.AddListener(OnBackToMenuBtnClicked);
            _settingBtn.onClick.AddListener(OnSettingBtnClicked);
            _container.anchoredPosition = _startPosition;

            _healthIcons.Clear();
            for (int i = 0; i < Constants.MAX_HEALTH; i++)
            {
                GameObject healthIcon = Instantiate(_healthIconTemplate, _healthBar);
                healthIcon.SetActive(true);
                Image iconImage = healthIcon.GetComponent<Image>();
                iconImage.color = new Color(1, 1, 1, 0);
                _healthIcons.Add(iconImage);
            }


        }

        public override void Show(object data)
        {
            base.Show(data);
            RegisterEvents();
            ShowAnimation();
            _redDotController.OnGameOverlapInit();
            if (!_showTutorial)
            {
                ShowTutorial();
                _showTutorial = true;
            }
        }

        public override void Hide()
        {
            UnregisterEvents();
            HideAnimation(base.Hide);
        }

        private void RegisterEvents()
        {
            this.PubSubRegister(EventID.OnTakeDamage, OnTakeDamage);
        }

        private void UnregisterEvents()
        {
            this.PubSubUnregister(EventID.OnTakeDamage, OnTakeDamage);
        }

        
        private void ShowAnimation()
        {
            _container.DOAnchorPos(_endPosition, _showDuration)
                .SetEase(_showEase);

            for (int i = 0; i < _healthIcons.Count; i++)
            {
                _healthIcons[i].DOFade(1, _showDuration * 0.5f)
                    .SetDelay(i * _healthFadeDelay);
            }
        }

        
        private void HideAnimation(TweenCallback onComplete = null)
        {
            _container.DOAnchorPos(_startPosition, _hideDuration)
                .SetEase(_hideEase);

            for (int i = _healthIcons.Count - 1; i >= 0; i--)
            {
                _healthIcons[i].DOFade(0, _hideDuration * 0.5f)
                    .SetDelay((_healthIcons.Count - 1 - i) * _healthFadeDelay);
            }

            DOVirtual.DelayedCall(_hideDuration, onComplete);
        }


        
        public void OnTakeDamage(object data = null)
        {
            for (int i = _healthIcons.Count - 1; i >= 0; i--)
            {
                if (_healthIcons[i].color.a == 1)
                {
                    _healthIcons[i].DOFade(0, _damageFadeDuration);
                    return;
                }
            }
        }

        
        private void OnBackToMenuBtnClicked()
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            this.PubSubBroadcast(EventID.OnBackToMenuClicked);
        }

        
        private void OnSettingBtnClicked()
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            UIManager.Instance.ShowPopup<SettingPopup>(forceShowData: true);
        }

        private void ShowTutorial()
        {
            DOTween.Sequence()
                .Append(_tutorialText.DOFade(1f, 1f).SetEase(Ease.OutSine))
                .AppendInterval(1.5f)
                .Append(_tutorialText.DOFade(0f, 1f).SetEase(Ease.InSine));
        }
    }
}

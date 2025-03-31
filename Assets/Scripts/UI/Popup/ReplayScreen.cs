using Commons;
using DG.Tweening;
using Game.Audio;
using Game.Config;
using Patterns;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Game.UI
{
    public class ReplayScreen : BaseScreen
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private CanvasGroup _panelCanvasGroup;
        [SerializeField] private Button _replayBtn;
        [SerializeField] private Button _backToMenuBtn;

        [SerializeField] private GameObject _deadTitle;
        [SerializeField] private GameObject _deadParticle;
        [SerializeField] private GameObject _deadText;

        [SerializeField] private GameObject _finishLevelTitle;
        [SerializeField] private GameObject _finishParticle;
        [SerializeField] private GameObject _finishText;

        [SerializeField] private RectTransform _buttonsGroup;


        [SerializeField] private TMP_Text _totalTime;

        private bool _isPlayerDead = false;
        public override void Hide()
        {
            DoHideAnim(() =>
            {
                base.Hide();
                _deadTitle.SetActive(false);
                _deadParticle.SetActive(false);
                _deadText.SetActive(false);
                _finishLevelTitle.SetActive(false);
                _finishParticle.SetActive(false);
                _finishText.SetActive(false);
            });
        }

        public override void Init()
        {
            base.Init();
            _replayBtn.onClick.AddListener(() => OnReplayBtnClicked());
            _backToMenuBtn.onClick.AddListener(() => OnBackToMainMenuBtnClicked());
        }

        public override void Show(object data)
        {
            base.Show(data);
            if(data is bool isPlayerDead)
            {
                _isPlayerDead = isPlayerDead;
            }
            AudioManager.Instance.StopMusic();
            if (_isPlayerDead)
            {
                AudioManager.Instance.PlaySFX(Constants.AUDIO_LOSE);
            }
            else
            {
                AudioManager.Instance.PlaySFX(Constants.AUDIO_WIN);
            }
            _totalTime.text = $"Total run {Common.FormatTime(GameManager.Instance.GameStopwatch.GetElapsedTime())}";
            DoShowAnim();
        }

        private void DoShowAnim(Action callback = null)
        {
            if (_isPlayerDead)
            {
                _deadTitle.SetActive(true);
                _deadParticle.SetActive(true);
                _deadText.SetActive(true);
            }
            else
            {
                _finishLevelTitle.SetActive(true);
                _finishParticle.SetActive(true);
                _finishText.SetActive(true);
            }
            Canvas.ForceUpdateCanvases();

            DOTween.Sequence()
                .Join(_panel.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.InOutExpo))
                .Join((_buttonsGroup.transform as RectTransform).DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.InOutExpo))
                .Join(_panelCanvasGroup.DOFade(1, 1f).SetEase(Ease.InOutExpo))
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }


        private void DoHideAnim(Action callback = null)
        {
            DOTween.Sequence()
                .Join(_panel.DOAnchorPos(new Vector2(0f, -200f), 1f).SetEase(Ease.InOutExpo))
                .Join((_buttonsGroup.transform as RectTransform).DOAnchorPos(new Vector2(0f, -60f), 1f).SetEase(Ease.InOutExpo))
                .Join(_panelCanvasGroup.DOFade(0, 1f).SetEase(Ease.InOutExpo))
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }


        public void OnReplayBtnClicked()
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            this.PubSubBroadcast(EventID.OnReplayBtnClicked);
        }

       

        public void OnBackToMainMenuBtnClicked()
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            this.PubSubBroadcast(EventID.OnBackToMenuClicked);
        }
    }

}

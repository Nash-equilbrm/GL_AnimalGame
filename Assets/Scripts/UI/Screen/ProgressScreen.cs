using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Patterns;
using Game.Audio;
using Game.Config;

namespace Game.UI
{
    public class ProgressScreen : BaseScreen
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private RectTransform _line;
        [SerializeField] private RectTransform _circleContainer;
        [SerializeField] private GameObject _circleTemplate;
        [SerializeField] private Image _progressIcon;

        [Header("Show Animation Settings")]
        [SerializeField] private float _showDuration = 0.5f;
        [SerializeField] private Ease _showEase = Ease.OutBack;

        [Header("Hide Animation Settings")]
        [SerializeField] private float _hideDuration = 0.5f;
        [SerializeField] private Ease _hideEase = Ease.InBack;

        [Header("Line Settings")]
        [SerializeField] private float _lineExpandDuration = 0.5f;
        [SerializeField] private float _lineWidth = 400f;

        [Header("Circle Settings")]
        [SerializeField] private float _circleScaleDuration = 0.3f;
        [SerializeField] private float _circleDelay = 0.1f;
        [SerializeField] private float _circleSize = 50f;

        [Header("Progress Icon Animation")]
        [SerializeField] private float _progressMoveDuration = 1.0f;
        [SerializeField] private float _progressIconOffsetY = 100f;

        private List<RectTransform> _circles = new List<RectTransform>();
        [SerializeField] private Vector3 _startPosition;
        [SerializeField] private Vector3 _endPosition;
        [SerializeField] private int _progress;

        public override void Init()
        {
            base.Init();
            _startPosition = new Vector3(0, -Screen.height, 0);
            _endPosition = Vector3.zero;
            _container.anchoredPosition = _startPosition;
        }

        public override void Show(object data)
        {
            base.Show(data);
            if (data is not int progress)
            {
                _progress = 0;
            }
            else
            {
                _progress = progress;
            }

            SetupCircles();
            ShowAnimation();
            //DOTween.Sequence().AppendInterval(Constants.INTERVAL_BETWEEN_LEVELS).OnComplete(() =>
            //{
            //    Hide();
            //});


            this.PubSubRegister(EventID.OnInitLevel, OnInitLevel);
        }

        public override void Hide()
        {
            HideAnimation(() => {
                base.Hide();
            });

            this.PubSubUnregister(EventID.OnInitLevel, OnInitLevel);
        }

        private void OnInitLevel(object obj)
        {
            Hide();
        }

        private void SetupCircles()
        {
            foreach (var circle in _circles)
            {
                Destroy(circle.gameObject);
            }
            _circles.Clear();

            int levelCount = GameManager.Instance.LevelConfigs.levels.Count;
            _line.sizeDelta = new Vector2(0, _line.sizeDelta.y);

            if (levelCount <= 1) return;

            float spacing = (_lineWidth - _circleSize) / Mathf.Max(1, levelCount - 1);

            for (int i = 0; i < levelCount; i++)
            {
                GameObject circleObj = Instantiate(_circleTemplate, _circleContainer);
                circleObj.SetActive(true);
                RectTransform circleTransform = circleObj.GetComponent<RectTransform>();
                circleTransform.localScale = Vector3.zero;
                circleTransform.anchoredPosition = new Vector2(i * spacing - _lineWidth / 2 + _circleSize / 2, 0);
                _circles.Add(circleTransform);

                if (i == levelCount - 1) // Circle cuối cùng có Image con
                {
                    Transform finalImage = circleTransform.GetChild(0);
                    finalImage.localScale = Vector3.zero;
                }
            }

            _progressIcon.rectTransform.localScale = Vector3.zero;
        }


        
        private void ShowAnimation()
        {
            _container.DOAnchorPos(_endPosition, _showDuration).SetEase(_showEase);

            _line.DOSizeDelta(new Vector2(_lineWidth, _line.sizeDelta.y), _lineExpandDuration);

            for (int i = 0; i < _circles.Count; i++)
            {
                _circles[i].DOScale(1, _circleScaleDuration)
                    .SetDelay(i * _circleDelay);

                if (i == _circles.Count - 1) // Circle cuối cùng có Image con
                {
                    Transform finalImage = _circles[i].GetChild(0);
                    finalImage.DOScale(1, _circleScaleDuration)
                        .SetDelay((_circles.Count - 1) * _circleDelay + _circleScaleDuration);
                }
            }

            int prevNode = Mathf.Max(0, _progress - 1);
            int currentNode = Mathf.Min(_progress, _circles.Count - 1);
            Vector2 startPos = _circles[prevNode].anchoredPosition + new Vector2(0, _progressIconOffsetY);
            Vector2 endPos = _circles[currentNode].anchoredPosition + new Vector2(0, _progressIconOffsetY);

            _progressIcon.rectTransform.anchoredPosition = startPos;
            _progressIcon.rectTransform.DOScale(1, _circleScaleDuration)
                .OnComplete(() =>
                {
                    _progressIcon.rectTransform.DOAnchorPos(endPos, _progressMoveDuration);
                });
        }


        
        private void HideAnimation(TweenCallback onComplete = null)
        {
            _container.DOAnchorPos(_startPosition, _hideDuration).SetEase(_hideEase);
            _line.DOSizeDelta(new Vector2(0, _line.sizeDelta.y), _hideDuration);

            for (int i = _circles.Count - 1; i >= 0; i--)
            {
                _circles[i].DOScale(0, _circleScaleDuration)
                    .SetDelay((_circles.Count - 1 - i) * _circleDelay);
            }

            _progressIcon.rectTransform.DOScale(0, _circleScaleDuration);
            DOVirtual.DelayedCall(_hideDuration, onComplete);
        }
    }
}

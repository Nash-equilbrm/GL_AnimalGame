using DG.Tweening;
using Game.Audio;
using Game.Config;
using System;
using UnityEngine;


namespace Game.Characters
{
    public class CharacterFX : MonoBehaviour
    {
        [SerializeField] private Character _controller;
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _takeDamageVFX;
        [SerializeField] private ParticleSystem _heartBrokenVFX;
        [SerializeField] private AudioSource _audioSource;

        [Header("Reach check point animation")]
        public float moveHeight = 2f;
        public float moveDuration = 2f;
        public float rotateDuration = 1f;
        public float scaleDuration = .6f;

        internal void PlayAnimation(float speedSqr)
        {
            float state = speedSqr/ Constants.MAX_SPEED_ANIMATE;
            _animator.SetFloat("State", state);
        }


        internal void OnTakeDamage(bool playAudio = false)
        {
            _takeDamageVFX.gameObject.SetActive(true);
            AudioManager.Instance.PlaySFX(Constants.AUDIO_TAKE_DAMAGE, _audioSource);
            if (_controller.IsPlayer)
            {
                DOTween.Sequence().AppendInterval(.5f).OnComplete(() =>
                {
                    _heartBrokenVFX.gameObject.SetActive(true);
                });
            }
        }

        internal void OnReachCheckPoint()
        {
            var startPosition = transform.position;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMoveY(startPosition.y + moveHeight / 2, moveDuration / 2).SetEase(Ease.OutQuad));
            sequence.Join(transform.DORotate(new Vector3(0, 360, 0), rotateDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear));
            sequence.Append(transform.DOMoveY(startPosition.y + moveHeight, moveDuration / 2).SetEase(Ease.InQuad));
            sequence.Append(transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InOutQuad));
            sequence.Join(transform.DOMoveY(startPosition.y, scaleDuration).SetEase(Ease.InOutQuad));
            sequence.OnComplete(() =>
            {
                transform.localScale = Vector3.one;
            });

            sequence.Play();
            AudioManager.Instance.PlaySFX(Constants.AUDIO_CHECKPOINT);
        }

        internal void OnInitLevel()
        {
            var startPosition = transform.position;
            transform.position = startPosition;
            transform.localScale = Vector3.zero;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutQuad));
            sequence.Join(transform.DOMoveY(startPosition.y + moveHeight / 2, scaleDuration).SetEase(Ease.OutQuad));
            sequence.Append(transform.DORotate(new Vector3(0, 360, 0), rotateDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear));
            sequence.Append(transform.DOMoveY(startPosition.y + moveHeight, moveDuration / 2).SetEase(Ease.InQuad));
            sequence.Append(transform.DOMoveY(startPosition.y, moveDuration / 2).SetEase(Ease.InOutQuad));
            sequence.Play();
        }

        internal void OnHitEnemy()
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_HIT_ENEMY);
        }
    }

}

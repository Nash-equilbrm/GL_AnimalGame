using Commons;
using DG.Tweening;
using Game.Audio;
using Game.Config;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


namespace Game.Map
{
    public class Block : MonoBehaviour
    {
     
        public float shakeDuration = 0.2f;
        public float shakeStrength = 0.2f;
        public float squeezeScale = .5f;
        public float squeezeDuration = 0.2f;
        public float bounceHeight = 2f;
        public float bounceDuration = 0.5f;
        [SerializeField] private Collider _collider;
        
        private void Awake()
        {
            gameObject.tag = Constants.BLOCK_TILE_TAG;
        }


        private void OnEnable()
        {
            _collider.enabled = true;
            this.PubSubRegister(EventID.OnCleanupLevel, OnCleanupLevel);
        }

        private void OnDisable()
        {
            _collider.enabled = true;

            this.PubSubUnregister(EventID.OnCleanupLevel, OnCleanupLevel);
        }

        private void OnCleanupLevel(object obj)
        {
            _collider.enabled = false;
            Vector3 originalScale = transform.localScale;
            Vector3 originalPosition = transform.position;

            var squeezeTo = transform.localScale;
            squeezeTo.y *= squeezeScale;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOScale(squeezeTo, squeezeDuration));
            sequence.Append(transform.DOMoveY(transform.position.y + bounceHeight, bounceDuration)
                .SetEase(Ease.OutQuad));
            sequence.Append(transform.DOScale(Vector3.zero, 0.3f));

            sequence.OnComplete(() =>
            {
                transform.localScale = originalScale;
                transform.position = originalPosition;

                ObjectPooling.RemoveToInactive(gameObject);
            });

            sequence.Play();
        }

        public void OnHitTarget()
        {
            _collider.enabled = false;
            Vector3 originalScale = transform.localScale;
            Vector3 originalPosition = transform.position;

            var squeezeTo = transform.localScale;
            squeezeTo.y *= squeezeScale;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOShakePosition(shakeDuration, shakeStrength));
            sequence.Append(transform.DOScale(squeezeTo, squeezeDuration));
            sequence.Append(transform.DOMoveY(transform.position.y + bounceHeight, bounceDuration)
                .SetEase(Ease.OutQuad));
            sequence.Append(transform.DOScale(Vector3.zero, 0.3f));

            sequence.OnComplete(() =>
            {
                transform.localScale = originalScale;
                transform.position = originalPosition;

                ObjectPooling.RemoveToInactive(gameObject);
            });

            sequence.Play();

            AudioManager.Instance.PlaySFX(Common.GetRandomItem(new string[] { Constants.AUDIO_HIT_BLOCK_2, Constants.AUDIO_HIT_BLOCK_1 }));
        }


       

    }

}

using Patterns;
using UnityEngine;
using DG.Tweening;
using System;
using Commons;

namespace Game.Characters
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private string _characterName;

        [SerializeField] private CharacterMovement _movement;
        [SerializeField] private CharacterCollision _collision;
        [SerializeField] private CharacterFX _fxController;

        public CharacterMovement Movement { get => _movement; }
        public CharacterCollision Collision { get => _collision; }
        public CharacterFX FxController { get => _fxController; }

        public bool IsPlayer { get; set; } = false;

        private void Awake()
        {
            _movement ??= GetComponent<CharacterMovement>();
            _collision ??= GetComponent<CharacterCollision>();
            _collision ??= GetComponent<CharacterCollision>();

            if (Movement) Movement.Controller = this;
            if (Collision && IsPlayer) Collision.Controller = this;
        }

        private void OnDestroy()
        {
            DOTween.Kill(gameObject);
        }


        private void OnEnable()
        {
            this.PubSubRegister(EventID.OnInitLevel, OnInitLevel);
        }

        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnInitLevel, OnInitLevel);
        }

        private void OnInitLevel(object obj)
        {
            FxController.OnInitLevel();
        }

        public void OnResetGameplay()
        {
            LogUtility.ConditionalInfo("Character.OnResetGameplay", "", IsPlayer);
            DOTween.Kill(gameObject);
            if (Movement) Movement.enabled = false;
            if (Collision && IsPlayer) Collision.enabled = false;
        }

        public void OnStartGameplay()
        {
            LogUtility.ConditionalInfo("Character.OnStartGameplay", "", IsPlayer);
            if (Movement) Movement.enabled = true;
            if (Collision && IsPlayer) Collision.enabled = true;
        }

        internal void OnTakeDamage()
        {
            if (Movement) Movement.enabled = false;
            if (Collision && IsPlayer) Collision.enabled = false;
        }

        internal void OnReachCheckPoint()
        {
            if (Movement)
            {
                Movement.enabled = false;
            }
            if (Collision && IsPlayer)
            {
                Collision.enabled = false;
            }
        }
    }
}



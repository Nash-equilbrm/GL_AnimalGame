using Game.Config;
using Patterns;
using System;
using UnityEngine;


namespace Game.Health
{
    public class HealthManager : MonoBehaviour
    {
        [field: SerializeField] public int CurrentHealth { get; set; }


        private void Awake()
        {
            CurrentHealth = Constants.MAX_HEALTH;
        }


        private void OnEnable()
        {
            this.PubSubRegister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubRegister(EventID.OnCleanupLevel, OnCleanupLevel);
            this.PubSubRegister(EventID.OnTakeDamage, OnTakeDamage);
        }

        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubUnregister(EventID.OnCleanupLevel, OnCleanupLevel);
            this.PubSubUnregister(EventID.OnTakeDamage, OnTakeDamage);
        }

        private void OnCleanupLevel(object obj)
        {
            CurrentHealth = Constants.MAX_HEALTH;
        }

        private void OnInitLevel(object obj)
        {
            CurrentHealth = Constants.MAX_HEALTH;
        }

        private void OnTakeDamage(object obj)
        {
            CurrentHealth--;
            if (CurrentHealth == 0)
            {
                OnFinishGame();
            }
            else
            {
                OnResetGameplay();
            }
        }

        private void OnResetGameplay()
        {
            this.PubSubBroadcast(EventID.OnResetGameplay, CurrentHealth);
        }

        private void OnFinishGame()
        {
            this.PubSubBroadcast(EventID.OnFinishGame, CurrentHealth);
        }
    }
}


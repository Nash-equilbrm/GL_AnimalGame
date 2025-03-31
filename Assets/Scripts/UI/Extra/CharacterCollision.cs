using Commons;
using Game.Audio;
using Game.Config;
using Game.Map;
using Patterns;
using UnityEngine;


namespace Game.Characters
{
    public class CharacterCollision : MonoBehaviour
    {
        [field: SerializeField] public Character Controller { get; internal set; }
        [field: SerializeField] public CharacterFX FxController { get; internal set; }
        [SerializeField] private Collider _collider;
        private bool _takingDamage = true;

        private void OnEnable()
        {
            _collider.enabled = true;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (other.CompareTag(Constants.ENEMY_TAG))
            {
                LogUtility.NotificationInfo("CharacterCollision", Constants.ENEMY_TAG);
                OnTakeDamage();
                OnHitEnemy(other);
            }
            else if (other.CompareTag(Constants.BLOCK_TILE_TAG))
            {
                LogUtility.NotificationInfo("CharacterCollision", Constants.BLOCK_TILE_TAG);
                OnTakeDamage();
                if (other.TryGetComponent(out Block block))
                {
                    block.OnHitTarget();
                }

            }
            else if (other.CompareTag(Constants.END_CHECKPOINT_TAG))
            {
                LogUtility.NotificationInfo("CharacterCollision", Constants.END_CHECKPOINT_TAG);
                OnReachCheckPoint();
            }
        }

        private void OnReachCheckPoint()
        {
            this.PubSubBroadcast(EventID.OnReachEndCheckPoint);
            Controller.OnReachCheckPoint();
            FxController.OnReachCheckPoint();
        }

        private void OnTakeDamage()
        {
            _collider.enabled = false;
            Controller.PubSubBroadcast(EventID.OnTakeDamage);
            Controller.OnTakeDamage();
            enabled = false;
            FxController.OnTakeDamage(true);
        }


        private void OnHitEnemy(Collider other)
        {
            FxController.OnHitEnemy();
            if(other.gameObject.TryGetComponent(out CharacterFX fx))
            {
                fx.OnTakeDamage();
            }
        }
    }
}
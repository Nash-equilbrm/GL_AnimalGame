using Commons;
using DG.Tweening;
using Game.Characters;
using Game.Config;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Game.Characters.CharacterFactory;


namespace Game.Map
{
    public class CharacterSpawner : MonoBehaviour
    {
        [SerializeField] private MapController _controller;
        [SerializeField] private Character _playerControlledCharacter;
        [SerializeField] private List<Character> _enemyCharacters;
        public Character PlayerControlledCharacter { get => _playerControlledCharacter; private set => _playerControlledCharacter = value; }
        public List<Character> EnemyCharacters { get => _enemyCharacters; private set => _enemyCharacters = value; }


        internal void SpawnPlayer(CharacterEnum characterType, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            LogUtility.Info("SpawnPlayer", $"{characterType.ToString()}");
            var obj = CharacterFactory.Instance.FactoryCreate(characterType);
            obj.transform.SetPositionAndRotation(position, rotation);
            var eulers = obj.transform.eulerAngles;
            eulers.y = 0f;
            obj.transform.eulerAngles = eulers;
            obj.transform.SetParent(parent);
            
            if(obj.TryGetComponent(out Character c))
            {
                _playerControlledCharacter = c;
                this.PubSubBroadcast(EventID.OnFinishSpawnPlayer);
                c.IsPlayer = true;
            }
        }


        internal void SpawnEnemies(CharacterEnum[] characterType, List<List<Vector3>> enemiesArea = null)
        {
            if (enemiesArea == null)
            {
                this.PubSubBroadcast(EventID.OnFinishSpawnEnemies);
                return;
            }
            ClearEnemyCharacters();

            _enemyCharacters = new List<Character>();
            for (int i = 0; i < enemiesArea.Count; i++)
            {
                List<Vector3> patrolPoints = enemiesArea[i];
                CharacterEnum character = characterType[i];
                if (patrolPoints.Count < 2) continue;

                Vector3 startPos = patrolPoints[0];
                Vector3 patrolTarget = patrolPoints[1];

                // Instantiate enemy
                LogUtility.Info("SpawnEnemies", $"{character.ToString()}");
                var obj = CharacterFactory.Instance.FactoryCreate(character);
                obj.transform.position = startPos;
                obj.transform.SetParent(GameManager.Instance.World);
                if (obj.TryGetComponent(out Character c))
                {
                    _enemyCharacters.Add(c);
                    c.Movement.SetMovementStrategy(new PatrolMovement(patrolPoints, 0.5f));
                    c.IsPlayer = false;
                }
                obj.GetComponent<CharacterCollision>().enabled = false;
                obj.tag = Constants.ENEMY_TAG;
            }


            this.PubSubBroadcast(EventID.OnFinishSpawnEnemies);
        }



        private void ClearEnemyCharacters()
        {
            foreach (var obj in EnemyCharacters)
            {
                if (obj != null)
                {
                    Destroy(obj.gameObject);
                }
            }

            EnemyCharacters.Clear();
        }

        internal void OnCleanupLevel()
        {
            // enemies
            ClearEnemyCharacters();

            // player
            Destroy(PlayerControlledCharacter.gameObject);
            PlayerControlledCharacter = null;
        }


        internal void OnResetGameplay()
        {
            var resetPosition = GameManager.Instance.StartPosition;

            LogUtility.Info("Kill tween in player", DOTween.Kill(PlayerControlledCharacter.transform).ToString());
            var collision = PlayerControlledCharacter.GetComponent<CharacterCollision>();
            var movement = PlayerControlledCharacter.GetComponent<CharacterMovement>();
            collision.enabled = false;
            movement.enabled = false;
            PlayerControlledCharacter.OnResetGameplay();
            PlayerControlledCharacter.transform.DOMove(resetPosition, .5f).SetEase(Ease.OutExpo)
                 .OnComplete(() =>
                 {
                     collision.enabled = true;
                     movement.enabled = true;
                     PlayerControlledCharacter.transform.position = resetPosition;
                 });
        }

        internal void OnStartGameplay()
        {
            PlayerControlledCharacter.OnStartGameplay();
        }
    }
}


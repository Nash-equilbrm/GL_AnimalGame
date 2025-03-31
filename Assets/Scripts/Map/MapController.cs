using Commons;
using Game.Characters;
using Game.Level;
using Patterns;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Map
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private ProceduralMapGenerator _generator;
        [SerializeField] private CharacterSpawner _characterSpawner;
        [SerializeField] private Transform _mapBlockContainer;
        public Vector3 StartPosition => _generator.StartPosition;
        public Vector3 EndPosition => _generator.EndPosition;
        private LevelConfig _currentConfig;

        public CharacterSpawner CharacterSpawner { get => _characterSpawner; private set => _characterSpawner = value; }

        private void Awake()
        {
            if (_generator == null) _generator = GetComponent<ProceduralMapGenerator>();
        }


        private void OnEnable()
        {
            this.PubSubRegister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubRegister(EventID.OnFinishPlacingSpawn, OnFinishPlacingSpawn);
            this.PubSubRegister(EventID.OnFinishInitAreaForEnemies, OnFinishInitAreaForEnemies);
            this.PubSubRegister(EventID.OnCleanupLevel, OnCleanupLevel);
            this.PubSubRegister(EventID.OnFinishInitRedDot, OnFinishSpawnRedDot);
            this.PubSubRegister(EventID.OnResetGameplay, OnResetGameplay);
            this.PubSubRegister(EventID.OnStartGameplay, OnStartGameplay);
        }

        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubUnregister(EventID.OnFinishPlacingSpawn, OnFinishPlacingSpawn);
            this.PubSubUnregister(EventID.OnFinishInitAreaForEnemies, OnFinishInitAreaForEnemies);
            this.PubSubUnregister(EventID.OnCleanupLevel, OnCleanupLevel);
            this.PubSubUnregister(EventID.OnFinishInitRedDot, OnFinishSpawnRedDot);
            this.PubSubRegister(EventID.OnResetGameplay, OnResetGameplay);
            this.PubSubRegister(EventID.OnStartGameplay, OnStartGameplay);
        }

        private void OnStartGameplay(object obj)
        {
            _characterSpawner.OnStartGameplay();
        }

        private void OnResetGameplay(object obj)
        {
            _characterSpawner.OnResetGameplay();
        }

        private void OnFinishInitAreaForEnemies(object obj)
        {
            if (obj is List<List<Vector3>> genData)
            {
                LogUtility.ValidInfo("MapController.OnFinishInitAreaForEnemies", "has data");
                _characterSpawner.SpawnEnemies(_currentConfig.EnemiesEnum, genData);
            }
            else
            {
                LogUtility.ValidInfo("MapController.OnFinishInitAreaForEnemies", "no data");
                _characterSpawner.SpawnEnemies(_currentConfig.EnemiesEnum);
            }
        }

        private void OnCleanupLevel(object obj)
        {
            CharacterSpawner.OnCleanupLevel();
        }

        private void OnFinishPlacingSpawn(object obj)
        {
            if (obj is not ProceduralMapGenerator mapGenerator) return;
            Vector3 position = mapGenerator.StartPosition;
            Vector3 direction = mapGenerator.EndPosition - mapGenerator.StartPosition;
            Quaternion rotation = Quaternion.LookRotation(direction);
            _characterSpawner.SpawnPlayer(_currentConfig.PlayerEnum, position, rotation, GameManager.Instance.World);
        }

        private void OnInitLevel(object obj)
        {
            if (obj is not LevelConfig config) return;
            _currentConfig = config;
            _generator.GenerateMap(config, _mapBlockContainer);
        }

        private void OnFinishSpawnRedDot(object obj)
        {
            if (obj is not Transform movementController) return;
            CharacterSpawner.PlayerControlledCharacter.Movement.SetMovementStrategy(new FollowRedDotMovement(movementController, 0.5f));
        }
    }
}
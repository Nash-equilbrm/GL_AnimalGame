using Commons;
using Game.Audio;
using Game.Characters;
using Game.Config;
using Game.Map;
using Game.UI;
using Patterns;
using System;
using System.Collections.Generic;


namespace  Game.States
{
    /// <summary>
    /// TODO: Init new level
    /// </summary>
    public class InitLevelState : State<GameManager>
    {
        private bool _objectInitialized = false;
        private bool _finishPlacingSpawns = false;
        private bool _finishSpawnPlayer = false;
        private bool _finishSpawnEnemy = false;
        private bool _validated = false;
        public InitLevelState(GameManager context) : base(context)
        {
        }

        public InitLevelState(GameManager context, string name) : base(context, name)
        {
        }

        public override void Enter()
        {
            base.Enter();

            if(_context.CurrentConfigIndex == 0)
            {
                _context.GameStopwatch.ResetTimer();
            }
            SetUpGameObjects();
            _context.PubSubRegister(EventID.OnFinishPlacingSpawn, OnFinishPlacingSpawn);
            _context.PubSubRegister(EventID.OnFinishSpawnPlayer, OnFinishSpawnPlayer);
            _context.PubSubRegister(EventID.OnFinishSpawnEnemies, OnFinishSpawnEnemies);


            _context.PubSubBroadcast(EventID.OnInitLevel, _context.CurrentConfig);
        }

        public override void Exit()
        {
            base.Exit();
            _context.PubSubUnregister(EventID.OnFinishPlacingSpawn, OnFinishPlacingSpawn);
            _context.PubSubUnregister(EventID.OnFinishSpawnPlayer, OnFinishSpawnPlayer);
            _context.PubSubUnregister(EventID.OnFinishSpawnEnemies, OnFinishSpawnEnemies);
            _finishPlacingSpawns = false;
            _finishSpawnPlayer = false;
            _finishSpawnEnemy = false;
            _validated = false;
    }

        private void OnFinishSpawnEnemies(object obj)
        {
            _finishSpawnEnemy = true;
            ValidateInitialization();
        }

        private void OnFinishPlacingSpawn(object obj)
        {
            _finishPlacingSpawns = true;
            ValidateInitialization();
        }

        private void OnFinishSpawnPlayer(object obj)
        {
            _finishSpawnPlayer = true;
            ValidateInitialization();
        }

        private void SetUpGameObjects()
        {
            if (_objectInitialized) return;

            _objectInitialized = true;
            
            foreach (var go in _context.initAfterPubSub)
            {
                if (go.activeSelf) go.SetActive(false); // force onEnable to run again to register events
                go.SetActive(true);
            }

        }


        private void ValidateInitialization()
        {
            if(!_validated && _finishPlacingSpawns && _finishPlacingSpawns && _finishSpawnEnemy)
            {
                _validated = true;
                _context.ChangeToGameplayState();
            }
        }
    }
}


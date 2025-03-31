using Commons;
using Game.Characters;
using Game.Health;
using Game.Level;
using Game.Map;
using Game.States;
using Game.UI;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


namespace Game
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] public Camera uiCamera;

        [SerializeField] internal TextAsset configFile;
        [SerializeField] private LevelConfigList _levelConfigs = new();
        internal LevelConfigList LevelConfigs { get => _levelConfigs; set => _levelConfigs = value; }
        [SerializeField] private int _currentConfigIndex = 0;
        public int CurrentConfigIndex
        {
            get => _currentConfigIndex;
            internal set => _currentConfigIndex = Mathf.Clamp(value, 0, LevelConfigs.levels.Count - 1);
        }
        public LevelConfig CurrentConfig => (LevelConfigs != null 
            && LevelConfigs.levels.Count > CurrentConfigIndex 
            && CurrentConfigIndex >= 0) 
            ? LevelConfigs.levels[CurrentConfigIndex] : default;


        public List<GameObject> initAfterPubSub;
        [field: SerializeField] public HealthManager HealthManager { get; internal set; }
        [field: SerializeField] public RedDotController RedDotController { get; internal set; }


        [field: SerializeField] public MapController MapController { get; internal set; }
        public Vector3 StartPosition => MapController.StartPosition;
        public Vector3 EndPosition => MapController.EndPosition;
        public Character PlayerControlledCharacter => MapController.CharacterSpawner.PlayerControlledCharacter;


        [field: SerializeField] public GameStopwatch GameStopwatch { get; set; }

        #region States
        private StateMachine<GameManager> _stateMachine = new();
        private InitState _initState;
        private MainMenuState _mainMenuState;
        private InitLevelState _initLevelState;
        private GameplayState _gameplayState;
        private CleanupLevelState _cleanupLevelState;
        private CleanupGameState _cleanupGameState;
        #endregion



        [Header("Hierachy")]
        public Transform World;

        public string stateName = "";
        private void Update()
        {
            stateName = _stateMachine.CurrentState.name;
        }       

        private void Start()
        {
            _initState = new(this, "Init State");
            _mainMenuState = new(this, "Main Menu State");
            _initLevelState = new(this, "Init Level State");
            _gameplayState = new(this, "Gameplay State");
            _cleanupLevelState = new(this, "Clean up Level State");
            _cleanupGameState = new(this, "Clean up Game State");

            _stateMachine.Initialize(_initState);
        }

        #region State Change
        internal void ChangeToInitLevelState()
        {
            LogUtility.Info("GameManager.ChangeToInitLevelState");
            _stateMachine.ChangeState(_initLevelState);
        }

        internal void ChangeToGameplayState()
        {
            LogUtility.Info("GameManager.ChangeToGameplayState");
            _stateMachine.ChangeState(_gameplayState);
        }

        internal void ChangeToCleanupLevelState()
        {
            LogUtility.Info("GameManager.ChangeToCleanupLevelState");
            _stateMachine.ChangeState(_cleanupLevelState);
        }

        internal void ChangeToCleanupGameState()
        {
            LogUtility.Info("GameManager.ChangeToCleanupGameState");
            _stateMachine.ChangeState(_cleanupGameState);
        }

        internal void ChangeToMainMenuState()
        {
            LogUtility.Info("GameManager.ChangeToMainMenuState");
            _stateMachine.ChangeState(_mainMenuState);
        }
        #endregion

    }

}

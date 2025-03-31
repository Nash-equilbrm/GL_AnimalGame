using Commons;
using DG.Tweening;
using Game.Audio;
using Game.Characters;
using Game.Config;
using Game.UI;
using Patterns;
using System.Collections;
using UnityEngine;


namespace Game.States
{
    public class GameplayState : State<GameManager>
    {
        public GameplayState(GameManager context) : base(context)
        {
        }

        public GameplayState(GameManager context, string name) : base(context, name)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _context.PubSubRegister(EventID.OnResetGameplay, OnResetGameplay);
            _context.PubSubRegister(EventID.OnFinishGame, OnFinishGame);
            _context.PubSubRegister(EventID.OnReplayBtnClicked, OnReplayBtnClicked);
            _context.PubSubRegister(EventID.OnReachEndCheckPoint, OnReachEndCheckPoint);
            _context.PubSubRegister(EventID.OnBackToMenuClicked, OnBackToMenuClicked);
            _context.PubSubRegister(EventID.OnContinueBtnClicked, OnContinueBtnClicked);

            UIManager.Instance.ShowOverlap<GameplayOverlap>(_context.CurrentConfig, forceShowData: true);
            DOTween.Sequence().AppendInterval(Constants.INTERVAL_BETWEEN_LEVELS / 2)
                .OnComplete(() =>
                {
                    _context.GameStopwatch.StartTimer();
                    AudioManager.Instance.PlayMusic(Constants.AUDIO_BGM_GAMEPLAY);
                    _context.PubSubBroadcast(EventID.OnStartGameplay);
                });
        }

        public override void Exit()
        {
            base.Exit();
            _context.PubSubUnregister(EventID.OnResetGameplay, OnResetGameplay);
            _context.PubSubUnregister(EventID.OnFinishGame, OnFinishGame);
            _context.PubSubUnregister(EventID.OnReplayBtnClicked, OnReplayBtnClicked);
            _context.PubSubUnregister(EventID.OnReachEndCheckPoint, OnReachEndCheckPoint);
            _context.PubSubUnregister(EventID.OnBackToMenuClicked, OnBackToMenuClicked);
            _context.PubSubUnregister(EventID.OnContinueBtnClicked, OnContinueBtnClicked);

        }

        private void OnResetGameplay(object obj)
        {
            _context.StartCoroutine(IEWaitForResetGameplay());
        }

        private IEnumerator IEWaitForResetGameplay()
        {
            yield return new WaitUntil(() =>
            {
                return (_context.PlayerControlledCharacter.transform.position - _context.StartPosition).magnitude < .1f;
            });
            AudioManager.Instance.PlayMusic(Constants.AUDIO_BGM_GAMEPLAY);
            _context.PubSubBroadcast(EventID.OnStartGameplay);
            _context.GameStopwatch.StartTimer();
        }

        private void OnReachEndCheckPoint(object obj)
        {
            CheckForNextLevel();
        }


        private void OnFinishGame(object obj = null)
        {
            _context.GameStopwatch.PauseTimer();
            UIManager.Instance.HideAllOverlaps();
            bool isPlayerDead = false;
            if(obj is int currentHealth && currentHealth == 0)
            {
                isPlayerDead = true;
            }

            UIManager.Instance.ShowScreen<ReplayScreen>(data: isPlayerDead, forceShowData: true);
        }

        private void CheckForNextLevel()
        {
            var totalLevel = _context.LevelConfigs.levels.Count;
            LogUtility.NotificationInfo("CheckForNextLevel", $"Go to next: {_context.CurrentConfigIndex < totalLevel - 1}");
            if (_context.CurrentConfigIndex < totalLevel - 1) 
            {
                GoToNextLevel();
                return;
            }
            OnFinishGame();
        }

        private void GoToNextLevel()
        {
            _context.CurrentConfigIndex++;
            _context.ChangeToCleanupLevelState();
        }


        private void OnReplayBtnClicked(object obj)
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            _context.CurrentConfigIndex = 0;
            _context.ChangeToCleanupLevelState();
            UIManager.Instance.HideAllPopups();
        }

        private void OnBackToMenuClicked(object obj)
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            _context.CurrentConfigIndex = 0;
            UIManager.Instance.HideAllPopups();
            UIManager.Instance.HideAllScreens();
            UIManager.Instance.HideAllOverlaps();
            _context.ChangeToCleanupGameState();
        }

        private void OnContinueBtnClicked(object obj)
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            _context.PubSubBroadcast(EventID.OnPlayerResetGameplay);
            _context.ChangeToCleanupLevelState();
            UIManager.Instance.HideAllPopups();
            UIManager.Instance.HideAllScreens();

        }
    }

}

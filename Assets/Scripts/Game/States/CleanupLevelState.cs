using DG.Tweening;
using Game.Config;
using Game.UI;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Game.States
{
    public class CleanupLevelState : State<GameManager>
    {
        public CleanupLevelState(GameManager context) : base(context)
        {
        }

        public CleanupLevelState(GameManager context, string name) : base(context, name)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _context.GameStopwatch.PauseTimer();
            UIManager.Instance.HideAllOverlaps();
            UIManager.Instance.ShowScreen<ProgressScreen>(data: GameManager.Instance.CurrentConfigIndex, forceShowData: true);
            DOTween.Sequence()
                .AppendInterval(Constants.INTERVAL_BETWEEN_LEVELS/2)
                .AppendCallback(() => {
                    _context.PubSubBroadcast(EventID.OnCleanupLevel);
                    CleanupContext();
                })
                .AppendInterval(Constants.INTERVAL_BETWEEN_LEVELS / 2)
                .OnComplete(() => {
                    _context.ChangeToInitLevelState();
                });


        }

        public override void Exit()
        {
            base.Exit();
        }

       
        
        private void CleanupContext()
        {
            
        }
    }
}


using DG.Tweening;
using Game.Config;
using Game.UI;
using Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.States
{
    public class CleanupGameState : State<GameManager>
    {
        public CleanupGameState(GameManager context) : base(context)
        {
        }

        public CleanupGameState(GameManager context, string name) : base(context, name)
        {
        }

        public override void Enter()
        {
            base.Enter();

            UIManager.Instance.HideAllOverlaps();

            DOTween.Sequence()
                .AppendInterval(Constants.INTERVAL_BETWEEN_LEVELS / 2)
                .AppendCallback(() => {
                    _context.PubSubBroadcast(EventID.OnCleanupLevel);
                    CleanupContext();
                })
                .AppendInterval(Constants.INTERVAL_BETWEEN_LEVELS / 2)
                .OnComplete(() => {
                    _context.ChangeToMainMenuState();
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


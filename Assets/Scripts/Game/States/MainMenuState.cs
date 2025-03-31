using Game.Audio;
using Game.Config;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.UI
{
    public class MainMenuState : State<GameManager>
    {
        public MainMenuState(GameManager context) : base(context)
        {
        }

        public MainMenuState(GameManager context, string name) : base(context, name)
        {
        }

        public override void Enter()
        {
            base.Enter();
            UIManager.Instance.ShowScreen<MainMenuScreen>(forceShowData: true);

            AudioManager.Instance.PlayMusic(Constants.AUDIO_BGM_MAIN_MENU);

            RegisterEvents();
        }

        public override void Exit()
        {
            base.Exit();
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            _context.PubSubRegister(EventID.OnPlayBtnClicked, OnPlayBtnClicked);
        }

        private void UnregisterEvents()
        {
            _context.PubSubUnregister(EventID.OnPlayBtnClicked, OnPlayBtnClicked);
        }

        private void OnPlayBtnClicked(object obj)
        {
            AudioManager.Instance.PlaySFX(Constants.AUDIO_BTN_CLICK);
            _context.ChangeToInitLevelState();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using Commons;
using DG.Tweening;
using Game.Config;
using Game.Level;
using Patterns;
using UnityEngine;


namespace Game.States
{
    /// <summary>
    /// TODO: Init game specs when loaded
    /// </summary>
    public class InitState : State<GameManager>
    {
        private bool _initialized = false;
        public InitState(GameManager context) : base(context)
        {
        }

        public InitState(GameManager context, string name) : base(context, name)
        {
        }

        public override void Enter()
        {
            base.Enter();
            if (!_initialized)
            {
                _initialized = true;
                DOTween.Init();
                DOTween.SetTweensCapacity(1000, 1000);
                Application.targetFrameRate = 60;
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                LoadLevelConfigs();
                _context.StartCoroutine(IEWaitForSingletons());
                PreparePools();
                _context.CurrentConfigIndex = 0;
            }
            _context.ChangeToMainMenuState();
        }

        public override void Exit()
        {
            base.Exit();
        }

        private void LoadLevelConfigs()
        {
            _context.LevelConfigs = JsonUtility.FromJson<LevelConfigList>(_context.configFile.text);
        }

        private void PreparePools()
        {
            ObjectPooling.Instance.GetPool(Constants.BLOCK_1_POOL_TAG).Prepare(60);
            ObjectPooling.Instance.GetPool(Constants.BLOCK_2_POOL_TAG).Prepare(60);
        }

        private IEnumerator IEWaitForSingletons()
        {
            yield return new WaitUntil(() => PubSub.HasInstance && ObjectPooling.HasInstance);
        }
    }

}

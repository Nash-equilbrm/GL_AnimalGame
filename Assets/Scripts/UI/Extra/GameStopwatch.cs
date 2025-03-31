using UnityEngine;
using System.Diagnostics;
using System;

namespace Game
{
    public class GameStopwatch : MonoBehaviour
    {
        private Stopwatch _stopwatch = new Stopwatch();
        public Action<TimeSpan> OnTimeUpdated;

        private void Start()
        {
            InvokeRepeating(nameof(UpdateTime), 1f, 1f);
        }

        private void UpdateTime()
        {
            if (_stopwatch.IsRunning)
            {
                OnTimeUpdated?.Invoke(_stopwatch.Elapsed);
            }
        }

        public void StartTimer()
        {
            _stopwatch.Start();
            InvokeRepeating(nameof(UpdateTime), 1f, 1f);
        }

        public void PauseTimer()
        {
            _stopwatch.Stop();
        }

        public void ResetTimer()
        {
            _stopwatch.Reset();
            OnTimeUpdated?.Invoke(TimeSpan.Zero);
        }

        public TimeSpan GetElapsedTime()
        {
            return _stopwatch.Elapsed;
        }

    }
}


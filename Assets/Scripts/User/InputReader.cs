using UnityEngine;
using System;
using Patterns;
using Commons;


namespace UserInput
{
    public class InputReader : Singleton<InputReader>
    {
        #region TouchScreen & Mouse Drag n Drop

        public event Action<Vector2> OnDragStart;
        public event Action<Vector2> OnDragging;
        public event Action<Vector2> OnDragEnd;

        private Vector2 _startPosition;
        private bool _isDragging = false;

        void Update()
        {
#if UNITY_IOS || UNITY_ANDROID
        HandleTouchInput();
#else
            HandleMouseInput();
#endif
        }
        
        
        #region Handling inputs
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _startPosition = Input.mousePosition;
                _isDragging = true;
                OnDragStart?.Invoke(_startPosition);
            }
            else if (Input.GetMouseButton(0) && _isDragging)
            {
                OnDragging?.Invoke(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                _isDragging = false;
                OnDragEnd?.Invoke(Input.mousePosition);
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _startPosition = touch.position;
                        _isDragging = true;
                        OnDragStart?.Invoke(_startPosition);
                        break;

                    case TouchPhase.Moved:
                        if (_isDragging)
                        {
                            OnDragging?.Invoke(touch.position);
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (_isDragging)
                        {
                            _isDragging = false;
                            OnDragEnd?.Invoke(touch.position);
                        }
                        break;
                }
            }
        }
        #endregion


        #endregion
    }
}

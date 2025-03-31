using UnityEngine;
using DG.Tweening;
using Patterns;
using UserInput;
using Game.Config;
using Commons;
using Game.Level;

namespace Game.UI
{
    public class RedDotController : MonoBehaviour
    {
        public GameObject redDotPrefab;
        public LayerMask mapTileLayerMask;
        [SerializeField] private Transform _redDot;
        public Transform RedDot { get => _redDot; }

        private Vector2 _lastDragPosition;
        private Vector3 _moveDirection;
        private float _moveSpeed;
        public float speedMultiplier = 0.2f;
        private const float MAX_SPEED = 5f;
        private const float MIN_SPEED = 2f;
        private Tweener _activeTween;

        [SerializeField] private bool _canControl = true;
        private Camera _mainCamera;

        // Lưu trữ giá trị của Time.deltaTime để tránh gọi liên tục
        private float _deltaTime;

        void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            this.PubSubRegister(EventID.OnStartGameplay, OnStartGameplay);
            this.PubSubRegister(EventID.OnResetGameplay, OnResetGameplay);
            this.PubSubRegister(EventID.OnCleanupLevel, OnCleanupLevel);
            this.PubSubRegister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubRegister(EventID.OnTakeDamage, OnTakeDamage);

            InputReader.Instance.OnDragStart += OnDragStart;
            InputReader.Instance.OnDragging += OnDragging;
            InputReader.Instance.OnDragEnd += OnDragEnd;
        }

        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnStartGameplay, OnStartGameplay);
            this.PubSubUnregister(EventID.OnResetGameplay, OnResetGameplay);
            this.PubSubUnregister(EventID.OnCleanupLevel, OnCleanupLevel);
            this.PubSubUnregister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubUnregister(EventID.OnTakeDamage, OnTakeDamage);

            InputReader.Instance.OnDragStart -= OnDragStart;
            InputReader.Instance.OnDragging -= OnDragging;
            InputReader.Instance.OnDragEnd -= OnDragEnd;
        }

        void Update()
        {
            // Cập nhật deltaTime mỗi frame
            _deltaTime = Time.deltaTime;
        }

        private void OnTakeDamage(object obj)
        {
            _canControl = false;
        }

        private void OnInitLevel(object obj)
        {
            _canControl = false;
        }


        private void OnCleanupLevel(object obj)
        {
            _canControl = false;
        }

        private void OnResetGameplay(object obj)
        {
            _canControl = false;
            var originalScale = RedDot.transform.localScale;
            DOTween.Sequence()
                .Append(RedDot.transform.DOScale(Vector3.zero, .3f).SetEase(Ease.Linear))
                .AppendCallback(() => RedDot.transform.position = GameManager.Instance.StartPosition)
                .Append(RedDot.transform.DOScale(originalScale, .3f).SetEase(Ease.Linear));
        }

        private void OnStartGameplay(object obj)
        {
            DOTween.Sequence().AppendInterval(1f)
                .OnComplete(() =>
                {
                    _canControl = true;
                    var config = GameManager.Instance.CurrentConfig;
                    var sz = Mathf.Max(config.mapSize[0], config.mapSize[1]) * Constants.CAMERA_SIZE_REF_FOR_MAP_10X10;
                    DOTween.Sequence().
                    AppendInterval(1f)
                   .Append(RedDot.transform.DOScale(sz / 50f, 1f)).SetEase(Ease.OutExpo);
                });
        }

        internal void OnGameOverlapInit()
        {
            if (RedDot == null)
            {
                _redDot = Instantiate(redDotPrefab).transform;
                GameManager.Instance.RedDotController = this;

            }
            var redDotPosition = GameManager.Instance.StartPosition;
            redDotPosition.y = 0f;
            RedDot.transform.position = redDotPosition;
            this.PubSubBroadcast(EventID.OnFinishInitRedDot, RedDot.transform);
        }

        private void OnDragStart(Vector2 vector)
        {
            if (!_canControl) return;
            _activeTween?.Kill(); // Sử dụng null conditional operator an toàn hơn
            _lastDragPosition = vector;
            _moveSpeed = 0;
        }

        private void OnDragging(Vector2 vector)
        {
            if (!_canControl) return;

            Vector2 dragVector = vector - _lastDragPosition;
            _lastDragPosition = vector;

            // Tính toán tốc độ dựa trên khoảng cách kéo và thời gian
            float distance = dragVector.magnitude;
            _moveSpeed = Mathf.Clamp(distance * speedMultiplier / _deltaTime, MIN_SPEED, MAX_SPEED);
            _moveDirection = CalculateMoveDirection(dragVector);

            // Di chuyển đối tượng ngay lập tức
            Vector3 nextPosition = _redDot.position + _moveDirection * _moveSpeed * _deltaTime;
            MoveRedDot(nextPosition); // Sử dụng hàm riêng để xử lý di chuyển và kiểm tra va chạm
        }

        private void OnDragEnd(Vector2 vector)
        {
            if (!_canControl) return;

            _moveSpeed *= 0.5f; // Giảm tốc độ
            Vector3 finalMove = _redDot.position + _moveDirection * _moveSpeed;

            // Sử dụng MoveRedDot cho cả OnDragging và OnDragEnd để nhất quán
            if (IsValidMove(finalMove))
            {
                _activeTween = _redDot.DOMove(finalMove, 0.5f).SetEase(Ease.OutExpo);
            }
        }

        private Vector3 CalculateMoveDirection(Vector2 dragVector)
        {
            Vector3 forward = _mainCamera.transform.forward;
            Vector3 right = _mainCamera.transform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            Vector3 moveDir = (right * dragVector.x + forward * dragVector.y).normalized;
            return moveDir;
        }

        private void MoveRedDot(Vector3 targetPosition)
        {
            if (IsValidMove(targetPosition))
            {
                _redDot.position = targetPosition;
            }
        }

        private bool IsValidMove(Vector3 targetPosition)
        {
            RaycastHit hit;

            if (Physics.Raycast(targetPosition + Vector3.up * 2f, Vector3.down, out hit, 5f, mapTileLayerMask))
            {
                return hit.collider.CompareTag(Constants.MAP_TILE_TAG);
            }
            return false;
        }
    }
}
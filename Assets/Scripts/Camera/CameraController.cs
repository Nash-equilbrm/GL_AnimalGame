using DG.Tweening;
using Game.Config;
using Game.Level;
using Game.Map;
using Patterns;
using UnityEngine;


namespace Game.CameraControl
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [SerializeField] private float _duration = 1.5f;
        [SerializeField] private Ease _ease = Ease.InOutSine;
        [SerializeField] private float _heightOffset = 10f;
        private GameObject _anchor;


        private void OnEnable()
        {
            this.PubSubRegister(EventID.OnFinishPlacingSpawn, OnFinishPlacingSpawn);
            this.PubSubRegister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubRegister(EventID.OnTakeDamage, OnTakeDamage);
        }

        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnFinishPlacingSpawn, OnFinishPlacingSpawn);
            this.PubSubUnregister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubUnregister(EventID.OnTakeDamage, OnTakeDamage);
        }

        private void OnTakeDamage(object obj)
        {
            Shake();
        }

        private void OnInitLevel(object obj)
        {
            if (obj is not LevelConfig config) return;
            _camera.DOOrthoSize(Mathf.Max(config.mapSize[0], config.mapSize[1]) * Constants.CAMERA_SIZE_REF_FOR_MAP_10X10,
                _duration).SetEase(Ease.OutSine);
        }

        private void OnFinishPlacingSpawn(object obj)
        {
            if (obj is not ProceduralMapGenerator mapGenerator) return;
            var target = new Vector3(mapGenerator.MapSize.x / 2 - .5f, 0f, mapGenerator.MapSize.y / 2 - .5f);
            _heightOffset = mapGenerator.MapSize.x;
            //_distance = mapGenerator.MapSize.x;
            //RotateCameraToTarget(target);
            SetupCamera(target, _heightOffset, _duration);
        }


        //private void RotateCameraToTarget(Vector3 target)
        //{
        //    LogUtility.Info("RotateCameraToTarget", target.ToString());

        //    float fixedYRotation = -45f;
        //    float fixedZRotation = 0f;

        //    Quaternion fixedRotation = Quaternion.Euler(0, fixedYRotation, fixedZRotation);
        //    Vector3 direction = fixedRotation * Vector3.forward;

        //    Vector3 cameraPosition = target - direction * _distance;
        //    cameraPosition.y = _heightOffset;

        //    Vector3 lookDirection = target - cameraPosition;
        //    float rotationX = Mathf.Atan2(-lookDirection.y, lookDirection.magnitude) * Mathf.Rad2Deg;

        //    transform.DOMove(cameraPosition, _duration);

        //    transform.DORotate(new Vector3(rotationX, fixedYRotation, fixedZRotation), _duration);
        //}

        public void Shake(float duration = 0.2f, float strength = 0.2f, int vibrato = 10)
        {
            Transform camTransform = Camera.main.transform;

            camTransform.DOShakePosition(duration, strength, vibrato)
                .SetEase(Ease.OutQuad);
        }

        public void SetupCamera(Vector3 position, float targetHeight, float moveDuration)
        {
            if (_anchor == null) _anchor = new GameObject("CameraAnchor");
            _anchor.transform.position = position;
            _anchor.transform.rotation = Quaternion.Euler(52, -45, 0);

            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;

            mainCamera.transform.SetParent(_anchor.transform);
            mainCamera.transform.localPosition = Vector3.up * .8f;
            mainCamera.transform.localRotation = Quaternion.identity;

            var localPosition = mainCamera.transform.localPosition;
            localPosition.z -= 100;
            mainCamera.transform.localPosition = localPosition;
        }
    }
}


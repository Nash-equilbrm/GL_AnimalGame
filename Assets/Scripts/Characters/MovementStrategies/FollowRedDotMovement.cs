using DG.Tweening;
using UnityEngine;


namespace Game.Characters
{
    public class FollowRedDotMovement : IMovementStrategy
    {
        private Transform _redDot;
        private float _moveDuration;
        private Tween _movementTween;

        public FollowRedDotMovement(Transform redDot, float moveDuration)
        {
            _redDot = redDot;
            _moveDuration = moveDuration;
        }

        public float CurrentSpeed()
        {
            throw new System.NotImplementedException();
        }

        public void Move(Transform animalTransform)
        {
            if (animalTransform == null || animalTransform.gameObject == null) return;
            if (_redDot == null) return;

            var redDotPosition = _redDot.position;
            redDotPosition.y = 0f;

            Vector3 direction = redDotPosition - animalTransform.position;
            direction.y = 0;

            float distance = direction.magnitude;

            if (distance > Mathf.Epsilon)
            {
                float maxSpeed = 10f;
                float minSpeed = 0.1f;

                float t = Mathf.Clamp01(distance / 5f);
                float speedMultiplier = Mathf.Lerp(minSpeed, maxSpeed, t);

                Vector3 newPosition = Vector3.MoveTowards(
                    animalTransform.position,
                    redDotPosition,
                    Time.deltaTime * speedMultiplier * (1f / _moveDuration)
                );
                animalTransform.position = newPosition;

                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                animalTransform.rotation = Quaternion.Slerp(animalTransform.rotation, targetRotation, Time.deltaTime * 5f);
            }
            else
            {
                animalTransform.position = redDotPosition;
            }
        }
    }
}

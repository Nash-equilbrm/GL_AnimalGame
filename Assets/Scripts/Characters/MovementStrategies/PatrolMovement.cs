using System.Collections.Generic;
using UnityEngine;

namespace Game.Characters
{
    public class PatrolMovement : IMovementStrategy
    {
        private List<Vector3> _patrolPoints;
        private int _currentIndex;
        private float _moveSpeed;

        public PatrolMovement(List<Vector3> patrolPoints, float moveSpeed)
        {
            _patrolPoints = patrolPoints;
            _moveSpeed = moveSpeed;
            _currentIndex = 0;
        }

        public float CurrentSpeed()
        {
            throw new System.NotImplementedException();
        }

        public void Move(Transform animalTransform)
        {
            if (animalTransform == null || animalTransform.gameObject == null || _patrolPoints.Count == 0)
            {
                return;
            }

            Vector3 targetPosition = _patrolPoints[_currentIndex];
            Vector3 direction = targetPosition - animalTransform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > Mathf.Epsilon * Mathf.Epsilon)
            {
                animalTransform.position = Vector3.MoveTowards(animalTransform.position, targetPosition, Time.deltaTime * _moveSpeed);

                if (direction.sqrMagnitude > Mathf.Epsilon)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                    animalTransform.rotation = Quaternion.Slerp(animalTransform.rotation, targetRotation, Time.deltaTime * 5f);
                }
            }
            else
            {
                _currentIndex = (_currentIndex + 1) % _patrolPoints.Count;
            }
        }
    }
}

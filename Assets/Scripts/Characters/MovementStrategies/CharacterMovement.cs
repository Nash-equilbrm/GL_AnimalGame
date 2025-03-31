using Commons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Characters
{
    public class CharacterMovement : MonoBehaviour
    {
        [field: SerializeField] public Character Controller { get; internal set; }
        [field: SerializeField] public CharacterFX FxController { get; internal set; }

        private IMovementStrategy _movementStrategy;


        public float Speed { get; private set; }
        private Vector3 _previousPosition;

        private void Start()
        {
            _previousPosition = transform.position;
        }
       
        private void Update()
        {
            _movementStrategy?.Move(transform);

            Speed = (transform.position - _previousPosition).magnitude / Time.deltaTime;
            _previousPosition = transform.position;
            FxController.PlayAnimation(Speed);
            //FxController.PlayAnimation(_movementStrategy.CurrentSpeed());

        }

        public void SetMovementStrategy(IMovementStrategy movementStrategy)
        {
            _movementStrategy = movementStrategy;
        }

    }

}

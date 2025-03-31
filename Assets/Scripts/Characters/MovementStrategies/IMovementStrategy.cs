using UnityEngine;


namespace Game.Characters
{
    public interface IMovementStrategy
    {
        void Move(Transform t);
        float CurrentSpeed();
    }
}


using UnityEngine;

namespace UnSrp2d.Core.Contracts
{
    public readonly struct MovementState
    {
        public Vector2 Velocity { get; }
        public Vector2 Direction { get; }
        public float Speed { get; }
        public bool IsMoving { get; }
        public Vector2 ActualVelocity { get; }
        public bool IsActuallyMoving { get; }

        public MovementState(
            Vector2 velocity,
            Vector2 direction,
            float speed,
            bool isMoving,
            Vector2 actualVelocity,
            bool isActuallyMoving)
        {
            Velocity = velocity;
            Direction = direction;
            Speed = speed;
            IsMoving = isMoving;
            ActualVelocity = actualVelocity;
            IsActuallyMoving = isActuallyMoving;
        }
    }
}

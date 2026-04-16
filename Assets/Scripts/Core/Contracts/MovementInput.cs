using UnityEngine;

namespace UnSrp2d.Core.Contracts
{
    public readonly struct MovementInput
    {
        public readonly Vector2 Direction;
        public readonly float Magnitude;

        public MovementInput(Vector2 direction, float magnitude)
        {
            Direction = direction;
            Magnitude = magnitude;
        }

        public static MovementInput Zero => new(Vector2.zero, 0f);
    }
}
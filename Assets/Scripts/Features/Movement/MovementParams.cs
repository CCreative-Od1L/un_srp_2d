using UnityEngine;

namespace UnSrp2d.Features.Movement
{
    [CreateAssetMenu(fileName = "MovementParams", menuName = "UnSrp2d/Movement/MovementParams")]
    public class MovementParams : ScriptableObject
    {
        public float MaxSpeed = 5f;
        public float Acceleration = 25f;
        public float Deceleration = 20f;
        public float MinMoveSpeed = 0.1f;
    }
}

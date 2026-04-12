using UnSrp2d.Core.Contracts;
using UnityEngine;

namespace UnSrp2d.Features.Movement
{
    public class DefaultMovementParamsProvider : IMovementParamsProvider
    {
        readonly MovementParams _source;

        public DefaultMovementParamsProvider(MovementParams source)
        {
            _source = source;
        }

        public float MaxSpeed => _source.MaxSpeed;
        public float Acceleration => _source.Acceleration;
        public float Deceleration => _source.Deceleration;
        public float MinMoveSpeed => _source.MinMoveSpeed;
    }
}

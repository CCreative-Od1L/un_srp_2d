using UnSrp2d.Core.Contracts;
using UnityEngine;

namespace UnSrp2d.Features.Movement
{
    public class InputMovementStrategy : IMovementStrategy
    {
        readonly IInputProvider _inputProvider;
        readonly IInputConfig _inputConfig;

        public InputMovementStrategy(IInputProvider inputProvider, IInputConfig inputConfig)
        {
            _inputProvider = inputProvider;
            _inputConfig = inputConfig;
        }

        public Vector2 GetDirection()
        {
            var raw = _inputProvider.MoveDirection;

            if (raw.magnitude < _inputConfig.Deadzone)
                return Vector2.zero;

            return raw.normalized;
        }
    }
}

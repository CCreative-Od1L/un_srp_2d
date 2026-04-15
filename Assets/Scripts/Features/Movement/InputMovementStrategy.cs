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

        public MovementInput GetMovementInput()
        {
            var raw = _inputProvider.MoveDirection;
            var magnitude = raw.magnitude;

            if (magnitude < _inputConfig.Deadzone)
                return MovementInput.Zero;

            var remappedMagnitude = Mathf.InverseLerp(_inputConfig.Deadzone, 1f, magnitude);
            return new MovementInput(raw.normalized, remappedMagnitude);
        }
    }
}

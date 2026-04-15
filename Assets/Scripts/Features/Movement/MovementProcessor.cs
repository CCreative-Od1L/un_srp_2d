using UnSrp2d.Core.Contracts;
using UnityEngine;

namespace UnSrp2d.Features.Movement
{
    public class MovementProcessor : IMovementStateProvider
    {
        readonly IMovementStrategy _strategy;
        readonly IMovementParamsProvider _params;

        Vector2 _currentVelocity;
        Vector2 _lastDirection = Vector2.right;

        public MovementState CurrentState { get; private set; }

        public MovementProcessor(IMovementStrategy strategy, IMovementParamsProvider movementParams)
        {
            _strategy = strategy;
            _params = movementParams;
        }

        public MovementState Tick(float deltaTime, Vector2 actualVelocity)
        {
            var input = _strategy.GetMovementInput();
            var targetVelocity = input.Direction * _params.MaxSpeed * input.Magnitude;

            var maxDelta = input.Magnitude > 0f
                ? _params.Acceleration * deltaTime
                : _params.Deceleration * deltaTime;

            _currentVelocity = Vector2.MoveTowards(_currentVelocity, targetVelocity, maxDelta);
            _currentVelocity = Vector2.ClampMagnitude(_currentVelocity, _params.MaxSpeed);

            if (input.Magnitude <= 0f && _currentVelocity.magnitude < _params.MinMoveSpeed)
                _currentVelocity = Vector2.zero;

            if (input.Direction != Vector2.zero)
                _lastDirection = input.Direction;

            var speed = _currentVelocity.magnitude;

            CurrentState = new MovementState(
                _currentVelocity,
                _lastDirection,
                speed,
                speed > _params.MinMoveSpeed,
                actualVelocity,
                actualVelocity.magnitude > _params.MinMoveSpeed
            );

            return CurrentState;
        }
    }
}

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
            var direction = _strategy.GetDirection();
            var targetVelocity = direction * _params.MaxSpeed;

            var maxDelta = direction != Vector2.zero
                ? _params.Acceleration * deltaTime
                : _params.Deceleration * deltaTime;

            _currentVelocity = Vector2.MoveTowards(_currentVelocity, targetVelocity, maxDelta);
            _currentVelocity = Vector2.ClampMagnitude(_currentVelocity, _params.MaxSpeed);

            if (direction == Vector2.zero && _currentVelocity.magnitude < _params.MinMoveSpeed)
                _currentVelocity = Vector2.zero;

            if (direction != Vector2.zero)
                _lastDirection = direction;

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

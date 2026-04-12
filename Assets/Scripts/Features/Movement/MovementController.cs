using UnSrp2d.Core.Contracts;
using UnityEngine;
using VContainer;

namespace UnSrp2d.Features.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementController : MonoBehaviour
    {
        [SerializeField] Rigidbody2D _rb;

        MovementProcessor _processor;

        public IMovementStateProvider StateProvider => _processor;

        [Inject]
        public void Construct(MovementProcessor processor)
        {
            _processor = processor;
        }

        void Awake()
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            if (_processor == null) return;
            
            var actualVelocity = _rb.velocity;
            _processor.Tick(Time.fixedDeltaTime, actualVelocity);
            _rb.velocity = _processor.CurrentState.Velocity;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (_processor == null) return;

            var state = _processor.CurrentState;
            var pos = transform.position;

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(pos, state.Velocity);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(pos, state.ActualVelocity);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(pos, state.Direction * 2f);
        }
#endif
    }
}

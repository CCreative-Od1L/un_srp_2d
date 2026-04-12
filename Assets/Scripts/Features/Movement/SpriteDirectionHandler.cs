using UnSrp2d.Core.Contracts;
using UnityEngine;
using VContainer;

namespace UnSrp2d.Features.Movement
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteDirectionHandler : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _renderer;

        IMovementStateProvider _stateProvider;

        [Inject]
        public void Construct(IMovementStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        void Awake()
        {
            if (_renderer == null)
                _renderer = GetComponent<SpriteRenderer>();
        }

        void LateUpdate()
        {
            if (_stateProvider == null) return;
            _renderer.flipX = _stateProvider.CurrentState.Direction.x < 0;
        }
    }
}

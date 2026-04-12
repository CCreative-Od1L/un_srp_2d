using UnSrp2d.Core.Contracts;
using UnityEngine;
using VContainer;

namespace UnSrp2d.Features.Movement
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteDirectionHandler : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _renderer;

        [Inject]
        IMovementStateProvider _stateProvider;

        void LateUpdate()
        {
            _renderer.flipX = _stateProvider.CurrentState.Direction.x < 0;
        }
    }
}

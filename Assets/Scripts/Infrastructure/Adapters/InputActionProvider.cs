using UnSrp2d.Core.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnSrp2d.Infrastructure.Adapters
{
    public class InputActionProvider : MonoBehaviour, IInputProvider
    {
        [SerializeField] InputActionAsset _actionAsset;

        InputAction _moveAction;
        Vector2 _cachedDirection;

        public Vector2 MoveDirection => _cachedDirection;

        void Awake()
        {
            var playerMap = _actionAsset.FindActionMap("Player");
            _moveAction = playerMap.FindAction("Move");
        }

        void OnEnable()
        {
            _moveAction?.Enable();
        }

        void OnDisable()
        {
            _moveAction?.Disable();
        }

        void Update()
        {
            if (_moveAction != null)
                _cachedDirection = _moveAction.ReadValue<Vector2>();
        }
    }
}

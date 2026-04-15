using UnSrp2d.Core.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnSrp2d.Infrastructure.Adapters
{
    public class CameraInputProvider : MonoBehaviour, ICameraInput
    {
        [SerializeField] InputActionAsset _actionAsset;

        InputAction _zoomModeAction;
        InputAction _zoomInAction;
        InputAction _zoomOutAction;

        public bool IsZoomModeActive { get; private set; }
        public bool IsZoomInPressed { get; private set; }
        public bool IsZoomOutPressed { get; private set; }

        void Awake()
        {
            var cameraMap = _actionAsset.FindActionMap("Camera");
            _zoomModeAction = cameraMap.FindAction("ZoomMode");
            _zoomInAction = cameraMap.FindAction("ZoomIn");
            _zoomOutAction = cameraMap.FindAction("ZoomOut");
        }

        void OnEnable()
        {
            _zoomModeAction?.Enable();
            _zoomInAction?.Enable();
            _zoomOutAction?.Enable();
        }

        void OnDisable()
        {
            _zoomModeAction?.Disable();
            _zoomInAction?.Disable();
            _zoomOutAction?.Disable();
        }

        void Update()
        {
            IsZoomModeActive = _zoomModeAction?.IsPressed() ?? false;
            IsZoomInPressed = _zoomInAction?.IsPressed() ?? false;
            IsZoomOutPressed = _zoomOutAction?.IsPressed() ?? false;
        }
    }
}
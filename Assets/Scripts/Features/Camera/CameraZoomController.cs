using Cinemachine;
using UnSrp2d.Core.Contracts;
using UnityEngine;
using VContainer;

namespace UnSrp2d.Features.Camera
{
    public class CameraZoomController : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera _virtualCamera;
        [SerializeField] CameraZoomParams _zoomParams;

        ICameraInput _cameraInput;
        float _currentOrthographicSize;

        [Inject]
        public void Construct(ICameraInput cameraInput)
        {
            _cameraInput = cameraInput;
        }

        void Awake()
        {
            _currentOrthographicSize = _zoomParams.DefaultOrthographicSize;
        }

        void Update()
        {
            if (_cameraInput.IsZoomModeActive)
            {
                if (_cameraInput.IsZoomInPressed)
                    _currentOrthographicSize -= _zoomParams.ZoomSpeed * Time.deltaTime;
                if (_cameraInput.IsZoomOutPressed)
                    _currentOrthographicSize += _zoomParams.ZoomSpeed * Time.deltaTime;
            }

            _currentOrthographicSize = Mathf.Clamp(
                _currentOrthographicSize,
                _zoomParams.MinOrthographicSize,
                _zoomParams.MaxOrthographicSize);

            _virtualCamera.m_Lens.OrthographicSize = _currentOrthographicSize;
        }
    }
}
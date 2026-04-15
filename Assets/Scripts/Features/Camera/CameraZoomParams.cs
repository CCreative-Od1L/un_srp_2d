using UnityEngine;

namespace UnSrp2d.Features.Camera
{
    [CreateAssetMenu(fileName = "CameraZoomParams", menuName = "UnSrp2d/Camera/CameraZoomParams")]
    public class CameraZoomParams : ScriptableObject
    {
        public float MinOrthographicSize = 2f;
        public float MaxOrthographicSize = 10f;
        public float ZoomSpeed = 3f;
        public float DefaultOrthographicSize = 5f;
    }
}
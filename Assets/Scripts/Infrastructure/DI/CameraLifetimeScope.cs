using UnSrp2d.Features.Camera;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UnSrp2d.Infrastructure.DI
{
    public class CameraLifetimeScope : LifetimeScope
    {
        [SerializeField] CameraZoomParams _zoomParams;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<CameraZoomParams>(_zoomParams);
            builder.RegisterComponentInHierarchy<CameraZoomController>();
        }
    }
}
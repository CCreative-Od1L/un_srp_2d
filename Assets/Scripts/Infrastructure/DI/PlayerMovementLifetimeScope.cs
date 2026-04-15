using UnSrp2d.Core.Contracts;
using UnSrp2d.Features.Movement;
using UnSrp2d.Infrastructure.Adapters;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UnSrp2d.Infrastructure.DI
{
    public class PlayerMovementLifetimeScope : LifetimeScope
    {
        [SerializeField] InputConfig _inputConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<InputActionProvider>()
                .As<IInputProvider>();

            builder.RegisterComponentInHierarchy<CameraInputProvider>()
                .As<ICameraInput>();

            builder.RegisterInstance<IInputConfig>(_inputConfig);
        }
    }
}

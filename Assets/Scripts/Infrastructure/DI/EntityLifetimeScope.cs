using UnSrp2d.Core.Contracts;
using UnSrp2d.Features.Movement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UnSrp2d.Infrastructure.DI
{
    public class EntityLifetimeScope : LifetimeScope
    {
        [SerializeField] MovementParams _movementParams;

        protected override void Configure(IContainerBuilder builder)
        {
            var paramsProvider = new DefaultMovementParamsProvider(_movementParams);

            builder.RegisterInstance<IMovementParamsProvider>(paramsProvider);

            builder.Register<InputMovementStrategy>(Lifetime.Singleton)
                .As<IMovementStrategy>();

            builder.Register<MovementProcessor>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }
    }
}

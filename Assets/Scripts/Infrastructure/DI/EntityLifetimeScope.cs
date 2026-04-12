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
            // 注册参数提供者
            var paramsProvider = new DefaultMovementParamsProvider(_movementParams);
            builder.RegisterInstance<IMovementParamsProvider>(paramsProvider);

            // 注册策略（从父 Scope 获取 IInputProvider 和 IInputConfig）
            builder.Register<InputMovementStrategy>(Lifetime.Singleton)
                .As<IMovementStrategy>();

            // 注册处理器（同时注册为具体类型和接口）
            builder.Register<MovementProcessor>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces();

            // 注册场景中的 MonoBehaviour 组件（VContainer 会自动注入）
            builder.RegisterComponentInHierarchy<MovementController>();
            builder.RegisterComponentInHierarchy<SpriteDirectionHandler>();
        }
    }
}

namespace UnSrp2d.Core.Contracts
{
    public interface IMovementParamsProvider
    {
        float MaxSpeed { get; }
        float Acceleration { get; }
        float Deceleration { get; }
        float MinMoveSpeed { get; }
    }
}

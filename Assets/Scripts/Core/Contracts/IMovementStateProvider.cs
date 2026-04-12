namespace UnSrp2d.Core.Contracts
{
    public interface IMovementStateProvider
    {
        MovementState CurrentState { get; }
    }
}

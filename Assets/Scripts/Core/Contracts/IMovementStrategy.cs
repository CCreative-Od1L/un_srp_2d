using UnityEngine;

namespace UnSrp2d.Core.Contracts
{
    public interface IMovementStrategy
    {
        MovementInput GetMovementInput();
    }
}

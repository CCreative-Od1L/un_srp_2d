using UnityEngine;

namespace UnSrp2d.Core.Contracts
{
    public interface IInputProvider
    {
        Vector2 MoveDirection { get; }
    }
}

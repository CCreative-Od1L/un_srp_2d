using UnityEngine;

namespace UnSrp2d.Core.Contracts
{
    public interface IMovementStrategy
    {
        Vector2 GetDirection();
    }
}
